using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class VideoActivityCanvas : UICanvas
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );

        UILabel l_allVideosLabel = getView("allContentLabel") as UILabel;
        l_allVideosLabel.text = Localization.getString(Localization.TXT_TAB_ALL_VIDEOS);

        //UILabel l_featuredVideoLabel = getView("featuredLabel") as UILabel;
        //l_featuredVideoLabel.text = Localization.getString(Localization.TXT_LABEL_FEATURED_VIDEOS);

        //UILabel l_videoLabel = getView("contentLabel") as UILabel;
        //l_videoLabel.text = Localization.getString(Localization.TXT_LABEL_VIDEOS);

        UILabel l_favorateLabel = getView("favorateLabel") as UILabel;
        l_favorateLabel.text = Localization.getString(Localization.TXT_TAB_FAVORITES);


        UIToggle l_favorateTab = getView("favorateTab") as UIToggle;
        l_favorateTab.addValueChangedCallback(onFeaturedToggled);

        UIToggle l_allContentTab = getView("allContentTab") as UIToggle;
        l_allContentTab.addValueChangedCallback(onAllToggled);

        m_videoSwipeList = getView("allContentScrollView") as UISwipeList;

        m_videoFavorateSwipeList = getView("favorateScrollView") as UISwipeList;
        m_videoFavorateSwipeList.active = false;

		m_videoInfo = getView("info") as UILabel;
		m_videoFavorateInfo = getView("favoriteInfo") as UILabel;

		_setupList();
	}
	
	public override void enteringTransition()
	{
		base.enteringTransition();
		
		tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
	}
	
	public override void exitingTransition()
	{
		base.exitingTransition();
	}

	//----------------- Private Implementation -------------------
	
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

    private void onFeaturedToggled(UIToggle p_toggle, bool p_isOn)
    {
        if( p_isOn )
        {
            m_videoSwipeList.active = false;
	        m_videoFavorateSwipeList.active = true;
			m_videoFavorateInfo.active = (m_videoFavorateSwipeList.getData().Count <= 0);
			m_videoInfo.active = false;
        }
    }

    private void onAllToggled(UIToggle p_toggle, bool p_isOn)
    {
        if( p_isOn )
        {
            m_videoSwipeList.active = true;
			m_videoFavorateSwipeList.active = false;
			m_videoInfo.active = (m_videoSwipeList.getData().Count <= 0);
			m_videoFavorateInfo.active = false;
        }
    }

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UIButton l_button = p_element as UIButton;
		DebugUtils.Assert( l_button != null );
		
		WebViewInfo l_info = p_data as WebViewInfo;
		DebugUtils.Assert( l_info != null );

        //*Temporary*
        UILabel l_numberLabel = p_element.getView("number") as UILabel;
//        DebugUtils.Assert(l_numberLabel != null);

        l_numberLabel.text = string.Empty;

		UIImage l_rawImage = p_element.getView("icon") as UIImage;
        if (l_rawImage == null)
            return;

		if( l_info.icon == null )
		{
			l_rawImage.setTexture( new Texture2D(1, 1) );
		}
		else
		{
			l_rawImage.setTexture(l_info.icon);
		}
	}
	
	private void _setupList()
	{
		//List< System.Object > l_infoData = new List< System.Object >();
        //l_infoData.Add( new WebViewInfo( null, "https://www.youtube.com/embed/PzFXfvZuLK0?rel=0" ) );
		//l_infoData.Add( new WebViewInfo( null, "https://www.youtube.com/embed/AsAzI4Jdsj0?rel=0" ) );
		//l_infoData.Add( new WebViewInfo( null, "https://www.youtube.com/embed/PJhXVg2QisM?rel=0" ) );
        //l_infoData.Add( new WebViewInfo( null, "https://www.youtube.com/embed/yCjJyiqpAuU?rel=0" ) );
		//l_infoData.Add( new WebViewInfo( null, "https://www.youtube.com/embed/tbbKjDjMDok?rel=0" ) );
		//l_infoData.Add( new WebViewInfo( null, "https://www.youtube.com/embed/OKbpLQp509Y?rel=0" ) );
		//l_infoData.Add( new WebViewInfo( null ) );
		
		//l_swipe.setData( l_infoData );
        m_videoSwipeList.setDrawFunction(onListDraw);
        m_videoSwipeList.redraw();

        m_videoFavorateSwipeList.setDrawFunction(onListDraw);
        m_videoFavorateSwipeList.redraw();
	}

    private UISwipeList     m_videoSwipeList;
    private UISwipeList     m_videoFavorateSwipeList;
	private UILabel			m_videoFavorateInfo;
	private UILabel 		m_videoInfo;

	private List< Button > 	m_buttonList;
}
