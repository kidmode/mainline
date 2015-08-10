﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TimerController : MonoBehaviour {

	//static 
	private static TimerController _instance;
	public static TimerController Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = GameObject.FindObjectOfType<TimerController>();
				
				//Tell unity not to destroy this object when loading a new scene!
				DontDestroyOnLoad(_instance.gameObject);
			}
			
			return _instance;
		}
	}
	//normal
	public bool isRunning
	{
		get
		{
			if (timer.Enabled == true)
				return true;
			else
				return false;
		}
	}

	[SerializeField]
	private Game game;

	private System.Timers.Timer timer;
	//format: second
	private float countdownTime;
	private float timeLeft;
	private int kid_id;
	private bool isTimesUp;
	private Text m_text;

	void Awake()
	{
		if(_instance == null)
		{
			//If I am the first instance, make me the Singleton
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			//If a Singleton already exists and you find
			//another reference in scene, destroy it!
			if(_instance != this)
				Destroy(this.gameObject);
		}
	}
	
	void Start () 
	{
		UIManager uiManager = game.gameController.getUI();
		m_text = GameObject.FindGameObjectWithTag("TimerUI").GetComponent<Text>();

		isTimesUp = false;
		countdownTime = 0;
		timeLeft = countdownTime;
		//Initialize timer with 1 second intervals
		timer = new System.Timers.Timer (1000);
		timer.Elapsed += timeElapsed;

		m_text.text = "time left";
	}

	void Update()
	{
		if (isTimesUp)
		{
			isTimesUp = false;
			m_text.text = "time's up"; 

			if (game.isNotPlayingNativeWebView)
			{
				createTimesUpScreen();
			}
			else
			{
				//honda
				//TODO: froce to stop native webview
				KidMode.closeNativeWebview();
				createTimesUpScreen();
			}
		}
		else
		{
			if (timer.Enabled)
			{
				m_text.text = timeLeft.ToString() + " seconds left"; 
			}
//			else
//			{
//				m_text.text = "";
//			}
		}
	}

	public void startTimer()
	{
		if (countdownTime <= 0 )
			return;

		if (timer.Enabled == false)
		{
			timer.Start();
			Debug.Log("Countdown Timer starts: " + timeLeft);
			m_text.text = timeLeft.ToString() + " seconds left(start)"; 
		}
		else
		{
			Debug.Log("Countdown Timer is running");
		}
	}

	public void stopTimer()
	{
		if (countdownTime <= 0)
			return;

		if (timer.Enabled == true)
		{
			timer.Stop();

			timeLeft = (timeLeft < 0)?0:timeLeft;
			SessionHandler.getInstance().currentKid.updateAndSaveTimeLeft(timeLeft, isTimesUp);

			Debug.Log("Countdown Timer stops: " + timeLeft);
			timeLeft = countdownTime;

			m_text.text = timeLeft.ToString() + " seconds left(stop)"; 
		}
		else
		{
			Debug.Log("Countdown Timer is already down");
		}
	}

	public void pauseTimer()
	{
		if (countdownTime <= 0)
			return;

		if (timer.Enabled == true)
		{
			timer.Stop();
			SessionHandler.getInstance().currentKid.updateAndSaveTimeLeft(timeLeft, isTimesUp);

			Debug.Log("Countdown Timer pauses: " + timeLeft);

			m_text.text = timeLeft.ToString() + " seconds left(pause)";
		}
		else
		{
			Debug.Log("Countdown Timer has paused");
		}
	}

	public void resumeTimer()
	{
		if (countdownTime <= 0)
			return;

		if (timer.Enabled == false)
		{
			timer.Start();
			Debug.Log("Countdown Timer resumes: " + timeLeft);

			m_text.text = timeLeft.ToString() + " seconds left(resume)";
		}
		else
		{
			Debug.Log("Countdown Timer is running");
		}
	}

	private void timeElapsed(object sender, System.Timers.ElapsedEventArgs e)
	{
		timeLeft--; 
		Debug.Log("Countdown Timer: " + timeLeft);
		if (timeLeft <= 0)
		{
			isTimesUp = true;
			stopTimer();
		}
	}
	
	public void setKidTimer(int kidId, float timelimit, float timeleft)
	{
		if (timer.Enabled == true)
			return;

		kid_id = kidId;
		countdownTime = timelimit;
		timeLeft = 30;//timeleft;
	}

	public void resetKidTimer()
	{
		stopTimer();
		kid_id = -1;
		countdownTime = 0;
		timeLeft = countdownTime;
	}

	public void createTimesUpScreen()
	{
		game.gameController.getUI().createScreen(UIScreen.TIMES_UP, false, 20);
	}

	public void removeTimesUpScreen()
	{
		GameController gameController = game.gameController;

		gameController.getUI().removeScreenImmediately(UIScreen.TIMES_UP);

		UIManager l_ui = gameController.getUI();
		UICanvas l_backScreen = l_ui.findScreen(UIScreen.SPLASH_BACKGROUND);
		if (l_backScreen == null)
		{
			l_backScreen = l_ui.createScreen(UIScreen.SPLASH_BACKGROUND,true, -1);
			SplashBackCanvas l_splashBack = l_backScreen as SplashBackCanvas;
			l_splashBack.setDown();
		}
		//we don't back button because kids have spent whole playing time.
//		gameController.connectState(ZoodleState.BIRTHYEAR, ZoodleState);
		gameController.changeState(ZoodleState.BIRTHYEAR);
	}

}
