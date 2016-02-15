using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System; 
using System.Globalization; 
using System.Diagnostics;

public class DashBoardState : GameState 
{
	//Public variables

	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );
		m_game = p_gameController.game;
		_setupScreen( p_gameController.getUI() );
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
	}
	
	
	//---------------- Private Implementation ----------------------
	
	private void _setupScreen( UIManager p_uiManager )
	{
	
		m_leftSideMenuButton = m_dashboardCommonCanvas.getView ("menuButton") as UIButton;
		m_leftSideMenuButton.addClickCallback (toShowMenu);

		m_childModeButton = m_dashboardCommonCanvas.getView ("childModelButton") as UIButton;
		m_childModeButton.addClickCallback (toChildMode);

	}

	private void goToChildLock(UIButton p_button)
	{
		m_game.gameController.changeState (ZoodleState.CHILD_LOCK_STATE);
	}




	private void toChildMode(UIButton p_button)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if (KidMode.isHomeLauncherKidMode ()) {
			
			m_gameController.changeState (ZoodleState.PROFILE_SELECTION);
			
		} else {
			
			KidMode.enablePluginComponent();
			
			KidMode.openLauncherSelector ();
			
		}
		#else
		m_gameController.changeState (ZoodleState.PROFILE_SELECTION);
		#endif
	}

	private void toShowMenu(UIButton p_button)
	{
//		Vector3 l_position = m_menu.transform.localPosition;

//		List<Vector3> l_posList = new List<Vector3> ();
//		l_posList.Add (l_position);
//		l_posList.Add (l_position + new Vector3 (200, 0, 0));
//		m_menu.tweener.addPositionTrack (l_posList, 1.0f, null, Tweener.Style.QuadOutReverse);
	}
	

	//Private variables
	

	private UIButton 	m_leftSideMenuButton;
	private UIButton    m_childModeButton;


	private Game 		m_game;
	
	private bool isAllChildShow = false;

	private UICanvas    		m_dashboardCommonCanvas;

}
