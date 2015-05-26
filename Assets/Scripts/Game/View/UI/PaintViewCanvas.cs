using UnityEngine;
using System.Collections;

public class PaintViewCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_library = getView("backButton").getView ("Text") as UILabel;
		l_library.text = Localization.getString (Localization.TXT_93_BUTTON_LIBRARY);
		UILabel l_edit = getView("editButton").getView ("Text") as UILabel;
		l_edit.text = Localization.getString (Localization.TXT_93_BUTTON_EDIT);
		UILabel l_delete = getView("deleteButton").getView ("Text") as UILabel;
		l_delete.text = Localization.getString (Localization.TXT_93_BUTTON_DELETE);
		UILabel l_load = getView("loadingText") as UILabel;
		l_load.text = Localization.getString (Localization.TXT_93_LABEL_LOADING);
		UILabel l_dialogTitle = getView("messageDialog").getView ("titleText") as UILabel;
		l_dialogTitle.text = Localization.getString (Localization.TXT_93_LABEL_MESSAGE_TITLE);
		UILabel l_deleting = getView("messageDialog").getView ("contentText") as UILabel;
		l_deleting.text = Localization.getString (Localization.TXT_93_LABEL_DELETING);
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
