using UnityEngine;
using System.Collections;

public class OverallProgressCanvas : UICanvas
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		SetupLocalizition ();

		string l_mappingJson = LocalSetting.find("ServerSetting").getString( ZoodlesConstants.SUBJECTS,"" );
		
		DebugUtils.Assert( l_mappingJson != null );
		
		Hashtable l_jsonTable = MiniJSON.MiniJSON.jsonDecode( l_mappingJson ) as Hashtable;
		Hashtable l_jsonResponse = l_jsonTable["jsonResponse"] as Hashtable;
		if(null != l_jsonResponse)
		{
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
			_setupProfileInfo( );
		}
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
	
	private void _setupProfileInfo( )
	{
		m_mathSlider 		= getView( "mathSlider" ) 		as UISlider;
		m_readingSlider 	= getView( "readingSlider" ) 	as UISlider;
		m_scienceSlider 	= getView( "scienceSlider" ) 	as UISlider;
		m_socialSlider 		= getView( "socialSlider" ) 	as UISlider;
		m_cognitiveSlider 	= getView( "cognitiveSlider" ) 	as UISlider;
		m_creativeSlider 	= getView( "creativeSlider" ) 	as UISlider;
		m_lifeSkillsSlider 	= getView( "lifeSkillsSlider" ) as UISlider;
	}

	private void _setupData()
	{
		float l_mathValue = 0;
		float l_readingValue = 0;
		float l_scienceValue = 0;
		float l_socialValue = 0;
		float l_cognitiveValue = 0;
		float l_creativeValue = 0;
		float l_lifeSkillsValue = 0;

		float l_maxValue = 0;
		
		foreach( Hashtable l_table in m_data)
		{
			float l_value = float.Parse(l_table["value"].ToString());

			if( l_maxValue < l_value )
			{
				l_maxValue = l_value;
			}
			
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

		if( 0 == l_maxValue )
		{
			l_maxValue = 1;
		}

		m_mathSlider.value		 = l_mathValue / l_maxValue * 80;
		m_readingSlider.value	 = l_readingValue / l_maxValue * 80;
		m_scienceSlider.value	 = l_scienceValue / l_maxValue * 80;
		m_socialSlider.value	 = l_socialValue / l_maxValue * 80;
		m_cognitiveSlider.value	 = l_cognitiveValue / l_maxValue * 80;
		m_creativeSlider.value	 = l_creativeValue / l_maxValue * 80;
		m_lifeSkillsSlider.value = l_lifeSkillsValue / l_maxValue * 80;
	}

	private void SetupLocalizition()
	{
		UILabel l_title = getView("titleText") as UILabel;
		UILabel l_promote = getView("promoteButton").getView("Text") as UILabel;
		UILabel l_math = getView("mathText") as UILabel;
		UILabel l_reading = getView("readingText") as UILabel;
		UILabel l_science = getView("scienceText") as UILabel;
		UILabel l_social = getView("socialText") as UILabel;
		UILabel l_cognitive = getView("cognitiveText") as UILabel;
		UILabel l_creative = getView("creativeText") as UILabel;
		UILabel l_life = getView("lifeSkillsText") as UILabel;
		
		l_title.text = Localization.getString( Localization.TXT_57_LABEL_PROGRESS );
		l_promote.text = Localization.getString( Localization.TXT_57_LABEL_PROMOTE );
		l_math.text = Localization.getString( Localization.TXT_COURSE_MATH );
		l_reading.text = Localization.getString( Localization.TXT_COURSE_READING );
		l_science.text = Localization.getString( Localization.TXT_COURSE_SCIENCE );
		l_social.text = Localization.getString( Localization.TXT_COURSE_SOCIAL );
		l_cognitive.text = Localization.getString( Localization.TXT_COURSE_COGNITIVE );
		l_creative.text = Localization.getString( Localization.TXT_COURSE_CREATIVE );
		l_life.text = Localization.getString( Localization.TXT_COURSE_LIFE );
	}

	private UISlider m_mathSlider;
	private UISlider m_readingSlider;
	private UISlider m_scienceSlider;
	private UISlider m_socialSlider;
	private UISlider m_cognitiveSlider;
	private UISlider m_creativeSlider;
	private UISlider m_lifeSkillsSlider;

	private int m_mathId;
	private int m_readingId;
	private int m_scienceId;
	private int m_socialId;
	private int m_cognitiveId;
	private int m_creativeId;
	private int m_lifeSkillsId;

	private ArrayList m_data;
}
