﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class BookInfo : object
{
    public Texture2D icon;
    public int bookId;
	public int bookReadingId;
	public BookState bookState; 
	public string bookName;
	public bool disposed;
	public RequestQueue request;
	public string iconUrl;
	public bool iconRequested;

	public BookInfo(string p_bookName, BookState p_state, string p_iconUrl, int p_bookId, int p_bookReadingId = 0 )
    {
		bookName = p_bookName;
		bookId = p_bookId;
		bookReadingId = p_bookReadingId;
		bookState = p_state;
		iconUrl = p_iconUrl;
		iconRequested = false;
    }

	//honda: added
	public void requestIcon()
	{
		if (loadImage())
			return;
	}
	//end

	public void dispose()
	{
		if (null != icon)
		{
//			GameObject.Destroy(icon);
			Resources.UnloadAsset(icon);
			icon = null;
		}
		disposed = true;
		disposeRequest();
	}

	public void reload ()
	{	
		//honda: added
		if (loadImage())
			return;

		disposed = false;
		disposeRequest();
	}

	private bool loadImage()
	{
		//honda: Now, books are all in the local place. So, don't need to request cover image from server
		//honda: cover images are loaded directly locally
		string contentName = "book_" + bookId;
		string imagePath = "Books/Images/" + contentName;
		Texture2D texture = Resources.Load(imagePath) as Texture2D;
		icon = texture;
		iconRequested = true;
		return true;
		//honda: comment out the code due to above reason
		//honda: check icon existed or not. if not, load icon from server
//		string contentName = "book_" + bookId + ".jpg";
//		Texture2D texture = ImageCache.getCacheImage(contentName);
//		icon = texture;
//		if (icon == null)
//		{
//			request = new RequestQueue();
//			request.add(new ImageRequest("icon", iconUrl, _requestBookIconComplete));
//			request.request(RequestType.RUSH);
//			iconRequested = true;
//		}
	}

	private void _requestBookIconComplete(WWW p_response)
	{
		if (p_response.error == null
		    
		    && false == disposed)
		{
			icon = p_response.texture;
			
			string name = "book_" + bookId + ".jpg";
			Debug.Log(name);
			ImageCache.saveCacheImage(name, icon);
		}
		disposeRequest();
	}

	private void disposeRequest()
	{
		if (null != request)
		{
			request.reset();
		}
	}
}

public enum BookState
{
	Free,
	Recorded,
	NotRecorded,
	NeedToBuy
}

public class BookActivityCanvas : UICanvas
{

	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
//		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );

        UIToggle l_favorateTab      = getView("favorateTab") as UIToggle;
        l_favorateTab.addValueChangedCallback( onFeaturedToggled );

        UIToggle l_allContentTab    = getView("allContentTab") as UIToggle;
        l_allContentTab.addValueChangedCallback( onAllToggled );
        
        m_bookSwipeList             = getView( "allContentScrollView" ) as UISwipeList;

        m_bookFavorateSwipeList     = getView( "favorateScrollView" )   as UISwipeList;
        m_bookFavorateSwipeList.active = false;

		m_bookFavorateInfo = getView("favoriteInfo") as UILabel;
		m_bookInfo = getView("info") as UILabel;
		
		m_emptyTexture = new Texture2D (1, 1);

		_setupList();
		
		SetupLocalizition ();

		setScrollView ();

		m_lockIcon = Resources.Load ( "GUI/2048/common/icon/icon_lock" ) as Texture2D;
		m_playIcon = Resources.Load ( "GUI/800/common/button/icon_play" ) as Texture2D;
		m_recordableIcon = Resources.Load ( "GUI/800/common/button/icon_record" ) as Texture2D;
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
	
	public override void enteringTransition(  )
	{
		base.enteringTransition( );

		tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
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
            m_bookSwipeList.active          = false;
			m_bookFavorateSwipeList.active 	= true;
			m_bookFavorateInfo.active = (m_bookFavorateSwipeList.getData().Count <= 0);
			m_bookInfo.active = false;
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
            m_bookSwipeList.active          = true;
            m_bookFavorateSwipeList.active  = false;
			m_bookInfo.active = (m_bookSwipeList.getData().Count <= 0);
			m_bookFavorateInfo.active = false;
        }
    }

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UIButton l_button = p_element as UIButton;
		DebugUtils.Assert( l_button != null );

        BookInfo l_info = p_data as BookInfo;
		DebugUtils.Assert( l_info != null );

        //*Temporary*
        UILabel l_nameLabel = p_element.getView("name") as UILabel;
		DebugUtils.Assert(l_nameLabel != null);

		l_nameLabel.text = l_info.bookName;

		UILabel l_stateLabel = p_element.getView("state") as UILabel;
		DebugUtils.Assert(l_stateLabel != null);

		UIImage l_stateIcon = p_element.getView ("stateIcon") as UIImage;
		DebugUtils.Assert(l_stateIcon != null);

		switch( l_info.bookState )
		{
//		case BookState.Free :
//			l_stateLabel.text = "Free";
//			l_button.enabled = false;
//			break;
		case BookState.NeedToBuy :
			l_stateLabel.text = "Need To Buy";
			l_stateIcon.setTexture(m_lockIcon);
			break;
		case BookState.NotRecorded :
			l_stateLabel.text = "Not Recorded";
			l_stateIcon.setTexture(m_recordableIcon);
			//honda: play voice recording, currently hide recording icon
			l_stateLabel.active = false;
			l_stateIcon.active = false;
			break;
		case BookState.Recorded :
			l_stateLabel.text = "Recorded";
			l_stateIcon.setTexture(m_playIcon);
			//honda: play voice recording, currently hide recording icon
			l_stateLabel.active = false;
			l_stateIcon.active = false;
			break;
		}

		UIImage l_rawImage = p_element.getView("icon") as UIImage;
		UIImage shadowImage = p_element.getView("shadow") as UIImage;
        if (l_rawImage == null)
            return;

		//honda: added
		if( !l_info.iconRequested )
		{
			l_info.requestIcon();
		}
		//end

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
		//l_swipe.addClickListener( "Prototype", onBookClicked );

		m_bookSwipeList.setDrawFunction( onListDraw );
        m_bookSwipeList.redraw();

        m_bookFavorateSwipeList.setDrawFunction(onListDraw);
        m_bookFavorateSwipeList.redraw();
	}

	private void SetupLocalizition()
	{		
		UILabel l_allContentLabel   = getView("allContentLabel") as UILabel;
		UILabel l_favorateLabel 	= getView("favorateLabel") as UILabel;
		UILabel l_infoLabel 		= getView("info") as UILabel;
		UILabel l_favorateInfoLabel = getView("favoriteInfo") as UILabel;
		UILabel l_headerLabel 		= getView("header") as UILabel;
		
		l_favorateLabel.text 		= Localization.getString(Localization.TXT_TAB_FAVORITES);
		l_allContentLabel.text    	= Localization.getString(Localization.TXT_TAB_ALL_BOOKS);
//		l_infoLabel.text 			= Localization.getString(Localization.TXT_14_LABEL_INFO);
//		l_favorateInfoLabel.text 	= Localization.getString(Localization.TXT_14_LABEL_FAVORITE);
		l_infoLabel.text 			= Localization.getString(Localization.TXT_LABEL_LOADING);
		l_favorateInfoLabel.text 	= Localization.getString(Localization.TXT_LABEL_LOADING);
		l_headerLabel.text 			= Localization.getString(Localization.TXT_14_LABEL_HEADER);
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

    private UISwipeList     m_bookSwipeList;
	private UISwipeList     m_bookFavorateSwipeList;
	private UILabel			m_bookFavorateInfo;
	private UILabel			m_bookInfo;

	private List< Button > 	m_buttonList;

	private Texture2D		m_playIcon;
	private Texture2D		m_lockIcon;
	private Texture2D		m_recordableIcon;
	
	private Texture2D 		m_emptyTexture;

	private RectTransform m_contentPanel;
	private RectTransform m_favorcontentPanel;
	
	private UIButton m_leftButton;
	private UIButton m_rightButton;
	private UIButton m_favorleftButton;
	private UIButton m_favorrightButton;
	
	private float OFFSET = 500;
	private float MOVESPEED = 25;
	private float mMovingOffset = 0;
	private float move = 0;
	private float moveTotal;
	private bool isFavor = false; // true: favor false: all

}
