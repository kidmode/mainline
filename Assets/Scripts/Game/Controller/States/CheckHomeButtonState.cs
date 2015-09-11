using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class CheckHomeButtonState: GameState 
{
	private Game game;

	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );

		game = p_gameController.game;
		KidMode.broadcastCurrentMode("ParentMode");
	}

	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );	

		if (game.IsFirstLaunch == 0) 
		{
			p_gameController.changeState(ZoodleState.ZOODLES_ANIMATION);
		} 
		else 
		{
			p_gameController.changeState(ZoodleState.LOADING_PAGE);
		}

//#if UNITY_EDITOR
//		p_gameController.changeState(ZoodleState.ZOODLES_ANIMATION);
//#else
//		if(KidMode.hasHomeButton())
//		{
//			KidMode.disableHomeButton();
//			p_gameController.changeState(ZoodleState.ZOODLES_ANIMATION);
//		}
//		else if (KidMode.hasAvailableHomeButton())
//		{
//			p_gameController.changeState(ZoodleState.REQUEST_HOME_STATE);
//		}
//		else
//		{
//			p_gameController.changeState(ZoodleState.RESET_HOME_STATE);
//		}
//#endif
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );		
	}
	

}
