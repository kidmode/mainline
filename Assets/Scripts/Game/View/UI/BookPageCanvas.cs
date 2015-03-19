using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BookPageCanvas : UICanvas
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
        getView("content").tweener.addAlphaTrack( 0.0f, 1.0f, 2.0f );
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



//------------------------------- Private Implementation -------------------------------
//--------------------------------------------------------------------------------------
    private void onFadeFinish(UIElement p_element, Tweener.TargetVar p_targetVariable)
    {
        UICanvas l_canvas = p_element as UICanvas;
        l_canvas.isTransitioning = false;
    }
}
