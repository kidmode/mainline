using UnityEngine;
using System.Collections;

public class BuyGemsCanvas : UICanvas 
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
		UILabel l_top = getView("topicArea").getView("topic") as UILabel;
		UILabel l_notice = getView("topicArea").getView("notice") as UILabel;
		UILabel l_topGood = getView("goodCard").getView("topText") as UILabel;
		UILabel l_gemGood = getView("goodCard").getView("goodPrice") as UILabel;
		UILabel l_purchaseGood = getView("goodCard").getView("purchaseBtnText") as UILabel;
		UILabel l_topBetter = getView("betterCard").getView("topText") as UILabel;
		UILabel l_gemBetter = getView("betterCard").getView("betterPrice") as UILabel;
		UILabel l_purchaseBetter = getView("betterCard").getView("purchaseBtnText") as UILabel;
		UILabel l_topBest = getView("bestCard").getView("topText") as UILabel;
		UILabel l_gemBest = getView("bestCard").getView("bestPrice") as UILabel;
		UILabel l_purchaseBest = getView("bestCard").getView("purchaseBtnText") as UILabel;
		
		l_back.text = Localization.getString( Localization.TXT_BUTTON_BACK );
		l_top.text = Localization.getString( Localization.TXT_36_LABEL_TOP );
		l_notice.text = Localization.getString( Localization.TXT_36_LABEL_NOTICE );
		l_topGood.text = Localization.getString( Localization.TXT_36_LABEL_TOP_GOOD );
		l_gemGood.text = Localization.getString( Localization.TXT_36_LABEL_GEMS );
		l_purchaseGood.text = Localization.getString( Localization.TXT_36_LABEL_PURCHASE );
		l_topBetter.text = Localization.getString( Localization.TXT_36_LABEL_TOP_BETTER );
		l_gemBetter.text = Localization.getString( Localization.TXT_36_LABEL_GEMS );
		l_purchaseBetter.text = Localization.getString( Localization.TXT_36_LABEL_PURCHASE );
		l_topBest.text = Localization.getString( Localization.TXT_36_LABEL_TOP_BEST );
		l_gemBest.text = Localization.getString( Localization.TXT_36_LABEL_GEMS );
		l_purchaseBest.text = Localization.getString( Localization.TXT_36_LABEL_PURCHASE );
	}
}
