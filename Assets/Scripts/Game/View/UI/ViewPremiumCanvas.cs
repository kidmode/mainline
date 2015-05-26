using UnityEngine;
using System.Collections;

public class ViewPremiumCanvas : UICanvas 
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
		UILabel l_exit = getView("exitButton").getView("btnText") as UILabel;
		UILabel l_top = getView("topImage").getView("topText") as UILabel;
		UILabel l_line1 = getView("line1").getView("Text") as UILabel;
		UILabel l_line2 = getView("line2").getView("Text") as UILabel;
		UILabel l_line3 = getView("line3").getView("Text") as UILabel;
		UILabel l_line4 = getView("line4").getView("Text") as UILabel;
		UILabel l_topMonth = getView("monthCard").getView("topText") as UILabel;
		UILabel l_topAnnual = getView("yearlyCard").getView("topText") as UILabel;
		UILabel l_saveMonth = getView("monthCard").getView("saveText") as UILabel;
		UILabel l_saveAnnual = getView("yearlyCard").getView("saveText") as UILabel;
		UILabel l_limitMonth = getView("monthCard").getView("nowPrice").getView("Text") as UILabel;
		UILabel l_limitAnnual = getView("yearlyCard").getView("nowPrice").getView("Text") as UILabel;
		UILabel l_upgradeMonth = getView("monthCard").getView("purchaseBtnText") as UILabel;
		UILabel l_upgradeAnnual = getView("yearlyCard").getView("purchaseBtnText") as UILabel;
		
		l_exit.text = Localization.getString( Localization.TXT_BUTTON_QUIT );
		l_top.text = Localization.getString( Localization.TXT_35_LABEL_TOP );
		l_line1.text = Localization.getString( Localization.TXT_35_LABEL_LINE_1 );
		l_line2.text = Localization.getString( Localization.TXT_35_LABEL_LINE_2 );
		l_line3.text = Localization.getString( Localization.TXT_35_LABEL_LINE_3 );
		l_line4.text = Localization.getString( Localization.TXT_35_LABEL_LINE_4 );
		l_topMonth.text = Localization.getString( Localization.TXT_35_LABEL_TOP_MONTH );
		l_topAnnual.text = Localization.getString( Localization.TXT_35_LABEL_TOP_ANNUAL );
		l_saveMonth.text = Localization.getString( Localization.TXT_35_LABEL_SAVE );
		l_saveAnnual.text = Localization.getString( Localization.TXT_35_LABEL_SAVE );
		l_limitMonth.text = Localization.getString( Localization.TXT_35_LABEL_LIMIT );
		l_limitAnnual.text = Localization.getString( Localization.TXT_35_LABEL_LIMIT );
		l_upgradeMonth.text = Localization.getString( Localization.TXT_35_LABEL_UPGRADE );
		l_upgradeAnnual.text = Localization.getString( Localization.TXT_35_LABEL_UPGRADE );
	}
}
