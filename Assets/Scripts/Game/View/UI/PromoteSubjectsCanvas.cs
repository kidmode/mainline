using UnityEngine;
using System.Collections;

public class PromoteSubjectsCanvas : UICanvas
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		SetupLocalizition ();

		if( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() )
		{
			_setupProfileInfo( );
			refreshInfo();
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
	
	public void refreshInfo()
	{
		if (m_kid == null)
			return;
		
		m_mathSlider.value 			= m_kid.weightMath;
		m_readingSlider.value 		= m_kid.weightReading;
		m_scienceSlider.value 		= m_kid.weightScience;
		m_socialSlider.value 		= m_kid.weightSocialStudies;
		m_cognitiveSlider.value 	= m_kid.weightCognitiveDevelopment;
		m_creativeSlider.value 		= m_kid.weightCreativeDevelopment;
		m_lifeSkillsSlider.value 	= m_kid.weightLifeSkills;
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

		if ( SessionHandler.getInstance ().kidList.Count > 0 && SessionHandler.getInstance ().currentKid == null )
		{
			SessionHandler.getInstance ().currentKid = SessionHandler.getInstance ().kidList [0];
		}
		m_kid = SessionHandler.getInstance().currentKid;
	}

	private void SetupLocalizition()
	{
		UILabel l_top = getView("titleText") as UILabel;
		
		l_top.text = Localization.getString( Localization.TXT_61_LABEL_TITLE );
		
		UILabel l_math = getView("mathText") as UILabel;
		UILabel l_reading = getView("readingText") as UILabel;
		UILabel l_science = getView("scienceText") as UILabel;
		UILabel l_social = getView("socialText") as UILabel;
		UILabel l_cognitive = getView("cognitiveText") as UILabel;
		UILabel l_creative = getView("creativeText") as UILabel;
		UILabel l_life = getView("lifeSkillsText") as UILabel;

		UILabel l_baseline = getView("TextBaseLine") as UILabel;
		
		l_math.text = Localization.getString( Localization.TXT_COURSE_MATH );
		l_reading.text = Localization.getString( Localization.TXT_COURSE_READING );
		l_science.text = Localization.getString( Localization.TXT_COURSE_SCIENCE );
		l_social.text = Localization.getString( Localization.TXT_COURSE_SOCIAL );
		l_cognitive.text = Localization.getString( Localization.TXT_COURSE_COGNITIVE );
		l_creative.text = Localization.getString( Localization.TXT_COURSE_CREATIVE );
		l_life.text = Localization.getString( Localization.TXT_COURSE_LIFE );

//		l_baseline.text = Localization.getString( Localization.TXT_BASELINE );

	}

	UISlider m_mathSlider;
	UISlider m_readingSlider;
	UISlider m_scienceSlider;
	UISlider m_socialSlider;
	UISlider m_cognitiveSlider;
	UISlider m_creativeSlider;
	UISlider m_lifeSkillsSlider;
	
	Kid m_kid;
}
