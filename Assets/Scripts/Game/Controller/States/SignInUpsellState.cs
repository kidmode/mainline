using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SignInUpsellState : GameState 
{
	//Public variables
	
	
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );

		_setupScreen( p_gameController.getUI() );

	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_signinUpsellCanvas );
	}

	//---------------- Private Implementation ----------------------
	
	private void _setupScreen( UIManager p_uiManager )
	{	
		m_backScreen = p_uiManager.findScreen( UIScreen.SPLASH_BACKGROUND );
		if (m_backScreen == null)
		{
			m_backScreen = p_uiManager.createScreen(UIScreen.SPLASH_BACKGROUND,true, -1);
			(m_backScreen as SplashBackCanvas).setDown();
		}
		m_signinUpsellCanvas = p_uiManager.createScreen( UIScreen.SIGN_IN_UPSELL, false, 1 );

		m_iconArea = m_signinUpsellCanvas.getView ("iconArea") as UIElement;
		m_listArea = m_signinUpsellCanvas.getView ("listArea") as UIElement;
		m_buttonArea = m_signinUpsellCanvas.getView ("ButtonArea") as UIElement;

		m_iconArea.active = false;
		m_listArea.active = false;
		m_buttonArea.active = false;

		m_getTrialButton = m_buttonArea.getView ("tryFreeButton") as UIButton;
		m_cancelButton = m_buttonArea.getView ("cancelButton") as UIButton;

		m_getTrialButton.addClickCallback (toTryFree);
		m_cancelButton.addClickCallback (toBack);

		m_iconArea.tweener.addAlphaTrack (0.0f, 1.0f, 0.5f,onIconTweenFinish);
	}

	private void onIconTweenFinish( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_listArea.tweener.addAlphaTrack (0.0f, 1.0f, 0.5f,onCTAsTweenFinish);
	}

	private void onCTAsTweenFinish( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_buttonArea.tweener.addAlphaTrack (0.0f, 1.0f, 0.5f);
	}

	private void toTryFree(UIButton p_button)
	{
		m_gameController.connectState ( ZoodleState.SIGN_UP_UPSELL, ZoodleState.SIGN_IN_UPSELL );
		m_gameController.changeState (ZoodleState.SIGN_UP_UPSELL);
	}

	private void toBack(UIButton p_button)
	{
		int l_state = m_gameController.getConnectedState( ZoodleState.SIGN_IN_UPSELL );
		if (l_state == ZoodleState.PROFILE_VIEW)
		{
			m_gameController.changeState(ZoodleState.PROFILE_SELECTION);
		}
		else
		{
			m_gameController.changeState(l_state);
		}
	}

	//Private variables
	
	private UICanvas    m_signinUpsellCanvas;
	private UICanvas	m_backScreen;

	private UIButton 	m_getTrialButton;
	private UIButton 	m_cancelButton;
	
	private UIElement 	m_iconArea;
	private UIElement	m_listArea;
	private UIElement 	m_buttonArea;
}