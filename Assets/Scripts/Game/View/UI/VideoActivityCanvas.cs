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

		setScrollView ();
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
			moveTotal = 0;
			move = 0;
			isFavor = true;
			m_favorcontentPanel.localPosition = new Vector3 (0, 0, 0);
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
			moveTotal = 0;
			move = 0;
			isFavor = false;
			m_contentPanel.localPosition = new Vector3 (0, 0, 0);
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

		//honda comment: requestIcon check icon from local or server
		if( !l_info.iconRequested )
		{
			l_info.requestIcon();
		}

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




	public override void update()
	{
		base.update();

		if (mMovingOffset == 0) {
			if(isFavor)
				move = m_favorcontentPanel.localPosition.x;
			else
				move = m_contentPanel.localPosition.x;
			moveTotal = move;
		}
		if (mMovingOffset > 0) {
			move += mMovingOffset * MOVESPEED;
			if (move <= moveTotal) {
				if(isFavor)
					m_favorcontentPanel.localPosition = new Vector3 (move, 0, 0);
				else
					m_contentPanel.localPosition = new Vector3 (move, 0, 0);
			}
			else {
				if(!m_leftButton.active) {
					mMovingOffset = 0;
					if(isFavor)
						moveTotal = m_favorcontentPanel.localPosition.x;
					else
						moveTotal = m_contentPanel.localPosition.x;
				}
				else {
					moveTotal += (mMovingOffset * OFFSET);
					mMovingOffset = 0;
				}

			}
		} else if (mMovingOffset < 0) {

			move += mMovingOffset * MOVESPEED;
			if (move > moveTotal) {
				if(isFavor)
					m_favorcontentPanel.localPosition = new Vector3 (move, 0, 0);
				else
					m_contentPanel.localPosition = new Vector3 (move, 0, 0);
			}
			else {
				if(!m_rightButton.active) {
					mMovingOffset = 0;
					if(isFavor)
						moveTotal = m_favorcontentPanel.localPosition.x;
					else
						moveTotal = m_contentPanel.localPosition.x;
				}
				else {
					moveTotal += (mMovingOffset * OFFSET);
					mMovingOffset = 0;
				}

			}
		}

	}

	private void SetupLocalizition()
	{		
		UILabel l_allVideosLabel 	= getView("allContentLabel") as UILabel;
		UILabel l_favorateLabel 	= getView("favorateLabel") as UILabel;
		UILabel l_allContentLabelOff   = getView("allContentLabelOff") as UILabel;
		UILabel l_favorateLabelOff 	= getView("favorateLabelOff") as UILabel;
		UILabel l_infoLabel 		= getView("info") as UILabel;
		UILabel l_favorateInfoLabel = getView("favoriteInfo") as UILabel;
		UILabel l_headerLabel 		= getView("header") as UILabel;

		l_allVideosLabel.text 		= Localization.getString(Localization.TXT_TAB_ALL_VIDEOS);
		l_favorateLabel.text 		= Localization.getString(Localization.TXT_TAB_FAVORITES);
		l_allContentLabelOff.text = l_allVideosLabel.text;
		l_favorateLabelOff.text = l_favorateLabel.text;
//		l_infoLabel.text 			= Localization.getString(Localization.TXT_11_LABEL_INFO);
//		l_favorateInfoLabel.text 	= Localization.getString(Localization.TXT_11_LABEL_FAVORITE);
		l_infoLabel.text 			= Localization.getString(Localization.TXT_LABEL_LOADING);
		l_favorateInfoLabel.text 	= Localization.getString(Localization.TXT_LABEL_LOADING);
		l_headerLabel.text 			= Localization.getString(Localization.TXT_11_LABEL_HEADER);
	}

	public void onLeftButtonClick( UIButton p_button )
	{
		mMovingOffset = 1;
		if (move == moveTotal) {
			moveTotal = move + mMovingOffset * OFFSET;
		}
	}

	public void onRightButtonClick( UIButton p_button )
	{
		mMovingOffset = -1;
		if (moveTotal == 0) {
			if(isFavor)
				move = m_favorcontentPanel.localPosition.x;
			else
				move = m_contentPanel.localPosition.x;
			moveTotal = move + mMovingOffset * OFFSET;
		} else if (move == moveTotal) {
			moveTotal = move + mMovingOffset * OFFSET;
		}
	}

	public void onLeftFavorButtonClick( UIButton p_button )
	{
		mMovingOffset = 1;
		if (move == moveTotal) {
			moveTotal = move + mMovingOffset * OFFSET;
		}
	}
	
	public void onRightFavorButtonClick( UIButton p_button )
	{
		mMovingOffset = -1;
		if (moveTotal == 0) {
			if(isFavor)
				move = m_favorcontentPanel.localPosition.x;
			else
				move = m_contentPanel.localPosition.x;
			moveTotal = move + mMovingOffset * OFFSET;
		} else if (move == moveTotal) {
			moveTotal = move + mMovingOffset * OFFSET;
		}
	}

	void setScrollView() {
		
		m_contentPanel = getView ("allContentScrollView").getChildAt(0).gameObject.GetComponent<RectTransform>();
		m_favorcontentPanel = getView ("favorateScrollView").getChildAt(0).gameObject.GetComponent<RectTransform>();
		
		m_leftButton = getView( "allContentArrowLeft" ) as UIButton;
		m_rightButton = getView( "allContentArrowRight" ) as UIButton;
		m_favorleftButton = getView( "favorateScrollArrowLeft" ) as UIButton;
		m_favorrightButton = getView( "favorateScrollArrowRight" ) as UIButton;
		
		m_leftButton.addClickCallback( onLeftButtonClick );
		m_rightButton.addClickCallback( onRightButtonClick );
		
		m_favorleftButton.addClickCallback( onLeftFavorButtonClick );
		m_favorrightButton.addClickCallback( onRightFavorButtonClick );
	}

    private UISwipeList     m_videoSwipeList;
    private UISwipeList     m_videoFavorateSwipeList;
	private UILabel			m_videoFavorateInfo;
	private UILabel 		m_videoInfo;

	private UIButton m_leftButton;
	private UIButton m_rightButton;
	private UIButton m_favorleftButton;
	private UIButton m_favorrightButton;

	private List< Button > 	m_buttonList;
	
	private Texture2D m_emptyTexture;

	private RectTransform m_contentPanel;
	private RectTransform m_favorcontentPanel;
	

	private float OFFSET = 500;
	private float MOVESPEED = 30;
	private float mMovingOffset = 0;
	private float move = 0;
	private float moveTotal;
	private bool isFavor = false; // true: favor false: all
	
}
