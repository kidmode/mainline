using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlTimeState : GameState
{
	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);
		
		m_uiManager = m_gameController.getUI();
		m_requestQueue = new RequestQueue();
		_setupScreen( p_gameController );
		_setupElment();

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
		
		m_uiManager.removeScreen( UIScreen.DASHBOARD_CONTROLLER );
		m_uiManager.removeScreen( UIScreen.DASHBOARD_COMMON );
		m_uiManager.removeScreen( UIScreen.LEFT_MENU );
		m_uiManager.removeScreen( UIScreen.COMMON_DIALOG );

		m_uiManager.removeScreen( UIScreen.TIME_LIMITS );
		m_uiManager.removeScreen( UIScreen.PAYWALL );
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

		m_timeLimitsCanvas 			= m_uiManager.createScreen( UIScreen.TIME_LIMITS, true, 1 ) 			as TimeLimitsCanvas;
		m_dashboardCommonCanvas 	= m_uiManager.createScreen( UIScreen.DASHBOARD_COMMON, true, 0 );
		
		if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
		{
			m_uiManager.setScreenEnable( UIScreen.TIME_LIMITS, false );
		}
	}

	private void _setupElment()
	{
		m_helpButton = m_timeLimitsCanvas.getView ("helpButton") as UIButton;
		m_helpButton.addClickCallback (onHelpButtonClick);
		
		int l_listCount = 4;

		m_dashboardControllerCanvas.setupDotList (l_listCount);
		m_dashboardControllerCanvas.setCurrentIndex (2);
		
		m_leftButton = 	m_dashboardControllerCanvas.getView( "leftButton" ) 	as UIButton;
		m_rightButton = m_dashboardControllerCanvas.getView( "rightButton" ) 	as UIButton;
		m_rightButton.	addClickCallback( onRightButtonClick );
		m_leftButton.	addClickCallback( onLeftButtonClick );
		
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_timeLimitsCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 830, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, 0.5f);
		
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

		m_appsButton = m_dashboardCommonCanvas.getView ("appsButton") as UIButton;
		m_appsButton.addClickCallback(goToAddApps);

		m_leftSideMenuButton = 	m_dashboardCommonCanvas.getView ("menuButton") as UIButton;
		m_childModeButton = 	m_dashboardCommonCanvas.getView ("childModelButton") as UIButton;
		m_overviewButton = 		m_dashboardCommonCanvas.getView ("overviewButton") 	as UIButton;
		m_statChartButton = 	m_dashboardCommonCanvas.getView ("starButton") 		as UIButton;
		m_controlsButton = 		m_dashboardCommonCanvas.getView ("controlButton") 	as UIButton;
		m_leftSideMenuButton.	addClickCallback (toShowMenu);
		m_childModeButton.		addClickCallback (toChildMode);
		m_overviewButton.		addClickCallback (goToOverview);
		m_statChartButton.		addClickCallback (goToStarChart);
		m_controlsButton.		enabled = false;
		
		//time part
		UIToggleGroup l_weekGroup= m_timeLimitsCanvas.getView( "weekGroup" ) as UIToggleGroup;
		m_weekThirtyMin 		= l_weekGroup.getView( "thirtyMinToggle" )	 as UIToggle;
		m_weekOneHour 			= l_weekGroup.getView( "oneHourToggle" )	 as UIToggle;
		m_weekTwoHours 			= l_weekGroup.getView( "twoHoursToggle" )	 as UIToggle;
		m_weekFourHours 		= l_weekGroup.getView( "fourHoursToggle" )	 as UIToggle;
		m_weekUnlimited 		= l_weekGroup.getView( "unlimitedToggle" )	 as UIToggle;
		
		UIToggleGroup l_weekendGroup= m_timeLimitsCanvas.getView( "weekendGroup" ) as UIToggleGroup;
		m_weekendThirtyMin 		= l_weekendGroup.getView( "thirtyMinToggle" )	 as UIToggle;
		m_weekendOneHour 		= l_weekendGroup.getView( "oneHourToggle" )		 as UIToggle;
		m_weekendTwoHours 		= l_weekendGroup.getView( "twoHoursToggle" )	 as UIToggle;
		m_weekendFourHours 		= l_weekendGroup.getView( "fourHoursToggle" )	 as UIToggle;
		m_weekendUnlimited 		= l_weekendGroup.getView( "unlimitedToggle" )	 as UIToggle;
		
		m_weekThirtyMin.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekOneHour.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekTwoHours.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekFourHours.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekUnlimited.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekendThirtyMin.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekendOneHour.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekendTwoHours.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekendFourHours.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekendUnlimited.addValueChangedCallback ( onTimeLimitsChanged );

		if( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() )
		{
			UIElement l_panel = m_timeLimitsCanvas.getView("panel");
			l_panel.active = false;
			m_requestQueue.reset ();
			m_requestQueue.add (new GetTimeLimitsRequest(_getTimeLimitRequestComplete));
			m_requestQueue.request( RequestType.SEQUENCE );
		}
	}

	private void checkRequest()
	{
		if (checkInternet() == false)
			return;

		if( m_isValueChanged )
		{
			m_isValueChanged = false;
			updateTimeLimits();
		}
	}

	private void onHelpButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks ();
		m_commonDialog.setOriginalPosition ();
		UIButton l_closeButton = m_commonDialog.getView ("closeMark") as UIButton;
		
		UILabel l_titleLabel = m_commonDialog.getView ("dialogText") as UILabel;
		UILabel l_contentLabel = m_commonDialog.getView ("contentText") as UILabel;
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_57_HELP_TITLE);
		l_contentLabel.text = Localization.getString(Localization.TXT_STATE_57_HELP_CONTENT);

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
				m_uiManager.setScreenEnable( UIScreen.TIME_LIMITS, false );
			}
		}
	}
	
	private void onCloseMenuTweenFinished( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		canMoveLeftMenu = true;
	}

	private void toSettingScreen(UIButton p_button)
	{
		if (checkInternet())
		{
			p_button.removeClickCallback (toSettingScreen);
			m_gameController.changeState (ZoodleState.SETTING_STATE);
		}
	}

	private void onSelectThisChild(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		if (checkInternet() == false)
			return;

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
			//sendCall (m_game.gameController,null,"/api/gems_amount/gems_amount?" + ZoodlesConstants.PARAM_TOKEN + "=" + SessionHandler.getInstance ().token.getSecret (),CallMethod.GET,ZoodleState.BUY_GEMS);
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
		if(canMoveLeftMenu && checkInternet())
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

	private void goToAddApps( UIButton p_button )
	{
		m_gameController.changeState (ZoodleState.CONTROL_APP);
	}

	private void goToOverview( UIButton p_button )
	{
		if (checkInternet())
		{
			m_gameController.changeState (ZoodleState.OVERVIEW_INFO);
		}
	}

	private bool checkInternet()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			Game game = GameObject.FindWithTag("GameController").GetComponent<Game>();
			game.gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
			
			ErrorMessageScript error = GameObject.FindWithTag("ErrorMessageTag").GetComponent<ErrorMessageScript>() as ErrorMessageScript;
			if (error != null)
				error.onClick += onClickExit;
			
			return false;
		}
		return true;
	}

	private void onClickExit()
	{
		ErrorMessageScript error = GameObject.FindWithTag("ErrorMessageTag").GetComponent<ErrorMessageScript>() as ErrorMessageScript;
		error.onClick -= onClickExit;;
		m_gameController.changeState (ZoodleState.CONTROL_APP);
	}
	
	private void goToStarChart( UIButton p_button )
	{
		m_gameController.changeState (ZoodleState.DASHBOARD_STAR_CHART);
	}

	private void onLeftButtonClick( UIButton p_button )
	{
		if (checkInternet())
		{
			m_gameController.changeState (ZoodleState.CONTROL_LANGUAGE);
		}
	}
	
	private void onRightButtonClick( UIButton p_button )
	{
		if (checkInternet())
		{
			m_gameController.changeState (ZoodleState.CONTROL_VIOLENCE);
		}
	}

	private void onTimeLimitsChanged( UIToggle p_toggle, bool p_bool )
	{
		m_isValueChanged = true;
	}

	private void _getTimeLimitRequestComplete(WWW p_response)
	{
		m_timeLimitsCanvas.setTimeLimits(MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable);
		UIElement l_panel = m_timeLimitsCanvas.getView("panel");
		
		if( null == l_panel )
		{
			return;
		}

		l_panel.active = true;
		m_isValueChanged = false;
	}

	private void updateTimeLimits ()
	{
		Hashtable l_param = new Hashtable ();
		l_param [ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance ().token.getSecret ();
		
		if( m_weekUnlimited.isOn )
		{
			l_param[ZoodlesConstants.PARAM_WEEKDAY_DISABLED] = true;
			l_param[ZoodlesConstants.PARAM_WEEKDAY_LIMIT] = "";
		}
		else
		{
			l_param[ZoodlesConstants.PARAM_WEEKDAY_DISABLED] = false;
			if( m_weekThirtyMin.isOn )
				l_param[ZoodlesConstants.PARAM_WEEKDAY_LIMIT] = "30";
			if( m_weekOneHour.isOn )
				l_param[ZoodlesConstants.PARAM_WEEKDAY_LIMIT] = "60";
			if( m_weekTwoHours.isOn )
				l_param[ZoodlesConstants.PARAM_WEEKDAY_LIMIT] = "120";
			if( m_weekFourHours.isOn )
				l_param[ZoodlesConstants.PARAM_WEEKDAY_LIMIT] = "240";
		}
		
		if( m_weekendUnlimited.isOn )
		{
			l_param[ZoodlesConstants.PARAM_WEEKEND_DISABLED] = true;
			l_param[ZoodlesConstants.PARAM_WEEKEND_LIMIT] = "";
		}
		else
		{
			l_param[ZoodlesConstants.PARAM_WEEKEND_DISABLED] = false;
			if( m_weekendThirtyMin.isOn )
				l_param[ZoodlesConstants.PARAM_WEEKEND_LIMIT] = "30";
			if( m_weekendOneHour.isOn )
				l_param[ZoodlesConstants.PARAM_WEEKEND_LIMIT] = "60";
			if( m_weekendTwoHours.isOn )
				l_param[ZoodlesConstants.PARAM_WEEKEND_LIMIT] = "120";
			if( m_weekendFourHours.isOn )
				l_param[ZoodlesConstants.PARAM_WEEKEND_LIMIT] = "240";
		}

		m_requestQueue.reset ();
		m_requestQueue.add ( new SetTimeLimitsRequest(l_param));
		m_requestQueue.request (RequestType.SEQUENCE);
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
	private UIButton		m_appsButton;
	private UIButton 		m_overviewButton;
	private UIButton		m_controlsButton;
	private UIButton		m_statChartButton;
	private UIElement 		m_menu;
	private bool 			canMoveLeftMenu = true;

	// time part
	private TimeLimitsCanvas m_timeLimitsCanvas;

	private UIToggle m_weekThirtyMin;
	private UIToggle m_weekOneHour;
	private UIToggle m_weekTwoHours;
	private UIToggle m_weekFourHours;
	private UIToggle m_weekUnlimited;
	
	private UIToggle m_weekendThirtyMin;
	private UIToggle m_weekendOneHour;
	private UIToggle m_weekendTwoHours;
	private UIToggle m_weekendFourHours;
	private UIToggle m_weekendUnlimited;
}
