using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GAF.Core;

public class ZoodlesAnimatedIntroState : GameState 
{
	private const float ANIMATION_SOUND_TRIGGER_PERCENTAGE = 0.4f;
	private const float WAIT_TIME_ON_FINISH = 200f;
	private const float WAIT_TIME_ON_LAODING_FINISH = 1000f;
	private const float ANIMATION_PIXEL_WIDTH = 1160;
	private const float ANIMATION_PIXEL_HEIGHT = 640;
	
	private GameObject loadingPage;
	private GameObject l_animationObject;
	private Game game;

	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);

		m_isFinished = false;
		m_hasPlayedSound = false;
		m_timeFinished = 0;
		readSetting ();
		_setupScreen(p_gameController.getUI());
		
		GAUtil.logScreen("ZoodlesIntroScreen");
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
		if (!PlayerPrefs.HasKey("master_volume"))
		{
			PlayerPrefs.SetInt("master_volume", 50);
			PlayerPrefs.SetInt("music_volume", 50);
			PlayerPrefs.SetInt("effects_volume", 50);
			PlayerPrefs.Save();
		}

		SessionHandler.getInstance().masterVolum = PlayerPrefs.GetInt("master_volume");
		SessionHandler.getInstance().musicVolum = PlayerPrefs.GetInt("music_volume");
		SessionHandler.getInstance().effectsVolum = PlayerPrefs.GetInt("effects_volume");

		SoundManager.getInstance ().effectVolume = (float)SessionHandler.getInstance ().effectsVolum/100;
		SoundManager.getInstance ().musicVolume = (float)SessionHandler.getInstance ().musicVolum/100;
		SoundManager.getInstance ().masterVolume = (float)SessionHandler.getInstance ().masterVolum/100;
	
		Debug.Log("Animated PlayerPref has master volume: " + PlayerPrefs.HasKey("master_volume") + " & volume: " + PlayerPrefs.GetInt("master_volume"));
		Debug.Log("Animated PlayerPref has music volume: " + PlayerPrefs.HasKey("music_volume") + " & volume: " + PlayerPrefs.GetInt("music_volume"));
		Debug.Log("Animated PlayerPref has effects volume: " + PlayerPrefs.HasKey("effects_volume") + " & volume: " + PlayerPrefs.GetInt("effects_volume"));
		Debug.Log("Animated SessionHandler master volume: " + SessionHandler.getInstance().masterVolum);
		Debug.Log("Animated SessionHandler music volume: " + SessionHandler.getInstance().musicVolum);
		Debug.Log("Animated SessionHandler effects volume: " + SessionHandler.getInstance().effectsVolum);
		Debug.Log("Animated SoundManager master volume: " + SoundManager.getInstance().masterVolume);
		Debug.Log("Animated SoundManager music volume: " + SoundManager.getInstance().musicVolume);
		Debug.Log("Animated SoundManager effects volume: " + SoundManager.getInstance().effectVolume);
	}
	
	private void _setupScreen(UIManager p_uiManager)
	{
		m_animationCanvas = p_uiManager.createScreen (UIScreen.ZOODLES_INTRO, false, 1);
		UIElement l_animationElement = m_animationCanvas.getView ("ZoodlesAnimation") as UIElement;
		l_animationObject = l_animationElement.gameObject;
		m_clip = l_animationObject.GetComponent<GAFMovieClip> ();
	}
	
	private void updateAnimationProgress(float p_time)
	{
		m_animationPercentage = m_clip.getCurrentFrameNumber() / (float)m_clip.getFramesCount();
		if (m_animationPercentage >= 1)
		{
			m_timeFinished += p_time;
		}
	}

	bool isRequest = false;
	private void evaluateTransitions(GameController p_gameController)
	{
		Game game = p_gameController.game;
		if (game.IsFirstLaunch == 0)
		{
			if ((false == m_isFinished && m_timeFinished > WAIT_TIME_ON_FINISH)) 
			{
				p_gameController.changeState(ZoodleState.LOADING_PAGE);
				m_isFinished = true;
			}
		} 
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
