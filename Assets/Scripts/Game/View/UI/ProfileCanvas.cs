using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProfileCanvas : UICanvas
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );

		_setupList();
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

	//----------------- Private Implementation -------------------

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UIButton l_button = p_element as UIButton;
		DebugUtils.Assert( l_button != null );
		
		Kid l_kid = p_data as Kid;
		
		UILabel l_label = l_button.getView( "name" ) as UILabel;
		l_label.text = l_kid.name;
		
		if( l_kid.gems != ProfileInfo.ADD_PROFILE_CODE && null != l_kid.kid_photo )
		{
			UIImage l_image = l_button.getView( "avatarIcon" ) as UIImage;
			l_image.setTexture( l_kid.kid_photo );
		}
	}

	private void _setupList()
	{
		//m_addChildTex = Resources.Load("GUI/2048/profile_menu/avatar_blank") as Texture2D;
		//DebugUtils.Assert( m_addChildTex != null );
		UISwipeList l_swipe = null;
		UISwipeList l_multiKidSwipe = getView( "profileSwipeList" ) as UISwipeList;
		UISwipeList l_oneKidSwipe = getView( "oneProfileSwipeList" ) as UISwipeList;
		List<Kid> l_kidList = SessionHandler.getInstance ().kidList;
		int l_kidCount = l_kidList.Count;
		if(1 == l_kidCount)
		{
			l_multiKidSwipe.active = false;
			l_oneKidSwipe.active = true;
			l_swipe = l_oneKidSwipe;
		}
		else
		{
			l_multiKidSwipe.active = true;
			l_oneKidSwipe.active = false;
			l_swipe = l_multiKidSwipe;
		}

		List< System.Object > infoData = new List< System.Object >();

		if (null != l_kidList)
		{
			for(int i = 0; i < l_kidCount; i++)
			{
				infoData.Add( l_kidList[i] );
			}
		}

		l_swipe.setData( infoData );
		l_swipe.setDrawFunction( onListDraw );
		l_swipe.redraw();
	}
	
	private ProfileInfo _createNewProfile()
	{
		ProfileInfo newProfile = new ProfileInfo( "New Person", 0, 1, 0,null );
		return newProfile;
	}
	
	private void _setCurrentProfile( ProfileInfo p_profile )
	{
		
	}

	private Texture2D m_addChildTex;
}
