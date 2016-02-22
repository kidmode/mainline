using System.Collections;

public class PayGemsConfirmState : GameState
{
	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);
		
		_setupScreen(p_gameController.getUI());
	}
	
	public override void exit(GameController p_gameController)
	{
		p_gameController.getUI().removeScreenImmediately(UIScreen.PAY_GEMS_COMFIRM);

		base.exit(p_gameController);
	}
	
	//---------------- Private Implementation ----------------------
	
	private void _setupScreen( UIManager p_uiManager )
	{
		string l_gemAmount = m_gameController.board.read("gems") as string;

		UICanvas l_payConfirmCanvas = p_uiManager.createScreen(UIScreen.PAY_GEMS_COMFIRM, true, 1);

		UILabel l_congratsLabel = l_payConfirmCanvas.getView("congraturationLabel") as UILabel;
		l_congratsLabel.text = Localization.getString(Localization.TXT_66_LABEL_TOP);

		UILabel l_gemsText = l_payConfirmCanvas.getView("gemsText") as UILabel;
		l_gemsText.text = l_gemAmount;

		ArrayList l_replaces = new ArrayList();
		l_replaces.Add(l_gemAmount);
		UILabel l_noticeText = l_payConfirmCanvas.getView("noticeText") as UILabel;
		l_noticeText.text = Localization.getString(Localization.TXT_STATE_20_GEMS_NOTICE, l_replaces);;
		
		UIButton l_backButton = l_payConfirmCanvas.getView("backButton") as UIButton;
		l_backButton.addClickCallback(_goBack);
	}
	
	private void _goBack(UIButton p_button)
	{
		int state = m_gameController.getConnectedState(ZoodleState.PAY_GEMS_CONFIRM);
		m_gameController.changeState(state);
		m_gameController.game.setPDMenuBarVisible(true, false);
	}
}

