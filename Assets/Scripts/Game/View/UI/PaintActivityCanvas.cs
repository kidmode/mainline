using UnityEngine;
using System.Collections;

public class PaintActivityCanvas : UICanvas
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
		UILabel l_save = getView ("SaveAndExitText") as UILabel;
		UILabel l_undo = getView ("UndoText") as UILabel;
		UILabel l_paint = getView ("PaintText") as UILabel;
		UILabel l_erase = getView ("EraseText") as UILabel;
		UILabel l_brush = getView ("BrushSizeText") as UILabel;
		UILabel l_colors = getView ("ColorsText") as UILabel;
		
		l_save.text = Localization.getString (Localization.TXT_15_LABEL_SAVE);
		l_undo.text = Localization.getString (Localization.TXT_15_LABEL_UNDO);
		l_paint.text = Localization.getString (Localization.TXT_15_LABEL_PAINT);
		l_erase.text = Localization.getString (Localization.TXT_15_LABEL_ERASE);
		l_brush.text = Localization.getString (Localization.TXT_15_LABEL_BRUSH);
		l_colors.text = Localization.getString (Localization.TXT_15_LABEL_COLORS);
	}
}
