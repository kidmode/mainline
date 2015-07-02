using UnityEngine;
using System.Collections;

public class RecordAReadingCanvas : UICanvas
{	
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_title = getView ("titleImage").getView ("titleText") as UILabel;
		l_title.text = Localization.getString (Localization.TXT_98_LABEL_TITLE);
//		UILabel l_yourGems = getView ("gemText").getView ("Text") as UILabel;
//		l_yourGems.text = Localization.getString (Localization.TXT_98_BUTTON_YOUR_GEMS);
		UILabel l_seeMoreBook = getView ("bookListButton").getView ("Text") as UILabel;
		l_seeMoreBook.text = Localization.getString (Localization.TXT_98_BUTTON_SEE_MORE);
		UILabel l_memberButton = getView ("memberButton").getView ("Text") as UILabel;
		l_memberButton.text = Localization.getString (Localization.TXT_98_BUTTON_BECOME_MEMBER);

		UILabel l_loading = getView ("loadingText") as UILabel;
		l_loading.text = Localization.getString (Localization.TXT_LABEL_LOADING);
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
}
