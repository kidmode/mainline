using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CreateAccountSelectState : GameState 
{

	enum ScreenChange
	{
		None,
		SetUpAccountScreen,
		PreviousScreen,
		SignInScreen,
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
				case ScreenChange.SetUpAccountScreen:
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
		
		m_createAccountSelectCanvas = p_uiManager.createScreen( UIScreen.CREATE_ACCOUNT_SELECTION, true, 1 );
		m_panel = m_createAccountSelectCanvas.getView("panel") as UIElement;
		m_panel.active = false;
		m_title = m_createAccountSelectCanvas.getView("topicText") as UILabel;
		m_title.active = false;

		m_signUpButton = m_createAccountSelectCanvas.getView("signUpButton") as UIButton;

		m_signInButton = m_createAccountSelectCanvas.getView("signInButton") as UIButton;

		m_title.tweener.addAlphaTrack (0.0f, 1.0f, 0.5f,onTitleTweenFinish);

		m_quitButton = m_createAccountSelectCanvas.getView ("exitButton") as UIButton;
		m_quitButton.addClickCallback (toExitApplication);
		List< Vector3 > l_quitPosList = new List< Vector3 >();
		l_quitPosList.Add( m_quitButton.transform.localPosition + new Vector3(  100, 0, 0 ) );
		l_quitPosList.Add( m_quitButton.transform.localPosition );
		m_quitButton.tweener.addPositionTrack( l_quitPosList, 1.0f );

		m_createAccountBackgroundCanvas.setDown ();
	}

	private void addListeners( UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		//m_quitButton.addClickCallback (toBack);
		m_signUpButton.addClickCallback (gotoSignUpScreen);
		m_signInButton.addClickCallback (gotoSignInScreen);
	}

	private void removeListeners()
	{
		//m_quitButton.removeAllCallbacks();
		m_signUpButton.removeAllCallbacks();
		m_signInButton.removeAllCallbacks();
	}

	private void toExitApplication(UIButton p_button)
	{
		Application.Quit ();
	}
	
	private void onTitleTweenFinish( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_panel.tweener.addAlphaTrack (0.0f, 1.0f, 0.5f, addListeners);
	}
	
	private void toBack(UIButton p_button)
	{
		changeToState = ScreenChange.Back;
        m_createAccountBackgroundCanvas.transitionUp(2.5f);
	}

	private void reload(UIButton p_button)
	{
		//changeState = true;
	}

	private void gotoSignUpScreen(UIButton p_button)
	{
		changeToState = ScreenChange.SetUpAccountScreen;
	}
	private void gotoSignInScreen(UIButton p_button)
	{
		changeToState = ScreenChange.SignInScreen;
	}
	
	//Private variables
	
	private UILabel		m_title;
	
	private UIButton 	m_signUpButton;
	private UIButton 	m_connectButton;
	private UIButton 	m_signInButton;
	private UIButton 	m_quitButton;
	private UIElement 	m_panel;
	
	private UICanvas    m_createAccountSelectCanvas;
	private SplashBackCanvas	m_createAccountBackgroundCanvas;
	
	//private bool changeState = false;

	private ScreenChange changeToState = ScreenChange.None;
	
}
