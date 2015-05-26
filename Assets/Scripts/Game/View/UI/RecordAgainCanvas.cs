using UnityEngine;
using System.Collections;

public class RecordAgainCanvas : UICanvas
{

	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_message = getView ("contentText") as UILabel;
		l_message.text =  Localization.getString (Localization.TXT_81_LABEL_MESSAGE);
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
