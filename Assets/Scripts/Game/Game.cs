using UnityEngine;
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
	
	private static bool mIsPlay = true;

	//honda
	public delegate void onRequestCompletedEvent(bool isCOmpleted);
	public event onRequestCompletedEvent onRequestCompleted;
	
	private RequestQueue m_request;
	private bool isClientIdCompleted;
	private bool isPremiumCompleted;
	private int testTimes;
	//end

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
		this.gameSwitcher (true);
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

		//honda
//		PlayerPrefs.DeleteAll();
		
		m_request = new RequestQueue ();
		isClientIdCompleted = false;
		isPremiumCompleted = false;
		testTimes = 0;
		//end

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

		mIsPlay = isPlay;
//		if(isPlay)
//			Time.timeScale = 1;
//		else
//			Time.timeScale = 0;
	}
	
	public void FixedUpdate ()
	{
//		int l_time = (int)(Time.deltaTime * 1000.0f);
//		Server.update(l_time);
//		m_gameController.update(l_time);
//		SoundManager.getInstance().updateSystemSound();

	}

	public void Update ()
	{
		if (mIsPlay) {
			int l_time = (int)(Time.deltaTime * 1000.0f);
			Server.update(l_time);
			m_gameController.update(l_time);
			SoundManager.getInstance().updateSystemSound();
		}
		
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

	//honda
	public void clientIdAndPremiumRequests(onRequestCompletedEvent completedEvent)
	{
		if (completedEvent != null)
		{
			onRequestCompleted += completedEvent;
		}
		
		if (Application.internetReachability != NetworkReachability.NotReachable)
		{
			setClientIdAndPremiumRequests();
		}
		else
		{
			if (onRequestCompleted != null)
			{
				onRequestCompleted(false);
				onRequestCompleted = null;
			}
		}
	}
	
	private void setClientIdAndPremiumRequests()
	{
		m_request.add ( new ClientIdRequest(getClientIdComplete) );
		m_request.add ( new CheckFreePremiumRequest(getCheckComplete) );
		m_request.request ( RequestType.SEQUENCE );
	}
	
	private void getClientIdComplete(WWW p_response)
	{
		if(p_response.error == null)
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			SessionHandler.getInstance ().clientId = l_data.ContainsKey("id") ? double.Parse(l_data["id"].ToString()) : -1;
			
			isClientIdCompleted = true;
			checkRequestCompleted();
		}
		else
		{
			if (!SessionHandler.getInstance().token.isExist()) //cynthia
			{
				testTimes++;
				if (testTimes <= 3)
				{
					m_request.reset();
					clientIdAndPremiumRequests(null);
				}
				else
				{
					Game game = m_gameController.game;;
					game.gameController.getUI().createScreen(UIScreen.NO_INTERNET, false, 6);
				}
				//cynthia vzw
//				Game game = GameObject.FindWithTag("GameController").GetComponent<Game>();
//				game.gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
				//setErrorMessage( m_gameController, Localization.getString(Localization.TXT_STATE_0_FAIL), Localization.getString(Localization.TXT_STATE_0_FAIL_MESSAGE) );
				//vzw end
			}
		}
	}
	
	private void getCheckComplete(WWW p_response)
	{
		if(p_response.error == null)
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			l_data = (l_data["jsonResponse"] as Hashtable)["response"] as Hashtable;
			SessionHandler.getInstance().renewalPeriod = (int)((double)l_data["renewal_period"]);
		}
		else
		{
			if (!SessionHandler.getInstance().token.isExist()) //cynthia
			{
				m_request.reset();
				//cynthia vzw
//				Game game = GameObject.FindWithTag("GameController").GetComponent<Game>();
//				game.gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
				//setErrorMessage( m_gameController, Localization.getString(Localization.TXT_STATE_0_FAIL), Localization.getString(Localization.TXT_STATE_0_FAIL_MESSAGE) );
				//vzw end
			} 
		}
		isPremiumCompleted = true;
		checkRequestCompleted();
	}
	
	private void checkRequestCompleted()
	{
		if (isClientIdCompleted && isPremiumCompleted)
		{
			if (onRequestCompleted != null)
			{
				onRequestCompleted(true);
				onRequestCompleted = null;
			}
			isClientIdCompleted = false;
			isPremiumCompleted = false;
		}
	}
	
	//end

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
