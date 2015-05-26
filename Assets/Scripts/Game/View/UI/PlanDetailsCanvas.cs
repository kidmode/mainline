using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlanDetails : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );

		SetupLocalizition ();
		
		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );

		m_dialog = getView ("dialog") as UIElement;
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
		UILabel l_title = getView ("dialogTitle").getView("Text") as UILabel;
		UILabel l_plan = getView ("planText") as UILabel;
		UILabel l_notice = getView ("noticeText") as UILabel;
		UILabel l_cancel = getView ("cancelSubscriptionNoticeplanText") as UILabel;
		
		l_title.text = Localization.getString (Localization.TXT_30_LABEL_TITLE);
		l_plan.text = Localization.getString (Localization.TXT_30_LABEL_PLAN);
		l_notice.text = Localization.getString (Localization.TXT_30_LABEL_NOTICE);
		l_cancel.text = Localization.getString (Localization.TXT_30_LABEL_CANCEL);
	}

	private UIElement m_dialog;
	private int m_dialogMovePosition = 800;
}
