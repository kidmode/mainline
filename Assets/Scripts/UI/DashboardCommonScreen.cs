using UnityEngine;
using System.Collections;
using System;

public class DashboardCommonScreen : MonoBehaviour {

	public GameObject swith2DefaultLauncherButton;

	public GameObject swith2KidModeLauncherButton;

	// Use this for initialization
	void Start () {

		string settingsLauncher = PlayerPrefs.GetString ("settingsLauncher");

		KidModeLockController.Instance.stateHomeLauncher = (KidModeLockController.StateHomeLauncher) Enum.Parse(typeof(KidModeLockController.StateHomeLauncher), settingsLauncher);    

		updateToggleButtons ();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void updateToggleButtons(){

		Debug.Log ("  updateToggleButtons  " + KidModeLockController.Instance.stateHomeLauncher.ToString() );

		if (KidModeLockController.Instance.stateHomeLauncher == KidModeLockController.StateHomeLauncher.Default) {

			swith2DefaultLauncherButton.SetActive(false);

			swith2KidModeLauncherButton.SetActive(true);

		}else if (KidModeLockController.Instance.stateHomeLauncher == KidModeLockController.StateHomeLauncher.KidMode) {

			swith2DefaultLauncherButton.SetActive(true);
			
			swith2KidModeLauncherButton.SetActive(false);

			
		}

	}

	public void switchToDefaultLauncher(){

		KidModeLockController.Instance.swith2DefaultLauncher ();

		updateToggleButtons ();

		PlayerPrefs.SetString ("settingsLauncher", KidModeLockController.Instance.stateHomeLauncher.ToString());

	}

	public void switchToKidModeLauncher(){

		KidModeLockController.Instance.swith2KidModeLauncher ();

		updateToggleButtons ();

		PlayerPrefs.SetString ("settingsLauncher", KidModeLockController.Instance.stateHomeLauncher.ToString());

	}

}
