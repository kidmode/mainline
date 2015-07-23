using UnityEngine;
using System.Collections;

public class ErrorMessage : MonoBehaviour {

	public delegate void onClickEvent();
	public event onClickEvent onClick;

//	[SerializeField]
	private Game game;
	
	private UICanvas m_errorMessageCanvas;

	void Start () 
	{
		GameObject gameLogic = GameObject.FindWithTag("GameController");
		game = gameLogic.GetComponent<Game>();
		UIManager l_ui = game.gameController.getUI();

		m_errorMessageCanvas = l_ui.findScreen(UIScreen.ERROR_MESSAGE) as UICanvas;
	
		SetupLocalization();
	}
	
	public void leaveErrorMessage()
	{
		if (game != null) 
		{
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
