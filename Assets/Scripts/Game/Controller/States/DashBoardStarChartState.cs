using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DashBoardStarChartState : GameState
{
	//--------------------Public Interface -----------------------
	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);
		
		m_uiManager = m_gameController.getUI();
		m_requestQueue = new RequestQueue();

		m_currentPage = 1;
		m_currentSubjectId = 0;
		m_isToRequest = true;

		_setupScreen( p_gameController );
		_setupElment();
	}

	public override void update (GameController p_gameController, int p_time)
	{
		base.update (p_gameController, p_time);
	}
	
	public override void exit (GameController p_gameController)
	{
		base.exit (p_gameController);

		if( m_upgradeScreen != null )
		{
			m_uiManager.removeScreen( UIScreen.UPGRADE );
		}
		m_uiManager.removeScreen( UIScreen.DASHBOARD_COMMON );
		m_uiManager.removeScreen( UIScreen.LEFT_MENU );
		m_uiManager.removeScreen( UIScreen.STAR_CHART );
		m_uiManager.removeScreen( UIScreen.COMMON_DIALOG );
	}
	
	//----------------- Private Implementation -------------------
	
	private void _setupScreen( GameController p_gameController )
	{
		m_upgradeScreen 		= m_uiManager.createScreen ( UIScreen.UPGRADE, false, 4 );
		m_commonDialog 			= m_uiManager.createScreen( UIScreen.COMMON_DIALOG, false, 3 ) as CommonDialogCanvas;
		m_commonDialog			.setUIManager (p_gameController.getUI());
		m_leftMenuCanvas 		= m_uiManager.createScreen( UIScreen.LEFT_MENU, true, 2 ) as LeftMenuCanvas;
		m_starChartCanvas 		= m_uiManager.createScreen( UIScreen.STAR_CHART, false, 1 ) as StarChartCanvas;
		m_dashboardCommonCanvas = m_uiManager.createScreen( UIScreen.DASHBOARD_COMMON, true, 0 );

		m_helpButton 			= m_starChartCanvas.getView ("helpButton") as UIButton;
		m_tryPremiumButton 		= m_leftMenuCanvas.getView ("premiumButton") as UIButton;
		m_buyGemsButton 		= m_leftMenuCanvas.getView ("buyGemsButton") as UIButton;
		m_helpButton			.addClickCallback (onHelpButtonClick);
		m_tryPremiumButton		.addClickCallback (toPremiumScreen);
		m_buyGemsButton			.addClickCallback (toBuyGemsScreen);

		m_exitUpgradeButton 	= m_upgradeScreen.getView ("exitButton") 	as UIButton;
		m_upgradeButton 		= m_upgradeScreen.getView ("upgradeButton") as UIButton;
		m_exitUpgradeButton		.addClickCallback (onExitUpgradeButtonClick);
		m_upgradeButton			.addClickCallback (onUpgradeButtonClick);

		m_uiManager				.changeScreen ( UIScreen.UPGRADE, true );
	}

	private void onExitUpgradeButtonClick(UIButton p_button)
	{
		m_gameController.changeState (ZoodleState.OVERVIEW_INFO);
	}

	private void onUpgradeButtonClick(UIButton p_button)
	{
		SwrveComponent.Instance.SDK.NamedEvent("UpgradeBtnInDashBoard");

		if(string.Empty.Equals(SessionHandler.getInstance().PremiumJson))
		{
			Server.init (ZoodlesConstants.getHttpsHost());
			m_requestQueue.reset ();
			m_requestQueue.add (new GetPlanDetailsRequest(viewPremiumRequestComplete));
			m_requestQueue.request ();
		}
		else
		{
			m_gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName) );
			m_gameController.changeState( ZoodleState.VIEW_PREMIUM );
		}
	}

	private void viewPremiumRequestComplete(WWW p_response)
	{
		Server.init (ZoodlesConstants.getHost());
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

	private void onHelpButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks ();
		m_commonDialog.setOriginalPosition ();
		UIButton l_closeButton = m_commonDialog.getView ("closeMark") as UIButton;
		
		UILabel l_titleLabel = m_commonDialog.getView ("dialogText") as UILabel;
		UILabel l_contentLabel = m_commonDialog.getView ("contentText") as UILabel;
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_43_HELP_TITLE);
		l_contentLabel.text = Localization.getString(Localization.TXT_STATE_43_HELP_CONTENT);

		l_closeButton.addClickCallback (onCloseDialogButtonClick);
	}
	
	private void onCloseDialogButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		m_commonDialog.setOutPosition ();
		m_helpButton.addClickCallback (onHelpButtonClick);
	}

	private void toPremiumScreen(UIButton p_button)
	{
		if (LocalSetting.find("User").getBool("UserTry",true))
		{
			if(!SessionHandler.getInstance().token.isCurrent())
			{
				m_gameController.connectState (ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName));
				m_gameController.changeState (ZoodleState.VIEW_PREMIUM);	
			}
		}
		else
		{
			m_gameController.connectState (ZoodleState.SIGN_IN_UPSELL, int.Parse(m_gameController.stateName));
			m_gameController.changeState (ZoodleState.SIGN_IN_UPSELL);
		}
	}
	
	private void toBuyGemsScreen(UIButton p_button)
	{
		//m_game.gameController.changeState(ZoodleState.BUY_GEMS);
		gotoGetGems ();
	}
	
	private void gotoGetGems()
	{	
		string l_returnJson = SessionHandler.getInstance ().GemsJson;
		
		if(l_returnJson.Length > 0)
		{
			Hashtable l_date = MiniJSON.MiniJSON.jsonDecode (l_returnJson) as Hashtable;
			if(l_date.ContainsKey("jsonResponse"))
			{
				m_gameController.connectState( ZoodleState.BUY_GEMS, int.Parse(m_gameController.stateName) );
				m_gameController.changeState (ZoodleState.BUY_GEMS);
			}
			else
			{
				//sendCall (m_game.gameController,null,"/api/gems_amount/gems_amount?" + ZoodlesConstants.PARAM_TOKEN + "=" + SessionHandler.getInstance ().token.getSecret (),CallMethod.GET,ZoodleState.BUY_GEMS);
				Server.init (ZoodlesConstants.getHttpsHost());
				m_requestQueue.reset ();
				m_requestQueue.add (new ViewGemsRequest(viewGemsRequestComplete));
				m_requestQueue.request ();
			}
		}
		else
		{
			Server.init (ZoodlesConstants.getHttpsHost());
			m_requestQueue.reset ();
			m_requestQueue.add (new ViewGemsRequest(viewGemsRequestComplete));
			m_requestQueue.request ();
		}
	}

	private void viewGemsRequestComplete(WWW p_response)
	{
		Server.init (ZoodlesConstants.getHost());
		if(p_response.error == null)
		{
			SessionHandler.getInstance ().GemsJson = p_response.text;
			m_gameController.connectState( ZoodleState.BUY_GEMS, int.Parse(m_gameController.stateName) );
			m_gameController.changeState (ZoodleState.BUY_GEMS);
		}
		else
		{
			setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_11_FAIL),Localization.getString(Localization.TXT_STATE_11_FAIL_DATA));
		}
	}

	private void _setupElment()
	{
//		m_starChartCanvas.tweener.addAlphaTrack( 0, 1.0f, 0.5f );
		m_starChartCanvas		.active = false;

		List<Vector3> l_pointListTop = new List<Vector3>();
		UIElement l_topPanel = m_dashboardCommonCanvas.getView ("topPanel") as UIElement;
		l_pointListTop.Add( l_topPanel.transform.localPosition + new Vector3( 0, 100, 0 ));
		l_pointListTop.Add( l_topPanel.transform.localPosition );
		l_topPanel.tweener.addPositionTrack( l_pointListTop, 0.5f );

		m_menu = m_leftMenuCanvas.getView ("LeftMenu") as UIElement;
		m_leftSideMenuButton = m_dashboardCommonCanvas.getView ("menuButton") as UIButton;
		m_leftSideMenuButton.addClickCallback (toShowMenu);
		m_showProfileButton = m_menu.getView ("profileButton") as UIButton;
		m_showProfileButton.addClickCallback (toShowAllChilren);
		
		m_closeLeftMenuButton = m_leftMenuCanvas.getView ("closeButton") as UIButton;
		m_closeLeftMenuButton.addClickCallback (onCloseMenu);
		m_childModeButton = m_dashboardCommonCanvas.getView ("childModelButton") as UIButton;
		m_childModeButton.addClickCallback (toChildMode);
		
		m_settingButton = m_leftMenuCanvas.getView ("settingButton") as UIButton;
		m_settingButton.addClickCallback (toSettingScreen);
		
		m_overviewButton = m_dashboardCommonCanvas.getView ("overviewButton") as UIButton;
		m_overviewButton.addClickCallback (goToOverview);
		
		m_statChartButton = m_dashboardCommonCanvas.getView ("starButton") as UIButton;
		m_statChartButton.enabled = false;
		
		m_controlsButton = m_dashboardCommonCanvas.getView ("controlButton") as UIButton;
		m_controlsButton.addClickCallback (goToControls);

		m_childrenList = m_leftMenuCanvas.getView ("childSwipeList") as UISwipeList;
		m_childrenList.addClickListener ("Prototype",onSelectThisChild);

		m_mathToggle = 			m_starChartCanvas.getView( "mathToggle" ) 		as UIToggle;
		m_readingToggle = 		m_starChartCanvas.getView( "readingToggle" ) 	as UIToggle;
		m_scienceToggle = 		m_starChartCanvas.getView( "scienceToggle" ) 	as UIToggle;
		m_socialToggle = 		m_starChartCanvas.getView( "socialToggle" ) 	as UIToggle;
		m_cognitiveToggle = 	m_starChartCanvas.getView( "cognitiveToggle" ) 	as UIToggle;
		m_creativeToggle = 		m_starChartCanvas.getView( "creativeToggle" ) 	as UIToggle;
		m_lifeSkillsToggle = 	m_starChartCanvas.getView( "lifeSkillsToggle" ) as UIToggle;

		m_mathToggle.			addValueChangedCallback (onMathNavClicked);
		m_readingToggle.		addValueChangedCallback (onReadingNavClicked);
		m_scienceToggle.		addValueChangedCallback (onScienceNavClicked);
		m_socialToggle.			addValueChangedCallback (onSocialNavClicked);
		m_cognitiveToggle.		addValueChangedCallback (onCognitiveNavClicked);
		m_creativeToggle.		addValueChangedCallback (onCreativeNavClicked);
		m_lifeSkillsToggle.		addValueChangedCallback (onLifeSkillsNavClicked);

		m_swipeList = 			m_starChartCanvas.getView( "starSwipeList" ) as UISwipeList;

		if( SessionHandler.getInstance().token.isPremium()  || SessionHandler.getInstance().token.isCurrent() )
		{
			if( m_upgradeScreen != null )
			{
				m_uiManager.changeScreen( UIScreen.UPGRADE, false );
				m_uiManager.removeScreen( UIScreen.UPGRADE );
			}
			m_starChartCanvas.active = true;

			string l_mappingJson = LocalSetting.find("ServerSetting").getString( ZoodlesConstants.SUBJECTS,"" );
			
			DebugUtils.Assert( l_mappingJson != null );
			
			Hashtable l_jsonTable = MiniJSON.MiniJSON.jsonDecode( l_mappingJson ) as Hashtable;
			Hashtable l_jsonResponse = l_jsonTable["jsonResponse"] as Hashtable;
			ArrayList l_response = l_jsonResponse["response"] as ArrayList;
			foreach( Hashtable l_table in l_response)
			{
				switch( l_table["en_name"].ToString() )
				{
				case "Math":
					m_mathId = int.Parse( l_table["id"].ToString() );
					break;
				case "Reading":
					m_readingId = int.Parse( l_table["id"].ToString() );
					break;
				case "Science":
					m_scienceId = int.Parse( l_table["id"].ToString() );
					break;
				case "Social Studies":
					m_socialId = int.Parse( l_table["id"].ToString() );
					break;
				case "Cognitive Development":
					m_cognitiveId = int.Parse( l_table["id"].ToString() );
					break;
				case "Creative Development":
					m_creativeId = int.Parse( l_table["id"].ToString() );
					break;
				case "Life Skills":
					m_lifeSkillsId = int.Parse( l_table["id"].ToString() );
					break;
				}
			}
	
			m_mathToggle.isOn = true;
		}
	}

	private void onSelectThisChild(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		Kid l_kid = p_data as Kid;
		if (Localization.getString(Localization.TXT_86_BUTTON_ADD_CHILD).Equals (l_kid.name))
		{
			SessionHandler.getInstance().CreateChild = true;
			m_gameController.connectState(ZoodleState.CREATE_CHILD_NEW,int.Parse(m_gameController.stateName));
			m_gameController.changeState (ZoodleState.CREATE_CHILD_NEW);
		}
		else
		{
			List<Kid> l_kidList = SessionHandler.getInstance().kidList;
			SessionHandler.getInstance().currentKid = l_kidList[p_index-1];
			m_gameController.changeState(ZoodleState.OVERVIEW_INFO);
		}
	}

	private void goToControls( UIButton p_button )
	{
		m_gameController.changeState (ZoodleState.CONTROL_APP);
	}

	private void goToOverview( UIButton p_button )
	{
		m_gameController.changeState (ZoodleState.OVERVIEW_INFO);
	}
	
	private void goToChildLock(UIButton p_button)
	{
		m_gameController.changeState (ZoodleState.CHILD_LOCK_STATE);
	}
	
	private void toSettingScreen(UIButton p_button)
	{
		p_button.removeClickCallback (toSettingScreen);
		m_gameController.changeState (ZoodleState.SETTING_STATE);
	}
	
	private void onCloseMenu(UIButton p_button)
	{
		if(canMoveLeftMenu)
		{
			m_uiManager.changeScreen(UIScreen.LEFT_MENU,false);
			Vector3 l_position = m_menu.transform.localPosition;
			
			List<Vector3> l_posList = new List<Vector3> ();
			l_posList.Add (l_position);
			l_posList.Add (l_position + new Vector3 (-200, 0, 0));
			m_menu.tweener.addPositionTrack (l_posList, m_leftMenuCanvas.displaySpeed, onCloseMenuTweenFinished, Tweener.Style.QuadOutReverse);
			canMoveLeftMenu = false;
		}
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
		if(canMoveLeftMenu)
		{
			m_uiManager.changeScreen(UIScreen.LEFT_MENU,true);
			Vector3 l_position = m_menu.transform.localPosition;
			List<Vector3> l_posList = new List<Vector3> ();
			l_posList.Add (l_position);
			l_posList.Add (l_position + new Vector3 (200, 0, 0));
			m_menu.tweener.addPositionTrack (l_posList, m_leftMenuCanvas.displaySpeed, toShowMenuTweenFinished, Tweener.Style.QuadOutReverse);
			canMoveLeftMenu = false;
		}
	}

	private void toShowMenuTweenFinished( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		canMoveLeftMenu = true;
	}

	private void onCloseMenuTweenFinished( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		canMoveLeftMenu = true;
	}
	
	private void toShowAllChilren(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		m_leftMenuCanvas.showKids (addButtonClickCall);
	}
	
	private void addButtonClickCall( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_showProfileButton.addClickCallback (toShowAllChilren);
	}

	//star part
	
	private void onMathNavClicked(UIToggle p_toggle, bool p_bool)
	{
		if(p_bool == true)
		{
			m_currentSubjectId = m_mathId;
			checkRootList (m_currentSubjectId);
		}
	}

	private void onReadingNavClicked(UIToggle p_toggle, bool p_bool)
	{
		if(p_bool == true)
		{
			m_currentSubjectId = m_readingId;
			checkRootList (m_currentSubjectId);
		}
	}

	private void onScienceNavClicked(UIToggle p_toggle, bool p_bool)
	{
		if(p_bool == true)
		{
			m_currentSubjectId = m_scienceId;
			checkRootList (m_currentSubjectId);
		}
	}

	private void onSocialNavClicked(UIToggle p_toggle, bool p_bool)
	{
		if(p_bool == true)
		{
			m_currentSubjectId = m_socialId;
			checkRootList (m_currentSubjectId);
		}
	}

	private void onCognitiveNavClicked(UIToggle p_toggle, bool p_bool)
	{
		if(p_bool == true)
		{
			m_currentSubjectId = m_cognitiveId;
			checkRootList (m_currentSubjectId);
		}
	}

	private void onCreativeNavClicked(UIToggle p_toggle, bool p_bool)
	{
		if(p_bool == true)
		{
			m_currentSubjectId = m_creativeId;
			checkRootList (m_currentSubjectId);
		}
	}

	private void onLifeSkillsNavClicked(UIToggle p_toggle, bool p_bool)
	{
		if(p_bool == true)
		{
			m_currentSubjectId = m_lifeSkillsId;
			checkRootList (m_currentSubjectId);
		}
	}

	private void onListToEnd(Vector2 p_value)
	{
		if( p_value.y <= 0 )
		{
			m_swipeList.removeValueChangeListener(onListToEnd);
			if( m_isToRequest )
			{
				m_currentPage++;
				m_requestQueue.reset ();
				m_requestQueue.add ( new GetStarChartRequest( m_currentSubjectId, m_currentPage, _getStarChartRequestComplete ) );
				m_requestQueue.request (RequestType.RUSH);
			}
			else
			{
				loadLocal();
			}
		}
	}
	
	private void checkRootList(int p_rootId)
	{
		m_swipeList.removeValueChangeListener( onListToEnd );
		m_swipeList.active = false;
		m_currentList = new List<object> ();
		m_currentPage = 1;
		m_isToRequest = true;

		string l_appListJson = PlayerPrefs.GetString( "rootList" +  p_rootId);
		m_rootTable = MiniJSON.MiniJSON.jsonDecode( l_appListJson ) as Hashtable;
		
		if( m_rootTable == null || m_rootTable.Count == 0 )
		{
			m_requestQueue.reset ();
			m_requestQueue.add ( new GetStarChartListRequest( p_rootId, _getStarChartListRequestComplete ) );
			m_requestQueue.request (RequestType.RUSH);
		}
		else
		{
			m_swipeList.removeDrawFunction();
			m_starChartCanvas.setRootTable(m_rootTable);
			m_requestQueue.reset ();
			m_requestQueue.add ( new GetStarChartRequest( p_rootId, m_currentPage, _getStarChartRequestComplete ) );
			m_requestQueue.request (RequestType.RUSH);
		}
	}
		
	private void _getStarChartListRequestComplete(WWW p_response)
	{
		ArrayList l_list = MiniJSON.MiniJSON.jsonDecode (p_response.text) as ArrayList;
		Hashtable l_rootTable = new Hashtable ();
		foreach(Hashtable l_data in l_list)
		{
			l_rootTable.Add(l_data["id"].ToString(), l_data["name"].ToString());
		}
		m_rootTable = l_rootTable;

		PlayerPrefs.SetString( "rootList" + m_currentSubjectId, MiniJSON.MiniJSON.jsonEncode(m_rootTable) );

		m_swipeList.removeDrawFunction ();
		m_starChartCanvas.setRootTable (l_rootTable);

		m_requestQueue.reset ();
		m_requestQueue.add ( new GetStarChartRequest( m_currentSubjectId, m_currentPage, _getStarChartRequestComplete ) );
		m_requestQueue.request (RequestType.RUSH);
	}
	
	private void _getStarChartRequestComplete(WWW p_response)
	{
		ArrayList l_list = MiniJSON.MiniJSON.jsonDecode (p_response.text) as ArrayList;

		if( l_list == null )
			l_list = new ArrayList();

		int l_count = 10 - l_list.Count;

		foreach( Hashtable l_obj in l_list )
		{
			int l_id = int.Parse(l_obj["id"].ToString());
			int l_star = int.Parse(l_obj["star"].ToString());
			StarChartInfo l_info = new StarChartInfo(l_id, l_star);
			m_currentList.Add(l_info);
			m_rootTable.Remove(l_obj["id"].ToString());
		}

		if( l_count > 0 && m_rootTable.Count > 0 )
		{
			m_isToRequest = false;
			List<object> l_remove = new List<object>();
			foreach( object key in m_rootTable.Keys )
			{
				int l_key = int.Parse(key.ToString());
				StarChartInfo l_info = new StarChartInfo(l_key, 0);
				m_currentList.Add(l_info);
				l_remove.Add(key);
				l_count--;
				if( l_count <= 0)
					break;
			}
			foreach( object key in l_remove)
			{
				m_rootTable.Remove(key);
			}
		}

		m_starChartCanvas.setData(m_currentList);
		m_swipeList.addValueChangeListener (onListToEnd);
		m_swipeList.active = true;
	}

	private void loadLocal()
	{
		int l_count = 10;
		List<object> l_remove = new List<object>();

		foreach( object key in m_rootTable.Keys )
		{
			int l_key = int.Parse(key.ToString());
			StarChartInfo l_info = new StarChartInfo(l_key, 0);
			m_currentList.Add(l_info);
			l_remove.Add(key);
			l_count--;
			if( l_count <= 0)
				break;
		}

		foreach( object key in l_remove)
		{
			m_rootTable.Remove(key);
		}

		m_starChartCanvas.setData(m_currentList);
		m_swipeList.addValueChangeListener (onListToEnd);
	}

	private UIManager m_uiManager;

	private UICanvas m_dashboardCommonCanvas;
	private StarChartCanvas m_starChartCanvas;
	private LeftMenuCanvas m_leftMenuCanvas;
	private CommonDialogCanvas m_commonDialog;
	private UICanvas m_upgradeScreen;

	private UISwipeList m_childrenList;
	//top bar part
	private UIButton	m_leftSideMenuButton;
	private UIButton 	m_showProfileButton;
	private UIButton	m_closeLeftMenuButton;
	private UIButton    m_childModeButton;
	private UIButton    m_settingButton;
	private UIButton 	m_overviewButton;
	private UIButton	m_controlsButton;
	private UIButton	m_statChartButton;
	private UIButton 	m_helpButton;

	private UIElement 	m_menu;

	//nav part
	private UIToggle	m_mathToggle;
	private UIToggle	m_readingToggle;
	private UIToggle	m_scienceToggle;
	private UIToggle	m_socialToggle;
	private UIToggle	m_cognitiveToggle;
	private UIToggle	m_creativeToggle;
	private UIToggle	m_lifeSkillsToggle;
	private UIButton 	m_tryPremiumButton;
	private UIButton 	m_buyGemsButton;
	private RequestQueue m_requestQueue;

	private bool 		canMoveLeftMenu = true;

	//star part
	private Hashtable 	m_rootTable = new Hashtable();
	private List<object> 	m_currentList = new List<object>();
	private bool 		m_isToRequest = true;
	private int 		m_currentSubjectId;
	private int 		m_currentPage;

	private int 		m_mathId;
	private int 		m_readingId;
	private int 		m_scienceId;
	private int 		m_socialId;
	private int 		m_cognitiveId;
	private int 		m_creativeId;
	private int 		m_lifeSkillsId;

	private UISwipeList m_swipeList;
	private UIButton 	m_exitUpgradeButton;
	private UIButton 	m_upgradeButton;
}
