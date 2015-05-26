using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SentFeedBackCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		SetupLocalizition ();

		m_dialog = getView ("dialog") as UIElement;

		m_dialogMovePosition = 800;
	}
	
	public override void update()
	{
		base.update();
	}
	
	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}
	
	public override void enteringTransition()
	{
		base.enteringTransition();
		tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
	}

	public void setOriginalPosition()
	{
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_dialog.transform.localPosition );
		l_pointListOut.Add( m_dialog.transform.localPosition - new Vector3( 0, m_dialogMovePosition, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f);
	}

	public void setOutPosition()
	{
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_dialog.transform.localPosition );
		l_pointListOut.Add( m_dialog.transform.localPosition + new Vector3( 0, m_dialogMovePosition, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f);
	}
	
	//-- Private Implementation --
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}
	
	private void SetupLocalizition()
	{
		UILabel l_title = getView("dialogTitle").getView("Text") as UILabel;
		UILabel l_content = getView("dialogContent").getView("Text") as UILabel;
		UILabel l_ask = getView("askQuestion").getView("messageText") as UILabel;
		UILabel l_idea = getView("submitIdea").getView("messageText") as UILabel;
		UILabel l_problem = getView("reportProblem").getView("messageText") as UILabel;
		UILabel l_compliment = getView("giveCompliment").getView("messageText") as UILabel;
		UILabel l_send = getView("sendButton").getView("sendBtnText") as UILabel;
		
		l_title.text = Localization.getString( Localization.TXT_41_LABEL_TITLE );
		l_content.text = Localization.getString( Localization.TXT_41_LABEL_CONTENT );
		l_ask.text = Localization.getString( Localization.TXT_41_LABEL_ASK );
		l_idea.text = Localization.getString( Localization.TXT_41_LABEL_IDEA );
		l_problem.text = Localization.getString( Localization.TXT_41_LABEL_PROBLEM );
		l_compliment.text = Localization.getString( Localization.TXT_41_LABEL_COMPLIMENT );
		l_send.text = Localization.getString( Localization.TXT_41_LABEL_SEND );
	}

	private UIElement m_dialog;
	private int m_dialogMovePosition;
}
