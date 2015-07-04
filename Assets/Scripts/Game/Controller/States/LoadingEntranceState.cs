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

		m_triggerNextScreen = false;

		//honda: kid photo, app list and top recommended app request
		RequestQueue l_request = new RequestQueue ();
		l_request.add (new GetKidRequest(SessionHandler.getInstance().currentKid.id, onRequestComplete));
		l_request.request ();

		m_loadingLabel.tweener.addAlphaTrack( 1.0f, 0.0f, 0.5f, onLoadingFadeOutFinish );

		//honda: videos, games and books lists request
		l_game.user.contentCache.startRequests();
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
	
	private void onRequestComplete(WWW p_response)
	{
		if (p_response.error != null)
			m_gameController.changeState(ZoodleState.SERVER_ERROR);
		else
		{
			string l_string = "";
			
			l_string = UnicodeDecoder.Unicode(p_response.text);
			l_string = UnicodeDecoder.UnicodeToChinese(l_string);
			l_string = UnicodeDecoder.CoverHtmlLabel(l_string);

			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(l_string) as Hashtable;
			Kid l_currentKid = new Kid(l_data);
			l_currentKid.requestPhoto();
			
			SessionHandler.getInstance().currentKid = l_currentKid;
			
			List<Kid> l_kidList = SessionHandler.getInstance().kidList;
			for (int i = 0; i < l_kidList.Count; ++i)
			{
				if (l_kidList[i].id == l_currentKid.id)
				{
					if(null != l_kidList[i].appList)
						l_currentKid.appList = l_kidList[i].appList;
					if(null != l_kidList[i].topRecommendedApp)
						l_currentKid.topRecommendedApp = l_kidList[i].topRecommendedApp;
					l_kidList[i] = l_currentKid;
					break;
				}
			}
		}

		m_triggerNextScreen = true;
	}

	//Private variables
	private bool m_triggerNextScreen = false;

	private UICanvas	m_backCanvas;
	private UICanvas	m_loadingCanvas;
	private UILabel		m_loadingLabel;
}
