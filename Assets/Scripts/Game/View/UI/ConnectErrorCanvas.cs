using UnityEngine;
using System.Collections;

public class ConnectErrorCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_titleText = getView ("titleText") as UILabel;
		l_titleText.text = Localization.getString (Localization.TXT_95_LABEL_TITLE);
		UILabel l_summaryText = getView ("summary") as UILabel;
		l_summaryText.text = Localization.getString (Localization.TXT_95_LABEL_SUMMARY);
		UILabel l_tipText = getView ("tip") as UILabel;
		l_tipText.text = Localization.getString (Localization.TXT_95_LABEL_TIP);
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
