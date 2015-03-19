using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class SignInState : GameState 
{
	enum SubState
	{
		NONE,
		LOADING,
		GO_PREVIOUS,
		GO_PROFILE
	}

	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);
		m_controller = p_gameController;
		m_subState = SubState.NONE;
		m_loginSuccess = false;
		_setupScreen(p_gameController.getUI());

		GAUtil.logScreen("SignInScreen");
	}
	
	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		switch (m_subState)
		{
		case SubState.GO_PREVIOUS:
			p_gameController.changeState(ZoodleState.CREATE_ACCOUNT_SELECTION);
			m_subState = SubState.NONE;
			break;
		case SubState.LOADING:
			m_signInCanvas.active = false;
			p_gameController.getUI().createScreen(UIScreen.LOADING_SPINNER, false, 2);
			m_subState = SubState.NONE;
			break;
		}

		if (m_loginSuccess)
		{
			if (SessionHandler.getInstance().hasPin)
			{
				if (LocalSetting.find("User").getBool("UserTry",true))
				{
					if (null != SessionHandler.getInstance().kidList && SessionHandler.getInstance().kidList.Count > 0)
					{
						p_gameController.changeState(ZoodleState.PROFILE_SELECTION);
					}
					else
					{
						m_gameController.connectState(ZoodleState.CREATE_CHILD,int.Parse(m_gameController.stateName));
						p_gameController.changeState(ZoodleState.CREATE_CHILD);
					}
				}
				else
				{
					p_gameController.changeState(ZoodleState.SIGN_UP_UPSELL);
				}
			}
			else
			{
				p_gameController.changeState(ZoodleState.SET_BIRTHYEAR);
			}
		}
		#if UNITY_ANDROID
		if (m_signInCanvas != null)
		{
			if (null != m_addressText && m_addressText.color.a < 0.5f)
			{
				m_addressText.color= new Color(m_addressText.color.r, m_addressText.color.g, m_addressText.color.b, 1.0f);
			}
			if (null != m_passwordText && m_passwordText.color.a < 0.5f)
			{
				m_passwordText.color= new Color(m_passwordText.color.r, m_passwordText.color.g, m_passwordText.color.b, 1.0f);
			}
		}
		#endif
	}
	
	public override void exit(GameController p_gameController)
	{
		base.exit(p_gameController);

		p_gameController.getUI().removeScreen(UIScreen.SIGN_IN);
		p_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER);
	}
	
	//---------------- Private Implementation ----------------------
	
	private void _setupScreen(UIManager p_uiManager)
	{
		m_signInButtonBackgroundCanvas = p_uiManager.findScreen(UIScreen.SPLASH_BACKGROUND) as SplashBackCanvas;
		if (m_signInButtonBackgroundCanvas == null)
		{
			m_signInButtonBackgroundCanvas = p_uiManager.createScreen(UIScreen.SPLASH_BACKGROUND, true, -1) as SplashBackCanvas;
			m_signInButtonBackgroundCanvas.setDown();
		}
		
		m_signInCanvas = p_uiManager.createScreen(UIScreen.SIGN_IN, true, 1);
		
		m_backButton = m_signInCanvas.getView("exitButton") as UIButton;
		m_backButton.addClickCallback (toBack);
		m_signInButton = m_signInCanvas.getView("signInButton") as UIButton;
		m_signInButton.addClickCallback(toCreateChildrenScreen);
		m_dialog = m_signInCanvas.getView ("dialog") as UIElement;
		m_errorTitle = m_dialog.getView ("dialogTitleText") as UILabel;
		m_errorContent = m_dialog.getView ("contentText") as UILabel;
		m_panel = m_signInCanvas.getView("panel") as UIElement;
		m_panel.active = false;
		m_title = m_signInCanvas.getView("topicText") as UILabel;
		m_account = m_panel.getView("emailInput").gameObject.GetComponent<InputField>();
		m_password = m_panel.getView("pwdInput").gameObject.GetComponent<InputField>();
		m_title.tweener.addAlphaTrack(0.0f, 1.0f, 0.5f,onTitleTweenFinish);
		m_closeDialogButton = m_dialog.getView ("closeMark") as UIButton;
		m_closeDialogButton.addClickCallback (closeDialog);

		#if UNITY_ANDROID
		m_addressText = m_signInCanvas.getView("emailAddressText") as UILabel;
		m_passwordText = m_signInCanvas.getView("passwordText") as UILabel;
		#endif
	}

	private void invokeDialog(string p_errorTitle, string p_errorContent)
	{
		m_signInCanvas.active = true;

		m_errorTitle.text = p_errorTitle;
		m_errorContent.text = p_errorContent;
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_dialog.transform.localPosition );
		l_pointListOut.Add( m_dialog.transform.localPosition - new Vector3( 0, 582, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f );
		m_controller.getUI().removeScreen(UIScreen.LOADING_SPINNER);
	}

	private void closeDialog(UIButton p_button)
	{
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_dialog.transform.localPosition );
		l_pointListOut.Add( m_dialog.transform.localPosition + new Vector3( 0, 582, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f );
		m_signInButton.addClickCallback (toCreateChildrenScreen);
	}

	private void getClientIdComplete(WWW p_response)
	{
		if(p_response.error == null)
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;

			SessionHandler.getInstance ().clientId = l_data.ContainsKey("id") ? double.Parse(l_data["id"].ToString()) : -1;
//			SessionHandler.getInstance ().clientId = 1203;
		}
		else
		{
			m_queue.reset();
			invokeDialog("Login Failed","Please retry your password.");
		}
	}

	private void loginRequestComplete(WWW p_response)
	{
		if(null == p_response.error)
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			if(l_data.ContainsKey("jsonResponse"))
			{
				invokeDialog("Login Failed","Please retry your password.");
			}
			else
			{
				string l_secret = l_data.ContainsKey(ZoodlesConstants.PARAM_TOKEN) ? l_data[ZoodlesConstants.PARAM_TOKEN].ToString() : "";
				//bool l_premium = l_data.ContainsKey("premium") && "True".Equals(l_data["premium"].ToString());
				bool l_premium = l_data.ContainsKey("premium") && (bool)l_data["premium"];
				bool l_current = l_data.ContainsKey("is_current") && (bool)l_data["is_current"];
				int l_vpc = l_data.ContainsKey("vpc_level") ? int.Parse(l_data["vpc_level"].ToString()) : 0;
				//on trial
				bool l_tried = l_data.ContainsKey ("is_tried") && null != l_data ["is_tried"] ? (bool)l_data ["is_tried"] : true;
				SessionHandler.getInstance().username = m_account.text;
				SessionHandler.getInstance().token.setToken(l_secret, l_premium,l_tried,l_current,l_vpc);
				SessionHandler.getInstance().hasPin = l_data.ContainsKey("has_pin") && "true".Equals(l_data["has_pin"].ToString());
				SessionHandler.getInstance().pin = l_data.ContainsKey("pin")&& null != l_data["pin"] ? int.Parse(l_data["pin"].ToString()) : -1;

				if (m_queue == null)
					m_queue = new RequestQueue();
				m_queue.reset();
				m_queue.add(new GetUserSettingRequest());
				LocalSetting l_setting = LocalSetting.find("ServerSetting");
				if (!l_setting.hasKey(ZoodlesConstants.ZPS_LEVEL))
					m_queue.add(new GetLevelsInfoRequest());
				if (!l_setting.hasKey(ZoodlesConstants.EXPERIENCE_POINTS))
					m_queue.add(new GetExperiencePointsInfoRequest());
				if (!l_setting.hasKey(ZoodlesConstants.CATEGORIES))
					m_queue.add(new GetCategoriesInfoRequest());
				if (!l_setting.hasKey(ZoodlesConstants.TAGS))
					m_queue.add(new GetTagsInfoRequest());
				if (!l_setting.hasKey(ZoodlesConstants.SUBJECTS))
					m_queue.add(new GetSubjectsInfoRequest());
				m_queue.add(new GetKidListRequest(allRequestComplete));
				m_queue.request(RequestType.SEQUENCE);
			}
		}
		else
		{
			invokeDialog("Login Failed","Please retry your password.");
		}

	}
	private void allRequestComplete(WWW p_response)
	{
		List<Kid> l_kidList = new List<Kid>();
		ArrayList l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as ArrayList;
		foreach(object o in l_data)
		{
			Kid l_kid = new Kid( o as Hashtable );
			l_kid.requestPhoto();
			l_kidList.Add( l_kid );
		}
		
		SessionHandler.getInstance().kidList = l_kidList;
		if (l_kidList.Count > 0)
			SessionHandler.getInstance().currentKid = l_kidList[0];

		m_loginSuccess = true;
	}

	private void onTitleTweenFinish(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		m_panel.tweener.addAlphaTrack(0.0f, 1.0f, 0.2f);
	}
	
	private void toBack(UIButton p_button)
	{ 
		m_subState = SubState.GO_PREVIOUS;
	}

	private bool IsMatch(string p_pattern, string p_input)
	{
		if (p_input == null || p_input == "") return false;
		Regex regex = new Regex(p_pattern);
		return regex.IsMatch(p_input);
	}
	
	private void toCreateChildrenScreen(UIButton p_button)
	{
		p_button.removeClickCallback(toCreateChildrenScreen);
		if (!IsMatch (ZoodlesConstants.EMAIL_REGULAR_STRING,m_account.text)) 
		{
			invokeDialog("Login failed","Please enter valid email address");
			return;
		}
		m_subState = SubState.LOADING;
		if (m_queue == null)
			m_queue = new RequestQueue();
		m_queue.reset ();
		m_queue.add(new ClientIdRequest(getClientIdComplete));
		m_queue.add(new LoginRequest(m_account.text,m_password.text,loginRequestComplete));
		m_queue.request(RequestType.SEQUENCE);
	}
	
	private UILabel		m_title;
	private UIElement 	m_panel;
	private UIElement	m_dialog;
	private UILabel		m_errorTitle;
	private UILabel		m_errorContent;
	private UIButton 	m_backButton;
	private UIButton 	m_signInButton;
	private UIButton 	m_closeDialogButton;
	
	private UICanvas    m_signInCanvas;
	private SplashBackCanvas	m_signInButtonBackgroundCanvas;
	
	private InputField  m_account;
	private InputField 	m_password;
	
	private Hashtable 	m_postData;
	private UILabel 	m_addressText;
	private UILabel 	m_passwordText;
	private bool 		m_loginSuccess = false;
	private GameController m_controller;

	private SubState	m_subState = SubState.NONE;
	private RequestQueue m_queue = null;
}
