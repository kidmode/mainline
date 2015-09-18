using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CongraturationsState : GameState 
{
	//Public variables

	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );

		//Kev
		UIManager l_ui = p_gameController.getUI();
		SplashBackCanvas splashCanvas = l_ui.findScreen (UIScreen.SPLASH_BACKGROUND) as SplashBackCanvas;
		if(splashCanvas != null)
			splashCanvas.gameObject.SetActive(false);
		//Kev

		
		m_game = p_gameController.game;
		m_canChangeState = true;
		m_time = 0.0f;
		m_isFreeAccount = string.Empty.Equals (SessionHandler.getInstance ().creditCardNum);
		_setupScreen( p_gameController.getUI() );
		p_gameController.game.StartCoroutine( _tweenFillBar( 1.0f, 1.25f ) );
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		m_time += Time.deltaTime;
		if(m_time>= 2.5 && m_canChangeState)
		{
			m_canChangeState = false;
			m_gameController.changeState(ZoodleState.SET_BIRTHYEAR);
		}
		base.update( p_gameController, p_time );
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_congraturationCanvas );
	}

	//---------------- Private Implementation ----------------------
	
	private void _setupScreen( UIManager p_uiManager )
	{	
		m_createAccountBackgroundCanvas = p_uiManager.findScreen( UIScreen.SPLASH_BACKGROUND );
		if( m_createAccountBackgroundCanvas == null )
			m_createAccountBackgroundCanvas = p_uiManager.createScreen( UIScreen.SPLASH_BACKGROUND, true, -1 );
		m_congraturationCanvas = p_uiManager.createScreen( UIScreen.CONGRATURATION, true, 1 );
		m_loadingBarImg     = m_congraturationCanvas.getView( "loadingBarSprite" ) as UIImage;
		m_welcomeText = m_congraturationCanvas.getView( "welcomeText" ) as UILabel;
		m_freeKidModeLogo = m_congraturationCanvas.getView( "freeLogo" ) as UIImage;
		m_premiumKidModeLogo = m_congraturationCanvas.getView( "premiumLogo" ) as UIImage;
		if(m_isFreeAccount && SessionHandler.getInstance().renewalPeriod == 0)
		{
			m_welcomeText.text = Localization.getString (Localization.TXT_104_LABEL_WELCOME);
			m_freeKidModeLogo.active = true;
			m_premiumKidModeLogo.active = false;
		}
		else
		{
			m_welcomeText.text = Localization.getString (Localization.TXT_104_LABEL_WELCOME_PREMIUM);
			m_freeKidModeLogo.active = false;
			m_premiumKidModeLogo.active = true;
		}
		//m_continuedButton = m_congraturationCanvas.getView ("continueButton") as UIButton;
		//m_continuedButton.addClickCallback (onContinue);
	}

	private void goBack( UIButton p_button )
	{
		m_game.gameController.changeState (ZoodleState.PAYMENT);
	}

	private void onContinue( UIButton p_button )
	{
		if(null != SessionHandler.getInstance().kidList && SessionHandler.getInstance().kidList.Count > 0)
			m_game.gameController.changeState (ZoodleState.PROFILE_SELECTION);
		else
			m_game.gameController.changeState (ZoodleState.CREATE_CHILD_NEW);
	}

	private IEnumerator _tweenFillBar( float p_filledAmount, float p_duration )
	{
		float l_time = 0;
		while( l_time < p_duration )
		{
			float l_fillAmount = Mathf.Lerp( 0, p_filledAmount, l_time / p_duration );
			
			m_loadingBarImg.fillAmount = l_fillAmount;
			l_time += Time.deltaTime;
			
			yield return new WaitForEndOfFrame();
		}
		
		m_loadingBarImg.fillAmount = 1.0f;
		
		yield return null;
	}


	//Private variables
	
	private UICanvas    m_congraturationCanvas;

	private UIButton 	m_backButton;
	private UIButton 	m_continuedButton;
	private UIImage 	m_loadingBarImg;
	private UICanvas	m_createAccountBackgroundCanvas;
	private float 		m_time;
	private bool		m_canChangeState;
	private bool		m_isFreeAccount;
	private UILabel 	m_welcomeText;
	private UIImage 	m_freeKidModeLogo;
	private UIImage 	m_premiumKidModeLogo;

	private Game 		m_game;
}
