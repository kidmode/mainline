using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SettingChildLockState : GameState 
{
	//Public variables
	
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );
		
		m_settingCache = SessionHandler.getInstance ().settingCache;

		_setupScreen( p_gameController.getUI() );

		m_requestQueue = new RequestQueue ();

		m_pinInputField.interactable = false;

		if( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() )
		{
			if(m_settingCache.active)
			{
				if( m_settingCache.childLockSwitch )
				{
					m_verifyBirth.enabled = true;
				}
				else
				{
					m_verifyBirth.enabled = false;
				}
				
				if(!m_settingCache.childLockSwitch || m_settingCache.verifyBirth)
				{
					m_pinInputField.interactable = false;
				}
				else
				{
					m_pinInputField.interactable = true;
				}
			}
			else
			{
				if( SessionHandler.getInstance().childLockSwitch )
				{
					m_verifyBirth.enabled = true;
				}
				else
				{
					m_verifyBirth.enabled = false;
				}
				
				if(!SessionHandler.getInstance().childLockSwitch || SessionHandler.getInstance().verifyBirth)
				{
					m_pinInputField.interactable = false;
				}
				else
				{
					m_pinInputField.interactable = true;
				}
			}
		}
		else
		{
			m_verifyBirth.enabled = false;
		}
		m_lockSwitchButton.addValueChangedCallback (onOffLock);
		m_verifyBirth.addValueChangedCallback (toCheckVerifyBirth);
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
	}
	
	public override void exit( GameController p_gameController )
	{
		int l_changedStateName = int.Parse( m_gameController.stateName);
		if (SessionHandler.getInstance().settingCache.active && 
		    !(l_changedStateName == ZoodleState.SETTING_STATE || 
		      l_changedStateName == ZoodleState.DEVICE_OPTIONS_STATE || 
		       l_changedStateName == ZoodleState.NOTIFICATION_STATE) && checkInternet())
			updateSetting ();
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_childLockCanvas );
		p_gameController.getUI().removeScreen( m_dashboardCommonCanvas );
		p_gameController.getUI().removeScreen( m_leftMenuCanvas );
		p_gameController.getUI().removeScreen( m_childLockHelpCanvas );
		p_gameController.getUI().removeScreen( m_cancelLockConfirmCanvas );
	}

	public void updateSetting()
	{

//		return;
		//update childLock
		m_requestQueue.reset ();
		if(m_settingCache.childLockSwitch && !m_settingCache.verifyBirth)
		{
			if(m_settingCache.childLockPassword.Length != 4)
			{
				//m_noticeLabel.text = Localization.getString(Localization.TXT_STATE_31_PIN);
				m_closeButton.active = true;
				return;
			}
			else
			{
				m_requestQueue.add(new EnableLockRequest(m_settingCache.childLockPassword));
				m_requestQueue.add(new UpdateSettingRequest("true"));
			}
		}
		else if( m_settingCache.childLockSwitch && m_settingCache.verifyBirth)
		{
			m_requestQueue.add(new CancelLockRequest());
			m_requestQueue.add(new UpdateSettingRequest("true"));
		}
		else if(!m_settingCache.childLockSwitch)
		{
			m_requestQueue.add(new UpdateSettingRequest("false"));
		}
		//update notifucation
		m_requestQueue.add (new UpdateNotificateRequest(m_settingCache.newAddApp,m_settingCache.smartSelect,m_settingCache.freeWeeklyApp));
		m_requestQueue.add (new UpdateDeviceOptionRequest(m_settingCache.allowCall?"true":"false",m_settingCache.tip?"true":"false",m_settingCache.masterVolum,m_settingCache.musicVolum,m_settingCache.effectsVolum));
		m_requestQueue.request (RequestType.SEQUENCE);
		//update device option
		SessionHandler.getInstance().resetSetting ();
		Debug.Log("SettingChildLock set volume data from setting cache to setting cache");
		SoundManager.getInstance ().effectVolume = (float) SessionHandler.getInstance ().effectsVolum / 100;
		SoundManager.getInstance ().musicVolume = (float) SessionHandler.getInstance ().musicVolum / 100;
		SoundManager.getInstance ().masterVolume = (float) SessionHandler.getInstance ().masterVolum / 100;
		PlayerPrefs.SetInt ("master_volume",SessionHandler.getInstance ().masterVolum);
		PlayerPrefs.SetInt ("music_volume",SessionHandler.getInstance ().musicVolum);
		PlayerPrefs.SetInt ("effects_volume",SessionHandler.getInstance ().effectsVolum);
		PlayerPrefs.Save ();
	}
	
	//---------------- Private Implementation ----------------------
	private void _setupScreen( UIManager p_uiManager )
	{



		m_cancelLockConfirmCanvas = p_uiManager.createScreen( UIScreen.CANCEL_CHILD_LOCK, true, 6 ) as CancelLockConfirmCanvas;
		m_childLockHelpCanvas =  p_uiManager.createScreen( UIScreen.CHILD_LOCK_HELP, true, 5 ) as ChildLockHelpCanvas;
		m_leftMenuCanvas = p_uiManager.createScreen (UIScreen.LEFT_MENU, true, 3) as LeftMenuCanvas;
		m_childLockCanvas = p_uiManager.createScreen( UIScreen.CHILD_LOCK_SCREEN, true, 2 );

		m_dashboardCommonCanvas = p_uiManager.createScreen( UIScreen.SETTING_COMMON, true, 1 );

		m_lockSwitchButton = m_childLockCanvas.getView ("childLockCheckButton") as UIToggle;

		m_pinInputField = m_childLockCanvas.getView ("pinInputField").gameObject.GetComponent<InputField> ();
		m_pinInputField.onValueChange.AddListener (pinValueChange);
		m_menu = m_leftMenuCanvas.getView ("LeftMenu") as UIElement;
		m_leftSideMenuButton = m_dashboardCommonCanvas.getView ("menuButton") as UIButton;
		m_leftSideMenuButton.addClickCallback (toShowMenu);
		m_showProfileButton = m_menu.getView ("profileButton") as UIButton;
		m_showProfileButton.addClickCallback (toShowAllChilren);

		//Kevin  ==== pin ux chagne
		m_currentPinDisplay = m_childLockCanvas.getView("txtChildCodeDisplay") as UILabel;

		m_saveNewPinButton = m_childLockCanvas.getView("saveButton") as UIButton; 

		InputFieldNewChildCode = m_childLockCanvas.getView("InputFieldNewChildCode") as UIInputField;

		m_saveNewPinButton.addClickCallback (onSaveButtonClicked);


		m_childLockChangeOKay = m_childLockCanvas.getView("dialogChildLockChangeOK") as UIImage;

		m_childLockChangeOKay.active = false;

		m_childLockChangeError = m_childLockCanvas.getView("dialogChildLockChangeError") as UIImage;

		m_childLockChangeError.active = false;



		m_childLockChangeHelp = m_childLockCanvas.getView("dialogChildLockChangeHelp") as UIImage;

//		UILabel dialogChildLockChangeHelpTitleLabel = m_childLockCanvas.getView("dialogChildLockChangeHelpTitle") as UILabel;
//
//		dialogChildLockChangeHelpTitleLabel.text =  Localization.getString(Localization.TXT_STATE_31_PIN);
//		
//		UILabel dialogChildLockChangeHelpTxt1Label = m_childLockCanvas.getView("dialogChildLockChangeHelpTxt1") as UILabel;
//		
//		dialogChildLockChangeHelpTxt1Label.text =  Localization.getString(Localization.TXT_STATE_31_PIN);

//		UIButton dialogChildLockChangeHelpCloseMark = m_childLockCanvas.getView("dialogChildLockChangeHelpCloseMark") as UIButton;
//
//		dialogChildLockChangeHelpCloseMark.addClickCallback(closeHelpDialog);

		m_childLockChangeHelp.active = false;


		//honda 
		m_settingButton = m_leftMenuCanvas.getView ("settingButton") as UIButton;
		m_settingButton.addClickCallback(onCloseMenu);
		//end
		m_closeLeftMenuButton = m_leftMenuCanvas.getView ("closeButton") as UIButton;
		m_closeLeftMenuButton.addClickCallback (onCloseMenu);

		m_leftButton = m_childLockCanvas.getView ("leftButton") as UIButton;
		m_leftButton.addClickCallback (toAppInfoPage);

		m_verifyBirth = m_childLockCanvas.getView ("verifyBirthButton") as UIToggle;
		m_verifyBirth.isOn = m_settingCache.active?m_settingCache.verifyBirth:SessionHandler.getInstance().verifyBirth;

		m_deviceButton = m_dashboardCommonCanvas.getView ("controlButton") as UIButton;
		m_deviceButton.addClickCallback (toDeviceScreen);

		m_faqButton = m_dashboardCommonCanvas.getView ("starButton") as UIButton;
		m_faqButton.addClickCallback (toFAQScreen);

		UIButton l_overviewButton = m_dashboardCommonCanvas.getView ("overviewButton") as UIButton;
		l_overviewButton.enabled = false;
		m_deviceButton.enabled = true;
		m_faqButton.enabled = true;
		m_childModeButton = m_dashboardCommonCanvas.getView ("childModelButton") as UIButton;
		m_childModeButton.addClickCallback (toChildMode);

		m_helpButton = m_childLockCanvas.getView ("helpButton") as UIButton;
		m_helpButton.addClickCallback (showHelpDialog);
		if(m_settingCache.active)
		{
			if(m_settingCache.childLockSwitch)
			{
				m_lockSwitchButton.isOn = true;
			}
			else
			{
				m_lockSwitchButton.isOn = false;
			}
			
			if(m_settingCache.verifyBirth)
			{
				m_verifyBirth.isOn = true;
			}
			else
			{
				m_verifyBirth.isOn = false;
				if(null != m_settingCache.childLockPassword && !string.Empty.Equals(m_settingCache.childLockPassword)){

					m_currentPinDisplay.text = m_settingCache.childLockPassword;
//					m_pinInputField.text = m_settingCache.childLockPassword;

				}
			}
		}
		else
		{
			if(SessionHandler.getInstance().childLockSwitch)
			{
				m_lockSwitchButton.isOn = true;
			}
			else
			{
				m_lockSwitchButton.isOn = false;
			}
			
			if(SessionHandler.getInstance().verifyBirth)
			{

				m_currentPinDisplay.text = SessionHandler.getInstance().pin.ToString();

				m_verifyBirth.isOn = true;
			}
			else
			{
				m_verifyBirth.isOn = false;
				if(!string.Empty.Equals(SessionHandler.getInstance().childLockPassword)){

					m_currentPinDisplay.text = SessionHandler.getInstance().childLockPassword;
//					m_pinInputField.text = SessionHandler.getInstance().childLockPassword;

				}
			}
		}
		m_childrenList = m_leftMenuCanvas.getView ("childSwipeList") as UISwipeList;
		m_childrenList.addClickListener ("Prototype",onSelectThisChild);

		m_tryPremiumButton = m_leftMenuCanvas.getView ("premiumButton") as UIButton;
		m_buyGemsButton = m_leftMenuCanvas.getView ("buyGemsButton") as UIButton;
		m_tryPremiumButton.addClickCallback (toPremiumScreen);
		m_buyGemsButton.addClickCallback (toBuyGemsScreen);
		m_closeButton = m_childLockHelpCanvas.getView ("closeMark") as UIButton;
		m_closeButton.addClickCallback (closeHelpDialogButton);
		m_premiumPurchaseButton = m_childLockCanvas.getView ("kidModeButton") as UIButton;
		m_buyNowButton = m_childLockCanvas.getView ("buyNowButton") as UIButton;
		m_premiumPurchaseButton.addClickCallback (gotoPremiumPurchase);
		m_buyNowButton.addClickCallback (gotoPremiumPurchase);

		m_cancelTurnOffButton = m_cancelLockConfirmCanvas.getView ("cancelButton") as UIButton;
		m_turnOffButton = m_cancelLockConfirmCanvas.getView ("turnOffButton") as UIButton;
		m_closeTurnOnConfirmButton = m_cancelLockConfirmCanvas.getView ("exitButton") as UIButton;

		m_cancelTurnOffButton.addClickCallback (closeTurnOnConfirmButton);
		m_turnOffButton.addClickCallback (turnOnChildLock);
		m_closeTurnOnConfirmButton.addClickCallback (closeTurnOnConfirmButton);

		m_upsellPanel = m_childLockCanvas.getView ("upsellPanel") as UIElement;
		if (SessionHandler.getInstance ().token.isPremium () || SessionHandler.getInstance ().token.isCurrent ())
			m_upsellPanel.active = false;
		else
			m_upsellPanel.active = true;

	}

	public void pinValueChange( string p_pinValue )
	{
		m_settingCache.childLockPassword = p_pinValue;
		m_settingCache.active = true;
	}

	private bool checkPin()
	{

		return true;

		if(SessionHandler.getInstance().settingCache.childLockSwitch && !SessionHandler.getInstance().settingCache.verifyBirth && m_pinInputField.text.Length != 4)
		{
			m_gameController.getUI ().changeScreen (UIScreen.CHILD_LOCK_HELP,true);
			UILabel l_text = m_childLockHelpCanvas.getView ("dialogContent").getView("Text") as UILabel;
			l_text.text = Localization.getString(Localization.TXT_STATE_31_PIN);
			m_childLockHelpCanvas.setOriginalPosition ();
			m_closeButton.addClickCallback (closeHelpDialog);
			return false;
		}
		else
		{
			return true;
		}
	}

	private void closeHelpDialogButton(UIButton p_button)
	{
		m_gameController.getUI ().changeScreen (UIScreen.CHILD_LOCK_HELP,false);
		m_childLockHelpCanvas.setOutPosition();
	}

	private void closeTurnOnConfirmButton(UIButton p_button)
	{
		m_gameController.getUI ().changeScreen (UIScreen.CANCEL_CHILD_LOCK,false);
		m_lockSwitchButton.isOn = true;
		m_cancelLockConfirmCanvas.setOutPosition();
	}

	private void turnOnChildLock(UIButton p_button)
	{
		m_gameController.getUI ().changeScreen (UIScreen.CANCEL_CHILD_LOCK,false);
		m_lockSwitchButton.isOn = false;
		m_settingCache.childLockSwitch = false;
		m_pinInputField.interactable = false;
		m_cancelLockConfirmCanvas.setOutPosition();
	}

	private void gotoPremiumPurchase(UIButton p_button)
	{
		if (string.Empty.Equals(SessionHandler.getInstance().PremiumJson))
		{
			Server.init (ZoodlesConstants.getHttpsHost());
			m_requestQueue.reset ();
			m_requestQueue.add (new GetPlanDetailsRequest(viewPremiumRequestComplete));
			m_requestQueue.request ();
		}
		else
		{
			if(checkPin())
			{
				m_gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName) );
				m_gameController.changeState( ZoodleState.VIEW_PREMIUM );
			}
		}
	}

	private void viewPremiumRequestComplete(HttpsWWW p_response)
	{
		Server.init (ZoodlesConstants.getHttpsHost());
		if(null == p_response.error)
		{
			if(checkPin())
			{
				SessionHandler.getInstance ().PremiumJson = p_response.text;
				m_gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName) );
				m_gameController.changeState( ZoodleState.VIEW_PREMIUM );
			}
		}
		else
		{
			setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_11_FAIL),Localization.getString(Localization.TXT_STATE_11_FAIL_DATA));
		}
	}

	private void buyGemsRequestComplete(HttpsWWW p_response)
	{
		Server.init (ZoodlesConstants.getHttpsHost());
		if(null == p_response.error)
		{
			if(checkPin())
			{
				SessionHandler.getInstance ().GemsJson = p_response.text;
				m_gameController.connectState( ZoodleState.BUY_GEMS, int.Parse(m_gameController.stateName) );
				m_gameController.changeState (ZoodleState.BUY_GEMS);
			}
		}
		else
		{
			setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_11_FAIL),Localization.getString(Localization.TXT_STATE_11_FAIL_DATA));
		}
	}
	
	private void showHelpDialog(UIButton p_button)
	{
		m_gameController.getUI ().changeScreen (UIScreen.CHILD_LOCK_HELP,true);
		p_button.removeClickCallback (showHelpDialog);
		m_childLockHelpCanvas.setOriginalPosition ();
		m_closeButton.addClickCallback (closeHelpDialog);
	}

	private void closeHelpDialog(UIButton p_button)
	{
		UILabel l_text = m_childLockHelpCanvas.getView ("dialogContent").getView("Text") as UILabel;
		l_text.text = Localization.getString(Localization.TXT_87_LABEL_CONTENT);
		m_gameController.getUI ().changeScreen (UIScreen.CHILD_LOCK_HELP,false);
		p_button.removeClickCallback (closeHelpDialog);
		m_childLockHelpCanvas.setOutPosition ();
		m_helpButton.addClickCallback (showHelpDialog);
	}

	private void closeNoticeDialog(UIButton p_button)
	{
		UILabel l_text = m_childLockHelpCanvas.getView ("dialogContent").getView("Text") as UILabel;
		l_text.text = Localization.getString(Localization.TXT_87_LABEL_CONTENT);
		m_gameController.getUI ().changeScreen (UIScreen.CHILD_LOCK_HELP,false);
		p_button.removeClickCallback (closeHelpDialog);
		m_childLockHelpCanvas.setOutPosition ();
	}

	private void onSelectThisChild(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		if (checkInternet() == false)
			return;

		Kid l_kid = p_data as Kid;
		if (Localization.getString(Localization.TXT_86_BUTTON_ADD_CHILD).Equals (l_kid.name))
		{
			if(checkPin())
			{
				SessionHandler.getInstance().CreateChild = true;
				m_gameController.connectState(ZoodleState.CREATE_CHILD_NEW,int.Parse(m_gameController.stateName));
				m_gameController.changeState (ZoodleState.CREATE_CHILD_NEW);
			}
		}
		else
		{
			List<Kid> l_kidList = SessionHandler.getInstance().kidList;
			if(checkPin())
			{
				SessionHandler.getInstance().currentKid = l_kidList[p_index-1];
				m_gameController.changeState(ZoodleState.OVERVIEW_INFO);
			}
		}
	}

	private void onOffLock( UIToggle p_toggle, bool p_value )
	{
		if( false == p_value )
		{
			m_gameController.getUI ().changeScreen (UIScreen.CANCEL_CHILD_LOCK,true);
			m_cancelLockConfirmCanvas.setOriginalPosition();

			m_verifyBirth.enabled = false;
		}
		else
		{
			if( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() )
				m_verifyBirth.enabled = true;
			else
				m_verifyBirth.enabled = false;
			m_settingCache.childLockSwitch = true;
			if( m_verifyBirth.isOn )
			{
				m_pinInputField.interactable = false;
			}
			else
			{
				m_pinInputField.interactable = true;
			}
		}
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

	private void toDeviceScreen(UIButton p_button)
	{
		if (checkInternet() == false)
			return;

		p_button.removeClickCallback (toDeviceScreen);
		if(checkPin())
		{
			m_gameController.changeState (ZoodleState.NOTIFICATION_STATE);
		}
		else
		{
			p_button.addClickCallback(toDeviceScreen);
		}
	}

	private void toFAQScreen(UIButton p_button)
	{
		p_button.removeClickCallback (toFAQScreen);
		if(checkPin())
		{
			m_gameController.changeState (ZoodleState.FAQ_STATE);
		}
		else
		{
			p_button.addClickCallback (toFAQScreen);
		}
	}

	private void toChildMode(UIButton p_button)
	{


//		if (KidMode.isHomeLauncherKidMode ()) {
//			
//			if(checkPin())
//			{
//				m_gameController.changeState (ZoodleState.PROFILE_SELECTION);
//			}
//			
//		} else {
//			
//			KidMode.enablePluginComponent();
//			
//			KidMode.openLauncherSelector ();
//			
//		}


		#if UNITY_ANDROID && !UNITY_EDITOR
		if (KidMode.isHomeLauncherKidMode ()) {
			
			if(checkPin())
			{
				m_gameController.changeState (ZoodleState.PROFILE_SELECTION);
			}
			
		} else {
			
			KidMode.enablePluginComponent();
			
			KidMode.openLauncherSelector ();
			
		}
		#else
		if(checkPin())
		{
			m_gameController.changeState (ZoodleState.PROFILE_SELECTION);
		}
		#endif
	}

	private void toCheckVerifyBirth(UIToggle p_button, bool p_value)
	{
		if(m_settingCache.childLockSwitch)
		{
			if( false == p_value )
			{
				m_settingCache.verifyBirth = false;
				m_pinInputField.interactable = true;
			}
			else
			{
				m_settingCache.verifyBirth = true;
				m_pinInputField.interactable = false;
			}
		}
	}

	private void toAppInfoPage(UIButton p_button)
	{
		if (checkInternet() == false)
			return;

		p_button.removeClickCallback (toAppInfoPage);
		if(checkPin())
		{
			m_gameController.changeState (ZoodleState.SETTING_STATE);
		}
		else
		{
			p_button.addClickCallback (toAppInfoPage);
		}
	}

	private void addButtonClickCall( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_showProfileButton.addClickCallback (toShowAllChilren);
	}


	private void onSaveButtonClicked(UIButton p_button){


		if( InputFieldNewChildCode.text.Length != 4)
		{
			m_childLockChangeHelp.active = true;

			return ;
		}

		m_gameController.getUI().createScreen(UIScreen.LOADING_SPINNER_ELEPHANT, false, 6);

		//update childLock
		m_requestQueue.reset ();

		m_settingCache.childLockPassword = InputFieldNewChildCode.text;

		m_requestQueue.add(new EnableLockRequest(m_settingCache.childLockPassword, saveNewChildLockComplete));
		m_requestQueue.add(new UpdateSettingRequest("true"));

		m_requestQueue.request (RequestType.SEQUENCE);

		//SessionHandler.getInstance().resetSetting ();

		//update notifucation
//		m_requestQueue.add (new UpdateNotificateRequest(m_settingCache.newAddApp,m_settingCache.smartSelect,m_settingCache.freeWeeklyApp));
//		m_requestQueue.add (new UpdateDeviceOptionRequest(m_settingCache.allowCall?"true":"false",m_settingCache.tip?"true":"false",m_settingCache.masterVolum,m_settingCache.musicVolum,m_settingCache.effectsVolum));
//		m_requestQueue.request (RequestType.SEQUENCE);
//		//update device option
//		SessionHandler.getInstance().resetSetting ();
//		Debug.Log("SettingChildLock set volume data from setting cache to setting cache");
//		SoundManager.getInstance ().effectVolume = (float) SessionHandler.getInstance ().effectsVolum / 100;
//		SoundManager.getInstance ().musicVolume = (float) SessionHandler.getInstance ().musicVolum / 100;
//		SoundManager.getInstance ().masterVolume = (float) SessionHandler.getInstance ().masterVolum / 100;
//		PlayerPrefs.SetInt ("master_volume",SessionHandler.getInstance ().masterVolum);
//		PlayerPrefs.SetInt ("music_volume",SessionHandler.getInstance ().musicVolum);
//		PlayerPrefs.SetInt ("effects_volume",SessionHandler.getInstance ().effectsVolum);
//		PlayerPrefs.Save ();


	}

	private void saveNewChildLockComplete(HttpsWWW p_response)
	{

		if(p_response.error == null){

			m_currentPinDisplay.text = m_settingCache.childLockPassword;

			InputFieldNewChildCode.text = "";

			m_childLockChangeOKay.active = true;

			m_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);

			m_settingCache.verifyBirth = false;

			SessionHandler.getInstance().resetSetting ();

		}else{
			m_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);

			m_childLockChangeError.active = true;

		}



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

	private void toShowAllChilren(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		m_leftMenuCanvas.showKids (addButtonClickCall);
	}

	private void toPremiumScreen(UIButton p_button)
	{
		if(checkPin())
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
	}
	
	private void toBuyGemsScreen(UIButton p_button)
	{
		if(string.Empty.Equals(SessionHandler.getInstance().GemsJson))
		{
			Server.init (ZoodlesConstants.getHttpsHost());
			m_requestQueue.reset ();
			m_requestQueue.add (new ViewGemsRequest(viewGemsRequestComplete));
			m_requestQueue.request ();
		}
		else
		{
			if(checkPin())
			{
				m_gameController.connectState( ZoodleState.BUY_GEMS, int.Parse(m_gameController.stateName) );
				m_gameController.changeState (ZoodleState.BUY_GEMS);
			}
		}
	}

	private void viewGemsRequestComplete(HttpsWWW p_response)
	{
		Server.init (ZoodlesConstants.getHttpsHost());
		if(p_response.error == null)
		{
			if(checkPin())
			{
				SessionHandler.getInstance ().GemsJson = p_response.text;
				m_gameController.connectState( ZoodleState.BUY_GEMS, int.Parse(m_gameController.stateName) );
				m_gameController.changeState (ZoodleState.BUY_GEMS);
			}
		}
		else
		{
			setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_11_FAIL),Localization.getString(Localization.TXT_STATE_11_FAIL_DATA));
		}
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
	
	private UICanvas    m_childLockCanvas;
	private CancelLockConfirmCanvas	m_cancelLockConfirmCanvas;
	private UICanvas    m_dashboardCommonCanvas;
	private ChildLockHelpCanvas m_childLockHelpCanvas;
	private LeftMenuCanvas	m_leftMenuCanvas;
	private UIButton 	m_leftSideMenuButton;
	private UIButton 	m_showProfileButton;
	private UIButton	m_helpButton;
	private UIButton	m_closeButton;
	private UIElement 	m_menu;
	private UIElement	m_upsellPanel;
	private InputField  m_pinInputField;
	//honda
	private UIButton	m_settingButton;
	//end
	private UIButton	m_closeLeftMenuButton;
	private UIButton    m_leftButton;
	private UIToggle    m_verifyBirth;
	private UIButton    m_deviceButton;
	private UIButton	m_faqButton;
	private UIButton    m_childModeButton;
	private UIButton	m_cancelTurnOffButton;
	private UIButton	m_turnOffButton;
	private UIButton	m_closeTurnOnConfirmButton;
	private UISwipeList m_childrenList;
	private UIButton 	m_tryPremiumButton;
	private UIButton 	m_buyGemsButton;
	private UIButton 	m_premiumPurchaseButton;
	private UIButton 	m_buyNowButton;
	private RequestQueue m_requestQueue;
	private SettingCache m_settingCache;

	private UIToggle 	m_lockSwitchButton;
	private bool 		canMoveLeftMenu = true;

	//New UX changes for pin
	private UILabel 	m_currentPinDisplay;
	private UIButton 	m_saveNewPinButton;
	private UIInputField InputFieldNewChildCode;

	private UIImage 	m_childLockChangeOKay;
	private UIImage 	m_childLockChangeError;
	private UIImage 	m_childLockChangeHelp; // when telling the user we only accept 4 pins

}