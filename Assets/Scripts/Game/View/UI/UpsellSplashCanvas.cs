using UnityEngine;
using System.Collections;

public class UpsellSplashCanvas : UICanvas 
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
		UILabel l_profiles   = getView("profilesButton").getView("btnText") as UILabel;
		UILabel l_premiumTitle   = getView("premiumCard").getView("titleText") as UILabel;
		UILabel l_premiumContent   = getView("premiumCardContent").getView("contentText") as UILabel;
		UILabel l_plan   = getView("viewPlanButton").getView("continueBtnText") as UILabel;
		UILabel l_gemsTitle   = getView("gemsCard").getView("titleText") as UILabel;
		UILabel l_gemsContent   = getView("gemCardContent").getView("contentText") as UILabel;
		UILabel l_gems   = getView("getGemsButton").getView("getGemsBtnText") as UILabel;
		UILabel l_dashboard   = getView("continueButton").getView("continueBtnText") as UILabel;
		
		l_profiles.text = Localization.getString( Localization.TXT_BUTTON_PROFILES );
		l_premiumTitle.text = Localization.getString( Localization.TXT_34_LABEL_PREMIUM_TITLE );
		l_premiumContent.text = Localization.getString( Localization.TXT_34_LABEL_PREMIUM_CONTENT );
		l_plan.text = Localization.getString( Localization.TXT_34_LABEL_PLAN );
		l_gemsTitle.text = Localization.getString( Localization.TXT_34_LABEL_GEMS_TITLE );
		l_gemsContent.text = Localization.getString( Localization.TXT_34_LABEL_GMES_CONTENT );
		l_gems.text = Localization.getString( Localization.TXT_34_LABEL_GEMS );
		l_dashboard.text = Localization.getString( Localization.TXT_34_LABEL_DASHBORAD );
	}
}
