using UnityEngine;
using System.Collections;

public class PromoteLanguagesCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);

		_setupElement();
	}

	public override void update ()
	{
		base.update ();
	}

	public override void dispose (bool p_deep)
	{
		base.dispose (p_deep);
	}

	public override void enteringTransition ()
	{
		base.enteringTransition ();
		tweener.addAlphaTrack( 1.0f, 0.0f, 0.0f, onFadeFinish );
	}

	public override void exitingTransition ()
	{
		base.exitingTransition ();
	}

	public void setLanguageData ( ArrayList p_languageList )
	{
		DebugUtils.Assert( p_languageList != null);

		if( 0 == p_languageList.Count )
			return;

		m_data = p_languageList;
		_setupData();
	}

	//------------------ Private Implementation --------------------
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}
	
	private void _setupElement()
	{
		m_englishToggle 	= getView( "englishToggle" )	 as UIToggle;
		m_simpChineseToggle = getView( "simpChineseToggle" ) as UIToggle;
		m_tradChineseToggle = getView( "tradChineseToggle" ) as UIToggle;
		m_spanishToggle 	= getView( "spanishToggle" )	 as UIToggle;
		m_japaneseToggle 	= getView( "japaneseToggle" )	 as UIToggle;
		m_koreanToggle 		= getView( "koreanToggle" )		 as UIToggle;
		m_frenchToggle 		= getView( "frenchToggle" )		 as UIToggle;
		m_italianToggle 	= getView( "italianToggle" )	 as UIToggle;
		m_dutchToggle 		= getView( "dutchToggle" )		 as UIToggle;
		m_germanToggle 		= getView( "germanToggle" )		 as UIToggle;
	}

	private void _setupData()
	{
		foreach( Hashtable l_info in m_data )
		{
			switch( l_info["locale_symbol"].ToString() )
			{
				case EN :
					m_englishToggle.isOn = (bool)l_info["enabled"];
					break;
				case ZH_CN :
					m_simpChineseToggle.isOn = (bool)l_info["enabled"];
					break;
				case ZH_TW :
					m_tradChineseToggle.isOn = (bool)l_info["enabled"];
					break;
				case ES :
					m_spanishToggle.isOn = (bool)l_info["enabled"];
					break;
				case JA :
					m_japaneseToggle.isOn = (bool)l_info["enabled"];
					break;
				case KO :
					m_koreanToggle.isOn = (bool)l_info["enabled"];
					break;
				case FR :
					m_frenchToggle.isOn = (bool)l_info["enabled"];
					break;
				case IT :
					m_italianToggle.isOn = (bool)l_info["enabled"];
					break;
				case NL :
					m_dutchToggle.isOn = (bool)l_info["enabled"];
					break;
				case DE :
					m_germanToggle.isOn = (bool)l_info["enabled"];
					break;
			}
		}
	}

	private UIToggle m_englishToggle;
	private UIToggle m_simpChineseToggle;
	private UIToggle m_tradChineseToggle;
	private UIToggle m_spanishToggle;
	private UIToggle m_japaneseToggle;
	private UIToggle m_koreanToggle;
	private UIToggle m_frenchToggle;
	private UIToggle m_italianToggle;
	private UIToggle m_dutchToggle;
	private UIToggle m_germanToggle;

	private ArrayList m_data;

	private const string EN = "en";
	private const string ZH_CN = "zh-CN";
	private const string ZH_TW = "zh-TW";
	private const string ES = "es";
	private const string JA = "ja";
	private const string DE = "de";
	private const string KO = "ko";
	private const string FR = "fr";
	private const string IT = "it";
	private const string NL = "nl";
}