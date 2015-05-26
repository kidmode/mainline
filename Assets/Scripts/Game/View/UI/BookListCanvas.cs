using UnityEngine;
using System.Collections;

public class BookListCanvas : UICanvas
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
		UILabel l_recorded = getView ("Prototype").getView("recordText") as UILabel;
		UILabel l_record = getView ("Prototype").getView("recordButton").getView("Text") as UILabel;
		
		l_top.text = Localization.getString (Localization.TXT_73_LABEL_TITLE);
		l_recorded.text = Localization.getString (Localization.TXT_73_LABEL_RECORDED);
		l_record.text = Localization.getString (Localization.TXT_73_LABEL_RECORD);
	}
}
