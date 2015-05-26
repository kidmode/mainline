using UnityEngine;
using System.Collections;

public class TrialMessageCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_tltleLabel = getView ("title").getView("Text") as UILabel;
		UILabel l_contentLabel = getView ("content").getView("Text") as UILabel;
		UILabel l_choosePremiumLabel = getView ("subscriptionButton").getView("Text") as UILabel;
		UILabel l_continuTrialLabel = getView ("continueButton").getView("continueText") as UILabel;

		l_tltleLabel.text = Localization.getString (Localization.TXT_103_LABEL_TITLE);
		l_contentLabel.text = Localization.getString (Localization.TXT_103_LABEL_CONTENT_NOTICE);
		l_choosePremiumLabel.text = Localization.getString (Localization.TXT_103_BUTTON_CHOOSE_SUBSCRIPTION);
		l_continuTrialLabel.text = Localization.getString (Localization.TXT_103_BUTTON_CONTINUE_TRIAL);
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
