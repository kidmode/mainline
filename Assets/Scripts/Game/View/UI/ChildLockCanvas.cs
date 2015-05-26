using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChildLockCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		SetupLocalizition ();
		
		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );
		m_dialog = getView ("dialog") as UIElement;
		m_dialogMovePosition = 891;
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
		tweener.addAlphaTrack( 1.0f, 0.0f, 0.0f, onFadeFinish );
	}

	public void setOriginalPosition()
	{
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_dialog.transform.localPosition );
		l_pointListOut.Add( m_dialog.transform.localPosition + new Vector3( 0, m_dialogMovePosition, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f );
	}
	
	public void setOutPosition()
	{
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_dialog.transform.localPosition );
		l_pointListOut.Add( m_dialog.transform.localPosition + new Vector3( 0, m_dialogMovePosition, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f );
	}
	
	//-- Private Implementation --
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private void SetupLocalizition()
	{
		UILabel l_title = getView("Title") as UILabel;
		UILabel l_titleLock = getView("childLockCheckButton").getView("titleText") as UILabel;
		UILabel l_notice = getView("noticeLabel") as UILabel;
		UILabel l_titleBirth = getView("verifyBirthButton").getView("titleText") as UILabel;
		UILabel l_feature = getView("featureText") as UILabel;
		UILabel l_buy = getView("buyText") as UILabel;
		
		l_title.text = Localization.getString( Localization.TXT_44_LABEL_TITLE );
		l_titleLock.text = Localization.getString( Localization.TXT_44_LABEL_TITLE_LOCK );
		l_notice.text = Localization.getString( Localization.TXT_44_LABEL_NOTICE );
		l_titleBirth.text = Localization.getString( Localization.TXT_44_LABEL_TITLE_BIRTH );
		l_feature.text = Localization.getString( Localization.TXT_44_LABEL_FEATURE );
		l_buy.text = Localization.getString( Localization.TXT_44_LABEL_BUY );
	}

	private UIElement m_dialog;
	private int m_dialogMovePosition;
}