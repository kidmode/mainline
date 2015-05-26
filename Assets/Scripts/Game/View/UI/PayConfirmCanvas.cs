using UnityEngine;
using System.Collections;

public class PayConfirmCanvas : UICanvas 
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
		UILabel l_back = getView("backButton").getView("btnText") as UILabel;
		UILabel l_top = getView("congraturationLabel") as UILabel;
		UILabel l_notice = getView("noticeText") as UILabel;
		
		l_back.text = Localization.getString( Localization.TXT_BUTTON_BACK );
		l_top.text = Localization.getString( Localization.TXT_38_LABEL_TOP );
		l_notice.text = Localization.getString( Localization.TXT_38_LABEL_NOTICE );
	}
}
