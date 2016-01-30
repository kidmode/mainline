using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PDMenuBarScript : MonoBehaviour {

	public GameObject menuBarObject;
	public Canvas menuBarCanvas;

	[SerializeField]
	public GameObject[] firstTierBar;
	[SerializeField]
	public GameObject[] secondTierBar;

	public GameObject childPrototype;
	public GameObject childernList;
	public GridLayoutGroup gridLayout;

	public GameObject secondTier;
	public GameObject childSelector;

	public Image currentKidImage;
	public Text currentkidText;

	private Game game;
	private Color normalColor = new Color32(0, 126, 255, 255);
	private Color pressedColor = new Color32(247, 147 ,30, 255);
	private int currentToogle = 0;


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
 
		setCurrentKidOnFirstMenuBar();
		addChildern();
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

	private void addChildern()
	{
		List<Kid> kidList = SessionHandler.getInstance().kidList;
		if (kidList != null)
		{
		 	foreach (Kid item in kidList)
			{
				Kid kid = item;
				GameObject child = GameObject.Instantiate(childPrototype) as GameObject;
				//change text
				Text text = child.transform.FindChild("Name").gameObject.GetComponent<Text>();
				text.text = kid.name;
				//add button listener
				Button button = child.GetComponent<Button>();
				button.onClick.AddListener(() => kidSelected(kid));
				//change image
				Image image = child.transform.FindChild("IconBG/Icon").gameObject.GetComponent<Image>();
				image.sprite = createSprite(kid.kid_photo);
				//added to childernList
				child.transform.localScale = menuBarCanvas.transform.localScale;
				child.transform.parent = gridLayout.transform;
//				child.SetActive(true);
			}

			//add child icon
			{
				GameObject child = GameObject.Instantiate(childPrototype) as GameObject;
				//change text
				Text text = child.transform.FindChild("Name").gameObject.GetComponent<Text>();
				text.text = Localization.getString(Localization.TXT_86_BUTTON_ADD_CHILD);
				//add button listener
				Button button = child.GetComponent<Button>();
				button.onClick.AddListener(() => kidSelected(null));
				//change image
				GameObject icon = child.transform.FindChild("IconBG/Icon").gameObject;
				Image image = icon.GetComponent<Image>();
				Texture2D texture = Resources.Load("GUI/2048/common/icon/icon_profile_add") as Texture2D;
				image.sprite = createSprite(texture);
				icon.transform.localScale = Vector3.one * .5f;
				//added to childernList
				child.transform.localScale = menuBarCanvas.transform.localScale;
				child.transform.parent = gridLayout.transform;
			}

			setContentWidth();
		}
		else
		{

		}
	}

	public void setContentWidth()
	{
		float scrollContentWidth = (gridLayout.transform.childCount * gridLayout.cellSize.x) + ((gridLayout.transform.childCount - 1) * gridLayout.spacing.x);
		RectTransform rt = (RectTransform)childernList.transform;
		rt.sizeDelta = new Vector2(scrollContentWidth, rt.sizeDelta.y);
	}

	private void kidSelected(Kid kid)
	{
		if (kid != null)
		{
			SessionHandler.getInstance().currentKid = kid;
			//change state to overview info
			currentToogle = 0;
			onButtonClicked(getSteteByNumber(currentToogle));
			changeSecondTierButtonColor();

			childSelector.SetActive(false);
			secondTier.SetActive(true);
			setCurrentKidOnFirstMenuBar();
		}
		else
		{

		}
	}

	private Sprite createSprite(Texture2D p_texture)
	{
		Sprite l_sprite = Sprite.Create(p_texture, 
		                                new Rect(0, 0, p_texture.width, p_texture.height), 
		                                new Vector2(0, 0));
		return l_sprite;
	}

	public void onChildSelectorClicked()
	{
		if (childSelector.activeInHierarchy)
		{
			childSelector.SetActive(false);
			secondTier.SetActive(true);
		}
		else
		{
			childSelector.SetActive(true);
			secondTier.SetActive(false);
		}
	}

	public void setCurrentKidOnFirstMenuBar()
	{
		Kid kid = SessionHandler.getInstance().currentKid;
		currentkidText.text = kid.name;
		kid.requestPhoto();
		currentKidImage.sprite = createSprite(kid.kid_photo);
	}
}
