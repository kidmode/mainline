using UnityEngine;
using System.Collections;

public class DashBoardCommonCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		if( SessionHandler.getInstance ().kidList.Count > 0 )
		{
			if( null == SessionHandler.getInstance().currentKid )
			{
				SessionHandler.getInstance().currentKid = SessionHandler.getInstance().kidList[0];
			}
		}
		
		SetupLocalizition ();
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
		UILabel l_dashborad = getView ("middleLabelArea").getView("Text") as UILabel;
		UILabel l_profile = getView ("middleLabelArea").getView("profileNameText") as UILabel;
		UILabel l_overview = getView ("overviewButton").getView("Text") as UILabel;
		UILabel l_control = getView ("controlButton").getView("Text") as UILabel;
		UILabel l_apps = getView ("appsButton").getView("Text") as UILabel;
		UILabel l_setting = getView ("buttonSettings").getView("Text") as UILabel;
//		UILabel l_assessment = getView ("starButton").getView("Text") as UILabel;
//		UILabel l_block = getView ("starButton").getView("blockText") as UILabel;
		
		l_dashborad.text = Localization.getString (Localization.TXT_26_LABEL_DASHBOARD);
		l_profile.text = SessionHandler.getInstance().currentKid.name + " " + Localization.getString (Localization.TXT_26_LABEL_PROFILE);
		l_overview.text = Localization.getString (Localization.TXT_26_LABEL_OVERVIEW);
		l_control.text = Localization.getString (Localization.TXT_26_LABEL_CONTROL);
		l_apps.text = Localization.getString (Localization.TXT_26_LABEL_APPS);
		l_setting.text = Localization.getString (Localization.TXT_26_LABEL_SETTINGS);
//		l_assessment.text = Localization.getString (Localization.TXT_26_LABEL_ASSESSMENT);
//		l_block.text = Localization.getString (Localization.TXT_26_LABEL_BLOCK);
	}
}
