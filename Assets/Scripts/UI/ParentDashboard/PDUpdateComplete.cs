using UnityEngine;
using System;
using System.Collections;


public class PDUpdateComplete : MonoBehaviour {

	public static event Action onUpdateComplete;

	private Game game;

	// Use this for initialization
	void Start () {

		game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void updateComplete(HttpsWWW p_response){

		UICanvas messageCanvas = game.gameController.getUI().createScreen(UIScreen.PD_MESSAGE, false, 20);
		
		UIButton messageCloseButton = messageCanvas.getView("quitButton") as UIButton;
		
		messageCloseButton.addClickCallback(closePDMessage);
		
		
		if(p_response.error == null){
			
			game.gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);
			
		}else{
			
			game.gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 20);
			
		}

		if(onUpdateComplete != null)
			onUpdateComplete();

	}

	void closePDMessage(UIButton button){

		game.gameController.getUI().removeScreen(UIScreen.PD_MESSAGE);

	}

}
