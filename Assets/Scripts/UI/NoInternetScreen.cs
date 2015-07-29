using UnityEngine;
using System.Collections;

public class NoInternetScreen : MonoBehaviour {

//	[SerializeField]
	private Game game;

	private UICanvas noInternetCanvas;
	
	void Start () 
	{
		GameObject gameLogic = GameObject.FindWithTag("GameController");
		game = gameLogic.GetComponent<Game>();
		UIManager l_ui = game.gameController.getUI();

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
		if (game != null) 
		{
			game.gameController.getUI().removeScreen(UIScreen.NO_INTERNET);
		}
	}

	private void SetupLocalization()
	{
		UILabel errorLabel = noInternetCanvas.getView("errorLabel") as UILabel;
		errorLabel.text = Localization.getString(Localization.NO_INTERNET_TEXT);
	}
}
