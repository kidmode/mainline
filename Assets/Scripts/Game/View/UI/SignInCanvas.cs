using UnityEngine;
using System.Collections;

public class SignInCanvas : UICanvas 
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
		tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
	}
	
	
	//-- Private Implementation --
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private void SetupLocalizition()
	{
		UILabel l_back = getView ("backButton").getView("btnText") as UILabel;
		UILabel l_top = getView ("topicText") as UILabel;
		UILabel l_email = getView ("emailAddressText") as UILabel;
		UILabel l_password = getView ("passwordText") as UILabel;
		UILabel l_signIn = getView ("signInBtnText") as UILabel;

		UILabel l_quit = getView ("quitButton").getView("btnText") as UILabel;
		
		l_back.text = Localization.getString (Localization.TXT_BUTTON_BACK);
		l_top.text = Localization.getString (Localization.TXT_21_LABEL_TOP);
		l_email.text = Localization.getString (Localization.TXT_21_LABEL_EMAIL);
		l_password.text = Localization.getString (Localization.TXT_21_LABEL_PASSWORD);
		l_signIn.text = Localization.getString (Localization.TXT_21_LABEL_SIGNIN);

		l_quit.text = Localization.getString (Localization.TXT_BUTTON_QUIT);
	}
}
