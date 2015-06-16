using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NotificationState : GameState 
{
	//Public variables
	
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );
		#if UNITY_ANDROID && !UNITY_EDITOR
		m_changeState = true;
		#endif 

		m_settingCache = SessionHandler.getInstance ().settingCache;

		_setupScreen( p_gameController.getUI() );

		m_requestQueue = new RequestQueue ();
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
		  l_changedStateName == ZoodleState.CHILD_LOCK_STATE))
			updateSetting ();
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_notificationCanvas );
		p_gameController.getUI().removeScreen( m_dashboardCommonCanvas );
		p_gameController.getUI().removeScreen( m_leftMenuCanvas );
		p_gameController.getUI().removeScreen( m_commonDialog );
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
		m_requestQueue.add (new UpdateDeviceOptionRequest(m_settingCache.allowCall?"true":"false",m_settingCache.tip?"true":"false",m_settingCache.masterVolum,m_settingCache.musicVolum,m_settingCache.effectsVolum));
		m_requestQueue.request (RequestType.SEQUENCE);
		//update device option
		SessionHandler.getInstance().resetSetting ();
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
		m_commonDialog 	= p_uiManager.createScreen( UIScreen.COMMON_DIALOG, false, 5 ) as CommonDialogCanvas;
		m_commonDialog.setUIManager (p_uiManager);
		m_leftMenuCanvas = p_uiManager.createScreen (UIScreen.LEFT_MENU, true, 3) as LeftMenuCanvas;
		m_notificationCanvas = p_uiManager.createScreen( UIScreen.NOTIFICATION, true, 2 );

		m_dashboardCommonCanvas = p_uiManager.createScreen( UIScreen.SETTING_COMMON, true, 1 );

		m_helpButton = m_notificationCanvas.getView ("helpButton") as UIButton;
		m_helpButton.addClickCallback (onHelpButtonClick);
		m_childModeButton = m_dashboardCommonCanvas.getView ("childModelButton") as UIButton;
		m_childModeButton.addClickCallback (toChildMode);
		m_menu = m_leftMenuCanvas.getView ("LeftMenu") as UIElement;
		m_leftSideMenuButton = m_dashboardCommonCanvas.getView ("menuButton") as UIButton;
		m_leftSideMenuButton.addClickCallback (toShowMenu);
		m_showProfileButton = m_menu.getView ("profileButton") as UIButton;
		m_showProfileButton.addClickCallback (toShowAllChilren);

		m_closeLeftMenuButton = m_leftMenuCanvas.getView ("closeButton") as UIButton;
		m_closeLeftMenuButton.addClickCallback (onCloseMenu);

		m_rightButton = m_notificationCanvas.getView ("rightButton") as UIButton;
		m_rightButton.addClickCallback (toDeviceOptions);

		m_weeklyAppsNotificationButton = m_notificationCanvas.getView ("WeeklyAppsNotification") as UIToggle;
		m_smartSelectNotificationButton = m_notificationCanvas.getView ("smartSelectNotification") as UIToggle;
		m_newAppAddedButton = m_notificationCanvas.getView ("NewAppAddNotification") as UIToggle;
		m_weeklyAppsNotificationButton.addValueChangedCallback (onSetWeeklyApps);
		m_smartSelectNotificationButton.addValueChangedCallback (onSetSmartSelect);
		m_newAppAddedButton.addValueChangedCallback (onNewAppAdded);

		m_generalButton = m_dashboardCommonCanvas.getView ("overviewButton") as UIButton;
		m_generalButton.addClickCallback (toGeneralScreen);
		
		m_faqButton = m_dashboardCommonCanvas.getView ("starButton") as UIButton;
		m_faqButton.addClickCallback (toFAQScreen);

		UIButton l_deviceOptionButton = m_dashboardCommonCanvas.getView ("controlButton") as UIButton;
		l_deviceOptionButton.enabled = false;
		m_generalButton.enabled = true;
		m_faqButton.enabled = true;

		if(m_settingCache.active)
		{
			if(m_settingCache.freeWeeklyApp)
			{
				m_weeklyAppsNotificationButton.isOn = true;
			}
			else
			{
				m_weeklyAppsNotificationButton.isOn = false;
			}
			
			if(m_settingCache.smartSelect)
			{
				m_smartSelectNotificationButton.isOn = true;
			}
			else
			{
				m_smartSelectNotificationButton.isOn = false;
			}
			
			if(m_settingCache.newAddApp)
			{
				m_newAppAddedButton.isOn = true;
			}
			else
			{
				m_newAppAddedButton.isOn = false;
			}
		}
		else
		{
			if(SessionHandler.getInstance().freeWeeklyApp)
			{
				m_weeklyAppsNotificationButton.isOn = true;
			}
			else
			{
				m_weeklyAppsNotificationButton.isOn = false;
			}
			
			if(SessionHandler.getInstance().smartSelect)
			{
				m_smartSelectNotificationButton.isOn = true;
			}
			else
			{
				m_smartSelectNotificationButton.isOn = false;
			}
			
			if(SessionHandler.getInstance().newAddApp)
			{
				m_newAppAddedButton.isOn = true;
			}
			else
			{
				m_newAppAddedButton.isOn = false;
			}
		}

		m_childrenList = m_leftMenuCanvas.getView ("childSwipeList") as UISwipeList;
		m_childrenList.addClickListener ("Prototype",onSelectThisChild);

		m_tryPremiumButton = m_leftMenuCanvas.getView ("premiumButton") as UIButton;
		m_buyGemsButton = m_leftMenuCanvas.getView ("buyGemsButton") as UIButton;
		m_tryPremiumButton.addClickCallback (toPremiumScreen);
		m_buyGemsButton.addClickCallback (toBuyGemsScreen);
	}

	private void onHelpButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks ();
		m_commonDialog.setOriginalPosition ();
		UIButton l_closeButton = m_commonDialog.getView ("closeMark") as UIButton;
		
		UILabel l_titleLabel = m_commonDialog.getView ("dialogText") as UILabel;
		UILabel l_contentLabel = m_commonDialog.getView ("contentText") as UILabel;
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_28_HELP_TITLE);
		l_contentLabel.text = Localization.getString(Localization.TXT_STATE_28_HELP_CONTENT);

		l_closeButton.addClickCallback (onCloseDialogButtonClick);
	}
	
	private void onCloseDialogButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		m_commonDialog.setOutPosition ();
		m_helpButton.addClickCallback (onHelpButtonClick);
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
			m_gameController.connectState( ZoodleState.BUY_GEMS, int.Parse(m_gameController.stateName) );
			m_gameController.changeState (ZoodleState.BUY_GEMS);
		}
	}
	
	private void toPremiumScreen(UIButton p_button)
	{
		m_gameController.connectState (ZoodleState.SIGN_IN_UPSELL, int.Parse(m_gameController.stateName));
		m_gameController.changeState (ZoodleState.SIGN_IN_UPSELL);
	}

	private void onSelectThisChild(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		Kid l_kid = p_data as Kid;
		if (Localization.getString(Localization.TXT_86_BUTTON_ADD_CHILD).Equals (l_kid.name))
		{
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

	private void toShowMenu(UIButton p_button)
	{
		if(canMoveLeftMenu)
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

	private void toDeviceOptions(UIButton p_button)
	{
		p_button.addClickCallback (toDeviceOptions);
		m_gameController.changeState (ZoodleState.DEVICE_OPTIONS_STATE);
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

	private void setValue(Hashtable p_hashTable,string p_fieldName ,bool p_condition)
	{
		if(p_condition)
		{
			p_hashTable.Add (p_fieldName, "true");
		}
		else
		{
			p_hashTable.Add (p_fieldName, "false");
		}
	}

	private void onSetWeeklyApps(UIToggle p_toggle, bool p_value)
	{
		m_settingCache.active = true;
		if( false == p_value )
		{
//			m_weeklyAppsNotification = false;
			m_settingCache.freeWeeklyApp = false;
		}
		else
		{
//			m_weeklyAppsNotification = true;
			m_settingCache.freeWeeklyApp = true;
		}
	}

	private void toGeneralScreen(UIButton p_button)
	{
		p_button.removeClickCallback (toGeneralScreen);
		m_gameController.changeState (ZoodleState.SETTING_STATE);
	}
	
	private void toFAQScreen(UIButton p_button)
	{
		p_button.removeClickCallback (toFAQScreen);
		m_gameController.changeState (ZoodleState.FAQ_STATE);
	}

	private void onSetSmartSelect(UIToggle p_toggle, bool p_value)
	{
		m_settingCache.active = true;
		if( false == p_value )
		{
//			m_smartSelectNotification = false;
			m_settingCache.smartSelect = false;
		}
		else
		{
//			m_smartSelectNotification = true;
			m_settingCache.smartSelect = true;
		}
	}

	private void onNewAppAdded(UIToggle p_toggle, bool p_value)
	{
		m_settingCache.active = true;
		if( false == p_value )
		{
//			m_newAppAdded = false;
			m_settingCache.newAddApp = false;
		}
		else
		{
//			m_newAppAdded = true;
			m_settingCache.newAddApp = true;
		}
	}

	private void addButtonClickCall( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_showProfileButton.addClickCallback (toShowAllChilren);
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
	//Private variables
	
	private UICanvas    m_notificationCanvas;
	private UICanvas    m_dashboardCommonCanvas;
	private LeftMenuCanvas	m_leftMenuCanvas;
	private CommonDialogCanvas m_commonDialog;
	private UIButton 	m_leftSideMenuButton;
	private UIButton 	m_showProfileButton;
	private UIElement 	m_menu;
	private UIButton	m_closeLeftMenuButton;
	private UIButton    m_rightButton;
	private UIButton    m_childModeButton;
	private UISwipeList m_childrenList;
	private UIButton    m_generalButton;
	private UIButton	m_faqButton;
	private UIButton 	m_helpButton;
	private UIButton 	m_tryPremiumButton;
	private UIButton 	m_buyGemsButton;
	private UIToggle 	m_weeklyAppsNotificationButton;
	private UIToggle 	m_smartSelectNotificationButton;
	private UIToggle 	m_newAppAddedButton;
	private RequestQueue m_requestQueue;
	private SettingCache m_settingCache;
	#if UNITY_ANDROID && !UNITY_EDITOR
	private bool 		m_changeState = true;
	#endif

//	private bool 		m_weeklyAppsNotification = false;
//	private bool 		m_smartSelectNotification = false;
//	private bool 		m_newAppAdded = false;
	private bool 		canMoveLeftMenu = true;

	private int 		m_state;
}