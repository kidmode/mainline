using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class ActivityInfo : object
{
    public ActivityInfo(Drawing p_drawing)
    {
		m_drawing = p_drawing;
		if (m_drawing != null)
		{
			RequestQueue l_queue = new RequestQueue();
			l_queue.add(new ImageRequest("icon", m_drawing.mediumUrl, _requestIconComplete));
			l_queue.request(RequestType.RUSH);
		}
	}

	public bool isNew()
	{
		return (m_drawing == null);
	}

	public Drawing drawing { get { return m_drawing; } }
	public Texture2D icon { get { return m_icon; } }

	private void _requestIconComplete(WWW p_response)
	{
		if (p_response.error == null)
			m_icon = p_response.texture;
	}

	public void dispose()
	{
		if (null != m_icon)
		{
			GameObject.Destroy(m_icon);
		}
	}

	private Drawing m_drawing;
	private Texture2D m_icon;
}


public class FunActivityCanvas : UICanvas
{

	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );


        UILabel l_allContentLabel   = getView("allContentLabel") as UILabel;
        l_allContentLabel.text      = Localization.getString( Localization.TXT_TAB_ALL_ACTIVITY );

        UILabel l_booksLabel        = getView("tabLabel") as UILabel;
        l_booksLabel.text           = Localization.getString(Localization.TXT_LABEL_ACTIVITIES);

        UILabel l_favorateLabel     = getView("favorateLabel") as UILabel;
        l_favorateLabel.text        = Localization.getString(Localization.TXT_TAB_FAVORITES);

        m_funActivitySwipeList      = getView( "allContentScrollView" ) as UISwipeList;

		_setupList();
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


	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UIButton l_button = p_element as UIButton;
		DebugUtils.Assert( l_button != null );

        ActivityInfo l_info = p_data as ActivityInfo;
		DebugUtils.Assert( l_info != null );

        //*Temporary*
        UILabel l_titleLabel = p_element.getView("title") as UILabel;
        DebugUtils.Assert(l_titleLabel != null);

		if (!l_info.isNew())
		{
			l_titleLabel.active = false;

			UIRawImage l_rawImage = p_element.getView("icon") as UIRawImage;
			l_rawImage.texture = l_info.icon;
		}

//        l_titleLabel.text = l_info.title;

		//if( l_info.icon == null )
		//	return;
        //
        //UIRawImage l_rawImage = p_element.getView("icon") as UIRawImage;
        //if (l_rawImage == null)
        //    return;
        //
        //l_rawImage.texture = l_info.icon;
	}
	
	private void _setupList()
	{
		//l_swipe.addClickListener( "Prototype", onBookClicked );

		m_funActivitySwipeList.setDrawFunction( onListDraw );
        m_funActivitySwipeList.redraw();

        //m_bookFavorateSwipeList.setDrawFunction(onListDraw);
        //m_bookFavorateSwipeList.redraw();
	}



    private UISwipeList     m_funActivitySwipeList;

	private List< Button > 	m_buttonList;

}
