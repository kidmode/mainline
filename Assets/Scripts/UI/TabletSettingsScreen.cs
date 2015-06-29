using UnityEngine;
using System.Collections;

public class TabletSettingsScreen : MonoBehaviour {

	[SerializeField]
	private GameObject highlightKidmodeLauncher;

	[SerializeField]
	private GameObject highlightDeaultLauncher;

	[SerializeField]
	private GameObject homeSettingsPanel;

	[SerializeField]
	private GameObject gameLogic;



	// Use this for initialization
	void Start () {

		gameLogic = GameObject.Find ("GameLogic");

		//=========================
		//Add localisation here


		//Set Home settings panel
		homeSettingsPanel.SetActive (false);
	
	}

	void showHomeSettingsHighLights(){

		if (KidModeLockController.Instance.stateHomeLauncher == KidModeLockController.StateHomeLauncher.Default) {

			highlightKidmodeLauncher.SetActive(false);

			highlightDeaultLauncher.SetActive(true);

		}else if (KidModeLockController.Instance.stateHomeLauncher == KidModeLockController.StateHomeLauncher.KidMode) {

			highlightKidmodeLauncher.SetActive(true);
			
			highlightDeaultLauncher.SetActive(false);

			
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void buttonSettings(){

		KidMode.openSettings ();

	}

	public void buttonHome(){

		homeSettingsPanel.SetActive (true);

		showHomeSettingsHighLights ();
		
	}

	public void buttonExit(){

		KidModeLockController.Instance.swith2DefaultLauncher ();

//		KidModeLockController.Instance.stateHomeLauncher = KidModeLockController.StateHomeLauncher.Default;
//		
//		PlayerPrefs.SetString ("settingsLauncher", KidModeLockController.Instance.stateHomeLauncher.ToString());
//
//		KidMode.disablePluginComponent ();

		KidMode.openDefaultLauncher ();

//		KidMode.startActivity ("com.android.launcher3");
//		KidMode.openDefaultLauncher ();

	}

	public void openDefaultLauncher(){

		KidMode.openDefaultLauncher ();

	}

	public void buttonWifi(){

		KidMode.openWifi ();
		
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

		homeSettingsPanel.SetActive (false);

	}

	public void setLauncherKidmode(){

		KidModeLockController.Instance.swith2KidModeLauncher ();

		showHomeSettingsHighLights ();

	}

	public void setLauncherDefault(){

		KidModeLockController.Instance.swith2DefaultLauncher ();

		showHomeSettingsHighLights ();

	}



}
