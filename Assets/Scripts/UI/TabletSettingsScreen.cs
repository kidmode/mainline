﻿using UnityEngine;
using System.Collections;

public class TabletSettingsScreen : MonoBehaviour {

	[SerializeField]
	private GameObject gameLogic;

	private UICanvas m_TabletSettingsCanvas;

	// Use this for initialization
	void Start () {

		gameLogic = GameObject.Find ("GameLogic");

		GameController gameController = gameLogic.GetComponent<Game>().gameController;

		UIManager l_ui = gameController.getUI();

		m_TabletSettingsCanvas = l_ui.findScreen(UIScreen.TABLET_SETTINGS) as UICanvas;

		SetupLocalization();
	
	}



	//=========================
	//Add localisation here

	private void SetupLocalization()
	{
		UILabel l_tabletSettingsTitle    = m_TabletSettingsCanvas.getView("textTabletSettingsTitle") as UILabel;
		UILabel l_settingsLabel    = m_TabletSettingsCanvas.getView("textButtonSettings") as UILabel;
		UILabel l_exitLabel    = m_TabletSettingsCanvas.getView("textButtonExit") as UILabel;
		UILabel l_wifiLabel      = m_TabletSettingsCanvas.getView("textButtonWifi") as UILabel;
		UILabel l_googleplayLabel      = m_TabletSettingsCanvas.getView("textButtonGooglePlay") as UILabel;
		
		l_tabletSettingsTitle.text       = Localization.getString(Localization.TXT_SCREEN_1001_TABLET_SETTINGS_TITLE);
		l_settingsLabel.text       = Localization.getString(Localization.TXT_SCREEN_1001_TABLET_SETTINGS_SETTINGS);
		l_exitLabel.text       = Localization.getString(Localization.TXT_SCREEN_1001_TABLET_SETTINGS_EXIT);
		l_wifiLabel.text       = Localization.getString(Localization.TXT_SCREEN_1001_TABLET_SETTINGS_WIFI);
		l_googleplayLabel.text       = Localization.getString(Localization.TXT_SCREEN_1001_TABLET_SETTINGS_GOOGLE);
	}


	
	// Update is called once per frame
	void Update () {
	
	}

	public void buttonSettings(){

		KidMode.openSettings ();

	}

	public void buttonHome(){


		KidMode.openDefaultLauncher ();
		
	}

	public void buttonExit(){

		KidModeLockController.Instance.swith2DefaultLauncher ();

		KidMode.openDefaultLauncher ();

	}

	public void openDefaultLauncher(){

		KidMode.openDefaultLauncher ();

	}

	public void buttonWifi(){

		KidMode.openWifi(false);
		
	}

	public void buttonGooglePlay(){

		KidMode.openGooglePlay ();
		
	}

	public void closeScreen(){

		if (gameLogic != null) {
			
			Game game = gameLogic.GetComponent<Game>();
			
			game.gameController.getUI().removeScreen(UIScreen.TABLET_SETTINGS);
			
		}

	}


	public void closeHomeSettings(){

	}

	public void setLauncherKidmode(){

		KidModeLockController.Instance.swith2KidModeLauncher ();

	}

	public void setLauncherDefault(){

		KidModeLockController.Instance.swith2DefaultLauncher ();

	}



}
