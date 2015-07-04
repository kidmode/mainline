using UnityEngine;
using System.Collections;

public class ErrorMessageScript : MonoBehaviour {

	[SerializeField]
	private GameObject gameLogic;
	
	private UICanvas m_errorMessageCanvas;

	// Use this for initialization
	void Start () {

		gameLogic = GameObject.Find ("GameLogic");
		
		GameController gameController = gameLogic.GetComponent<Game>().gameController;
		
		UIManager l_ui = gameController.getUI();
		
		m_errorMessageCanvas = l_ui.findScreen(UIScreen.ERROR_MESSAGE) as UICanvas;
	
	}
	
	public void leaveErrorMessage()
	{
		if (gameLogic != null) {
			
			Game game = gameLogic.GetComponent<Game>();

			game.gameController.getUI().removeScreen(UIScreen.ERROR_MESSAGE);
			
		}
	}
}
