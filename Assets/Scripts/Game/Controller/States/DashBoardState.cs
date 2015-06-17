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
		p_gameController.getUI().removeScreen( m_dashboardCommonCanvas );
		p_gameController.getUI().removeScreen( m_leftMenuCanvas );
	}
	
	
	//---------------- Private Implementation ----------------------
	
	private void _setupScreen( UIManager p_uiManager )
	{
	
		m_leftMenuCanvas = p_uiManager.createScreen (UIScreen.LEFT_MENU, true, 3);
		m_dashboardCommonCanvas = p_uiManager.createScreen (UIScreen.DASHBOARD_COMMON, true, 1);

		m_menu = m_leftMenuCanvas.getView ("LeftMenu") as UIElement;
		m_leftSideMenuButton = m_dashboardCommonCanvas.getView ("menuButton") as UIButton;
		m_leftSideMenuButton.addClickCallback (toShowMenu);
		m_showProfileButton = m_menu.getView ("profileButton") as UIButton;
		m_showProfileButton.addClickCallback (toShowAllChilren);
		m_sliderDownPanel = m_menu.getView ("sildeDownPanel") as UIElement;

		m_closeLeftMenuButton = m_leftMenuCanvas.getView ("closeButton") as UIButton;
		m_closeLeftMenuButton.addClickCallback (onCloseMenu);
		m_childModeButton = m_dashboardCommonCanvas.getView ("childModelButton") as UIButton;
		m_childModeButton.addClickCallback (toChildMode);

		m_settingButton = m_leftMenuCanvas.getView ("settingButton") as UIButton;
		m_settingButton.addClickCallback (toSettingScreen);
	}

	private void goToChildLock(UIButton p_button)
	{
		m_game.gameController.changeState (ZoodleState.CHILD_LOCK_STATE);
	}

	private void toSettingScreen(UIButton p_button)
	{
		p_button.removeClickCallback (toSettingScreen);
		m_game.gameController.changeState (ZoodleState.SETTING_STATE);
	}

	private void onCloseMenu(UIButton p_button)
	{
		Vector3 l_position = m_menu.transform.localPosition;
		
		List<Vector3> l_posList = new List<Vector3> ();
		l_posList.Add (l_position);
		l_posList.Add (l_position + new Vector3 (-200, 0, 0));
		m_menu.tweener.addPositionTrack (l_posList, 1.0f, null, Tweener.Style.QuadOutReverse);
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
		Vector3 l_position = m_menu.transform.localPosition;

		List<Vector3> l_posList = new List<Vector3> ();
		l_posList.Add (l_position);
		l_posList.Add (l_position + new Vector3 (200, 0, 0));
		m_menu.tweener.addPositionTrack (l_posList, 1.0f, null, Tweener.Style.QuadOutReverse);
	}

	private void toShowAllChilren(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		if (!isAllChildShow) 
		{
			Vector3 l_position = m_sliderDownPanel.transform.localPosition;
			List<Vector3> l_posList = new List<Vector3> ();
			l_posList.Add (l_position);
			l_posList.Add (l_position + new Vector3 (0, -200, 0));
			m_sliderDownPanel.tweener.addPositionTrack (l_posList, 1.0f, addButtonClickCall, Tweener.Style.QuadOutReverse);
			isAllChildShow= true;
		}
		else
		{
			Vector3 l_position = m_sliderDownPanel.transform.localPosition;
			List<Vector3> l_posList = new List<Vector3> ();
			l_posList.Add (l_position);
			l_posList.Add (l_position + new Vector3 (0, 200, 0));
			m_sliderDownPanel.tweener.addPositionTrack (l_posList, 1.0f, addButtonClickCall, Tweener.Style.QuadOutReverse);
			isAllChildShow= false;
		}
	}

	private void addButtonClickCall( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_showProfileButton.addClickCallback (toShowAllChilren);
	}

	//Private variables
	

	private UIButton 	m_leftSideMenuButton;
	private UIButton 	m_showProfileButton;
	private UIButton	m_closeLeftMenuButton;
	private UIButton    m_childModeButton;
	private UIButton    m_settingButton;


	private UIElement 	m_menu;
	private Game 		m_game;

	private UIElement 	m_sliderDownPanel;

	private bool isAllChildShow = false;

	private UICanvas    		m_dashboardCommonCanvas;
	private UICanvas			m_leftMenuCanvas;

}
