using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AddItemManuallyPopup : MonoBehaviour {

	public delegate void onClickEvent(List<object> list);
	public event onClickEvent onClick;

	public delegate void onItemsFromGDriveCompletedEvent(string jsonData);
	public event onItemsFromGDriveCompletedEvent onItemsFromGDriveCompleted;

	private Game game;
	private UICanvas m_addNewItemPopupCanvas;
	private InputField url;
	private InputField displayName;

	private UIButton addItemButton;
	private UIButton updateGDriveButton;
	private UIButton closeButton;

	//popup type should be "Game" or "Video"
	public string popupType;

	void Start () {

		GameObject gameLogic = GameObject.FindWithTag("GameController");
		game = gameLogic.GetComponent<Game>();
		UIManager l_ui = game.gameController.getUI();

		m_addNewItemPopupCanvas = l_ui.findScreen(UIScreen.ADD_ITEMS_MANUALLY) as UICanvas;
		
		setupScreen();
		setupLocalization();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setPopupType(string type)
	{
		popupType = type;
	}

	private void setupScreen()
	{
		url = m_addNewItemPopupCanvas.getView("URLField").gameObject.GetComponent<InputField>();
		displayName = m_addNewItemPopupCanvas.getView("DisplayNameField").gameObject.GetComponent<InputField>();

		addItemButton = m_addNewItemPopupCanvas.getView("AddItemButton") as UIButton;
		updateGDriveButton = m_addNewItemPopupCanvas.getView("DownloadItemButton") as UIButton;
		closeButton = m_addNewItemPopupCanvas.getView("QuitButton") as UIButton;
	}

	private void setupLocalization()
	{
		UILabel titlelabel = m_addNewItemPopupCanvas.getView("TitleLabel") as UILabel;
		UILabel addItemlabel = m_addNewItemPopupCanvas.getView("AddItemText") as UILabel;
		if (popupType.Equals("Game"))
		{
			titlelabel.text = "Add New Games";
			addItemlabel.text = "Add Game";
		}
		else
		{
			titlelabel.text = "Add New Videos";
			addItemlabel.text = "Add Video";
		}
	}

	public void singleItemClicked()
	{
		addItemButton.enabled = false;
		updateGDriveButton.enabled = false;
		closeButton.enabled = false;

		List<string> item = new List<string>();
		item.Add(displayName.text);
		item.Add(url.text);
		item.Add(popupType);

		List<object> list = new List<object>();
		list.Add(item);

		if (onClick != null)
		{
			onClick(list);
		}
		leavePopup();
	}

	public void leavePopup()
	{
		addItemButton.enabled = false;
		updateGDriveButton.enabled = false;
		closeButton.enabled = false;

		onClick = null;
		onItemsFromGDriveCompleted = null;
		if (game != null) 
		{
			game.gameController.getUI().removeScreen(UIScreen.ADD_ITEMS_MANUALLY);
		}
	}

	public void updateItemsFromGDrive()
	{
		addItemButton.enabled = false;
		updateGDriveButton.enabled = false;
		closeButton.enabled = false;

		KidMode.refreshTestingContent(popupType.ToUpper());
	}

	public void itemsFromGDriveCompleted(string jsonData)
	{
		if (onItemsFromGDriveCompleted != null)
		{
			onItemsFromGDriveCompleted(jsonData);
		}
		leavePopup();
	}

}
