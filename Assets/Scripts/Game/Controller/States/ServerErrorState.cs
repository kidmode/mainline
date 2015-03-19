using System;

public class ServerErrorState : GameState
{
	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);

		m_closed = false;

		UIManager l_ui = p_gameController.getUI();
		UICanvas l_screen = l_ui.createScreen(UIScreen.CONNECTION_ERROR, false, 10);
		l_ui.changeScreen(l_screen, true);
		UIButton l_closeBtn = l_screen.getView("close") as UIButton;
		l_closeBtn.addClickCallback(_onCloseErrorScreen);
	}

	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		if (m_closed)
		{
			if (SessionHandler.getInstance().token.isExist())
				p_gameController.changeState(ZoodleState.SIGN_IN_CACHE);
			else
				p_gameController.changeState(ZoodleState.CREATE_ACCOUNT_SELECTION);
		}
	}

	public override void exit(GameController p_gameController)
	{
		p_gameController.getUI().removeScreenImmediately(UIScreen.CONNECTION_ERROR);

		base.exit(p_gameController);
	}

	private void _onCloseErrorScreen(UIButton p_button)
	{
		m_closed = true;
	}

	private bool m_closed = false;
}

