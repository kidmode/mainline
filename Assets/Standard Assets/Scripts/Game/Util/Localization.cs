using UnityEngine;
using System.Collections;

public class Localization : object 
{
	public static string Text_String                = "Text_String";

	public const string TXT_LABEL_WELCOME           = "TXT_LABEL_WELCOME";
	public const string TXT_LABEL_BY                = "TXT_LABEL_BY";
	public const string TXT_LABEL_LOADING           = "TXT_LABEL_LOADING";
	public const string TXT_BUTTON_TAP_CONTINUE     = "TXT_BUTTON_TAP_CONTINUE";
    public const string TXT_BUTTON_PARENT_DASHBOARD = "TXT_BUTTON_PARENT_DASHBOARD";
    public const string TXT_BUTTON_FACEBOOK         = "TXT_BUTTON_FACEBOOK";
    public const string TXT_BUTTON_EMAIL_INVITE     = "TXT_BUTTON_EMAIL_INVITE";
    public const string TXT_BUTTON_QUIT             = "TXT_BUTTON_QUIT";
    public const string TXT_LABEL_PROFILE_INFO      = "TXT_LABEL_PROFILE_INFO";
    public const string TXT_LABEL_TERMS_OF_SERVICE  = "TXT_LABEL_TERMS_OF_SERVICE";
    public const string TXT_LABEL_SIGNED_IN_AS      = "TXT_LABEL_SIGNED_IN_AS";
    public const string TXT_LABEL_PRIVACY_POLICY    = "TXT_LABEL_PRIVACY_POLICY";
    public const string TXT_LABEL_LOADING_ENTRANCE  = "TXT_LABEL_LOADING_ENTRANCE";
    public const string TXT_LABEL_WELCOME_BACK      = "TXT_LABEL_WELCOME_BACK";
    public const string TXT_LABEL_WELCOME_INFO      = "TXT_LABEL_WELCOME_INFO";
    public const string TXT_BUTTON_PROFILES         = "TXT_BUTTON_PROFILES";
    public const string TXT_BUTTON_MAPS             = "TXT_BUTTON_MAPS";
    public const string TXT_LABEL_NEWS_FEED         = "TXT_LABEL_NEWS_FEED";
    public const string TXT_BUTTON_ENTRANCE         = "TXT_BUTTON_ENTRANCE";
    public const string TXT_BUTTON_JUNGLE           = "TXT_BUTTON_JUNGLE";
    public const string TXT_LABEL_JUNGLE_HEADER     = "TXT_LABEL_JUNGLE_HEADER";
    public const string TXT_LABEL_JUNGLE_BODY       = "TXT_LABEL_JUNGLE_BODY";
    public const string TXT_BUTTON_SAVANNAH         = "TXT_BUTTON_SAVANNAH";
    public const string TXT_BUTTON_BACK             = "TXT_BUTTON_BACK";
    public const string TXT_LABEL_REGION_HEADER     = "TXT_LABEL_REGION_HEADER";
    public const string TXT_LABEL_REGION_BODY       = "TXT_LABEL_REGION_BODY";
    public const string TXT_LABEL_VIDEOS            = "TXT_LABEL_VIDEOS";
    public const string TXT_LABEL_GAMES             = "TXT_LABEL_GAMES";
    public const string TXT_LABEL_BOOKS             = "TXT_LABEL_BOOKS";
    public const string TXT_LABEL_ACTIVITIES        = "TXT_LABEL_ACTIVITIES";
    public const string TXT_TAB_FAVORITES           = "TXT_TAB_FAVORITES";
    public const string TXT_TAB_ALL_VIDEOS          = "TXT_TAB_ALL_VIDEOS";
    public const string TXT_TAB_ALL_GAMES           = "TXT_TAB_ALL_GAMES";
    public const string TXT_TAB_ALL_BOOKS           = "TXT_TAB_ALL_BOOKS";
    public const string TXT_TAB_ALL_ACTIVITY        = "TXT_TAB_ALL_ACTIVITY";
    public const string TXT_LABEL_FEATURED_VIDEOS   = "TXT_LABEL_FEATURED_VIDEOS";
    public const string TXT_LABEL_FEATURED_GAMES    = "TXT_LABEL_FEATURED_GAMES";
    public const string TXT_LABEL_FEATURED_BOOKS    = "TXT_LABEL_FEATURED_BOOKS";
    public const string TXT_LABEL_FEATURED_ACTIVITY = "TXT_LABEL_FEATURED_ACTIVITY";
    public const string TXT_LABEL_RECORD            = "TXT_LABEL_RECORD";
    public const string TXT_LABEL_PAUSE             = "TXT_LABEL_PAUSE";
    public const string TXT_LABEL_AUTHOR            = "TXT_LABEL_AUTHOR";
    public const string TXT_LABEL_ILLUSTRATOR       = "TXT_LABEL_ILLUSTRATOR";


	private string m_currLanguage   = "EN";
	private Hashtable m_dictionary  = new Hashtable();
	
	public Localization( Accessor p_accessor )
	{
		TextAsset l_asset   = Resources.Load( "Localization/localization" + m_currLanguage ) as TextAsset;
		Hashtable l_object  = MiniJSON.MiniJSON.jsonDecode( l_asset.text ) as Hashtable;
		Resources.UnloadAsset( l_asset );
		parseData( l_object );
	}
	
	public static string getString( string p_string )
	{
		if( null == s_instance )
			s_instance = new Localization( new Accessor() );
			
		string l_string = s_instance.lookup( p_string );
		if( null != l_string )
			return l_string;
		else
			return p_string.ToLower();
	}
	
	public static string getString( string p_string, ArrayList p_replaces )
	{
		if( null == s_instance )
			s_instance = new Localization( new Accessor() );
			
		string l_string = s_instance.lookup( p_string );
		if( null == l_string )
			l_string = p_string.ToLower();
		
		for (int i = 0; i < p_replaces.Count; ++i)
		{
			l_string = l_string.Replace("<" + (i + 1) + ">", p_replaces[i] as string);
		}
		
		return l_string;
	}
	
	public static void changeLanguage( string p_language )
	{
		if( null == s_instance )
			s_instance = new Localization( new Accessor() );
			
		s_instance.setLanguage( p_language );
	}
	
	public static string getCurrLanguage()
	{
		if( null == s_instance )
			s_instance = new Localization( new Accessor() );
			
		return s_instance.currLanguage;
	}
	
	public static bool isKey( string p_string ) 
	{
		if( null == s_instance )
			s_instance = new Localization( new Accessor() );
			
		return s_instance.checkKey( p_string );
	}
	
	
	//Instance functions
	public void setLanguage( string p_language )
	{
		m_dictionary.Clear();
		m_dictionary = new Hashtable();
		
		m_currLanguage = p_language;
		
		TextAsset l_asset = Resources.Load("Localization/localization" + m_currLanguage) as TextAsset;
		Hashtable l_object = MiniJSON.MiniJSON.jsonDecode( (l_asset).text) as Hashtable;
		Resources.UnloadAsset(l_asset);
		parseData( l_object );
	}
	
	private void parseData( Hashtable p_object )
	{	
		foreach ( string key in p_object.Keys )
		{
			m_dictionary[key] = p_object[key];
		}
	}
	
	public string currLanguage { get { return m_currLanguage; }}
	
	public string lookup( string p_string )
	{
		return (string)m_dictionary[ p_string ];
	}
	
	public bool checkKey( string p_string )
	{
		object l_obj = m_dictionary[ p_string ];
		
		return ( null != l_obj );
	}
	
	private static Localization s_instance;
	
}

public class Accessor{}
