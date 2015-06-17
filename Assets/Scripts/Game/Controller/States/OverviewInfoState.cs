using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverviewInfoState : GameState {

	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);
		canMoveLeftMenu = true;
		m_uiManager = m_gameController.getUI();
		m_requestQueue = new RequestQueue ();
		UICanvas l_backScreen = m_uiManager.findScreen( UIScreen.SPLASH_BACKGROUND );
		if (l_backScreen != null)
		{
			m_uiManager.removeScreenImmediately(UIScreen.SPLASH_BACKGROUND);
		}
		m_app = SessionHandler.getInstance ().currentKid.topRecommendedApp;
		canLoadTopRecommandApp = true;
		m_getTopRecommendAppRequestQueue = new RequestQueue ();
		_setupScreen( p_gameController );
		_setupElment();
		turnOffChildLock();
	}

	private void turnOffChildLock()
	{
//		KidMode.setKidsModeActive(false);	
		KidModeLockController.Instance.swith2DParentMode();
	}
	//private float l_time = 0.0f;
	public override void update (GameController p_gameController, int p_time)
	{
		base.update (p_gameController, p_time);
		//l_time += Time.deltaTime;
		if(null != SessionHandler.getInstance().currentKid.topRecommendedApp && canLoadTopRecommandApp)
		{
			canLoadTopRecommandApp = false;
			m_app = SessionHandler.getInstance().currentKid.topRecommendedApp;
			if( 0 != m_app.id )
			{
				m_topRecommendAppArea.active = true;
				_setupSignleApp(m_topRecommendAppArea,m_app);
				loadAppDetail();
			}
			else
			{
				m_topRecommendAppArea.active = false;
				
				UILabel l_emptyText = m_dashboardInfoCanvas.getView("emptyText") as UILabel;
				l_emptyText.text = Localization.getString(Localization.TXT_STATE_50_EMPTY);
			}

		}
	}

	public override void exit (GameController p_gameController)
	{
		base.exit (p_gameController);
		m_requestQueue.dispose ();
		m_uiManager.removeScreen( UIScreen.DASHBOARD_CONTROLLER );
		m_uiManager.removeScreen( UIScreen.DASHBOARD_COMMON );
		m_uiManager.removeScreen( UIScreen.LEFT_MENU );
		m_uiManager.removeScreen( UIScreen.DASHBOARD_INFO );
		m_uiManager.removeScreen( UIScreen.CONFIRM_DIALOG );
		m_uiManager.removeScreen( UIScreen.APP_DETAILS );
	}

	//----------------- Private Implementation -------------------

	private void _setupScreen( GameController p_gameController )
	{
		m_confirmDialogCanvas 	= m_uiManager.createScreen( UIScreen.CONFIRM_DIALOG, false, 15 );

		m_appDetailsCanvas 	= m_uiManager.createScreen( UIScreen.APP_DETAILS, false, 14 );

		m_leftMenuCanvas = m_uiManager.createScreen( UIScreen.LEFT_MENU, true, 3 )  as LeftMenuCanvas;
		
		m_dashboardControllerCanvas = m_uiManager.createScreen( UIScreen.DASHBOARD_CONTROLLER, false, 2 ) as DashBoardControllerCanvas;

		m_dashboardInfoCanvas 	= m_uiManager.createScreen( UIScreen.DASHBOARD_INFO, true, 1 );

		m_dashboardCommonCanvas = m_uiManager.createScreen( UIScreen.DASHBOARD_COMMON, true, 0 );
	}

	private void _setupElment()
	{
		m_leftButton = m_dashboardControllerCanvas.getView( "leftButton" ) as UIButton;
		m_leftButton.addClickCallback( onLeftButtonClick );
		
		m_rightButton = m_dashboardControllerCanvas.getView( "rightButton" ) as UIButton;
		m_rightButton.addClickCallback( onRightButtonClick );

		m_exitAppDetailsButton = m_appDetailsCanvas.getView( "exitButton" ) as UIButton;
		m_exitAppDetailsButton.addClickCallback( onExitAppDetailsButtonClick );

		m_editProfileButton = m_dashboardInfoCanvas.getView ("editProfileButton") as UIButton;
		m_editProfileButton.addClickCallback (editProfile);

		m_dashboardControllerCanvas.setupDotList( 7 );
		m_dashboardControllerCanvas.setCurrentIndex( 0 );

		UIElement l_newPanel = m_dashboardInfoCanvas.getView ("mainPanel");
		List<Vector3> l_pointListIn = new List<Vector3>();
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 830, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, 0.5f);
		
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
		m_overviewButton.enabled = false;
		
		m_controlsButton = m_dashboardCommonCanvas.getView ("controlButton") as UIButton;
		m_controlsButton.addClickCallback (goToControls);
		
		m_statChartButton = m_dashboardCommonCanvas.getView ("starButton") as UIButton;
		m_statChartButton.addClickCallback (goToStarChart);

		m_childrenList = m_leftMenuCanvas.getView ("childSwipeList") as UISwipeList;
		m_childrenList.addClickListener ("Prototype",onSelectThisChild);

		m_tryPremiumButton = m_leftMenuCanvas.getView ("premiumButton") as UIButton;
		m_buyGemsButtonOnLeftMenu = m_leftMenuCanvas.getView ("buyGemsButton") as UIButton;
		m_tryPremiumButton.addClickCallback (toPremiumScreen);
		m_buyGemsButtonOnLeftMenu.addClickCallback (toBuyGemsScreen);

		m_costArea = m_confirmDialogCanvas.getView("costArea");
		m_needMoreArea = m_confirmDialogCanvas.getView("needMoreArea");
		m_exitConfirmDialogButton = m_confirmDialogCanvas.getView ("exitButton") as UIButton;
		m_exitConfirmDialogButton.addClickCallback (onExitConfiemDialogButtonClick);
		m_cancelBuyButton = m_confirmDialogCanvas.getView ("cancelButton") as UIButton;
		m_cancelBuyButton.addClickCallback (onExitConfiemDialogButtonClick);
		m_confirmBuyButton = m_confirmDialogCanvas.getView ("confirmButton") as UIButton;
		m_confirmBuyButton.addClickCallback (onConfiemButtonClick);
		m_buyGemsButtonOnConfirm = m_confirmDialogCanvas.getView ("buyGemsButton") as UIButton;
		m_buyGemsButtonOnConfirm.addClickCallback (toBuyGemsScreen);

		m_topRecommendAppArea = m_dashboardInfoCanvas.getView ("app") as UIButton;
		m_topRecommendAppArea.addClickCallback (showAppDetail);

		if (null == m_app)
		{
			m_topRecommendAppArea.active = false;

			UILabel l_emptyText = m_dashboardInfoCanvas.getView("emptyText") as UILabel;
			l_emptyText.text = Localization.getString(Localization.TXT_LABEL_LOADING);
			if(SessionHandler.getInstance().appRequest.isCompleted())
			{
				m_getTopRecommendAppRequestQueue.reset();
				m_getTopRecommendAppRequestQueue.add(new GetTopRecommandRequest(ZoodlesConstants.GOOGLE,SessionHandler.getInstance().currentKid,getTopRecommendRequestComplete));
				m_getTopRecommendAppRequestQueue.request(RequestType.SEQUENCE);
			}
		}

	}

	private void editProfile(UIButton p_button)
	{
		SessionHandler.getInstance ().inputedChildName = SessionHandler.getInstance ().currentKid.name;
		SessionHandler.getInstance ().inputedbirthday = SessionHandler.getInstance ().currentKid.birthday;
		SessionHandler.getInstance ().selectAvatar = string.Empty;
		SessionHandler.getInstance ().CreateChild = false;
		m_gameController.changeState (ZoodleState.CREATE_CHILD);
	}

	private void onConfiemButtonClick( UIButton p_button )
	{
		if(null != m_app)
		{
			if(SessionHandler.getInstance().currentKid.gems >= m_app.gems)
				sendBuyAppRequest ();
			else
			{
				UILabel l_costGems = m_confirmDialogCanvas.getView("costPriceText") as UILabel;
				UILabel l_needGems = m_confirmDialogCanvas.getView("needPriceText") as UILabel;
				l_costGems.text = m_app.gems.ToString ();
				l_needGems.text = (m_app.gems - SessionHandler.getInstance().currentKid.gems).ToString ();
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
		m_requestQueue.reset ();
		m_requestQueue.add (new BuyRecommendAppRequest(m_app,onBuyAppComplete));
		m_requestQueue.request ();
	}

	private void removeAppPriceAndBuyButton(UIElement p_element)
	{
		if(null != p_element)
		{
			UILabel l_costLabel = p_element.getView("appCostText") as UILabel;
			UILabel l_freeLabel = p_element.getView("appFreeText") as UILabel;
			UILabel l_sponsoredLabel = p_element.getView("sponsoredText") as UILabel;
			
			if(null != l_costLabel)
				l_costLabel.active = false;

			if(null != l_freeLabel)
				l_freeLabel.active = false;

			if(null != l_sponsoredLabel)
				l_sponsoredLabel.active = false;
		}
	}

	private void onBuyAppComplete(WWW p_response)
	{
		if(null == p_response.error)
		{
			m_app.own = true;
			UILabel l_appCostText = m_appDetailsCanvas.getView("appCostText") as UILabel;
			UIButton l_buyAppButton = m_appDetailsCanvas.getView("buyAppButton") as UIButton;
			l_appCostText.active = false;
			l_buyAppButton.active = false;
			removeAppPriceAndBuyButton(m_appDetailsCanvas);
			removeAppPriceAndBuyButton(m_topRecommendAppArea);
			if(null != m_confirmDialogCanvas)
			{
				m_uiManager.changeScreen(UIScreen.CONFIRM_DIALOG,false);
				m_uiManager.changeScreen(UIScreen.APP_DETAILS,true);
				List<Vector3> l_pointListOut = new List<Vector3>();
				UIElement l_currentPanel = m_confirmDialogCanvas.getView ("mainPanel");
				l_pointListOut.Add( l_currentPanel.transform.localPosition );
				l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
				l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );
			}

			installApp( SessionHandler.getInstance().currentKid.topRecommendedApp.packageName );
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

	private void loadAppDetail()
	{
		if(null == m_appDetailsCanvas)
		{
			return;
		}
		UILabel l_appCostText = m_appDetailsCanvas.getView("appCostText") as UILabel;
		UIButton l_buyAppButton = m_appDetailsCanvas.getView("buyAppButton") as UIButton;
		UILabel l_appFreeText = m_appDetailsCanvas.getView("appFreeText") as UILabel;
		
		if(m_app.gems == 0)
		{
			l_appCostText.active = false;
			l_buyAppButton.active = false;
			l_appFreeText.active = true;
		}
		else
		{
			if(m_app.own)
			{
				l_appCostText.active = false;
				l_buyAppButton.active = false;
				l_appFreeText.active = false;
			}
			else if(!m_app.own)
			{
				l_appFreeText.active = false;
				l_appCostText.active = true;
				l_buyAppButton.active = true;
			}
		}
		
		Token l_token = SessionHandler.getInstance ().token;
		if( l_token.isPremium() || l_token.isCurrent() )
		{
			l_appCostText.active = false;
			l_appFreeText.active = false;
			l_buyAppButton.active = true;
			UILabel l_text = l_buyAppButton.getView( "Text" ) as UILabel;
			l_text.text = Localization.getString(Localization.TXT_STATE_45_INSTALL);
		}
		
		resetSubjectColor ();
		_setupSignleApp (m_appDetailsCanvas,m_app);
		
		UILabel l_description = m_appDetailsCanvas.getView("appDescriptionText") as UILabel;
		UILabel l_violence = m_appDetailsCanvas.getView("violenceLevelText") as UILabel;
		UILabel l_age = m_appDetailsCanvas.getView("ageText") as UILabel;
		UIImage l_icon = m_appDetailsCanvas.getView ("appImage") as UIImage;
		UIButton l_buyButton = m_appDetailsCanvas.getView ("buyAppButton") as UIButton;
		l_buyButton.removeClickCallback (buyApp);
		l_buyButton.addClickCallback (buyApp);
		
		l_description.text = m_app.description;
		l_violence.text = m_app.violence.ToString();
		l_age.text = m_app.ageMin.ToString() +"-"+ m_app.ageMax.ToString();
		if(null != m_app.icon)
		{
			l_icon.setTexture(m_app.icon);
		}
	}

	private void showAppDetail(UIButton p_button)
	{
		m_uiManager.changeScreen (UIScreen.APP_DETAILS,true);
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_appDetailsCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
	}

	private void buyApp(UIButton p_button)
	{
		Token l_token = SessionHandler.getInstance ().token;
		if( l_token.isPremium() || l_token.isCurrent() )
		{
			installApp(m_app.packageName);
		}
		else
		{
			confirmBuyApp (m_app);
		}
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

	public void confirmBuyApp(App p_app)
	{
		m_costArea.active = true;
		m_needMoreArea.active = false;
		m_uiManager.changeScreen (UIScreen.APP_DETAILS,false);
		m_uiManager.changeScreen (UIScreen.CONFIRM_DIALOG,true);
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

	private void iconRequestComplete(WWW p_response)
	{
		if(null != p_response.error)
		{
			_Debug.log(p_response);
		}
		else
		{
			App l_app = SessionHandler.getInstance().currentKid.topRecommendedApp;
			l_app.iconDownload = true;
			l_app.icon = p_response.texture;
			if(null != m_topRecommendAppArea)
			{
				UIImage l_image = m_topRecommendAppArea.getView("appImage") as UIImage;
				
				if( null == l_image )
				{
					return;
				}
				
				l_image.setTexture(p_response.texture);
			}

			if(null != m_appDetailsCanvas)
			{
				UIImage l_image = m_appDetailsCanvas.getView("appImage") as UIImage;
				
				if( null == l_image )
				{
					return;
				}
				
				l_image.setTexture(p_response.texture);
			}

		}
	}

	private void _setupSignleApp(UIElement p_element, App p_app)
	{
		UILabel l_appName = p_element.getView ("appNameText") as UILabel;
		if(null != l_appName)
			l_appName.text = p_app.name;
		UILabel l_appCostText = p_element.getView("appCostText") as UILabel;
		UILabel l_appFreeText = p_element.getView("appFreeText") as UILabel;
		UILabel l_sponsoredText = p_element.getView("sponsoredText") as UILabel;
		if(null == p_app.icon)
		{
			m_getTopRecommendAppRequestQueue.reset();
			m_getTopRecommendAppRequestQueue.add(new IconRequest(p_app,p_element,iconRequestComplete));
			m_getTopRecommendAppRequestQueue.request();
		}
		else
		{
			UIImage l_image = p_element.getView("appImage") as UIImage;
			
			if( null == l_image )
			{
				return;
			}
			
			l_image.setTexture(p_app.icon);
		}
		if(p_app.gems == 0)
		{
			if(null != l_appCostText)
				l_appCostText.active = false;
			if(null != l_appFreeText)
				l_appFreeText.active = true;
			if(null != l_sponsoredText)
				l_sponsoredText.active = true;
		}
		else
		{
			if(p_app.own)
			{
				if(null != l_appCostText)
					l_appCostText.active = false;
				if(null != l_appFreeText)
					l_appFreeText.active = false;
				if(null != l_sponsoredText)
					l_sponsoredText.active = false;
			}
			else if(!p_app.own)
			{
				if(null != l_appFreeText)
					l_appFreeText.active = false;
				if(null != l_sponsoredText)
					l_sponsoredText.active = false;
				if(null != l_appCostText)
				{
					l_appCostText.active = true;
					l_appCostText.text = p_app.gems.ToString();
				}
			}
		}
		
		Token l_token = SessionHandler.getInstance ().token;
		if( l_token.isPremium() || l_token.isCurrent() )
		{
			if(null != l_appCostText)
				l_appCostText.active = false;
			if(null != l_appFreeText)
				l_appFreeText.active = false;
		}

		Dictionary< string, int > l_subjects = p_app.subjects;
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_MATH))
		{
			UIElement l_mathColor = p_element.getView ("mathColor") as UIElement;
			if(null != l_mathColor)
			l_mathColor.active = true;
		}
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_LANGUAGE))
		{
			UIElement l_readingColor = p_element.getView ("readingColor") as UIElement;
			if(null != l_readingColor)
			l_readingColor.active = true;
		}
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_SCIENCE))
		{
			UIElement l_scienceColor = p_element.getView ("scienceColor") as UIElement;
			if(null != l_scienceColor)
				l_scienceColor.active = true;
		}
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_SOCIAL_SCIENCE))
		{
			UIElement l_socialColor = p_element.getView ("socialColor") as UIElement;
			if(null != l_socialColor)
			l_socialColor.active = true;
		}
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_CONITIVE_DEVELOPMENT))
		{
			UIElement l_cognitiveColor = p_element.getView ("cognitiveColor") as UIElement;
			if(null != l_cognitiveColor)
			l_cognitiveColor.active = true;
		}
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_CREATIVE_DEVELOPMENT))
		{
			UIElement l_creativeColor = p_element.getView ("creativeColor") as UIElement;
			if(null != l_creativeColor)
			l_creativeColor.active = true;
			
		}
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_LIFE_SKILLS))
		{
			UIElement l_lifeSkillsColor = p_element.getView ("lifeSkillsColor") as UIElement;
			if(null != l_lifeSkillsColor)
			l_lifeSkillsColor.active = true;
		}

		if(null != p_element && null != p_element.gameObject)
			p_element.active = true;
	}

	private void getTopRecommendRequestComplete(WWW p_response)
	{
		if(null == p_response.error && !"null".Equals(p_response.text))
		{
			string l_string = UnicodeDecoder.Unicode(p_response.text);
			l_string = UnicodeDecoder.UnicodeToChinese(l_string);
			Hashtable l_hashTable = MiniJSON.MiniJSON.jsonDecode(l_string) as Hashtable;
			App l_app = new App(l_hashTable);
			if(l_hashTable.ContainsKey("owned"))
			{
				l_app.own = (bool)l_hashTable["owned"];
			}
			SessionHandler.getInstance().currentKid.topRecommendedApp = l_app;
			m_app = l_app;
		}
	}

	private void onExitAppDetailsButtonClick( UIButton p_button )
	{
		m_uiManager.changeScreen (UIScreen.APP_DETAILS,false);
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_currentPanel = m_appDetailsCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_currentPanel.transform.localPosition );
		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	private void onLeftButtonClick( UIButton p_button )
	{
		return;
	}
	
	private void onRightButtonClick( UIButton p_button )
	{
		m_gameController.changeState( ZoodleState.OVERVIEW_TIMESPENT );
	}
	
	private void goToControls( UIButton p_button )
	{
		m_gameController.changeState (ZoodleState.CONTROL_SUBJECT);
	}
	
	private void goToStarChart( UIButton p_button )
	{
		m_gameController.changeState (ZoodleState.DASHBOARD_STAR_CHART);
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
	
	private void onCloseMenuTweenFinished( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		canMoveLeftMenu = true;
	}
	
	private void toShowMenuTweenFinished( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		canMoveLeftMenu = true;
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
	
	private void toShowAllChilren(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		m_leftMenuCanvas.showKids (addButtonClickCall);
	}
	
	private void addButtonClickCall( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_showProfileButton.addClickCallback (toShowAllChilren);
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

	private UIManager m_uiManager;
	private App m_app;

	private DashBoardControllerCanvas m_dashboardControllerCanvas;
	private UICanvas m_dashboardInfoCanvas;
	private UICanvas m_dashboardCommonCanvas;
	private UICanvas m_appDetailsCanvas;
	private LeftMenuCanvas m_leftMenuCanvas;
	private UICanvas	m_confirmDialogCanvas;

	private UIElement m_costArea;
	private UIButton m_buyGemsButton;
	private UIButton m_buyGemsButtonOnConfirm;
	private UIElement m_needMoreArea;
	private UIButton m_exitConfirmDialogButton;
	private UIButton m_cancelBuyButton;
	private UIButton m_confirmBuyButton;

	private UIButton m_leftButton;
	private UIButton m_rightButton;
	private UIButton m_leftSideMenuButton;
	private UIButton m_showProfileButton;
	private UIButton m_closeLeftMenuButton;
	private UIButton m_childModeButton;
	private UIButton m_settingButton;
	private UIButton m_tryPremiumButton;
	private UIButton m_buyGemsButtonOnLeftMenu;
	private UIButton m_exitAppDetailsButton;

	private UIButton m_overviewButton;
	private UIButton m_controlsButton;
	private UIButton m_statChartButton;
	private UIButton m_editProfileButton;
	
	private UISwipeList m_childrenList;
	private UIElement 	m_menu;
	private UIButton	m_topRecommendAppArea;
	private RequestQueue m_requestQueue;
	private RequestQueue m_getTopRecommendAppRequestQueue;

	private bool 		canMoveLeftMenu;
	private bool 		canLoadTopRecommandApp;
}
