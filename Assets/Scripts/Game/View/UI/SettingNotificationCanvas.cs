using UnityEngine;
using System.Collections;

public class SettingNotificationCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		m_notificationPanel = getView ("contentPanel");
		m_notificationPanel.active = false;
		tweener.addAlphaTrack( 0.0f, 1.0f, 0.1f,onShowFinish );
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

	private void onShowFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		m_notificationPanel.active = true;
	}
	
	//-- Private Implementation --
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private UIElement m_notificationPanel;
}