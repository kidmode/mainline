﻿using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class Game : MonoBehaviour 
{

	private static String IS_RELAUNCH		=	"IS_RELAUCH";
	private static String IS_FIRST_LAUNCH	=	"IS_FIRST_LAUNCH";
	private static String IS_LOGING			=	"IS_LOGING";

	private static int isReLaunch			=	0;  // 0: Normal launch, 1: Relaunch
	private static int isFirstLaunch		=	0;	// 0: First launch, 1: Not first launch
	private static int isLogin				=	0;	// 0: Not login , 1: Logined


	private static bool isPause = false;


	public bool IsPause {
		get { 
			return isPause;   
		}
		set { isPause = value;  }

	}

	public int IsLogin
	{
		get { 
			return PlayerPrefs.GetInt(IS_LOGING, 0);   
		}
		set { PlayerPrefs.SetInt(IS_LOGING, value);  }
	}

	public int IsReLaunch
	{
		get { 
			return PlayerPrefs.GetInt(IS_RELAUNCH, 0);   
		}
		set { PlayerPrefs.SetInt(IS_RELAUNCH, value);  }
	}

	public int IsFirstLaunch
	{
		get { 
			return PlayerPrefs.GetInt(IS_FIRST_LAUNCH, 0);   
		}
		set { PlayerPrefs.SetInt(IS_FIRST_LAUNCH, value); }
	}

	public Game()
	{
		delayedParentDashboard = false;
	}
	
/*	public void OnGUI () 
	{
		#if SHOW_STATS
		ShowStatistics();
		#endif
	}
*/


	public void closeYoutube() {
		GameObject gameLogic = GameObject.FindWithTag("GameController");
		gameLogic.GetComponent<Game> ().gameSwitcher (true);
		WebViewState._clickBackBtn ();
	}

	public void OnLoadYoutubeComplete() {
		WebViewState.HandleOnLoadComplete ();
	}


	public void Start()
	{
		_Debug.mode = OutputMode.DISABLE;

		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.orientation = ScreenOrientation.AutoRotation;

//		switch (Input.deviceOrientation) 
//		{
//		case DeviceOrientation.FaceDown:
//		case DeviceOrientation.FaceUp:
//		case DeviceOrientation.Portrait:
//		case DeviceOrientation.PortraitUpsideDown:
//		case DeviceOrientation.Unknown:
//		case DeviceOrientation.LandscapeLeft:
//			// None landscape orientation, set it manually
//			Screen.orientation = ScreenOrientation.LandscapeLeft;
//			// Wait a bit
//			//yield WaitForSeconds(0.1f);
//			// Set back to autorotation, it should be alright by now
//			Screen.orientation = ScreenOrientation.AutoRotation;
//			break;
//		case DeviceOrientation.LandscapeRight:
//			Screen.orientation = ScreenOrientation.LandscapeRight;
//			// Wait a bit
//			//yield WaitForSeconds(0.1f);
//			// Set back to autorotation, it should be alright by now
//			Screen.orientation = ScreenOrientation.AutoRotation;
//			break;    	    
//		}

	//	if( Application.platform == RuntimePlatform.Android )
	//		_loadTestWebpage( "https://www.youtube.com/embed/G1UdkMDAdsU" );

	}
	
	public void Awake () 
	{
		// Sean: vzw
		#if UNITY_ANDROID && !UNITY_EDITOR
		Application.targetFrameRate = 30;
		
		QualitySettings.vSyncCount = 0; 
		
		QualitySettings.antiAliasing = 0;
		QualitySettings.SetQualityLevel (0);
		#endif
 		// vzw end

		GCS.Environment.init();
		FB.Init(_initFacebookComplete);

		ZoodlesScreenFactory l_screenFactory = new ZoodlesScreenFactory();
		ZoodlesStateFactory	 l_stateFactory	= new ZoodlesStateFactory();
		m_gameController = new GameController(this, l_screenFactory, l_stateFactory);

		Localization.loadLanguage ();

        startLoading();

		GAUtil.startSession("Login");
	}

	public void OnApplicationFocus(bool p_focus)
	{
		if (p_focus)
		{
			m_gameController.handleMessage(1, ""); // Notify game restore message to state
			GAUtil.startSession("WakeUpApp");
		}
		else
			GAUtil.stopSession("Sleep");
	}

	public void OnApplicationQuit()
	{
		GAUtil.stopSession("ExitApp");
	}
	
	public void startLoading()
	{
		m_user = new User();
	}	
	
	public User user
	{
		get { return m_user; }
		set { m_user = value;}
	}
	
	public GameController gameController
	{
		get { return m_gameController; }
	}

	public void gameSwitcher(Boolean isPlay) {
		if(isPlay)
			Time.timeScale = 1;
		else
			Time.timeScale = 0;
	}
	
	public void FixedUpdate ()
	{
		int l_time = (int)(Time.deltaTime * 1000.0f);
		Server.update(l_time);
		m_gameController.update(l_time);
		SoundManager.getInstance().updateSystemSound();

	}
	
	public string getVersion()
	{
		return GCS.Environment.getVersion();
	}
	
	//public float loading
	//{
	//	set { m_loading = value; }
	//	get { return m_loading;  }
	//}
	
	public void handleMessage(int p_type, System.Object p_data)
	{
		
	}


    public void onEnglishToggle()
    {
        Localization.changeLanguage("EN");
    }

    public void onSpanishToggle()
    {
        Localization.changeLanguage("ES");
    }

	//The listening method of OnLoadComplete method.
	public void OnLoadComplete(UniWebView webView, bool success, string errorMessage) 
	{
		if (success) 
		{
			//Great, everything goes well. Show the web view now.
			webView.Show();
		} 
		else 
		{
			//Oops, something wrong.
			_Debug.logError("Something wrong in webview loading: " + errorMessage);
		}
	}

	public void gotoParentDashboard(string message) 
	{ 
		_Debug.log("message from java: " + message); 
		SessionHandler l_sessionHandler = SessionHandler.getInstance();
		List<Kid> l_kidList = l_sessionHandler.kidList;

		if (l_sessionHandler.SignInFail == false
		    && l_sessionHandler.clientId != 0
		    && l_kidList.Count > 0)
		{
			gameController.changeState(ZoodleState.GOTO_PARENT_DASHBOARD);
		}
		else
		{
			delayedParentDashboard = true;
		}
	}

	public void onAndroidPause(string info){
		
		Debug.LogWarning (" onAndroidPause " + info);
		
		KidModeLockController.Instance.onAndroidPause ();
		
	}
	
	
	public void onAndroidResume(string info){
		
		Debug.LogWarning (" onAndroidResume " + info);
		
		KidModeLockController.Instance.onAndroidResume ();
		
	}

	public bool delayedParentDashboard
	{
		get; set;
	}

	#if SHOW_STATS
	private function ShowStatistics():void
	{
		GUILayout.Label("All " + Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)).Length);
		GUILayout.Label("Textures " + Resources.FindObjectsOfTypeAll(typeof(Texture)).Length);
		GUILayout.Label("AudioClips " + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length);
		GUILayout.Label("Meshes " + Resources.FindObjectsOfTypeAll(typeof(Mesh)).Length);
		GUILayout.Label("Materials " + Resources.FindObjectsOfTypeAll(typeof(Material)).Length);
		GUILayout.Label("GameObjects " + Resources.FindObjectsOfTypeAll(typeof(GameObject)).Length);
		GUILayout.Label("Components " + Resources.FindObjectsOfTypeAll(typeof(Component)).Length);
	}
	#endif

	private void _initFacebookComplete()
	{
		_Debug.log("Facebook initialize completed.");
	}

	private GameController m_gameController;
	private User m_user;
	private WWW m_www;
	private bool m_delayedParentDashboard;
}
