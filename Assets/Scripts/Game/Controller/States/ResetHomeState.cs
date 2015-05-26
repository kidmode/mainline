using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ResetHomeState: GameState 
{
	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);
		setupScreen(p_gameController);
	}
	
	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		if (m_began 
		    && KidMode.hasRequestFinished())
		{
			p_gameController.changeState(ZoodleState.CHECK_HOME_BUTTON);
		}
	}
	
	public override void exit(GameController p_gameController)
	{
		base.exit(p_gameController);		
		destroyScreen(p_gameController);
	}


	private void setupScreen(GameController p_gameController)
	{
		UIManager l_ui = p_gameController.getUI();
		m_screen = l_ui.createScreen(UIScreen.HOME_RESET, true, -1);
		m_screen.hasTransition = false;

		UILabel l_explanationText = m_screen.getView("ExplanationText") as UILabel;
		string l_homeName = KidMode.getHomeName();
		l_explanationText.text = string.Format (Localization.getString(Localization.TXT_STATE_26_HOME), l_homeName);

		m_beginButton = m_screen.getView("BeginButton") as UIButton;
		m_beginButton.addClickCallback(begin);

		m_began = false;
	}

	private void destroyScreen(GameController p_gameController)
	{
		UIManager l_ui = p_gameController.getUI();
		l_ui.removeScreen(m_screen);
		m_screen = null;
		m_beginButton = null;
	}	

	private void begin(UIButton p_button)
	{
		if (m_began == false)
		{
			KidMode.clearHomeButton();
			KidMode.makeToast(Localization.getString(Localization.TXT_STATE_26_CLEAR));
			m_began = true;
		}
	}

	private UICanvas m_screen;
	private UIButton m_beginButton;
	private bool m_began;
}
