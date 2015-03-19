using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PayConfirmState : GameState 
{
	//Public variables

	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );
		
		m_game = p_gameController.game;

		_setupScreen( p_gameController.getUI() );

	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_payConfirmCanvas );
	}

	//---------------- Private Implementation ----------------------
	
	private void _setupScreen( UIManager p_uiManager )
	{
		m_payConfirmCanvas = p_uiManager.createScreen( UIScreen.PAY_CONFIRM, true, 1 );

		m_backButton = m_payConfirmCanvas.getView ("backButton") as UIButton;
		m_backButton.addClickCallback (goBack);
	}

	private void goBack( UIButton p_button )
	{
		m_game.gameController.changeState (ZoodleState.PROFILE_SELECTION);
	}


	//Private variables
	
	private UICanvas    m_payConfirmCanvas;

	private UIButton 	m_backButton;

	private Game 		m_game;
}
