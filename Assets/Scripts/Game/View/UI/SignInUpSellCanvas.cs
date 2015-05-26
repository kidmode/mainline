using UnityEngine;
using System.Collections;

public class SignInUpSellCanvas : UICanvas 
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
		UILabel l_top = getView("topicText") as UILabel;
		UILabel l_clause1 = getView("clause1").getView("Text") as UILabel;
		UILabel l_clause2 = getView("clause2").getView("Text") as UILabel;
		UILabel l_clause3 = getView("clause3").getView("Text") as UILabel;
		UILabel l_clause4 = getView("clause4").getView("Text") as UILabel;
		UILabel l_trial = getView("tryFreeButton").getView("tryFreeBtnText") as UILabel;
		UILabel l_thanks = getView("cancelButton").getView("cancelBtnText") as UILabel;
		
		l_top.text = Localization.getString( Localization.TXT_58_LABEL_TOP );
		l_clause1.text = Localization.getString( Localization.TXT_58_LABEL_CLAUSE1 );
		l_clause2.text = Localization.getString( Localization.TXT_58_LABEL_CLAUSE2 );
		l_clause3.text = Localization.getString( Localization.TXT_58_LABEL_CLAUSE3 );
		l_clause4.text = Localization.getString( Localization.TXT_58_LABEL_CLAUSE4 );
		l_trial.text = Localization.getString( Localization.TXT_58_LABEL_TRIAL );
		l_thanks.text = Localization.getString( Localization.TXT_58_LABEL_THANKS );
	}
}
