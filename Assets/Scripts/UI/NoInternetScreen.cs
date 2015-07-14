using UnityEngine;
using System.Collections;

public class NoInternetScreen : MonoBehaviour {
	
	private GameObject gameLogic;
	private UICanvas noInternetCanvas;
	
	void Start () {

		gameLogic = GameObject.FindWithTag("GameController");

		GameController gameController = gameLogic.GetComponent<Game>().gameController;

		UIManager l_ui = gameController.getUI();

		noInternetCanvas = l_ui.findScreen(UIScreen.NO_INTERNET) as UICanvas;

		SetupLocalization();
	}

	public void buttonExit()
	{
		KidModeLockController.Instance.swith2DefaultLauncher ();
		KidMode.openDefaultLauncher ();
	}
	
	public void buttonWifi()
	{	
		KidMode.openWifi(true);
	}

	public void removeScreen()
	{
		if (gameLogic != null) 
		{
			Game game = gameLogic.GetComponent<Game>();	
			game.gameController.getUI().removeScreen(UIScreen.NO_INTERNET);
		}
	}

	private void SetupLocalization()
	{
		UILabel errorLabel = noInternetCanvas.getView("errorLabel") as UILabel;
		errorLabel.text = Localization.getString(Localization.NO_INTERNET_TEXT);
	}
}
