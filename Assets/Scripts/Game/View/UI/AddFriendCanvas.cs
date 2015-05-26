using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AddFriendCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_titleText = getView ("titleText") as UILabel;
		l_titleText.text = Localization.getString (Localization.TXT_96_LABEL_MESSAGE);
		UILabel l_emailText = getView ("content").getView ("bgImage").getView("Text") as UILabel;
		l_emailText.text = Localization.getString (Localization.TXT_96_LABEL_EMAIL_ADDRESS);
		UILabel l_addFriendText = getView ("AddFriendlText") as UILabel;
		l_addFriendText.text = Localization.getString (Localization.TXT_96_BUTTON_ADD_FRIEND);
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