using UnityEngine;
using System.Collections;

public class RecommendedBookCanvas : UICanvas
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
		tweener.addAlphaTrack( 1.0f, 0.0f, 0.0f, onFadeFinish );
	}

	public override void exitingTransition ()
	{
		base.exitingTransition ();
	}
	
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private void SetupLocalizition()
	{
		UILabel l_top = getView ("titleText") as UILabel;
		UILabel l_gem = getView ("gemText") as UILabel;
		UILabel l_more = getView ("bookListButton").getView ("Text") as UILabel;
		
		l_top.text = Localization.getString (Localization.TXT_69_LABEL_TITLE);
		l_gem.text = Localization.getString (Localization.TXT_68_LABEL_GEMS);
		l_more.text = Localization.getString (Localization.TXT_69_LABEL_MORE);

		UILabel l_loading = getView ("loadingText") as UILabel;
		l_loading.text = Localization.getString (Localization.TXT_LABEL_LOADING);
	}
}
