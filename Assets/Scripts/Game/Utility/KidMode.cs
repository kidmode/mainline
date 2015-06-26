using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class KidMode
{

	//================================
	//This is now Kid lock native call Settings ON or OFF
	public static void setKidsModeActive(bool p_isActive)
	{

//		if (p_isActive) {
//
//			KidModeLockController.Instance.swith2KidMode();
//
//
//		} else {
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
		#else
		return false;		
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

		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		
		object[] l_params = new object[2];
		l_params[0] = p_packageName;
		l_params[1] = p_activityName;
		
		jo.Call("startApp", l_params); 
		#endif
	}

	public static void startActivity(string p_packageName)
	{

		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		
		object[] l_params = new object[1];
		l_params[0] = p_packageName;

		jo.Call("startApp", l_params); 
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
			if( l_packageName.Equals("com.zoodles.ellipsiskids") )
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
			l_textureIcon.LoadImage( l_byteIcon );
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
			List<object> allAppList = KidMode.getLocalApps();
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

	//vzw: get apps for parent dashboard
	public static List<System.Object> getApps()
	{
		List<System.Object> appList = new List<object>();
		#if UNITY_ANDROID && !UNITY_EDITOR
		KidMode.addDefaultAppsInTheFirstTime();
		string l_appListJson = PlayerPrefs.GetString( "addedAppList" );
		ArrayList l_appNameList = MiniJSON.MiniJSON.jsonDecode( l_appListJson ) as ArrayList;
		List<System.Object> l_list = KidMode.getLocalApps();
		if ( l_list != null && l_list.Count > 0)
		{
			foreach (AppInfo l_app in l_list)
			{
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

	public static bool isLauncherKidmode(){
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 		
		
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		
		bool launcher = jo.Call<bool>("isMyLauncherDefault"); 
		
		return launcher;
		#endif
		return false;
		
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
		
		object[] l_args = new object[] {true};

		jo.Call("setTaskManagerLock", l_args); 
		
		#endif
		
	}
	
	
	
	public static void taskManagerLockFalse(){
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 		
		
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 				
		
		object[] l_args = new object[] {false};

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

		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		
		jo.Call("setFullScreen"); 

		#endif

	}

	public static void openSettings(){

		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		
		jo.Call("openSettings1"); 
		
		#endif
		
	}


	public static void openWifi(){

		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		
		jo.Call("openWifiSettings"); 
		
		#endif
		
	}

	public static void openGooglePlay(){

		string packageName = "com.android.vending";

		startActivity (packageName);
		
	}


}
