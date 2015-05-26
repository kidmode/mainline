using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CancelLockConfirmCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );

		m_dialog = getView ("mainPanel") as UIElement;
		m_dialogMovePosition = 800;
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_title = getView ("titleArea").getView ("Text") as UILabel;
		l_title.text = Localization.getString (Localization.TXT_90_LABEEL_TITLE);
		UILabel l_message = getView ("contentArea").getView ("messageText") as UILabel;
		l_message.text = Localization.getString (Localization.TXT_90_LABEEL_MESSAGE_TEXT);
		UILabel l_cancel = getView ("cancelButton").getView ("Text") as UILabel;
		l_cancel.text = Localization.getString (Localization.TXT_90_BUTTON_CANCEL);
		UILabel l_turnOff = getView ("turnOffButton").getView ("Text") as UILabel;
		l_turnOff.text = Localization.getString (Localization.TXT_90_BUTTON_TURN_OFF);
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
		l_pointListOut.Add( m_dialog.transform.localPosition + new Vector3( 0, m_dialogMovePosition, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	public void setOutPosition()
	{
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_dialog.transform.localPosition );
		l_pointListOut.Add( m_dialog.transform.localPosition - new Vector3( 0, m_dialogMovePosition, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f );
	}
	
	//-- Private Implementation --
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private UIElement m_dialog;
	private int m_dialogMovePosition;
}
