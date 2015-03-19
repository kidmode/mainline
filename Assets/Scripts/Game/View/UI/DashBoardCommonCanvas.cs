using UnityEngine;
using System.Collections;

public class DashBoardCommonCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		if( SessionHandler.getInstance ().kidList.Count > 0 )
		{
			if( null == SessionHandler.getInstance().currentKid )
			{
				SessionHandler.getInstance().currentKid = SessionHandler.getInstance().kidList[0];
			}
			UILabel l_profileNameLabel = getView( "profileNameText" ) as UILabel;
			l_profileNameLabel.text = SessionHandler.getInstance().currentKid.name + "'s Profile";
		}
	}
	
	public override void update()
	{
		base.update();
	}

	
	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}
	
	public override void enteringTransition()
	{
		base.enteringTransition();
		tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
	}
	
	
	//-- Private Implementation --
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}
}
