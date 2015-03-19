using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CancelSubscriptionCanvas : UICanvas 
{
	private const float TRANSITION_DISTANCE = 200;

	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		//tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );

		m_dialog = getView ("dialog") as UIElement;
		m_dialogMovePosition = 578;
		_setupList();
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

	private void _setupList()
	{	
		UISwipeList l_swipe = getView( "resonSelectSwipList" ) as UISwipeList;
		
		List< System.Object > infoData = new List< System.Object >();
		infoData.Add ("Kids outgrown the service");
		infoData.Add ("Device issues");
		infoData.Add ("Software issues");
		infoData.Add ("Insufficient content");
		infoData.Add ("Going to competitor");
		l_swipe.changeContentHeight (640f);
		l_swipe.setData( infoData );
		l_swipe.setDrawFunction( onListDraw );
		l_swipe.redraw();
	}

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UIButton l_button = p_element as UIButton;
		DebugUtils.Assert( l_button != null );
		
		string l_message = p_data as string;
		
		UILabel l_label = l_button.getView( "messageText" ) as UILabel;
		l_label.text = l_message;
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
