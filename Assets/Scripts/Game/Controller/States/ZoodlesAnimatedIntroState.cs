﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ZoodlesAnimatedIntroState : GameState 
{
	private const float ANIMATION_SOUND_TRIGGER_PERCENTAGE = 0.4f;
	private const float WAIT_TIME_ON_FINISH = 200f;
	private const float WAIT_TIME_ON_LAODING_FINISH = 1000f;
	private const float ANIMATION_PIXEL_WIDTH = 1160;
	private const float ANIMATION_PIXEL_HEIGHT = 640;

	private GameObject gameLogic;
	private GameObject loadingPage;
	private GameObject l_animationObject;

	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);

		gameLogic = GameObject.FindWithTag("GameController");

		m_isFinished = false;
		m_hasPlayedSound = false;
		m_timeFinished = 0;
		readSetting ();
		_setupScreen(p_gameController.getUI());
		
		GAUtil.logScreen("ZoodlesIntroScreen");

//		Game game = gameLogic.GetComponent<Game>();	
	}
	
	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);		
		
		updateAnimationProgress(p_time);
		evaluateTransitions(p_gameController);
		evaluateSounds(p_gameController);
	}
	
	public override void exit(GameController p_gameController)
	{
		base.exit(p_gameController);	
		
		UIManager l_ui = p_gameController.getUI();
		l_ui.removeScreen(m_animationCanvas);
		m_animationCanvas = null;
		m_clip = null;
	}
	
	
	//---------------- Private Implementation ----------------------
	
	private void readSetting()
	{

		SessionHandler.getInstance().masterVolum = PlayerPrefs.GetInt("master_volume",50);
		SessionHandler.getInstance().musicVolum = PlayerPrefs.GetInt("music_volume",50);
		SessionHandler.getInstance().effectsVolum = PlayerPrefs.GetInt("effects_volume",50);
		
		SoundManager.getInstance ().effectVolume = (float)SessionHandler.getInstance ().effectsVolum/100;
		SoundManager.getInstance ().musicVolume = (float)SessionHandler.getInstance ().musicVolum/100;
		SoundManager.getInstance ().masterVolume = (float)SessionHandler.getInstance ().masterVolum/100;
	}
	
	private void _setupScreen(UIManager p_uiManager)
	{
		m_animationCanvas = p_uiManager.createScreen (UIScreen.ZOODLES_INTRO, false, 1);
		UIElement l_animationElement = m_animationCanvas.getView ("ZoodlesAnimation") as UIElement;
		loadingPage = GameObject.FindWithTag ("LoadingEntrance");
		l_animationObject = l_animationElement.gameObject;
		m_clip = l_animationObject.GetComponent<GAFMovieClip> ();
		Game game = gameLogic.GetComponent<Game>();	
		if (game.IsFirstLaunch == 0) {
			l_animationObject.SetActive(true);
			loadingPage.SetActive(false);
		} 
		else {
			l_animationObject.SetActive(false);
			loadingPage.SetActive(true);
		}
	}
	
	private void updateAnimationProgress(float p_time)
	{
		Game game = gameLogic.GetComponent<Game>();	
		if (game.IsFirstLaunch == 0) {
			m_animationPercentage = m_clip.getCurrentFrameNumber() / (float)m_clip.getFramesCount();
		} else {
			m_animationPercentage = 1;
		}

		if (m_animationPercentage >= 1)
		{
			m_timeFinished += p_time;
		}
	}

	bool isRequest = false;
	private void evaluateTransitions(GameController p_gameController)
	{
		Game game = gameLogic.GetComponent<Game>();
		if (game.IsFirstLaunch == 0) {
			if ((false == m_isFinished && m_timeFinished > WAIT_TIME_ON_FINISH)) {

				l_animationObject.SetActive(false);
				loadingPage.SetActive(true);
				game.clientIdAndPremiumRequests(onRequestsCompleted);

//				p_gameController.changeState (ZoodleState.INITIALIZE_GAME);
				m_isFinished = true;
			}
		} else {
			if ((false == m_isFinished && m_timeFinished > WAIT_TIME_ON_LAODING_FINISH)) {
				if(game.IsReLaunch == 1) {
					if(SessionHandler.LoadCurrentKid() != -1) {
						if (game.user.contentCache.isFinishedLoadingWebContent) {
							p_gameController.changeState(ZoodleState.REGION_LANDING);
							m_isFinished = true;
						}
						
						if(!isRequest) {
							isRequest = true;
							KidModeLockController.Instance.swith2KidMode();
							SessionHandler.getInstance().resetKidCache();
							//			m_loadingLabel.tweener.addAlphaTrack( 1.0f, 0.0f, 0.5f, onLoadingFadeOutFinish );

							game.clientIdAndPremiumRequests(toDoActivityRequest);
						}
					}
				}
				//normal launch
				else {
					//has kids
					if(SessionHandler.LoadCurrentKid() != -1) {
						if(SessionHandler.getInstance().currentKid != null) {
							if (game.user.contentCache.isFinishedLoadingWebContent) {
								p_gameController.changeState(ZoodleState.REGION_LANDING);
								m_isFinished = true;
							}
							
							if(!isRequest) {
								isRequest = true;
								KidModeLockController.Instance.swith2KidMode();
								SessionHandler.getInstance().resetKidCache();
								//			m_loadingLabel.tweener.addAlphaTrack( 1.0f, 0.0f, 0.5f, onLoadingFadeOutFinish );

								game.clientIdAndPremiumRequests(toDoActivityRequest);
							}
						}
						//honda: it should not enter here
						else {
							game.clientIdAndPremiumRequests(onRequestsCompleted);
							m_isFinished = true;
						}

					}
					//honda: currently, if have user account but no kid list, enter here
					else {
						game.clientIdAndPremiumRequests(onRequestsCompleted);
						m_isFinished = true;
					}
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
		Game game = gameLogic.GetComponent<Game>();
		//honda: videos, games and books lists request
		game.user.contentCache.startRequests();
		SessionHandler.getInstance().getAllKidApplist();
	}
	
	private void evaluateSounds(GameController p_gameController)
	{
		if (false == m_hasPlayedSound 
		    && m_animationPercentage > ANIMATION_SOUND_TRIGGER_PERCENTAGE)
		{
			SoundManager.getInstance().play("g_zoodles");
			m_hasPlayedSound = true;
		}
	}
	
	
	public void setFinished(bool isFinish) {
		m_isFinished = isFinish;
	}
	//Private variables
	private bool m_isFinished;	
	private bool m_hasPlayedSound;
	private float m_animationPercentage;
	private float m_timeFinished;
	
	private UICanvas m_animationCanvas;
	private GAFMovieClip m_clip;
}
