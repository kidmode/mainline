using UnityEngine;
using System.Collections;

public class DeviceOptionsCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		m_sliderArea = getView ("sliderArea");
		m_allowIncomingCall = getView ("smartSelect");
		m_displayHelpFulTips = getView ("addApp");
		m_allowIncomingCall.active = false;
		m_displayHelpFulTips.active = false;
		m_sliderArea.active = false;
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
		tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
	}

	//-- Private Implementation --
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private void onShowFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		m_sliderArea.active = true;
		m_allowIncomingCall.active = true;
		m_displayHelpFulTips.active = true;
	}

	private UIElement m_sliderArea;
	private UIElement m_allowIncomingCall;
	private UIElement m_displayHelpFulTips;

}