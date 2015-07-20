using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System;

public class SessionHandler 
{
    private SessionHandler()
    {
		m_token = new Token();
		_load();
    }

    public static SessionHandler getInstance()
    {
        if (m_instance == null)
            m_instance = new SessionHandler();

        return m_instance;
    }
	
    public Kid currentKid
    {
        get 
		{
			if(m_kid == null) {
				int currentKidId = LoadCurrentKid();
				if(currentKidId != null && m_kidList != null) {
					for(int i = 0; i < m_kidList.Count; i++) {
						if(currentKidId == m_kidList[i].id) {
							m_kid = m_kidList[i];
							break;
						}
					}
				}
			}
			if(null == m_kid)
			{
				if(null != m_kidList && m_kidList.Count > 0) {
					m_kid = m_kidList[0];
				}
			}
			return m_kid;   
		}
        set 
		{
			m_appList = null;
			Kid l_lastKid = m_kid;
			m_kid = value;
			SessionHandler.getInstance().drawingList = null;

			if (l_lastKid != m_kid)
				GAUtil.changeKid();
		}
    }

	public List<Kid> kidList
	{
		get {   return m_kidList;   }
		set 
		{  
			m_kidList = value; 
		}
	}

	public Token token
    {
        get { return m_token;   }
    }

	public bool passwordVerified
	{
		get { return m_passwordVerified;   }
		set { m_passwordVerified = value;  }
	}

	public bool hasPin
	{
		get { return m_hasPin;   }
		set { m_hasPin = value;  }
	}

	public bool childLockSwitch
	{
		get { return m_childLockSwitch;   }
		set
		{
			m_childLockSwitch = value;
			LocalSetting l_setting = LocalSetting.find( "User" );
			l_setting.setBool( ZoodlesConstants.USER_CHILDLOCK_SWITCH, m_childLockSwitch );
		}
	}

	public string selectAvatar
	{
		get { return m_selectedAvatar;   }
		set { m_selectedAvatar = value;  }
	}

    public double clientId
    {
        get;
        set;
    }

	public WebContent currentContent
	{
		get;
		set;
	}

    public Book currentBook
    {
        get;
        set;
    }

	public Drawing currentDrawing
	{
		get;
		set;
	}

	public BookReading currentBookReading
	{
		get;
		set;
	}

	public string errorName
	{
		get { return m_errorName;}
		set { m_errorName = value;}
	}

	public string errorMessage
	{
		get { return m_errorMessage;}
		set { m_errorMessage = value;}
	}

	public string inputedChildName
	{
		get { return m_inputedChildName;}
		set { m_inputedChildName = value;}
	}
	
	public string inputedbirthday
	{
		get { return m_inputedbirthday;}
		set { m_inputedbirthday = value;}
	}

	public int pin
	{
		get { return m_pin;}
		set
		{
			m_pin = value;
			LocalSetting l_setting = LocalSetting.find( "User" );
			l_setting.setInt( ZoodlesConstants.USER_PIN, m_pin );
		}
	}

	public string url
	{
		get { return m_url;}
		set { m_url = value;}
	}

	public Hashtable parameter
	{
		get { return m_parameter;}
		set { m_parameter = value;}
	}

	public string returnJson
	{
		get { return m_returnJson;}
		set { m_returnJson = value;}
	}

	public string handlePage
	{
		get { return m_handlePage;}
		set { m_handlePage = value;}
	}

	public string username
	{
		get { return m_username;}
		set
		{
			m_username = value;
			LocalSetting l_setting = LocalSetting.find( "User" );
			l_setting.setString( ZoodlesConstants.USER_NAME, m_username );
		}
	}

	public CallMethod callMethod
	{
		get { return m_callMethod;}
		set { m_callMethod = value;}
	}

	public string purchaseObject
	{
		get{return m_purchaseObject;}
		set{m_purchaseObject = value;}
	}

	public bool verifyBirth
	{
		get{return m_verifyBirth;}
		set
		{
			m_verifyBirth = value;
			LocalSetting l_setting = LocalSetting.find( "User" );
			l_setting.setBool( ZoodlesConstants.USER_VERIFY_BIRTHYEAR, m_verifyBirth );
		}
	}

	public string childLockPassword
	{
		get{return m_childLockPassword;}
		set
		{
			m_childLockPassword = value;
			LocalSetting l_setting = LocalSetting.find( "User" );
			l_setting.setString( ZoodlesConstants.CHILD_LOCK_PASSWORD, m_childLockPassword );
		}
	}

	public int invokeCallServerState
	{
		get{return m_invokeCallServerState;}
		set{m_invokeCallServerState = value;}
	}

	public List<SingleCall> callList
	{
		get{return m_callList;}
		set{m_callList = value;}
	}

	public bool freeWeeklyApp
	{
		get{return m_freeWeeklyApp;}
		set{m_freeWeeklyApp = value;}
	}

	public bool enableChildLockPin
	{
		get{return m_enableChildLockPin;}
		set{m_enableChildLockPin = value;}
	}

	public bool newAddApp
	{
		get{return m_newAddApp;}
		set{m_newAddApp = value;}
	}

	public int musicVolum
	{
		get{return m_musicVolum;}
		set{m_musicVolum = value;}
	}

	public int effectsVolum
	{
		get{return m_effectsVolum;}
		set{m_effectsVolum = value;}
	}

	public int masterVolum
	{
		get{return m_masterVolum;}
		set{m_masterVolum = value;}
	}

	public bool smartSelect
	{
		get{return m_smartSelect;}
		set{m_smartSelect = value;}
	}

	public bool allowCall
	{
		get{return m_allowCall;}
		set{m_allowCall = value;}
	}

	public bool tip
	{
		get{return m_tip;}
		set{m_tip = value;}
	}

	public Hashtable appOwn
	{
		get{return m_appOwn;}
		set{m_appOwn = value;}
	}
	public List<object> appList
	{
		get{return m_appList;}
		set{m_appList = value;}
	}

	public List<Book> bookList
	{
		get{return m_bookList;}
		set
		{
			if (m_bookList != null)
			{
				int l_numBooks = m_bookList.Count;
				for (int i = 0; i < l_numBooks; ++i)
				{
					Book l_book = m_bookList[i];
					l_book.dispose();
				}
			}
			m_bookList = value;
		}
	}

	public List<Drawing> drawingList
	{
		get{return m_drawingList;}
		set
		{
			if (m_drawingList != null)
			{
				int l_numDraws = m_drawingList.Count;

				for (int i = 0; i < l_numDraws; ++i)
				{
					Drawing l_drawing = m_drawingList[i];
					l_drawing.dispose();
				}
			}
			m_drawingList = value;
		}
	}

	public List<Kid> recordKidList
	{
		get
		{
			if( null == m_recordKidList )
				m_recordKidList = new List<Kid>();
			return m_recordKidList;
		}
		set{m_recordKidList = value;}
	}

	public List<object> webContentList
	{
		get
		{
			if( null == m_webContentList )
				m_webContentList = new List<object>();
			return m_webContentList;
		}
		set{m_webContentList = value;}
	}

	public List<object> bookContentList
	{
		get
		{
			if( null == m_bookContentList )
				m_bookContentList = new List<object>();
			return m_bookContentList;
		}
		set{m_bookContentList = value;}
	}

	public Hashtable bookTable
	{
		get
		{
			if( null == m_bookTable )
				m_bookTable = new Hashtable();
			return m_bookTable;
		}
		set
		{
			if (m_bookTable != value)
			{
				foreach (object obj in m_bookTable.Keys)
				{
					Book l_book = m_bookTable[obj] as Book;
					l_book.dispose();
				}
			}

			m_bookTable = value;
		}
	}

	public Hashtable readingTable
	{
		get
		{
			if( null == m_readingTable )
				m_readingTable = new Hashtable();
			return m_readingTable;
		}
		set{m_readingTable = value;}
	}

	public string GemsJson
	{
		get{return m_gemsJson;}
		set{m_gemsJson = value;}
	}

	public string PremiumJson
	{
		get{return m_premiumJson;}
		set{m_premiumJson = value;}
	}

	public bool CreateChild
	{
		get { return m_createChild;   }
		set { m_createChild = value;  }
	}

	public bool SignInFail
	{
		get { return m_signInFail;   }
		set { m_signInFail = value;  }
	}

	public bool flashInstall
	{
		get { return m_flashInstall;   }
		set { m_flashInstall = value;  }
	}

	public bool IsParent
	{
		get{return m_isParent;}
		set{m_isParent = value;}
	}

	public SettingCache settingCache
	{
		get{return m_settingCache;}
		set{m_settingCache = value;}
	}

	public string creditCardNum
	{
		get{return m_creditCardNum;}
		set{m_creditCardNum = value;}
	}

	public string cardMonth
	{
		get{return m_cardMonth;}
		set{m_cardMonth = value;}
	}

	public string cardYear
	{
		get{return m_cardYear;}
		set{m_cardYear = value;}
	}

	public string deviceName
	{
		get{return m_deviceName;}
		set{m_deviceName = value;}
	}

	public int renewalPeriod
	{
		get{return m_renewalPeriod;}
		set{m_renewalPeriod = value;}
	}

	public RequestQueue appRequest
	{
		get{return m_request;}
		set{m_request = value;}
	}

	public RequestQueue bookRequest
	{
		get{return m_bookRequest;}
		set{m_bookRequest = value;}
	}
	public void clearUserData()
	{
		m_kid   = null;
		m_kidList = null;
		m_passwordVerified = false;
		m_selectedAvatar = null;
		m_hasPin = false;
		m_inputedChildName = "";
		m_inputedbirthday = "";
		m_errorName = "";
		m_errorMessage = "";
		m_url = "";
		m_returnJson = "";
		m_handlePage = "";
		m_parameter = null;
		m_pin = 0;
		m_purchaseObject = "";
		m_childLockSwitch = true;
		m_verifyBirth = true;
		m_childLockPassword = "";
		m_invokeCallServerState = -1;
		m_callList = null;
		m_freeWeeklyApp = true;
		m_smartSelect = true;
		m_newAddApp = true;
		m_musicVolum = 0;
		m_effectsVolum = 0;
		m_masterVolum = 0;
		m_allowCall = true;
		m_tip = true;
		//need to clear token. if not, previous token info still exists after new Token(). 
		m_token.clear();
		m_token = new Token();
		m_username = "";
		m_callMethod = CallMethod.NULL;
		m_appList = null;
		m_bookList = null;
		m_drawingList = null;
		m_recordKidList = null;
		m_gemsJson = string.Empty;
		m_premiumJson = string.Empty;
		m_createChild = true;
		m_isParent = false;
		m_signInFail = false;
		m_webContentList = null;
		m_bookContentList = null;
		m_bookTable = null;
		m_readingTable = null;
		m_allowCall = KidMode.incomingCallsEnabled();
		m_creditCardNum = string.Empty;
		m_cardMonth = string.Empty;
		m_cardYear = string.Empty;
		m_deviceName = string.Empty;
		m_renewalPeriod = 0;
		m_settingCache = new SettingCache();
	}

	private void _load()
	{
		LocalSetting l_setting = LocalSetting.find( "User" );
		m_username = l_setting.getString( ZoodlesConstants.USER_NAME, "" );
		m_allowCall = KidMode.incomingCallsEnabled();
		////
		m_pin = l_setting.getInt( ZoodlesConstants.USER_PIN, 0 );
		m_hasPin = (m_pin > 0);
		m_verifyBirth = l_setting.getBool (ZoodlesConstants.USER_VERIFY_BIRTHYEAR,true);
		m_childLockPassword = l_setting.getString (ZoodlesConstants.CHILD_LOCK_PASSWORD,string.Empty);
		m_childLockSwitch = l_setting.getBool (ZoodlesConstants.USER_CHILDLOCK_SWITCH,false);

		try {

			List<Kid> l_kidList = new List<Kid>();
			String str = SessionHandler.LoadKidList();

			if (str != null && str.Length > 0)
			{
				ArrayList l_data = MiniJSON.MiniJSON.jsonDecode(str) as ArrayList;
				if (l_data != null)
				{
					foreach (object o in l_data)
					{
						Kid l_kid = new Kid( o as Hashtable );
//						if (this.selectAvatar !=  null)
//							kid.kid_photo = Resources.Load("GUI/2048/common/avatars/" + SessionHandler.getInstance().selectAvatar) as Texture2D;

						l_kidList.Add( l_kid );
					}
					m_kidList = l_kidList;
				}
			}
		} catch (Exception e) {
			Debug.Log(e);
		}
		
	}

	private void LoadKidListDatas() {
		try {
			
			List<Kid> l_kidList = new List<Kid>();
			String str = SessionHandler.LoadKidList();
			
			if (str != null && str.Length > 0)
			{
				ArrayList l_data = MiniJSON.MiniJSON.jsonDecode(str) as ArrayList;
				if (l_data != null)
				{
					foreach (object o in l_data)
					{
						Kid l_kid = new Kid( o as Hashtable );

						l_kidList.Add( l_kid );
					}
					m_kidList = l_kidList;
				}
			}
		} catch (Exception e) {
		}
	}
 
	public void addBook (int p_id, Book p_book)
	{
		if (null == m_bookTable) 
		{
			m_bookTable = new Hashtable();
		}

		if (m_bookTable.ContainsKey(p_id))
		{
			Book l_book = m_bookTable[p_id] as Book;
			l_book.dispose();
		}
		m_bookTable[p_id] = p_book;
	}

	public void initSettingCache()
	{
		m_settingCache.childLockSwitch = childLockSwitch;
		m_settingCache.verifyBirth = verifyBirth;
		m_settingCache.freeWeeklyApp = freeWeeklyApp;
		m_settingCache.smartSelect = smartSelect;
		m_settingCache.newAddApp = newAddApp;
		m_settingCache.musicVolum = musicVolum;
		m_settingCache.effectsVolum = effectsVolum;
		m_settingCache.masterVolum = masterVolum;
		m_settingCache.allowCall = allowCall;
		m_settingCache.tip = tip;
		m_settingCache.childLockPassword = childLockPassword;
		m_settingCache.active = false;
	}

	public void getAllKidApplist()
	{
		if(null != m_kidList && m_kidList.Count > 0)
		{
			if(null == m_request)
				m_request = new RequestQueue();
			if(null == m_iconRequest)
				m_iconRequest = new RequestQueue();
			foreach(Kid l_kid in m_kidList)
			{
				m_request.add(new NewGetAppByPageRequest(l_kid,"google",1));
				m_request.add(new GetTopRecommandRequest(ZoodlesConstants.GOOGLE,l_kid));
			}
			m_request.request(RequestType.RUSH);
		}
	}

	public void getSingleKidApplist(Kid p_kid)
	{
		if(null == m_request)
			m_request = new RequestQueue(); 
		if(null != p_kid)
		{
			m_request.reset();
			if(null == m_singleKidRequest)
				m_singleKidRequest = new RequestQueue();
			if(null != p_kid.appList)
				p_kid.appList = new List<object>();
			m_request.add(new NewGetAppByPageRequest(p_kid,"google",1));
			m_request.add(new GetTopRecommandRequest(ZoodlesConstants.GOOGLE,p_kid));
			m_request.request(RequestType.RUSH);
		}
	}

	public void getBooklist()
	{
		if (null == m_bookRequest)
			m_bookRequest = new RequestQueue ();
		m_bookRequest.add(new GetBookRequest("false"));
		m_bookRequest.request ();
	}

	public void getBookIcon()
	{
		if (null == m_bookRequest)
			m_bookRequest = new RequestQueue ();

		if(null != m_bookList && m_bookList.Count > 0)
		{
			foreach (Book l_book in m_bookList)
			{
				m_bookRequest.add(new BookIconRequest(l_book));
			}
		}
		m_bookRequest.request (RequestType.RUSH);
	}

	public void getAllAppIcon()
	{
		if (null == m_iconRequest)
			m_iconRequest = new RequestQueue ();
		if(null != m_kidList && m_kidList.Count > 0)
		{
			foreach(Kid l_kid in m_kidList)
			{
				List<object> l_appList = l_kid.appList;
				foreach (App l_app in l_appList)
				{
					if(null == l_app.icon && l_app.iconUrl != null && !string.Empty.Equals(l_app.iconUrl))
					{
						m_iconRequest.add(new IconRequest(l_app));
					}
				}
			}
			m_iconRequest.request(RequestType.RUSH);
		}
	}

	public void getAppIconByKid(Kid p_kid)
	{
		if (null == m_iconRequest)
			m_iconRequest = new RequestQueue ();
		//m_iconRequest.reset ();
		List<object> l_appList = p_kid.appList;
		foreach (App l_app in l_appList)
		{
			if(null == l_app.icon && l_app.iconUrl != null && !string.Empty.Equals(l_app.iconUrl))
			{
				m_iconRequest.add(new IconRequest(l_app));
			}
		}
		m_iconRequest.request(RequestType.RUSH);
	}

	public void getAppIconByApp(App p_app)
	{
		if (null == m_iconRequest)
			m_iconRequest = new RequestQueue ();

		if(null == p_app.icon && p_app.iconUrl != null && !string.Empty.Equals(p_app.iconUrl))
		{
			m_iconRequest.add(new IconRequest(p_app));
		}
		m_iconRequest.request(RequestType.RUSH);
	}
	
	public void resetSetting()
	{
		childLockSwitch = m_settingCache.childLockSwitch;
		verifyBirth = m_settingCache.verifyBirth;
		freeWeeklyApp = m_settingCache.freeWeeklyApp;
		smartSelect = m_settingCache.smartSelect;
		newAddApp = m_settingCache.newAddApp;
		musicVolum = m_settingCache.musicVolum;
		effectsVolum = m_settingCache.effectsVolum;
		masterVolum = m_settingCache.masterVolum;
		allowCall = m_settingCache.allowCall;
		tip = m_settingCache.tip;
		childLockPassword = m_settingCache.childLockPassword;
		settingCache.active = false;
	}


	public void resetKidCacheLists()
	{
		webContentList = null;
		bookContentList = null;
	}
	
	public void resetKidCache()
	{
		webContentList = null;
		bookContentList = null;
		bookTable = null;
		readingTable = null;
	}

	// Cynthia: vzw
	public static void SaveKidList(String str) 
	{
		try {

			StreamWriter sw = File.CreateText (KIDLIST_PATH_TEMP);
			sw.Write (str);
			sw.Flush ();
			sw.Close ();

			if (File.Exists(KIDLIST_PATH))
			{
				File.Replace (KIDLIST_PATH_TEMP, KIDLIST_PATH, KIDLIST_PATH_BACKUP); 
			}
			else
			{
				File.Move(KIDLIST_PATH_TEMP, KIDLIST_PATH);
			}

		}
		catch (Exception e) {

			Debug.Log(e);
		}
	}
	
	public static String LoadKidList() 
	{
		string str = null;
		try {

			str = File.ReadAllText(KIDLIST_PATH);
		}
		catch (Exception e) {
			Debug.Log(e);
		}
		return str;	
	}

	public static int LoadCurrentKid() 
	{
		string str = null;
		try {
			
			str = File.ReadAllText(KID_CURRENT);
		}
		catch (Exception e) {
			Debug.Log(e);
			return -1;
		}
		return Int32.Parse(str);	
	}

	public static void SaveCurrentKid(int kidId) 
	{
		try {
			
			File.WriteAllText(KID_CURRENT,  Convert.ToString(kidId));
		}
		catch (Exception e) {
			
			Debug.Log(e);
		}
	}


	private static string KIDLIST_PATH	= Application.persistentDataPath + "/kidList.txt";
	private static string KIDLIST_PATH_TEMP	= Application.persistentDataPath + "/kidList_temp.txt";
	private static string KIDLIST_PATH_BACKUP	= Application.persistentDataPath + "/kidList_backup.txt";

	private static string KID_CURRENT	= Application.persistentDataPath + "/current.txt";

	// end vzw

	private Kid     m_kid   = null;
    private Token   m_token = null;
	private List<Kid> m_kidList = null;
	private bool 	m_passwordVerified;
	private string  m_selectedAvatar = null;
	private bool 	m_hasPin = false;
	private int 	m_pin;
	private string m_username;

	private string m_errorName;
	private string m_errorMessage;

	//record call server's message and result.
	private string m_url;
	private Hashtable m_parameter;
	private string m_returnJson = "";
	private CallMethod m_callMethod = CallMethod.NULL;
	//record which page send the call.
	private string m_handlePage;
	//Store the state which invoke callServiceState. For error state.
	private int m_invokeCallServerState = -1;
	//Calls
	private List<SingleCall> m_callList;
	//upsell
	private string m_purchaseObject;

	private bool m_signInFail = false;

	//setting
	private SettingCache m_settingCache = new SettingCache();
	//action to turn off child lock.
	private bool m_childLockSwitch = true;
	private bool m_verifyBirth = true;
	private bool m_freeWeeklyApp = true;
	private bool m_smartSelect = true;
	private bool m_newAddApp = true;
	private int  m_musicVolum = 0;
	private int  m_effectsVolum = 0;
	private int  m_masterVolum = 0;
	private bool m_allowCall = true;
	private bool m_tip = true;
	private bool m_enableChildLockPin;
	//4 digit pin to turn off child lock.
	private string m_childLockPassword;

	private string m_inputedChildName = "";
	private string m_inputedbirthday = "";

	private bool m_flashInstall = false;

	private List<object> m_appList = null;
	private List<Book> m_bookList = null;
	private List<Drawing> m_drawingList = null;
	private List<Kid> m_recordKidList = null;
	private bool m_createChild = true;
	private Hashtable m_appOwn;
	private bool m_isParent = false;

	private List<object> m_webContentList = null;
	private List<object> m_bookContentList = null;
	private Hashtable m_bookTable = null;
	private Hashtable m_readingTable = null;

	//gems and premium.
	private string m_gemsJson = string.Empty;
	private string m_premiumJson = string.Empty;

	private string m_creditCardNum = string.Empty;
	private string m_cardMonth = string.Empty;
	private string m_cardYear = string.Empty;

	private string m_deviceName = string.Empty;
	private int m_renewalPeriod = 0;
	private RequestQueue m_request;
	private RequestQueue m_iconRequest;
	private RequestQueue m_singleKidRequest;
	private RequestQueue m_bookRequest;
    private static SessionHandler m_instance = null;





	// get kid info except app list, top recommend apps
	public void fetchCurrentKidDetail()
	{
		RequestQueue l_request = new RequestQueue ();
		l_request.add (new GetKidRequest(currentKid.id, onRequestComplete));
		l_request.request ();
	}
	
	// private
	private void onRequestComplete(WWW p_response)
	{
		if (p_response.error != null)
		{
			//if (!SessionHandler.getInstance().token.isExist()) //cynthia
			{
//				m_gameController.changeState(ZoodleState.SERVER_ERROR);  
			}
		}
		else
		{
			string l_string = "";
			
			l_string = UnicodeDecoder.Unicode(p_response.text);
			l_string = UnicodeDecoder.UnicodeToChinese(l_string);
			l_string = UnicodeDecoder.CoverHtmlLabel(l_string);
			
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(l_string) as Hashtable;
			Kid l_currentKid = new Kid(l_data);

			SessionHandler.getInstance().currentKid = l_currentKid;
			
			for (int i = 0; i < kidList.Count; ++i)
			{
				if (kidList[i].id == l_currentKid.id)
				{
					if(null != kidList[i].appList)
						l_currentKid.appList = kidList[i].appList;
					if(null != kidList[i].topRecommendedApp)
						l_currentKid.topRecommendedApp = kidList[i].topRecommendedApp;
					kidList[i] = l_currentKid;
					break;
				}
			}
		}
	}








}
