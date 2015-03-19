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
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_childLockCanvas );
		p_gameController.getUI().removeScreen( m_dashboardCommonCanvas );
		p_gameController.getUI().removeScreen( m_leftMenuCanvas );
		p_gameController.getUI().removeScreen( m_childLockHelpCanvas );
		p_gameController.getUI().removeScreen( m_cancelLockConfirmCanvas );
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
				if(null != m_settingCache.childLockPassword && !string.Empty.Equals(m_settingCache.childLockPassword))
					m_pinInputField.text = m_settingCache.childLockPassword;
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
				m_verifyBirth.isOn = true;
			}
			else
			{
				m_verifyBirth.isOn = false;
				if(!string.Empty.Equals(SessionHandler.getInstance().childLockPassword))
					m_pinInputField.text = SessionHandler.getInstance().childLockPassword;
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
			m_gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName) );
			m_gameController.changeState( ZoodleState.VIEW_PREMIUM );
		}
	}

	private void viewPremiumRequestComplete(WWW p_response)
	{
		Server.init (ZoodlesConstants.getHost());
		if(null == p_response.error)
		{
			SessionHandler.getInstance ().PremiumJson = p_response.text;
			m_gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName) );
			m_gameController.changeState( ZoodleState.VIEW_PREMIUM );
		}
		else
		{
			setErrorMessage(m_gameController,"fail","Get date failed please try it again.");
		}
	}

	private void buyGemsRequestComplete(WWW p_response)
	{
		Server.init (ZoodlesConstants.getHost());
		if(null == p_response.error)
		{
			SessionHandler.getInstance ().GemsJson = p_response.text;
			m_gameController.connectState( ZoodleState.BUY_GEMS, int.Parse(m_gameController.stateName) );
			m_gameController.changeState (ZoodleState.BUY_GEMS);
		}
		else
		{
			setErrorMessage(m_gameController,"fail","Get date failed please try it again.");
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
		m_gameController.getUI ().changeScreen (UIScreen.CHILD_LOCK_HELP,false);
		p_button.removeClickCallback (closeHelpDialog);
		m_childLockHelpCanvas.setOutPosition ();
		m_helpButton.addClickCallback (showHelpDialog);
	}

	private void onSelectThisChild(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		Kid l_kid = p_data as Kid;
		if (ZoodlesConstants.ADD_CHILD_TEXT.Equals (l_kid.name))
		{
			m_gameController.changeState(ZoodleState.CREATE_CHILD);
		}
		else
		{
			List<Kid> l_kidList = SessionHandler.getInstance().kidList;
			SessionHandler.getInstance().currentKid = l_kidList[p_index-1];
			m_gameController.changeState(ZoodleState.OVERVIEW_INFO);
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

	private void toDeviceScreen(UIButton p_button)
	{
		p_button.removeClickCallback (toDeviceScreen);
		m_gameController.changeState (ZoodleState.NOTIFICATION_STATE);
	}

	private void toFAQScreen(UIButton p_button)
	{
		p_button.removeClickCallback (toFAQScreen);
		m_gameController.changeState (ZoodleState.FAQ_STATE);
	}

	private void toChildMode(UIButton p_button)
	{
		m_gameController.changeState (ZoodleState.PROFILE_SELECTION);
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
		p_button.removeClickCallback (toAppInfoPage);
		m_gameController.changeState (ZoodleState.SETTING_STATE);
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

	private void toPremiumScreen(UIButton p_button)
	{
		m_gameController.connectState (ZoodleState.SIGN_IN_UPSELL, int.Parse(m_gameController.stateName));
		m_gameController.changeState (ZoodleState.SIGN_IN_UPSELL);
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
			setErrorMessage(m_gameController,"fail","Get date failed please try it again.");
		}
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
}