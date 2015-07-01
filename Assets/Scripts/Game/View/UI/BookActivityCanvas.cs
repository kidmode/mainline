using UnityEngine;
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

	public BookInfo(string p_bookName, BookState p_state, string p_iconUrl, int p_bookId, int p_bookReadingId = 0 )
    {
		bookName = p_bookName;
		bookId = p_bookId;
		bookReadingId = p_bookReadingId;
		bookState = p_state;
		iconUrl = p_iconUrl;

		//honda: check icon existed or not. if not, load icon from server
		string contentName = "book_" + bookId + ".png";
		Texture2D texture = ImageCache.getCacheImage(contentName);
		icon = texture;
		if (icon == null)
		{
			request = new RequestQueue();
			request.add(new ImageRequest("icon", iconUrl, _requestBookIconComplete));
			request.request(RequestType.RUSH);
		}
//		else
//			Debug.Log(contentName + " cached");
    }

	private void _requestBookIconComplete(WWW p_response)
	{
		if (p_response.error == null
		  
		    && false == disposed)
		{
			icon = p_response.texture;

			string name = "book_" + bookId + ".png";
			Debug.Log(name);
			ImageCache.saveCacheImage(name, icon);
		}
		disposeRequest();
	}

	public void dispose()
	{
		if (null != icon)
		{
			GameObject.Destroy(icon);
			icon = null;
		}
		disposed = true;
		disposeRequest();
	}

	public void reload ()
	{
		disposed = false;
		disposeRequest();

		//honda: check icon existed or not. if not, load icon from server
		string contentName = "book_" + bookId + ".png";
		Texture2D texture = ImageCache.getCacheImage(contentName);
		icon = texture;
		if (icon == null)
		{
			request = new RequestQueue();
			request.add(new ImageRequest("icon", iconUrl, _requestBookIconComplete));
			request.request(RequestType.RUSH);
		}
		else
			Debug.Log("reload function " + contentName + " cached");
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

		m_lockIcon = Resources.Load ( "GUI/2048/common/icon/icon_lock" ) as Texture2D;
		m_playIcon = Resources.Load ( "GUI/800/common/button/icon_play" ) as Texture2D;
		m_recordableIcon = Resources.Load ( "GUI/800/common/button/icon_record" ) as Texture2D;
	}
	
	public override void update()
	{
		base.update();
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
			break;
		case BookState.Recorded :
			l_stateLabel.text = "Recorded";
			l_stateIcon.setTexture(m_playIcon);
			break;
		}

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

    private UISwipeList     m_bookSwipeList;
	private UISwipeList     m_bookFavorateSwipeList;
	private UILabel			m_bookFavorateInfo;
	private UILabel			m_bookInfo;

	private List< Button > 	m_buttonList;

	private Texture2D		m_playIcon;
	private Texture2D		m_lockIcon;
	private Texture2D		m_recordableIcon;
	
	private Texture2D 		m_emptyTexture;
}
