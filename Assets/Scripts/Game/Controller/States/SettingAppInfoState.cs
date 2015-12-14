using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System; 
using System.Globalization; 
using System.Diagnostics;

public class SettingAppInfoState : GameState 
{

	//Public variables

	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );

		game = p_gameController.game;
		m_session = SessionHandler.getInstance ();
		m_requestQueue = new RequestQueue ();
		_setupScreen( p_gameController.getUI() );
		canClick = true;
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_dashboardCommonCanvas );
		p_gameController.getUI().removeScreen( m_planDetailsCanvas );
		p_gameController.getUI().removeScreen( m_cancelSubscriptionCanvas );
		p_gameController.getUI().removeScreen( m_appInfoCanvas );
		p_gameController.getUI().removeScreen( m_thankCanvas );
		p_gameController.getUI().removeScreen( m_signOutConfirmCanvas );
		p_gameController.getUI().removeScreen( m_leftMenuCanvas );
		p_gameController.getUI().removeScreen( m_sentFeedBackCanvas );
	}
	
	
	//---------------- Private Implementation ----------------------
	
	private void _setupScreen( UIManager p_uiManager )
	{
		m_planDetailsCanvas = p_uiManager.createScreen (UIScreen.PLAN_DEATILS, true, 6) as PlanDetails;

		m_commonDialogCanvas =  p_uiManager.createScreen (UIScreen.COMMON_DIALOG, true, 11) as CommonDialogCanvas;
		m_cancelSubscriptionCanvas = p_uiManager.createScreen (UIScreen.CANCEL_SUB, true, 10) as CancelSubscriptionCanvas;
		m_thankCanvas =  p_uiManager.createScreen (UIScreen.THANK, true, 7) as ThankCanvas;
		m_signOutConfirmCanvas =  p_uiManager.createScreen (UIScreen.SIGN_OUT, true, 8) as SignOutConfirmCanvas;
		m_sentFeedBackCanvas = p_uiManager.createScreen (UIScreen.SENT_FEED_BACK, true, 9) as SentFeedBackCanvas;
	
		m_leftMenuCanvas = p_uiManager.createScreen (UIScreen.LEFT_MENU, true, 3) as LeftMenuCanvas;
		m_appInfoCanvas = p_uiManager.createScreen (UIScreen.APP_INFO, true, 2);
		m_dashboardCommonCanvas = p_uiManager.createScreen( UIScreen.SETTING_COMMON, true, 1 );

		m_usernameText = m_appInfoCanvas.getView ("usernameText") as UILabel;
		m_usernameText.text = m_session.username;

		m_menu = m_leftMenuCanvas.getView ("LeftMenu") as UIElement;
		m_leftSideMenuButton = m_dashboardCommonCanvas.getView ("menuButton") as UIButton;
		m_leftSideMenuButton.addClickCallback (toShowMenu);
		m_showProfileButton = m_menu.getView ("profileButton") as UIButton;
		m_showProfileButton.addClickCallback (toShowAllChilren);

		UILabel l_dialogText = m_commonDialogCanvas.getView ("dialogText") as UILabel;
		UILabel l_contentText = m_commonDialogCanvas.getView ("contentText") as UILabel;
		l_dialogText.text = Localization.getString(Localization.TXT_STATE_SETTING_APP_INFO_DIALOG_TITLE);
		l_contentText.text = Localization.getString(Localization.TXT_STATE_SETTING_APP_INFO_DIALOG);

		m_editProfileButton = m_appInfoCanvas.getView ("editProfileButton") as UIButton;
		m_editProfileButton.addClickCallback (onProfileButtonClick);

		m_planDetailsButton = m_appInfoCanvas.getView ("planDetailsButton") as UIButton;
		m_planDetailsButton.addClickCallback (onShowPlanDetails);

		UILabel l_premuimLabel = m_appInfoCanvas.getView ("premiumText") as UILabel;
		if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
		{
			m_planDetailsButton.enabled = false;
			l_premuimLabel.text = Localization.getString(Localization.TXT_STATE_29_FREE);
		}
		if( SessionHandler.getInstance().token.isCurrent() )
		{
			l_premuimLabel.text = Localization.getString(Localization.TXT_STATE_29_TRIAL);
		}
		if( SessionHandler.getInstance().token.isPremium() )
		{
			l_premuimLabel.text = Localization.getString(Localization.TXT_STATE_29_PREMIUM);
		}

		UILabel l_versionText = m_appInfoCanvas.getView ("versionMessage") as UILabel;
		l_versionText.text = l_versionText.text.Replace("{0}", CurrentBundleVersion.version);

		m_dialogCloseButton = m_planDetailsCanvas.getView ("closeMark") as UIButton;
		m_cancelSubDialogCloseButton = m_cancelSubscriptionCanvas.getView ("closeMark") as UIButton;

		m_dialogCloseButton.addClickCallback (onCloseDialog);
		m_cancelSubDialogCloseButton.addClickCallback (onCloseCalSubDialog);


		m_cancelSubButton = m_planDetailsCanvas.getView ("cancelButton") as UIButton;
		m_cancelSubButton.addClickCallback (toCancelSubScreen);

		//honda 
		m_settingButton = m_leftMenuCanvas.getView ("settingButton") as UIButton;
		m_settingButton.addClickCallback(onCloseMenu);
		//end
		m_closeLeftMenuButton = m_leftMenuCanvas.getView ("closeButton") as UIButton;
		m_closeLeftMenuButton.addClickCallback (onCloseMenu);
		m_childModeButton = m_dashboardCommonCanvas.getView ("childModelButton") as UIButton;
		m_childModeButton.addClickCallback (toChildMode);

		m_submitButton = m_cancelSubscriptionCanvas.getView ("submitButton") as UIButton;
		m_submitButton.addClickCallback (onConfirm);

		m_closeThankDialogButton = m_thankCanvas.getView ("closeMark") as UIButton;
		m_closeThankDialogButton.addClickCallback (onCloseThankDialog);

		m_buttonList = m_cancelSubscriptionCanvas.getView ("resonSelectSwipList") as UISwipeList;

		m_buttonList.addClickListener ("unCheckButton",OnClickReasonButton);

		m_descriptionInput = m_cancelSubscriptionCanvas.getView ("reasonInputField").gameObject.GetComponent<InputField>();
		m_signOutButton = m_appInfoCanvas.getView ("signOutButton") as UIButton;
		m_signOutButton.addClickCallback (onSignOut);

		m_confirmSignOutButton = m_signOutConfirmCanvas.getView ("affirmButton") as UIButton;
		m_confirmSignOutButton.addClickCallback (onConfirmSignOut);
		m_cancelSignOutButton =  m_signOutConfirmCanvas.getView ("cancelButton") as UIButton;
		m_cancelSignOutButton.addClickCallback (onCancelSignOut);
		m_closeSignOutButton = m_signOutConfirmCanvas.getView ("closeMark") as UIButton;
		m_closeSignOutButton.addClickCallback (onCancelSignOut);

		m_sendFeedBackButton = m_appInfoCanvas.getView ("feedBackButton") as UIButton;
		m_sendFeedBackButton.addClickCallback (onSendFeedBack);

		m_sendFeedBackbuttonList = new ArrayList();
		m_sentFeedBackCanvas.getView ("dialogContent").getViewsByTag ("reasonCheckBtn",m_sendFeedBackbuttonList);
		int l_listCount = m_sendFeedBackbuttonList.Count;
		for(int l_i = 0; l_i < l_listCount; l_i++)
		{
			UIToggle l_button = m_sendFeedBackbuttonList[l_i] as UIToggle;
			l_button.isOn = false;
		}

		m_sendButton = m_sentFeedBackCanvas.getView ("sendButton") as UIButton;
		m_sendButton.addClickCallback (onSend);

		m_closeSendFeedBackButton = m_sentFeedBackCanvas.getView("closeMark") as UIButton;
		m_closeSendFeedBackButton.addClickCallback (onCloseSendBack);
		m_rightButton = m_appInfoCanvas.getView ("rightButton") as UIButton;
		m_rightButton.addClickCallback (goToChildLock);

		m_deviceButton = m_dashboardCommonCanvas.getView ("controlButton") as UIButton;
		m_deviceButton.addClickCallback (toDeviceScreen);
		m_faqButton = m_dashboardCommonCanvas.getView ("starButton") as UIButton;
		m_faqButton.addClickCallback (toShowFAQ);
		m_overviewButton = m_dashboardCommonCanvas.getView ("overviewButton") as UIButton;
		m_overviewButton.enabled = false;
		m_deviceButton.enabled = true;
		m_faqButton.enabled = true;
		m_childrenList = m_leftMenuCanvas.getView ("childSwipeList") as UISwipeList;
		m_childrenList.addClickListener ("Prototype",onSelectThisChild);

		m_tryPremiumButton = m_leftMenuCanvas.getView ("premiumButton") as UIButton;
		m_buyGemsButton = m_leftMenuCanvas.getView ("buyGemsButton") as UIButton;
		m_tryPremiumButton.addClickCallback (toPremiumScreen);
		m_buyGemsButton.addClickCallback (toBuyGemsScreen);
	}

	private void onSend(UIButton p_button)
	{
		UIToggle l_feedBackBUtton = null;
		int l_lenght = m_sendFeedBackbuttonList.Count;
		for(int l_i = 0; l_i < l_lenght; l_i++)
		{
			UIToggle l_button = m_sendFeedBackbuttonList[l_i] as UIToggle;
			if(l_button.isOn)
			{
				l_feedBackBUtton = l_button;
				break;
			}
		}
		if(null != l_feedBackBUtton)
		{
			string l_to = "";
			string l_subject = "";
			bool l_sendEmail = true;
			switch(l_feedBackBUtton.name)
			{
			case "askQuestionButton":
				l_to = ZoodlesConstants.ZOODLES_SUPPORT_PROBLEM_EMAIL + ZoodlesConstants.ZOODLES_EMAIL_DOMAIN;
				l_subject = Localization.getString(Localization.TXT_STATE_29_QUESTION);
				break;
			case "submitIdealButton":
				l_to = ZoodlesConstants.ZOODLES_SUPPORT_IDEA_EMAIL + ZoodlesConstants.ZOODLES_EMAIL_DOMAIN;
				l_subject = Localization.getString(Localization.TXT_STATE_29_IDEA);
				break;
			case "reportProblemButton":
				l_to = ZoodlesConstants.ZOODLES_SUPPORT_PROBLEM_EMAIL + ZoodlesConstants.ZOODLES_EMAIL_DOMAIN;
				l_subject = Localization.getString(Localization.TXT_STATE_29_PROBLEM);
				break;
			case "complimentButton":
				l_to = ZoodlesConstants.ZOODLES_SUPPORT_COMPLIMENT_EMAIL + ZoodlesConstants.ZOODLES_EMAIL_DOMAIN;
				l_subject = Localization.getString(Localization.TXT_STATE_29_COMPLIMENT);
				break;
			default: 
				l_sendEmail = false;
				break;
			}

			l_subject = l_subject + " " + SystemInfo.deviceModel + " " + SystemInfo.operatingSystem;

			#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
			if(l_sendEmail)
			{
				Process.Start( string.Format("mailto:{0}?subject={1}", l_to, l_subject));
			}
			#endif
			//TODO
			#if UNITY_ANDROID && !UNITY_EDITOR

			AndroidJavaClass l_jcPlayer = new AndroidJavaClass ( "com.unity3d.player.UnityPlayer" );
			AndroidJavaObject l_joActivity = l_jcPlayer.GetStatic<AndroidJavaObject>( "currentActivity" );
			AndroidJavaObject l_joPackageManager = l_joActivity.Call<AndroidJavaObject> ( "getPackageManager" );
			string l_packageName = l_joActivity.Call<string>("getPackageName");
			AndroidJavaObject l_joPackageInfo = l_joPackageManager.Call<AndroidJavaObject>("getPackageInfo", l_packageName, 0);
			string l_versionName = l_joPackageInfo.Get<string>("versionName");
			
			l_subject = l_subject + " " + l_versionName;

			AndroidJavaClass jc_uri = new AndroidJavaClass("android.net.Uri");

			AndroidJavaObject l_uri = jc_uri.CallStatic<AndroidJavaObject>("parse", string.Format("mailto:{0}", l_to));

			string[] l_email = {l_to};

			AndroidJavaClass jc_intent = new AndroidJavaClass("android.content.Intent");

			AndroidJavaObject jo_sendTo 	= jc_intent.GetStatic<AndroidJavaObject>("ACTION_SENDTO");
			AndroidJavaObject jo_cc 		= jc_intent.GetStatic<AndroidJavaObject>("EXTRA_CC");
			AndroidJavaObject jo_subject 	= jc_intent.GetStatic<AndroidJavaObject>("EXTRA_SUBJECT");
			AndroidJavaObject jo_text 		= jc_intent.GetStatic<AndroidJavaObject>("EXTRA_TEXT");

			AndroidJavaObject jo_intent = new AndroidJavaObject( "android.content.Intent", jo_sendTo, l_uri );
			jo_intent.Call<AndroidJavaObject>( "putExtra", jo_cc, l_email );
			jo_intent.Call<AndroidJavaObject>( "putExtra", jo_subject, l_subject );
			jo_intent.Call<AndroidJavaObject>( "putExtra", jo_text, "" );

			AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");
			jc.CallStatic("sendFeedback");

			AndroidJavaObject jo_chooser = jc_intent.CallStatic<AndroidJavaObject>( "createChooser", jo_intent, Localization.getString(Localization.TXT_STATE_29_CHOOSE) );

			l_joActivity.Call("startActivity", jo_chooser );


			#endif
		}
	}

	private void onProfileButtonClick(UIButton p_button)
	{
		if(canClick)
		{
			canClick = false;
			p_button.removeAllCallbacks ();
			m_commonDialogCanvas.setOriginalPosition ();
			UIButton l_closeButton = m_commonDialogCanvas.getView ("closeMark") as UIButton;
			l_closeButton.addClickCallback (onCloseDialogButtonClick);
		}
	}

	private void onCloseDialogButtonClick(UIButton p_button)
	{
		canClick = true;
		p_button.removeAllCallbacks();
		m_commonDialogCanvas.setOutPosition ();
		m_editProfileButton.addClickCallback (onProfileButtonClick);
	}
	
	private void onSelectThisChild(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		if (checkInternet() == false)
			return;

		Kid l_kid = p_data as Kid;
		if (Localization.getString(Localization.TXT_86_BUTTON_ADD_CHILD).Equals (l_kid.name))
		{
			SessionHandler.getInstance().CreateChild = true;
			m_gameController.connectState(ZoodleState.CREATE_CHILD_NEW,int.Parse(m_gameController.stateName));
			m_gameController.changeState (ZoodleState.CREATE_CHILD_NEW);
		}
		else
		{
			List<Kid> l_kidList = SessionHandler.getInstance().kidList;
			SessionHandler.getInstance().currentKid = l_kidList[p_index-1];
			m_gameController.changeState(ZoodleState.OVERVIEW_INFO);
		}
	}

	private void OnClickReasonButton(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		if (null != m_currentReasonButton) 
		{
			m_currentReasonButton.alpha = 0.0f;
		}
		m_currentReasonButton = p_button;
		p_button.alpha = 1.0f;
		m_reason = p_data.ToString ();
	}

	private void toDeviceScreen(UIButton p_button)
	{
		if (checkInternet() == false)
			return;

		p_button.removeClickCallback (toDeviceScreen);
		m_gameController.changeState(ZoodleState.NOTIFICATION_STATE);
	}

	private void toShowFAQ(UIButton p_button)
	{
		p_button.removeClickCallback (toShowFAQ);
		m_gameController.changeState(ZoodleState.FAQ_STATE);
	}

	private void toBuyGemsScreen(UIButton p_button)
	{
		gotoGetGems ();
	}
	
	private void gotoGetGems()
	{	
		string l_returnJson = SessionHandler.getInstance ().GemsJson;
		
		if(l_returnJson.Length > 0)
		{
			Hashtable l_date = MiniJSON.MiniJSON.jsonDecode (l_returnJson) as Hashtable;
			if(l_date.ContainsKey("jsonResponse"))
			{
				m_gameController.connectState( ZoodleState.BUY_GEMS, int.Parse(m_gameController.stateName) );
				m_gameController.changeState (ZoodleState.BUY_GEMS);
			}
			else
			{
				//sendCall (m_game.gameController,null,"/api/gems_amount/gems_amount?" + ZoodlesConstants.PARAM_TOKEN + "=" + SessionHandler.getInstance ().token.getSecret (),CallMethod.GET,ZoodleState.BUY_GEMS);
				Server.init (ZoodlesConstants.getHttpsHost());
				m_requestQueue.reset ();
				m_requestQueue.add (new ViewGemsRequest(viewGemsRequestComplete));
				m_requestQueue.request ();
			}
		}
		else
		{
			Server.init (ZoodlesConstants.getHttpsHost());
			m_requestQueue.reset ();
			m_requestQueue.add (new ViewGemsRequest(viewGemsRequestComplete));
			m_requestQueue.request ();
			//sendCall (m_game.gameController,null,"/api/gems_amount/gems_amount?" + ZoodlesConstants.PARAM_TOKEN + "=" + SessionHandler.getInstance ().token.getSecret (),CallMethod.GET,ZoodleState.BUY_GEMS);
		}
	}

	private void viewGemsRequestComplete(HttpsWWW p_response)
	{
		Server.init (ZoodlesConstants.getHttpsHost());
		if(p_response.error == null)
		{
			SessionHandler.getInstance ().GemsJson = p_response.text;
			m_gameController.connectState( ZoodleState.BUY_GEMS, int.Parse(m_gameController.stateName) );
			m_gameController.changeState (ZoodleState.BUY_GEMS);
		}
		else
		{
			setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_11_FAIL),Localization.getString(Localization.TXT_STATE_11_FAIL_DATA));
		}
	}

	private void toPremiumScreen(UIButton p_button)
	{
		if (LocalSetting.find("User").getBool("UserTry",true))
		{
			if(!SessionHandler.getInstance().token.isCurrent())
			{
				m_gameController.connectState (ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName));
				m_gameController.changeState (ZoodleState.VIEW_PREMIUM);	
			}
		}
		else
		{
			m_gameController.connectState (ZoodleState.SIGN_IN_UPSELL, int.Parse(m_gameController.stateName));
			m_gameController.changeState (ZoodleState.SIGN_IN_UPSELL);
		}
	}

	private void goToChildLock(UIButton p_button)
	{
		if (checkInternet() == false)
			return;

		p_button.removeClickCallback (goToChildLock);
		m_gameController.changeState (ZoodleState.CHILD_LOCK_STATE);
	}

	private void onShowPlanDetails(UIButton p_button)
	{
		if(canClick)
		{
			canClick = false;
			p_button.removeClickCallback (onShowPlanDetails);
			m_requestQueue = new RequestQueue ();
			m_requestQueue.add (new ShowPlanDetailsRequest (showPlanDetails));
			m_requestQueue.request ();
		}
	}

	private void showPlanDetails(HttpsWWW p_response)
	{
		m_gameController.getUI ().changeScreen (UIScreen.PLAN_DEATILS,true);
		if(null == p_response.error)
		{
			m_planText = m_planDetailsCanvas.getView("planText") as UILabel;
			//m_renewText = m_planDetailsCanvas.getView("renewsText") as UILabel;
			Hashtable l_table = (Hashtable)MiniJSON.MiniJSON.jsonDecode(p_response.text);
			//DateTime l_dt = DateTime.Parse( (string)l_table["next_renewal_at"]);
			string l_plan = string.Format( Localization.getString(Localization.TXT_STATE_29_CURRENT), (string)l_table["plan_name"]);
			//string l_renewTime ="You plan renews on " + l_dt.ToString("m, yyyy", DateTimeFormatInfo.InvariantInfo);
			m_planText.text = l_plan;
			//m_renewText.text = l_renewTime;
			m_planDetailsCanvas.setOriginalPosition ();
			m_session.handlePage = "";
		}
		else
		{
			m_planDetailsButton.addClickCallback(onShowPlanDetails);
		}

	}

	private void onSendFeedBack(UIButton p_button)
	{
		if (checkInternet() == false)
			return;

		if(canClick)
		{
			canClick = false;
			m_gameController.getUI ().changeScreen (UIScreen.SENT_FEED_BACK,true);
			m_sentFeedBackCanvas.setOriginalPosition ();
		}
	}

	private void onCloseSendBack(UIButton p_button)
	{
		canClick = true;
		m_gameController.getUI ().changeScreen (UIScreen.SENT_FEED_BACK,false);
		m_sentFeedBackCanvas.setOutPosition ();
	}

	private void onSignOut(UIButton p_button)
	{
		if (checkInternet() == false)
			return;

		if(canClick)
		{	
			game.IsReLaunch = 0;
			game.IsFirstLaunch = 0;
			game.IsLogin = 0;
			SessionHandler.SaveCurrentKid(-1);
			canClick = false;
			m_gameController.getUI ().changeScreen (UIScreen.SIGN_OUT,true);
			m_signOutConfirmCanvas.setOriginalPosition ();
		}
	}

	private void onCancelSignOut(UIButton p_button)
	{
		canClick = true;
		m_gameController.getUI ().changeScreen (UIScreen.SIGN_OUT,false);
		m_signOutConfirmCanvas.setOutPosition ();
	}

	private void onConfirmSignOut(UIButton p_button)
	{
		m_session.clearUserData (true);
		LocalSetting.find ("User").delete ();
		m_gameController.changeState (ZoodleState.ZOODLES_ANIMATION);
	}

	private void onCloseThankDialog(UIButton p_button)
	{
		m_thankCanvas.setOutPosition ();
	}

	private void onConfirm(UIButton p_button)
	{
		Server.init (ZoodlesConstants.getHttpsHost());
		m_requestQueue.reset ();
		m_requestQueue.add (new CancelSubcriptionRequest(m_reason,m_descriptionInput.text,cancelSubcriptionComplete));
		m_requestQueue.request ();
	}

	private void cancelSubcriptionComplete(HttpsWWW p_response)
	{
		if(null == p_response.error)
		{
			m_cancelSubscriptionCanvas.setOutPosition ();
			m_thankCanvas.setOriginalPosition ();
		}
		Server.init (ZoodlesConstants.getHttpsHost());
	}

	private void toCancelSubScreen(UIButton p_button)
	{
		m_planDetailsCanvas.setOutPosition ();
		m_cancelSubscriptionCanvas.setOriginalPosition ();
	}
	
	private void onCloseDialog(UIButton p_button)
	{
		canClick = true;
		m_gameController.getUI().changeScreen(UIScreen.PLAN_DEATILS,false);
		m_planDetailsCanvas.setOutPosition ();
		m_planDetailsButton.addClickCallback (onShowPlanDetails);
	}

	private void onCloseCalSubDialog(UIButton p_button)
	{
		m_cancelSubscriptionCanvas.setOutPosition ();
	}

	private void onCloseMenu(UIButton p_button)
	{
		if(canMoveLeftMenu)
		{
			m_gameController.getUI().changeScreen(UIScreen.LEFT_MENU,false);
			Vector3 l_position = m_menu.transform.localPosition;
			
			List<Vector3> l_posList = new List<Vector3> ();
			l_posList.Add (l_position);
			l_posList.Add (l_position + new Vector3 (-200, 0, 0));
			m_menu.tweener.addPositionTrack (l_posList, m_leftMenuCanvas.displaySpeed, onCloseMenuTweenFinished, Tweener.Style.QuadOutReverse);
			canMoveLeftMenu = false;
		}
	}

	private void onCloseMenuTweenFinished( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		canMoveLeftMenu = true;
	}
	
	private void toShowMenuTweenFinished( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		canMoveLeftMenu = true;
	}
	
	private void toChildMode(UIButton p_button)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if (KidMode.isHomeLauncherKidMode ()) {
			
			m_gameController.changeState (ZoodleState.PROFILE_SELECTION);
			
		} else {
			
			KidMode.enablePluginComponent();
			
			KidMode.openLauncherSelector ();
			
		}
		#else
		m_gameController.changeState (ZoodleState.PROFILE_SELECTION);
		#endif
	}

	private void toShowMenu(UIButton p_button)
	{
		if(canMoveLeftMenu && checkInternet())
		{
			m_gameController.getUI().changeScreen(UIScreen.LEFT_MENU,true);
			Vector3 l_position = m_menu.transform.localPosition;
			List<Vector3> l_posList = new List<Vector3> ();
			l_posList.Add (l_position);
			l_posList.Add (l_position + new Vector3 (200, 0, 0));
			m_menu.tweener.addPositionTrack (l_posList, m_leftMenuCanvas.displaySpeed, toShowMenuTweenFinished, Tweener.Style.QuadOutReverse);
			canMoveLeftMenu = false;
		}
	}

	private void toShowAllChilren(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		m_leftMenuCanvas.showKids (addButtonClickCall);
	}

	private void addButtonClickCall( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_showProfileButton.addClickCallback (toShowAllChilren);
	}

	private bool checkInternet()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable 
		    || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected())
		{
			m_gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
			
			ErrorMessage error = GameObject.FindWithTag("ErrorMessageTag").GetComponent<ErrorMessage>() as ErrorMessage;
			if (error != null)
				error.onClick += onClickExit;
			
			return false;
		}
		return true;
	}
	
	private void onClickExit()
	{
		ErrorMessage error = GameObject.FindWithTag("ErrorMessageTag").GetComponent<ErrorMessage>() as ErrorMessage;
		error.onClick -= onClickExit;;
		m_gameController.changeState (ZoodleState.CONTROL_APP);
	}


	//Private variables
	private UIButton 	m_leftSideMenuButton;
	private UIButton 	m_showProfileButton;
	private UIButton    m_planDetailsButton;
	private UIButton	m_dialogCloseButton;
	private UIButton	m_cancelSubDialogCloseButton;
	private UIButton	m_cancelSubButton;
	//honda
	private UIButton	m_settingButton;
	//end
	private UIButton	m_closeLeftMenuButton;
	private UIButton    m_childModeButton;
	private UIButton  	m_submitButton;
	private UIButton  	m_closeThankDialogButton;
	private UIButton  	m_signOutButton;
	private UIButton  	m_confirmSignOutButton;
	private UIButton  	m_cancelSignOutButton;
	private UIButton  	m_closeSignOutButton;
	private UIButton  	m_sendFeedBackButton;
	private UIButton  	m_closeSendFeedBackButton;
	private UIButton    m_rightButton;
	private UIButton    m_deviceButton;
	private UIButton    m_faqButton;
	private UIButton 	m_tryPremiumButton;
	private UIButton 	m_buyGemsButton;
	private UIButton 	m_sendButton;
	private UIButton	m_editProfileButton;
	private UIButton	m_overviewButton;
	
	private UIButton 	m_currentReasonButton;
	private string 		m_reason;
	private InputField  m_descriptionInput;
	private UILabel     m_planText;
	private UILabel     m_renewText;
	private UILabel 	m_usernameText;

	private UISwipeList m_buttonList;
	private UISwipeList m_childrenList;

	private UIElement 	m_menu;

	private ArrayList	 m_sendFeedBackbuttonList;
	private SessionHandler m_session;

	private const string CANCEL_SUBSCRIPTION = "cancelSubscription";
	private const string SHOW_DETAILS = "showDetail";

	private UICanvas    		m_dashboardCommonCanvas;
	private UICanvas			m_appInfoCanvas;
	private PlanDetails			m_planDetailsCanvas;
	private CancelSubscriptionCanvas	m_cancelSubscriptionCanvas;
	private LeftMenuCanvas		m_leftMenuCanvas;
	private ThankCanvas 		m_thankCanvas;
	private SignOutConfirmCanvas m_signOutConfirmCanvas;
	private SentFeedBackCanvas  m_sentFeedBackCanvas;
	private CommonDialogCanvas  m_commonDialogCanvas;

	private bool 		canMoveLeftMenu = true;
	private bool		canClick = true;

	private RequestQueue m_requestQueue;

	//honda
	Game game;
}
