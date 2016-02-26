using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FAQDialogCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_faq1 = getView ("1.faqcontent") as UILabel;
		l_faq1.text = Localization.getString (Localization.TXT_88_LABEL_1);
		UILabel l_faq2 = getView ("2.faqcontent") as UILabel;
		l_faq2.text = Localization.getString (Localization.TXT_88_LABEL_2);
		UILabel l_faq3 = getView ("3.faqcontent") as UILabel;
		l_faq3.text = Localization.getString (Localization.TXT_88_LABEL_3);
		UILabel l_faq4 = getView ("4.faqcontent") as UILabel;
		l_faq4.text = Localization.getString (Localization.TXT_88_LABEL_4);
		UILabel l_faq5 = getView ("5.faqcontent") as UILabel;
		l_faq5.text = Localization.getString (Localization.TXT_88_LABEL_5);
		UILabel l_faq6 = getView ("6.faqcontent") as UILabel;
		l_faq6.text = Localization.getString (Localization.TXT_88_LABEL_6);
		UILabel l_faq7 = getView ("7.faqcontent") as UILabel;
		l_faq7.text = Localization.getString (Localization.TXT_88_LABEL_7);
		UILabel l_faq8 = getView ("8.faqcontent") as UILabel;
		l_faq8.text = Localization.getString (Localization.TXT_88_LABEL_8);
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
