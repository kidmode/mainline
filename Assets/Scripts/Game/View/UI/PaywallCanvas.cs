using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PaywallCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_message = getView ("messageText") as UILabel;
		l_message.text = Localization.getString (Localization.TXT_97_LABEL_MESSAGE);
		UILabel l_upgradeButton = getView ("upgradeButton").getView("Text") as UILabel;
		l_upgradeButton.text = Localization.getString (Localization.TXT_97_BUTTON_UPGRADE);
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
}
