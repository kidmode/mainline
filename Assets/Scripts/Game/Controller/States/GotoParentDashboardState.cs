using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/*
 * 	Transition state used to setup the Dashboard States assumptions from
 *  any other state in the game
 */
public class GotoParentDashboardState: GameState 
{

	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);

		turnOffChildLock();
		wipeScreens(p_gameController);
		wipeWebCalls(p_gameController);
		setupDashboardAssumptions(p_gameController);
		changeToDashboardState(p_gameController);
	}

	private void turnOffChildLock()
	{
//		KidMode.setKidsModeActive(false);	
		KidModeLockController.Instance.swith2DParentMode();
	}
	
	private void wipeScreens(GameController p_gameController)
	{
		UIManager l_ui = p_gameController.getUI();
		l_ui.clearAllLayers();
	}

	private void wipeWebCalls(GameController p_gameController)
	{
		//TODO: Can we access active calls somehow? Do we need to?
	}

	private void setupDashboardAssumptions(GameController p_gameController)
	{
		UIManager l_ui = p_gameController.getUI();
		UICanvas l_backScreen = l_ui.findScreen(UIScreen.SPLASH_BACKGROUND);
		if (l_backScreen == null)
		{
			l_backScreen = l_ui.createScreen(UIScreen.SPLASH_BACKGROUND,true, -1);
			SplashBackCanvas l_splashBack = l_backScreen as SplashBackCanvas;
			l_splashBack.setDown();
		}
	}

	private void changeToDashboardState(GameController p_gameController)
	{
		SessionHandler l_sessionHandler = SessionHandler.getInstance();
		Token l_token = l_sessionHandler.token;

		if (l_token.isPremium())
		{
			if (l_sessionHandler.childLockSwitch)
			{
				int l_pin = l_sessionHandler.pin;
				if (0 != l_pin) 
				{
					p_gameController.changeState(ZoodleState.BIRTHYEAR);
				}
				else
				{
					setErrorMessage(p_gameController,Localization.getString( Localization.TXT_STATE_1_ERROR ),Localization.getString( Localization.TXT_STATE_64_PIN_ERROR ));
				}
			}
			else
			{
				p_gameController.changeState(ZoodleState.OVERVIEW_INFO);
			}
		}
		else
		{
			p_gameController.changeState(ZoodleState.UPSELL_SPLASH);
		}
	}
}
