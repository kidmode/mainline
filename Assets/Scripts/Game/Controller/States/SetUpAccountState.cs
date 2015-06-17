using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class SetUpAccountState : GameState 
{

	enum ScreenChange
	{
		None,
		CreateAccountSelectScreen,
		NextScreen
	}
	
	//Public variables
	
	
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );
		
		m_IsCreateFreeAccount = string.Empty.Equals(SessionHandler.getInstance ().creditCardNum);

		_setupScreen( p_gameController.getUI() );

		m_passwordText = string.Empty;

		m_repasswordText = string.Empty;

//		m_game = p_gameController.game;
//
//		if (null == m_postData) 
//		{
//			m_postData = new Hashtable();
//		}
		SwrveComponent.Instance.SDK.NamedEvent("tutorial.SET_UP_ACCOUNT_UI");
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
		if (ScreenChange.None != changeToState) 
		{
			switch(changeToState)
			{
			case ScreenChange.CreateAccountSelectScreen:
				SessionHandler.getInstance().creditCardNum = string.Empty;
				p_gameController.changeState(ZoodleState.CREATE_ACCOUNT_SELECTION);
				changeToState = ScreenChange.None;
				break;
			case ScreenChange.NextScreen:
				m_signUpCanvas.active = false;
				p_gameController.getUI().createScreen(UIScreen.LOADING_SPINNER,false,2);
				changeToState = ScreenChange.None;
				break;
			default:
				changeToState = ScreenChange.None;
				break;
			}
		}

		if(SessionHandler.getInstance().token.isExist())
		{
			p_gameController.changeState(ZoodleState.CONGRATURATION);
		}
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( UIScreen.LOADING_SPINNER );
		p_gameController.getUI().removeScreenImmediately( UIScreen.SIGN_UP_AFTER_INPUT_CREDITCARD );
	}
	
	
	//---------------- Private Implementation ----------------------

	private void _setupScreen( UIManager p_uiManager )
	{
		m_createAccountBackgroundCanvas = p_uiManager.findScreen( UIScreen.SPLASH_BACKGROUND );
		if( m_createAccountBackgroundCanvas == null )
            m_createAccountBackgroundCanvas = p_uiManager.createScreen( UIScreen.SPLASH_BACKGROUND, true, -1 );
		
		m_signUpCanvas = p_uiManager.createScreen( UIScreen.SIGN_UP_AFTER_INPUT_CREDITCARD, true, 1 );

		m_backButton = m_signUpCanvas.getView("backButton") as UIButton;
		m_backButton.addClickCallback (toBack);
		m_createFreeAccountButton = m_signUpCanvas.getView("createFreeAccountButton") as UIButton;
		m_createFreeAccountButton.addClickCallback (toCreateChildrenScreen);
		m_createAccountButton = m_signUpCanvas.getView("createAccountButton") as UIButton;
		m_createAccountButton.addClickCallback (toCreateChildrenScreen);
		m_getPremiumButton = m_signUpCanvas.getView("getPremiumButton") as UIButton;
		m_getPremiumButton.addClickCallback (onClickGetPremium);

		m_emailCheckImage = m_signUpCanvas.getView ("emailInputConfirm") as UIImage;
		m_passwordCheckImage = m_signUpCanvas.getView ("passwordInputConfirm") as UIImage;
		m_premiumLogoArea = m_signUpCanvas.getView ("premiumLogoArea") as UIElement;
		m_title			  = m_signUpCanvas.getView ("setupDialogTitleText") as UILabel;

		m_passwordCheckImage.active = true;
		m_emailCheckImage.active = true;
		m_emailCheckImage.setSprite(Resources.Load<Sprite>("GUI/2048/common/icon/icon_close_red"));
		m_passwordCheckImage.setSprite(Resources.Load<Sprite>("GUI/2048/common/icon/icon_close_red"));

		m_dialog = m_signUpCanvas.getView ("dialog") as UIElement;
		m_errorTitle = m_dialog.getView ("dialogTitleText") as UILabel;
		m_errorContent = m_dialog.getView ("contentText") as UILabel;

		m_panel = m_signUpCanvas.getView("panel") as UIElement;
		m_account = m_panel.getView ("emailInput").gameObject.GetComponent<InputField>();
		m_password = m_panel.getView ("pwdInput").gameObject.GetComponent<InputField>();
		m_rePassword = m_panel.getView ("repwdInput").gameObject.GetComponent<InputField>();

		m_closeDialogButton = m_dialog.getView ("closeMark") as UIButton;
		m_closeDialogButton.addClickCallback (closeDialog);

		if(m_IsCreateFreeAccount && SessionHandler.getInstance().renewalPeriod == 0)
		{
			m_title.text = Localization.getString(Localization.TXT_STATE_9_ACCOUNT_FREE);
			m_premiumLogoArea.active = false;
			m_getPremiumButton.active =	true;
			m_createAccountButton.active = false;
			m_createFreeAccountButton.active = true;
		}
		else
		{
			m_title.text = Localization.getString(Localization.TXT_STATE_9_ACCOUNT);
			m_premiumLogoArea.active = true;
			m_getPremiumButton.active =	false;
			m_createAccountButton.active = true;
			m_createFreeAccountButton.active = false;
		}
		//m_account.onValueChange.AddListener (onUsernameChange);
		m_account.onEndEdit.AddListener (onFinsihedEdit);
	//	#if !UNITY_ANDROID
		m_rePassword.onValueChange.AddListener (onRepasswordChange);
		m_password.onValueChange.AddListener (onPasswordChange);
	//	#endif	
	}

	private void onFinsihedEdit(string p_value)
	{
		if(emailPasses())
		{
			if(null == m_checkAccountRequest)
				m_checkAccountRequest = new RequestQueue();
			else
				m_checkAccountRequest.reset();
			//m_emailCheckImage.setSprite(Resources.Load<Sprite>("GUI/2048/common/icon/icon_check"));
			m_checkAccountRequest.add(new CheckAccountRequest(p_value,finsihCheckEmail));
			m_checkAccountRequest.request();
		}
		else
		{
			m_emailCheckImage.setSprite(Resources.Load<Sprite>("GUI/2048/common/icon/icon_close_red"));
		}
	}

	private void finsihCheckEmail(WWW p_response)
	{
		m_emailCheckImage.setSprite(Resources.Load<Sprite>("GUI/2048/common/icon/icon_close_red"));
		if(null == p_response.error)
		{
			Hashtable l_jsonResponse = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			if(l_jsonResponse.ContainsKey("jsonResponse"))
			{
				Hashtable l_response = l_jsonResponse["jsonResponse"] as Hashtable;
				if(l_response.ContainsKey("response"))
				{
					Hashtable l_bookData = l_response["response"] as Hashtable;
					if(l_bookData.ContainsKey("validate"))
					{
						bool l_validate = (bool)l_bookData["validate"];
						if(l_validate)
							m_emailCheckImage.setSprite(Resources.Load<Sprite>("GUI/2048/common/icon/icon_check"));
						else
							m_emailCheckImage.setSprite(Resources.Load<Sprite>("GUI/2048/common/icon/icon_close_red"));
					}
				}
			}
		}
	}

	private void onClickGetPremium(UIButton p_button)
	{
		m_gameController.connectState (ZoodleState.SIGN_UP_UPSELL, int.Parse(m_gameController.stateName));
		m_gameController.changeState (ZoodleState.SIGN_UP_UPSELL);
	}

	private void onUsernameChange(string p_value)
	{
		m_emailCheckImage.setSprite(Resources.Load<Sprite>("GUI/2048/common/icon/icon_close_red"));
	}

	private void onPasswordChange(string p_value)
	{
		m_passwordText = p_value;
		if(passwordPasses())
		{
			m_passwordCheckImage.setSprite(Resources.Load<Sprite>("GUI/2048/common/icon/icon_check"));
		}
		else
		{
			m_passwordCheckImage.setSprite(Resources.Load<Sprite>("GUI/2048/common/icon/icon_close_red"));
		}
	}

	private void onRepasswordChange(string p_value)
	{
		m_repasswordText = p_value;
		
		if(passwordPasses())
		{
			m_passwordCheckImage.setSprite(Resources.Load<Sprite>("GUI/2048/common/icon/icon_check"));
		}
		else
		{
			m_passwordCheckImage.setSprite(Resources.Load<Sprite>("GUI/2048/common/icon/icon_close_red"));
		}
	}

	private void onTitleTweenFinish( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_panel.tweener.addAlphaTrack (0.0f, 1.0f, 0.2f);
	}

	private void toBack( UIButton p_button )
	{
		changeToState = ScreenChange.CreateAccountSelectScreen;
	}

	private bool IsMatch(string p_pattern, string p_input)
	{
		if (p_input == null || p_input == "") return false;
		Regex regex = new Regex(p_pattern);
		return regex.IsMatch(p_input);
	}

	private bool passwordPasses()
	{
//		return m_password.text.Equals(m_rePassword.text) && m_password.text.Length > 3;
		return m_passwordText.Equals (m_repasswordText) && m_passwordText.Length > 3;
	}

	private bool emailPasses()
	{
		string l_email = m_account.text;
		return IsMatch(ZoodlesConstants.EMAIL_REGULAR_STRING,l_email);
	}

	private void toCreateChildrenScreen( UIButton p_button )
	{
		bool l_emailPasses = emailPasses();
		bool l_passwordPasses = passwordPasses();

		if(l_emailPasses && l_passwordPasses)
		{
			p_button.removeClickCallback( toCreateChildrenScreen );

			changeToState = ScreenChange.NextScreen;
			RequestQueue l_queue = new RequestQueue();
//				l_queue.add(new ClientIdRequest());
			l_queue.add(new SignUpRequest(m_account.text, m_password.text,createAccountComplete));
			l_queue.request(RequestType.SEQUENCE);
			SwrveComponent.Instance.SDK.NamedEvent("tutorial.end");
		}
		else
		{
			if (!l_emailPasses)
			{
				invokeDialog(Localization.getString(Localization.TXT_STATE_9_FAIL),Localization.getString(Localization.TXT_STATE_9_ERROR_EMAIL));
			}
			else
			{
				if (m_password.text.Length < 1 || m_rePassword.text.Length < 1)
				{
					invokeDialog(Localization.getString(Localization.TXT_STATE_9_FAIL),Localization.getString(Localization.TXT_STATE_9_EMPTY_PASSWORD));
				}
				else if (m_password.text.Length < 4 || m_rePassword.text.Length < 4)
				{
					invokeDialog(Localization.getString(Localization.TXT_STATE_9_FAIL),Localization.getString(Localization.TXT_STATE_11_PASSWORD));
				}
				else
				{
					invokeDialog(Localization.getString(Localization.TXT_STATE_9_FAIL),Localization.getString(Localization.TXT_STATE_11_PASSWORD));
				}
			}
		}
	}

	private void createAccountComplete(WWW p_response)
	{
		if(null == p_response.error)
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			if(l_data.ContainsKey("jsonResponse"))
			{
				invokeDialog(Localization.getString(Localization.TXT_STATE_9_UNAVAILABLE), Localization.getString(Localization.TXT_STATE_9_UNAVAILABLE_CONTENT));
			}
			else
			{
				string l_secret = l_data.ContainsKey(ZoodlesConstants.PARAM_TOKEN) ? l_data[ZoodlesConstants.PARAM_TOKEN].ToString() : "";
				bool l_premium = l_data.ContainsKey("premium") && (bool)l_data["premium"];
				bool l_current = l_data.ContainsKey("is_current") && null != l_data["is_current"] && (bool)l_data["is_current"];
				int l_vpc = l_data.ContainsKey("vpc_level") ? int.Parse(l_data["vpc_level"].ToString()) : 0;
				bool l_tried = l_data.ContainsKey ("is_tried") && null != l_data ["is_tried"] ? (bool)l_data ["is_tried"] : true;
				SessionHandler.getInstance().token.setToken(l_secret, l_premium,l_tried,l_current,l_vpc);
				
				SessionHandler.getInstance().username = m_account.text;

				RequestQueue l_queue = new RequestQueue();
				l_queue.add(new GetUserSettingRequest());
				LocalSetting l_setting = LocalSetting.find("ServerSetting");
				if (!l_setting.hasKey(ZoodlesConstants.ZPS_LEVEL))
					l_queue.add(new GetLevelsInfoRequest());
				if (!l_setting.hasKey(ZoodlesConstants.EXPERIENCE_POINTS))
					l_queue.add(new GetExperiencePointsInfoRequest());
				if (!l_setting.hasKey(ZoodlesConstants.CATEGORIES))
					l_queue.add(new GetCategoriesInfoRequest());
				if (!l_setting.hasKey(ZoodlesConstants.TAGS))
					l_queue.add(new GetTagsInfoRequest());
				if (!l_setting.hasKey(ZoodlesConstants.SUBJECTS))
					l_queue.add(new GetSubjectsInfoRequest());

				l_queue.request(RequestType.SEQUENCE);
			}
		}
		else
		{
			invokeDialog(Localization.getString(Localization.TXT_STATE_9_FAIL), Localization.getString(Localization.TXT_STATE_9_NETWORK));
		}
	}

	private void invokeDialog(string p_errorTitle, string p_errorContent)
	{
		m_signUpCanvas.active = true;
		if(null != m_gameController.getUI().findScreen(UIScreen.LOADING_SPINNER))
			m_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER);

		m_errorTitle.text = p_errorTitle;
		m_errorContent.text = p_errorContent;
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_dialog.transform.localPosition );
		l_pointListOut.Add( m_dialog.transform.localPosition - new Vector3( 0, 582, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	private void closeDialog(UIButton p_button)
	{
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_dialog.transform.localPosition );
		l_pointListOut.Add( m_dialog.transform.localPosition + new Vector3( 0, 582, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f );
		m_createFreeAccountButton.addClickCallback (toCreateChildrenScreen);
	}

	//Private variables

	private UIElement 	m_panel;
	private UIElement	m_dialog;
	private UIButton 	m_backButton;
	private UIButton 	m_createFreeAccountButton;
	private UILabel		m_errorTitle;
	private UILabel		m_errorContent;
	private UIButton 	m_closeDialogButton;

	private UICanvas    m_signUpCanvas;
	private UICanvas	m_createAccountBackgroundCanvas;

	private InputField 	m_account;
	private InputField 	m_password;
	private InputField 	m_rePassword;

	private UIButton 	m_createAccountButton;
	private UIButton 	m_getPremiumButton;
	private UIImage 	m_emailCheckImage;
	private UIImage 	m_passwordCheckImage;
	private UIElement	m_premiumLogoArea;
	private bool 		m_IsCreateFreeAccount;
	private UILabel		m_title;

	private string 		m_passwordText;
	private string 		m_repasswordText;

	private Hashtable 	m_postData;
	private RequestQueue m_checkAccountRequest;
	private UILabel 	m_pwdText;

	private ScreenChange changeToState = ScreenChange.None;
}
