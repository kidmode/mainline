using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class VideoActivityCanvas : UICanvas
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
//		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );
		
		UIToggle l_favorateTab = getView("favorateTab") as UIToggle;
		l_favorateTab.addValueChangedCallback(onFeaturedToggled);
		
		UIToggle l_allContentTab = getView("allContentTab") as UIToggle;
		l_allContentTab.addValueChangedCallback(onAllToggled);

        m_videoSwipeList = getView("allContentScrollView") as UISwipeList;

        m_videoFavorateSwipeList = getView("favorateScrollView") as UISwipeList;
        m_videoFavorateSwipeList.active = false;

		m_videoInfo = getView("info") as UILabel;
		m_videoFavorateInfo = getView("favoriteInfo") as UILabel;
		
		m_emptyTexture = new Texture2D (1, 1);

		_setupList();

		SetupLocalizition ();
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
			l_rawImage.setTexture( m_emptyTexture );
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

	private void SetupLocalizition()
	{		
		UILabel l_allVideosLabel 	= getView("allContentLabel") as UILabel;
		UILabel l_favorateLabel 	= getView("favorateLabel") as UILabel;
		UILabel l_infoLabel 		= getView("info") as UILabel;
		UILabel l_favorateInfoLabel = getView("favoriteInfo") as UILabel;
		UILabel l_headerLabel 		= getView("header") as UILabel;

		l_favorateLabel.text 		= Localization.getString(Localization.TXT_TAB_FAVORITES);
		l_allVideosLabel.text 		= Localization.getString(Localization.TXT_TAB_ALL_VIDEOS);
//		l_infoLabel.text 			= Localization.getString(Localization.TXT_11_LABEL_INFO);
//		l_favorateInfoLabel.text 	= Localization.getString(Localization.TXT_11_LABEL_FAVORITE);
		l_infoLabel.text 			= Localization.getString(Localization.TXT_LABEL_LOADING);
		l_favorateInfoLabel.text 	= Localization.getString(Localization.TXT_LABEL_LOADING);
		l_headerLabel.text 			= Localization.getString(Localization.TXT_11_LABEL_HEADER);
	}

    private UISwipeList     m_videoSwipeList;
    private UISwipeList     m_videoFavorateSwipeList;
	private UILabel			m_videoFavorateInfo;
	private UILabel 		m_videoInfo;

	private List< Button > 	m_buttonList;
	
	private Texture2D m_emptyTexture;
}
