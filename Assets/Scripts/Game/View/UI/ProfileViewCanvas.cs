using UnityEngine;
using System.Collections;

public class ProfileViewCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		//tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_addChild = getView ("addChildBtnText") as UILabel;
		l_addChild.text = Localization.getString (Localization.TXT_86_BUTTON_ADD_CHILD);
		UILabel l_finish = getView ("finishBtnText") as UILabel;
		l_finish.text = Localization.getString (Localization.TXT_86_BUTTON_FINISH);
		UILabel l_back = getView ("backButton").getView("btnText") as UILabel;
		l_back.text = Localization.getString (Localization.TXT_BUTTON_BACK);
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
