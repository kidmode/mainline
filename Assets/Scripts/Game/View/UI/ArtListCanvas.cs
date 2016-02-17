using UnityEngine;
using System.Collections;

public class ArtListCanvas : UICanvas {

	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
//		UILabel l_title = getView("titleArea").getView ("Text") as UILabel;
//		l_title.text =  Localization.getString (Localization.TXT_78_LABEL_TITLE);
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
