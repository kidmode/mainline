using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PanelMarkingScreenCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
//		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );

		m_title1 = getView ("title1") as UILabel;
		m_title2 = getView ("title2") as UILabel;
		m_title3 = getView ("title3") as UILabel;
		m_title2.active = false;
		m_title3.active = false;

		m_toggle1 = getView ("Toggle1") as UIToggle;
		m_toggle2 = getView ("Toggle2") as UIToggle;
		m_toggle3 = getView ("Toggle3") as UIToggle;

		m_threePanel = gameObject.GetComponentInChildren<ScrollRect> ();
		m_threePanel.onValueChanged.AddListener( onChange );
		setupLocalization ();
	}
	

	public void setupLocalization()
	{
		UILabel l_title1 = getView ("title1") as UILabel;
		l_title1.text = Localization.getString (Localization.TXT_101_LABEL_PANEL_1_TITLE);
		UILabel l_title2 = getView ("title2") as UILabel;
		l_title2.text = Localization.getString (Localization.TXT_101_LABEL_PANEL_2_TITLE);
		UILabel l_title3 = getView ("title3") as UILabel;
		l_title3.text = Localization.getString (Localization.TXT_101_LABEL_PANEL_3_TITLE);
		
		UILabel l_starNow = getView ("startButton").getView("Text") as UILabel;
		l_starNow.text = Localization.getString (Localization.TXT_101_BUTTON_START_NOW);
		
		UILabel l_signin = getView ("signInButton").getView("Text") as UILabel;
		l_signin.text = Localization.getString (Localization.TXT_101_BUTTON_SIGN_IN);
		
		UILabel l_exit = getView ("exitButton").getView("Text") as UILabel;
		l_exit.text = Localization.getString (Localization.TXT_BUTTON_QUIT);
	}
	
	public override void update()
	{
		base.update();
	}
	
	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}

	private void onChange(Vector2 p_value)
	{
		float l_panelXValue = p_value.x;
		if(l_panelXValue <= 0.35 && !m_title1.active)
		{
			m_title1.active = true;
			m_title2.active = false;
			m_toggle1.isOn = true;
		}
		else if (l_panelXValue > 0.35 && l_panelXValue < 0.85 && !m_title2.active)
		{
			m_title1.active = false;
			m_title2.active = true;
			m_title3.active = false;
			m_toggle2.isOn = true;
		}
		else if(l_panelXValue >= 0.85 && !m_title3.active)
		{
			m_title2.active = false;
			m_title3.active = true;
			m_toggle3.isOn = true;
		}
//		if(l_panelXValue <= 0.35)
//		{
//			m_title1.text = Localization.getString (Localization.TXT_101_LABEL_PANEL_3_TITLE);
//			m_toggle1.isOn = true;
//		}
//		else if (l_panelXValue > 0.35 && l_panelXValue < 0.85)
//		{
//			m_title1.text = Localization.getString (Localization.TXT_101_LABEL_PANEL_2_TITLE);
//			m_toggle2.isOn = true;
//		}
//		else if(l_panelXValue >= 0.85)
//		{
//			m_title1.text = Localization.getString (Localization.TXT_101_LABEL_PANEL_3_TITLE);
//			m_toggle3.isOn = true;
//		}
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

	private ScrollRect m_threePanel;
	private int m_dialogMovePosition;
	private UILabel m_title1;
	private UILabel m_title2;
	private UILabel m_title3;
	private UIToggle m_toggle1;
	private UIToggle m_toggle2;
	private UIToggle m_toggle3;
}
