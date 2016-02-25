


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HelpDialogCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		

		m_dialog = getView ("dialog") as UIElement;
		m_dialogMovePosition = 1236;
	}
	
	public override void update()
	{
		base.update();
	}
	
	public void setUIManager(UIManager p_UIManager)
	{
		m_uiManager = p_UIManager;
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
		if(null != m_uiManager)
		{
			m_uiManager.changeScreen(UIScreen.COMMON_DIALOG,true);
		}
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_dialog.transform.localPosition );
		l_pointListOut.Add( m_dialog.transform.localPosition - new Vector3( 0, m_dialogMovePosition, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f );
	}
	
	public void setOutPosition()
	{
		if(null != m_uiManager)
		{
			m_uiManager.changeScreen(UIScreen.COMMON_DIALOG,false);
		}
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
	
	private UIManager m_uiManager;
	private UIElement m_dialog;
	private int m_dialogMovePosition;
}

