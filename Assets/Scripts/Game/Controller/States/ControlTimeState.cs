using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ControlTimeState : GameState
{
	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);
		
		m_uiManager = m_gameController.getUI();
		m_requestQueue = new RequestQueue();
		_setupScreen( p_gameController );
		_setupElment();

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

		m_uiManager.removeScreen( UIScreen.TIME_LIMITS );
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

		m_timeLimitsCanvas 			= m_uiManager.createScreen( UIScreen.TIME_LIMITS, true, 1 ) 			as TimeLimitsCanvas;

		if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
		{
			m_uiManager.setScreenEnable( UIScreen.TIME_LIMITS, false );
		}
	}

	private void _setupElment()
	{
		m_helpButton = m_timeLimitsCanvas.getView ("helpButton") as UIButton;
		m_helpButton.addClickCallback (onHelpButtonClick);
		
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_timeLimitsCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, mainPanelOffset, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, 0.5f);
		
		//time part
		UIToggleGroup l_weekGroup= m_timeLimitsCanvas.getView( "weekGroup" ) as UIToggleGroup;
		m_weekThirtyMin 		= l_weekGroup.getView( "thirtyMinToggle" )	 as UIToggle;
		m_weekOneHour 			= l_weekGroup.getView( "oneHourToggle" )	 as UIToggle;
		m_weekTwoHours 			= l_weekGroup.getView( "twoHoursToggle" )	 as UIToggle;
		m_weekFourHours 		= l_weekGroup.getView( "fourHoursToggle" )	 as UIToggle;
		m_weekUnlimited 		= l_weekGroup.getView( "unlimitedToggle" )	 as UIToggle;
		
		UIToggleGroup l_weekendGroup= m_timeLimitsCanvas.getView( "weekendGroup" ) as UIToggleGroup;
		m_weekendThirtyMin 		= l_weekendGroup.getView( "thirtyMinToggle" )	 as UIToggle;
		m_weekendOneHour 		= l_weekendGroup.getView( "oneHourToggle" )		 as UIToggle;
		m_weekendTwoHours 		= l_weekendGroup.getView( "twoHoursToggle" )	 as UIToggle;
		m_weekendFourHours 		= l_weekendGroup.getView( "fourHoursToggle" )	 as UIToggle;
		m_weekendUnlimited 		= l_weekendGroup.getView( "unlimitedToggle" )	 as UIToggle;
		
		m_weekThirtyMin.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekOneHour.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekTwoHours.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekFourHours.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekUnlimited.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekendThirtyMin.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekendOneHour.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekendTwoHours.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekendFourHours.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekendUnlimited.addValueChangedCallback ( onTimeLimitsChanged );

		if( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() )
		{
			UIElement l_panel = m_timeLimitsCanvas.getView("panel");
			l_panel.active = false;
			m_requestQueue.reset ();
			m_requestQueue.add (new GetTimeLimitsRequest(_getTimeLimitRequestComplete));
			m_requestQueue.request( RequestType.SEQUENCE );
		}

		//New Save Button
		mSaveButton = m_timeLimitsCanvas.getView ("saveButton") as UIButton;
		mSaveButton.addClickCallback (checkRequest);

	}

	private void checkRequest(UIButton button)
	{
		if (checkInternet() == false)
			return;

		if( m_isValueChanged )
		{

			m_isValueChanged = false;

			updateTimeLimits();

		}
	}

	private void onHelpButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks ();
		m_commonDialog.setOriginalPosition ();
		UIButton l_closeButton = m_commonDialog.getView ("closeMark") as UIButton;
		
		UILabel l_titleLabel = m_commonDialog.getView ("dialogText") as UILabel;
		UILabel l_contentLabel = m_commonDialog.getView ("contentText") as UILabel;
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_57_HELP_TITLE);
		l_contentLabel.text = Localization.getString(Localization.TXT_STATE_57_HELP_CONTENT);

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

	private void onTimeLimitsChanged( UIToggle p_toggle, bool p_bool )
	{

		m_isValueChanged = true;

		if( onControlValueChanged != null){

			onControlValueChanged(m_isValueChanged);

		}

	}

	private void _getTimeLimitRequestComplete(HttpsWWW p_response)
	{
		Hashtable table = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
		m_timeLimitsCanvas.setTimeLimits(table);
		UIElement l_panel = m_timeLimitsCanvas.getView("panel");

		if( null == l_panel )
		{
			return;
		}

		l_panel.active = true;
		m_isValueChanged = false;
	}

	//honda: update TimeLimits to currentkid and save kidinfo to local
	private void updateTimeLimits ()
	{
		Hashtable l_param = new Hashtable ();
		l_param [ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance ().token.getSecret ();

		int weekdayTimeLimits = -1;
		int weekendTimeLimits = -1;
		if( m_weekUnlimited.isOn )
		{
			l_param[ZoodlesConstants.PARAM_WEEKDAY_DISABLED] = "true";
			l_param[ZoodlesConstants.PARAM_WEEKDAY_LIMIT] = "-1";
			weekdayTimeLimits = -1;
		}
		else
		{
			l_param[ZoodlesConstants.PARAM_WEEKDAY_DISABLED] = "false";
			if( m_weekThirtyMin.isOn )
				weekdayTimeLimits = 30;
			else if( m_weekOneHour.isOn )
				weekdayTimeLimits = 60;
			else if( m_weekTwoHours.isOn )
				weekdayTimeLimits = 120;
			else if( m_weekFourHours.isOn )
				weekdayTimeLimits = 240;
			l_param[ZoodlesConstants.PARAM_WEEKDAY_LIMIT] = weekdayTimeLimits.ToString();
		}
		
		if( m_weekendUnlimited.isOn )
		{
			l_param[ZoodlesConstants.PARAM_WEEKEND_DISABLED] = "true";
			l_param[ZoodlesConstants.PARAM_WEEKEND_LIMIT] = "-1";
			weekendTimeLimits = -1;
		}
		else
		{
			l_param[ZoodlesConstants.PARAM_WEEKEND_DISABLED] = "false";
			if( m_weekendThirtyMin.isOn )
				weekendTimeLimits = 30;
			if( m_weekendOneHour.isOn )
				weekendTimeLimits = 60;
			if( m_weekendTwoHours.isOn )
				weekendTimeLimits = 120;
			if( m_weekendFourHours.isOn )
				weekendTimeLimits = 240;
			l_param[ZoodlesConstants.PARAM_WEEKEND_LIMIT] = weekendTimeLimits.ToString();
		}
		SessionHandler.getInstance().currentKid.updateTimeLimitsInfo(weekdayTimeLimits, weekendTimeLimits);

		foreach(DictionaryEntry item in l_param)
		{
			Debug.Log("time limit param: " + item.Key + "->" + item.Value);
		}

		m_requestQueue.reset ();
		m_requestQueue.add ( new SetTimeLimitsRequest(l_param, _setTimeLimitRequestComplete));
		m_requestQueue.request (RequestType.SEQUENCE);

		m_uiManager.createScreen(UIScreen.LOADING_SPINNER_ELEPHANT, false, 10);

	}

	private void _setTimeLimitRequestComplete(HttpsWWW p_response)
	{

		UICanvas messageCanvas = m_uiManager.createScreen(UIScreen.PD_MESSAGE, false, 20);
		
		UIButton messageCloseButton = messageCanvas.getView("quitButton") as UIButton;
		
		messageCloseButton.addClickCallback(closePDMessage);

		if (p_response.error == null)
		{

			Debug.Log(p_response.text);

			m_uiManager.removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);

		}
		else
		{

			Debug.Log(p_response.error);

			m_uiManager.createScreen(UIScreen.ERROR_MESSAGE, false, 20);

		}


		if( onControlValueChanged != null){
			
			onControlValueChanged(m_isValueChanged);
			
		}

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
	
	private UIButton 		m_leftButton;
	private UIButton 		m_rightButton;

	private UIButton 		m_helpButton;
	private CommonDialogCanvas m_commonDialog;
	
//	private UICanvas 		m_paywallCanvas;
//	private UIButton 		m_upgradeButton;

	// time part
	private TimeLimitsCanvas m_timeLimitsCanvas;

	private UIToggle m_weekThirtyMin;
	private UIToggle m_weekOneHour;
	private UIToggle m_weekTwoHours;
	private UIToggle m_weekFourHours;
	private UIToggle m_weekUnlimited;
	
	private UIToggle m_weekendThirtyMin;
	private UIToggle m_weekendOneHour;
	private UIToggle m_weekendTwoHours;
	private UIToggle m_weekendFourHours;
	private UIToggle m_weekendUnlimited;


	//Kevin
	private float mainPanelOffset = 1124.25f;

	//Kevin - event when control values changed to true 
	public static event Action<bool>	onControlValueChanged;

	//Kevin - Save Button
	UIButton mSaveButton;

}
