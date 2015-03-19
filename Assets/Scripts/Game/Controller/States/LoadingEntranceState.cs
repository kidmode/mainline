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
      
		KidMode.setKidsModeActive(true);

		m_loadingLabel.tweener.addAlphaTrack( 1.0f, 0.0f, 0.5f, onLoadingFadeOutFinish );
		l_game.StartCoroutine( _tweenFillBar( 1.0f, 1.25f ) );
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );		
		
		if (m_triggerNextScreen)
		{
			p_gameController.changeState( ZoodleState.MAP );

			const float COMPLETE_TIME = 0.5f;

			m_backCanvas.tweener.addAlphaTrack(     1.0f, 0.0f, COMPLETE_TIME, onFadeFinish );
			m_loadingCanvas.tweener.addAlphaTrack(  1.0f, 0.0f, COMPLETE_TIME, onFadeFinish );

			m_triggerNextScreen = false;
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
	private void onNextClicked( UIButton p_button )
	{
		m_triggerNextScreen = true;
	}	

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
	
	private IEnumerator _tweenFillBar( float p_filledAmount, float p_duration )
	{
		float l_time = 0;
		while( l_time < p_duration )
		{
			//float l_fillAmount = Mathf.Lerp( 0, p_filledAmount, l_time / p_duration );
			
			l_time += Time.deltaTime;
			
			yield return new WaitForEndOfFrame();
		}

		m_triggerNextScreen = true;
		yield return null;
	}


	//Private variables
	private bool m_triggerNextScreen = false;

	private UICanvas	m_backCanvas;
	private UICanvas	m_loadingCanvas;
	private UILabel		m_loadingLabel;
}
