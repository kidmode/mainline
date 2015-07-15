using UnityEngine;
using System.Collections;

public class CornerProfileCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );

		_setupProfileInfo( );
		refreshInfo();

//		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );
	}
	
	public override void update()
	{
		base.update();

		m_childAvatar.setTexture( m_kid.kid_photo );
	}
	
	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}
	
	public override void enteringTransition()
	{
		base.enteringTransition();
		tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
	}

//------------------ Private Implementation --------------------
	
	public void refreshInfo()
	{

		Debug.Log ("  refreshInfo  ======= corner profile canvas");
		if (m_kid == null)
			return;
		m_kid.requestPhoto(); //cynthia
		m_childAvatar.setTexture( m_kid.kid_photo );
		
		m_childNameLabel.text 	= m_kid.name;
		m_childNameLabel.fontSize = Mathf.Min((int)(350.0f / m_kid.name.Length), 32);
		m_zpLevelLabel.text		= m_kid.level.ToString("N0");
		m_starNumberLabel.text 	= m_kid.stars.ToString("N0");
		m_gemNumberLabel.text 	= m_kid.gems.ToString("N0");
		m_starMeter.sizeDelta	= new Vector2(m_kid.stars * 150.0f / ServerSettings.getInstance().levels.getLevelPoints(m_kid.level + 1), 37.0f);
	}
	
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}


	private void _setupProfileInfo( )
	{
		GameObject l_obj 	= GameObject.FindWithTag( "GameController" );
		Game l_game 		= l_obj.GetComponent< Game >();
		DebugUtils.Assert( l_game != null );
		
		//added by joshua
		m_childAvatar 			= getView( "avatarIcon" ) 	as UIImage;

		m_childNameLabel 		= getView( "childName" 	) 	as UILabel;
		m_zpLevelLabel			= getView( "zpLevel"	)	as UILabel;
		m_starNumberLabel 		= getView( "starNumber" ) 	as UILabel;
		m_gemNumberLabel 		= getView( "gemNumber" 	) 	as UILabel;

		m_starMeter				= getView( "starMeter" ).gameObject.GetComponent<RectTransform>();

		m_kid 					= SessionHandler.getInstance().currentKid;
	}

	private Kid m_kid;

	private UIImage m_childAvatar;

	private UILabel m_childNameLabel;
	private UILabel m_zpLevelLabel;
	private UILabel m_starNumberLabel;
	private UILabel m_gemNumberLabel;
	private RectTransform m_starMeter;
}
