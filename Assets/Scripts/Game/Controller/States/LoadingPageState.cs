using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadingPageState : GameState {
	
	private Game game;
	private bool isRequest;
	private UICanvas loadingCanvas;
	private UILabel loadingLabel;
	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );

		game = p_gameController.game;

		setupScreen(p_gameController);
	}

	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );

		p_gameController.getUI().removeScreenImmediately(loadingCanvas);
	}
	
	private void setupScreen(GameController p_gameController)
	{
		loadingCanvas = p_gameController.getUI().createScreen( UIScreen.LOADING_PAGE, true, 1 );
		loadingCanvas.enterTransitionEvent += onTransitionEnter;
		loadingCanvas.exitTransitionEvent += onTransitionExit;


		loadingLabel = loadingCanvas.getView("loadingLabel") as UILabel;
		loadingLabel.text = Localization.getString(Localization.TXT_LABEL_LOADING);

		doAPICall(p_gameController);
	}

	private void doAPICall(GameController p_gameController)
	{
		if (game.IsFirstLaunch == 0) 
		{
			game.clientIdAndPremiumRequests(onRequestsCompleted);
		} 
		else 
		{
			if(game.IsReLaunch == 1) 
			{
				if(SessionHandler.LoadCurrentKid() != -1) 
				{
//					if (game.user.contentCache.isFinishedLoadingWebContent) 
//					{
//						p_gameController.changeState(ZoodleState.REGION_LANDING);
//					}
					
					if(!isRequest) 
					{
						isRequest = true;
						game.clientIdAndPremiumRequests(toDoActivityRequest);
					}
				}
			}
			//normal launch
			else {
				//has kids
				if(SessionHandler.LoadCurrentKid() != -1) 
				{
					if(SessionHandler.getInstance().currentKid != null) 
					{
//						if (game.user.contentCache.isFinishedLoadingWebContent) 
//						{
//							p_gameController.changeState(ZoodleState.REGION_LANDING);
//						}
						
						if(!isRequest)
						{
							isRequest = true;
							game.clientIdAndPremiumRequests(toDoActivityRequest);
						}
					}
					//honda: it should not enter here
					else 
					{
						game.clientIdAndPremiumRequests(onRequestsCompleted);
					}
				}
				//honda: currently, if have user account but no kid list, enter here
				else 
				{
					game.clientIdAndPremiumRequests(onRequestsCompleted);
				}
			}
		}
	}

	private void onRequestsCompleted(bool isComplated)
	{
		m_gameController.changeState(ZoodleState.INITIALIZE_GAME);
	}

	private void toDoActivityRequest(bool isCompleted)
	{
		SessionHandler.getInstance().resetKidCache();

		game.user.contentCache.startRequests(onLoadingCompleted);
		SessionHandler.getInstance().getAllKidApplist();
	}

	private void onLoadingCompleted()
	{
		m_gameController.changeState(ZoodleState.REGION_LANDING);
	}

	private void onTransitionEnter( UICanvas p_canvas )
	{
		p_canvas.graphicRaycaster.enabled = false;
	}
	
	private void onTransitionExit( UICanvas p_canvas )
	{
		p_canvas.graphicRaycaster.enabled = true;
	}
}
