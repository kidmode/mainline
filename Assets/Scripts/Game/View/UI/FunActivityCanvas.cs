using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading;


public class ActivityInfo : object
{
    public ActivityInfo(Drawing p_drawing)
    {
		m_drawing = p_drawing;
		//honda: move drawing request on onlistdraw()
//		if (m_drawing != null)
//		{
//			RequestQueue l_queue = new RequestQueue();
//			l_queue.add(new ImageRequest("icon", m_drawing.mediumUrl, _requestIconComplete));
//			l_queue.request(RequestType.RUSH);
//		}
	}

	public bool isNew()
	{
		return (m_drawing == null);
	}

	public Drawing drawing { get { return m_drawing; } }
	public Texture2D icon { get { return m_icon; } }
	//honda
	private bool isRequested = false;
	private int tryingTimes = 0;
	private int maxTryingTimes = 3;
	private RequestQueue rQueue;
	//end
	public void requestIcon()
	{
		if( m_icon == null && m_drawing != null && isRequested == false)
		{
			if (rQueue == null)
			{
				rQueue = new RequestQueue();
			}
			rQueue.add(new ImageRequest("icon", m_drawing.mediumUrl, _requestIconComplete));
			rQueue.request(RequestType.RUSH);
			isRequested = true;
		}
	}

	private void _requestIconComplete(WWW p_response)
	{
		if (p_response.error == null)
		{
			m_icon = p_response.texture;
		}
		else
		{
			tryingTimes++;
			if (tryingTimes <= maxTryingTimes)
			{
				isRequested = false;
			}
			else
			{
				Debug.Log("Drawing icon requests fail 3 times\nDrawing icon request error: " + p_response.error);
			}
		}
	}

	public void dispose()
	{
		if (null != m_icon)
		{
			GameObject.Destroy(m_icon);
		}

		if (rQueue != null)
		{
			rQueue.dispose();
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

		SetupLocalizition ();

//		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );

        m_funActivitySwipeList = getView( "allContentScrollView" ) as UISwipeList;

		//honda
		_setupDrawingList(SessionHandler.getInstance().drawingList);
		//add listener for drawing list updates
		SessionHandler.getInstance().onUpdateDrawingList += updateDrawingList;
		//end

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

		//honda
		_disposeActivityInfos(m_funViewList);
		m_funViewList.Clear();
		SessionHandler.getInstance().onUpdateDrawingList -= updateDrawingList;
		//end
	}
	
	//----------------- Private Implementation -------------------
	private void updateDrawingList()
	{
		_setupDrawingList(SessionHandler.getInstance().drawingList);
	}

	private void _setupDrawingList(List<Drawing> p_contentList)
	{
		m_funViewList.Clear();
		m_funViewList.Add(new ActivityInfo(null));
		
		if (p_contentList != null)
		{
			foreach (Drawing drawing in p_contentList)
			{
				ActivityInfo l_info = new ActivityInfo(drawing);
				m_funViewList.Add(l_info);
			}
		}
		m_funActivitySwipeList.setData(m_funViewList);
	}

	private void _disposeActivityInfos(List<object> p_list)
	{
		int l_numInfos = p_list.Count;
		for (int i = 0; i < l_numInfos; ++i)
		{
			ActivityInfo l_info = p_list[i] as ActivityInfo;
			l_info.dispose();
		}
	}
	//end

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

			if( null != l_info.icon )
			{
				UIImage l_rawImage = p_element.getView("icon") as UIImage;
				l_rawImage.setTexture( l_info.icon );
			}
			else
			{
				l_info.requestIcon();
			}
		}
	}
	
	private void _setupList()
	{
		m_funActivitySwipeList.setDrawFunction( onListDraw );
        m_funActivitySwipeList.redraw();
	}

	private void SetupLocalizition()
	{
		UILabel l_allContentLabel   = getView("allContentLabel") as UILabel;
		UILabel l_new = getView ("title").getView("title") as UILabel;

		l_allContentLabel.text      = Localization.getString( Localization.TXT_TAB_ALL_ACTIVITY );
		l_new.text        			= Localization.getString(Localization.TXT_33_LABEL_NEW);
	}


    private UISwipeList     m_funActivitySwipeList;
	private List<Button> 	m_buttonList;
	//honda
	private List<object> 	m_funViewList = new List<object>();
	//end
}
