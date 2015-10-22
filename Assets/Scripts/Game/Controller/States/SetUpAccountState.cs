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
		NextScreen,
		SignInAccountScreen,
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

		game = p_gameController.game;
//
//		if (null == m_postData) 
//		{
//			m_postData = new Hashtable();
//		}
		//SwrveComponent.Instance.SDK.NamedEvent("SignUp.SET_UP_ACCOUNT_UI");
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );

		if (Application.internetReachability == NetworkReachability.NotReachable 
		    || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected())
		{
			// TODO: Sean
		}

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
				p_gameController.getUI().createScreen(UIScreen.LOADING_SPINNER_ELEPHANT,false,2);
				changeToState = ScreenChange.None;
				break;
			case ScreenChange.SignInAccountScreen:
				p_gameController.connectState(ZoodleState.SIGN_IN, ZoodleState.SET_UP_ACCOUNT);
				p_gameController.changeState(ZoodleState.SIGN_IN);
				changeToState = ScreenChange.None;
				break;
			default:
				changeToState = ScreenChange.None;
				break;
			}
		}

		if(SessionHandler.getInstance().token.isExist() && SessionHandler.getInstance().username != "")
		{
			p_gameController.changeState(ZoodleState.CONGRATURATION);
		}
	}
	
	public override void exit( GameController p_gameController )
	{

		m_createAccountButton.removeClickCallback( toCreateChildrenScreen );

		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( UIScreen.LOADING_SPINNER_ELEPHANT );
		p_gameController.getUI().removeScreenImmediately( UIScreen.SIGN_UP_AFTER_INPUT_CREDITCARD );


	}
	
	
	//---------------- Private Implementation ----------------------

	private void _setupScreen( UIManager p_uiManager )
	{
		m_createAccountBackgroundCanvas = p_uiManager.findScreen( UIScreen.SPLASH_BACKGROUND ) as SplashBackCanvas;
		if( m_createAccountBackgroundCanvas == null )
            m_createAccountBackgroundCanvas = p_uiManager.createScreen( UIScreen.SPLASH_BACKGROUND, true, -1 ) as SplashBackCanvas;

		m_createAccountBackgroundCanvas.setDown();

		m_signUpCanvas = p_uiManager.createScreen( UIScreen.SIGN_UP_AFTER_INPUT_CREDITCARD, true, 1 );

		m_backButton = m_signUpCanvas.getView("backButton") as UIButton;
		m_backButton.addClickCallback (toBack);
		m_createFreeAccountButton = m_signUpCanvas.getView("createFreeAccountButton") as UIButton;
		m_createFreeAccountButton.addClickCallback (toCreateChildrenScreen);
		m_createAccountButton = m_signUpCanvas.getView("createAccountButton") as UIButton;
		m_createAccountButton.addClickCallback (toCreateChildrenScreen);
//		m_getPremiumButton = m_signUpCanvas.getView("getPremiumButton") as UIButton;
//		m_getPremiumButton.addClickCallback (onClickGetPremium);

		// Sean: vzw
		m_GoogleAccountButton = m_signUpCanvas.getView ("buttonGoogleAccount") as UIButton;
		m_GoogleAccountButton.addClickCallback (toLoginWithGoogle);
		// end vzw

		//Honda
		m_SignInAccountButton = m_signUpCanvas.getView("signInAccountButton") as UIButton;
		m_SignInAccountButton.addClickCallback(onSignInAccountButtonClicked);
		UILabel signInText = m_SignInAccountButton.getView("signInAccountText") as UILabel;
		signInText.text = Localization.getString(Localization.TXT_STATE_9_SIGNIN_BTN);
		UILabel orLabel = m_signUpCanvas.getView("orText") as UILabel;
		UILabel signInLabel = m_signUpCanvas.getView("existingAccountText") as UILabel;
		orLabel.text = Localization.getString(Localization.TXT_STATE_9_OR);
		signInLabel.text = Localization.getString(Localization.TXT_STATE_9_SIGNIN_LABEL);
		//end

		//Kev
//		SplashBackCanvas splashCanvas = p_uiManager.findScreen (UIScreen.SPLASH_BACKGROUND) as SplashBackCanvas;
//		m_createAccountBackgroundCanvas.gameObject.SetActive(false);

		m_quitButton = m_signUpCanvas.getView("quitButton") as UIButton;
		m_quitButton.addClickCallback(onBackClicked);

		UILabel quitText = m_signUpCanvas.getView ("quitButton").getView("btnText") as UILabel;

		quitText.text = Localization.getString (Localization.TXT_BUTTON_QUIT);

//		UILabel quitFakeText = m_signUpCanvas.getView ("quitButtonFakeMove").getView("btnText") as UILabel;
//		quitFakeText.text = Localization.getString (Localization.TXT_BUTTON_QUIT);

		//end

		m_emailCheckImage = m_signUpCanvas.getView ("emailInputConfirm") as UIImage;
		m_passwordCheckImage = m_signUpCanvas.getView ("passwordInputConfirm") as UIImage;
		m_premiumLogoArea = m_signUpCanvas.getView ("premiumLogoArea") as UIElement;
		m_title			  = m_signUpCanvas.getView ("setupDialogTitleText") as UILabel;

		m_passwordCheckImage.active = false;
		m_emailCheckImage.active = false;
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
			m_createAccountButton.active = false;
			m_createFreeAccountButton.active = true;
		}
		else
		{
			m_title.text = Localization.getString(Localization.TXT_STATE_9_ACCOUNT);
			m_premiumLogoArea.active = true;
			m_createAccountButton.active = true;
			m_createFreeAccountButton.active = false;
		}
		//m_account.onValueChange.AddListener (onUsernameChange);
		m_account.onEndEdit.AddListener (onFinsihedEdit);
	//	#if !UNITY_ANDROID
		m_rePassword.onValueChange.AddListener (onRepasswordChange);
		m_password.onValueChange.AddListener (onPasswordChange);
	//	#endif	


		//TODO
		//localization for google account
		UILabel l_signInWithGoogle    = m_signUpCanvas.getView("textButtonGoogleAccount") as UILabel;

		l_signInWithGoogle.text       = Localization.getString(Localization.TXT_SCREEN_102_SIGN_WITH_GOOGLE);
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
			m_emailCheckImage.active = true;
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
						m_emailCheckImage.active = true;
						bool l_validate = (bool)l_bookData["validate"];
						if(l_validate)
							m_emailCheckImage.setSprite(Resources.Load<Sprite>("GUI/2048/common/icon/icon_check"));
						else
							m_emailCheckImage.setSprite(Resources.Load<Sprite>("GUI/2048/common/icon/icon_close_red"));
					}
				}
			}
		}
		else
		{
			m_emailCheckImage.active = true;
		}
	}

//	private void onClickGetPremium(UIButton p_button)
//	{
//		m_gameController.connectState (ZoodleState.SIGN_UP_UPSELL, int.Parse(m_gameController.stateName));
//		m_gameController.changeState (ZoodleState.SIGN_UP_UPSELL);
//	}

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
			m_passwordCheckImage.active = false;
		}
		else
		{
			m_passwordCheckImage.setSprite(Resources.Load<Sprite>("GUI/2048/common/icon/icon_close_red"));
			m_passwordCheckImage.active = true;
		}
	}

	private void onRepasswordChange(string p_value)
	{
		m_repasswordText = p_value;
		m_passwordCheckImage.active = true;
		if(confirmPasswordPasses())
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
		return  m_passwordText.Length > 3;
	}

	private bool confirmPasswordPasses()
	{
		return m_passwordText.Equals(m_repasswordText);
	}

	private bool emailPasses()
	{
		string l_email = m_account.text;
		return IsMatch(ZoodlesConstants.EMAIL_REGULAR_STRING,l_email);
	}

	// Sean: vzw
	private bool isLoaded = false;
	private void toLoginWithGoogle(UIButton p_button)
	{
		return;
		if (!isLoaded) {
			isLoaded = true;
			GooglePlayManager.ActionOAuthTokenLoaded += ActionOAuthTokenLoaded;
			GooglePlayManager.ActionAvailableDeviceAccountsLoaded += ActionAvailableDeviceAccountsLoaded;

			GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_CONNECTED, OnPlayerConnected);
			GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_DISCONNECTED, OnPlayerDisconnected);
			GooglePlayConnection.ActionConnectionResultReceived += ActionConnectionResultReceived;

		}
		GooglePlayManager.instance.RetrieveDeviceGoogleAccounts();
	}
	private void ActionConnectionResultReceived(GooglePlayConnectionResult result) {
		
		if (result.IsSuccess) {
			Debug.Log ("Connected!");
		} else {
			Debug.Log ("Cnnection failed with code: " + result.code.ToString ());
		}
//		SA_StatusBar.text = "ConnectionResul:  " + result.code.ToString ();
	}

	private void ActionOAuthTokenLoaded(string token) {

		AndroidDialog dialog = AndroidDialog.Create("Authentication token", token, GooglePlayManager.instance.loadedAuthToken, "Close");

//		AN_PoupsProxy.showMessage("Token Loaded", GooglePlayManager.instance.loadedAuthToken);
	}

	private void ActionAvailableDeviceAccountsLoaded(List<string> accounts) {
		string msg = "Device contains following google accounts:" + "\n";
		foreach(string acc in GooglePlayManager.instance.deviceGoogleAccountList) {
			msg += acc + "\n";
		} 
		
		AndroidDialog dialog = AndroidDialog.Create("Accounts Loaded", msg, "Sign With First one", "Do Nothing");
		dialog.OnComplete += SignDialogComplete;	
	}

	private void SignDialogComplete (AndroidDialogResult res) {
		if(res == AndroidDialogResult.YES) {


			GooglePlayConnection.instance.connect(GooglePlayManager.instance.deviceGoogleAccountList[0]);

			if (GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
				string scope = "oauth2:server:client_id:1093303544366-lavp8gpk436prjivuhfm828436q5nsp6.apps.googleusercontent.com:api_scope:https://www.googleapis.com/auth/plus.me https://www.googleapis.com/auth/userinfo.email";
				GooglePlayManager.instance.LoadToken(GooglePlayManager.instance.deviceGoogleAccountList[0], scope);
			}
		}

	}

	private void OnPlayerDisconnected() {

	}
	
	private void OnPlayerConnected() {
		string scope = "oauth2:server:client_id:1093303544366-lavp8gpk436prjivuhfm828436q5nsp6.apps.googleusercontent.com:api_scope:https://www.googleapis.com/auth/plus.me https://www.googleapis.com/auth/userinfo.email";
		GooglePlayManager.instance.LoadToken(GooglePlayManager.instance.deviceGoogleAccountList[0], scope);
		AndroidDialog dialog = AndroidDialog.Create("connected", "cool", "OK", "Close");
	}


	// end vzw

	//honda
	private void onSignInAccountButtonClicked(UIButton p_button)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable 
		    || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected()) //cynthia
		{
			m_gameController.getUI().createScreen(UIScreen.NO_INTERNET, false, 6);
			return;
		}

		changeToState = ScreenChange.SignInAccountScreen;
	}

	//end

	//Kev
	private void onBackClicked(UIButton p_button)
	{
		
		KidModeLockController.Instance.swith2DefaultLauncher ();
		KidMode.openDefaultLauncher ();

	}
	//end

	private void toCreateChildrenScreen( UIButton p_button )
	{
		if (Application.internetReachability == NetworkReachability.NotReachable 
		    || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected()) //cynthia
		{
			m_gameController.getUI().createScreen(UIScreen.NO_INTERNET, false, 6);
			return;
		}

		bool l_emailPasses = emailPasses();
		bool l_passwordPasses = passwordPasses();

		if(l_emailPasses && l_passwordPasses)
		{
//			p_button.removeClickCallback( toCreateChildrenScreen );

			popupCheckParentBirth();

			//move to popupCheckParentBirth()
//			changeToState = ScreenChange.NextScreen;
//			RequestQueue l_queue = new RequestQueue();
////				l_queue.add(new ClientIdRequest());
//			l_queue.add(new SignUpRequest(m_account.text, m_password.text,createAccountComplete));
//			l_queue.request(RequestType.SEQUENCE);
//			//SwrveComponent.Instance.SDK.NamedEvent("SignUp.end");
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



	private void popupCheckParentBirth()
	{

		if(m_agePopPanelObject == null){

			m_agePopPanelObject = m_gameController.getUI().createScreen(UIScreen.PARENT_BIRTH_CHECK, false, 7).gameObject;	

			GameObject[] findObjects = GameObject.FindGameObjectsWithTag("CheckParentBirthTag");
			Debug.Log(" findObjects " + findObjects.Length);

			CheckParentBirthPopup parentBirthPop = GameObject.FindWithTag("CheckParentBirthTag").GetComponent<CheckParentBirthPopup>() as CheckParentBirthPopup;
			if (parentBirthPop != null)
				parentBirthPop.onClick += onCreateAccountClicked;

		}

	}

	private void onCreateAccountClicked(bool successful)
	{

		CheckParentBirthPopup parentBirthPop = GameObject.FindWithTag("CheckParentBirthTag").GetComponent<CheckParentBirthPopup>() as CheckParentBirthPopup;

		parentBirthPop.onClick -= onCreateAccountClicked;

		if (successful) 
		{
			changeToState = ScreenChange.NextScreen;
			RequestQueue l_queue = new RequestQueue ();
//			l_queue.add(new ClientIdRequest());
			l_queue.add (new SignUpRequest (m_account.text, m_password.text, createAccountComplete));
			l_queue.request (RequestType.SEQUENCE);
			//SwrveComponent.Instance.SDK.NamedEvent ("SignUp.end");
		}
		else 
		{
			m_account.text = "";
			m_password.text = "";
			m_rePassword.text = "";
			m_passwordCheckImage.active = false;
			m_emailCheckImage.active = false;

			if(m_IsCreateFreeAccount && SessionHandler.getInstance().renewalPeriod == 0)
			{
				m_createFreeAccountButton.addClickCallback (toCreateChildrenScreen);
			}
			else
			{
				m_createAccountButton.addClickCallback (toCreateChildrenScreen);
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
				game.IsFirstLaunch = 1;
				game.IsLogin = 1;

				string l_secret = l_data.ContainsKey(ZoodlesConstants.PARAM_TOKEN) ? l_data[ZoodlesConstants.PARAM_TOKEN].ToString() : "";
				bool l_premium = l_data.ContainsKey("premium") && (bool)l_data["premium"];
				bool l_current = l_data.ContainsKey("is_current") && null != l_data["is_current"] && (bool)l_data["is_current"];
				int l_vpc = l_data.ContainsKey("vpc_level") ? int.Parse(l_data["vpc_level"].ToString()) : 0;
				bool l_tried = l_data.ContainsKey ("is_tried") && null != l_data ["is_tried"] ? (bool)l_data ["is_tried"] : true;
				SessionHandler.getInstance().token.setToken(l_secret, l_premium,l_tried,l_current,l_vpc);
				
				SessionHandler.getInstance().username = m_account.text;
				CrittercismAndroid.SetUsername(m_account.text);

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
			invokeDialog(Localization.getString(Localization.TXT_STATE_9_FAIL), Localization.getString(Localization.ERROR_MESSAGE_ERROR_TEXT));
		}
	}

	private void invokeDialog(string p_errorTitle, string p_errorContent)
	{
		m_signUpCanvas.active = true;
		if(null != m_gameController.getUI().findScreen(UIScreen.LOADING_SPINNER_ELEPHANT))
			m_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);

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

	// Sean: vzw
	private UIButton    m_GoogleAccountButton;
	// end vzw

	// Honda
	private UIButton  	m_SignInAccountButton;
	private Game 		game;
	// end

	private UICanvas    m_signUpCanvas;
	private SplashBackCanvas	m_createAccountBackgroundCanvas;

	private InputField 	m_account;
	private InputField 	m_password;
	private InputField 	m_rePassword;

	private UIButton 	m_createAccountButton;
	private UIButton 	m_quitButton;
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

	[SerializeField]
	private GameObject m_agePopPanelObject;
}
