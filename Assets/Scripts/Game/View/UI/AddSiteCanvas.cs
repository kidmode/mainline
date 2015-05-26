using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AddSiteCanvas : UICanvas
{

	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);

		m_btnBgIcon = Resources.Load ( "GUI/800/common/button/bt_dash_red_up" ) as Texture2D;
		m_btnIcon = Resources.Load ( "GUI/2048/common/icon/icon_delete" ) as Texture2D;
		
		m_normalBtnBgIcon = Resources.Load ( "GUI/800/common/button/bt_dash_green_up" ) as Texture2D;
		m_normalBtnIcon = Resources.Load ( "GUI/2048/common/icon/icon_add" ) as Texture2D;

		m_searchData = new List<object> ();
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_title = getView("titleImage").getView ("titleText") as UILabel;
		l_title.text =  Localization.getString (Localization.TXT_76_LABEL_TITLE);
		UILabel l_searchPanel = getView("searchPanel").getView ("Text") as UILabel;
		l_searchPanel.text =  Localization.getString (Localization.TXT_76_LABEL_PANEL_TITLE);
		UILabel l_searchButton = getView("searchButton").getView ("Text") as UILabel;
		l_searchButton.text =  Localization.getString (Localization.TXT_76_BUTTON_SEARCH);
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

	public void setData( List<object> p_dataList )
	{
		m_dataList = p_dataList;

		UISwipeList l_swipe = getView ( "siteSwipeList" ) as UISwipeList;
		l_swipe.setData ( m_dataList );
		
		l_swipe.setDrawFunction ( onListDraw );
		l_swipe.redraw ();
	}

	public void onButtonClicked(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		SiteInfo l_siteInfo = p_data as SiteInfo;
		DebugUtils.Assert ( l_siteInfo != null );
		
		l_siteInfo.isBlocked = !l_siteInfo.isBlocked;

		if( m_searchData.Count > 0 )
		{
			for( int i = 0; i < m_searchData.Count; i++ )
			{
				if( l_siteInfo.id == (m_searchData[i] as SiteInfo).id )
				{
					m_searchData[i] = l_siteInfo;
				}
			}
			p_list.setData (m_searchData);
		}
		else
		{
			for( int i = 0; i < m_dataList.Count; i++ )
			{
				if( l_siteInfo.id == (m_dataList[i] as SiteInfo).id )
				{
					m_dataList[i] = l_siteInfo;
				}
			}
			p_list.setData (m_dataList);
		}
	}

	public void setSearchData( List<object> p_dataList )
	{
		m_searchData = p_dataList;
		UISwipeList l_swipe = getView ( "siteSwipeList" ) as UISwipeList;
		l_swipe.setData ( m_searchData );
	}

	//----------------- Private Implementation -------------------
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UIElement l_prototype = p_element;
		DebugUtils.Assert( l_prototype != null );
		
		UILabel l_siteName 		= l_prototype.getView ( "siteText" ) as UILabel;	
		UILabel l_message 		= l_prototype.getView ( "messageText" ) as UILabel;
		UIImage l_buttonBgIcon 	= l_prototype.getView ( "btnBgImage" ) 	as UIImage;
		UIImage l_buttonIcon 	= l_prototype.getView ( "btnImage" ) 	as UIImage;
		
		SiteInfo l_site = p_data as SiteInfo;
		
		l_siteName.text = l_site.name;
		
		if( l_site.isBlocked )
		{
			l_message.text = "Added";
			l_buttonBgIcon.setTexture( m_btnBgIcon );
			l_buttonIcon.setTexture( m_btnIcon );
		}
		else
		{
			l_message.text = "";
			l_buttonBgIcon.setTexture( m_normalBtnBgIcon );
			l_buttonIcon.setTexture( m_normalBtnIcon );
		}
	}

	private Texture2D m_btnBgIcon;
	private Texture2D m_btnIcon;
	
	private Texture2D m_normalBtnBgIcon;
	private Texture2D m_normalBtnIcon;

	private List<object> m_dataList;
	private List<object> m_searchData;
}

public class SiteInfo
{
	private int m_id;
	private string m_name;
	private bool m_isBlocked;

	public int id
	{
		get
		{
			return m_id;
		}
		set
		{
			m_id = value;
		}
	}
	public string name
	{
		get
		{
			return m_name;
		}
		set
		{
			m_name = value;
		}
	}
	public bool isBlocked
	{
		get
		{
			return m_isBlocked;
		}
		set
		{
			m_isBlocked = value;
		}
	}
}