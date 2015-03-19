using UnityEngine;
using System.Collections;

public class PromoteSubjectsCanvas : UICanvas
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );

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
	
	UISlider m_mathSlider;
	UISlider m_readingSlider;
	UISlider m_scienceSlider;
	UISlider m_socialSlider;
	UISlider m_cognitiveSlider;
	UISlider m_creativeSlider;
	UISlider m_lifeSkillsSlider;
	
	Kid m_kid;
}
