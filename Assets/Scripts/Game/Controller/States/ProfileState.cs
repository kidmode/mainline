using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ProfileState : GameState
{
	//consts
	private const float LOADING_WEIGHT 	= 1;
	private const float LOADING_START 	= 80;
	
	//--------------------Public Interface -----------------------
	
	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);

		//Kev
		UIManager l_ui = p_gameController.getUI();
		SplashBackCanvas splashCanvas = l_ui.findScreen (UIScreen.SPLASH_BACKGROUND) as SplashBackCanvas;

		if(splashCanvas != null)
			splashCanvas.gameObject.SetActive(true);
		//Kev

		TutorialController.Instance.showTutorial(TutorialSequenceName.MAIN_PROCESS);
		
		m_requestQueue = new RequestQueue ();

		m_gotoEntrance = false;
		m_gotoCREATE_CHILD_NEW = false;
		m_gotoDashBoard = false;

//		KidMode.setKidsModeActive(true);	
		KidModeLockController.Instance.swith2KidMode();
		_setupScreen(p_gameController.getUI());

		SoundManager.getInstance().play("Jungle Jam Loopable", 0, 1, "", null, true);

		GAUtil.logScreen("ProfileScreen");

		SessionHandler.getInstance ().settingCache.active = false;
		SessionHandler.getInstance ().initSettingCache ();
		Debug.Log("Profile set volume data from session handler to setting cache");

		//check timer if kid is back from map
		TimerController.Instance.resetKidTimer();

		if (p_gameController.game.delayedParentDashboard)
		{
			p_gameController.changeState(ZoodleState.GOTO_PARENT_DASHBOARD);
			p_gameController.game.delayedParentDashboard = false;
		}
	}

	public override void update(GameController p_gameController,  int p_time)
	{
		base.update(p_gameController, p_time);
		
		if (Input.anyKeyDown && m_isIntro)
			m_profileScreen.tweener.stop(true);

		if (m_gotoEntrance)
		{
			KidMode.broadcastCurrentMode("KidMode");
			p_gameController.changeState(ZoodleState.LOADING_ENTRANCE);
			m_profileScreen.tweener.addAlphaTrack(1.0f, 0.0f, 0.3f, onFadeFinish);
			m_gotoEntrance = false;
		}

		if (m_gotoCREATE_CHILD_NEW)
		{

			PlayerPrefs.SetInt("newChildInKidMode", 1);

			SessionHandler.getInstance().CreateChild = true;
			m_gameController.connectState(ZoodleState.CREATE_CHILD_NEW,int.Parse(m_gameController.stateName));
			p_gameController.changeState(ZoodleState.CREATE_CHILD_NEW);
			m_profileScreen.tweener.addAlphaTrack(1.0f, 0.0f, 0.3f, onFadeFinish);
			m_gotoCREATE_CHILD_NEW = false;
		}
	
		if (m_gotoDashBoard) 
		{
			p_gameController.changeState(ZoodleState.BIRTHYEAR);
			m_profileScreen.tweener.addAlphaTrack(1.0f, 0.0f, 0.3f, onFadeFinish);
			m_gotoDashBoard = false;
		}
	}
	
	public override void exit(GameController p_gameController)
	{
		m_uiManager.removeScreen(m_profileScreen);
		m_uiManager.removeScreen(m_addFriendCanvas);
		m_uiManager.removeScreen(m_tellFriendCanvas);
		m_uiManager.removeScreenImmediately(UIScreen.SPLASH_BACKGROUND);
		base.exit(p_gameController);
	}

	public override bool handleMessage(GameController p_gameController, int p_type, string p_string)
	{
		if (p_type == 1)
		{
			UICanvas l_screen = m_uiManager.findScreen(UIScreen.LEGAL_CONTENT);
			if (l_screen != null)
			{
				UniWebView l_webView = l_screen.gameObject.GetComponentInChildren<UniWebView>();
				l_webView.Reload();
			}
		}

		return true;
	}

	private void _setupScreen(UIManager p_uiManager)
	{
		m_uiManager = p_uiManager;

		// Remove map if it is exist
		p_uiManager.removeScreen(UIScreen.MAP);	

		m_isIntro = true;
		m_addFriendCanvas  = p_uiManager.createScreen (UIScreen.ADD_FRIEND,  false, 7);
		m_tellFriendCanvas = p_uiManager.createScreen (UIScreen.TELL_FRIEND, false, 6);
		m_backScreen = p_uiManager.findScreen(UIScreen.SPLASH_BACKGROUND);
        if (m_backScreen == null)
        {
			m_backScreen = p_uiManager.createScreen(UIScreen.SPLASH_BACKGROUND,true, -1);
            (m_backScreen as SplashBackCanvas).setDown();
        }

		m_profileScreen = p_uiManager.createScreen(UIScreen.PROFILE_SELECTION, false);
		m_profileScreen.enterTransitionEvent += onTransitionEnter;
		m_profileScreen.exitTransitionEvent += onTransitionExit;
		m_profileScreen.tweener.addAlphaTrack(0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED);

		m_closeTellFriendButton = m_tellFriendCanvas.getView ("exitButton") as UIButton;
		m_closeTellFriendButton.addClickCallback (onCloseDialog);
		m_quitButton = m_profileScreen.getView("quitButton") as UIButton;
		m_quitButton.addClickCallback(onBackClicked);

		List<Vector3> l_quitPosList = new List<Vector3>();
		l_quitPosList.Add(m_quitButton.transform.localPosition + new Vector3(100, 0, 0));
		l_quitPosList.Add(m_quitButton.transform.localPosition);
		m_quitButton.tweener.addPositionTrack(l_quitPosList, 1.0f);

		UILabel l_titleLabel = m_profileScreen.getView("introLabel") as UILabel;

		List<Vector3> l_titlePosList = new List<Vector3>();
		l_titlePosList.Add(l_titleLabel.transform.localPosition + new Vector3(-200, 400, 0)); 
		l_titlePosList.Add(l_titleLabel.transform.localPosition + new Vector3(  20,  30, 0));
		l_titlePosList.Add(l_titleLabel.transform.localPosition + new Vector3( 100,  50, 0));
		l_titlePosList.Add(l_titleLabel.transform.localPosition + new Vector3( -20, -10, 0));
		l_titlePosList.Add(l_titleLabel.transform.localPosition);
         
        l_titleLabel.tweener.addPositionTrack(l_titlePosList, 1.0f, onTitleIntroFinished, Tweener.Style.QuadOut);
        l_titleLabel.tweener.addRotationTrack(20, 0.0f, 1.0f, onIntroOver, Tweener.Style.QuadInOut);

		List<Kid> l_kidList = SessionHandler.getInstance ().kidList;
		int l_kidCount = l_kidList.Count;
		UISwipeList l_swipe = m_profileScreen.getView(l_kidCount == 1 ? "oneProfileSwipeList" : "profileSwipeList") as UISwipeList;
		l_swipe.addClickListener("Prototype", onProfileSelected);

		m_parentDashboardButton = m_profileScreen.getView("parentDashButton") as UIButton;
		List<Vector3> l_dashboardList = new List<Vector3>();
		l_dashboardList.Add(m_parentDashboardButton.transform.localPosition + new Vector3(-100,0,0));
		l_dashboardList.Add(m_parentDashboardButton.transform.localPosition);
		m_parentDashboardButton.tweener.addPositionTrack(l_dashboardList, 1.0f, null, Tweener.Style.QuadOut);
		m_parentDashboardButton.addClickCallback(onGotoDashboard);
		
        m_facebookButton = m_profileScreen.getView("facebookButton") as UIButton;
		m_facebookButton.addClickCallback(_onFacebookButtonClick);

        m_emailInviteButton = m_profileScreen.getView("inviteButton") as UIButton;
		m_emailInviteButton.addClickCallback(_onInviteButtonClick);

		m_createChildButton = m_profileScreen.getView("addChildButton") as UIButton;
		m_createChildButton.addClickCallback(onCreateChild);

		m_tryFreeButton = m_profileScreen.getView("tryFreeButton") as UIButton;
		m_tryFreeButton.addClickCallback(toSignInUpsell);

		UIElement l_tryFreeArea = m_profileScreen.getView("tryFreeButtonArea") as UIElement;
		if (SessionHandler.getInstance().token.isTried() || SessionHandler.getInstance().token.isPremium())
			l_tryFreeArea.active = false;

		UIButton l_termsButton = m_profileScreen.getView("termsOfService") as UIButton;
		l_termsButton.addClickCallback(_onTermsButtonClick);

		UIButton l_policyButton = m_profileScreen.getView("privacy Policy") as UIButton;
		l_policyButton.addClickCallback(_onPolicyButtonClick);

		m_addFriendButton = m_tellFriendCanvas.getView ("addButton") as UIButton;
		m_addFriendButton.addClickCallback (showAddFriendCanvas);
		m_titleText = m_tellFriendCanvas.getView ("titleText") as UILabel;
		m_titleText.active = false;

		m_closeAddFriendScreenButton = m_addFriendCanvas.getView ("exitButton") as UIButton;
		m_closeAddFriendScreenButton.addClickCallback (onCloseAddFriendDialog);

		m_addFriendAddressButton = m_addFriendCanvas.getView ("AddFriendButton") as UIButton;
		m_addFriendAddressButton.addClickCallback (onAddFriendDialog);

		m_errorLabel = m_tellFriendCanvas.getView ("errorLabel") as UILabel;

		m_emailInput = m_addFriendCanvas.getView("emailInput").gameObject.GetComponent<InputField>();
		m_fromInput = m_tellFriendCanvas.getView("fromInput").gameObject.GetComponent<InputField>();
		m_fromInput.text = SessionHandler.getInstance ().username;
		m_toInput = m_tellFriendCanvas.getView("toInput").gameObject.GetComponent<InputField>();
		m_optionInput = m_tellFriendCanvas.getView("optionalInput").gameObject.GetComponent<InputField>();

		m_sendEmailButton = m_tellFriendCanvas.getView ("sendEmailButton") as UIButton;
		m_sendEmailButton.addClickCallback (sendTellFriendEmail);

		m_dialog = m_tellFriendCanvas.getView ("dialog");
		m_confirmDialog = m_tellFriendCanvas.getView ("confirmDialog");
		m_dialog.active = true;
		m_confirmDialog.active = false;
		m_confirmContentText = m_confirmDialog.getView ("contentText") as UILabel;
		m_closeConfirmButton = m_confirmDialog.getView ("exitButton") as UIButton;
		m_closeDialogButton	 = m_confirmDialog.getView ("closeButton") as UIButton;
		m_closeConfirmButton.addClickCallback (onCloseDialog);
		m_closeDialogButton.addClickCallback (onCloseButtonClick);
	}

	private void toSignInUpsell(UIButton p_button)
	{
		m_gameController.connectState (ZoodleState.SIGN_IN_UPSELL, int.Parse(m_gameController.stateName));
		m_gameController.changeState (ZoodleState.SIGN_IN_UPSELL);
	}

	private void sendTellFriendEmail(UIButton p_button)
	{
		string l_from = m_fromInput.text;
		string l_to = m_toInput.text;
		string l_option = m_optionInput.text;
		if (string.Empty.Equals (l_from) || string.Empty.Equals (l_to)) 
		{
			m_dialog.active = false;
			m_confirmDialog.active = true;
			m_closeConfirmButton.active = false;
			m_closeDialogButton.active = true;
			m_titleText.text = Localization.getString(Localization.TXT_STATE_11_FAIL);
			m_titleText.active = true;
			m_confirmContentText.text = Localization.getString(Localization.TXT_STATE_1_EMPTY);
			m_toInput.text = string.Empty;
			m_optionInput.text = string.Empty;
			m_errorLabel.text = string.Empty;
		}
		else
		{
			m_requestQueue.reset ();
			m_requestQueue.add (new SendTellFriendRequest(l_from,l_to,l_option,sendTellFriendRequestComplete));
			m_requestQueue.request();
			m_dialog.active = false;
			m_confirmDialog.active = true;
			m_closeConfirmButton.active = false;
		}
	}

	private void sendTellFriendRequestComplete(WWW p_response)
	{

		m_toInput.text = "";

		m_closeConfirmButton.active = true;
		if (null == p_response.error) 
		{
			Hashtable l_jsonResponse = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			if(l_jsonResponse.ContainsKey("jsonResponse"))
			{
				Hashtable l_response = l_jsonResponse["jsonResponse"] as Hashtable;
				if(l_response.ContainsKey("response"))
				{
					ArrayList l_emailData = l_response["response"] as ArrayList;
					if(null == l_emailData || l_emailData.Count <= 0)
					{	
						m_titleText.text = Localization.getString(Localization.TXT_94_LABEL_SUCCESS);
						m_titleText.active = true;
						m_confirmContentText.text = Localization.getString(Localization.TXT_STATE_1_THANK);
					}
					else
					{
						string l_failEmail = string.Empty;
						int l_count = l_emailData.Count;
						for(int l_i = 0; l_i < l_count; l_i++)
						{
							l_failEmail = l_failEmail + " " + l_emailData[l_i];
						}
						m_confirmContentText.text = l_failEmail + Localization.getString(Localization.TXT_STATE_1_REGISTERED);;
					}
				}
				else
					m_confirmContentText.text = Localization.getString(Localization.TXT_STATE_1_FAIL);
			}
			else
				m_confirmContentText.text = Localization.getString(Localization.TXT_STATE_1_FAIL);

		}
		else
			m_confirmContentText.text = Localization.getString(Localization.TXT_STATE_1_FAIL);
	}

	private void showAddFriendCanvas(UIButton p_button)
	{
		m_gameController.getUI ().changeScreen (UIScreen.TELL_FRIEND,false);
		m_gameController.getUI ().changeScreen (UIScreen.ADD_FRIEND,true);
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_addFriendCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_newPanel.tweener.addPositionTrack(l_pointListIn, 0.0f);
	}

	private void onTransitionEnter(UICanvas p_canvas)
	{
		p_canvas.graphicRaycaster.enabled = false;
	}

	private void onTransitionExit(UICanvas p_canvas)
	{
		p_canvas.graphicRaycaster.enabled = true;
	}
	
	private void onTitleIntroFinished(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		List<Vector3> l_idlePosList = new List<Vector3>();
		l_idlePosList.Add(p_element.transform.localPosition);
		l_idlePosList.Add(p_element.transform.localPosition + new Vector3(20,  10, 0));
		l_idlePosList.Add(p_element.transform.localPosition + new Vector3(10, -10, 0));
		l_idlePosList.Add(p_element.transform.localPosition + new Vector3(-5, - 5, 0));
		l_idlePosList.Add(p_element.transform.localPosition + new Vector3(10,   5, 0));
		l_idlePosList.Add(p_element.transform.localPosition + new Vector3(-8,   5, 0));
		l_idlePosList.Add(p_element.transform.localPosition);

		p_element.tweener.addPositionTrack(l_idlePosList, 10.0f, null, Tweener.Style.Standard, true);
	}

	private void onCloseButtonClick( UIButton p_button )
	{
		p_button.removeAllCallbacks();
		m_closeDialogButton.active = false;
		m_dialog.active = true;
		m_confirmDialog.active = false;
		m_titleText.active = false;
		m_confirmContentText.text = Localization.getString(Localization.TXT_STATE_1_SENDING);
		p_button.addClickCallback (onCloseButtonClick);
	}

	private void onCloseDialog(UIButton p_button)
	{
		m_gameController.getUI ().changeScreen (UIScreen.TELL_FRIEND,false);
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_newPanel = m_tellFriendCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_newPanel.transform.localPosition );
		l_pointListOut.Add( l_newPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_newPanel.tweener.addPositionTrack(l_pointListOut, 0.0f);
		m_dialog.active = true;
		m_confirmDialog.active = false;
		m_titleText.active = false;
		m_confirmContentText.text = Localization.getString(Localization.TXT_STATE_1_SENDING);
		m_fromInput.text = SessionHandler.getInstance ().username;
		m_toInput.text = string.Empty;
		m_optionInput.text = string.Empty;
	}

	private void onCloseAddFriendDialog(UIButton p_button)
	{
		m_gameController.getUI ().changeScreen (UIScreen.ADD_FRIEND,false);
		m_gameController.getUI ().changeScreen (UIScreen.TELL_FRIEND,true);
		m_emailInput.text = string.Empty;
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_newPanel = m_addFriendCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_newPanel.transform.localPosition );
		l_pointListOut.Add( l_newPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_newPanel.tweener.addPositionTrack(l_pointListOut, 0.0f);
	}

	private void onAddFriendDialog(UIButton p_button)
	{
		if (null != m_emailInput)
		{
			string l_email = m_emailInput.text;
			if(IsMatch(ZoodlesConstants.EMAIL_REGULAR_STRING,l_email))
			{
				if(l_email.Equals(SessionHandler.getInstance().username))
				{
					m_errorLabel.text = Localization.getString(Localization.TXT_STATE_1_SELF);
					m_errorLabel.active = true;
					onCloseAddFriendDialog (p_button);
				}
				else
				{
					m_errorLabel.active = false;
					if(null != m_toInput)
					{
						string l_emailList = m_toInput.text;
						m_toInput.text = string.Empty.Equals(l_emailList) ? l_email : l_emailList + "," + l_email;
					}
				}
			}
			else
			{
				m_errorLabel.text = Localization.getString(Localization.TXT_STATE_1_FORMAT);
				m_errorLabel.active = true;
			}
		}
		onCloseAddFriendDialog (p_button);
	}

	private bool IsMatch(string p_pattern, string p_input)
	{
		if (p_input == null || p_input == "") return false;
		Regex regex = new Regex(p_pattern);
		return regex.IsMatch(p_input);
	}

	private void onIntroOver(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		m_isIntro = false;
	}

	private void onFadeFinish(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		UICanvas p_canvas = p_element as UICanvas;
		p_canvas.isTransitioning = false;
	}
	
	private void onProfileSelected(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		Kid l_kid = p_data as Kid;
		DebugUtils.Assert(l_kid != null);

		if (l_kid.gems != ProfileInfo.ADD_PROFILE_CODE)
		{
			if (l_kid.timeLeft == 0)
				return;
			//**To-do** Set current profile Data
			m_gameController.game.user.currentKid = l_kid;
			SessionHandler.getInstance().currentKid = l_kid;
			Debug.Log("time limits: " + l_kid.timeLimits);
            
			m_gotoEntrance = true;

			SessionHandler.SaveCurrentKid(l_kid.id);
		}
		else
		{
			//**To-do**  Create a new profile
			m_profileScreen.tweener.addAlphaTrack(1.0f, 0.0f, 0.3f, onFadeFinish);
			m_backScreen.tweener.addAlphaTrack(1.0f, 0.0f, 0.3f, onFadeFinish);
			m_gameController.changeState(ZoodleState.CREATE_CHILD_NEW);
		}
	}

	private void onBackClicked(UIButton p_button)
	{

		KidModeLockController.Instance.swith2DefaultLauncher ();
		KidMode.openDefaultLauncher ();

//		if(SessionHandler.getInstance().childLockSwitch)
//		{
//			m_gameController.connectState (ZoodleState.BIRTHYEAR,int.Parse(m_gameController.stateName));
//			m_gameController.changeState (ZoodleState.BIRTHYEAR);
//		}
//		else
//		{
////			KidMode.setKidsModeActive(false);	
//			KidModeLockController.Instance.swith2DParentMode();
//			PlayerPrefs.Save();
//			Application.Quit();
//		}

    }	

	private void onCreateChild(UIButton p_button)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable || KidMode.isAirplaneModeOn())
		{
			Game game = GameObject.FindWithTag("GameController").GetComponent<Game>();
			game.gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
		}
		else
		{
			m_gotoCREATE_CHILD_NEW = true;
		}
	}

	private void onGotoDashboard(UIButton p_button)
	{
		SessionHandler l_sessionHandler = SessionHandler.getInstance();
		Token l_token = l_sessionHandler.token;
		if (l_token.isPremium())
		{
//			if (SessionHandler.getInstance().childLockSwitch)
//			{
//				if (0 != SessionHandler.getInstance().pin) 
//				{
//					m_gameController.changeState(ZoodleState.BIRTHYEAR);
//				}
//				else
//				{
//					setErrorMessage(m_gameController, Localization.getString(Localization.TXT_STATE_1_ERROR), Localization.getString(Localization.TXT_STATE_1_PIN));
//				}
//			}
//			else
			{
				m_uiManager.removeScreenImmediately(UIScreen.SPLASH_BACKGROUND);
				m_gameController.changeState(ZoodleState.CONTROL_APP);
			}
		}
		else
		{
			m_gameController.changeState(ZoodleState.UPSELL_SPLASH);
		}
		m_profileScreen.tweener.addAlphaTrack(1.0f, 0.0f, 0.3f, onFadeFinish);
	}

	private void _onFacebookButtonClick(UIButton p_button)
	{
	//	if (!FB.IsLoggedIn)
	//		FB.Login("", _onFacebookLogin);
	//	else
			_sendFacebookFeed();
	}

	private void _sendFacebookFeed()
	{
	//	FB.Feed(linkDescription: Localization.getString(Localization.TXT_STATE_1_LINK));
	}

//	private void _onFacebookLogin(FBResult p_result)
//	{
	//	if (FB.IsLoggedIn)
//			_sendFacebookFeed();
//	}

	private void _onInviteButtonClick(UIButton p_button)
	{
	
//	 	 #if UNITY_ANDROID && !UNITY_EDITOR
//		string l_to = "";
//		string l_subject = "Zoodles Kid Mode: See why 18M parents have downloaded this app for fun, learning, and safety.";
//		string l_body = "<html><body>We think you'll love this new app we downloaded - it features the best educational content in a safe, adaptive learning environment for most popular devices. As a friend, for a limited time, try <a href='#'>Zoodles Premium free!</a></body></html>";
//		string l_type = "text/html";
//		AndroidJavaClass jc_uri = new AndroidJavaClass("android.net.Uri");
//
//		AndroidJavaClass l_htmlPrase = new AndroidJavaClass("android.text.Html");
//
//		AndroidJavaObject l_bodyAfterPrase = l_htmlPrase.CallStatic<AndroidJavaObject>("fromHtml", l_body);
//
////		AndroidJavaObject l_uri = jc_uri.CallStatic<AndroidJavaObject>("parse", string.Format("mailto:{0}", l_to));
//		
//		AndroidJavaClass jc_intent = new AndroidJavaClass("android.content.Intent");
//		
//		AndroidJavaObject jo_sendTo 	= jc_intent.GetStatic<AndroidJavaObject>("ACTION_SENDTO");
//		AndroidJavaObject jo_subject 	= jc_intent.GetStatic<AndroidJavaObject>("EXTRA_SUBJECT");
//		AndroidJavaObject jo_text 		= jc_intent.GetStatic<AndroidJavaObject>("EXTRA_TEXT");
//		
////		AndroidJavaObject jo_intent = new AndroidJavaObject( "android.content.Intent", jo_sendTo, l_uri );
//		AndroidJavaObject jo_intent = new AndroidJavaObject( "android.content.Intent", jo_sendTo);
//
//		jo_intent.Call<AndroidJavaObject>( "setType", l_type);
//		jo_intent.Call<AndroidJavaObject>( "putExtra", jo_subject, l_subject );
//		jo_intent.Call<AndroidJavaObject>( "putExtra", jo_text, l_bodyAfterPrase );
//		
//		AndroidJavaObject jo_chooser = jc_intent.CallStatic<AndroidJavaObject>( "createChooser", jo_intent, "Choose An Email App" );
//		
//		AndroidJavaClass l_jcPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
//		AndroidJavaObject l_joActivity = l_jcPlayer.GetStatic<AndroidJavaObject>("currentActivity");
//		l_joActivity.Call("startActivity", jo_chooser );
//		#endif 
		if (Application.internetReachability == NetworkReachability.NotReachable || KidMode.isAirplaneModeOn())
		{
			Game game = GameObject.FindWithTag("GameController").GetComponent<Game>();
			game.gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
		}
		else
		{
			m_gameController.getUI ().changeScreen (UIScreen.TELL_FRIEND,true);
			List<Vector3> l_pointListIn = new List<Vector3>();
			UIElement l_newPanel = m_tellFriendCanvas.getView ("mainPanel");
			l_pointListIn.Add( l_newPanel.transform.localPosition );
			l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
			l_newPanel.tweener.addPositionTrack(l_pointListIn, 0.0f);
		}
	}
	
	private void _onTermsButtonClick(UIButton p_button)
	{

		m_quitButton.gameObject.SetActive(false);

		if (Application.internetReachability == NetworkReachability.NotReachable || KidMode.isAirplaneModeOn())
		{
			Game game = GameObject.FindWithTag("GameController").GetComponent<Game>();
			game.gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
		}
		else
		{
			_setupWebview("http://www.zoodles.com/en-US/home/legal/terms");
		}
	}

	private void _onPolicyButtonClick(UIButton p_button)
	{

		m_quitButton.gameObject.SetActive(false);

		if (Application.internetReachability == NetworkReachability.NotReachable || KidMode.isAirplaneModeOn())
		{
			Game game = GameObject.FindWithTag("GameController").GetComponent<Game>();
			game.gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
		}
		else
		{
			_setupWebview("http://www.zoodles.com/en-US/home/legal/privacy");
		}
	}

	private void _setupWebview(string p_url)
	{
		UICanvas l_screen = m_uiManager.createScreen(UIScreen.LEGAL_CONTENT, false, 5);
		m_uiManager.changeScreen(l_screen, true);

		UIButton l_confirm = l_screen.getView("quitButton") as UIButton;
		l_confirm.addClickCallback(_onConfirmButtonClick);
		UniWebView l_webView = l_screen.gameObject.GetComponentInChildren<UniWebView>();
		l_webView.insets = new UniWebViewEdgeInsets((int)(70.0f * l_screen.scaleFactor), (int)(120.0f * l_screen.scaleFactor), (int)(70.0f * l_screen.scaleFactor), (int)(120.0f * l_screen.scaleFactor));
		l_webView.OnReceivedKeyCode += _onBackKeyCode;
		l_webView.OnWebViewShouldClose += _onShouldCloseView;
		l_webView.Load(p_url);
		l_webView.Show();
	}

	private void _closeWebview()
	{

		m_quitButton.gameObject.SetActive(true);

		UICanvas l_screen = m_uiManager.findScreen(UIScreen.LEGAL_CONTENT);
		UniWebView l_webView = l_screen.gameObject.GetComponentInChildren<UniWebView>();
		l_webView.Hide();
		l_webView.CleanCache();
		m_uiManager.removeScreenImmediately(l_screen);
	}
	
	private void _onBackKeyCode(UniWebView p_view, int p_keyCode)
	{
		_closeWebview();
	}

	private bool _onShouldCloseView(UniWebView p_webView)
	{



		_closeWebview();
		return true;
	}

	private void _onConfirmButtonClick(UIButton p_button)
	{
		_closeWebview();
	}

	private UIManager m_uiManager;
	
	private UICanvas m_profileScreen;
	private UICanvas m_backScreen;
	private UICanvas m_tellFriendCanvas;
	private UICanvas m_addFriendCanvas;

    private UIButton m_addChildButton;
	private UIButton m_quitButton;
    private UIButton m_parentDashboardButton;
    private UIButton m_facebookButton;
    private UIButton m_emailInviteButton;
	private UIButton m_createChildButton;
	private UIButton m_tryFreeButton;
	private UIButton m_closeTellFriendButton;
	private UIButton m_addFriendButton;
	private UIButton m_closeAddFriendScreenButton;
	private UIButton m_addFriendAddressButton;
	private UIButton m_sendEmailButton;
	private UIButton m_closeConfirmButton;
	private UIButton m_closeDialogButton;

    private UILabel m_quitLabel;
    private UILabel m_termsOfServicelabel;
    private UILabel m_signedInLabel;
    private UILabel m_emailInviteLabel;
    private UILabel m_privacyPolicyLabel;
    private UILabel m_parentDashboardLabel;
	private UILabel m_facebookPostLabel;
	private UILabel m_errorLabel;
	private UILabel m_confirmContentText;
	private UILabel	m_titleText;

	private UIElement m_dialog;
	private UIElement m_confirmDialog;

	private InputField m_fromInput;
	private InputField m_toInput;
	private InputField m_emailInput;
	private InputField m_optionInput;
	
	private UISwipeList m_gameList;

	private bool m_gotoEntrance;
	private bool m_gotoCREATE_CHILD_NEW;
	private bool m_gotoDashBoard;

	private RequestQueue m_requestQueue;
	private bool m_isIntro = true;
}
