using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DeviceOptionsState : GameState 
{
	//Public variables
	
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );
		
//		#if UNITY_ANDROID && !UNITY_EDITOR
//		m_changeState = true;
//		#endif 
		m_settingCache = SessionHandler.getInstance ().settingCache;
		m_requestQueue = new RequestQueue ();
		exitState = false;
		_setupScreen( p_gameController.getUI() );
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
		  l_changedStateName == ZoodleState.NOTIFICATION_STATE || 
		  l_changedStateName == ZoodleState.CHILD_LOCK_STATE) && checkInternet())
		{
			exitState = true;
			updateSetting ();
		}
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_dashboardCommonCanvas );
		p_gameController.getUI().removeScreen( m_leftMenuCanvas );
		p_gameController.getUI().removeScreen( m_deviceOptionCanvas );
		p_gameController.getUI().removeScreen( m_commonDialog );
	}
	
	//---------------- Private Implementation ----------------------

	private void _setupScreen( UIManager p_uiManager )
	{
		m_commonDialog 	= p_uiManager.createScreen( UIScreen.COMMON_DIALOG, false, 5 ) as CommonDialogCanvas;
		m_commonDialog.setUIManager (p_uiManager);
		m_leftMenuCanvas = p_uiManager.createScreen (UIScreen.LEFT_MENU, true, 3) as LeftMenuCanvas;
		m_deviceOptionCanvas = p_uiManager.createScreen( UIScreen.DEVICE_OPTIONS_SCREEN, true, 2 ) as UICanvas;
		m_dashboardCommonCanvas = p_uiManager.createScreen( UIScreen.SETTING_COMMON, true, 1 );
		m_lodaing = m_deviceOptionCanvas.getView ("noticePanel") as UIElement;

		m_helpButton = m_deviceOptionCanvas.getView ("helpButton") as UIButton;
		m_helpButton.addClickCallback (onHelpButtonClick);
		m_childModeButton = m_dashboardCommonCanvas.getView ("childModelButton") as UIButton;
		m_childModeButton.addClickCallback (toChildMode);
		m_menu = m_leftMenuCanvas.getView ("LeftMenu") as UIElement;
		m_leftSideMenuButton = m_dashboardCommonCanvas.getView ("menuButton") as UIButton;
		m_leftSideMenuButton.addClickCallback (toShowMenu);
		m_showProfileButton = m_menu.getView ("profileButton") as UIButton;
		m_showProfileButton.addClickCallback (toShowAllChilren);
	//	m_sliderDownPanel = m_menu.getView ("sildeDownPanel") as UIElement;

		//honda 
		m_settingButton = m_leftMenuCanvas.getView ("settingButton") as UIButton;
		m_settingButton.addClickCallback(onCloseMenu);
		//end
		m_closeLeftMenuButton = m_leftMenuCanvas.getView ("closeButton") as UIButton;
		m_closeLeftMenuButton.addClickCallback (onCloseMenu);
		m_allowCallButton = m_deviceOptionCanvas.getView ("AllowCallButton") as UIToggle;
		m_allowCallButton.isOn = false;
		m_allowCallButton.addValueChangedCallback (toAllowCall);
		m_tipButton = m_deviceOptionCanvas.getView ("tipButton") as UIToggle;
		m_tipButton.isOn = false;
		m_tipButton.addValueChangedCallback (toTip);
		m_generalButton = m_dashboardCommonCanvas.getView ("overviewButton") as UIButton;
		m_generalButton.addClickCallback (toGeneral);
		m_FAQButton = m_dashboardCommonCanvas.getView ("starButton") as UIButton;
		m_FAQButton.addClickCallback (toFAQ);

		UIButton l_deviceOptionButton = m_dashboardCommonCanvas.getView ("controlButton") as UIButton;
		l_deviceOptionButton.enabled = false;
		m_generalButton.enabled = true;
		m_FAQButton.enabled = true;

		m_musicVolumeSlider = m_deviceOptionCanvas.getView ("musicVolumeSlider") as UISlider;
		m_masterVolumeSlider = m_deviceOptionCanvas.getView ("masterVolumeSlider") as UISlider;
		m_effectsVolumeSlider = m_deviceOptionCanvas.getView ("effectsVolumeSlider") as UISlider;
		m_musicVolumeSlider.addValueChangedCallback (onMusicVolumeValueChanged);
		m_masterVolumeSlider.addValueChangedCallback (onMasterVolumeValueChanged);
		m_effectsVolumeSlider.addValueChangedCallback (onEffectVolumeValueChanged);
		m_leftButton = m_deviceOptionCanvas.getView ("leftButton") as UIButton;
		m_leftButton.addClickCallback (toNotificationScreen);
		m_noticeLabel = m_lodaing.getView ("noticeText") as UILabel;
		m_closeButton = m_lodaing.getView ("closeButton") as UIButton;
		m_closeButton.active = false;
		m_noticeLabel.text = Localization.getString(Localization.TXT_LABEL_LOADING);
		m_closeButton.addClickCallback (closeNoticeDialog);
		m_refreshButton = m_deviceOptionCanvas.getView ("refreshButton") as UIButton;
		m_refreshButton.addClickCallback (toRefreshContent);

		if(m_settingCache.active)
		{
			m_musicVolumeSlider.value   =(float) m_settingCache.musicVolum;
			m_masterVolumeSlider.value  =(float) m_settingCache.masterVolum;
			m_effectsVolumeSlider.value =(float) m_settingCache.effectsVolum;
			Debug.Log("volume contol: use setting cache");
			if(m_settingCache.allowCall)
			{
				m_allowCallButton.isOn = true;
			}
			else
			{
				m_allowCallButton.isOn = false;
			}
			
			if(m_settingCache.tip)
			{
				m_tipButton.isOn = true;
			}
			else
			{
				m_tipButton.isOn = false;
			}
		}
		else
		{
			m_musicVolumeSlider.value   =(float) SessionHandler.getInstance ().musicVolum;
			m_masterVolumeSlider.value  =(float) SessionHandler.getInstance ().masterVolum;
			m_effectsVolumeSlider.value =(float) SessionHandler.getInstance ().effectsVolum;
			Debug.Log("volume contol: use session handler");
			if(SessionHandler.getInstance().allowCall)
			{
				m_allowCallButton.isOn = true;
			}
			else
			{
				m_allowCallButton.isOn = false;
			}
			
			if(SessionHandler.getInstance().tip)
			{
				m_tipButton.isOn = true;
			}
			else
			{
				m_tipButton.isOn = false;
			}
		}

		m_childrenList = m_leftMenuCanvas.getView ("childSwipeList") as UISwipeList;
		m_childrenList.addClickListener ("Prototype",onSelectThisChild);

		m_tryPremiumButton = m_leftMenuCanvas.getView ("premiumButton") as UIButton;
		m_buyGemsButton = m_leftMenuCanvas.getView ("buyGemsButton") as UIButton;
		m_tryPremiumButton.addClickCallback (toPremiumScreen);
		m_buyGemsButton.addClickCallback (toBuyGemsScreen);
	}
	
	private void onMasterVolumeValueChanged( float p_float )
	{
		m_settingCache.masterVolum =  (int)p_float;
	}

	private void onMusicVolumeValueChanged( float p_float )
	{
		m_settingCache.musicVolum =  (int)p_float;
	}

	private void onEffectVolumeValueChanged( float p_float )
	{
		m_settingCache.effectsVolum =  (int)p_float;
	}

	private void onHelpButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks ();
		m_commonDialog.setOriginalPosition ();
		UIButton l_closeButton = m_commonDialog.getView ("closeMark") as UIButton;
		
		UILabel l_titleLabel = m_commonDialog.getView ("dialogText") as UILabel;
		UILabel l_contentLabel = m_commonDialog.getView ("contentText") as UILabel;
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_31_HELP_TITLE);
		l_contentLabel.text = Localization.getString(Localization.TXT_STATE_31_HELP_CONTENT);

		l_closeButton.addClickCallback (onCloseDialogButtonClick);
	}

	private void closeNoticeDialog(UIButton p_button)
	{
		m_refreshButton.addClickCallback (toRefreshContent);
		m_noticeLabel.text = Localization.getString(Localization.TXT_STATE_31_LOADING);
		p_button.active = false;
		List<Vector3> l_pointListIn = new List<Vector3>();
		l_pointListIn.Add( m_lodaing.transform.localPosition );
		l_pointListIn.Add( m_lodaing.transform.localPosition - new Vector3( 0, 800, 0 ));
		m_lodaing.tweener.addPositionTrack(l_pointListIn, 0.0f);
	}
	
	private void onCloseDialogButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		m_commonDialog.setOutPosition ();
		m_helpButton.addClickCallback (onHelpButtonClick);
	}

	private void toBuyGemsScreen(UIButton p_button)
	{
		p_button.removeClickCallback (toBuyGemsScreen);
		if(string.Empty.Equals(SessionHandler.getInstance().GemsJson))
		{
			Server.init (ZoodlesConstants.getHttpsHost());
			m_requestQueue.reset();
			m_requestQueue.add(new ViewGemsRequest(viewGemsRequestComplete));
			m_requestQueue.request();
		}
		else
		{
			m_gameController.connectState( ZoodleState.BUY_GEMS, int.Parse(m_gameController.stateName) );
			m_gameController.changeState (ZoodleState.BUY_GEMS);
		}
	}

	private void toRefreshContent(UIButton p_button)
	{
		if (checkInternet() == false)
			return;

		int l_changedStateName = int.Parse( m_gameController.stateName);
		if (SessionHandler.getInstance ().settingCache.active && 
						!(l_changedStateName == ZoodleState.SETTING_STATE || 
						l_changedStateName == ZoodleState.NOTIFICATION_STATE || 
						l_changedStateName == ZoodleState.CHILD_LOCK_STATE))
		{
			updateSetting ();
			p_button.removeAllCallbacks ();
			List<Vector3> l_pointListIn = new List<Vector3>();
			l_pointListIn.Add( m_lodaing.transform.localPosition );
			l_pointListIn.Add( m_lodaing.transform.localPosition + new Vector3( 0, 800, 0 ));
			m_lodaing.tweener.addPositionTrack(l_pointListIn, 0.0f);
		}
	}

	public void updateSetting()
	{
		//update childLock
		m_requestQueue.reset ();
		if(m_settingCache.childLockSwitch && !m_settingCache.verifyBirth)
		{
			if(m_settingCache.childLockPassword.Length != 4)
			{
				//m_noticeLabel.text = Localization.getString(Localization.TXT_STATE_31_PIN);
				//m_closeButton.active = true;
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
		m_requestQueue.add (new UpdateDeviceOptionRequest(m_settingCache.allowCall?"true":"false",m_settingCache.tip?"true":"false",m_settingCache.masterVolum,m_settingCache.musicVolum,m_settingCache.effectsVolum,updateDeviceRequestComplete));
		m_requestQueue.request (RequestType.SEQUENCE);
		//update device option
		SessionHandler.getInstance().resetSetting ();
		Debug.Log("DeviceOption set volume data from setting cache to session handler");
		SoundManager.getInstance ().effectVolume = (float) SessionHandler.getInstance ().effectsVolum / 100;
		SoundManager.getInstance ().musicVolume = (float) SessionHandler.getInstance ().musicVolum / 100;
		SoundManager.getInstance ().masterVolume = (float) SessionHandler.getInstance ().masterVolum / 100;
		PlayerPrefs.SetInt ("master_volume",SessionHandler.getInstance ().masterVolum);
		PlayerPrefs.SetInt ("music_volume",SessionHandler.getInstance ().musicVolum);
		PlayerPrefs.SetInt ("effects_volume",SessionHandler.getInstance ().effectsVolum);
		PlayerPrefs.Save ();

		Debug.Log("updated master_volume volume: " + PlayerPrefs.GetInt("master_volume"));
		Debug.Log("updated music_volume volume: " + PlayerPrefs.GetInt("music_volume"));
		Debug.Log("updated effects_volume volume: " + PlayerPrefs.GetInt("effects_volume"));

	}
	
	private void toPremiumScreen(UIButton p_button)
	{
		updateSetting();

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

	private void onSelectThisChild(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		if (checkInternet() == false)
			return;

		Kid l_kid = p_data as Kid;
		if (Localization.getString(Localization.TXT_86_BUTTON_ADD_CHILD).Equals (l_kid.name))
		{
			updateSetting ();
			m_gameController.connectState(ZoodleState.CREATE_CHILD_NEW,int.Parse(m_gameController.stateName));
			m_gameController.changeState (ZoodleState.CREATE_CHILD_NEW);
		}
		else
		{
			updateSetting ();
			List<Kid> l_kidList = SessionHandler.getInstance().kidList;
			SessionHandler.getInstance().currentKid = l_kidList[p_index-1];
			m_gameController.changeState(ZoodleState.OVERVIEW_INFO);
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

	private void toNotificationScreen(UIButton p_button)
	{
		if (checkInternet() == false)
			return;

		//p_button.removeClickCallback (toNotificationScreen);
		m_gameController.changeState (ZoodleState.NOTIFICATION_STATE);
	}

	private void toGeneral(UIButton p_button)
	{
		if (checkInternet() == false)
			return;

		p_button.removeClickCallback (toGeneral);
		m_gameController.changeState (ZoodleState.SETTING_STATE);
	}

	private void toFAQ(UIButton p_button)
	{
		p_button.removeClickCallback (toFAQ);
		m_gameController.changeState (ZoodleState.FAQ_STATE);
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

	private void addButtonClickCall( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_showProfileButton.addClickCallback (toShowAllChilren);
	}

	private void toAllowCall(UIToggle p_toggle, bool p_value)
	{
		if( false == p_value )
		{
			m_allowCall = false;

			IncomingCallControl.StartBlock();

			m_settingCache.allowCall = false;
		}
		else
		{
			m_allowCall = true;

			IncomingCallControl.EndBlock();

			m_settingCache.allowCall = true;
		}
	}

	private void toTip(UIToggle p_toggle, bool p_value)
	{
		if( false == p_value )
		{
			m_allowTip = false;
			m_settingCache.tip = false;
		}
		else
		{
			m_allowTip = true;
			m_settingCache.tip = true;
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

	private void updateDeviceRequestComplete(WWW p_response)
	{
		if(!exitState)
		{
			if(null != m_noticeLabel)
				m_noticeLabel.text = Localization.getString(Localization.TXT_STATE_31_UPDATE);
			if(null != m_closeButton)
				m_closeButton.active = true;
		}
	}

	private void viewGemsRequestComplete(WWW p_response)
	{
		Server.init (ZoodlesConstants.getHost());
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
	
	private UICanvas    m_deviceOptionCanvas;
	private UICanvas    m_dashboardCommonCanvas;
	private LeftMenuCanvas	m_leftMenuCanvas;
	private CommonDialogCanvas m_commonDialog;
	private UIButton 	m_leftSideMenuButton;
	private UIButton 	m_showProfileButton;
	private UIElement 	m_menu;
	private UIElement	m_lodaing;
	private UILabel		m_noticeLabel;
	private UIButton	m_closeButton;
//	private UIElement 	m_sliderDownPanel;
	//honda
	private UIButton	m_settingButton;
	//end
	private UIButton	m_closeLeftMenuButton;
	private UIButton    m_childModeButton;
	private UIButton    m_leftButton;
	private int 		m_volume;
	private UIToggle	m_allowCallButton;
	private UIToggle	m_tipButton;
	private UISlider 	m_musicVolumeSlider;
	private UISlider 	m_masterVolumeSlider;
	private UISlider 	m_effectsVolumeSlider;
	private UIButton    m_generalButton;
	private UIButton    m_FAQButton;
	private UIButton 	m_helpButton;
	private UIButton	m_deviceButton;
	private bool 		m_allowCall;
	private bool 		m_allowTip;
	private UISwipeList m_childrenList;
	private UIButton 	m_tryPremiumButton;
	private UIButton 	m_buyGemsButton;
	private UIButton	m_refreshButton;
	private RequestQueue m_requestQueue;
	private SettingCache m_settingCache;
//	#if UNITY_ANDROID && !UNITY_EDITOR
//	private bool 		m_changeState = true;
//	#endif
	private bool 		canMoveLeftMenu = true;
	private bool 		exitState = false;
}