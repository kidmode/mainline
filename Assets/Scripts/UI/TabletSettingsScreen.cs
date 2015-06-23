using UnityEngine;
using System.Collections;

public class TabletSettingsScreen : MonoBehaviour {

	private GameObject gameLogic;

	// Use this for initialization
	void Start () {

		gameLogic = GameObject.Find ("GameLogic");

		//=========================
		//Add localisation here
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void buttonSettings(){

	}

	public void buttonHome(){
		
	}

	public void buttonWifi(){
		
	}

	public void buttonGooglePlay(){
		
	}

	public void closeScreen(){

		if (gameLogic != null) {
			
			Game game = gameLogic.GetComponent<Game>();
			
			game.gameController.getUI().removeScreen(UIScreen.TABLET_SETTINGS);
			
		}

	}
}
