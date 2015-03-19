using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class Game : MonoBehaviour 
{
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

	public void Start()
	{
		_Debug.mode = OutputMode.CONSOLE;

		switch (Input.deviceOrientation) 
		{
		case DeviceOrientation.FaceDown:
		case DeviceOrientation.FaceUp:
		case DeviceOrientation.Portrait:
		case DeviceOrientation.PortraitUpsideDown:
		case DeviceOrientation.Unknown:
		case DeviceOrientation.LandscapeLeft:
			// None landscape orientation, set it manually
			Screen.orientation = ScreenOrientation.LandscapeLeft;
			// Wait a bit
			//yield WaitForSeconds(0.1f);
			// Set back to autorotation, it should be alright by now
			Screen.orientation = ScreenOrientation.AutoRotation;
			break;
		case DeviceOrientation.LandscapeRight:
			Screen.orientation = ScreenOrientation.LandscapeRight;
			// Wait a bit
			//yield WaitForSeconds(0.1f);
			// Set back to autorotation, it should be alright by now
			Screen.orientation = ScreenOrientation.AutoRotation;
			break;    	    
		}

	//	if( Application.platform == RuntimePlatform.Android )
	//		_loadTestWebpage( "https://www.youtube.com/embed/G1UdkMDAdsU" );

	}
	
	public void Awake () 
	{
		GCS.Environment.init();
		FB.Init(_initFacebookComplete);

		ZoodlesScreenFactory l_screenFactory = new ZoodlesScreenFactory();
		ZoodlesStateFactory	 l_stateFactory	= new ZoodlesStateFactory();
		m_gameController = new GameController(this, l_screenFactory, l_stateFactory);

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
	}
	
	public GameController gameController
	{
		get { return m_gameController; }
	}
	
	public void Update ()
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
