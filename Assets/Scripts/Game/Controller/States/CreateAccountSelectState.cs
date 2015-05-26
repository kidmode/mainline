using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CreateAccountSelectState : GameState 
{

	enum ScreenChange
	{
		None,
		SetUp,
		PreviousScreen,
		SignInScreen,
		CreatePremiumScreen,
		Back
	}

	
	//Public variables
	
	
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );
		_setupScreen( p_gameController.getUI() );
		if(null != SessionHandler.getInstance().token)
			SessionHandler.getInstance().token.clear();

		GAUtil.logScreen("CreateAccountOrSignInScreen");
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
		if(ScreenChange.None != changeToState)
		{
			switch(changeToState)
			{
				case ScreenChange.SetUp:
					m_gameController.connectState(ZoodleState.SET_UP_ACCOUNT,int.Parse(m_gameController.stateName));
					p_gameController.changeState(ZoodleState.SET_UP_ACCOUNT);
					break;
				case ScreenChange.CreatePremiumScreen:
					m_gameController.connectState(ZoodleState.SET_UP_ACCOUNT,int.Parse(m_gameController.stateName));
					p_gameController.changeState(ZoodleState.SET_UP_ACCOUNT);
					break;
				case ScreenChange.SignInScreen:
					p_gameController.changeState(ZoodleState.SIGN_IN);
					break;
				case ScreenChange.Back:
					p_gameController.changeState(ZoodleState.INITIALIZE_GAME);
					break;
			}

			changeToState = ScreenChange.None;
		}

		if (Application.platform == RuntimePlatform.Android && Input.GetKeyUp(KeyCode.Escape))
			toExitApplication(null);
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		removeListeners();
		p_gameController.getUI().removeScreen( m_createAccountSelectCanvas );
	}
	
	
	//---------------- Private Implementation ----------------------
	
	private void _setupScreen( UIManager p_uiManager )
	{
		m_createAccountBackgroundCanvas = p_uiManager.findScreen( UIScreen.SPLASH_BACKGROUND ) as SplashBackCanvas;
		if( m_createAccountBackgroundCanvas == null )
            m_createAccountBackgroundCanvas = p_uiManager.createScreen(UIScreen.SPLASH_BACKGROUND, true, -1) as SplashBackCanvas;
		
		m_createAccountSelectCanvas = p_uiManager.createScreen( UIScreen.PANEL_MARKETING_SCREEN, true, 1 );
		
		m_startButton = m_createAccountSelectCanvas.getView("startButton") as UIButton;

//		#if UNITY_ANDROID && !UNITY_EDITOR
		if( SessionHandler.getInstance().renewalPeriod > 0 )
		{
			UIElement l_mainPanel = m_createAccountSelectCanvas.getView("mainPanel");
			l_mainPanel.active = false;

			UICanvas l_premiumEligibleCanvas = p_uiManager.createScreen( UIScreen.PREMIUM_ELIGIBLE, false , 2 );
			UIButton l_continueButton = l_premiumEligibleCanvas.getView("continueButton") as UIButton;
			UIButton l_exitButton = l_premiumEligibleCanvas.getView("exitButton") as UIButton;
			l_continueButton.addClickCallback( onContinueClick );
			l_exitButton.addClickCallback( onContinueClick );

			UILabel l_message = l_premiumEligibleCanvas.getView("messageText") as UILabel;

			string l_deviceName = SessionHandler.getInstance().deviceName;
			int l_renewalPeriod = SessionHandler.getInstance().renewalPeriod;

			string l_messageText = string.Format( Localization.getString (Localization.TXT_105_LABEL_CONTENT_NOTICE), l_deviceName, l_renewalPeriod);
			l_message.text = l_messageText;
			m_startButton.addClickCallback (gotoCreatePremiumScreen);
		}
		else
		{
			m_startButton.addClickCallback (gotoSetUpScreen);
		}
//		#else
		
//		m_startButton.addClickCallback (gotoSetUpScreen);
//		#endif

		m_signInButton = m_createAccountSelectCanvas.getView("signInButton") as UIButton;


		m_quitButton = m_createAccountSelectCanvas.getView ("exitButton") as UIButton;
		m_quitButton.addClickCallback (toExitApplication);
		List< Vector3 > l_quitPosList = new List< Vector3 >();
		l_quitPosList.Add( m_quitButton.transform.localPosition + new Vector3(  100, 0, 0 ) );
		l_quitPosList.Add( m_quitButton.transform.localPosition );
		m_quitButton.tweener.addPositionTrack( l_quitPosList, 1.0f );
		m_signInButton.addClickCallback (gotoSignInScreen);
		m_createAccountBackgroundCanvas.setDown ();
	}

	private void removeListeners()
	{
		//m_quitButton.removeAllCallbacks();
		m_startButton.removeAllCallbacks();
		m_signInButton.removeAllCallbacks();
	}

	private void toExitApplication(UIButton p_button)
	{
		KidMode.setKidsModeActive(false);	
		Application.Quit ();
	}

	
	private void toBack(UIButton p_button)
	{
		changeToState = ScreenChange.Back;
        m_createAccountBackgroundCanvas.transitionUp(2.5f);
	}

	private void onContinueClick(UIButton p_button)
	{
		m_gameController.getUI ().removeScreen ( UIScreen.PREMIUM_ELIGIBLE );
		UIElement l_mainPanel = m_createAccountSelectCanvas.getView("mainPanel");
		l_mainPanel.tweener.addAlphaTrack ( 0f, 1.0f, 0.5f);
	}

	private void gotoSetUpScreen(UIButton p_button)
	{
		changeToState = ScreenChange.SetUp;
	}
	private void gotoSignInScreen(UIButton p_button)
	{
		changeToState = ScreenChange.SignInScreen;
	}

	private void gotoCreatePremiumScreen(UIButton p_button)
	{
		changeToState = ScreenChange.CreatePremiumScreen;
	}
	
	//Private variables
	
	private UIButton 	m_startButton;
	private UIButton 	m_signInButton;
	private UIButton 	m_quitButton;
	
	private UICanvas    m_createAccountSelectCanvas;
	private SplashBackCanvas	m_createAccountBackgroundCanvas;
	
	//private bool changeState = false;

	private ScreenChange changeToState = ScreenChange.None;
	
}
