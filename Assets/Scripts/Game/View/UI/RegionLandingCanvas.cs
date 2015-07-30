using UnityEngine;
using System.Collections;

public class RegionLandingCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);

		SetupLocalizition ();
	}

	public override void update ()
	{
		base.update ();
	}

	public override void dispose (bool p_deep)
	{
		base.dispose (p_deep);
	}

	public override void enteringTransition ()
	{
		base.enteringTransition ();
	}

	public override void exitingTransition ()
	{
		base.exitingTransition ();
	}

	private void SetupLocalizition()
	{
//		UILabel l_regionHeader 	= getView("speechBubble").getView("header") as UILabel;
//		UILabel l_regionBody 	= getView("speechBubble").getView("body") as UILabel;
		UILabel l_mapLabel 		= getView("mapsButton").getView("btnText") as UILabel;
		UILabel l_backLabel 	= getView("backButton").getView("btnText") as UILabel;

//		l_regionHeader.text = Localization.getString(Localization.TXT_LABEL_REGION_HEADER);
//		l_regionBody.text 	= Localization.getString(Localization.TXT_LABEL_REGION_BODY);
		l_mapLabel.text 	= Localization.getString(Localization.TXT_BUTTON_MAPS);
		l_backLabel.text 	= Localization.getString(Localization.TXT_BUTTON_BACK);
	}
}
