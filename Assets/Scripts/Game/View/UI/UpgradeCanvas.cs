using UnityEngine;
using System.Collections;

public class UpgradeCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_title = getView("titleArea").getView ("Text") as UILabel;
		l_title.text = Localization.getString (Localization.TXT_92_LABEL_TITLE);
		UILabel l_itlem1 =  getView("Text1") as UILabel;
		l_itlem1.text = Localization.getString (Localization.TXT_92_LABEL_ITLEM1);
		UILabel l_itlem2 =  getView("Text2") as UILabel;
		l_itlem2.text = Localization.getString (Localization.TXT_92_LABEL_ITLEM2);
		UILabel l_itlem3 =  getView("Text3") as UILabel;
		l_itlem3.text = Localization.getString (Localization.TXT_92_LABEL_ITLEM3);
		UILabel l_itlem4 =  getView("Text4") as UILabel;
		l_itlem4.text = Localization.getString (Localization.TXT_92_LABEL_ITLEM4);
		UILabel l_upgrade =  getView("upgradeButton").getView("Text") as UILabel;
		l_upgrade.text = Localization.getString (Localization.TXT_92_BUTTON_UPGRADE);

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
