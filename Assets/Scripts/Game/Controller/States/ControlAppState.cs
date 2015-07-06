using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlAppState : GameState
{
	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);
		
		m_uiManager = m_gameController.getUI();
		m_requestQueue = new RequestQueue();
		m_appList = new ArrayList ();
		_setupScreen( p_gameController );
		_setupElment();

//		TutorialController.Instance.showTutorial(TutorialSequenceName.Add_YOUR_APP);
		TutorialController.Instance.showNextPage();
	}
	
	public override void update (GameController p_gameController, int p_time)
	{
		base.update (p_gameController, p_time);
	}
	
	public override void exit (GameController p_gameController)
	{
		checkRequest ();
		
		base.exit (p_gameController);
		
		m_uiManager.removeScreenImmediately( UIScreen.DASHBOARD_CONTROLLER );
		m_uiManager.removeScreenImmediately( UIScreen.DASHBOARD_COMMON );
		m_uiManager.removeScreenImmediately( UIScreen.LEFT_MENU );
		m_uiManager.removeScreenImmediately( UIScreen.COMMON_DIALOG );
		
		m_uiManager.removeScreenImmediately( UIScreen.ADD_APPS );
		m_uiManager.removeScreenImmediately( UIScreen.PAYWALL );
	}

	private void _setupScreen( GameController p_gameController )
	{
		m_commonDialog 				= m_uiManager.createScreen( UIScreen.COMMON_DIALOG, true, 5 ) 			as CommonDialogCanvas;
		m_commonDialog.setUIManager (p_gameController.getUI());
		m_leftMenuCanvas 			= m_uiManager.createScreen( UIScreen.LEFT_MENU, true, 4 ) 				as LeftMenuCanvas;
		m_dashboardControllerCanvas = m_uiManager.createScreen( UIScreen.DASHBOARD_CONTROLLER, false, 3 ) 	as DashBoardControllerCanvas;

		if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
		{
			m_paywallCanvas = m_uiManager.createScreen( UIScreen.PAYWALL, false, 2 );
			m_upgradeButton = m_paywallCanvas.getView( "upgradeButton" ) as UIButton;
			m_upgradeButton.addClickCallback( onUpgradeButtonClick );
		}

		m_addAppCanvas 				= m_uiManager.createScreen( UIScreen.ADD_APPS, true, 1 ) 				as AddAppCanvas;
		m_dashboardCommonCanvas 	= m_uiManager.createScreen( UIScreen.DASHBOARD_COMMON, true, 0 );

		if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
		{
			m_uiManager.setScreenEnable( UIScreen.ADD_APPS, false );
		}
	}

	private void _setupElment()
	{
		m_helpButton = m_addAppCanvas.getView ("helpButton") as UIButton;
		m_helpButton.addClickCallback (onHelpButtonClick);
		
		int l_listCount = 1;

		m_dashboardControllerCanvas.setupDotList (l_listCount);
		m_dashboardControllerCanvas.setCurrentIndex (0);
		
		m_leftButton = 	m_dashboardControllerCanvas.getView( "leftButton" ) 	as UIButton;
		m_rightButton = m_dashboardControllerCanvas.getView( "rightButton" ) 	as UIButton;
		m_leftButton.enabled = false;
		m_rightButton.enabled = false;

		
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_addAppCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 830, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, 0.5f, onShowFinish, Tweener.Style.Standard, false);
		
		m_menu = m_leftMenuCanvas.getView ("LeftMenu") as UIElement;
		m_showProfileButton = m_menu.getView ("profileButton") as UIButton;
		m_showProfileButton.addClickCallback (toShowAllChilren);
		
		m_closeLeftMenuButton = m_leftMenuCanvas.getView ("closeButton") as UIButton;
		m_settingButton = 		m_leftMenuCanvas.getView ("settingButton") 	as UIButton;
		m_childrenList = 		m_leftMenuCanvas.getView ("childSwipeList") as UISwipeList;
		m_tryPremiumButton = 	m_leftMenuCanvas.getView ("premiumButton") 	as UIButton;
		m_buyGemsButton = 		m_leftMenuCanvas.getView ("buyGemsButton") 	as UIButton;
		m_closeLeftMenuButton.	addClickCallback (onCloseMenu);
		m_settingButton.		addClickCallback (toSettingScreen);
		m_childrenList.			addClickListener ("Prototype",onSelectThisChild);
		m_tryPremiumButton.		addClickCallback (toPremiumScreen);
		m_buyGemsButton.		addClickCallback (toBuyGemsScreen);

		m_appsButton = m_dashboardCommonCanvas.getView("appsButton") as UIButton;
		m_appsButton.enabled = false;

		m_leftSideMenuButton = 	m_dashboardCommonCanvas.getView ("menuButton") as UIButton;
		m_childModeButton = 	m_dashboardCommonCanvas.getView ("childModelButton") as UIButton;
		m_overviewButton = 		m_dashboardCommonCanvas.getView ("overviewButton") 	as UIButton;
		m_statChartButton = 	m_dashboardCommonCanvas.getView ("starButton") 		as UIButton;
		m_controlsButton = 		m_dashboardCommonCanvas.getView ("controlButton") 	as UIButton;
		m_leftSideMenuButton.	addClickCallback (toShowMenu);
		m_childModeButton.		addClickCallback (toChildMode);
		m_overviewButton.		addClickCallback (goToOverview);
		m_statChartButton.		addClickCallback (goToStarChart);
		m_controlsButton.		addClickCallback (goToSubject);
		
		//app part
		m_appSwipeList = m_addAppCanvas.getView ( "appSwipeList" ) as UISwipeList;
		m_appSwipeList.addClickListener ( "controlButton", m_addAppCanvas.onButtonClicked );
		m_appSwipeList.addClickListener ( "controlButton", onAppButtonClicked );
		m_appSwipeList.active = false;
	}

	private void checkRequest()
	{
		if( m_isValueChanged )
		{
			m_isValueChanged = false;
			updateAddApp();
		}
	}
	
	private void onHelpButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks ();
		m_commonDialog.setOriginalPosition ();
		UIButton l_closeButton = m_commonDialog.getView ("closeMark") as UIButton;

		UILabel l_titleLabel = m_commonDialog.getView ("dialogText") as UILabel;
		UILabel l_contentLabel = m_commonDialog.getView ("contentText") as UILabel;
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_59_HELP_TITLE);
		l_contentLabel.text = Localization.getString(Localization.TXT_STATE_59_HELP_CONTENT);

		l_closeButton.addClickCallback (onCloseDialogButtonClick);
	}
	
	private void onCloseDialogButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		m_commonDialog.setOutPosition ();
		m_helpButton.addClickCallback (onHelpButtonClick);
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

			if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
			{
				m_uiManager.setScreenEnable( UIScreen.ADD_APPS, false );
			}
		}
	}
	
	private void onCloseMenuTweenFinished( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		canMoveLeftMenu = true;
	}
	
	private void toSettingScreen(UIButton p_button)
	{
		p_button.removeClickCallback (toSettingScreen);
		m_gameController.changeState (ZoodleState.SETTING_STATE);
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
	
	private void toPremiumScreen(UIButton p_button)
	{
		m_gameController.connectState (ZoodleState.SIGN_IN_UPSELL, int.Parse(m_gameController.stateName));
		m_gameController.changeState (ZoodleState.SIGN_IN_UPSELL);
	}
	
	private void toBuyGemsScreen(UIButton p_button)
	{
		gotoGetGems ();
	}
	
	private void gotoGetGems()
	{	
		string l_returnJson = SessionHandler.getInstance ().returnJson;
		
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
	
	private void goToOverview( UIButton p_button )
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			Game game = GameObject.Find("GameLogic").GetComponent<Game>();
			game.gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
		}
		else
		{
			m_gameController.changeState (ZoodleState.OVERVIEW_INFO);
		}
	}

	private void goToSubject( UIButton p_button )
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			Game game = GameObject.Find("GameLogic").GetComponent<Game>();
			game.gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
		}
		else
		{
			m_gameController.changeState (ZoodleState.CONTROL_SUBJECT);
		}
	}
	
	private void goToStarChart( UIButton p_button )
	{
		m_gameController.changeState (ZoodleState.DASHBOARD_STAR_CHART);
	}

	private void onLeftButtonClick( UIButton p_button )
	{
		return;
	}
	
	private void onRightButtonClick( UIButton p_button )
	{
		m_gameController.changeState( ZoodleState.CONTROL_SUBJECT );
	}

	private void onAppButtonClicked(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
//		p_list.removeClickListener ( "controlButton", onAppButtonClicked );

		TutorialController.Instance.showNextPage();
		
		AppInfo l_appInfo = p_data as AppInfo;
		DebugUtils.Assert ( l_appInfo != null );
		
		string l_appListJson = PlayerPrefs.GetString( "addedAppList" );
		ArrayList l_appNameList = MiniJSON.MiniJSON.jsonDecode( l_appListJson ) as ArrayList;
		
		if( null == l_appNameList )
		{
			_Debug.log( "null list" );
			l_appNameList = new ArrayList();
			PlayerPrefs.SetString( "addedAppList", MiniJSON.MiniJSON.jsonEncode( l_appNameList ) );
		}
		
		if( l_appNameList.Count > 0 && l_appNameList.Contains(l_appInfo.packageName) )
		{
			_Debug.log( "remove" );
			l_appNameList.Remove( l_appInfo.packageName );
			
			if( m_appList.Count > 0 )
			{
				for( int i = 0; i < m_appList.Count; i++)
				{
					if( (m_appList[i] as AppInfoData).packageName.Equals(l_appInfo.packageName) )
					{
						m_appList.RemoveAt(i);
						break;
					}
				}
			}
		}
		else
		{
			_Debug.log( "add" );
			m_isValueChanged = true;
			
			l_appNameList.Add( l_appInfo.packageName );
			
			AppInfoData l_appData = new AppInfoData();
			
			l_appData.age = SessionHandler.getInstance().currentKid.age;
			l_appData.name = l_appInfo.appName;
			l_appData.packageName = l_appInfo.packageName;

			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaClass l_jcLocale = new AndroidJavaClass ( "java.util.Locale" );
			AndroidJavaObject l_joLocale = l_jcLocale.CallStatic<AndroidJavaObject>( "getDefault" );
			string l_country = l_joLocale.Call<string>( "getCountry" );
			string l_language = l_joLocale.Call<string>( "getLanguage" );
			l_appData.country = l_country;
			l_appData.language = l_language;
			#endif
			
			m_appList.Add( l_appData );
		}
		_Debug.log ( MiniJSON.MiniJSON.jsonEncode(l_appNameList) );
		
		PlayerPrefs.SetString( "addedAppList", MiniJSON.MiniJSON.jsonEncode(l_appNameList) );
	}

	private void onShowFinish(UIElement p_element, Tweener.TargetVar p_target)
	{
//		if( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() )
//		{

		try 
		{
			m_addAppCanvas.firstLoadApp();
		}
		catch (System.Exception e)
		{

		}
//		m_appSwipeList.removeClickListener( "controlButton", onAppButtonClicked );
//		}
//		else
//		{
//			m_appSwipeList.setData( new List<object>() );
//		}
	}

	private void updateAddApp()
	{
		_Debug.log ( "update add app" );
		
		string l_jsonString = "";
		l_jsonString += "[";
		foreach( AppInfoData l_appInfoData in m_appList)
		{
			l_jsonString += l_appInfoData.toJson();
			l_jsonString += ",";
		}
		l_jsonString = l_jsonString.TrimEnd(',');
		l_jsonString += "]";
		
		_Debug.log( l_jsonString );

		m_requestQueue.reset ();
		m_requestQueue.add( new SetAddAppRequest( l_jsonString ) );
		m_requestQueue.request( RequestType.SEQUENCE );
	}

	private void onUpgradeButtonClick(UIButton p_button)
	{
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
	
	private UIManager 		m_uiManager;
	private RequestQueue 	m_requestQueue;
	private bool 			m_isValueChanged = false;

	private UICanvas 		m_dashboardCommonCanvas;
	
	private LeftMenuCanvas 	m_leftMenuCanvas;
	private UISwipeList 	m_childrenList;
	
	private DashBoardControllerCanvas m_dashboardControllerCanvas;
	private UIButton 		m_leftButton;
	private UIButton 		m_rightButton;

	private UIButton 		m_helpButton;
	private CommonDialogCanvas m_commonDialog;
		
	private UICanvas 		m_paywallCanvas;
	private UIButton 		m_upgradeButton;
	
	//top bar part
	private UIButton 		m_tryPremiumButton;
	private UIButton 		m_buyGemsButton;
	private UIButton 		m_leftSideMenuButton;
	private UIButton 		m_showProfileButton;
	private UIButton		m_closeLeftMenuButton;
	private UIButton	    m_childModeButton;
	private UIButton	    m_settingButton;
	private UIButton 		m_appsButton;
	private UIButton 		m_overviewButton;
	private UIButton		m_controlsButton;
	private UIButton		m_statChartButton;
	private UIElement 		m_menu;
	private bool 			canMoveLeftMenu = true;

	//app part
	private AddAppCanvas 	m_addAppCanvas;

	private UISwipeList 	m_appSwipeList;
	private ArrayList 		m_appList;
}

public class AppInfoData
{
	public int age;
	public string name;
	public string packageName;
	public string country;
	public string language;
	
	public string toJson()
	{
		string l_jsonString = "";
		
		l_jsonString += "{";
		l_jsonString += "\"age\":";
		l_jsonString += age;
		l_jsonString += ",\"name\": \"";
		l_jsonString += name;
		l_jsonString += "\",\"package_name\":\"";
		l_jsonString += packageName;
		l_jsonString += "\",\"country\":\"";
		l_jsonString += country;
		l_jsonString += "\",\"language\":\"";
		l_jsonString += language;
		l_jsonString += "\"}";
		
		return l_jsonString;
	}
}
