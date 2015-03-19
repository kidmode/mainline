using UnityEngine;
using System.Collections;
using System;

public class DashBoardProfileInfoCanvas : UICanvas
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		_setupProfileInfo( );
		refreshInfo();
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
		Kid l_kid = SessionHandler.getInstance ().currentKid;

		m_childAvatar.setTexture( l_kid.kid_photo );
		m_childNameLabel.text 	= l_kid.name;
		m_starNumberLabel.text 	= l_kid.stars.ToString("N0");
		m_gemNumberLabel.text 	= l_kid.gems.ToString("N0");
		m_levelLabel.text 		= l_kid.level.ToString ();

		m_ageLabel.text = SessionHandler.getInstance().currentKid.age.ToString();
	}

	//------------------ Private Implementation --------------------
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

		if (SessionHandler.getInstance ().kidList.Count > 0 && null == SessionHandler.getInstance ().currentKid)
		{
			SessionHandler.getInstance ().currentKid = SessionHandler.getInstance ().kidList [0];
		}

		m_childAvatar 			= getView( "avatarIcon" ) 	as UIImage;
		m_childNameLabel 		= getView( "childName" 	) 	as UILabel;
		m_starNumberLabel 		= getView( "zoodlePointsText" ) 	as UILabel;
		m_gemNumberLabel 		= getView( "gemsText" 	) 	as UILabel;
		m_ageLabel 				= getView( "ageText" ) 		as UILabel;
		m_levelLabel 			= getView( "levelText" ) 	as UILabel;
	}
	
	private UIImage m_childAvatar;
	
	private UILabel m_childNameLabel;
	private UILabel m_starNumberLabel;
	private UILabel m_gemNumberLabel;
	private UILabel m_ageLabel;
	private UILabel m_levelLabel;
}
