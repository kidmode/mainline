using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KidMode
{
	public static void setKidsModeActive(bool p_isActive)
	{
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
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		bool l_incomingCallsEnabled = jo.Call<bool>("_incomingCallsEnabled"); 
		return l_incomingCallsEnabled;
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
			if( (l_flag & l_flagSystem) != 0 )
			{
				continue;
			}
			
			string l_appName = l_joPackageManager.Call<string>( "getApplicationLabel", l_joApplication );
			string l_packageName = l_joApplication.Get<string>( "packageName" );
			byte[] l_byteIcon;

			if( l_packageName.Equals("com.zoodles.kidmode") )
			{
				continue;
			}
			
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
			Texture2D l_textureIcon = new Texture2D(1, 1);
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
}
