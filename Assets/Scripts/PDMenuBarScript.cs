using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PDMenuBarScript : MonoBehaviour {

	public GameObject menuBarObject;
	public Canvas menuBarCanvas;

	[SerializeField]
	public GameObject[] firstTierBar;
	[SerializeField]
	public GameObject[] secondTierBar;

	private Game game;

	// Use this for initialization
	void Start () {

		game = GameObject.FindWithTag("GameController").GetComponent<Game>();

		menuBarCanvas.renderMode = RenderMode.ScreenSpaceCamera;
		menuBarCanvas.worldCamera = Camera.main;
		menuBarCanvas.sortingOrder = 10;

		//default page on parent dashboard
		changeSecondTierButtonColor();

		foreach (GameObject panel in secondTierBar)
		{
			foreach (Transform t in panel.transform)
			{
				if (t.gameObject.tag == "ButtonTag")
				{
					Button button = t.gameObject.GetComponent<Button>() as Button;
					int stateType = getStateTypeByName(t.gameObject.name);
					string buttoName = t.gameObject.name;
					button.onClick.AddListener(() => onButtonClicked(stateType));
					button.onClick.AddListener(() => changeSecondTierButtonColor(buttoName));
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onToogleChanged(bool isOn)
	{
		if (isOn)
		{
			int number = 0;
			foreach (GameObject item in firstTierBar)
			{
				Toggle toggle = item.GetComponent<Toggle>() as Toggle;
				if (toggle.isOn)
				{
					break;
				}
				number++;
			}

			int count = 0;
			foreach (GameObject item in secondTierBar)
			{
				if (count == number)
				{
					item.SetActive(true);
				}
				else
				{
					item.SetActive(false);
				}
				count++;
			}
			currentToogle = number;
			onButtonClicked(getSteteByNumber(number));
			changeSecondTierButtonColor();
		}
	}

	public void onBackButtonClicked()
	{
		game.gameController.changeState(ZoodleState.PROFILE_SELECTION);

		GameObject.Destroy(menuBarObject);
	}

	public void onButtonClicked(int stateType)
	{
		game.gameController.changeState(stateType);
	}

	private int getStateTypeByName(string name)
	{	
		int type = ZoodleState.OVERVIEW_INFO;
		switch (name)
		{
		case "Info":
			type = ZoodleState.OVERVIEW_INFO;
			break;
		case "Subjects":
			type = ZoodleState.OVERVIEW_TIMESPENT;
			break;
		case "Gallery":
			type = ZoodleState.OVERVIEW_ART;
			break;
		case "Apps":
			type = ZoodleState.CONTROL_APP;
			break;
		case "Books":
			type = ZoodleState.OVERVIEW_READING;
			break;
		case "ContentSubjects":
			type = ZoodleState.CONTROL_SUBJECT;
			break;
		case "Language":
			type = ZoodleState.CONTROL_LANGUAGE;
			break;
		case "Time":
			type = ZoodleState.CONTROL_TIME;
			break;
		case "Violence":
			type = ZoodleState.CONTROL_VIOLENCE;
			break;
		case "ChildLock":
			type = ZoodleState.CHILD_LOCK_STATE;
			break;
		case "Sound":
			type = ZoodleState.DEVICE_OPTIONS_STATE;
			break;
		case "Account":
			type = ZoodleState.SETTING_STATE;
			break;
		case "Notification":
			type = ZoodleState.NOTIFICATION_STATE;
			break;
		default:
			type = ZoodleState.OVERVIEW_INFO;
			break;
		}
		return type;
	}

	private int getSteteByNumber(int number)
	{
		if (number == 0)
			return ZoodleState.OVERVIEW_INFO;
		else if (number == 1)
			return ZoodleState.CONTROL_APP;
		else if (number == 2)
			return ZoodleState.CHILD_LOCK_STATE;
		else
			return ZoodleState.SETTING_STATE;
	}

	private void buttonColorChange(Button button, bool isFocused)
	{
		if (isFocused)
		{
			ColorBlock colors = button.colors;
			colors.normalColor = pressedColor;
			button.colors = colors;
		}
		else
		{
			ColorBlock colors = button.colors;
			colors.normalColor = normalColor;
			button.colors = colors;
		}
	}

	private void changeSecondTierButtonColor(string name)
	{
		GameObject panel = secondTierBar[currentToogle];
		foreach (Transform t in panel.transform)
		{
			if (t.gameObject.tag == "ButtonTag")
			{
				Button button = t.gameObject.GetComponent<Button>() as Button;
				bool isFocused = false;
				if (t.gameObject.name == name)
				{
					isFocused = true;
				}
				buttonColorChange(button, isFocused);
			}
		}
	}

	private void changeSecondTierButtonColor()
	{
		GameObject panel = secondTierBar[currentToogle];
		bool isBegin = true;
		foreach (Transform t in panel.transform)
		{
			if (t.gameObject.tag == "ButtonTag")
			{
				Button button = t.gameObject.GetComponent<Button>() as Button;
				bool isFocused = false;
				if (isBegin == true)
				{
					isFocused = true;
					isBegin = false;
				}
				buttonColorChange(button, isFocused);
			}
		}
	}

	private Color normalColor = new Color32(0, 126, 255, 255);
	private Color pressedColor = new Color32(247, 147 ,30, 255);
	private int currentToogle = 0;
}
