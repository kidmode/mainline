using UnityEngine;
using System.Collections;

public class RequestHomeCanvas : UICanvas
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
		UILabel l_title = getView("Title") as UILabel;
		UILabel l_explanation = getView("Explanation") as UILabel;
		UILabel l_step1 = getView("Step1") as UILabel;
		UILabel l_step2 = getView("Step2") as UILabel;
		UILabel l_begin = getView("StartText") as UILabel;
		
		l_title.text = Localization.getString( Localization.TXT_43_LABEL_TITLE );
		l_explanation.text = Localization.getString( Localization.TXT_43_LABEL_EXPLANATION );
		l_step1.text = Localization.getString( Localization.TXT_43_LABEL_STEP1 );
		l_step2.text = Localization.getString( Localization.TXT_43_LABEL_STEP2 );
		l_begin.text = Localization.getString( Localization.TXT_43_LABEL_BEGIN );
	}
}
