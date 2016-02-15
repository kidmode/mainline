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
		  l_changedStateName == ZoodleState.CHILD_LOCK_STATE) && checkInternet())
			updateSetting ();
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_notificationCanvas );
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
		Debug.Log("Notification set volume data from setting cache to session handler");
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
		m_notificationCanvas = p_uiManager.createScreen( UIScreen.NOTIFICATION, true, 2 );

		m_helpButton = m_notificationCanvas.getView ("helpButton") as UIButton;
		m_helpButton.addClickCallback (onHelpButtonClick);


//		m_rightButton = m_notificationCanvas.getView ("rightButton") as UIButton;
//		m_rightButton.addClickCallback (toDeviceOptions);

		m_weeklyAppsNotificationButton = m_notificationCanvas.getView ("WeeklyAppsNotification") as UIToggle;
		m_smartSelectNotificationButton = m_notificationCanvas.getView ("smartSelectNotification") as UIToggle;
		m_newAppAddedButton = m_notificationCanvas.getView ("NewAppAddNotification") as UIToggle;
		m_weeklyAppsNotificationButton.addValueChangedCallback (onSetWeeklyApps);
		m_smartSelectNotificationButton.addValueChangedCallback (onSetSmartSelect);
		m_newAppAddedButton.addValueChangedCallback (onNewAppAdded);

//		m_generalButton.enabled = true;
//		m_faqButton.enabled = true;

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


	



	private void toDeviceOptions(UIButton p_button)
	{
		if (checkInternet() == false)
			return;

		p_button.addClickCallback (toDeviceOptions);
		m_gameController.changeState (ZoodleState.DEVICE_OPTIONS_STATE);
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
	
	private void onCloseMenuTweenFinished( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		canMoveLeftMenu = true;
	}
	
	private void toShowMenuTweenFinished( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		canMoveLeftMenu = true;
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
	
	private UICanvas    m_notificationCanvas;
	private CommonDialogCanvas m_commonDialog;

	private UIButton	m_closeLeftMenuButton;
//	private UIButton    m_rightButton;

	private UIButton 	m_helpButton;
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