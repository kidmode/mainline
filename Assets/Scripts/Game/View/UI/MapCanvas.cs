using UnityEngine;
using System.Collections;

public class MapCanvas : UICanvas
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
		UILabel l_jungleHeaderLabel = getView("infoPanel")		.getView("headerText") as UILabel;
		UILabel l_jungleBodyLabel 	= getView("infoPanel")		.getView("bodyText") as UILabel;
		UILabel l_savannaLabel 		= getView("savannaButton")	.getView("locationText") as UILabel;
		UILabel l_jungleLabel 		= getView("jungleButton")	.getView("locationText") as UILabel;
		UILabel l_entranceLabel 	= getView("entranceButton")	.getView("btnText") as UILabel;

		l_jungleHeaderLabel.text = Localization.getString(Localization.TXT_LABEL_JUNGLE_HEADER);
		l_jungleBodyLabel.text	 = Localization.getString(Localization.TXT_LABEL_JUNGLE_BODY);
		l_savannaLabel.text		 = Localization.getString(Localization.TXT_BUTTON_SAVANNAH);
		l_jungleLabel.text		 = Localization.getString(Localization.TXT_BUTTON_JUNGLE);
		l_entranceLabel.text	 = Localization.getString(Localization.TXT_BUTTON_PROFILES);
	}
}
