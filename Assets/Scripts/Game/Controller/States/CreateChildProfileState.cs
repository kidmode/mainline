using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text.RegularExpressions;


public class CreateChildProfileState : GameState 
{	
	enum SubState
	{
		NONE,
		LOADING,
		GO_PREVIOUS,
		GO_PROFILE,
		GO_PROFILE_VIEW,
	}

	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);

		if (m_queue == null)
			m_queue = new RequestQueue();

		m_subState = SubState.NONE;
		m_queue.reset ();

		_setupScreen(p_gameController.getUI());

		GAUtil.logScreen("CreateChildScreen");
	}
	
	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);
		switch (m_subState)
		{
		case SubState.GO_PREVIOUS:
			p_gameController.changeState(ZoodleState.PROFILE_SELECTION);
			m_subState = SubState.NONE;
			break;
		case SubState.GO_PROFILE:
			p_gameController.changeState(ZoodleState.PROFILE_SELECTION);
			m_subState = SubState.NONE;
			break;
		case SubState.GO_PROFILE_VIEW:
			p_gameController.changeState(ZoodleState.PROFILE_VIEW);
			m_subState = SubState.NONE;
			break;
		case SubState.LOADING:
			p_gameController.getUI().createScreen(UIScreen.LOADING_SPINNER, false, 1);
			m_createChildCanvas = null;
			m_subState = SubState.NONE;
			break;
		}

		if (m_queue.isCompleted())
		{
			if(SessionHandler.getInstance().CreateChild)
				m_subState = SubState.GO_PROFILE_VIEW;
			else
				p_gameController.changeState(ZoodleState.OVERVIEW_INFO);
		}

#if UNITY_ANDROID
		if (m_createChildCanvas != null)
		{
			if (null != m_firstNameText && m_firstNameText.color.a < 0.5f)
			{
				m_firstNameText.color= new Color(m_firstNameText.color.r, m_firstNameText.color.g, m_firstNameText.color.b, 1.0f);
			}
			if (null != m_lastNameText && m_lastNameText.color.a < 0.5f)
			{
				m_lastNameText.color= new Color(m_lastNameText.color.r, m_lastNameText.color.g, m_lastNameText.color.b, 1.0f);
			}
			if (null != m_yearText && m_yearText.color.a < 0.5f)
			{
				m_yearText.color= new Color(m_yearText.color.r, m_yearText.color.g, m_yearText.color.b, 1.0f);
			}
			if (null != m_monthText && m_monthText.color.a < 0.5f)
			{
				m_monthText.color= new Color(m_monthText.color.r, m_monthText.color.g, m_monthText.color.b, 1.0f);
			}
		}
#endif
	}
	
	public override void exit(GameController p_gameController)
	{
		p_gameController.getUI().removeScreen(UIScreen.CREATE_CHILDPROFILE);
		p_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER);
		base.exit(p_gameController);
	}

	//---------------- Private Implementation ----------------------
	
	private void _setupScreen(UIManager p_uiManager)
	{
		m_backScreen = p_uiManager.findScreen( UIScreen.SPLASH_BACKGROUND );
		if (m_backScreen == null)
		{
			m_backScreen = p_uiManager.createScreen(UIScreen.SPLASH_BACKGROUND,true, -1);
			(m_backScreen as SplashBackCanvas).setDown();
		}
		m_createChildCanvas = p_uiManager.createScreen(UIScreen.CREATE_CHILDPROFILE, true, 1);

		m_backButton = m_createChildCanvas.getView("backButton") as UIButton;
		m_backButton.tweener.addAlphaTrack (0.0f, 1.0f, 1.0f,null);

		m_title = m_createChildCanvas.getView("topicGroup") as UIElement;
		m_title.tweener.addAlphaTrack(0.0f, 1.0f, 1.0f,onTitleTweenFinish);

		m_dialog = m_createChildCanvas.getView("dialog") as UIElement;
		m_dialog.active = false;
		m_content = m_createChildCanvas.getView("mainContent") as UIElement;
		m_content.active = false;

		m_addPictureButton = m_createChildCanvas.getView("addPictureButton") as UIButton;
		m_addPictureButton.addClickCallback(toAddPhoto);
		m_createProfileButton = m_createChildCanvas.getView("createProfileButton") as UIButton;
		m_createProfileButton.addClickCallback(toCreateProfile);

		m_childFirstName = m_createChildCanvas.getView("childFirstNameArea").gameObject.GetComponent<InputField>();
		m_childLastName = m_createChildCanvas.getView("childLastNameArea").gameObject.GetComponent<InputField>();
		m_childBirthYear = m_createChildCanvas.getView("YearArea").gameObject.GetComponent<InputField>();
		m_childBirthMonth = m_createChildCanvas.getView("MonthArea").gameObject.GetComponent<InputField>();
		m_noticeText = m_createChildCanvas.getView ("noticeText") as UILabel;
		m_avatarImage = m_createChildCanvas.getView("avatarImage") as UIImage;

		if (!SessionHandler.getInstance ().CreateChild)
		{
			UIButton l_deleteChildButton = m_createChildCanvas.getView("deleteChildButton") as UIButton;
			l_deleteChildButton.active = true;
			m_currentChildName = m_createChildCanvas.getView("noticeText2") as UILabel;
			m_currentChildName.text = SessionHandler.getInstance().currentKid.wholeName;
			l_deleteChildButton.addClickCallback(toConfirmDeleteChild);
			m_confirmDeleteButton = m_createChildCanvas.getView("confirmButton") as UIButton;
			m_cancelDeleteButton = m_createChildCanvas.getView("cancelButton") as UIButton;
			m_confirmPanel = m_createChildCanvas.getView("confirmPanel") as UIElement;
			m_loadingPanel = m_createChildCanvas.getView("noticePanel") as UIElement;
			m_confirmDeleteButton.addClickCallback(toDeleteChild);
			m_cancelDeleteButton.addClickCallback(toCancelDeleteChild);
			m_loadingLabel = m_createChildCanvas.getView("LoadingText") as UILabel;
			m_loadingLabel.text = Localization.getString(Localization.TXT_STATE_13_LOADING);
			m_closeDialogButton = m_createChildCanvas.getView("closeButton") as UIButton;
			m_closeDialogButton.addClickCallback(toCancelDeleteChild);
			m_closeDialogButton.active = false;
		}

		SessionHandler l_session = SessionHandler.getInstance();
		if (null != l_session.selectAvatar && string.Empty != l_session.selectAvatar )
		{
			string l_avatarPath = "GUI/2048/common/avatars/" + l_session.selectAvatar;
			m_avatarImage.setTexture(l_avatarPath);
		}
		else
		{
			if(!l_session.CreateChild)
			{
				m_avatarImage.setTexture(l_session.currentKid.kid_photo);
			}
		}

		if (l_session.inputedChildName.Length > 0)
		{
			List<string> l_name = separateChildName(l_session.inputedChildName);
			if(null != l_name && l_name.Count >= 2)
			{
				m_childFirstName.text = l_name[0].ToString();
				m_childLastName.text = l_name[1].ToString();
			}
		}

		if (l_session.inputedbirthday.Length > 0)
		{
			List<string> l_birth = separateBirthdayString(l_session.inputedbirthday);
			if (null != l_birth && l_birth.Count >= 3)
			{
				m_childBirthYear.text = l_birth[0].ToString();
				m_childBirthMonth.text = l_birth[1].ToString();
			}
		}

		if(SessionHandler.getInstance().CreateChild)
		{
			if (null == SessionHandler.getInstance().kidList || 0 == SessionHandler.getInstance().kidList.Count)
			{
				m_backButton.addClickCallback(toBackSign);
			}
			else
			{
				m_backButton.addClickCallback(toBack);
			}
		}
		else
		{
			m_backButton.addClickCallback(toBackOverview);
			m_noticeText.text = Localization.getString(Localization.TXT_STATE_13_EDIT);
			UILabel l_buttonText = m_createProfileButton.getView("createProfileText") as UILabel;
			l_buttonText.text = Localization.getString(Localization.TXT_STATE_13_SAVE);
		}

#if UNITY_ANDROID
		m_firstNameText = m_createChildCanvas.getView("childFirstNameText") as UILabel;
		m_lastNameText = m_createChildCanvas.getView("childLastNameText") as UILabel;
		m_yearText = m_createChildCanvas.getView("YearText") as UILabel;
		m_monthText	= m_createChildCanvas.getView("MonthText") as UILabel;
#endif
	}

	private void onTitleTweenFinish(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		m_dialog.tweener.addAlphaTrack(0.0f, 1.0f, 0.5f, onDialogTweenFinish);
	}

	private void onDialogTweenFinish(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		m_content.tweener.addAlphaTrack(0.0f, 1.0f, 0.5f, null);
	}

	private void toBack(UIButton p_button)
	{
		SessionHandler.getInstance().inputedChildName = "";
		SessionHandler.getInstance().inputedbirthday = "";
		m_subState = SubState.GO_PREVIOUS;
		int l_state = m_gameController.getConnectedState (int.Parse (m_gameController.stateName));
		if(-999 == l_state)
			m_gameController.changeState (ZoodleState.PROFILE_SELECTION);
		else
		{
			m_gameController.changeState (l_state);
		}
	}
	
	private void toConfirmDeleteChild(UIButton p_button)
	{
		m_confirmPanel.active = true;
		m_loadingPanel.active = false;
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_currentPanel = m_createChildCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_currentPanel.transform.localPosition );
		l_pointListIn.Add( l_currentPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListIn, 0f );
	}

	private void toCancelDeleteChild(UIButton p_button)
	{
		m_closeDialogButton.active = false;
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_currentPanel = m_createChildCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_currentPanel.transform.localPosition );
		l_pointListIn.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListIn, 0f );
	}

	private void toDeleteChild(UIButton p_button)
	{
		m_confirmPanel.active = false;
		m_loadingPanel.active = true;
		if (!SessionHandler.getInstance ().CreateChild)
		{
			if(SessionHandler.getInstance().kidList.Count <= 1)
			{
				m_loadingLabel.text = Localization.getString(Localization.TXT_STATE_13_MIN);
				m_closeDialogButton.active = true;
			}
			else
			{
				m_queue.reset();
				m_queue.add(new DeleteChildRequest(_requestComplete));
				m_queue.request();
			}
		}
	}

	private void _requestComplete(WWW p_response)
	{
		if(null == p_response.error)
		{
			//SessionHandler.getInstance ().kidList.Remove(SessionHandler.getInstance ().currentKid);
			List<Kid> l_kidList = SessionHandler.getInstance ().kidList;
			int l_count = l_kidList.Count;
			int l_id =  SessionHandler.getInstance ().currentKid.id;
			for(int l_i = 0; l_i < l_count; l_i++)
			{
				if(l_kidList[l_i].id == l_id)
				{
					SessionHandler.getInstance ().kidList.RemoveAt(l_i);
					break;
				}
			}
			SessionHandler.getInstance ().currentKid = SessionHandler.getInstance ().kidList [0];
		}
		else
		{
			m_loadingLabel.text = Localization.getString(Localization.TXT_STATE_13_DELETE_FAIL);
			m_closeDialogButton.active = true;
		}
	}

	private void toBackOverview(UIButton p_button)
	{
		SessionHandler.getInstance().inputedChildName = "";
		SessionHandler.getInstance().inputedbirthday = "";
		m_gameController.changeState(ZoodleState.OVERVIEW_INFO);
	}

	private void toBackSign(UIButton p_button)
	{
		SessionHandler.getInstance().clearUserData();
		LocalSetting.find("User").delete();
		m_gameController.changeState(ZoodleState.CREATE_ACCOUNT_SELECTION);
	}

	private string combineChildName(string p_firstName, string p_lastName)
	{
		if (string.Empty.Equals(p_firstName) && string.Empty.Equals(p_lastName))
			return string.Empty;
		else if (string.Empty.Equals(p_firstName) && !string.Empty.Equals(p_lastName))
			return ZoodlesConstants.BLANK + p_lastName;
		else if (!string.Empty.Equals(p_firstName) && string.Empty.Equals(p_lastName))
			return p_firstName;
		else 
			return p_firstName + ZoodlesConstants.BLANK + p_lastName;
	}

	private List<string> separateChildName(string p_childName)
	{
		List<string> l_nameList = new List<string>();
		if (string.Empty.Equals(p_childName))
		{
			l_nameList.Add(string.Empty);
			l_nameList.Add(string.Empty);
		}
		else
		{
			if (p_childName.StartsWith(ZoodlesConstants.BLANK))
			{
				l_nameList.Add(string.Empty);
				l_nameList.Add(p_childName.Replace(ZoodlesConstants.BLANK,string.Empty));
			}
			else
			{
				if (p_childName.Contains(ZoodlesConstants.BLANK))
				{
					string[] l_nameArray = p_childName.Split(ZoodlesConstants.BLANK.ToCharArray());
					if (l_nameArray.Length >= 2)
					{
						l_nameList.Add(l_nameArray[0].ToString());
						l_nameList.Add(l_nameArray[1].ToString());
					}
					else
					{
						l_nameList.Add(string.Empty);
						l_nameList.Add(p_childName.Replace(ZoodlesConstants.BLANK,string.Empty));
					}
				}
				else
				{
					l_nameList.Add(p_childName);
					l_nameList.Add(string.Empty);
				}
			}
		}

		return l_nameList;
	}

	private string combineBirthdayString(string p_year, string p_month, string p_day)
	{
		return p_year + ZoodlesConstants.MIDDLE_LINE + p_month + ZoodlesConstants.MIDDLE_LINE + p_day;
	}

	private List<string> separateBirthdayString(string p_birth)
	{
		List<string> l_birthdayList = new List<string> ();
		if (p_birth.Contains(ZoodlesConstants.MIDDLE_LINE))
		{
			string[] l_birthArray = p_birth.Split(ZoodlesConstants.MIDDLE_LINE.ToCharArray());
			int l_lenght = l_birthArray.Length;
			for (int l_i = 0; l_i < l_lenght; l_i ++)
			{
				l_birthdayList.Add(l_birthArray[l_i].ToString());
			}
		}

		return l_birthdayList;
	}

	private void toAddPhoto(UIButton p_button)
	{
		SessionHandler.getInstance().inputedChildName = combineChildName(m_childFirstName.text, m_childLastName.text);
		SessionHandler.getInstance().inputedbirthday = combineBirthdayString(m_childBirthYear.text, m_childBirthMonth.text, "01");
		m_gameController.changeState(ZoodleState.SELECT_AVATAR);
	}

	private bool IsMatch(string p_pattern, string p_input)
	{
		if (p_input == null || p_input == "") return false;
		Regex regex = new Regex(p_pattern);
		return regex.IsMatch(p_input);
	}

	private bool checkData()
	{
		if (string.Empty.Equals (m_childFirstName.text))
		{
			setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_13_VARIF_FAIL),Localization.getString(Localization.TXT_STATE_13_EMPTY_NAME));
			return false;
		}
		else
		{
			if(m_childFirstName.text.Length + m_childLastName.text.Length > 32)
			{
				setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_13_VARIF_FAIL),Localization.getString(Localization.TXT_STATE_13_LENGTH_NAME));
				return false;
			}

			if (string.Empty.Equals(m_childBirthYear.text) || string.Empty.Equals(m_childBirthMonth.text))
			{
				if(string.Empty.Equals(m_childBirthYear.text))
					setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_13_VARIF_FAIL),Localization.getString(Localization.TXT_STATE_13_EMPTY_YEAR));
				else
					setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_13_VARIF_FAIL),Localization.getString(Localization.TXT_STATE_13_EMPTY_MONTH));

				return false;
			}
			else
			{
				//Is number
				if (IsMatch(ZoodlesConstants.DATE_REGULAR_NUMBER,m_childBirthYear.text) && IsMatch(ZoodlesConstants.DATE_REGULAR_NUMBER,m_childBirthMonth.text))
				{
					string l_birthYear;
					//year is 2 or 4 digit.
					if (m_childBirthYear.text.Length == 2)
					{
						l_birthYear = "20" + m_childBirthYear.text;
					}
					else if (m_childBirthYear.text.Length == 4)
					{
						l_birthYear = m_childBirthYear.text;
					}
					else
					{
						setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_13_VARIF_FAIL),Localization.getString(Localization.TXT_STATE_13_COUNT_YEAR));
						return false;
					}
					string l_birthday = combineBirthdayString(l_birthYear,m_childBirthMonth.text,"01");
					DateTime l_date = new DateTime();
					//can't convert to date.
					if (DateTime.TryParse(l_birthday, out l_date))
					{
						//kid doesn't born
						if (DateTime.Compare(DateTime.Now,l_date) < 0)
						{
							setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_13_VARIF_FAIL),Localization.getString(Localization.TXT_STATE_13_UNBORN));
							return false;
						}
						int l_age = 0;
						l_age = DateTime.Now.Year - l_date.Year;
						DateTime l_now = DateTime.Now;
						
						if( l_now.Month < l_date.Month )
						{
							l_age--;
						}
						else if( l_now.Month == l_date.Month && l_now.Day < l_date.Day )
						{
							l_age--;
						}
						if(l_age > 11)
						{
							setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_13_VARIF_FAIL),Localization.getString(Localization.TXT_STATE_13_11_YEAR));
							return false;
						}
					}
					else
					{
						setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_13_VARIF_FAIL),Localization.getString(Localization.TXT_STATE_13_DATE_ERROR));
						return false;
					}
					m_birthday = l_birthday;
					return true;
				}
				else
				{
					setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_13_VARIF_FAIL),Localization.getString(Localization.TXT_STATE_13_DATE_NUM));
					return false;
				}
			}
		}
	}

	private void toCreateProfile(UIButton p_button)
	{
		p_button.removeClickCallback(toCreateProfile);
		int l_count = null != SessionHandler.getInstance ().kidList?SessionHandler.getInstance ().kidList.Count:0;
		
		SessionHandler.getInstance().inputedChildName = combineChildName(m_childFirstName.text, m_childLastName.text);
		SessionHandler.getInstance().inputedbirthday = combineBirthdayString(m_childBirthYear.text, m_childBirthMonth.text, "01");
	
		if (checkData ()) 
		{
			if (SessionHandler.getInstance().CreateChild)
			{
				if (l_count >= 6) 
				{
					setErrorMessage(m_gameController, Localization.getString(Localization.TXT_STATE_13_CREATE_FAIL), Localization.getString(Localization.TXT_STATE_13_MAX));
					return;
				}
				if (null == SessionHandler.getInstance().selectAvatar || string.Empty.Equals(SessionHandler.getInstance().selectAvatar))
				{
					SessionHandler.getInstance().selectAvatar = "icon_avatar_gen";
				}
				
				string l_url = "@absolute:";
				if (Application.platform == RuntimePlatform.Android)
				{
					l_url += "jar:file://"+Application.dataPath+"!/assets/"+ SessionHandler.getInstance().selectAvatar + ".png";
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					l_url += Application.dataPath + "/Raw/" + SessionHandler.getInstance().selectAvatar + ".png";
				}
				else
				{
					l_url += "file://" + Application.dataPath + "/StreamingAssets/" + SessionHandler.getInstance().selectAvatar + ".png";
				}
				
				m_queue.add(new ImageRequest("childAvatar", l_url));
				m_queue.add(new CreateChildRequest(combineChildName(m_childFirstName.text,m_childLastName.text), m_birthday, "childAvatar"));
				m_queue.request(RequestType.SEQUENCE);
				m_subState = SubState.LOADING;
			}
			else
			{
				if (null == SessionHandler.getInstance().selectAvatar || string.Empty.Equals(SessionHandler.getInstance().selectAvatar))
				{
					m_queue.add(new EditChildRequest(combineChildName(m_childFirstName.text,m_childLastName.text),m_birthday));
					m_queue.request(RequestType.SEQUENCE);
					m_subState = SubState.LOADING;
				}
				else
				{
					string l_url = "@absolute:";
					if (Application.platform == RuntimePlatform.Android)
					{
						l_url += "jar:file://" + Application.dataPath+"!/assets/"+ SessionHandler.getInstance().selectAvatar + ".png";
					}
					else if (Application.platform == RuntimePlatform.IPhonePlayer)
					{
						l_url += Application.dataPath + "/Raw/" + SessionHandler.getInstance().selectAvatar + ".png";
					}
					else
					{
						l_url += "file://" + Application.dataPath + "/StreamingAssets/" + SessionHandler.getInstance().selectAvatar + ".png";
					}

					if (m_queue == null)
						m_queue = new RequestQueue();
					m_queue.add(new ImageRequest("newAvatar", l_url));
					m_queue.add(new UpdatePhotoRequest("newAvatar", null ));
					m_queue.add(new EditChildRequest(combineChildName(m_childFirstName.text,m_childLastName.text),m_birthday));
					m_queue.request(RequestType.SEQUENCE);
					m_subState = SubState.LOADING;
				}
			}
		}
	}
	
	//Private variables

	private UIElement	m_title;
	private UIElement	m_content;
	private UIElement	m_dialog;
	private UIElement	m_confirmPanel;
	private UIElement	m_loadingPanel;
	private UIButton 	m_backButton;
	private UIElement 	m_avatarArea;
	private UIButton 	m_createProfileButton;
	private UIButton 	m_selectAvatarButton;
	private UIButton 	m_addPictureButton;
	private UIButton 	m_confirmDeleteButton;
	private UIButton 	m_cancelDeleteButton;
	private UIButton 	m_closeDialogButton;
	private UIImage 	m_avatarImage;

	private InputField 	m_childFirstName;
	private InputField 	m_childLastName;

	private InputField 	m_childBirthYear;
	private InputField 	m_childBirthMonth;

	#if UNITY_ANDROID
	private UILabel 	m_firstNameText;
	private UILabel 	m_lastNameText;
	private UILabel 	m_yearText;
	private UILabel 	m_monthText;
	#endif

	private UILabel 	m_loadingLabel;
	private UILabel 	m_currentChildName;
	private UILabel		m_noticeText;
	private string 		m_inputNum;
	private UICanvas	m_backScreen;

	private UICanvas    m_createChildCanvas;

	private Hashtable 	m_postData;

	private string 		m_birthday;

	private SubState	m_subState = SubState.NONE;

	private RequestQueue m_queue;
}
