﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CongratsScreen : MonoBehaviour {

//	public Image

	private GameObject gameLogic;


	//===============================
	//Moved the animation from CongratState to screen sprcific
	private UIImage m_loadingBarImg;

	private int m_time;

	// Use this for initialization
	void Start () {

		gameLogic = GameObject.Find ("GameLogic");

		Game game = gameLogic.GetComponent<Game>();
		
		UIManager uiManager = game.gameController.getUI();

		UICanvas congratsScreen = uiManager.findScreen (UIScreen.CONGRATS);

		m_loadingBarImg = congratsScreen.getView("loadingBarSprite") as UIImage;
	
	}
	
	// Update is called once per frame
	void Update () {

		int p_time = (int)(Time.deltaTime * 1000.0f);

		m_time += p_time;
		if (m_time < 1250)
		{
			float l_fillAmount = Mathf.Lerp(0, 1.0f, m_time / 1250.0f);
			m_loadingBarImg.fillAmount = l_fillAmount;
		}
		else
			m_loadingBarImg.fillAmount = 1.0f;

//	
	}

	public void closeCongratsScreen(){

		Game game = gameLogic.GetComponent<Game>();

		UIManager l_ui = game.gameController.getUI();

		l_ui.removeScreen(UIScreen.CONGRATS_BACKGROUND);

		l_ui.removeScreen(UIScreen.CONGRATS);

	}


}