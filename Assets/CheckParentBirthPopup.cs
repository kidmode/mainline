using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;
using System;

public class CheckParentBirthPopup : MonoBehaviour {

	public delegate void onClickEvent(bool successful);
	public event onClickEvent onClick;

	private Game game;
	private UICanvas m_checkParentBirthPopupCanvas;

	private bool isSuccessful = false;
	private UILabel yearText;
	private UILabel monthText;
	private UILabel dayText;
	private UILabel errorMessage;

	private InputField birthYear;
	private InputField birthMonth;
	private InputField birthDay;

	private UIImage yearErrorImage;
	private UIImage monthErrorImage;
	private UIImage dayErrorImage;

	void Start () {

		GameObject gameLogic = GameObject.FindWithTag("GameController");
		game = gameLogic.GetComponent<Game>();
		UIManager l_ui = game.gameController.getUI();
		
		m_checkParentBirthPopupCanvas = l_ui.findScreen(UIScreen.PARENT_BIRTH_CHECK) as UICanvas;

		birthYear = m_checkParentBirthPopupCanvas.getView("YearArea").gameObject.GetComponent<InputField>();
		birthMonth = m_checkParentBirthPopupCanvas.getView("MonthArea").gameObject.GetComponent<InputField>();
		birthDay = m_checkParentBirthPopupCanvas.getView("DayArea").gameObject.GetComponent<InputField>();

		yearErrorImage = m_checkParentBirthPopupCanvas.getView("YearErrorImage") as UIImage;
		monthErrorImage = m_checkParentBirthPopupCanvas.getView("MonthErrorImage") as UIImage;
		dayErrorImage = m_checkParentBirthPopupCanvas.getView("DayErrorImage") as UIImage;

		yearText = m_checkParentBirthPopupCanvas.getView("YearText") as UILabel;
		monthText = m_checkParentBirthPopupCanvas.getView("MonthText") as UILabel;
		dayText = m_checkParentBirthPopupCanvas.getView("DayText") as UILabel;

		SetupLocalization();
	}

	public void leaveParentBirthCheck()
	{
		if (game != null) 
		{
			game.gameController.getUI().removeScreen(UIScreen.PARENT_BIRTH_CHECK);
		}

		if (onClick != null)
		{
			onClick(isSuccessful);
			onClick = null;
		}
	}
	
	public void checkParentBirth()
	{
		yearErrorImage.active = false;
		monthErrorImage.active = false;
		dayErrorImage.active = false;

		int check = checkBirth();
		if (check == 0) 
		{
			return;
		} 
		else if (check == 1)
		{
			isSuccessful = true;
			leaveParentBirthCheck();
		}
		else 
		{
			isSuccessful = false;

			UIElement parentBirth = m_checkParentBirthPopupCanvas.getView("ParentBirth");
			parentBirth.active = false;
			UIElement verficationFail = m_checkParentBirthPopupCanvas.getView("VerificationFail");
			verficationFail.active = true;
		}
	}

	//return value: 0 -> field error, 1 -> field correct and age correct, 2 -> field correct and age failed
	private int checkBirth()
	{
		bool fieldError = true;
		if (string.Empty.Equals(birthYear.text)) 
		{
			fieldError = false;
			yearErrorImage.active = true;
		}
		if (string.Empty.Equals(birthMonth.text)) 
		{
			fieldError = false;
			monthErrorImage.active = true;
		}
		if (string.Empty.Equals(birthDay.text)) 
		{
			fieldError = false;
			dayErrorImage.active = true;
		}
		if (IsMatch (ZoodlesConstants.DATE_REGULAR_NUMBER, birthYear.text)) 
		{
			if (birthYear.text.Length < 4) 
			{
				fieldError = false;
				yearErrorImage.active = true;
			}
		} 
		else 
		{
			fieldError = false;
			yearErrorImage.active = true;
		}
		if (IsMatch (ZoodlesConstants.DATE_REGULAR_NUMBER, birthMonth.text)) 
		{
			int month = int.Parse (birthMonth.text);
			if (month <= 0 || month >= 13)
			{
				fieldError = false;
				monthErrorImage.active = true;
			}
		} 
		else 
		{
			fieldError = false;
			monthErrorImage.active = true;
		}
		if (IsMatch (ZoodlesConstants.DATE_REGULAR_NUMBER, birthDay.text)) 
		{
			int day = int.Parse (birthDay.text);
			if (day <= 0 || day >= 32)
			{
				fieldError = false;
				dayErrorImage.active = true;
			}
		} 
		else
		{
			fieldError = false;
			dayErrorImage.active = true;
		}

		//field error
		if (fieldError == false)
			return 0;

		yearErrorImage.active = false;
		monthErrorImage.active = false;
		dayErrorImage.active = false;

		//year is smaller than 13 years old
		int year = int.Parse(birthYear.text);
		
		if( DateTime.Now.Year - year <= 13 )
		{
			return 2;
		}
		//correct
		return 1;
	}
	
	private void SetupLocalization()
	{
		UILabel titleText = m_checkParentBirthPopupCanvas.getView("TitleText") as UILabel;
		titleText.text = Localization.getString(Localization.TXT_CPB_POPUP_TITLE);

		UILabel yearText = m_checkParentBirthPopupCanvas.getView("YearTextPlaceholder") as UILabel;
		yearText.text = Localization.getString(Localization.TXT_CPB_POPUP_YEAR);

		UILabel monthText = m_checkParentBirthPopupCanvas.getView("MonthTextPlaceholder") as UILabel;
		monthText.text = Localization.getString(Localization.TXT_CPB_POPUP_MONTH);

		UILabel dayText = m_checkParentBirthPopupCanvas.getView("DayTextPlaceholder") as UILabel;
		dayText.text = Localization.getString(Localization.TXT_CPB_POPUP_DAY);

		UILabel okText = m_checkParentBirthPopupCanvas.getView("OKText") as UILabel;
		okText.text = Localization.getString(Localization.TXT_CPB_POPUP_OK_BTN);

		UILabel failTitleText = m_checkParentBirthPopupCanvas.getView("FailTitleText") as UILabel;
		failTitleText.text = Localization.getString (Localization.TXT_STATE_13_VARIF_FAIL);

		UILabel failContentText = m_checkParentBirthPopupCanvas.getView("FailContentText") as UILabel;
		failContentText.text = Localization.getString (Localization.TXT_25_LABEL_FAIL);
	}

	private bool IsMatch(string p_pattern, string p_input)
	{
		if (p_input == null || p_input == "") return false;
		Regex regex = new Regex(p_pattern);
		return regex.IsMatch(p_input);
	}
}
