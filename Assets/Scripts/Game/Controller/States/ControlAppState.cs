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

		m_currentRecommendedAppPage = 1;
		m_getRecommendedAppRequestQueue = new RequestQueue();
		m_recommendedAppIconRequests = new List<RequestQueue>();
		m_currentRecommendedAppList = SessionHandler.getInstance().currentKid.appList == null? new List<object>():SessionHandler.getInstance().currentKid.appList;

		_setupScreen( p_gameController );
		_setupElment();

		if(TutorialController.Instance != null)
			TutorialController.Instance.showNextPage();

		SwrveComponent.Instance.SDK.NamedEvent("Parent_Dashboard.start");

		GoogleInstallAutoAddController.OnNewAppAdded += OnNewAppAdded;
}
	
	public override void update (GameController p_gameController, int p_time)
	{
		base.update (p_gameController, p_time);
	}
	
	public override void exit (GameController p_gameController)
	{
		checkRequest();
		
		base.exit(p_gameController);
		disposeIconRequests();

		m_uiManager.removeScreenImmediately( UIScreen.ADD_APPS );
		m_uiManager.removeScreenImmediately( UIScreen.APP_DETAILS );
		m_uiManager.removeScreenImmediately( UIScreen.CONFIRM_DIALOG );
//		m_uiManager.removeScreenImmediately( UIScreen.PAYWALL );

		GoogleInstallAutoAddController.OnNewAppAdded -= OnNewAppAdded;
	}

	private void _setupScreen( GameController p_gameController )
	{
//		if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
//		{
//			m_paywallCanvas = m_uiManager.createScreen( UIScreen.PAYWALL, false, 2 );
//			m_upgradeButton = m_paywallCanvas.getView( "upgradeButton" ) as UIButton;
//			m_upgradeButton.addClickCallback( onUpgradeButtonClick );
//		}

		m_commonDialog = m_uiManager.createScreen( UIScreen.COMMON_DIALOG, true, 16 )  as CommonDialogCanvas;
		m_commonDialog.setUIManager (p_gameController.getUI());
		m_addAppCanvas = m_uiManager.createScreen( UIScreen.ADD_APPS, true, 1 ) as AddAppCanvas;
		m_recommendedAppDetailsCanvas = m_uiManager.createScreen( UIScreen.APP_DETAILS, false, 14 );
		m_confirmDialogCanvas = m_uiManager.createScreen( UIScreen.CONFIRM_DIALOG, false, 15 );

//		if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
//		{
//			m_uiManager.setScreenEnable( UIScreen.ADD_APPS, false );
//		}
	}

	private void _setupElment()
	{
		m_helpButton = m_addAppCanvas.getView ("helpButton") as UIButton;
		m_helpButton.addClickCallback (onHelpButtonClick);

		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_addAppCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 830, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, 0.5f, onShowFinish, Tweener.Style.Standard, false);

		//local app part
		m_appSwipeList = m_addAppCanvas.getView ( "appSwipeList" ) as UISwipeList;
		m_appSwipeList.addClickListener ( "controlButton", m_addAppCanvas.onButtonClicked );
		m_appSwipeList.addClickListener ( "controlButton", onAppButtonClicked );
		m_appSwipeList.active = false;

		//recommended app part
		//Create an empty list for set up swipeList.
		m_recommendedAppSwipeList = m_addAppCanvas.getView ( "RecommendedAppSwipeList" ) as UISwipeList;
		m_recommendedAppPrototype = m_recommendedAppSwipeList.getView ("Prototype");
		m_recommendedAppPrototype.active = false;
		m_recommendedAppTitle = m_addAppCanvas.getView("RecommandedAppPanel").getView("Title") as UILabel;

		List<System.Object> l_list = new List<System.Object>();
		m_recommendedAppSwipeList.setData (l_list);
		if((null == m_currentRecommendedAppList || m_currentRecommendedAppList.Count == 0) && SessionHandler.getInstance().appRequest.isCompleted())
			loadAppList ();
		else
			loadAppListImmediate();

		//recommended app detail part
		m_exitAppDetailsButton = m_recommendedAppDetailsCanvas.getView( "exitButton" ) as UIButton;
		m_exitAppDetailsButton.addClickCallback( onExitAppDetailsButtonClick );

		//cofirm dialog to buy or install a recommended app
		m_costArea = m_confirmDialogCanvas.getView("costArea");
		m_needMoreArea = m_confirmDialogCanvas.getView("needMoreArea");
		m_exitConfirmDialogButton = m_confirmDialogCanvas.getView ("exitButton") as UIButton;
		m_exitConfirmDialogButton.addClickCallback (onExitConfiemDialogButtonClick);
		m_cancelBuyButton = m_confirmDialogCanvas.getView ("cancelButton") as UIButton;
		m_cancelBuyButton.addClickCallback (onExitConfiemDialogButtonClick);
		m_confirmBuyButton = m_confirmDialogCanvas.getView ("confirmButton") as UIButton;
		m_confirmBuyButton.addClickCallback (onConfiemButtonClick);
	}

	private void checkRequest()
	{
		if (checkInternet() == false)
			return;

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

	private void onAppButtonClicked(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
//		p_list.removeClickListener ( "controlButton", onAppButtonClicked );
		if(TutorialController.Instance != null)
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
			Dictionary<string,string> payload = new Dictionary<string,string>() { {"AppName", l_appInfo.appName}};
			SwrveComponent.Instance.SDK.NamedEvent("AddApps",payload);
		}
		_Debug.log ( MiniJSON.MiniJSON.jsonEncode(l_appNameList) );
		
		PlayerPrefs.SetString( "addedAppList", MiniJSON.MiniJSON.jsonEncode(l_appNameList) );
	}

	private void onShowFinish(UIElement p_element, Tweener.TargetVar p_target)
	{
		try 
		{
			List<object> lastLocalAppsList = KidMode.getLastLocalApps();
			Debug.LogWarning("      ****************************   lastLocalAppsList " + lastLocalAppsList.Count);
			m_addAppCanvas.firstLoadApp();
		}
		catch (System.Exception e)
		{

		}
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

	private void OnNewAppAdded(){

		m_addAppCanvas.firstLoadApp();

	}
	
	private void viewPremiumRequestComplete(HttpsWWW p_response)
	{
		Server.init (ZoodlesConstants.getHttpsHost());
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

	private bool checkInternet()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable 
		    || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected())
		{
			m_gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
			return false;
		}
		return true;
	}

	//Begin of Recommended Apps---------------------------------------------------------------------------//
	private void loadAppList()
	{
		m_getRecommendedAppRequestQueue.reset();
		m_getRecommendedAppRequestQueue.add(
			new NewGetAppByPageRequest(SessionHandler.getInstance().currentKid, 
		                           	   "google", 
			                           m_currentRecommendedAppPage, 
			                           firstGetAppListComplete));
		m_getRecommendedAppRequestQueue.request(RequestType.SEQUENCE);
		
//		isLoadApp = true;
	}
	
	private void loadAppListImmediate()
	{
//		isLoadApp = true;
		setRecommendedAppsTitle();
		firstLoadMoreAppList();
	}
	
	private void firstGetAppListComplete(HttpsWWW p_response)
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
						Debug.Log(l_dataCount + " recommended apps feteched");
						Hashtable l_appOwn = SessionHandler.getInstance ().appOwn;
						for(int l_i = 0; l_i < l_dataCount; l_i++)
						{
							Hashtable l_table = l_data[l_i] as Hashtable;
							App l_app = new App(l_table);
							if(null != l_table)
							{
								l_app.own = l_appOwn.ContainsKey(l_app.id.ToString());
							}
							m_currentRecommendedAppList.Add(l_app);
						}
						setRecommendedAppsTitle();
						firstLoadMoreAppList();
					}
				}
			}
		}
	}

	private void firstLoadMoreAppList()
	{
		m_recommendedAppPrototype.active = true;
		if(null == m_appList)
			m_recommendedAppSwipeList = m_addAppCanvas.getView ("RecommendedAppSwipeList") as UISwipeList;
		if(null != m_currentRecommendedAppList)
		{
			m_recommendedAppSwipeList.setData( m_currentRecommendedAppList );
			m_recommendedAppSwipeList.setDrawFunction( onListDraw );
			m_recommendedAppSwipeList.redraw();
			m_recommendedAppSwipeList.addValueChangeListener(onListToEnd);
		}
		m_recommendedAppSwipeList.addClickListener("Prototype", onAppClick);
		
//		m_isLoaded = true;
	}

	private void onListToEnd(Vector2 p_value)
	{
		if( p_value.y <= 0.03 )
		{
			m_recommendedAppSwipeList.removeValueChangeListener(onListToEnd);
			
			if(m_currentRecommendedAppList.Count % 10 == 0 && m_getRecommendedAppRequestQueue.Completed())
			{
				m_currentRecommendedAppPage++;
				m_getRecommendedAppRequestQueue.reset();
				m_getRecommendedAppRequestQueue.add(
					new GetAppByPageRequest(SessionHandler.getInstance().currentKid.age, 
				                            "google", 
				                            m_currentRecommendedAppList.Count/10 + 1, 
				                            getRecommendedAppListComplete));
				m_getRecommendedAppRequestQueue.request();
			}
			else
			{
				m_recommendedAppSwipeList.addValueChangeListener(onListToEnd);
			}
		}
	}

	private void getRecommendedAppListComplete(HttpsWWW p_response)
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
				m_currentRecommendedAppList.Add(l_app);
			}
			setRecommendedAppsTitle();
			m_recommendedAppSwipeList.redraw();
		}
		if(l_dataCount >= 10)
			m_recommendedAppSwipeList.addValueChangeListener(onListToEnd);
	}

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		App l_app = (App)p_data;
		_setupSignleApp (p_element, l_app);
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
		bool l_canShowCosts = true;
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
				downloadAppIcon(p_element,p_app);
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

	private void downloadAppIcon(UIElement p_element, App p_app)
	{
		RequestQueue l_queue = new RequestQueue();
		l_queue.add(new IconRequest(p_app,p_element));
		l_queue.request(RequestType.SEQUENCE);
		m_recommendedAppIconRequests.Add(l_queue);

		p_app.iconDownload = true;
	}

	private void disposeIconRequests()
	{
		int l_numRequests = m_recommendedAppIconRequests.Count;
		for (int i = 0; i < l_numRequests; ++i)
		{
			RequestQueue l_queue = m_recommendedAppIconRequests[i];
			l_queue.dispose();
		}
		m_recommendedAppIconRequests.Clear();
		m_recommendedAppIconRequests = null;
	}

	private void onAppClick(UISwipeList p_list, UIButton p_listElement, System.Object p_data, int p_index)
	{
		m_buyedRecommendedApp = p_listElement;
		App l_app = (App)p_data;
		
		showAppDetails (l_app);
	}

	private void showAppDetails(App p_app)
	{
		m_clickedRecommendedAppDetails = p_app;
		UILabel l_appCostText = m_recommendedAppDetailsCanvas.getView("appCostText") as UILabel;
		UIButton l_buyAppButton = m_recommendedAppDetailsCanvas.getView("buyAppButton") as UIButton;
		UILabel l_appFreeText = m_recommendedAppDetailsCanvas.getView("appFreeText") as UILabel;
		
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
		_setupSignleApp (m_recommendedAppDetailsCanvas, p_app);
		
		UILabel l_description = m_recommendedAppDetailsCanvas.getView("appDescriptionText") as UILabel;
		UILabel l_violence = m_recommendedAppDetailsCanvas.getView("violenceLevelText") as UILabel;
		UILabel l_age = m_recommendedAppDetailsCanvas.getView("ageText") as UILabel;
		UIImage l_icon = m_recommendedAppDetailsCanvas.getView ("appImage") as UIImage;
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
		UIElement l_newPanel = m_recommendedAppDetailsCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
	}

	private void resetSubjectColor()
	{
		UIElement l_readingColor = m_recommendedAppDetailsCanvas.getView ("readingColor") as UIElement;
		l_readingColor.active = false;
		
		UIElement l_scienceColor = m_recommendedAppDetailsCanvas.getView ("scienceColor") as UIElement;
		l_scienceColor.active = false;
		
		UIElement l_socialColor = m_recommendedAppDetailsCanvas.getView ("socialColor") as UIElement;
		l_socialColor.active = false;
		
		UIElement l_cognitiveColor = m_recommendedAppDetailsCanvas.getView ("cognitiveColor") as UIElement;
		l_cognitiveColor.active = false;
		
		UIElement l_creativeColor = m_recommendedAppDetailsCanvas.getView ("creativeColor") as UIElement;
		l_creativeColor.active = false;
		
		UIElement l_lifeSkillsColor = m_recommendedAppDetailsCanvas.getView ("lifeSkillsColor") as UIElement;
		l_lifeSkillsColor.active = false;
		
		UIElement l_mathColor = m_recommendedAppDetailsCanvas.getView ("mathColor") as UIElement;
		l_mathColor.active = false;
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
			installApp(m_clickedRecommendedAppDetails.packageName);
		}
		else
		{
			m_uiManager.changeScreen (UIScreen.APP_DETAILS, false);
			m_uiManager.changeScreen (UIScreen.CONFIRM_DIALOG, true);
			m_buyAppButton = p_button;
			confirmBuyApp (m_clickedRecommendedAppDetails);
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

	private void onExitAppDetailsButtonClick( UIButton p_button )
	{
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_currentPanel = m_recommendedAppDetailsCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_currentPanel.transform.localPosition );
		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );
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

	private void onExitConfiemDialogButtonClick( UIButton p_button )
	{
		m_uiManager.changeScreen (UIScreen.CONFIRM_DIALOG, false);
		m_uiManager.changeScreen (UIScreen.APP_DETAILS, true);
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_currentPanel = m_confirmDialogCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_currentPanel.transform.localPosition );
		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	private void onConfiemButtonClick( UIButton p_button )
	{
		if(null != m_clickedRecommendedAppDetails)
		{
			if(SessionHandler.getInstance().currentKid.gems >= m_clickedRecommendedAppDetails.gems)
				sendBuyAppRequest ();
			else
			{
				UILabel l_costGems = m_confirmDialogCanvas.getView("costPriceText") as UILabel;
				UILabel l_needGems = m_confirmDialogCanvas.getView("needPriceText") as UILabel;
				l_costGems.text = m_clickedRecommendedAppDetails.gems.ToString ();
				l_needGems.text = (m_clickedRecommendedAppDetails.gems - SessionHandler.getInstance().currentKid.gems).ToString ();
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
		m_requestQueue.add(new BuyAppRequest(m_clickedRecommendedAppDetails.id, _buyAppComplete));
		m_requestQueue.request();
		m_uiManager.changeScreen(UIScreen.CONFIRM_DIALOG, false);
	}

	private void _buyAppComplete(HttpsWWW p_response)
	{
		if (null == p_response.error)
		{
			m_clickedRecommendedAppDetails.own = true;
			UILabel l_appCostText = m_buyAppButton.parent.getView("appCostText") as UILabel;
			UIButton l_buyAppButton = m_buyAppButton.parent.getView("buyAppButton") as UIButton;
			l_appCostText.active = false;
			l_buyAppButton.active = false;
			
			_updateAppState(m_buyedRecommendedApp);
			
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

	private void setRecommendedAppsTitle()
	{
		m_recommendedAppTitle.text = m_currentRecommendedAppList.Count.ToString() + " Recommended Apps";
	}
	
	//End of Recommended Apps-----------------------------------------------------------------------------//
	
	private UIManager 		m_uiManager;
	private RequestQueue 	m_requestQueue;
	private bool 			m_isValueChanged = false;

	private UIButton 		m_helpButton;
	private CommonDialogCanvas m_commonDialog;
		
	private UICanvas 		m_paywallCanvas;
	private UIButton 		m_upgradeButton;
	
	//local app part
	private AddAppCanvas 	m_addAppCanvas;
	private UISwipeList 	m_appSwipeList;
	private ArrayList 		m_appList;
	//end of local app part

	//recommended app part
	private UISwipeList     	m_recommendedAppSwipeList;
	private List<object>    	m_currentRecommendedAppList;
	private int 				m_currentRecommendedAppPage;

	private RequestQueue 		m_getRecommendedAppRequestQueue;
	private List<RequestQueue> 	m_recommendedAppIconRequests;
	private UIElement		 	m_recommendedAppPrototype;
	private App 				m_clickedRecommendedAppDetails;

	private UIElement			m_buyedRecommendedApp;
	private UILabel				m_recommendedAppTitle;
	//end of recommended app part

	//recommended app detail part
	private UICanvas	    	m_recommendedAppDetailsCanvas;
	private UIButton 			m_exitAppDetailsButton;
	private UIButton			m_buyAppButton;
	//end of recommended app detail part

	//confirm dialog to buy or install a recommended app
	private UICanvas	    	m_confirmDialogCanvas;
	private UIElement  			m_costArea;
	private UIElement 			m_needMoreArea;
	private UIButton 			m_exitConfirmDialogButton;
	private UIButton 			m_cancelBuyButton;
	private UIButton 			m_confirmBuyButton;
	//end of confirm dialog to buy or install a recommended app
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
