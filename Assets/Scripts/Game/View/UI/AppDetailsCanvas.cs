using UnityEngine;
using System.Collections;

public class AppDetailsCanvas : UICanvas
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
		UILabel l_top = getView ("titleArea").getView("Text") as UILabel;
		UILabel l_safety = getView ("safetyLabel") as UILabel;
		UILabel l_age = getView ("ageLabel") as UILabel;

		l_top.text = Localization.getString (Localization.TXT_70_LABEL_TITLE);
		l_safety.text = Localization.getString (Localization.TXT_70_LABEL_SAFETY);
		l_age.text = Localization.getString (Localization.TXT_70_LABEL_AGES);
	}
}
