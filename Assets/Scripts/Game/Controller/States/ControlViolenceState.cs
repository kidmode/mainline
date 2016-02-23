using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ControlViolenceState : GameState
{
	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);
		
		m_uiManager = m_gameController.getUI();
		m_requestQueue = new RequestQueue();
		_setupScreen( p_gameController );
		_setupElment();

//		TutorialController.Instance.showTutorial(TutorialSequenceName.VIOLENCE_LEVEL);
		if(TutorialController.Instance != null)
			TutorialController.Instance.showNextPage();

	}
	
	public override void update (GameController p_gameController, int p_time)
	{
		base.update (p_gameController, p_time);
	}
	
	public override void exit (GameController p_gameController)
	{
//		checkRequest ();
		
		base.exit (p_gameController);

		m_uiManager.removeScreen( UIScreen.COMMON_DIALOG );

		m_uiManager.removeScreen( UIScreen.VIOLENCE_FILTERS );
//		m_uiManager.removeScreen( UIScreen.PAYWALL );
	}

	private void _setupScreen( GameController p_gameController )
	{
		m_commonDialog 				= m_uiManager.createScreen( UIScreen.COMMON_DIALOG, true, 5 ) 			as CommonDialogCanvas;
		m_commonDialog.setUIManager (p_gameController.getUI());

		if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
		{
//			m_paywallCanvas = m_uiManager.createScreen( UIScreen.PAYWALL, false, 2 );
//			m_upgradeButton = m_paywallCanvas.getView( "upgradeButton" ) as UIButton;
//			m_upgradeButton.addClickCallback( onUpgradeButtonClick );
		}

		m_violenceFiltersCanvas 	= m_uiManager.createScreen( UIScreen.VIOLENCE_FILTERS, true, 1 );

		if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
		{
			m_uiManager.setScreenEnable( UIScreen.VIOLENCE_FILTERS, false );
		}
	}

	private void _setupElment()
	{
		m_helpButton = m_violenceFiltersCanvas.getView ("helpButton") as UIButton;
		m_helpButton.addClickCallback (onHelpButtonClick);

		//New Save Button
		mSaveButton = m_violenceFiltersCanvas.getView ("saveButton") as UIButton;
		mSaveButton.addClickCallback (checkRequest);

		
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_violenceFiltersCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, saveMessageDisplacement, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, 0.5f);

		
		//violence part
		m_levelZeroToggle 	= m_violenceFiltersCanvas.getView( "levelZeroToggle" ) 	as UIToggle;
		m_levelOneToggle 	= m_violenceFiltersCanvas.getView( "levelOneToggle" ) 	as UIToggle;
		m_levelTwoToggle 	= m_violenceFiltersCanvas.getView( "levelTwoToggle" ) 	as UIToggle;
		m_levelThreeToggle 	= m_violenceFiltersCanvas.getView( "levelThreeToggle" ) as UIToggle;
		m_levelFourToggle 	= m_violenceFiltersCanvas.getView( "levelFourToggle" ) 	as UIToggle;
		
		m_levelZeroToggle.addValueChangedCallback ( onViolenceChanged );
		m_levelOneToggle.addValueChangedCallback ( onViolenceChanged );
		m_levelTwoToggle.addValueChangedCallback ( onViolenceChanged );
		m_levelThreeToggle.addValueChangedCallback ( onViolenceChanged );
		m_levelFourToggle.addValueChangedCallback ( onViolenceChanged );

		//set up value changed events
		PDControlValueChanged valueChanged = m_violenceFiltersCanvas.gameObject.transform.parent.gameObject.GetComponent<PDControlValueChanged>();

		valueChanged.setListeners();

	}

	private void checkRequest(UIButton p_button)
	{
		if (checkInternet() == false)
			return;

//		if( m_isValueChanged )
//		{

			m_isValueChanged = false;

			updateViolenceFilters();
//		}

	}

	private void onHelpButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks ();
		m_commonDialog.setOriginalPosition ();
		UIButton l_closeButton = m_commonDialog.getView ("closeMark") as UIButton;
		
		UILabel l_titleLabel = m_commonDialog.getView ("dialogText") as UILabel;
		UILabel l_contentLabel = m_commonDialog.getView ("contentText") as UILabel;
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_58_HELP_TITLE);
		l_contentLabel.text = Localization.getString(Localization.TXT_STATE_58_HELP_CONTENT);

		l_closeButton.addClickCallback (onCloseDialogButtonClick);
	}

	private void onCloseDialogButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		m_commonDialog.setOutPosition ();
		m_helpButton.addClickCallback (onHelpButtonClick);
	}


	private bool checkInternet()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable
		    || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected())
		{
			m_gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
			
			ErrorMessage error = GameObject.FindWithTag("ErrorMessageTag").GetComponent<ErrorMessage>() as ErrorMessage;
			if (error != null)
				error.onClick += onClickExit;

			return false;
		}
		return true;
	}

	private void onClickExit()
	{
		ErrorMessage error = GameObject.FindWithTag("ErrorMessageTag").GetComponent<ErrorMessage>() as ErrorMessage;
		error.onClick -= onClickExit;;
		m_gameController.changeState (ZoodleState.CONTROL_APP);
	}

	


	private void onViolenceChanged( UIToggle p_toggle, bool p_bool )
	{
		m_isValueChanged = true;

		if(onControlValueChanged != null)
			onControlValueChanged(m_isValueChanged);

	}

	private void updateViolenceFilters ()
	{
		Hashtable l_param = new Hashtable ();
		if( m_levelZeroToggle.isOn )
		{
			l_param [ZoodlesConstants.PARAM_MAX_VIOLENCE] = 0;
		}
		if( m_levelOneToggle.isOn )
		{
			l_param [ZoodlesConstants.PARAM_MAX_VIOLENCE] = 1;
		}
		if( m_levelTwoToggle.isOn )
		{
			l_param [ZoodlesConstants.PARAM_MAX_VIOLENCE] = 2;
		}
		if( m_levelThreeToggle.isOn )
		{
			l_param [ZoodlesConstants.PARAM_MAX_VIOLENCE] = 3;
		}
		if( m_levelFourToggle.isOn )
		{
			l_param [ZoodlesConstants.PARAM_MAX_VIOLENCE] = 4;
		}

		m_requestQueue.reset ();
		m_requestQueue.add( new SetSubjectsRequest( l_param , updateViolenceSettingComplete) );
		m_requestQueue.request (RequestType.SEQUENCE);

		m_uiManager.createScreen(UIScreen.LOADING_SPINNER_ELEPHANT, false, 10);

	}

	private void updateViolenceSettingComplete(HttpsWWW p_response)
	{

		UICanvas messageCanvas = m_uiManager.createScreen(UIScreen.PD_MESSAGE, false, 20);

		UIButton messageCloseButton = messageCanvas.getView("quitButton") as UIButton;

		messageCloseButton.addClickCallback(closePDMessage);


		if(p_response.error == null){

			m_uiManager.removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);

		}else{

			m_uiManager.createScreen(UIScreen.ERROR_MESSAGE, false, 20);

		}

		if(onControlValueChanged != null)
			onControlValueChanged(m_isValueChanged);


	}

	private void closePDMessage(UIButton button){

		m_uiManager.removeScreen(UIScreen.PD_MESSAGE);

	}

//	private void onUpgradeButtonClick(UIButton p_button)
//	{
//		SwrveComponent.Instance.SDK.NamedEvent("UpgradeBtnInDashBoard");
//
//		if(string.Empty.Equals(SessionHandler.getInstance().PremiumJson))
//		{
//			Server.init (ZoodlesConstants.getHttpsHost());
//			m_requestQueue.reset ();
//			m_requestQueue.add (new GetPlanDetailsRequest(viewPremiumRequestComplete));
//			m_requestQueue.request ();
//		}
//		else
//		{
//			m_gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName) );
//			m_gameController.changeState( ZoodleState.VIEW_PREMIUM );
//		}
//	}
	
	private void viewPremiumRequestComplete(HttpsWWW p_response)
	{
		Server.init (ZoodlesConstants.getHttpsHost());
		if(null == p_response.error)
		{
			SessionHandler.getInstance ().PremiumJson = p_response.text;
			m_gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName) );
			m_gameController.changeState( ZoodleState.VIEW_PREMIUM );
		}
		else
		{
			setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_11_FAIL),Localization.getString(Localization.TXT_STATE_11_FAIL_DATA));
		}
	}

	private UIManager 		m_uiManager;
	private RequestQueue 	m_requestQueue;
	private bool 			m_isValueChanged = false;


	private UISwipeList 	m_childrenList;


	private UIButton 		m_helpButton;
	private CommonDialogCanvas m_commonDialog;
	
//	private UICanvas 		m_paywallCanvas;
//	private UIButton 		m_upgradeButton;


	//violence part
	private UICanvas 		m_violenceFiltersCanvas;

	private UIToggle 		m_levelZeroToggle;
	private UIToggle 		m_levelOneToggle;
	private UIToggle 		m_levelTwoToggle;
	private UIToggle 		m_levelThreeToggle;
	private UIToggle 		m_levelFourToggle;

	//Kevin - event when control values changed to true 
	public static event Action<bool>	onControlValueChanged;

	private float saveMessageDisplacement = 1092.0f;

	//Kevin - Save Button
	UIButton mSaveButton;

}
