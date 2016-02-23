using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ControlSubjectState : GameState
{
	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);

		//Kevin
		HasAnySliderChanged = false;
		//End

		m_uiManager = m_gameController.getUI();

		//make sure the menu bar is visible whenever we enter the state
		game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();

		m_requestQueue = new RequestQueue();
		_setupScreen( p_gameController );
		_setupElment();

		if(TutorialController.Instance != null)
			TutorialController.Instance.showNextPage();
		SwrveComponent.Instance.SDK.NamedEvent("Parent_Dashboard.controls");
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

		m_uiManager.removeScreen( UIScreen.PROMOTE_SUBJECTS );
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

		m_promoteSubjectsCanvas 	= m_uiManager.createScreen( UIScreen.PROMOTE_SUBJECTS, true, 1 );

//		if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
//		{
//			m_uiManager.setScreenEnable( UIScreen.PROMOTE_SUBJECTS, false );
//		}
	}

	private void _setupElment()
	{
		m_helpButton = m_promoteSubjectsCanvas.getView ("helpButton") as UIButton;
		m_helpButton.addClickCallback (onHelpButtonClick);

		saveButton = m_promoteSubjectsCanvas.getView ("saveButton") as UIButton; 
		saveButton.addClickCallback (onSaveButtonClick);

		goPremiumButton = m_promoteSubjectsCanvas.getView ("goPremiumButton") as UIButton; 

		int l_listCount = 4;

		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_promoteSubjectsCanvas.getView ("mainPanel");
//		l_pointListIn.Add( l_newPanel.transform.localPosition );
//		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 830, 0 ));
//		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, 0.5f);


		//promote subjects part
		m_mathSlider 		= m_promoteSubjectsCanvas.getView( "mathSlider" ) 		as UISlider;
		m_readingSlider 	= m_promoteSubjectsCanvas.getView( "readingSlider" ) 	as UISlider;
		m_scienceSlider 	= m_promoteSubjectsCanvas.getView( "scienceSlider" ) 	as UISlider;
		m_socialSlider 		= m_promoteSubjectsCanvas.getView( "socialSlider" ) 	as UISlider;
		m_cognitiveSlider 	= m_promoteSubjectsCanvas.getView( "cognitiveSlider" ) 	as UISlider;
		m_creativeSlider 	= m_promoteSubjectsCanvas.getView( "creativeSlider" ) 	as UISlider;
		m_lifeSkillsSlider 	= m_promoteSubjectsCanvas.getView( "lifeSkillsSlider" ) as UISlider;
		
		m_mathSlider.addValueChangedCallback( onSliderValueChanged );
		m_readingSlider.addValueChangedCallback( onSliderValueChanged );
		m_scienceSlider.addValueChangedCallback( onSliderValueChanged );
		m_socialSlider.addValueChangedCallback( onSliderValueChanged );
		m_cognitiveSlider.addValueChangedCallback( onSliderValueChanged );
		m_creativeSlider.addValueChangedCallback( onSliderValueChanged );
		m_lifeSkillsSlider.addValueChangedCallback( onSliderValueChanged );

		//Kevin
		//Disable Save buttons
		saveButton.enabled = false;

		goPremiumButton.enabled = false;
		//End
	}

	private void checkRequest()
	{
		if (checkInternet() == false)
			return;

		if( m_isValueChanged )
		{
			m_isValueChanged = false;
			updateSubjects();
		}
	}

	private void onSaveButtonClick(UIButton p_button){

		Debug.Log("  onSaveButtonClick ");

		checkRequest();

	}

	private void onHelpButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks ();
		m_commonDialog.setOriginalPosition ();
		UIButton l_closeButton = m_commonDialog.getView ("closeMark") as UIButton;
		
		UILabel l_titleLabel = m_commonDialog.getView ("dialogText") as UILabel;
		UILabel l_contentLabel = m_commonDialog.getView ("contentText") as UILabel;
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_55_HELP_TITLE);
		l_contentLabel.text = Localization.getString(Localization.TXT_STATE_55_HELP_CONTENT);

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


	private void onSliderValueChanged( float p_float )
	{
		m_isValueChanged = true;

		HasAnySliderChanged = true;

	}



	private void updateSubjects ()
	{
		SwrveComponent.Instance.SDK.NamedEvent("Parent_Dashboard.AdjustSubjects");

		Hashtable l_param = new Hashtable ();
		l_param [ZoodlesConstants.PARAM_WEIGHT_MATH] 					= m_mathSlider.value.ToString();
		l_param [ZoodlesConstants.PARAM_WEIGHT_READING] 				= m_readingSlider.value.ToString();
		l_param [ZoodlesConstants.PARAM_WEIGHT_SCIENCE] 				= m_scienceSlider.value.ToString();
		l_param [ZoodlesConstants.PARAM_WEIGHT_SOCIAL_STUDIES] 			= m_socialSlider.value.ToString();
		l_param [ZoodlesConstants.PARAM_WEIGHT_COGNITIVE_DEVELOPMENT] 	= m_cognitiveSlider.value.ToString();
		l_param [ZoodlesConstants.PARAM_WEIGHT_CREATIVE_DEVELOPMENT] 	= m_creativeSlider.value.ToString();
		l_param [ZoodlesConstants.PARAM_WEIGHT_LIFE_SKILLS] 			= m_lifeSkillsSlider.value.ToString();

		SessionHandler.getInstance ().currentKid.weightMath 				= Mathf.CeilToInt (m_mathSlider.value);
		SessionHandler.getInstance ().currentKid.weightReading 				= Mathf.CeilToInt (m_readingSlider.value);
		SessionHandler.getInstance ().currentKid.weightScience 				= Mathf.CeilToInt (m_scienceSlider.value);
		SessionHandler.getInstance ().currentKid.weightSocialStudies 		= Mathf.CeilToInt (m_socialSlider.value);
		SessionHandler.getInstance ().currentKid.weightCognitiveDevelopment = Mathf.CeilToInt (m_cognitiveSlider.value);
		SessionHandler.getInstance ().currentKid.weightCreativeDevelopment 	= Mathf.CeilToInt (m_creativeSlider.value);
		SessionHandler.getInstance ().currentKid.weightLifeSkills 			= Mathf.CeilToInt (m_lifeSkillsSlider.value);

		m_requestQueue.reset ();
		m_requestQueue.add( new SetSubjectsRequest( l_param, updateSubjectComplete ) );
		m_requestQueue.request (RequestType.RUSH);

		m_uiManager.createScreen(UIScreen.LOADING_SPINNER_ELEPHANT, false, 10);

	}

	private void updateSubjectComplete(HttpsWWW p_response)
	{

		UICanvas messageCanvas = m_uiManager.createScreen(UIScreen.PD_MESSAGE, false, 20);
		
		UIButton messageCloseButton = messageCanvas.getView("quitButton") as UIButton;
		
		messageCloseButton.addClickCallback(closePDMessage);
		
		
		if(p_response.error == null){
			
			m_uiManager.removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);
			
		}else{
			
			m_uiManager.createScreen(UIScreen.ERROR_MESSAGE, false, 20);
			
		}

		saveButton.enabled = false;

	}

	private void closePDMessage(UIButton button){
		
		m_uiManager.removeScreen(UIScreen.PD_MESSAGE);
		
	}

	#region properties
	//Kevin
	// use this so there is only one place to change it and we know when
	bool HasAnySliderChanged {
		get {
			return this.hasAnySliderChanged;
		}
		set {

			hasAnySliderChanged = value;

			if(hasAnySliderChanged){

				saveButton.enabled = true;

				goPremiumButton.enabled = true;

			}

		}
	}
	#endregion


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

	private UIButton 		m_helpButton;
	private UIButton		saveButton;
	private CommonDialogCanvas m_commonDialog;
	
	private UICanvas 		m_paywallCanvas;
	private UIButton 		m_upgradeButton;

	//subjects part
	private UICanvas m_promoteSubjectsCanvas;

	private UISlider 	m_mathSlider;
	private UISlider 	m_readingSlider;
	private UISlider 	m_scienceSlider;
	private UISlider 	m_socialSlider;
	private UISlider 	m_cognitiveSlider;
	private UISlider 	m_creativeSlider;
	private UISlider 	m_lifeSkillsSlider;

	//Kevin
	private Game		game;

	private bool hasAnySliderChanged = false;

	private UIButton	goPremiumButton; //used for free user
	//End

}
