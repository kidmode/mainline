using UnityEngine;
using System.Collections;

public class FOTAPopup : MonoBehaviour {

	public delegate void onClickEvent();
	public event onClickEvent onClick;

	public string fotaState = "Update";

	private Game game;
	private UICanvas m_fotaPopupCanvas;

	//TODO: need to know current fota state to have correct title and content. also, set onClick event if needed
	void Start () 
	{
		GameObject gameLogic = GameObject.FindWithTag("GameController");
		game = gameLogic.GetComponent<Game>();
		UIManager l_ui = game.gameController.getUI();

		m_fotaPopupCanvas = l_ui.findScreen(UIScreen.FOTA_POPUP) as UICanvas;
		
		SetupLocalization();
	}

	public void leaveFOTAPopup()
	{
		if (game != null) 
		{
			game.gameController.getUI().removeScreen(UIScreen.FOTA_POPUP);
		}
		
//		if (onClick != null)
//		{
//			onClick();
//			onClick = null;
//		}
	}

	public void fotaPopupConfirmed()
	{
		if (game != null) 
		{
			game.gameController.getUI().removeScreen(UIScreen.FOTA_POPUP);
		}
	}

	private void SetupLocalization()
	{
		UILabel fotaNameLabel = m_fotaPopupCanvas.getView("FotaNameLabel") as UILabel;
		fotaNameLabel.text = string.Format(Localization.getString(Localization.TXT_FOTA_POPUP_TITLE),
		                                   fotaState);

		UILabel fotaContentLabel = m_fotaPopupCanvas.getView("FotaContentLabel") as UILabel;
		fotaContentLabel.text = string.Format(Localization.getString(Localization.TXT_FOTA_POPUP_CONTENT),
		                                      fotaState.ToLower());

		UILabel confirmText = m_fotaPopupCanvas.getView("ConfirmText") as UILabel;
		confirmText.text = Localization.getString(Localization.TXT_FOTA_POPUP_CONFIRM_BTN);

		UILabel cancelText = m_fotaPopupCanvas.getView("CancelText") as UILabel;
		cancelText.text = Localization.getString(Localization.TXT_FOTA_POPUP_CANCEL_BTN);
	}
}
