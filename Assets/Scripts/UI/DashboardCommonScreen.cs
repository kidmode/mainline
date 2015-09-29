using UnityEngine;
using System.Collections;
//using System;

//===================================================
//Kevin
//Buttons Events are handeled across many any  state classes . .. .  want to do new button actions here to save time and less complexity.

public class DashboardCommonScreen : MonoBehaviour {

//	[SerializeField]
	private Game game;

//	public static DashboardCommonScreen Instance;
//
	public UICanvas tableSettingsCanvas;
//
//	void Awake(){
//
//		Instance = this;
//
//	}

	// Use this for initialization
	void Start () 
	{
		updateToggleButtons ();
		GameObject gameLogic = GameObject.FindWithTag("GameController");
		game = gameLogic.GetComponent<Game>();
	}

	void Update()
	{

		if( null != tableSettingsCanvas )
		{
			
			if(null != tableSettingsCanvas.getView("panel")){
				
				if( !tableSettingsCanvas.getView("panel").active){
					
					Tweener tw = tableSettingsCanvas.getView("panel").tweener;
					
					tw.addAlphaTrack( 0.0f, 1.0f, 0.2f );
				}
				
			}
			
		}

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
			tableSettingsCanvas = game.gameController.getUI().createScreen(UIScreen.TABLET_SETTINGS, false, 6);
		}
	}
}
