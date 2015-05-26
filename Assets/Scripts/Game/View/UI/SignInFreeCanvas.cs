using UnityEngine;
using System.Collections;

public class SignInFreeCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_title = getView ("PremiumList").getView ("titleText") as UILabel;
		l_title.text = Localization.getString (Localization.TXT_100_LABEL_TITLE_TEXT);
		UILabel l_list1 = getView ("list1").getView ("Text") as UILabel;
		l_list1.text = Localization.getString (Localization.TXT_100_LABEL_LIST1);
		UILabel l_list2 = getView ("list2").getView ("Text") as UILabel;
		l_list2.text = Localization.getString (Localization.TXT_100_LABEL_LIST2);
		UILabel l_list3 = getView ("list3").getView ("Text") as UILabel;
		l_list3.text = Localization.getString (Localization.TXT_100_LABEL_LIST3);
		UILabel l_messageText = getView ("messageText") as UILabel;
		l_messageText.text = Localization.getString (Localization.TXT_100_LABEL_MESSAGE_TEXT);
		UILabel l_continuedText = getView ("continueText") as UILabel;
		l_continuedText.text = Localization.getString (Localization.TXT_100_LABEL_CONTINU_TEXT);
		UILabel l_kidModeFree = getView ("freeButton").getView("Text") as UILabel;
		l_kidModeFree.text = Localization.getString (Localization.TXT_100_BUTTON_KIDMODE_FREE);
		UILabel l_back = getView ("backButton").getView("Text") as UILabel;
		l_back.text = Localization.getString (Localization.TXT_BUTTON_BACK);
		UILabel l_getPremium = getView ("upgradeButton").getView("Text") as UILabel;
		l_getPremium.text = Localization.getString (Localization.TXT_100_BUTTON_GET_PREMIUM);
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
