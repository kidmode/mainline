using UnityEngine;
using System.Collections;

public class LoadingEntranceCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		UILabel l_loading = getView ("loadingLabel") as UILabel;
		l_loading.text = Localization.getString (Localization.TXT_LABEL_LOADING);
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
