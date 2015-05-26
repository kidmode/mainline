using UnityEngine;
using System.Collections;

public class ResetHomeCanvas : UICanvas
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
		UILabel l_action = getView("CallToActionText") as UILabel;
		UILabel l_warning = getView("WarningText") as UILabel;
		UILabel l_titleStep1 = getView("Step1Title") as UILabel;
		UILabel l_titleStep2  = getView("Step2Title") as UILabel;
		UILabel l_titleStep3  = getView("Step3Title") as UILabel;
		UILabel l_step1 = getView("Step1") as UILabel;
		UILabel l_step2 = getView("Step2") as UILabel;
		UILabel l_step3 = getView("Step3") as UILabel;
		UILabel l_begin = getView("StartText") as UILabel;
		
		l_title.text = Localization.getString( Localization.TXT_42_LABEL_TITLT );
		l_action.text = Localization.getString( Localization.TXT_42_LABEL_ACTION );
		l_warning.text = Localization.getString( Localization.TXT_42_LABEL_WARNING );
		l_titleStep1.text = Localization.getString( Localization.TXT_42_LABEL_TITLE_STEP1 );
		l_titleStep2.text = Localization.getString( Localization.TXT_42_LABEL_TITLE_STEP2 );
		l_titleStep3.text = Localization.getString( Localization.TXT_42_LABEL_TITLE_STEP3 );
		l_step1.text = Localization.getString( Localization.TXT_42_LABEL_STEP1 );
		l_step2.text = Localization.getString( Localization.TXT_42_LABEL_STEP2 );
		l_step3.text = Localization.getString( Localization.TXT_42_LABEL_STEP3 );
		l_begin.text = Localization.getString( Localization.TXT_42_LABEL_BEGIN );
	}
}
