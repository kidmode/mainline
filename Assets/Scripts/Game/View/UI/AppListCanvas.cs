using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppListCanvas : UICanvas
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
		
		l_top.text = Localization.getString (Localization.TXT_72_LABEL_TITLE);
	}
}
