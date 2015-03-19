using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KidsProfileCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);

		_setupInfo();
		refreshInfo();

		m_profileImage = getView( "profile" ) as UIElement;
		m_profileImage.tweener.addAlphaTrack( 0.0f, 1.0f, 1.0f );

		m_backButton = getView ( "backButton" ) as UIButton;
		List	<Vector3> l_pointList = new List<Vector3> ();
		l_pointList.Add( m_backButton.transform.localPosition - new Vector3( 150, 0, 0 ) );
		l_pointList.Add( m_backButton.transform.localPosition );
		m_backButton.tweener.addPositionTrack( l_pointList, 1.0f );

		//TODO localization
	}

	public override void update()
	{
		base.update();
	}

	public override void enteringTransition(  )
	{
		base.enteringTransition( );
	}

	public override void exitingTransition( )
	{
		base.exitingTransition( );
	}
	
	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}

	public void refreshInfo()
	{
		Kid l_kid = SessionHandler.getInstance ().currentKid;

		m_childAvatar.setTexture( l_kid.kid_photo );
		m_childNameLabel.text 	= l_kid.name;
	}

	//----------------- Private Implementation -------------------

	private void _setupInfo()
	{
		m_childAvatar = getView( "avatarIcon" ) as UIImage;
		m_childNameLabel = getView ( "childName" ) as UILabel;
	}

	private UIImage m_childAvatar;
	private UILabel m_childNameLabel;

	private UIButton m_backButton;
	private UIElement m_profileImage;
}
