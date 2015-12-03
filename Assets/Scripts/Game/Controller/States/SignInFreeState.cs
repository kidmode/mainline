using UnityEngine;
using System.Collections;

public class SignInFreeState : GameState
{
	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);

		_setupScreen (p_gameController.getUI());

		GAUtil.logScreen("SignInFreeScreen");		
	}

	public override void update (GameController p_gameController, int p_time)
	{
		base.update (p_gameController, p_time);
	}

	public override void exit (GameController p_gameController)
	{
		base.exit (p_gameController);

		p_gameController.getUI ().removeScreenImmediately ( UIScreen.SIGN_IN_FREE );
	}

	private void _setupScreen( UIManager p_uiManager )
	{
		m_signInButtonBackgroundCanvas = p_uiManager.findScreen(UIScreen.SPLASH_BACKGROUND) as SplashBackCanvas;
		if (m_signInButtonBackgroundCanvas == null)
		{
			m_signInButtonBackgroundCanvas = p_uiManager.createScreen(UIScreen.SPLASH_BACKGROUND, true, -1) as SplashBackCanvas;
			m_signInButtonBackgroundCanvas.setDown();
		}

		m_signInFreeCanvas = p_uiManager.createScreen(UIScreen.SIGN_IN_FREE, false, 1);

		m_premiumButton = m_signInFreeCanvas.getView ("upgradeButton") 	as UIButton;
		m_freeButton 	= m_signInFreeCanvas.getView ("cancelButton") 	as UIButton;
//		m_backButton 	= m_signInFreeCanvas.getView ("backButton") 	as UIButton;
		m_premiumButton	.addClickCallback ( onPremiumClick );
		m_freeButton	.addClickCallback ( onFreeClick );
//		m_backButton	.addClickCallback ( onBackClick );
	}

	private void onPremiumClick( UIButton p_button )
	{
		SwrveComponent.Instance.SDK.NamedEvent("GoToPremiumAfterSignIn");
		p_button.removeClickCallback ( onPremiumClick );

		m_gameController.changeState(ZoodleState.SIGN_UP_UPSELL);
	}

	private void onFreeClick( UIButton p_button )
	{
		p_button.removeClickCallback ( onFreeClick );

		if (null != SessionHandler.getInstance().kidList && SessionHandler.getInstance().kidList.Count > 0)
		{
			m_gameController.changeState(ZoodleState.PROFILE_SELECTION);
		}
		else
		{
			m_gameController.connectState(ZoodleState.CREATE_CHILD_NEW,int.Parse(m_gameController.stateName));
			m_gameController.changeState(ZoodleState.CREATE_CHILD_NEW);
		}
	}

	private void onBackClick( UIButton p_button )
	{
		p_button.removeClickCallback ( onBackClick );

		SessionHandler.getInstance().clearUserData (false);
		LocalSetting.find ("User").delete ();
		m_gameController.changeState (ZoodleState.SET_UP_ACCOUNT);
	}

	private SplashBackCanvas m_signInButtonBackgroundCanvas;
	private UICanvas m_signInFreeCanvas;
	private UIButton m_premiumButton;
	private UIButton m_freeButton;
//	private UIButton m_backButton;
}
