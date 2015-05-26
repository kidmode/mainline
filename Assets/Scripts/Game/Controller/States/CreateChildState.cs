using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text.RegularExpressions;

public class CreateChildState : GameState
{
	enum SubState
	{
		NONE,
		GO_PREVIOUS,
		GO_PROFILE,
		GO_PROFILE_VIEW,
	}

	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);
		
		m_subState = SubState.NONE;
		
		_setupScreen(p_gameController.getUI());

		m_verfiyString = Localization.getString (Localization.TXT_STATE_69_VERIFICATION);
		
		GAUtil.logScreen("CreateChildScreen");
	}

	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);
		switch (m_subState)
		{
		case SubState.GO_PREVIOUS:
			SessionHandler.getInstance().inputedChildName = string.Empty;
			SessionHandler.getInstance().inputedbirthday = string.Empty;
			p_gameController.changeState(ZoodleState.PROFILE_SELECTION);
			m_subState = SubState.NONE;
			break;
		case SubState.GO_PROFILE:
			SessionHandler.getInstance().inputedChildName = string.Empty;
			SessionHandler.getInstance().inputedbirthday = string.Empty;
			p_gameController.changeState(ZoodleState.PROFILE_SELECTION);
			m_subState = SubState.NONE;
			break;
		case SubState.GO_PROFILE_VIEW:
			p_gameController.changeState(ZoodleState.PROFILE_VIEW);
			m_subState = SubState.NONE;
			break;
		}
		
		#if UNITY_ANDROID
		if (m_createChildCanvas != null)
		{
			if (null != m_firstNameText && m_firstNameText.color.a < 0.5f)
			{
				m_firstNameText.color= new Color(m_firstNameText.color.r, m_firstNameText.color.g, m_firstNameText.color.b, 1.0f);
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
		p_gameController.getUI().removeScreen(UIScreen.CREATE_CHILD_NEW);
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
		m_createChildCanvas = p_uiManager.createScreen(UIScreen.CREATE_CHILD_NEW, true, 1);
		
		m_backButton = m_createChildCanvas.getView("backButton") as UIButton;
		m_backButton.tweener.addAlphaTrack (0.0f, 1.0f, 1.0f,null);
		
		m_title = m_createChildCanvas.getView("topicText") as UIElement;
		m_title.tweener.addAlphaTrack(0.0f, 1.0f, 1.0f,onTitleTweenFinish);
		
		m_dialog = m_createChildCanvas.getView("dialog") as UIElement;
		m_dialog.active = false;

		m_createProfileButton = m_createChildCanvas.getView("createProfileButton") as UIButton;
		m_createProfileButton.addClickCallback(toAddPhoto);
		
		m_childFirstName = m_createChildCanvas.getView("childFirstNameArea").gameObject.GetComponent<InputField>();
		m_childBirthYear = m_createChildCanvas.getView("YearArea").gameObject.GetComponent<InputField>();
		m_childBirthMonth = m_createChildCanvas.getView("MonthArea").gameObject.GetComponent<InputField>();
		
		SessionHandler l_session = SessionHandler.getInstance();
		
		if (l_session.inputedChildName.Length > 0)
		{
			m_childFirstName.text = l_session.inputedChildName;
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

		if (null == SessionHandler.getInstance().kidList || 0 == SessionHandler.getInstance().kidList.Count)
		{
			m_backButton.addClickCallback(toBackSign);
		}
		else
		{
			m_backButton.addClickCallback(toBack);
		}
		
		#if UNITY_ANDROID
		m_firstNameText = m_createChildCanvas.getView("childFirstNameText") as UILabel;
		m_yearText = m_createChildCanvas.getView("YearText") as UILabel;
		m_monthText	= m_createChildCanvas.getView("MonthText") as UILabel;
		#endif
	}

	private void onTitleTweenFinish(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		m_dialog.tweener.addAlphaTrack(0.0f, 1.0f, 0.5f, null);
	}

	private void toBack(UIButton p_button)
	{
		p_button.removeClickCallback ( toBack );

		SessionHandler.getInstance().inputedChildName = "";
		SessionHandler.getInstance().inputedbirthday = "";
		m_subState = SubState.GO_PREVIOUS;
		int l_state = m_gameController.getConnectedState (int.Parse (m_gameController.stateName));
		if(GameController.UNDEFINED_STATE == l_state)
			m_gameController.changeState (ZoodleState.PROFILE_SELECTION);
		else
		{
			m_gameController.changeState (l_state);
		}
	}

	private void toBackSign(UIButton p_button)
	{
		p_button.removeClickCallback ( toBackSign );
		SessionHandler.getInstance().clearUserData();
		LocalSetting.find("User").delete();
		m_gameController.changeState(ZoodleState.CREATE_ACCOUNT_SELECTION);
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
			setErrorMessage(m_gameController,m_verfiyString,Localization.getString (Localization.TXT_STATE_69_CANNOT_BE_EMPTY));
			return false;
		}
		else
		{
			if(m_childFirstName.text.Length > 32)
			{
				setErrorMessage(m_gameController,m_verfiyString,Localization.getString (Localization.TXT_STATE_69_LENGHT_LESS_THAN_64));
				return false;
			}
			
			if (string.Empty.Equals(m_childBirthYear.text) || string.Empty.Equals(m_childBirthMonth.text))
			{
				if(string.Empty.Equals(m_childBirthYear.text))
					setErrorMessage(m_gameController,m_verfiyString,Localization.getString (Localization.TXT_STATE_69_YEAR_CANNOT_BE_EMPTY));
				else
					setErrorMessage(m_gameController,m_verfiyString,Localization.getString (Localization.TXT_STATE_69_MONTH_CANNOT_BE_EMPTY));
				
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
						setErrorMessage(m_gameController,m_verfiyString,Localization.getString (Localization.TXT_STATE_69_YEAR_LENGHT));
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
							setErrorMessage(m_gameController,m_verfiyString,Localization.getString (Localization.TXT_STATE_69_CANNOT_CREATE_UNBORN_KID));
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
							setErrorMessage(m_gameController,m_verfiyString,Localization.getString (Localization.TXT_STATE_69_KID_AGE));
							return false;
						}
					}
					else
					{
						setErrorMessage(m_gameController,m_verfiyString,Localization.getString (Localization.TXT_STATE_69_DATE_NOT_CORRECT));
						return false;
					}

					SessionHandler.getInstance().inputedbirthday = l_birthday;

					return true;
				}
				else
				{
					setErrorMessage(m_gameController,m_verfiyString,Localization.getString (Localization.TXT_STATE_69_DATE_MUST_BE_NUMBER));
					return false;
				}
			}
		}
	}

	private void toAddPhoto(UIButton p_button)
	{
		p_button.removeClickCallback(toAddPhoto);
		
		int l_count = null != SessionHandler.getInstance().kidList?SessionHandler.getInstance ().kidList.Count:0;
		
		if (checkData ()) 
		{
			if (l_count >= 6) 
			{
				setErrorMessage(m_gameController,Localization.getString (Localization.TXT_STATE_69_CREATE_KID_FAILED),Localization.getString (Localization.TXT_STATE_69_SIX_CHILD));
				return;
			}

			SessionHandler.getInstance().inputedChildName = m_childFirstName.text;
			m_gameController.changeState(ZoodleState.SELECT_AVATAR);
		}
	}

	private UIElement	m_title;
	private UIElement	m_dialog;
	private UIButton 	m_backButton;
	private UIButton 	m_createProfileButton;
	private string		m_verfiyString;
	
	private InputField 	m_childFirstName;
	private InputField 	m_childBirthYear;
	private InputField 	m_childBirthMonth;
	
	#if UNITY_ANDROID
	private UILabel 	m_firstNameText;
	private UILabel 	m_yearText;
	private UILabel 	m_monthText;
	#endif

	private UICanvas	m_backScreen;
	
	private UICanvas    m_createChildCanvas;
	
	private SubState	m_subState = SubState.NONE;
}
