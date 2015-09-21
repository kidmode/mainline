using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FAQState : GameState 
{
	//Public variables
	
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );
		
		m_requestQueue = new RequestQueue ();

		_setupScreen( p_gameController.getUI() );

	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_dashboardCommonCanvas );
		p_gameController.getUI().removeScreen( m_leftMenuCanvas );
		p_gameController.getUI().removeScreen( m_faqCanvas );
		p_gameController.getUI().removeScreen( m_faqDialogCanvas );
		p_gameController.getUI().removeScreen( m_commonDialog );
	}
	
	//---------------- Private Implementation ----------------------

	private void _setupScreen( UIManager p_uiManager )
	{
		m_commonDialog 	= p_uiManager.createScreen( UIScreen.COMMON_DIALOG, false, 7 ) as CommonDialogCanvas;
		m_commonDialog.setUIManager (p_uiManager);
		m_faqDialogCanvas = p_uiManager.createScreen (UIScreen.FAQ_DIALOG, true, 5);
		m_leftMenuCanvas = p_uiManager.createScreen (UIScreen.LEFT_MENU, true, 3) as LeftMenuCanvas;
		m_faqCanvas = p_uiManager.createScreen( UIScreen.FAQ_SCREEN, true, 2 ) as UICanvas;
		m_dashboardCommonCanvas = p_uiManager.createScreen( UIScreen.SETTING_COMMON, true, 1 );

		m_helpButton = m_faqCanvas.getView ("helpButton") as UIButton;
		m_helpButton.addClickCallback (onHelpButtonClick);
		m_faqSwipeList = m_faqCanvas.getView ("FAQSwipeList") as UISwipeList;
		m_faqSwipeList.addClickListener ( "Prototype", showDialog);

		m_childModeButton = m_dashboardCommonCanvas.getView ("childModelButton") as UIButton;
		m_childModeButton.addClickCallback (toChildMode);
		m_menu = m_leftMenuCanvas.getView ("LeftMenu") as UIElement;
		m_leftSideMenuButton = m_dashboardCommonCanvas.getView ("menuButton") as UIButton;
		m_leftSideMenuButton.addClickCallback (toShowMenu);
		m_showProfileButton = m_menu.getView ("profileButton") as UIButton;
		m_showProfileButton.addClickCallback (toShowAllChilren);
		//m_sliderDownPanel = m_menu.getView ("sildeDownPanel") as UIElement;

		m_questionLabel = m_faqDialogCanvas.getView("questionText") as UILabel;
	//	m_answerLabel 	= m_faqDialogCanvas.getView("answerText") 	as UILabel;
		m_titleLabel 	= m_faqDialogCanvas.getView("titleText") 	as UILabel;
		m_exitButton 	= m_faqDialogCanvas.getView("exitButton") 	as UIButton;
		m_exitButton.addClickCallback (onCloseDialog);

		//honda 
		m_settingButton = m_leftMenuCanvas.getView ("settingButton") as UIButton;
		m_settingButton.addClickCallback(onCloseMenu);
		//end
		m_closeLeftMenuButton = m_leftMenuCanvas.getView ("closeButton") as UIButton;
		m_closeLeftMenuButton.addClickCallback (onCloseMenu);

		m_generalButton = m_dashboardCommonCanvas.getView ("overviewButton") as UIButton;
		m_generalButton.addClickCallback (toGeneralOptions);

		m_deviceButton = m_dashboardCommonCanvas.getView ("controlButton") as UIButton;
		m_deviceButton.addClickCallback (toDeviceOptions);

		UIButton l_FAQButton = m_dashboardCommonCanvas.getView ("starButton") as UIButton;
		m_generalButton.enabled = true;
		m_deviceButton.enabled = true;
		l_FAQButton.enabled = false;
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
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_30_HELP_TITLE);
		l_contentLabel.text = Localization.getString(Localization.TXT_STATE_30_HELP_CONTENT);

		l_closeButton.addClickCallback (onCloseDialogButtonClick);
	}
	
	private void onCloseDialogButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		m_commonDialog.setOutPosition ();
		m_helpButton.addClickCallback (onHelpButtonClick);
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

	private void toShowMenu(UIButton p_button)
	{
		if(canMoveLeftMenu &&  checkInternet())
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
		if (checkInternet() == false)
			return;

		p_button.removeAllCallbacks ();
		m_gameController.changeState (ZoodleState.NOTIFICATION_STATE);
	}

	private void toGeneralOptions(UIButton p_button)
	{
		if (checkInternet() == false)
			return;

		p_button.removeClickCallback (toGeneralOptions);
		m_gameController.changeState (ZoodleState.SETTING_STATE);
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

	private void onCloseDialog(UIButton p_button)
	{
		m_gameController.getUI().changeScreen(UIScreen.FAQ_DIALOG,false);
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_newPanel = m_faqDialogCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_newPanel.transform.localPosition );
		l_pointListOut.Add( l_newPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_newPanel.tweener.addPositionTrack(l_pointListOut, 0.0f);
	}

	private void showDialog( UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index )
	{
		m_gameController.getUI ().changeScreen (UIScreen.FAQ_DIALOG,true);
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_faqDialogCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_newPanel.tweener.addPositionTrack(l_pointListIn, 0.0f);

		string[] l_data = p_data as string[];

		m_questionLabel.text = l_data[1];
		m_titleLabel.text = "FAQ" + (p_index + 1).ToString ();

		UILabel l_faqContent = m_faqDialogCanvas.getView (l_data[0].ToString()) as UILabel;
		if (null != l_faqContent)
		{
			if(null != m_showFaq)
				m_showFaq.active = false;
			l_faqContent.active = true;
			m_showFaq = l_faqContent;
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

	private UICanvas 	m_faqDialogCanvas;
	private UICanvas    m_faqCanvas;
	private UICanvas    m_dashboardCommonCanvas;
	private CommonDialogCanvas m_commonDialog;
	private UISwipeList m_faqSwipeList;


	private LeftMenuCanvas	m_leftMenuCanvas;

	private UIButton 	m_leftSideMenuButton;
	private UIButton 	m_helpButton;
	private UIButton 	m_showProfileButton;
	private UIElement 	m_menu;

	//honda
	private UIButton	m_settingButton;
	//end
	private UIButton	m_closeLeftMenuButton;
	private UIButton    m_childModeButton;

	private UISwipeList m_childrenList;
	
	private UIButton 	m_tryPremiumButton;
	private UIButton 	m_buyGemsButton;
	private RequestQueue m_requestQueue;

	private UIButton    m_generalButton;
	private UIButton	m_deviceButton;
	private bool 		canMoveLeftMenu = true;

	//dialog part
	private UILabel 	m_questionLabel;
	private UILabel 	m_answerLabel;
	private UILabel 	m_titleLabel;
	private UIButton 	m_exitButton;
	private UILabel 	m_showFaq;
}