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

		_setupScreen( p_gameController.getUI() );
//
//		m_game = p_gameController.game;
//
//		if (null == m_postData) 
//		{
//			m_postData = new Hashtable();
//		}
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
		if (ScreenChange.None != changeToState) 
		{
			switch(changeToState)
			{
			case ScreenChange.CreateAccountSelectScreen:
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
			p_gameController.changeState(ZoodleState.SET_BIRTHYEAR);
		}

		#if UNITY_ANDROID
		if( m_signUpCanvas != null )
		{
			if(null != m_emailText && m_emailText.color.a < 0.5f)
			{
				m_emailText.color= new Color(m_emailText.color.r,m_emailText.color.g,m_emailText.color.b,1.0f);
			}
			if(null != m_pwdText && m_pwdText.color.a < 0.5f)
			{
				m_pwdText.color= new Color(m_pwdText.color.r,m_pwdText.color.g,m_pwdText.color.b,1.0f);
			}
		}
		#endif
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( UIScreen.SET_ACCOUNT );
		p_gameController.getUI().removeScreen( UIScreen.LOADING_SPINNER );
	}
	
	
	//---------------- Private Implementation ----------------------

	private void _setupScreen( UIManager p_uiManager )
	{
		m_createAccountBackgroundCanvas = p_uiManager.findScreen( UIScreen.SPLASH_BACKGROUND );
		if( m_createAccountBackgroundCanvas == null )
            m_createAccountBackgroundCanvas = p_uiManager.createScreen( UIScreen.SPLASH_BACKGROUND, true, -1 );
		
		m_signUpCanvas = p_uiManager.createScreen( UIScreen.SET_ACCOUNT, true, 1 );

		m_backButton = m_signUpCanvas.getView("quitButton") as UIButton;
		m_backButton.addClickCallback (toBack);
		m_createAccount = m_signUpCanvas.getView("createAccountButton") as UIButton;
		m_createAccount.addClickCallback (toCreateChildrenScreen);
		m_dialog = m_signUpCanvas.getView ("dialog") as UIElement;
		m_errorTitle = m_dialog.getView ("dialogTitleText") as UILabel;
		m_errorContent = m_dialog.getView ("contentText") as UILabel;

		m_panel = m_signUpCanvas.getView("panel") as UIElement;
		m_panel.active = false;
		m_title = m_signUpCanvas.getView("topicText") as UILabel;
		m_account = m_panel.getView ("emailInput").gameObject.GetComponent<InputField>();
		m_password = m_panel.getView ("pwdInput").gameObject.GetComponent<InputField>();
		m_title.tweener.addAlphaTrack (0.0f, 1.0f, 0.5f,onTitleTweenFinish);
		m_closeDialogButton = m_dialog.getView ("closeMark") as UIButton;
		m_closeDialogButton.addClickCallback (closeDialog);
		#if UNITY_ANDROID
		m_emailText = m_signUpCanvas.getView("emailText") as UILabel;
		m_pwdText = m_signUpCanvas.getView("pwdText") as UILabel;
		#endif
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

	private void toCreateChildrenScreen( UIButton p_button )
	{
		if(IsMatch(ZoodlesConstants.EMAIL_REGULAR_STRING,m_account.text))
		{
			p_button.removeClickCallback( toCreateChildrenScreen );
			if( m_password.text.Length > 0 )
			{
				changeToState = ScreenChange.NextScreen;
				RequestQueue l_queue = new RequestQueue();
				l_queue.add(new ClientIdRequest());
				l_queue.add(new SignUpRequest(m_account.text, m_password.text,createAccountComplete));
				l_queue.request(RequestType.SEQUENCE);
			}
			else
			{
				invokeDialog("Create account failed","Please input your password.");
			}
		}
		else
		{
			invokeDialog("Create account failed","Please enter valid email address.");
		}
	}

	private void createAccountComplete(WWW p_response)
	{
		if(null == p_response.error)
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			if(l_data.ContainsKey("jsonResponse"))
			{
				invokeDialog("Email address is unavailable.", "Could not create account. Please use different email or log in with correct password.");
			}
			else
			{
				string l_secret = l_data.ContainsKey(ZoodlesConstants.PARAM_TOKEN) ? l_data[ZoodlesConstants.PARAM_TOKEN].ToString() : "";
				SessionHandler.getInstance().username = m_account.text;
				SessionHandler.getInstance().token.setToken(l_secret);
				
				RequestQueue l_queue = new RequestQueue();
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
			invokeDialog("Create account failed", "Please check your network and try again.");
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
		m_createAccount.addClickCallback (toCreateChildrenScreen);
	}

	//Private variables
	
	private UILabel		m_title;
	private UIElement 	m_panel;
	private UIElement	m_dialog;
	private UIButton 	m_backButton;
	private UIButton 	m_createAccount;
	private UILabel		m_errorTitle;
	private UILabel		m_errorContent;
	private UIButton 	m_closeDialogButton;

	private UICanvas    m_signUpCanvas;
	private UICanvas	m_createAccountBackgroundCanvas;

	private InputField 	m_account;
	private InputField 	m_password;

	private Hashtable 	m_postData;

	private UILabel 	m_emailText;
	private UILabel 	m_pwdText;

	private ScreenChange changeToState = ScreenChange.None;
}
