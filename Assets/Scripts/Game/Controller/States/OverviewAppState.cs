using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

public class OverviewAppState : GameState
{
	//--------------------Public Interface -----------------------

	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);

		m_uiManager = m_gameController.getUI();
	 	m_currentPage = 1;
		m_isLoaded = false;
		m_requestQueue = new RequestQueue();
		m_getAppRequestQueue = new RequestQueue();
		m_getIconRequestQueue = new RequestQueue();
		m_iconRequests = new List<RequestQueue>();

//		foreach (Kid l_k in SessionHandler.getInstance ().kidList)
//		{
//			if(l_k.id == SessionHandler.getInstance ().currentKid.id)
//				m_currentAppList = l_k.appList == null? new List<object>():l_k.appList;
//		}

		m_currentAppList = SessionHandler.getInstance().currentKid.appList == null? new List<object>():SessionHandler.getInstance().currentKid.appList;
		_setupScreen(p_gameController);
		_setupElment();
	}

	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		if (isLoadApp)
		{
			if ( m_isLoaded )
			{
				isLoadApp = false;
				m_isLoaded = false;
				_setupRecommendedAppCanvas();
			}
		}
		if (null != m_appCountLabel)
			m_appCountLabel.text = m_currentAppList.Count.ToString ();
//		m_gemCountLabel.text = SessionHandler.getInstance().currentKid.gems.ToString();
	}

	public override void exit(GameController p_gameController)
	{
		base.exit(p_gameController);
		disposeIconRequests();
		m_getIconRequestQueue.dispose ();
		m_uiManager.removeScreen(UIScreen.CONFIRM_DIALOG);
		m_uiManager.removeScreen(UIScreen.APP_DETAILS);
		m_uiManager.removeScreen(UIScreen.APP_LIST);
		m_uiManager.removeScreen(UIScreen.LEFT_MENU);
		m_uiManager.removeScreen(UIScreen.RECOMMENDED_APP);
		m_uiManager.removeScreen(UIScreen.DASHBOARD_CONTROLLER);
		m_uiManager.removeScreen(UIScreen.DASHBOARD_COMMON);
		m_uiManager.removeScreen(UIScreen.COMMON_DIALOG);
	}

	//----------------- Private Implementation -------------------

	private void disposeIconRequests()
	{
		int l_numRequests = m_iconRequests.Count;
		for (int i = 0; i < l_numRequests; ++i)
		{
			RequestQueue l_queue = m_iconRequests[i];
			l_queue.dispose();
		}
		m_iconRequests.Clear();
		m_iconRequests = null;
	}

	private void _setupScreen( GameController p_gameController )
	{
		m_commonDialog = m_uiManager.createScreen( UIScreen.COMMON_DIALOG, true, 16 )  as CommonDialogCanvas;
		m_commonDialog.setUIManager (p_gameController.getUI());
		m_confirmDialogCanvas 	= m_uiManager.createScreen( UIScreen.CONFIRM_DIALOG, false, 15 );
		m_appDetailsCanvas 	= m_uiManager.createScreen( UIScreen.APP_DETAILS, false, 14 );
		m_appListCanvas 	= m_uiManager.createScreen( UIScreen.APP_LIST, false, 12 );
		m_leftMenuCanvas = m_uiManager.createScreen( UIScreen.LEFT_MENU, true, 10 )  as LeftMenuCanvas;
		m_dashboardControllerCanvas = m_uiManager.createScreen( UIScreen.DASHBOARD_CONTROLLER, false, 9 ) as DashBoardControllerCanvas;
		m_recommendedAppCanvas	= m_uiManager.createScreen( UIScreen.RECOMMENDED_APP, true, 2 );
		m_dashboardCommonCanvas = m_uiManager.createScreen( UIScreen.DASHBOARD_COMMON, true, 0 );
		m_prototype = m_appListCanvas.getView ("Prototype");
		m_prototype.active = false;
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_recommendedAppCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 830, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
		m_helpButton = m_recommendedAppCanvas.getView ("helpButton") as UIButton;
		m_helpButton.addClickCallback (onHelpButtonClick);
		m_costArea = m_confirmDialogCanvas.getView("costArea");
		m_needMoreArea = m_confirmDialogCanvas.getView("needMoreArea");
		m_exitConfirmDialogButton = m_confirmDialogCanvas.getView ("exitButton") as UIButton;
		m_exitConfirmDialogButton.addClickCallback (onExitConfiemDialogButtonClick);
		m_cancelBuyButton = m_confirmDialogCanvas.getView ("cancelButton") as UIButton;
		m_cancelBuyButton.addClickCallback (onExitConfiemDialogButtonClick);
		m_confirmBuyButton = m_confirmDialogCanvas.getView ("confirmButton") as UIButton;
		m_confirmBuyButton.addClickCallback (onConfiemButtonClick);

		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED);
	}

	private void _setupElment()
	{
		m_leftButton = m_dashboardControllerCanvas.getView( "leftButton" ) as UIButton;
		m_leftButton.addClickCallback( onLeftButtonClick );
		
		m_rightButton = m_dashboardControllerCanvas.getView( "rightButton" ) as UIButton;
		m_rightButton.addClickCallback( onRightButtonClick );
		m_dashboardControllerCanvas.setupDotList( 6 );
		m_dashboardControllerCanvas.setCurrentIndex( 3 );
//		m_gemCountLabel = m_recommendedAppCanvas.getView("gemCountText") as UILabel;
		m_moreAppButton = m_recommendedAppCanvas.getView( "appListButton" ) as UIButton;
		m_moreAppButton.addClickCallback( onMoreAppButtonClick );
		m_exitAppListButton = m_appListCanvas.getView( "exitButton" ) as UIButton;
		m_exitAppListButton.addClickCallback( onExitAppListButtonClick );
		m_exitAppDetailsButton = m_appDetailsCanvas.getView( "exitButton" ) as UIButton;
		m_exitAppDetailsButton.addClickCallback( onExitAppDetailsButtonClick );
		m_appList = m_appListCanvas.getView ("appSwipeList") as UISwipeList;
		
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

		m_appsButton = m_dashboardCommonCanvas.getView ("appsButton") as UIButton;
		m_appsButton.addClickCallback(goToAddApps);

		m_overviewButton = m_dashboardCommonCanvas.getView ("overviewButton") as UIButton;
		m_overviewButton.enabled = false;
		
		m_controlsButton = m_dashboardCommonCanvas.getView ("controlButton") as UIButton;
		m_controlsButton.addClickCallback (goToControls);
		
		m_statChartButton = m_dashboardCommonCanvas.getView ("starButton") as UIButton;
		m_statChartButton.addClickCallback (goToStarChart);
		
		m_childrenList = m_leftMenuCanvas.getView ("childSwipeList") as UISwipeList;
		m_childrenList.addClickListener ("Prototype",onSelectThisChild);

		m_tryPremiumButton = m_leftMenuCanvas.getView ("premiumButton") as UIButton;
		m_buyGemsButton = m_leftMenuCanvas.getView ("buyGemsButton") as UIButton;
		m_buyGemsButtonOnConfirm = m_confirmDialogCanvas.getView ("buyGemsButton") as UIButton;
		m_tryPremiumButton.addClickCallback (toPremiumScreen);
		m_buyGemsButton.addClickCallback (toBuyGemsScreen);
		m_buyGemsButtonOnConfirm.addClickCallback (toBuyGemsScreen);

		m_appCountLabel = m_appListCanvas.getView ("appCountText") as UILabel;

		//Create an empty list for set up swipeList.
		List<System.Object> l_list = new List<System.Object>();
		m_appList.setData (l_list);
		if((null == m_currentAppList || m_currentAppList.Count == 0) && SessionHandler.getInstance().appRequest.isCompleted())
			loadAppList ();
		else
			loadAppListImmediate();
	}

	//-------------------------------------------------------------------------------------------------------------------------------------------------//

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

	private void onHelpButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks ();
		m_commonDialog.setOriginalPosition ();
		UIButton l_closeButton = m_commonDialog.getView ("closeMark") as UIButton;
		
		UILabel l_titleLabel = m_commonDialog.getView ("dialogText") as UILabel;
		UILabel l_contentLabel = m_commonDialog.getView ("contentText") as UILabel;
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_50_HELP_TITLE);
		l_contentLabel.text = Localization.getString(Localization.TXT_STATE_50_HELP_CONTENT);

		l_closeButton.addClickCallback (onCloseDialogButtonClick);
	}
	
	private void onCloseDialogButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		m_commonDialog.setOutPosition ();
		m_helpButton.addClickCallback (onHelpButtonClick);
	}

	private void onExitConfiemDialogButtonClick( UIButton p_button )
	{
		m_uiManager.changeScreen (UIScreen.CONFIRM_DIALOG,false);
		m_uiManager.changeScreen (UIScreen.APP_DETAILS,true);
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_currentPanel = m_confirmDialogCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_currentPanel.transform.localPosition );
		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	private void onConfiemButtonClick( UIButton p_button )
	{
		if(null != m_detailsApp)
		{
			if(SessionHandler.getInstance().currentKid.gems >= m_detailsApp.gems)
				sendBuyAppRequest ();
			else
			{
				UILabel l_costGems = m_confirmDialogCanvas.getView("costPriceText") as UILabel;
				UILabel l_needGems = m_confirmDialogCanvas.getView("needPriceText") as UILabel;
				l_costGems.text = m_detailsApp.gems.ToString ();
				l_needGems.text = (m_detailsApp.gems - SessionHandler.getInstance().currentKid.gems).ToString ();
				UILabel l_titleLabel = m_confirmDialogCanvas.getView("titleText") as UILabel;
				l_titleLabel.text = Localization.getString(Localization.TXT_STATE_45_GEM_TITLE);
				UILabel l_notice1label = m_confirmDialogCanvas.getView("noticeText1") as UILabel;
				l_notice1label.text = Localization.getString(Localization.TXT_STATE_45_GEM_INFO);
				m_costArea.active = false;
				m_needMoreArea.active = true;
			}
		}
	}

	private void sendBuyAppRequest()
	{
		m_requestQueue.reset();
		m_requestQueue.add(new BuyAppRequest(m_detailsApp.id, _buyAppComplete));
		m_requestQueue.request();
		m_uiManager.changeScreen(UIScreen.CONFIRM_DIALOG, false);
	}

	private void _buyAppComplete(WWW p_response)
	{
		if (null == p_response.error)
		{
			m_detailsApp.own = true;
			UILabel l_appCostText = m_buyAppButton.parent.getView("appCostText") as UILabel;
			UIButton l_buyAppButton = m_buyAppButton.parent.getView("buyAppButton") as UIButton;
			l_appCostText.active = false;
			l_buyAppButton.active = false;

			_updateAppState(m_buyedAppElement);
			_updateAppState(m_buyedAppElementInRecommendPage);
			
			if (null != m_confirmDialogCanvas)
			{
				m_uiManager.changeScreen(UIScreen.CONFIRM_DIALOG,false);
				m_uiManager.changeScreen(UIScreen.APP_DETAILS,true);
				List<Vector3> l_pointListOut = new List<Vector3>();
				UIElement l_currentPanel = m_confirmDialogCanvas.getView("mainPanel");
				l_pointListOut.Add(l_currentPanel.transform.localPosition);
				l_pointListOut.Add(l_currentPanel.transform.localPosition - new Vector3(0, 800, 0));
				l_currentPanel.tweener.addPositionTrack(l_pointListOut, 0f);
			}
			
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			Hashtable l_appDate = null;
			App l_app = null;
			if(l_data.ContainsKey("jsonResponse"))
			{
				Hashtable l_response = l_data["jsonResponse"] as Hashtable;
				if(l_response.ContainsKey("response"))
					l_appDate = l_response["response"] as Hashtable;
			}
			if(null != l_appDate)
			{
				l_app = new App(l_appDate);
			}
			else
			{
				setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_50_FAIL),Localization.getString(Localization.TXT_STATE_50_CONTACT));
			}

			installApp( l_app.packageName );
		}
	}

	private void installApp( string p_packageName )
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass l_jcPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject l_joActivity = l_jcPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		
		AndroidJavaClass jc_uri = new AndroidJavaClass("android.net.Uri");
		
		AndroidJavaObject l_uri = jc_uri.CallStatic<AndroidJavaObject>("parse", string.Format("market://details?id={0}", p_packageName));
		
		AndroidJavaClass jc_intent = new AndroidJavaClass("android.content.Intent");
		
		AndroidJavaObject jo_view = jc_intent.GetStatic<AndroidJavaObject>("ACTION_VIEW");
		
		AndroidJavaObject jo_intent = new AndroidJavaObject("android.content.Intent", jo_view, l_uri);
		
		AndroidJavaObject jo_chooser = jc_intent.CallStatic<AndroidJavaObject>("createChooser", jo_intent, Localization.getString(Localization.TXT_STATE_45_MARKET));
		
		l_joActivity.Call("startActivity", jo_chooser );
		
		GAUtil.logInstallApp(p_packageName);
		#endif
	}

	private void _updateAppState(UIElement p_element)
	{
		if (p_element == null)
			return;
		
		UILabel l_costLabel = p_element.getView("appCostText") as UILabel;
		UILabel l_freeLabel = p_element.getView("appFreeText") as UILabel;
		UILabel l_sponsoredLabel = p_element.getView("sponsoredText") as UILabel;
		
		if (null != l_costLabel)
			l_costLabel.active = false;
		
		l_freeLabel.active = false;
		l_sponsoredLabel.active = false;
	}
	
	public void confirmBuyApp(App p_app)
	{
		m_costArea.active = true;
		m_needMoreArea.active = false;
		
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_confirmDialogCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f);
		
		UILabel l_titleLabel = l_newPanel.getView("titleText") as UILabel;
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_45_CONFIRM);
		UILabel l_notice1label = l_newPanel.getView("noticeText1") as UILabel;
		l_notice1label.text = Localization.getString(Localization.TXT_STATE_45_PURCHASE);
		UILabel l_notice1label2 = l_newPanel.getView("noticeText2") as UILabel;
		l_notice1label2.text = p_app.name;
		UILabel l_priceLabel = l_newPanel.getView("priceText") as UILabel;
		l_priceLabel.text = p_app.gems.ToString ();
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

	private void toSettingScreen(UIButton p_button)
	{
		if (checkInternet())
		{
			p_button.removeClickCallback (toSettingScreen);
			m_gameController.changeState (ZoodleState.SETTING_STATE);
		}
	}

	private void goToAddApps( UIButton p_button )
	{
		m_gameController.changeState (ZoodleState.CONTROL_APP);
	}

	private void goToControls( UIButton p_button )
	{
		if (checkInternet())
		{
			m_gameController.changeState (ZoodleState.CONTROL_SUBJECT);
		}
	}

	private bool checkInternet()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable 
		    || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected())
		{
			m_gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
			
			ErrorMessage error = GameObject.FindWithTag("ErrorMessageTag").GetComponent<ErrorMessage>() as ErrorMessage;
			if (error != null)
				error.onClick += onClickExit;
			
			return false;
		}
		return true;
	}

	private void onClickExit()
	{
		ErrorMessage error = GameObject.FindWithTag("ErrorMessageTag").GetComponent<ErrorMessage>() as ErrorMessage;
		error.onClick -= onClickExit;;
		m_gameController.changeState (ZoodleState.CONTROL_APP);
	}

	private void goToStarChart( UIButton p_button )
	{
		m_gameController.changeState (ZoodleState.DASHBOARD_STAR_CHART);
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

	private void onCloseMenuTweenFinished( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		canMoveLeftMenu = true;
	}
	
	private void toShowMenuTweenFinished( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		canMoveLeftMenu = true;
	}

	private void onLeftButtonClick( UIButton p_button )
	{
		if (checkInternet())
		{
			m_gameController.changeState (ZoodleState.OVERVIEW_PROGRESS);
		}
	}
	
	private void onRightButtonClick( UIButton p_button )
	{
		if (checkInternet())
		{
			m_gameController.changeState (ZoodleState.OVERVIEW_BOOK);
		}
	}
	
	private void goToChildLock(UIButton p_button)
	{
		m_gameController.changeState (ZoodleState.CHILD_LOCK_STATE);
	}

	//-------------------------------------------------------------------------------------------------------------------------------------------------//
	private void _setupRecommendedAppCanvas()
	{
		UILabel l_loading = m_recommendedAppCanvas.getView ("loadingText") as UILabel;

		List<System.Object> l_list = m_currentAppList;
		
		if( m_currentAppList.Count > 0 )
		{
			l_loading.active = false;
		}
		else
		{
			l_loading.text = Localization.getString(Localization.TXT_STATE_50_EMPTY);
			return;
		}

		List<UIElement> l_canvasList = new List<UIElement> ();
		UIElement l_app1 = m_recommendedAppCanvas.getView ("appOne") as UIElement;
		UIElement l_app2 = m_recommendedAppCanvas.getView ("appTwo") as UIElement;
		UIElement l_app3 = m_recommendedAppCanvas.getView ("appThree") as UIElement;
		UIElement l_app4 = m_recommendedAppCanvas.getView ("appFour") as UIElement;
		l_canvasList.Add (l_app1);
		l_canvasList.Add (l_app2);
		l_canvasList.Add (l_app3);
		l_canvasList.Add (l_app4);

		int l_count = l_list.Count >= 4 ? 4 : l_list.Count;
		for(int l_i = 0; l_i < l_count; l_i++)
		{
			UIElement l_element = l_canvasList[l_i];
			UIButton l_thisAppButton = l_element as UIButton;
			l_thisAppButton.addClickCallback(onAppButtonClick);
			
			App l_app = l_list[l_i] as App;
			l_app.iconDownload = false;
			_setupSignleApp(l_element,l_app);

			l_element.active = true;
		}
		m_getIconRequestQueue.request (RequestType.SEQUENCE);
	}

	private void _setupSignleApp(UIElement p_element, App p_app)
	{
		UILabel l_appName = p_element.getView ("appNameText") as UILabel;
		l_appName.text = p_app.name;
		UILabel l_appCostText = p_element.getView("appCostText") as UILabel;
		UILabel l_appFreeText = p_element.getView("appFreeText") as UILabel;
		UILabel l_sponsoredText = p_element.getView("sponsoredText") as UILabel;
		UILabel l_subjectsText = p_element.getView ("subjectText") as UILabel;

		l_appFreeText.text = Localization.getString (Localization.TXT_56_LABEL_FREE);
		if( null != l_sponsoredText )
		{
			l_sponsoredText.text = Localization.getString (Localization.TXT_56_LABEL_SPONSORED);
		}
		l_subjectsText.text = Localization.getString (Localization.TXT_56_LABEL_SUBJECTS);

		Token l_token = SessionHandler.getInstance ().token;
		Boolean l_canShowCosts = true;
		if( l_token.isPremium() || l_token.isCurrent() )
		{
			l_canShowCosts = false;
			l_appCostText.active = false;
			l_appFreeText.active = false;
		}

		if(p_app.gems == 0)
		{
			l_appCostText.active = false;
			l_appFreeText.active = true && l_canShowCosts;
			if(null != l_sponsoredText)
				l_sponsoredText.active = true;
		}
		else
		{
			if(p_app.own)
			{
				l_appCostText.active = false;
				l_appFreeText.active = false;
				
				if(null != l_sponsoredText)
					l_sponsoredText.active = false;
			}
			else if(!p_app.own)
			{
				l_appFreeText.active = false;
				if(null != l_sponsoredText)
					l_sponsoredText.active = false;
				l_appCostText.active = true && l_canShowCosts;
				l_appCostText.text = p_app.gems.ToString();
			}
		}

		if(null == p_app.icon)
		{
			if(!p_app.iconDownload)
				downLoadAppIcon(p_element,p_app);
		}
		else
		{
			UIImage l_image = p_element.getView("appImage") as UIImage;
			l_image.setTexture(p_app.icon);
		}
		
		Dictionary< string, int > l_subjects = p_app.subjects;

		if (p_app.id == 8822) 
		{
//			int l_a = 100;
		}
		UIElement l_mathColor = p_element.getView ("mathColor") as UIElement;
		UIElement l_readingColor = p_element.getView ("readingColor") as UIElement;
		UIElement l_scienceColor = p_element.getView ("scienceColor") as UIElement;
		UIElement l_socialColor = p_element.getView ("socialColor") as UIElement;
		UIElement l_cognitiveColor = p_element.getView ("cognitiveColor") as UIElement;
		UIElement l_creativeColor = p_element.getView ("creativeColor") as UIElement;
		UIElement l_lifeSkillsColor = p_element.getView ("lifeSkillsColor") as UIElement;

		if(l_subjects.ContainsKey(AppTable.COLUMN_MATH))
			l_mathColor.active = true;
		else
			l_mathColor.active = false;
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_LANGUAGE))
			l_readingColor.active = true;
		else
			l_readingColor.active = false;
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_SCIENCE))
			l_scienceColor.active = true;
		else
			l_scienceColor.active = false;
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_SOCIAL_SCIENCE))
			l_socialColor.active = true;
		else
			l_socialColor.active = false;
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_CONITIVE_DEVELOPMENT))
			l_cognitiveColor.active = true;
		else
			l_cognitiveColor.active = false;
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_CREATIVE_DEVELOPMENT))
			l_creativeColor.active = true;
		else
			l_creativeColor.active = false;
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_LIFE_SKILLS))
			l_lifeSkillsColor.active = true;
		else
			l_lifeSkillsColor.active = false;
	}

	private void onAppButtonClick( UIButton p_button )
	{






		//m_uiManager.changeScreen (UIScreen.APP_LIST,false);
		m_uiManager.changeScreen (UIScreen.APP_DETAILS,true);

		m_buyedAppElement = (UIElement)p_button;
		UILabel l_appNameLable = p_button.getView ("appNameText") as UILabel;
		string l_name = l_appNameLable.text;
		App l_app = null;
		List<System.Object> l_list = m_currentAppList;
		int l_count = l_list.Count;
		for(int l_i = 0; l_i < l_count; l_i ++)
		{
			App l_currentApp = l_list[l_i] as App;
			if(l_currentApp.name.Equals(l_name))
			{
				l_app = l_currentApp;
				break;
			}
		}
		
		if(null != l_app)
			showAppDetails (l_app);
	}

	private void onAppClick(UISwipeList p_list, UIButton p_listElement, System.Object p_data, int p_index)
	{
		m_buyedAppElement = p_listElement;
		App l_app = (App)p_data;
		switch(p_index)
		{
		case 0:
			m_buyedAppElementInRecommendPage = m_recommendedAppCanvas.getView("appOne") as UIElement;
			break;
		case 1:
			m_buyedAppElementInRecommendPage = m_recommendedAppCanvas.getView("appTwo") as UIElement;
			break;
		case 2:
			m_buyedAppElementInRecommendPage = m_recommendedAppCanvas.getView("appThree") as UIElement;
			break;
		case 3:
			m_buyedAppElementInRecommendPage = m_recommendedAppCanvas.getView("appFour") as UIElement;
			break;
		}
		
		showAppDetails (l_app);
	}

	private void buyApp(UIButton p_button)
	{

		//Auto Add Code
		GoogleInstallAutoAddController.Instance.hasLuanchedGoogle = 1;
		
		List<object> currentAppList = KidMode.getApps();
		
		KidMode.setLastLocalAppInfo();
		//End Auto Add code

		Token l_token = SessionHandler.getInstance ().token;
		if( l_token.isPremium() || l_token.isCurrent() )
		{
			installApp(m_detailsApp.packageName);
		}
		else
		{
			m_uiManager.changeScreen (UIScreen.APP_DETAILS,false);
			m_uiManager.changeScreen (UIScreen.CONFIRM_DIALOG,true);
			m_buyAppButton = p_button;
			confirmBuyApp (m_detailsApp);
		}
	}

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		App l_app = (App)p_data;
		//m_requestQueue.reset ();
		_setupSignleApp (p_element, l_app);
	}

	private void onMoreAppButtonClick( UIButton p_button )
	{
		m_uiManager.changeScreen (UIScreen.APP_LIST,true);
		appListOpen = true;
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_appListCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f);
	}

	private void firstLoadMoreAppList()
	{
		m_prototype.active = true;
		if(null == m_appList)
			m_appList = m_appListCanvas.getView ("appSwipeList") as UISwipeList;
		if(null != m_currentAppList)
		{
			m_appList.setData( m_currentAppList );
			m_appList.setDrawFunction( onListDraw );
			m_appList.redraw();
			//if(m_getIconRequestQueue.isCompleted())
				m_getIconRequestQueue.request(RequestType.SEQUENCE);
			m_appList.addValueChangeListener(onListToEnd);
		}
		m_appList.addClickListener ("Prototype",onAppClick);

		m_isLoaded = true;
	}

	private void onListToEnd(Vector2 p_value)
	{
		if( p_value.y <= 0 )
		{
			m_appList.removeValueChangeListener(onListToEnd);

			if(m_currentAppList.Count % 10 == 0 && m_getAppRequestQueue.Completed())
			{
				m_currentPage++;
				m_getAppRequestQueue.reset ();
				m_getAppRequestQueue.add ( new GetAppByPageRequest(SessionHandler.getInstance().currentKid.age,"google",m_currentAppList.Count/10 + 1,getAppListComplete));
				m_getAppRequestQueue.request ();
			}
			else
			{
				m_appList.addValueChangeListener(onListToEnd);
			}
		}
	}

	private void getAppListComplete(WWW p_response)
	{
		int l_dataCount = 0;
		if(null == p_response.error)
		{
			string l_string = UnicodeDecoder.Unicode(p_response.text);
			l_string = UnicodeDecoder.UnicodeToChinese(l_string);
			ArrayList l_data = MiniJSON.MiniJSON.jsonDecode (l_string) as ArrayList;
		    l_dataCount = l_data.Count;
			Hashtable l_appOwn = SessionHandler.getInstance ().appOwn;
			for(int l_i = 0; l_i < l_dataCount; l_i++)
			{
				Hashtable l_table = l_data[l_i] as Hashtable;
				App l_app = new App(l_table);
				if(null != l_table)
				{
					l_app.own = l_appOwn.ContainsKey(l_app.id.ToString());
				}
				m_currentAppList.Add(l_app);
			}
			//m_appList.setData( m_currentAppList );
			m_appList.redraw();
			if(m_getIconRequestQueue.isCompleted())
				m_getIconRequestQueue.request(RequestType.SEQUENCE);
		}
		if(l_dataCount >= 10)
			m_appList.addValueChangeListener(onListToEnd);
	}

	private void onExitAppListButtonClick( UIButton p_button )
	{
		m_uiManager.changeScreen (UIScreen.APP_LIST,false);
		appListOpen = false;
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_currentPanel = m_appListCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_currentPanel.transform.localPosition );
		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	private void onExitAppDetailsButtonClick( UIButton p_button )
	{
		//m_uiManager.changeScreen (UIScreen.APP_LIST,true);
		m_uiManager.changeScreen (UIScreen.APP_DETAILS,false);
		if (appListOpen) 
			m_uiManager.changeScreen (UIScreen.APP_LIST,true);
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_currentPanel = m_appDetailsCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_currentPanel.transform.localPosition );
		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	private void showAppDetails(App p_app)
	{
		m_uiManager.changeScreen (UIScreen.APP_LIST,false);
		m_uiManager.changeScreen (UIScreen.APP_DETAILS,true);
		m_detailsApp = p_app;
		UILabel l_appCostText = m_appDetailsCanvas.getView("appCostText") as UILabel;
		UIButton l_buyAppButton = m_appDetailsCanvas.getView("buyAppButton") as UIButton;
		UILabel l_appFreeText = m_appDetailsCanvas.getView("appFreeText") as UILabel;
		
		if(p_app.gems == 0)
		{
			l_appCostText.active = false;
			l_buyAppButton.active = false;
			l_appFreeText.active = true;
		}
		else 
		{
			if(p_app.own)
			{
				l_appCostText.active = false;
				l_buyAppButton.active = false;
				l_appFreeText.active = false;
			}
			else if(!p_app.own)
			{
				l_appFreeText.active = false;
				l_appCostText.active = true;
				l_buyAppButton.active = true;
			}
		}

		Token l_token = SessionHandler.getInstance ().token;
		UILabel l_text = l_buyAppButton.getView( "Text" ) as UILabel;
		if( l_token.isPremium() || l_token.isCurrent() )
		{
			l_appCostText.active = false;
			l_appFreeText.active = false;
			l_buyAppButton.active = true;
			l_text.text = Localization.getString (Localization.TXT_70_LABEL_INSTALL);
		}
		else
		{
			l_text.text = Localization.getString (Localization.TXT_70_LABEL_BUY);
		}
		
		resetSubjectColor ();
		_setupSignleApp (m_appDetailsCanvas,p_app);
		
		UILabel l_description = m_appDetailsCanvas.getView("appDescriptionText") as UILabel;
		UILabel l_violence = m_appDetailsCanvas.getView("violenceLevelText") as UILabel;
		UILabel l_age = m_appDetailsCanvas.getView("ageText") as UILabel;
		UIImage l_icon = m_appDetailsCanvas.getView ("appImage") as UIImage;
		l_buyAppButton.removeClickCallback (buyApp);
		l_buyAppButton.addClickCallback (buyApp);
		
		l_description.text = p_app.description;
		l_violence.text = p_app.violence.ToString();
		l_age.text = p_app.ageMin.ToString() +"-"+ p_app.ageMax.ToString();
		if(null != p_app.icon)
		{
			l_icon.setTexture(p_app.icon);
		}
		
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_appDetailsCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
	}

	private void downLoadAppIcon(UIElement p_element, App p_app)
	{
		RequestQueue l_queue = new RequestQueue();
		l_queue.add(new IconRequest(p_app,p_element));
		l_queue.request(RequestType.SEQUENCE);
		m_iconRequests.Add(l_queue);

		//m_getIconRequestQueue.add (new IconRequest(p_app,p_element));
		p_app.iconDownload = true;
	}

	private void resetSubjectColor()
	{
		UIElement l_readingColor = m_appDetailsCanvas.getView ("readingColor") as UIElement;
		l_readingColor.active = false;
		
		UIElement l_scienceColor = m_appDetailsCanvas.getView ("scienceColor") as UIElement;
		l_scienceColor.active = false;
		
		UIElement l_socialColor = m_appDetailsCanvas.getView ("socialColor") as UIElement;
		l_socialColor.active = false;
		
		UIElement l_cognitiveColor = m_appDetailsCanvas.getView ("cognitiveColor") as UIElement;
		l_cognitiveColor.active = false;
		
		UIElement l_creativeColor = m_appDetailsCanvas.getView ("creativeColor") as UIElement;
		l_creativeColor.active = false;
		
		UIElement l_lifeSkillsColor = m_appDetailsCanvas.getView ("lifeSkillsColor") as UIElement;
		l_lifeSkillsColor.active = false;
		
		UIElement l_mathColor = m_appDetailsCanvas.getView ("mathColor") as UIElement;
		l_mathColor.active = false;
	}
	
	private void loadAppList()
	{
		m_getAppRequestQueue.reset();
		//m_getAppRequestQueue.add(new GetAppOwnRequest());
		m_getAppRequestQueue.add(new NewGetAppByPageRequest(SessionHandler.getInstance().currentKid,"google",m_currentPage,firstGetAppListComplete));
		m_getAppRequestQueue.request(RequestType.SEQUENCE);
		
		isLoadApp = true;
	}

	private void loadAppListImmediate()
	{
		isLoadApp = true;
		firstLoadMoreAppList ();
	}

	private void firstGetAppListComplete(WWW p_response)
	{
		if(null == p_response.error)
		{
			string l_string = UnicodeDecoder.Unicode(p_response.text);
			l_string = UnicodeDecoder.UnicodeToChinese(l_string);
			Hashtable l_jsonResponse = MiniJSON.MiniJSON.jsonDecode(l_string) as Hashtable;
			if(l_jsonResponse.ContainsKey("jsonResponse"))
			{
				Hashtable l_response = l_jsonResponse["jsonResponse"] as Hashtable;
				if(l_response.ContainsKey("response"))
				{
					Hashtable l_result = l_response["response"] as Hashtable;
					if(l_result.ContainsKey("app_owned"))
					{
						SessionHandler.getInstance().appOwn = l_result["app_owned"] as Hashtable;
					}
					if(l_result.ContainsKey("apps"))
					{
						ArrayList l_data = l_result["apps"] as ArrayList;
						int l_dataCount = l_data.Count;
						Hashtable l_appOwn = SessionHandler.getInstance ().appOwn;
						for(int l_i = 0; l_i < l_dataCount; l_i++)
						{
							Hashtable l_table = l_data[l_i] as Hashtable;
							App l_app = new App(l_table);
							if(null != l_table)
							{
								l_app.own = l_appOwn.ContainsKey(l_app.id.ToString());
							}
							m_currentAppList.Add(l_app);
						}
						firstLoadMoreAppList();
					}
				}
			}
		}
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

	
	private DashBoardControllerCanvas m_dashboardControllerCanvas;
	private UICanvas				  m_dashboardCommonCanvas;
	private LeftMenuCanvas 			  m_leftMenuCanvas;
	private CommonDialogCanvas 		  m_commonDialog;
	private UICanvas				  m_recommendedAppCanvas;
	private UICanvas				  m_appDetailsCanvas;
	private UICanvas				  m_appListCanvas;
	private UICanvas				  m_confirmDialogCanvas;
//	private UILabel 			      m_gemCountLabel;
	private UIButton 				  m_moreAppButton;
	private UIButton 				  m_exitAppListButton;
	private UIManager 				  m_uiManager;
	private UISwipeList				  m_appList;
	private UIButton 				  m_exitAppDetailsButton;
	private UIElement				  m_buyedAppElement;
	private UIElement				  m_buyedAppElementInRecommendPage;
	private UIElement				  m_prototype;
	private bool 					  isLoadApp = false;
	private App 					  m_detailsApp;
	private UIButton				  m_leftButton;
	private UIButton				  m_rightButton;
	private UIElement 				  m_menu;
	private bool 					  canMoveLeftMenu = true;
	private bool 					  appListOpen = false;
	private UILabel					  m_appCountLabel;
	private bool					  m_isLoaded = false;

	private UIElement  				  m_costArea;
	private UIButton 			      m_buyGemsButton;
	private UIButton 			      m_buyGemsButtonOnConfirm;
	private UIElement 				  m_needMoreArea;
	private UIButton 				  m_exitConfirmDialogButton;
	private UIButton 			      m_cancelBuyButton;
	private UIButton 				  m_confirmBuyButton;
	private UIButton 				  m_helpButton;
	private int 					  m_currentPage;
	private List<object> 			  m_currentAppList;

	private UIButton 				  m_leftSideMenuButton;
	private UIButton    			  m_showProfileButton;
	private UIButton 				  m_closeLeftMenuButton;
	private UIButton 				  m_childModeButton;
	private UIButton 				  m_settingButton;
	private UISwipeList				  m_childrenList;
	private UIButton 				  m_tryPremiumButton;

	private UIButton m_appsButton;
	private UIButton				  m_overviewButton;
	private UIButton 				  m_controlsButton;
	private UIButton				  m_statChartButton;
	private UIButton				  m_buyAppButton;

	private RequestQueue 			  m_requestQueue;
	private RequestQueue 			  m_getAppRequestQueue;
	private RequestQueue 			  m_getIconRequestQueue;

	private List<RequestQueue>			m_iconRequests;
}