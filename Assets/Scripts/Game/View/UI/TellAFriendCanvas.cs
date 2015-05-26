using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TellAFriendCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_titleText = getView ("tellFriendTitleText") as UILabel;
		l_titleText.text = Localization.getString (Localization.TXT_94_LABEL_TITLE);
		UILabel l_fromText = getView ("FromLabel") as UILabel;
		l_fromText.text = Localization.getString (Localization.TXT_94_LABEL_FROM);
		UILabel l_toText = getView("toArea").getView ("FromLabel") as UILabel;
		l_toText.text = Localization.getString (Localization.TXT_94_LABEL_TO);
		UILabel l_contextText = getView("contentText") as UILabel;
		l_contextText.text = Localization.getString (Localization.TXT_94_LABEL_CONTENT_TEXT);
		UILabel l_optionalText = getView("inputArea").getView("Text") as UILabel;
		l_optionalText.text = Localization.getString (Localization.TXT_94_LABEL_OPTIONAL_MESSAGE);
		UILabel l_sendEmailText = getView("sendEmailText") as UILabel;
		l_sendEmailText.text = Localization.getString (Localization.TXT_94_BUTTON_SEND_EMAIL);
		UILabel l_sendingText =getView("confirmDialog").getView("contentText") as UILabel;
		l_sendingText.text = Localization.getString (Localization.TXT_94_LABEL_SEND);
		UILabel l_closeButtonText = getView("confirmDialog").getView("closeButton").getView("Text") as UILabel;
		l_closeButtonText.text = Localization.getString (Localization.TXT_94_BUTTON_CLOSE);
		UILabel l_dialogTitleText = getView("confirmDialog").getView("titleText") as UILabel;
		l_dialogTitleText.text = Localization.getString (Localization.TXT_94_LABEL_SUCCESS);
		UILabel l_errorText = getView("errorLabel") as UILabel;
		l_errorText.text = Localization.getString (Localization.TXT_94_LABEL_ERROR);
		UILabel l_failText = getView("confirmDialog").getView("titleText") as UILabel;
		l_failText.text = Localization.getString (Localization.TXT_STATE_11_FAIL);
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