using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignOutConfirmCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		SetupLocalizition ();
		
		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );

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
		UILabel l_title = getView("dialogTitle").getView("Text") as UILabel;
		UILabel l_content = getView("dialogContent").getView("Text") as UILabel;
		UILabel l_no = getView("cancelButton").getView("cancelBtnText") as UILabel;
		UILabel l_yes = getView("affirmButton").getView("affirmBtnText") as UILabel;
		
		l_title.text = Localization.getString( Localization.TXT_40_LABEL_TITLE );
		l_content.text = Localization.getString( Localization.TXT_40_LABEL_CONTENT );
		l_no.text = Localization.getString( Localization.TXT_40_LABEL_NO );
		l_yes.text = Localization.getString( Localization.TXT_40_LABEL_YES );
	}

	private UIElement m_dialog;
	private int m_dialogMovePosition;
}
