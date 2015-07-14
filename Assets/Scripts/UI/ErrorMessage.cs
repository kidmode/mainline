using UnityEngine;
using System.Collections;

public class ErrorMessage : MonoBehaviour {

	public delegate void onClickEvent();
	public event onClickEvent onClick;

	[SerializeField]
	private GameObject gameLogic;
	
	private UICanvas m_errorMessageCanvas;

	// Use this for initialization
	void Start () {

		gameLogic = GameObject.FindWithTag("GameController");
		
		GameController gameController = gameLogic.GetComponent<Game>().gameController;
		
		UIManager l_ui = gameController.getUI();
		
		m_errorMessageCanvas = l_ui.findScreen(UIScreen.ERROR_MESSAGE) as UICanvas;
	
		SetupLocalization();
	}
	
	public void leaveErrorMessage()
	{
		if (gameLogic != null) {
			
			Game game = gameLogic.GetComponent<Game>();

			game.gameController.getUI().removeScreen(UIScreen.ERROR_MESSAGE);
		}

		if (onClick != null)
		{
			onClick();
		}
	}

	private void SetupLocalization()
	{
		UILabel errorLabel = m_errorMessageCanvas.getView("errorLabel") as UILabel;
		errorLabel.text = Localization.getString(Localization.ERROR_MESSAGE_ERROR_TEXT);
	}
}
