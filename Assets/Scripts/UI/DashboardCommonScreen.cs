using UnityEngine;
using System.Collections;
//using System;

//===================================================
//Kevin
//Buttons Events are handeled across many any  state classes . .. .  want to do new button actions here to save time and less complexity.

public class DashboardCommonScreen : MonoBehaviour {

	[SerializeField]
	private GameObject gameLogic;

	// Use this for initialization
	void Start () {

		updateToggleButtons ();

		gameLogic = GameObject.FindWithTag("GameController");
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void updateToggleButtons(){

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

	public void switchToDefaultLauncher(){

		KidModeLockController.Instance.swith2DefaultLauncher ();

		updateToggleButtons ();



	}

	public void switchToKidModeLauncher(){

		KidModeLockController.Instance.swith2KidModeLauncher ();

		updateToggleButtons ();

//		PlayerPrefs.SetString ("settingsLauncher", KidModeLockController.Instance.stateHomeLauncher.ToString());

	}

	public void openTabletSettings(){

		if (gameLogic != null) {

			Game game = gameLogic.GetComponent<Game>();

			game.gameController.getUI().createScreen(UIScreen.TABLET_SETTINGS, false,6);

		}

	}


}
