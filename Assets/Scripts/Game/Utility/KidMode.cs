using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class KidMode
{

	static List<System.Object> mAllAppList = new List<object>();

	//================================
	//This is now Kid lock native call Settings ON or OFF
	public static void setKidsModeActive(bool p_isActive)
	{
		
		//		if (p_isActive) {
		//
		//			KidModeLockController.Instance.swith2KidMode();
		//
		//
		//		}  else {
		//
		//			KidModeLockController.Instance.swith2DParentMode();
		//
		//
		//		}
		
		//		KidModeLockController.Instance.stateChanged ();
		
		return;
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		object[] l_args = new object[] {p_isActive};
		jo.Call("_setKidsModeActive", l_args); 
		#endif
	}

//	public static event Action onKidModeAndroidStop;

	public static void onActivityStop() {

		Game game = GameObject.FindWithTag("GameController").GetComponent<Game>();
		//screen off, stop timer
		if (!game.IsNativeAppRunning)
		{
			TimerController.Instance.stopTimer();
		}

		//stop midnight notifier
		MidnightNotifier.stopMidnightNotifier();
		//this one is test case
//		game.leaveAppDateTime = DateTime.Now.AddDays(-1);
		//this one is real thing
		game.leaveAppDateTime = DateTime.Now;

		TrialTimeController.Instance.androidExit();


	}

//	public static event Action onKidModeAndroidRestart;

	public static void onActivityRestart() {

		Game game = GameObject.FindWithTag("GameController").GetComponent<Game>();

		//reset midnight notifier to have correct timer
		MidnightNotifier.resetMidnightTimer();

		//app first launch, leave app date time should not have any info
		//if (game.leaveAppDateTime.ToString() == "01/01/0001 00:00:00")
		if (game.leaveAppDateTime.Equals(DateTime.MinValue))
			return;

		//this one is test case
//		DateTime now = DateTime.Now.AddDays(1);
		//this one is real thing
		DateTime now = DateTime.Now;
		int compareDate = now.Date.CompareTo(game.leaveAppDateTime.Date);
		//after comparing date, reset leaveAppDateTime
		game.leaveAppDateTime = DateTime.MinValue;
		//cross midnight, reset timer
		if (compareDate > 0)
		{
			SessionHandler.updateKidsTimeLeft();
			TimerController.Instance.runCurrentKidTimer();
			game.IsNativeAppRunning = false;
			return;
		}
		else if (compareDate == 0)
			Debug.Log("It is impossible(compareDate == 0)");
		else if (compareDate < 0)
			Debug.Log("It is impossible(compareDate < 0)");
		//back to kid mode form native app
		if (game.IsNativeAppRunning)
		{
			TimerController.Instance.resumeTimer();
			game.IsNativeAppRunning = false;
			return;
		}
		//back to kidmode when screen on
		TimerController.Instance.runCurrentKidTimer();


		//==============================
		//Google installed app, auto add to selected list Hack
		KidMode.googleInstalledAppAutoAdd();

		TrialTimeController.Instance.androidEnter();

	}

	// For refresh testing content
	// Type parameter : VIDEO, GAME

	public static void refreshTestingContent(string type)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");
		jc.CallStatic("refreshTestingContent", type);
		#endif
	}

	// On testing content refresh finish
	public static void onTestingContentRefreshFinish(string contents)
	{

	}


	public static void closeNativeWebview()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");
		jc.CallStatic("removeYoutubeView");
		#endif
	}


	public static void showWebViews()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		jo.Call("_showAllWebViews"); 
		#endif
	}
	
	public static bool incomingCallsEnabled()
	{
		return false;
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		bool l_incomingCallsEnabled = jo.Call<bool>("_incomingCallsEnabled"); 
		return l_incomingCallsEnabled;
		//		#else
		//		return false;		
		#endif
	}
	
	
	public static bool isHomeLauncherKidMode()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		bool isHomeLauncherKidMode = jo.Call<bool>("isMyLauncherDefault"); 
		return isHomeLauncherKidMode;
		#else
		return false;		
		#endif
	}
	//1 = DRAW_Z
	//2 = BIRTH_YEAR
	//3 = PIN
	public static int exitAction()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		int l_exitAction = jo.Call<int>("_exitAction"); 
		return l_exitAction;
		#else
		return 1;		
		#endif
	}
	
	public static bool hasHomeButton()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		bool l_hasHomeButton = jo.Call<bool>("_hasHomeButton"); 
		return l_hasHomeButton;
		#else
		return false;		
		#endif
	}
	
	public static bool hasAvailableHomeButton()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		bool l_hasHomeButton = jo.Call<bool>("_hasAvailableHomeButton"); 
		return l_hasHomeButton;
		#else
		return false;		
		#endif
	}
	
	public static bool hasRequestFinished()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		bool l_hasRequestFinished = jo.Call<bool>("_hasRequestFinished"); 
		return l_hasRequestFinished;
		#else
		return false;
		#endif
	}
	
	public static void requestHomeButton()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		jo.Call("_requestHomeButton"); 
		#endif
	}
	
	public static void disableHomeButton()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		jo.Call("_disableHomeButton"); 
		#endif
	}
	
	public static void clearHomeButton()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		jo.Call("_clearHomeButton"); 
		#endif
	}
	
	public static void makeToast(string p_message)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		object[] l_args = new object[] {p_message};
		jo.Call("_makeToast", l_args); 
		#endif
	}
	
	public static string getHomeName()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		string l_homeName = jo.Call<string>("_getHomeName"); 
		return l_homeName;
		#else
		return "";
		#endif
	}
	
	public static void startActivity(string p_packageName, string p_activityName)
	{
		
		GameObject gameLogic = GameObject.FindWithTag("GameController");
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		gameLogic.GetComponent<Game> ().IsReLaunch = 1;
		
		object[] l_params = new object[2];
		l_params[0] = p_packageName;
		l_params[1] = p_activityName;
		
		jo.Call("startApp", l_params); 
		#endif
	}
	
	public static void startActivity(string p_packageName)
	{
		GameObject gameLogic = GameObject.FindWithTag("GameController");
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		
		gameLogic.GetComponent<Game> ().IsReLaunch = 1;
		
		object[] l_params = new object[1];
		l_params[0] = p_packageName;
		
		jo.Call("startApp", l_params); 
		#endif
	}


	#region SystemApps

	public static void getAllSystemApps()
	{

		Debug.Log("   $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$     getAllSystemApps   ");

		#if UNITY_ANDROID && !UNITY_EDITOR
		if (mAllAppList.Count != 0)
			mAllAppList.Clear ();
		TextAsset package = Resources.Load("Data/VZW_System_Apps") as TextAsset;
		string[] names = package.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
		List<string> packageList = new List<string>(names);

		string allSysApps = "";
		for (int i = 0; i < packageList.Count; i++) {
			if(i != 0)
				allSysApps = allSysApps + "," + packageList[i];
			else
				allSysApps = packageList[i];
		}


		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");	
		string appDatasJSON = jc.CallStatic<string>("getAllApps", allSysApps);

		ArrayList l_appNameList = MiniJSON.MiniJSON.jsonDecode(appDatasJSON) as ArrayList;

		if(null != l_appNameList)
		{
			foreach(IDictionary dic in l_appNameList) {




				AppInfo app = new AppInfo();
				app.appName = (string)dic["appName"];
				app.packageName = (string)dic["packageName"];

				Debug.Log("   000000000000000000000  packageName   " + (string)dic["packageName"]);

				Texture2D l_textureIcon = new Texture2D(1, 1, TextureFormat.ARGB32, false);
				l_textureIcon = ImageCache.getCacheImage(app.packageName + ".png");
				app.appIcon = l_textureIcon;
				app.isAdded = false;
				mAllAppList.Add(app);

			}
		}
		#endif


	}


	
	public static List<System.Object> getLocalApps()
	{
		List<System.Object> l_list = new List<object>();
		#if UNITY_ANDROID && !UNITY_EDITOR
		TextAsset package = Resources.Load("Data/VZW_System_Apps") as TextAsset;
		string[] names = package.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
		List<string> packageList = new List<string>(names);
		
		AndroidJavaClass l_jcPlayer = new AndroidJavaClass ( "com.unity3d.player.UnityPlayer" );
		AndroidJavaObject l_joActivity = l_jcPlayer.GetStatic<AndroidJavaObject>( "currentActivity" );
		AndroidJavaObject l_joPackageManager = l_joActivity.Call<AndroidJavaObject> ( "getPackageManager" );
		AndroidJavaObject l_joPackageInfoList = l_joPackageManager.Call<AndroidJavaObject> ( "getInstalledPackages" , 0 );
		
		AndroidJavaClass l_jcBitMap = new AndroidJavaClass( "android.graphics.Bitmap$CompressFormat" );
		AndroidJavaObject l_joPNG = l_jcBitMap.GetStatic<AndroidJavaObject>( "PNG" );
		
		for( int i = 0; i < l_joPackageInfoList.Call<int>("size"); i++ )
		{
			AndroidJavaObject l_joPackageInfo = l_joPackageInfoList.Call<AndroidJavaObject>( "get", i );
			AndroidJavaObject l_joApplication = l_joPackageInfo.Get<AndroidJavaObject>( "applicationInfo" );
			
			int l_flag = l_joApplication.Get<int>("flags");
			AndroidJavaClass l_jcApplicationInfo = new AndroidJavaClass("android.content.pm.ApplicationInfo");
			int l_flagSystem = l_jcApplicationInfo.GetStatic<int>("FLAG_SYSTEM");
			
			string l_appName = l_joPackageManager.Call<string>( "getApplicationLabel", l_joApplication );
			string l_packageName = l_joApplication.Get<string>( "packageName" );
			
			if( (l_flag & l_flagSystem) != 0 )
			{
				//parent dashboard will show specific system apps
				if (!packageList.Contains(l_packageName))
				{
					continue;
				}
			}
			if( l_packageName.Equals("com.zoodles.kidmode") )
			{
				continue;
			}
			
			byte[] l_byteIcon;
			try
			{
				AndroidJavaObject l_jcBitmaoDrawable = l_joPackageManager.Call<AndroidJavaObject>( "getApplicationIcon", l_joApplication );
				AndroidJavaObject l_joBitMapIcon = l_jcBitmaoDrawable.Call<AndroidJavaObject>( "getBitmap" );
				AndroidJavaObject l_joBaos = new AndroidJavaObject( "java.io.ByteArrayOutputStream" );
				int l_quality = 100;
				l_joBitMapIcon.Call<bool>( "compress", l_joPNG, l_quality, l_joBaos);
				l_byteIcon = l_joBaos.Call<byte[]>( "toByteArray" );
			}
			catch( AndroidJavaException )
			{
				l_byteIcon = null;
			}
			
			AppInfo l_app = new AppInfo();
			Texture2D l_textureIcon = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			if(ImageCache.getCacheImage(l_packageName) == null) {
				l_textureIcon.LoadImage( l_byteIcon );
				ImageCache.saveCacheImage(l_packageName + ".png", l_textureIcon);
			}
			else {
				l_textureIcon = ImageCache.getCacheImage(l_packageName);
			}
			l_app.appName = l_appName;
			l_app.appIcon = l_textureIcon;
			l_app.packageName = l_packageName;
			l_app.isAdded = false;
			l_list.Add( l_app );
			
		}
		#endif
		
		return l_list;
	}
	
	//vzw: get selected apps
	public static List<System.Object> getSelectedApps()
	{
		List<System.Object> selectedAppList = new List<object>();
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		KidMode.addDefaultAppsInTheFirstTime();
		string l_appListJson = PlayerPrefs.GetString( "addedAppList" );
		ArrayList l_appNameList = MiniJSON.MiniJSON.jsonDecode( l_appListJson ) as ArrayList;
		if( null != l_appNameList )
		{
			List<object> allAppList = mAllAppList;
			if(allAppList != null && allAppList.Count > 0)
			{
				foreach(AppInfo l_app in allAppList)
				{
					if( l_appNameList.Count > 0 && l_appNameList.Contains(l_app.packageName) )
					{
						selectedAppList.Add(l_app);
					}
				}
			}
		}
		#endif
		
		return selectedAppList;
	}


	public static List<System.Object> getSelectedAppsSorted()
	{

		List<System.Object> sortedSelectedAppList = new List<object>();

		List<System.Object> selectedAppNamesList = getSelectedAppsNames();
//		#if UNITY_ANDROID && !UNITY_EDITOR
		
		KidMode.addDefaultAppsInTheFirstTime();
		string l_appListJson = PlayerPrefs.GetString( "addedAppList" );
		ArrayList l_appNameList = MiniJSON.MiniJSON.jsonDecode( l_appListJson ) as ArrayList;



		if( null != l_appNameList )
		{

			GoogleInstallAutoAddController.Instance.setLocalAppNamesSortedByAddedTime();

			ArrayList sortedAppNameList = GoogleInstallAutoAddController.Instance.getLocallAppNamesSoretedByAddedTime();

			List<object> allAppList = mAllAppList;
			if(sortedAppNameList != null && sortedAppNameList.Count > 0)
			{
				for (int i = 0; i < sortedAppNameList.Count; i++) {

					if( selectedAppNamesList.Count > 0 && selectedAppNamesList.Contains(sortedAppNameList[i]) )
					{
//						selectedAppList.Add(sortedAppNameList[i]);

						foreach(AppInfo l_app in allAppList)
						{

							string name = sortedAppNameList[i] as string;

							if(l_app.packageName == name){

								sortedSelectedAppList.Add(l_app);

							}

						}


					}

				}
			}
		}
//		#endif


		
		return sortedSelectedAppList;
	}


	public static List<System.Object> getSelectedAppsNames()
	{
		List<System.Object> selectedAppList = new List<object>();
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		KidMode.addDefaultAppsInTheFirstTime();
		string l_appListJson = PlayerPrefs.GetString( "addedAppList" );
		ArrayList l_appNameList = MiniJSON.MiniJSON.jsonDecode( l_appListJson ) as ArrayList;
		if( null != l_appNameList )
		{
			List<object> allAppList = mAllAppList;
			if(allAppList != null && allAppList.Count > 0)
			{
				foreach(AppInfo l_app in allAppList)
				{
					if( l_appNameList.Count > 0 && l_appNameList.Contains(l_app.packageName) )
					{
						selectedAppList.Add(l_app.packageName);
					}
				}
			}
		}
		#endif
		
		return selectedAppList;
	}
	
	//vzw: get apps for parent dashboard
	public static List<System.Object> getApps()
	{
		List<System.Object> appList = new List<object>();
		#if UNITY_ANDROID && !UNITY_EDITOR
		KidMode.addDefaultAppsInTheFirstTime();
		string l_appListJson = PlayerPrefs.GetString( "addedAppList" );
		ArrayList l_appNameList = MiniJSON.MiniJSON.jsonDecode( l_appListJson ) as ArrayList;
		List<System.Object> l_list = mAllAppList;

		TextAsset package = Resources.Load("Data/VZW_System_Ignore_Apps") as TextAsset;
		string[] ignoreNames = package.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
		List<string> ignoreNamesList = new List<string>(ignoreNames);


		if ( l_list != null && l_list.Count > 0)
		{
			foreach (AppInfo l_app in l_list)
			{

				if( ignoreNamesList.Contains(l_app.packageName) ){

					continue;

				}

				if ( l_appNameList.Count > 0 && l_appNameList.Contains(l_app.packageName) )
				{
					l_app.isAdded = true;
				}
				appList.Add( l_app );
			}
		}
		#endif
		
		return appList;
	}

	public static List<System.Object> getAppsSorted()
	{
		
		
		//		#if UNITY_ANDROID && !UNITY_EDITOR
		
		//		KidMode.addDefaultAppsInTheFirstTime();
		//		string l_appListJson = PlayerPrefs.GetString( "addedAppList" );
		//		ArrayList l_appNameList = MiniJSON.MiniJSON.jsonDecode( l_appListJson ) as ArrayList;
		
		
		
		//		if( null != l_appNameList )
		//		{
		
		List<System.Object> sortedAppsList = new List<object>();
		
		GoogleInstallAutoAddController.Instance.setLocalAppNamesSortedByAddedTime();
		
		ArrayList sortedAppNameList = GoogleInstallAutoAddController.Instance.getLocallAppNamesSoretedByAddedTime();
		
		List<object> allAppList = mAllAppList;
		if(sortedAppNameList != null && sortedAppNameList.Count > 0)
		{
			for (int i = 0; i < sortedAppNameList.Count; i++) {
				
				//				if( selectedAppNamesList.Count > 0 && selectedAppNamesList.Contains(sortedAppNameList[i]) )
				//				{
				//						selectedAppList.Add(sortedAppNameList[i]);
				
				foreach(AppInfo l_app in allAppList)
				{
					
					string name = sortedAppNameList[i] as string;
					
					if(l_app.packageName == name){
						
						sortedAppsList.Add(l_app);
						
					}
					
				}
				
				
				//				}
				
			}
		}
		//		}
		//		#endif
		
		
		
		return sortedAppsList;
	}


	public static void addDefaultAppsInTheFirstTime()
	{
		string l_appListJson = PlayerPrefs.GetString( "addedAppList" );
		ArrayList l_appNameList = MiniJSON.MiniJSON.jsonDecode( l_appListJson ) as ArrayList;
		//set default apps in the first time
		if( null == l_appNameList )
		{
			l_appNameList = new ArrayList();
			
			TextAsset defaultApps = Resources.Load("Data/Default_Native_Apps") as TextAsset;
			string[] names = defaultApps.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
			List<string> defaultAppsList = new List<string>(names);
			foreach (string name in defaultAppsList)
			{
				l_appNameList.Add(name);
			}
			PlayerPrefs.SetString( "addedAppList", MiniJSON.MiniJSON.jsonEncode( l_appNameList ) );
		}
	}





	public static List<System.Object> getLastLocalApps()
	{
		List<System.Object> selectedAppList = new List<object>();
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		KidMode.addDefaultAppsInTheFirstTime();
		string l_appListJson = PlayerPrefs.GetString( "lastLocalApps" );
		ArrayList l_appNameList = MiniJSON.MiniJSON.jsonDecode( l_appListJson ) as ArrayList;
		if( null != l_appNameList )
		{
			List<object> allAppList = mAllAppList;
			if(allAppList != null && allAppList.Count > 0)
			{
				foreach(AppInfo l_app in allAppList)
				{
					if( l_appNameList.Count > 0 && l_appNameList.Contains(l_app.packageName) )
					{
						selectedAppList.Add(l_app.packageName);
					}
				}
			}
		}
		#endif
		
		return selectedAppList;
	}

	public static void setLastLocalAppInfo()
	{
//		#if UNITY_ANDROID && !UNITY_EDITOR
		
//		List<object> m_dataList = new List<object> ();
		List<object> l_list = KidMode.getApps();
//		foreach (AppInfo l_app in l_list)
//		{
//			m_dataList.Add(l_app);
//		}


		ArrayList l_appNameList = new ArrayList();
		foreach (AppInfo l_app in l_list)
		{
			l_appNameList.Add(l_app.packageName);
		}

		PlayerPrefs.SetString( "lastLocalApps", MiniJSON.MiniJSON.jsonEncode(l_appNameList) );

//		#endif
		
//		return selectedAppList;
	}







	public static void googleInstalledAppAutoAdd()
	{

		Debug.Log("  ++++++++++++++++++++++++++++++++++++++++++++++++++++     ++googleInstalledAppAutoAdd");

		GoogleInstallAutoAddController.Instance.checkList();

		return;

		int hasLuanchedGoogle = PlayerPrefs.GetInt("hasLaunchedGoogle");

		if(hasLuanchedGoogle == 1){

			List<object> l_list = KidMode.getApps();

			List<object> lastLocalAppsList = KidMode.getLastLocalApps();



			List<object> selectedList = KidMode.getSelectedApps();

			ArrayList selectedArrayList = new ArrayList(selectedList);

			foreach (AppInfo l_app in l_list)
			{

				if(!lastLocalAppsList.Contains(l_app.packageName)){

					selectedArrayList.Add(l_app.packageName);

				}
//				selectedList.add
				//l_app

			}

			PlayerPrefs.SetString( "addedAppList", MiniJSON.MiniJSON.jsonEncode(selectedArrayList) );

		}

		PlayerPrefs.SetInt("hasLaunchedGoogle", 0);

	}


	#endregion



	//============
	
	public static bool hasFlashInstalled ()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		bool l_hasRequestFinished = jo.Call<bool>("_hasFlashInstalled"); 
		return l_hasRequestFinished;
		#else
		return false;
		#endif
	}
	
	
	public static void enablePluginComponent(){
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 		
		
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 		
		
		jo.Call("enablePluginComponent"); 
		#endif
		
	}
	
	
	
	public static void openLauncherSelector(){
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 		
		
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 		
		
		jo.Call("openLauncherSelector");
		
		#endif
		
	}
	
	public static void disablePluginComponent(){
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 		
		
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 		
		
		jo.Call("disablePluginComponent"); 
		
		#endif
		
	}
	
	public static void taskManagerLockTrue(){
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 		
		
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 		
		
		object[] l_args = new object[] {true} ;
		
		jo.Call("setTaskManagerLock", l_args); 
		
		#endif
		
	}
	
	
	
	public static void taskManagerLockFalse(){
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 		
		
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 				
		
		object[] l_args = new object[] {false} ;
		
		jo.Call("setTaskManagerLock", l_args); 
		
		#endif
		
	}
	
	
	public static void openDefaultLauncher(){
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		
		jo.Call("openDefaultLauncher"); 
		
		#endif
		
	}
	
	public static void setFullScreen(){
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		
//		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
//		
//		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
//		
//		jo.Call("setFullScreen"); 

		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");	
		jc.CallStatic("showFullScreen");
		
		#endif
		
	}

	public static void systemGC(){
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");	
		jc.CallStatic("systemGC");
		
		#endif
		
	}

	public static void setFullScreenDelay(){
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");	
		jc.CallStatic("resetScreenDelay");
		
		#endif
		
	}
	
	public static void openSettings(){
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		
		jo.Call("openSettings1"); 
		
		#endif
		
	}

	public static bool isAirplaneModeOn(){

		bool isAirplaneModeOn = false;
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");
		isAirplaneModeOn = jc.CallStatic<bool>("isAirplaneModeOn");

		#endif
		return isAirplaneModeOn;
	}
	
	
	public static void openWifi(bool check){
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		
		jo.Call("openWifiSettings", check);
		
		#endif
		
	}
	
	public static void openGooglePlay(){
		
		string packageName = "com.android.vending";
		
		startActivity (packageName);
		
	}

	//let system know which mode it is currently
	//Parameter: 1. ParentMode 2. KidMode
	//To cathy: please uncomment the code after you want to use it
	public static void broadcastCurrentMode(string mode)
	{
		Debug.Log("broadcast mode: " + mode);

		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");	
		jc.CallStatic("broadcastCurrentMode", mode);
		
		#endif
	}

	public static void broadcastKidmodeFota()
	{
		Debug.Log("broadcast Kid mode Fota");

		#if UNITY_ANDROID && !UNITY_EDITOR

		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");	
		jc.CallStatic("broadcastKidmodeFota");
		
		#endif

	}

	public static void showProgressBar()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");	
		jc.CallStatic("showProgressDialog");
		#endif
	}

	public static void dismissProgressBar()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");	
		jc.CallStatic("dismissProgressDialog");
		#endif
	}

	public static bool isWifiConnected()
	{
				
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");	
		bool isWifiConnectes = jc.CallStatic<bool>("isWifiConnected");

		return isWifiConnectes;
		#endif

		return true;
	}
	
}

