using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class DashBoardControllerCanvas : UICanvas
{
	public override void init( GameObject p_gameObject )
	{
		base.init (p_gameObject);
		m_dotList = new List<UIElement>();
		m_currentIndex = 0;

		m_leftButton = getView( "leftButton" ) as UIButton;
		m_rightButton = getView( "rightButton" ) as UIButton;

		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );

		m_leftButton.addClickCallback( onLeftButtonClick );
		m_rightButton.addClickCallback( onRightButtonClick );
	}

	public override void update()
	{
		base.update();
	}

	public override void dispose( bool p_deep )
	{
		m_dotList = null;
		base.dispose( p_deep );
	}

	public override void enteringTransition()
	{
		base.enteringTransition();
		tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
	}

	public void onLeftButtonClick( UIButton p_button )
	{
		m_currentIndex--;

		checkButton();
		setDot();
	}

	public void onRightButtonClick( UIButton p_button )
	{
		m_currentIndex++;
		
		checkButton();
		setDot();
	}

	public void setCurrentIndex( int p_index )
	{
		m_currentIndex = p_index;
		checkButton();
		setDot();
	}

	public void setupDotList(int p_dotCount)
	{
		GameObject l_prototype = GameObject.Find( "dotPrototype" );
		GameObject l_obj;
		for( int i = 0; i < p_dotCount; i++)
		{
			l_obj = GameObject.Instantiate( l_prototype ) as GameObject;
			l_obj.transform.SetParent( l_prototype.transform.parent );
			l_obj.transform.localScale = new Vector3 ( 1, 1, 1 );

			UIButton l_btn = attachToGameObject( l_obj ) as UIButton;
			m_dotList.Add( l_btn );
		}

		m_currentIndex = 0;
		if (m_dotList.Count == 0)
			return;

		m_currentDot = m_dotList[m_currentIndex] as UIButton;

		l_prototype.SetActive( false );

		checkButton();
		setDot();
	}
	//------------------ Private Implementation --------------------
	
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private void checkButton()
	{
		if( null != m_dotList )
		{
			if( m_currentIndex >= m_dotList.Count - 1 )
			{
				m_currentIndex = m_dotList.Count - 1;
				m_leftButton.enabled = true;
				m_rightButton.enabled = false;
			}
			else if( m_currentIndex <= 0 )
			{
				m_currentIndex = 0;
				m_leftButton.enabled = false;
				m_rightButton.enabled = true;
			}
			else
			{
				m_leftButton.enabled = true;
				m_rightButton.enabled = true;
			}
		}
		else
		{
			m_leftButton.enabled = false;
			m_rightButton.enabled = false;
		}
	}

	private void setDot()
	{
		if( 0 < m_dotList.Count )
		{
			m_currentDot.enabled = true;
			m_currentDot = m_dotList[m_currentIndex] as UIButton;
			m_currentDot.enabled = false;
		}
	}

	private UIButton m_leftButton;
	private UIButton m_rightButton;

	private List<UIElement> m_dotList = new List<UIElement>();

	private int m_currentIndex;
	private UIButton m_currentDot;
}
