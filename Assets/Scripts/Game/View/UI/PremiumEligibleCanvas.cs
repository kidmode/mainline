using UnityEngine;
using System.Collections;

public class PremiumEligibleCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		setupLocalization ();
	}

	public void setupLocalization()
	{
		UILabel l_title = getView ("title").getView ("Text") as UILabel;
		l_title.text = Localization.getString (Localization.TXT_105_LABEL_TITLE);
		UILabel l_contentText = getView ("content").getView ("Text") as UILabel;
		l_contentText.text = Localization.getString (Localization.TXT_105_LABEL_CONTENT);
		UILabel l_continueText = getView ("continueButton").getView("Text") as UILabel;
		l_continueText.text = Localization.getString (Localization.TXT_105_BUTTON_CONTINUE);
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
