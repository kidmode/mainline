using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class LoadingEntranceState : GameState 
{
	//consts
	private const float LOADING_WEIGHT 	= 1;
	private const float LOADING_START 	= 100;
	
	//Public variables
	
	
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );

		Game l_game = p_gameController.game;

		_setupScreen( p_gameController.getUI() );
      
//		KidMode.setKidsModeActive(true);
		KidModeLockController.Instance.swith2KidMode();
		SessionHandler.getInstance().resetKidCache();

		m_counterToNextScreen = 0;

		m_loadingLabel.tweener.addAlphaTrack( 1.0f, 0.0f, 0.5f, onLoadingFadeOutFinish );

		//honda: videos, games and books lists request
		l_game.user.contentCache.startRequests();
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );		

		if (m_counterToNextScreen++ == 5) 
		{
			//first time launch, reboot device(not select kid)
			p_gameController.changeState( ZoodleState.MAP );
			//reboot device/restart(selected kid)
//			p_gameController.changeState( ZoodleState.REGION_LANDING );

			const float COMPLETE_TIME = 0.5f;

			m_backCanvas.tweener.addAlphaTrack(     1.0f, 0.0f, COMPLETE_TIME, onFadeFinish );
			m_loadingCanvas.tweener.addAlphaTrack(  1.0f, 0.0f, COMPLETE_TIME, onFadeFinish );
		}
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );

        p_gameController.getUI().removeScreen( m_backCanvas     );
		p_gameController.getUI().removeScreen( m_loadingCanvas  );
	}


	//---------------- Private Implementation ----------------------
	


	private void _setupScreen( UIManager p_uiManager )
	{
		m_backCanvas = p_uiManager.findScreen( UIScreen.SPLASH_BACKGROUND );
        if (m_backCanvas == null)
        {
            m_backCanvas = p_uiManager.createScreen(UIScreen.SPLASH_BACKGROUND, true, -1);
            (m_backCanvas as SplashBackCanvas).setDown();
        }

		m_loadingCanvas = p_uiManager.createScreen( UIScreen.LOADING_ENTRANCE, true, 1 );
		m_loadingCanvas.enterTransitionEvent    += onTransitionEnter;
		m_loadingCanvas.exitTransitionEvent     += onTransitionExit;

		m_loadingLabel = m_loadingCanvas.getView("loadingLabel") as UILabel;
		m_loadingLabel.text = Localization.getString(Localization.TXT_LABEL_LOADING);
	}


//Listeners
	private void onTransitionEnter( UICanvas p_canvas )
	{
		p_canvas.graphicRaycaster.enabled = false;
	}
	
	private void onTransitionExit( UICanvas p_canvas )
	{
		p_canvas.graphicRaycaster.enabled = true;
	}

	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		UICanvas p_canvas = p_element as UICanvas;
		p_canvas.isTransitioning = false;
	}

	private void onLoadingFadeInFinish( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_loadingLabel.tweener.addAlphaTrack( 1.0f, 0.0f, 0.5f, onLoadingFadeOutFinish );
	}

	private void onLoadingFadeOutFinish( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_loadingLabel.tweener.addAlphaTrack( 0.0f, 1.0f, 0.5f, onLoadingFadeInFinish );
	}

	private int m_counterToNextScreen = 0;

	private UICanvas	m_backCanvas;
	private UICanvas	m_loadingCanvas;
	private UILabel		m_loadingLabel;
}
