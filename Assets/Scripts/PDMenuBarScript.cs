using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

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
	public ScrollRect childSelectorSrollRect;

	public Image currentKidImage;
	public Text currentkidText;
	public GameObject currentkidArrowDown;
	public GameObject currentkidArrowUp;

	private Game game;
	private Color normalColor = new Color32(0, 126, 255, 255);
	private Color pressedColor = new Color32(247, 147 ,30, 255);
	private int currentToogle = 0;
	private List<Texture2D> childButtonImages = new List<Texture2D>();
	private int textLengthLimit = 8;
	private int currentKidIndex = 0;

	public delegate void ButtonEventDelegate(UnityEngine.EventSystems.BaseEventData baseEvent);

	// Use this for initialization
	void Start () 
	{
		game = GameObject.FindWithTag("GameController").GetComponent<Game>();
		game.pdMenuBar = this;

		menuBarCanvas.renderMode = RenderMode.ScreenSpaceCamera;
		menuBarCanvas.worldCamera = Camera.main;
		menuBarCanvas.sortingOrder = 10;

		//show selected button of default page on parent dashboard
		changeSecondTierButtonColor();
		//set button listener on second tier menu bar
		foreach (GameObject panel in secondTierBar)
		{
			foreach (Transform t in panel.transform)
			{
				if (t.gameObject.tag == "ButtonTag")
				{
					Button button = t.gameObject.GetComponent<Button>() as Button;
					int stateType = getStateTypeByName(t.gameObject.name);
					string buttoName = t.gameObject.name;
					Debug.Log("buttonName: " + buttoName);

//					addButtonEventTrigger(t.gameObject, EventTriggerType.PointerDown, pointerDownEvent);
//					addButtonEventTrigger(t.gameObject, EventTriggerType.PointerUp, pointerUpEvent);

					button.onClick.AddListener(() => changeSecondTierButtonColor(buttoName));
					button.onClick.AddListener(() => onButtonClicked(stateType));

				}
			}
		}

		Texture2D texture1 = Resources.Load("GUI/2048/common/buttons/bt_circle_up") as Texture2D;
		Texture2D texture2 = Resources.Load("GUI/2048/common/buttons/bt_circle_down") as Texture2D;
		childButtonImages.Add(texture1);
		childButtonImages.Add(texture2);
		//show current kid info on first tier menu bar
		setCurrentKidOnFirstMenuBar();
		//add childern for child selector
		addAllChildern();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//when pressing button on first tier menu bar
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
			//change state to new page
			onButtonClicked(getSteteByNumber(number));
			//change first button color(defalut button) on second tier menu bar
			changeSecondTierButtonColor();
			//hide child selector if needed
			setChildSelectorVisible(false);
		}
	}

	//return to profile switch 
	public void onBackButtonClicked()
	{
		game.gameController.changeState(ZoodleState.PROFILE_SELECTION);
		removePDMenuBar();
	}

	//change state to new page
	public void onButtonClicked(int stateType)
	{
		Debug.Log(stateType + " Button clicked");
		game.gameController.changeState(stateType);
	}

	//get state id by button name 
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
//			type = ZoodleState.OVERVIEW_READING;
			type = ZoodleState.OVERVIEW_BOOK;
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

	//get state id by pressed button on first tier menu bar
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

	//change button normal, highlighted, pressed color
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

	//when pressed button on second tier menu bar, change selected button color  
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

	//change first button color(defalut button) on second tier menu bar
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

	//add childern for child selector
	private void addAllChildern()
	{
		List<Kid> kidList = SessionHandler.getInstance().kidList;
		if (kidList != null)
		{
		 	foreach (Kid item in kidList)
			{
				Kid kid = item;
				GameObject child = GameObject.Instantiate(childPrototype) as GameObject;
				//change text
				Text text1 = child.transform.FindChild("Name").gameObject.GetComponent<Text>();
				int textLengthLimit = 8;
				if (kid.name.Length > textLengthLimit)
					text1.text = kid.name.Substring(0, textLengthLimit) + "...";
				else
					text1.text = kid.name;
				Text text2 = child.transform.FindChild("HiddenName").gameObject.GetComponent<Text>();
				text2.text = kid.name;
				//add button listener
				Button button = child.GetComponent<Button>();
				button.onClick.AddListener(() => cancelChildSelectorSelectedImage());
				button.onClick.AddListener(() => kidSelected(kid));
				//change image
				Image image = child.transform.FindChild("IconBG/Icon").gameObject.GetComponent<Image>();
				image.sprite = createSprite(kid.kid_photo);
				Image image2 = child.transform.FindChild("IconBG2/Icon").gameObject.GetComponent<Image>();
				image2.sprite = createSprite(kid.kid_photo);
				//added to childernList
				child.transform.localScale = menuBarCanvas.transform.localScale;
				child.transform.parent = gridLayout.transform;
				child.SetActive(true);
			}

			//add child icon
			{
				GameObject child = GameObject.Instantiate(childPrototype) as GameObject;
				//change text
				Text text1 = child.transform.FindChild("Name").gameObject.GetComponent<Text>();
				text1.text = Localization.getString(Localization.TXT_86_BUTTON_ADD_CHILD);
				Text text2 = child.transform.FindChild("HiddenName").gameObject.GetComponent<Text>();
				text2.text = Localization.getString(Localization.TXT_86_BUTTON_ADD_CHILD);
				//add button listener
				Button button = child.GetComponent<Button>();
				button.onClick.AddListener(() => kidSelected(null));
				//change icon bg image
				Image iconBG = child.transform.FindChild("IconBG").gameObject.GetComponent<Image>();
				Texture2D bgTexture = Resources.Load("GUI/2048/parent_dashboard/pd_icon_addchild1") as Texture2D;
				iconBG.sprite = createSprite(bgTexture);
				Texture2D pressedBGTexture = Resources.Load("GUI/2048/parent_dashboard/pd_icon_addchild2") as Texture2D;
				SpriteState spriteState = new SpriteState();
				spriteState.pressedSprite = createSprite(pressedBGTexture);
				button.spriteState = spriteState;
				iconBG.transform.localScale = Vector3.one * .75f;
				//change icon image
				GameObject icon = child.transform.FindChild("IconBG/Icon").gameObject;
				Image image = icon.GetComponent<Image>();
				Texture2D texture = Resources.Load("GUI/2048/common/icon/icon_profile_add") as Texture2D;
				image.sprite = createSprite(texture);
				icon.transform.localScale = Vector3.one * .5f;
				//added to childernList
				child.transform.localScale = menuBarCanvas.transform.localScale;
				child.transform.parent = gridLayout.transform;
				child.SetActive(true);
			}

			setContentWidth();
		}
		else
		{

		}
	}

	//set content width for child selector 
	public void setContentWidth()
	{
		float scrollContentWidth = (gridLayout.transform.childCount * gridLayout.cellSize.x) + ((gridLayout.transform.childCount - 1) * gridLayout.spacing.x);
		RectTransform rt = (RectTransform)childernList.transform;
		rt.sizeDelta = new Vector2(scrollContentWidth, rt.sizeDelta.y);

		Debug.Log("######################## ChildCount: " + gridLayout.transform.childCount);

//		((RectTransform)menuBarObject.transform).localScale = Vector3.one * .5f;
	}

	//this pressed button listener for selected child on child selector
	private void kidSelected(Kid kid)
	{
		if (kid != null)
		{
			SessionHandler.getInstance().currentKid = kid;
			//change state to overview info
			currentToogle = 0;
			changeSecondTierButtonColor();
			onButtonClicked(getSteteByNumber(currentToogle));

			setChildSelectorVisible(false);
			setCurrentKidOnFirstMenuBar();
		}
		else
		{
			SessionHandler.getInstance().CreateChild = true;
			game.gameController.connectState(ZoodleState.CREATE_CHILD_NEW,int.Parse(game.gameController.stateName));
			onButtonClicked(ZoodleState.CREATE_CHILD_NEW);

			setChildSelectorVisible(false);
			setPDMenuBarVisible(false, false);
		}
	}

	private void setChildSelectorVisible(bool visible)
	{
		childSelector.SetActive(visible);
		secondTier.SetActive(!visible);
		currentkidArrowDown.SetActive(!visible);
		currentkidArrowUp.SetActive(visible);
	}

	//create image sprite from image texture
	private Sprite createSprite(Texture2D p_texture)
	{
		Sprite l_sprite = Sprite.Create(p_texture, 
		                                new Rect(0, 0, p_texture.width, p_texture.height), 
		                                new Vector2(0, 0));
		return l_sprite;
	}

	//this is button delegate for pressing current kid info button
	public void onChildSelectorClicked()
	{
		if (childSelector.activeInHierarchy)
		{
			setChildSelectorVisible(false);
//			currentkidArrowDown.SetActive(true);
//			currentkidArrowUp.SetActive(false);
		}
		else
		{
			setChildSelectorCurrentKidPosition();
			changeChildSelectorSelectedImage();
			setChildSelectorVisible(true);
//			currentkidArrowDown.SetActive(false);
//			currentkidArrowUp.SetActive(true);
		}
	}

	//set current kid info on first tier menu bar
	public void setCurrentKidOnFirstMenuBar()
	{
		Kid kid = SessionHandler.getInstance().currentKid;
		currentkidText.text = kid.name;
		if (kid.name.Length > textLengthLimit)
			currentkidText.text = kid.name.Substring(0, textLengthLimit) + "...";
		else
			currentkidText.text = kid.name;
		kid.requestPhoto();
		currentKidImage.sprite = createSprite(kid.kid_photo);
		currentkidArrowDown.SetActive(true);
		currentkidArrowUp.SetActive(false);
	}

	//change button normal image
	private void buttonImageChange(GameObject button, bool isFocused)
	{
		GameObject iconBG2 = button.transform.FindChild("IconBG2").gameObject;
		if (isFocused)
		{
			iconBG2.SetActive(true);
//			Image image = button.transform.FindChild("IconBG").gameObject.GetComponent<Image>();
//			image.sprite = createSprite(childButtonImages[1]);
		}
		else
		{
			iconBG2.SetActive(false);
//			Image image = button.transform.FindChild("IconBG").gameObject.GetComponent<Image>();
//			image.sprite = createSprite(childButtonImages[0]);
		}
	}

	//set selcted child selectd image for child selector 
	public void changeChildSelectorSelectedImage()
	{
		string addchildtext = Localization.getString(Localization.TXT_86_BUTTON_ADD_CHILD);
		foreach (Transform t in childernList.transform)
		{
			if (t.gameObject.name.Contains("ChildPrototype"))
			{
				Text text = t.transform.FindChild("HiddenName").gameObject.GetComponent<Text>();
				if (!text.text.Equals(addchildtext))
				{
					bool isFocused = false;
					if (SessionHandler.getInstance().currentKid.name.Equals(text.text))
					{
						isFocused = true;
					}
					GameObject button = t.gameObject;
					buttonImageChange(button, isFocused);
				}
			}
		}
	}

	public void cancelChildSelectorSelectedImage()
	{
		GameObject button = childernList.transform.GetChild(currentKidIndex).gameObject;
		buttonImageChange(button, false);
	}

	//this is button delegate for tablet setting button
	public void onSettingbuttonClicked()
	{
		game.gameController.getUI().createScreen(UIScreen.TABLET_SETTINGS, false, 11);
	}

	public void setPDMenuBarVisible(bool visible, bool isChildUpdated = false)
	{
		if (visible == menuBarObject.activeInHierarchy)
			return;

		if (visible)
		{
			menuBarCanvas.sortingOrder = 10;
			setCurrentKidOnFirstMenuBar();
			if (isChildUpdated)
				updateChildern();
		}
		else
		{
			menuBarCanvas.sortingOrder = 0;
		}

		menuBarObject.SetActive(visible);
	}

	private void updateChildern()
	{
		removeAllChildern();
		addAllChildern();
	}

	private void removeAllChildern()
	{
		int count = gridLayout.transform.childCount;
		for (int i = count - 1; i >= 0; i--)
		{
			GameObject child = gridLayout.transform.GetChild(i).gameObject;
#if UNITY_EDITOR
			DestroyImmediate(child);
#else
			Destroy(child);
#endif
		}
	}

	public void removePDMenuBar()
	{
		game.pdMenuBar = null;
		GameObject.Destroy(menuBarObject);
	}

	public void pointerDownEvent(UnityEngine.EventSystems.BaseEventData baseEvent) 
	{
		Debug.Log(baseEvent.selectedObject.name + " triggered an pointer down event!");
	}

	public void pointerUpEvent(UnityEngine.EventSystems.BaseEventData baseEvent) 
	{
		Debug.Log(baseEvent.selectedObject.name + " triggered an pointer up event!");
	}

	public void addButtonEventTrigger(GameObject button, EventTriggerType eventTriggerType, ButtonEventDelegate eventDelegate)
	{

		EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();

		if (eventTrigger == null)
		{
			Debug.Log("can not find event trigger component for button " + button.name);
			return;
		}
		//Create a new entry. This entry will describe the kind of event we're looking for
		// and how to respond to it
		EventTrigger.Entry entry = new EventTrigger.Entry();
		//This event will respond to a drop event
		entry.eventID = eventTriggerType;
		//Create a new trigger to hold our callback methods
		entry.callback = new EventTrigger.TriggerEvent();
		//Create a new UnityAction, it contains our DropEventMethod delegate to respond to events
		UnityAction<BaseEventData> callback = new UnityAction<BaseEventData>(eventDelegate);
		//Add our callback to the listeners
		entry.callback.AddListener(callback);
		//Add the EventTrigger entry to the event trigger component
		eventTrigger.delegates.Add(entry);
	}

	public void hideChildSeletor()
	{
		setChildSelectorVisible(false);
	}

	private int findCurrentKidIndex()
	{
		int index = 0;
		foreach (Kid kid in SessionHandler.getInstance().kidList)
		{
			if (kid.name == SessionHandler.getInstance().currentKid.name)
			{
				break;
			}
			index++;
		}

		if (index == SessionHandler.getInstance().kidList.Count)
		{
			index = 0;
		}

		return index;
	}

	private void setChildSelectorCurrentKidPosition()
	{
		currentKidIndex = findCurrentKidIndex();
		int totalItems = childernList.transform.childCount;

		childSelectorSrollRect.horizontalNormalizedPosition = (float)currentKidIndex/(float)totalItems;
	}
}
