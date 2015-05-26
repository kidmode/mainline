using UnityEngine;
using System.Collections;

public class SettingCommonCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		SetupLocalizition ();

//		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );
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
		UILabel l_dashboard = getView("dashboardText") as UILabel;
		UILabel l_setting = getView("settingsText") as UILabel;
		UILabel l_general = getView("overviewButton").getView("Text") as UILabel;
		UILabel l_device = getView("controlButton").getView("Text") as UILabel;
		UILabel l_faq = getView("starButton").getView("Text") as UILabel;
		
		l_dashboard.text = Localization.getString( Localization.TXT_46_LABEL_DASHBOARD );
		l_setting.text = Localization.getString( Localization.TXT_46_LABEL_SETTINGS );
		l_general.text = Localization.getString( Localization.TXT_46_LABEL_GENERAL );
		l_device.text = Localization.getString( Localization.TXT_46_LABEL_DEVICE );
		l_faq.text = Localization.getString( Localization.TXT_46_LABEL_FAQ );
	}
}
