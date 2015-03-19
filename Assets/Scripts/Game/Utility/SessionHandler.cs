using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
			if(null == m_kid)
			{
				if(null != m_kidList && m_kidList.Count > 0)
					m_kid = m_kidList[0];
			}
			return m_kid;   
		}
        set 
		{
			m_appList = null;
			Kid l_lastKid = m_kid;
			m_kid = value;

			if (l_lastKid != m_kid)
				GAUtil.changeKid();
		}
    }

	public List<Kid> kidList
	{
		get {   return m_kidList;   }
		set {   m_kidList = value;  }
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
		set { m_childLockSwitch = value;  }
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
		set{m_verifyBirth = value;}
	}

	public string childLockPassword
	{
		get{return m_childLockPassword;}
		set{m_childLockPassword = value;}
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
	public List<App> appList
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
		set{m_drawingList = value;}
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
			return m_webContentList;
		}
		set{m_webContentList = value;}
	}

	public List<object> bookContentList
	{
		get
		{
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
		m_settingCache = new SettingCache();
	}

	private void _load()
	{
		LocalSetting l_setting = LocalSetting.find( "User" );
		m_username = l_setting.getString( ZoodlesConstants.USER_NAME, "" );
		m_pin = l_setting.getInt( ZoodlesConstants.USER_PIN, 0 );
		m_hasPin = (m_pin > 0);
		m_allowCall = KidMode.incomingCallsEnabled();
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


	public void resetSetting()
	{
		childLockSwitch = m_settingCache.childLockSwitch;
		m_verifyBirth = m_settingCache.verifyBirth;
		m_freeWeeklyApp = m_settingCache.freeWeeklyApp;
		m_smartSelect = m_settingCache.smartSelect;
		m_newAddApp = m_settingCache.newAddApp;
		m_musicVolum = m_settingCache.musicVolum;
		m_effectsVolum = m_settingCache.effectsVolum;
		m_masterVolum = m_settingCache.masterVolum;
		m_allowCall = m_settingCache.allowCall;
		m_tip = m_settingCache.tip;
		m_childLockPassword = m_settingCache.childLockPassword;
	}


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

	private List<App> m_appList = null;
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

    private static SessionHandler m_instance = null;
}
