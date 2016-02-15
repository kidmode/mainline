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
		checkRequest ();
		
		base.exit (p_gameController);
		
		m_uiManager.removeScreenImmediately( UIScreen.COMMON_DIALOG );
		
		m_uiManager.removeScreenImmediately( UIScreen.ADD_APPS );
		m_uiManager.removeScreenImmediately( UIScreen.PAYWALL );

		GoogleInstallAutoAddController.OnNewAppAdded -= OnNewAppAdded;
	}

	private void _setupScreen( GameController p_gameController )
	{
		m_commonDialog 				= m_uiManager.createScreen( UIScreen.COMMON_DIALOG, true, 5 ) 			as CommonDialogCanvas;
		m_commonDialog.setUIManager (p_gameController.getUI());

		if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
		{
			m_paywallCanvas = m_uiManager.createScreen( UIScreen.PAYWALL, false, 2 );
			m_upgradeButton = m_paywallCanvas.getView( "upgradeButton" ) as UIButton;
			m_upgradeButton.addClickCallback( onUpgradeButtonClick );
		}

		m_addAppCanvas 				= m_uiManager.createScreen( UIScreen.ADD_APPS, true, 1 ) 				as AddAppCanvas;

		if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
		{
			m_uiManager.setScreenEnable( UIScreen.ADD_APPS, false );
		}
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

		//app part
		m_appSwipeList = m_addAppCanvas.getView ( "appSwipeList" ) as UISwipeList;
		m_appSwipeList.addClickListener ( "controlButton", m_addAppCanvas.onButtonClicked );
		m_appSwipeList.addClickListener ( "controlButton", onAppButtonClicked );
		m_appSwipeList.active = false;
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
//		if( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() )
//		{

		try 
		{

			List<object> lastLocalAppsList = KidMode.getLastLocalApps();
			
			Debug.LogWarning("      ****************************   lastLocalAppsList " + lastLocalAppsList.Count);

			m_addAppCanvas.firstLoadApp();

//			if(lastLocalAppsList.Count > 0 || PlayerPrefs.GetString( "lastLocalApps" ) == ""){
//
//				GoogleInstallAutoAddController.Instance.checkList();
//
//			}else{
//
//				m_addAppCanvas.firstLoadApp();
//
//			}


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
	
	private UIManager 		m_uiManager;
	private RequestQueue 	m_requestQueue;
	private bool 			m_isValueChanged = false;



	private UIButton 		m_helpButton;
	private CommonDialogCanvas m_commonDialog;
		
	private UICanvas 		m_paywallCanvas;
	private UIButton 		m_upgradeButton;


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
