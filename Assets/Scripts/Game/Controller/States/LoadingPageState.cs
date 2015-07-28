using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadingPageState : GameState {
	
	private Game game;
	private bool isRequest;
	private UICanvas loadingCanvas;
	private UILabel loadingLabel;
	private float currentTimer;
	private float timer;
	private bool _isTimerFinished;
	private bool isLoadingFinished;
	private int savedState;

	private bool isTimerFinished
	{
		get {return _isTimerFinished;}
		set
		{
			bool oldValue = _isTimerFinished;
			_isTimerFinished = value;
			if (onTimerFinished != null)
				onTimerFinished(_isTimerFinished);
		}
	}

	private delegate void onTimerFinishedEvent(bool value);
	private event onTimerFinishedEvent onTimerFinished;

	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );

		game = p_gameController.game;
		timer = 300;
		currentTimer = 0;
		isTimerFinished = false;
		isLoadingFinished = false;
		savedState = ZoodleState.NO_STATE;
		onTimerFinished += onTimerCompleted;

		setupScreen(p_gameController);
	}

	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );

		p_gameController.getUI().removeScreenImmediately(loadingCanvas);
	}

	public override void update( GameController p_gameController, int p_time )
	{
		currentTimer += p_time;
		if (currentTimer >= timer && isTimerFinished == false)
		{
			isTimerFinished = true;
		}
	}
	
	private void setupScreen(GameController p_gameController)
	{
		loadingCanvas = p_gameController.getUI().createScreen( UIScreen.LOADING_PAGE, true, 1 );
//		loadingCanvas.enterTransitionEvent += onTransitionEnter;
//		loadingCanvas.exitTransitionEvent += onTransitionExit;

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
		if (isTimerFinished == true)
		{
			onTimerFinished -= onTimerCompleted;
			isLoadingFinished = true;
			m_gameController.changeState(ZoodleState.INITIALIZE_GAME);
		}
		else
		{
			isLoadingFinished = true;
			savedState = ZoodleState.INITIALIZE_GAME;
		}
	}

	private void toDoActivityRequest(bool isCompleted)
	{
		SessionHandler.getInstance().resetKidCache();

		game.user.contentCache.startRequests(onLoadingCompleted);
		SessionHandler.getInstance().getAllKidApplist();
	}

	private void onLoadingCompleted()
	{
		if (isTimerFinished == true)
		{
			onTimerFinished -= onTimerCompleted;
			isLoadingFinished = true;
			m_gameController.changeState(ZoodleState.REGION_LANDING);
		}
		else
		{
			isLoadingFinished = true;
			savedState = ZoodleState.REGION_LANDING;
		}
	}

	private void onTransitionEnter( UICanvas p_canvas )
	{
		p_canvas.graphicRaycaster.enabled = false;
	}
	
	private void onTransitionExit( UICanvas p_canvas )
	{
		p_canvas.graphicRaycaster.enabled = true;
	}

	private void onTimerCompleted(bool value)
	{
		if (value == true)
		{
			if (savedState != ZoodleState.NO_STATE && isLoadingFinished == true)
			{
				onTimerFinished -= onTimerCompleted;
				m_gameController.changeState(savedState);
			}
			else if (isLoadingFinished == false)
			{
				Debug.Log("still loading data from server");
			}
			else if (savedState == ZoodleState.NO_STATE && isLoadingFinished == true)
			{
				DebugUtils.Assert(savedState != ZoodleState.NO_STATE);
			}
		}
	}
}
