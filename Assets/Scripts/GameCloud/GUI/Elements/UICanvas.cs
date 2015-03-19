using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public delegate void EnterTransitionEventHandler( UICanvas p_canvas );
public delegate void ExitTransitionEventHandler( UICanvas p_canvas );


public class UICanvas : UIElement
{

	public event EnterTransitionEventHandler enterTransitionEvent;
	public event ExitTransitionEventHandler exitTransitionEvent;

	public bool hasTransition 
	{
		get { return m_hasTransition; 	}
		set { m_hasTransition = value; 	}
	}

	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		m_canvas = p_gameObject.GetComponent< Canvas >();
		baseElement = m_canvas;
		worldCamera = Camera.main;

		DebugUtils.Assert( m_canvas != null );
	}
	
	public override void update()
	{
		base.update();
	}
	
	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}

	//public override float alpha 
	//{
	//	get { return canvasGroup.alpha; 	}
	//	set {  canvasGroup.alpha = value;	}
	//}

	public void setInputEnabled(bool p_isEnabled)
	{
        if( graphicRaycaster != null )
		    graphicRaycaster.enabled = p_isEnabled;

        if( canvasGroup != null )
		    canvasGroup.interactable = p_isEnabled;
	}

	public bool isTransitioning 
	{
		get { return m_isTransitioning; 	}
		set { m_isTransitioning = value; 	}
	}

	public virtual void enteringTransition( )
	{
		m_isTransitioning = true;
		if( enterTransitionEvent != null )
			enterTransitionEvent( this );
	}

	public virtual void exitingTransition( )
	{
		m_isTransitioning = false;
		if( exitTransitionEvent != null )
			exitTransitionEvent( this );
	}


	public int sortingOrder
	{
		get { return m_canvas.sortingOrder;	 }
		set { m_canvas.sortingOrder = value; }
	}

	public Camera worldCamera
	{
		get { return m_canvas.worldCamera; 	}
		set { m_canvas.worldCamera = value; }
	}

	public GraphicRaycaster graphicRaycaster
	{
		get 
		{ 
			if( m_graphicRaycaster != null )
				return m_graphicRaycaster;

            if (gameObject == null)
                return null;

			m_graphicRaycaster = gameObject.GetComponent< GraphicRaycaster >();
			return m_graphicRaycaster;
		}
	}

	public CanvasGroup canvasGroup
	{
		get 
		{
			if( m_canvasGroup != null )
				return m_canvasGroup;

            if (gameObject == null)
                return null;

			m_canvasGroup = gameObject.GetComponent< CanvasGroup >();
			if( m_canvasGroup != null )
				return m_canvasGroup;

			m_canvasGroup = gameObject.AddComponent< CanvasGroup >();
			return m_canvasGroup;
		}
	}

	public RenderMode renderMode
	{
		get { return m_canvas.renderMode; 	}
		set { m_canvas.renderMode = value; 	}
	}

    public float scaleFactor
    {
        get { return m_canvas.scaleFactor;  }
        set { m_canvas.scaleFactor = value; }
    }

//---------------------- Private Implementation -------------------
	private bool m_isTransitioning 	= false;
	private bool m_hasTransition	= false;

	private Canvas m_canvas;
	private CanvasGroup m_canvasGroup;
	private GraphicRaycaster m_graphicRaycaster;


}
