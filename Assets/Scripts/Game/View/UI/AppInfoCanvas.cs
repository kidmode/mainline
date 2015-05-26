using UnityEngine;
using System.Collections;

public class AppInfoCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );

		SetupLocalizition ();
		
		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );
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
		tweener.addAlphaTrack( 1.0f, 0.0f, 0.0f, onFadeFinish );
	}
	
	
	//-- Private Implementation --
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private void SetupLocalizition()
	{
		UILabel l_title = getView("titleText") as UILabel;
		UILabel l_plan = getView("planDetailsBtnText") as UILabel;
		UILabel l_message = getView("messageText") as UILabel;
		UILabel l_edit = getView("editProfileBtnText") as UILabel;
		UILabel l_signOut = getView("signOutBtnText") as UILabel;
		UILabel l_feedback = getView ("feedBackButton").getView("Text") as UILabel;
		
		l_title.text = Localization.getString (Localization.TXT_27_LABEL_TITLE);
		l_plan.text = Localization.getString (Localization.TXT_27_LABEL_PLAN);
		l_message.text = Localization.getString (Localization.TXT_27_LABEL_MESSAGE);
		l_edit.text = Localization.getString (Localization.TXT_27_LABEL_EDIT);
		l_signOut.text = Localization.getString (Localization.TXT_27_LABEL_SIGN_OUT);
		l_feedback.text = Localization.getString (Localization.TXT_27_LABEL_FEEDBACK);
	}
}
