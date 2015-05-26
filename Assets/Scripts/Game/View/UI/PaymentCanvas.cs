using UnityEngine;
using System.Collections;

public class PaymentCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		SetupLocalizition ();
		
		//tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );
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
		UILabel l_back = getView("exitButton").getView("btnText") as UILabel;
		UILabel l_best = getView("recommendImage").getView("Text") as UILabel;
		UILabel l_card = getView("cardNumberTitle") as UILabel;
		UILabel l_expire = getView("cardExpirationTitle") as UILabel;
		UILabel l_message = getView("messageText") as UILabel;
		UILabel l_amount = getView("amountText") as UILabel;
		UILabel l_purchase = getView("purchaseBtnText") as UILabel;
		
		l_back.text = Localization.getString( Localization.TXT_BUTTON_BACK );
		l_best.text = Localization.getString( Localization.TXT_37_LABEL_BEST );
		l_card.text = Localization.getString( Localization.TXT_37_LABEL_CARD );
		l_expire.text = Localization.getString( Localization.TXT_37_LABEL_EXPIRES );
		l_message.text = Localization.getString( Localization.TXT_37_LABEL_MESSAGE );
		l_amount.text = Localization.getString( Localization.TXT_37_LABEL_AMOUNT );
		l_purchase.text = Localization.getString( Localization.TXT_37_LABEL_PURCHASE );
	}
}
