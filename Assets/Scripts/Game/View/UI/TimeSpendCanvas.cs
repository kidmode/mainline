using UnityEngine;
using System.Collections;

public class TimeSpendCanvas : UICanvas
{	
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		SetupLocalizition ();
		
		string l_mappingJson = LocalSetting.find("ServerSetting").getString( ZoodlesConstants.SUBJECTS,"" );
		
		DebugUtils.Assert( l_mappingJson != null );
		
		Hashtable l_jsonTable = MiniJSON.MiniJSON.jsonDecode( l_mappingJson ) as Hashtable;
		Hashtable l_jsonResponse = l_jsonTable["jsonResponse"] as Hashtable;
		ArrayList l_response = l_jsonResponse["response"] as ArrayList;
		foreach( Hashtable l_table in l_response)
		{
			switch( l_table["en_name"].ToString() )
			{
			case "Math":
				m_mathId = int.Parse( l_table["id"].ToString() );
				break;
			case "Reading":
				m_readingId = int.Parse( l_table["id"].ToString() );
				break;
			case "Science":
				m_scienceId = int.Parse( l_table["id"].ToString() );
				break;
			case "Social Studies":
				m_socialId = int.Parse( l_table["id"].ToString() );
				break;
			case "Cognitive Development":
				m_cognitiveId = int.Parse( l_table["id"].ToString() );
				break;
			case "Creative Development":
				m_creativeId = int.Parse( l_table["id"].ToString() );
				break;
			case "Life Skills":
				m_lifeSkillsId = int.Parse( l_table["id"].ToString() );
				break;
			}
		}
		
		_setupElement( );
	}
	
	public override void update()
	{
		base.update();
	}
	
	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}
	
	public override void enteringTransition()
	{
		base.enteringTransition();
		tweener.addAlphaTrack( 1.0f, 0.0f, 0.0f, onFadeFinish );
	}
	
	public void setData( ArrayList p_data)
	{
		DebugUtils.Assert( p_data != null);
		if( 0 == p_data.Count )
			return;
		this.m_data = p_data;
		_setupData ();
	}
	
	//------------------ Private Implementation --------------------
	
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}
	
	private void _setupElement()
	{
		m_avatarIcon		 = getView( "avatarIcon" )		 as UIImage;
		m_mathImage			 = getView( "mathImage" )		 as UIImage;
		m_readingImage		 = getView( "readingImage" )	 as UIImage;
		m_scienceImage		 = getView( "scienceImage" )	 as UIImage;
		m_socialImage		 = getView( "socialImage" )		 as UIImage;
		m_cognitiveImage	 = getView( "cognitiveImage" )	 as UIImage;
		m_creativeImage		 = getView( "creativeImage" )	 as UIImage;
		m_lifeSkillsImage	 = getView( "lifeSkillsImage" )	 as UIImage;
		
		m_mathLabel			 = getView( "mathPercentText" )			as UILabel;
		m_readingLabel		 = getView( "readingPercentText" )		as UILabel;
		m_scienceLabel		 = getView( "sciencePercentText" )		as UILabel;
		m_socialLabel		 = getView( "socialPercentText" )		as UILabel;
		m_cognitiveLabel	 = getView( "cognitivePercentText" )	as UILabel;
		m_creativeLabel		 = getView( "creativePercentText" )		as UILabel;
		m_lifeSkillsLabel	 = getView( "lifeSkillsPercentText" ) 	as UILabel;
		
		m_avatarIcon.setTexture( SessionHandler.getInstance().currentKid.kid_photo );
	}
	
	private void _setupData()
	{
		float l_sum = 0;
		float l_mathValue = 0;
		float l_readingValue = 0;
		float l_scienceValue = 0;
		float l_socialValue = 0;
		float l_cognitiveValue = 0;
		float l_creativeValue = 0;
		float l_lifeSkillsValue = 0;
		
		foreach( Hashtable l_table in m_data)
		{
			float l_value = float.Parse(l_table["value"].ToString());
			l_sum += l_value;
			
			if( int.Parse(l_table["subject_id"].ToString()) == m_mathId )
			{
				l_mathValue = l_value;
			}
			if( int.Parse(l_table["subject_id"].ToString()) == m_readingId )
			{
				l_readingValue = l_value;
			}
			if( int.Parse(l_table["subject_id"].ToString()) == m_scienceId )
			{
				l_scienceValue = l_value;
			}
			if( int.Parse(l_table["subject_id"].ToString()) == m_socialId )
			{
				l_socialValue = l_value;
			}
			if( int.Parse(l_table["subject_id"].ToString()) == m_cognitiveId )
			{
				l_cognitiveValue = l_value;
			}
			if( int.Parse(l_table["subject_id"].ToString()) == m_creativeId )
			{
				l_creativeValue = l_value;
			}
			if( int.Parse(l_table["subject_id"].ToString()) == m_lifeSkillsId )
			{
				l_lifeSkillsValue = l_value;
			}
		}

		float l_base = 0;

		if( l_sum > 0 )
		{
			m_mathImage.fillAmount 			= l_mathValue / l_sum + l_base;
			l_base += m_mathImage.fillAmount;

			m_readingImage.fillAmount 		= l_readingValue / l_sum + l_base;
			l_base += l_readingValue / l_sum;

			m_scienceImage.fillAmount 		= l_scienceValue / l_sum + l_base;
			l_base += l_scienceValue / l_sum;

			m_socialImage.fillAmount 		= l_socialValue / l_sum + l_base;
			l_base += l_socialValue / l_sum;

			m_cognitiveImage.fillAmount 	= l_cognitiveValue / l_sum + l_base;
			l_base += l_cognitiveValue / l_sum;

			m_creativeImage.fillAmount 		= l_creativeValue / l_sum + l_base;
			l_base += l_creativeValue / l_sum;

			m_lifeSkillsImage.fillAmount 	= l_lifeSkillsValue / l_sum + l_base;
			
			m_mathLabel.text 		= string.Format( "{0:P0}", l_mathValue / l_sum );
			m_readingLabel.text 	= string.Format( "{0:P0}", l_readingValue / l_sum );
			m_scienceLabel.text 	= string.Format( "{0:P0}", l_scienceValue / l_sum );
			m_socialLabel.text 		= string.Format( "{0:P0}", l_socialValue / l_sum );
			m_cognitiveLabel.text 	= string.Format( "{0:P0}", l_cognitiveValue / l_sum );
			m_creativeLabel.text 	= string.Format( "{0:P0}", l_creativeValue / l_sum );
			m_lifeSkillsLabel.text 	= string.Format( "{0:P0}", l_lifeSkillsValue / l_sum );
		}
	}

	
	private void SetupLocalizition()
	{
//		UILabel l_top = getView("titleText") as UILabel;
//		
//		l_top.text = Localization.getString( Localization.TXT_59_LABEL_TITLE );
		
//		UILabel l_math = getView("mathText") as UILabel;
//		UILabel l_reading = getView("readingText") as UILabel;
//		UILabel l_science = getView("scienceText") as UILabel;
//		UILabel l_social = getView("socialText") as UILabel;
//		UILabel l_cognitive = getView("cognitiveText") as UILabel;
//		UILabel l_creative = getView("creativeText") as UILabel;
//		UILabel l_life = getView("lifeSkillsText") as UILabel;
//
//		l_math.text = Localization.getString( Localization.TXT_COURSE_MATH );
//		l_reading.text = Localization.getString( Localization.TXT_COURSE_READING );
//		l_science.text = Localization.getString( Localization.TXT_COURSE_SCIENCE );
//		l_social.text = Localization.getString( Localization.TXT_COURSE_SOCIAL );
//		l_cognitive.text = Localization.getString( Localization.TXT_COURSE_COGNITIVE );
//		l_creative.text = Localization.getString( Localization.TXT_COURSE_CREATIVE );
//		l_life.text = Localization.getString( Localization.TXT_COURSE_LIFE );
	}
	
	private UIImage m_avatarIcon;
	private UIImage m_mathImage;
	private UIImage m_readingImage;
	private UIImage m_scienceImage;
	private UIImage m_socialImage;
	private UIImage m_cognitiveImage;
	private UIImage m_creativeImage;
	private UIImage m_lifeSkillsImage;
	
	private UILabel m_mathLabel;
	private UILabel m_readingLabel;
	private UILabel m_scienceLabel;
	private UILabel m_socialLabel;
	private UILabel m_cognitiveLabel;
	private UILabel m_creativeLabel;
	private UILabel m_lifeSkillsLabel;
	
	private int m_mathId;
	private int m_readingId;
	private int m_scienceId;
	private int m_socialId;
	private int m_cognitiveId;
	private int m_creativeId;
	private int m_lifeSkillsId;
	
	private ArrayList m_data;
}
