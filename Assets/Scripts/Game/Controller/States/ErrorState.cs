using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class ErrorState : GameState 
{
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );

		m_gameController = p_gameController;

		m_changeState = false;

		SessionHandler.getInstance ().handlePage = "";

		_setupScreen( p_gameController.getUI() );
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );

		if(m_changeState)
		{
			int l_lastState = m_gameController.getConnectedState(ZoodleState.ERROR_STATE);
			if(l_lastState != ZoodleState.NO_STATE)
			{
				m_gameController.changeState(l_lastState);
			}
		}
	}
	
	public override void exit( GameController p_gameController )
	{
		m_gameController = null;
		p_gameController.getUI().removeScreen( m_errorCanvas );
		p_gameController.getUI().removeScreen( m_signInButtonBackgroundCanvas );
		m_errorCanvas = null;
		SessionHandler.getInstance ().errorName = null;
		SessionHandler.getInstance ().errorMessage = null;
		base.exit( p_gameController );	
	}
	
	
	//---------------- Private Implementation ----------------------
	
	
	
	private void _setupScreen( UIManager p_uiManager )
	{
		m_signInButtonBackgroundCanvas = p_uiManager.findScreen(UIScreen.SPLASH_BACKGROUND) as SplashBackCanvas;
		if (m_signInButtonBackgroundCanvas == null)
		{
			m_signInButtonBackgroundCanvas = p_uiManager.createScreen(UIScreen.SPLASH_BACKGROUND, true, -1) as SplashBackCanvas;
			m_signInButtonBackgroundCanvas.setDown();
		}
		m_errorCanvas = p_uiManager.findScreen( UIScreen.ERROR ) as ErrorCanvas;
		if( m_errorCanvas == null )
			m_errorCanvas = p_uiManager.createScreen( UIScreen.ERROR, false, 5 ) as ErrorCanvas;

		SessionHandler l_handler = SessionHandler.getInstance();

		UILabel l_errorName = m_errorCanvas.getView ("errorNameLabel") as UILabel;
		l_errorName.text = l_handler.errorName;

		UILabel l_errorMessage = m_errorCanvas.getView ("errorLabel") as UILabel;
		l_errorMessage.text = l_handler.errorMessage;

		UIImage l_background = m_errorCanvas.getView ("bodyBG") as UIImage;
		l_background.tweener.addAlphaTrack(0, 1, 1.0f, setCallbacks);
	}

	private void setCallbacks(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		UIButton l_quitButton = m_errorCanvas.getView ("quitButton") as UIButton;
		l_quitButton.addClickCallback(onExitClicked);
	}

	private void onExitClicked(UIButton p_button)
	{
		p_button.removeClickCallback(onExitClicked);
		m_changeState = true;
	}
	
	protected override void setErrorMessage(GameController p_gameController, string p_errorName, string p_errorMessage)
	{

		UILabel l_errorName = m_errorCanvas.getView ("errorNameLabel") as UILabel;
		l_errorName.text = p_errorName;
		
		UILabel l_errorMessage = m_errorCanvas.getView ("errorLabel") as UILabel;
		l_errorMessage.text = p_errorMessage;

		SessionHandler l_handler = SessionHandler.getInstance();

		l_handler.errorName 	= p_errorName;
		l_handler.errorMessage 	= p_errorMessage;
	}

	private bool m_changeState;
	private ErrorCanvas m_errorCanvas;
	private SplashBackCanvas	m_signInButtonBackgroundCanvas;
}
