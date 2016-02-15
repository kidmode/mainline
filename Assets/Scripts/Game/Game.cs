using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Serialization.Formatters.Binary;

public class Game : MonoBehaviour 
{

	private static String IS_RELAUNCH		=	"IS_RELAUCH";
	private static String IS_FIRST_LAUNCH	=	"IS_FIRST_LAUNCH";
	private static String IS_LOGING			=	"IS_LOGING";

	private static int isReLaunch			=	0;  // 0: Normal launch, 1: Relaunch
	private static int isFirstLaunch		=	0;	// 0: First launch, 1: Not first launch
	private static int isLogin				=	0;	// 0: Not login , 1: Logined
	
	private static bool mIsPlay = true;

	private static bool mAppIsLoad = false;

	//Kevin
	public static event Action OnVideoClosed;

	public static event Action OnPaymentSuccess;



	//honda
	public delegate void onRequestCompletedEvent(bool isCOmpleted);
	public event onRequestCompletedEvent onRequestCompleted;

	public PDMenuBarScript pdMenuBar;

	//Cathy for close webview dialog
	public delegate void onCloseWebviewDialog();
	public event onCloseWebviewDialog mOnCloseWebviewDialog; 
	
	private RequestQueue m_request;
	private bool isClientIdCompleted;
	private bool isPremiumCompleted;
	private int testTimes;

	private bool mIsRun = true;

	private bool isNativeAppRunning = false;
	public bool IsNativeAppRunning
	{
		get
		{
			return isNativeAppRunning;
		}
		set
		{
			isNativeAppRunning = value;
		}
	}
	public DateTime leaveAppDateTime;
	//end
	private bool mIsLoading = false;
	public bool isLoading
	{
		get { 
			return mIsLoading;   
		}
		set { mIsLoading = value;  }

	}

	void OnApplicationPause(bool pauseStatus) {
		if (pauseStatus) {
			if(mOnCloseWebviewDialog != null) {
				mOnCloseWebviewDialog();
				mOnCloseWebviewDialog = null;
			}
			KidMode.onActivityStop ();
		}
		else {
			KidMode.setFullScreenDelay();
			KidMode.onActivityRestart ();
		}
	}

	
	public bool IsAppLoad
	{
		get { 
			return mAppIsLoad;   
		}
		set { mAppIsLoad = value;  }
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

	public bool isNotPlayingNativeWebView
	{
		get{ return mIsPlay;}
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

	public bool isFotaBroadcast = false;

	public void receieveFOTABroadcast()
	{
		if (isKidMode())
		{
			Debug.Log("receieve FOTA Broadcast, do parent gate due to kid mode");
			isFotaBroadcast = true;

			UIManager l_ui = m_gameController.getUI();
			UICanvas l_backScreen = l_ui.findScreen(UIScreen.SPLASH_BACKGROUND);
			if (l_backScreen == null)
			{
				l_backScreen = l_ui.createScreen(UIScreen.SPLASH_BACKGROUND,true, -1);
				SplashBackCanvas l_splashBack = l_backScreen as SplashBackCanvas;
				l_splashBack.setDown();
			}

			m_gameController.connectState(ZoodleState.BIRTHYEAR, int.Parse(m_gameController.stateName));
			m_gameController.changeState(ZoodleState.BIRTHYEAR);
		}
		else
		{	
			Debug.Log("receieve FOTA Broadcast, do nothing due to parent mode");
		}
	}

	private bool isKidMode()
	{
		if (//map
		    gameController.stateName.Equals("4")  || 
		    //jungle
		    gameController.stateName.Equals("5")  ||
		    //region_video, game, drawing, book
		    gameController.stateName.Equals("66") || 
		    gameController.stateName.Equals("67") || 
		    gameController.stateName.Equals("68") || 
		    gameController.stateName.Equals("32") || 
		    //activity_video, game, drawing, book
		    gameController.stateName.Equals("52") || 
		    gameController.stateName.Equals("53") || 
		    gameController.stateName.Equals("7")  ||
		    gameController.stateName.Equals("10") ||
		    //congratulations
		    gameController.stateName.Equals("49") ||
		    //kids_profile
		    gameController.stateName.Equals("33"))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	public void refreshJungleMedia() 
	{
		Debug.Log("spinner: turn on wifi internetReachability" + Application.internetReachability);
//		if (Application.internetReachability != NetworkReachability.NotReachable)
		if (checkWebcontentList())
		{
			Debug.Log("spinner: refresh contents after having connection");
			showSpinnerWithFetchingContents();
		}
		if (checkDrawingList())
		{
			Debug.Log("spinner: refresh drawing contents after having connection");
			fetchDrawingList();
		}
	}

	public void closeWifi() 
	{

		Debug.Log("spinner: turn off wifi internetReachability" + Application.internetReachability);

	}

	private bool checkWebcontentList()
	{
		if (//map
		    m_gameController.stateName.Equals("4")  || 
		    //jungle
		    m_gameController.stateName.Equals("5")  ||
		    //kids_profile
		    m_gameController.stateName.Equals("33"))
		{

			if (SessionHandler.getInstance().webContentList == null || SessionHandler.getInstance().webContentList.Count == 0)
			{
				Debug.Log("spinner: (need to fetch)game state = " + m_gameController.stateName + " webcontent.count = " + SessionHandler.getInstance().webContentList.Count);
				return true;
			}
			else
			{
				Debug.Log("spinner: (don't need to fetch)game state = " + m_gameController.stateName + " webcontent.count = " + SessionHandler.getInstance().webContentList.Count);
				return false;
			}
		}
		else
		{
			Debug.Log("spinner: (not correct state)game state = " + m_gameController.stateName + " webcontent.count = " + SessionHandler.getInstance().webContentList.Count);
			return false;
		}
	}

	private bool checkDrawingList()
	{
		if (//jungle
		    m_gameController.stateName.Equals("5")  ||
		    //kids_profile
		    m_gameController.stateName.Equals("33"))
		{
			
			if (SessionHandler.getInstance().drawingList == null)
			{
				Debug.Log("spinner: (need to fetch)game state = " + m_gameController.stateName + " drawingList.count = null");
				return true;
			}
			else if (SessionHandler.getInstance().drawingList.Count == 0)
			{
				Debug.Log("spinner: (need to fetch)game state = " + m_gameController.stateName + " drawingList.count = 0");
				return true;
			}
			else
			{
				Debug.Log("spinner: (don't need to fetch)game state = " + m_gameController.stateName + " drawingList.count = " + SessionHandler.getInstance().drawingList.Count);
				return false;
			}
		}
		else
		{
			Debug.Log("spinner: (not correct state)game state = " + m_gameController.stateName);
			return false;
		}
	}

	public void onActivityRestart() {
		KidMode.onActivityRestart ();

//		KidMode.getAllSystemApps();
		//
	}

	public void closeYoutube() {
		this.gameSwitcher (true);
		WebViewState._clickBackBtn ();

		if(OnVideoClosed != null)
			OnVideoClosed();
	}

	public void OnLoadYoutubeComplete() {
		WebViewState.HandleOnLoadComplete ();
	}

	public void getAllSystemApps() {
		KidMode.getAllSystemApps();
	}

	public void onTestingContentRefreshFinish(string contents) {;
		KidMode.onTestingContentRefreshFinish(contents);
	}

	public void Start()
	{
		_Debug.mode = OutputMode.DISABLE;
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");	
		jc.CallStatic("setImagePath", Application.persistentDataPath + "/ImageCache/");
		
		#endif

		getAllSystemApps ();
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.orientation = ScreenOrientation.AutoRotation;

		//honda added: decrypt and encrypt keystore and PWD
		if (!File.Exists (Application.persistentDataPath + "/data.dat"))
		{
			StartCoroutine(GetPwd());
		}
		else
		{
			getPwdStore();
		}
		
		if (!File.Exists (Application.persistentDataPath + ZoodlesConstants.CLIENT_CERT))
		{
			StartCoroutine(GetCert());
		}
		else
		{
			getRealCer();
			Server.setCer(new X509Certificate2 (Application.persistentDataPath + ZoodlesConstants.CLIENT_CERT, m_pwdStore.cerPwd));
			setEncCer();
		}
		Debug.Log(SystemInfo.deviceUniqueIdentifier);

		Game.getCerPwd();

		//end
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


		#if UNITY_EDITOR
		//honda
//		PlayerPrefs.DeleteAll();
//
//		//Kev .... for deleting kid files
//		File.Delete( Application.persistentDataPath + "/kidList.txt");
//
//		File.Delete( Application.persistentDataPath + "/kidList_temp.txt");
//
//		File.Delete( Application.persistentDataPath + "/kidList_backup.txt");
//
//		DirectoryInfo dataDir = new DirectoryInfo(Application.persistentDataPath);
//		dataDir.Delete(true);
		#endif

		m_request = new RequestQueue ();
		isClientIdCompleted = false;
		isPremiumCompleted = false;
		testTimes = 0;
		//check if time left data expired or not, if expired, remove the item
		SessionHandler.updateKidsLocalTimeLeftFile();

		//set version text
		GameObject versionObject = GameObject.FindGameObjectWithTag("Version");
		if (versionObject != null)
		{
			Text versionText = versionObject.GetComponent<Text>();
			versionText.text = CurrentBundleVersion.version;
		}
		string[] keys = {"version"};
		string[] values = {CurrentBundleVersion.version};
		CrittercismAndroid.SetMetadata(keys, values);
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

	bool isPress = false;
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
		
		if (Application.internetReachability != NetworkReachability.NotReachable && !KidMode.isAirplaneModeOn())
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
	
	private void getClientIdComplete(HttpsWWW p_response)
	{
		if(p_response.error == null)
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			Debug.Log("client id " + double.Parse(l_data["id"].ToString()) + " request completed");
			SessionHandler.getInstance ().clientId = l_data.ContainsKey("id") ? double.Parse(l_data["id"].ToString()) : 0;
			
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
//					Game game = m_gameController.game;
					m_gameController.getUI().createScreen(UIScreen.NO_INTERNET, false, 6);
				}
				//cynthia vzw
//				Game game = GameObject.FindWithTag("GameController").GetComponent<Game>();
//				game.gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
				//setErrorMessage( m_gameController, Localization.getString(Localization.TXT_STATE_0_FAIL), Localization.getString(Localization.TXT_STATE_0_FAIL_MESSAGE) );
				//vzw end
			}
		}
	}
	
	private void getCheckComplete(HttpsWWW p_response)
	{

		if (p_response != null && p_response.error == null)
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

	private void showSpinnerWithFetchingContents()
	{
		Debug.Log("spinner: show spinner");

		m_gameController.getUI().createScreen(UIScreen.LOADING_SPINNER_ELEPHANT, false, 12);
		Debug.Log("spinner: start webcontent request");

		Invoke("startWebContentRequest", 10);
	}

	private void startWebContentRequest()
	{
		Game game = m_gameController.game;
		game.user.contentCache.startRequests(onLoadingCompleted);
	}

	private void onLoadingCompleted()
	{
		Debug.Log("spinner: remove spinner");
		m_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);

		Debug.Log("spinner: webcontent list = " + SessionHandler.getInstance().webContentList.Count);
	}

	private void fetchDrawingList()
	{
		Debug.Log("spinner: start drawing request");
		Invoke("startDrawingRequest", 10);
	}

	private void startDrawingRequest()
	{
		SessionHandler.getInstance().drawingListRequest();
	}

	public void callParentGate()
	{
		UIManager l_ui = m_gameController.getUI();
		UICanvas l_backScreen = l_ui.findScreen(UIScreen.SPLASH_BACKGROUND);
		if (l_backScreen == null)
		{
			l_backScreen = l_ui.createScreen(UIScreen.SPLASH_BACKGROUND,true, -1);
			SplashBackCanvas l_splashBack = l_backScreen as SplashBackCanvas;
			l_splashBack.setDown();
		}
		m_gameController.connectState(ZoodleState.BIRTHYEAR, ZoodleState.REGION_LANDING);
		m_gameController.changeState(ZoodleState.BIRTHYEAR);
	
	}


	public bool checkInternet()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable
		    || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected())
		{
			Game game = GameObject.FindWithTag("GameController").GetComponent<Game>();
			game.gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
			
			return false;
		}
		return true;
	}

	public void showNoInternetScreen()
	{
		UICanvas error_message = m_gameController.getUI().findScreen(UIScreen.NO_INTERNET);
		if (error_message == null)
			m_gameController.getUI().createScreen(UIScreen.NO_INTERNET, false, 15);
	}

	public void showNoInternetScreen2()
	{
		UICanvas error_message = m_gameController.getUI().findScreen(UIScreen.ERROR_MESSAGE);
		if (error_message == null)
			m_gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 20);
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

	//honda added: encrypt or decrypt keystore and PWD
	private IEnumerator GetCert ()
	{
		string l_url = string.Empty;
		if (Application.platform == RuntimePlatform.Android)
		{
			l_url = "jar:file://"+Application.dataPath+"!/assets"+ZoodlesConstants.CLIENT_CERT;
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			l_url =Application.dataPath + "/Raw/" +ZoodlesConstants.CLIENT_CERT;
		}
		else
		{
			l_url = "file://" + Application.dataPath + "/StreamingAssets/"+ZoodlesConstants.CLIENT_CERT;
		}
		WWW download = new WWW (l_url);
		yield return download;
		if (download.error != null) {
			Debug.Log ("Error downloading: " + download.error);
		} else {
			File.WriteAllBytes (Application.persistentDataPath + ZoodlesConstants.CLIENT_CERT , download.bytes);
			//real
			getRealCer();
			Server.setCer(new X509Certificate2 (Application.persistentDataPath + ZoodlesConstants.CLIENT_CERT, m_pwdStore.cerPwd));
			setEncCer();
			//enc
		}
	}
	
	private IEnumerator GetPwd ()
	{
		string l_url = string.Empty;
		if (Application.platform == RuntimePlatform.Android)
		{
			l_url = "jar:file://"+Application.dataPath+"!/assets/data.dat";
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			l_url =Application.dataPath + "/Raw/data.dat";
		}
		else
		{
			l_url = "file://" + Application.dataPath + "/StreamingAssets/data.dat";
		}
		WWW download = new WWW (l_url);
		yield return download;
		if (download.error != null) {
			Debug.Log ("Error downloading: " + download.error);
		} else {
			File.WriteAllBytes (Application.persistentDataPath + "/data.dat" , download.bytes);
			getPwdStore();
		}
	}

	public void getPwdStore()
	{
		FileStream fs = new FileStream(Application.persistentDataPath + "/data.dat", FileMode.Open);
		BinaryFormatter bf = new BinaryFormatter();
		m_pwdStore = bf.Deserialize (fs) as PwdStore;
		//for static pwd use
		_pwdStore = m_pwdStore;
//		Debug.Log("PwdStore: " + _pwdStore.cerPwd + " & " + _pwdStore.encPwd);
		fs.Close ();
	}

	private void getRealCer()
	{
		FileEncrypt.DecryptFile (Application.persistentDataPath + ZoodlesConstants.CLIENT_CERT,m_pwdStore.encPwd,progress);
	}
	
	private void setEncCer()
	{
		FileEncrypt.EncryptFile (Application.persistentDataPath + ZoodlesConstants.CLIENT_CERT,m_pwdStore.encPwd,progress);
	}
	
	public void progress(int max, int value)
	{
		_Debug.log (value + "/" + max);
	}

	//for cathy
	public static void decryptCer()
	{
		FileEncrypt.DecryptFile(Application.persistentDataPath + ZoodlesConstants.CLIENT_CERT,_pwdStore.encPwd, (int max, int value) => {Debug.Log(value + "/" + max);});
	}

	public static void encrpytCer()
	{
		FileEncrypt.EncryptFile(Application.persistentDataPath + ZoodlesConstants.CLIENT_CERT,_pwdStore.encPwd, (int max, int value) => {Debug.Log(value + "/" + max);});
	}

	public static void getCerPwd()
	{
//		Debug.Log("print pwd: " + _pwdStore.cerPwd);
		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");	
		jc.CallStatic("cerPwd", _pwdStore.cerPwd);

		#endif
	}


	public void onPaymentSuccess(string test){

		if(OnPaymentSuccess != null)
			OnPaymentSuccess();

		DebugController.Instance.showStatus("Success " );
	}

	public void onPaymentError(string test){
		
		DebugController.Instance.showStatus("Error " );
	}
	//end

	public void setPDMenuBarVisible(bool visible, bool isChildUpdated)
	{
		if (pdMenuBar != null)
			pdMenuBar.setPDMenuBarVisible(visible, isChildUpdated);
	}

	public void removePDMenuBar()
	{
		if (pdMenuBar != null)
			pdMenuBar.removePDMenuBar();
	}

	public void updateChildSelectorAndCurrentKidInfo()
	{
		if (pdMenuBar != null)
			pdMenuBar.updateChildSelectorAndCurrentKidInfo();
	}

	private PwdStore m_pwdStore;
	private static PwdStore _pwdStore;
	//end

	private GameController m_gameController;
	private User m_user;
	private WWW m_www;
	private bool m_delayedParentDashboard;

}
