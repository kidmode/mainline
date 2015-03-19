using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ProfileActivityCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);

		m_panel = getView( "panel" );

		List<Vector3> l_pointList = new List<Vector3>();
		l_pointList.Add( m_panel.transform.localPosition - new Vector3( 0, 800, 0) );
		l_pointList.Add( m_panel.transform.localPosition );
		
		m_panel.tweener.addPositionTrack( l_pointList, 0.3f );

		_setupList();
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
	}

	public override void exitingTransition ()
	{
		base.exitingTransition ();
	}

	//------------------ Private Implementation ----------------------

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		ProfileInfoData l_profile = p_data as ProfileInfoData;
		Texture2D l_iconImage = Resources.Load(l_profile.iconPath) as Texture2D;
		DebugUtils.Assert( l_iconImage != null );
		
		UIImage l_image = p_element.getView( "iconImage" ) as UIImage;
		l_image.setTexture( l_iconImage );

		UILabel l_titleText = p_element.getView( "titleText" ) as UILabel;
		l_titleText.text = l_profile.titleString;

		UILabel l_levelLabel = l_image.getView ("levelText") as UILabel;
		if( l_titleText.text.Equals("Zoodles Points") )
		{
			l_levelLabel.active = true;
			l_levelLabel.text = SessionHandler.getInstance().currentKid.level.ToString();
		}
		else
		{
			l_levelLabel.active = false;
		}

		UILabel l_contentText = p_element.getView( "contentText" ) as UILabel;
		l_contentText.text = l_profile.contentString;
	}

	public void _setupList()
	{
		string l_imagePath = "GUI/2048/common/icon/";

		UISwipeList l_swipeList = getView( "profileSwipeList" ) as UISwipeList;

		List<System.Object> l_infoData = new List<object>();

		Kid l_kid = SessionHandler.getInstance ().currentKid;

		l_infoData.Add( new ProfileInfoData( l_imagePath + "icon_star_rating", "Zoodles Points", l_kid.stars.ToString("N0") ) );
		l_infoData.Add( new ProfileInfoData( l_imagePath + "icon_gems", "Gems", l_kid.gems.ToString("N0") ) );
		l_infoData.Add( new ProfileInfoData( l_imagePath + "icon_videos", "Videos Watched", l_kid.videoWatchedCount.ToString("N0") ) );
		l_infoData.Add( new ProfileInfoData( l_imagePath + "icon_games", "Games Played", l_kid.gamePlayedCount.ToString("N0") ) );

		l_swipeList.setData( l_infoData );
		l_swipeList.setDrawFunction( onListDraw );
		l_swipeList.redraw();
	}

	private UIElement m_panel;
}

public class ProfileInfoData : System.Object
{
	public string iconPath;
	public string titleString;
	public string contentString;

	public ProfileInfoData(string p_iconPath, string p_titleString, string p_contentString)
	{
		iconPath = p_iconPath;
		titleString = p_titleString;
		contentString = p_contentString;
	}
}
