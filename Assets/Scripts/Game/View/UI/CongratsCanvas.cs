using UnityEngine;
using System.Collections;

public class CongratsCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_earn = getView ("staticInfo") as UILabel;
		l_earn.text =  Localization.getString (Localization.TXT_83_LABEL_STATIC_INFO);
		UILabel l_zp = getView ("zpInfo") as UILabel;
		l_zp.text =  Localization.getString (Localization.TXT_83_LABEL_ZP_INFO);
		UILabel l_request = getView ("requestInfo") as UILabel;
		l_request.text =  Localization.getString (Localization.TXT_83_LABEL_REQUEST_INFO);
		UILabel l_title = getView ("congratsLabel") as UILabel;
		l_title.text =  Localization.getString (Localization.TXT_38_LABEL_TOP);
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
