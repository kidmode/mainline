using UnityEngine;
using System.Collections;
//using System;

//===================================================
//Kevin
//Buttons Events are handeled across many any  state classes . .. .  want to do new button actions here to save time and less complexity.

public class DashboardCommonScreen : MonoBehaviour {

//	[SerializeField]
	private Game game;

	// Use this for initialization
	void Start () 
	{
		updateToggleButtons ();
		GameObject gameLogic = GameObject.FindWithTag("GameController");
		game = gameLogic.GetComponent<Game>();
	}

	void updateToggleButtons()
	{
		Debug.Log ("  updateToggleButtons  " + KidModeLockController.Instance.stateHomeLauncher.ToString() );
		return;

//		if (KidModeLockController.Instance.stateHomeLauncher == KidModeLockController.StateHomeLauncher.Default) {
//
//			swith2DefaultLauncherButton.SetActive(false);
//
//			swith2KidModeLauncherButton.SetActive(true);
//
//		}else if (KidModeLockController.Instance.stateHomeLauncher == KidModeLockController.StateHomeLauncher.KidMode) {
//
//			swith2DefaultLauncherButton.SetActive(true);
//			
//			swith2KidModeLauncherButton.SetActive(false);
//
//			
//		}

	}

	public void switchToDefaultLauncher()
	{
		KidModeLockController.Instance.swith2DefaultLauncher ();
		updateToggleButtons ();
	}

	public void switchToKidModeLauncher()
	{
		KidModeLockController.Instance.swith2KidModeLauncher ();
		updateToggleButtons ();

//		PlayerPrefs.SetString ("settingsLauncher", KidModeLockController.Instance.stateHomeLauncher.ToString());
	}

	public void openTabletSettings()
	{
		if (game != null) 
		{
			game.gameController.getUI().createScreen(UIScreen.TABLET_SETTINGS, false, 6);
		}
	}
}
