using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverviewProgressState : GameState {

	
	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);
		
		m_uiManager = m_gameController.getUI();
		
		m_requestQueue = new RequestQueue();
		_setupScreen( p_gameController );
		_setupElment();
	}
	
	public override void update (GameController p_gameController, int p_time)
	{
		base.update (p_gameController, p_time);
	}
	
	public override void exit (GameController p_gameController)
	{
		base.exit (p_gameController);

		m_requestQueue.dispose ();
		
		m_uiManager.removeScreen( UIScreen.DASHBOARDL_PROGRESS );
		m_uiManager.removeScreen( UIScreen.COMMON_DIALOG );

		m_uiManager.removeScreen( UIScreen.PAYWALL );
	}
	
	//----------------- Private Implementation -------------------
	
	private void _setupScreen( GameController p_gameController )
	{
		m_commonDialog = m_uiManager.createScreen( UIScreen.COMMON_DIALOG, true, 5 )  as CommonDialogCanvas;

		m_commonDialog.setUIManager (p_gameController.getUI());


		if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
		{
			m_paywallCanvas = m_uiManager.createScreen( UIScreen.PAYWALL, false, 2 );
			m_upgradeButton = m_paywallCanvas.getView( "upgradeButton" ) as UIButton;
			m_upgradeButton.addClickCallback( onUpgradeButtonClick );
		}
		
		m_overallProgressCanvas = m_uiManager.createScreen( UIScreen.DASHBOARDL_PROGRESS, true, 1 ) as OverallProgressCanvas;
		

		if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
		{
			m_uiManager.setScreenEnable( UIScreen.DASHBOARDL_PROGRESS, false );
		}
	}

	private void _setupElment()
	{

		m_helpButton = m_overallProgressCanvas.getView ("helpButton") as UIButton;
		m_helpButton.addClickCallback (onHelpButtonClick);
		
		UIElement l_newPanel = m_overallProgressCanvas.getView ("mainPanel");
		List<Vector3> l_pointListIn = new List<Vector3>();
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 830, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, 0.5f);


		m_promoteButton = m_overallProgressCanvas.getView( "promoteButton" ) as UIButton;
		m_promoteButton.addClickCallback( onPromoteButtonClick );

		if( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() )
		{
		m_requestQueue.reset();
		m_requestQueue.add( new GetTimeSpendRequest(_getTimeSpendRequestComplete) );
		m_requestQueue.request( RequestType.RUSH );
		}

	}

	private void onHelpButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks ();
		m_commonDialog.setOriginalPosition ();
		UIButton l_closeButton = m_commonDialog.getView ("closeMark") as UIButton;

		UILabel l_titleLabel = m_commonDialog.getView ("dialogText") as UILabel;
		UILabel l_contentLabel = m_commonDialog.getView ("contentText") as UILabel;
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_46_HELP_TITLE);
		l_contentLabel.text = Localization.getString(Localization.TXT_STATE_46_HELP_CONTENT);

		l_closeButton.addClickCallback (onCloseDialogButtonClick);
	}
	
	private void onCloseDialogButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		m_commonDialog.setOutPosition ();
		m_helpButton.addClickCallback (onHelpButtonClick);
	}

	private void onPromoteButtonClick( UIButton p_button )
	{
		m_gameController.changeState( ZoodleState.CONTROL_SUBJECT );
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

	
	private void goToChildLock(UIButton p_button)
	{
		m_gameController.changeState (ZoodleState.CHILD_LOCK_STATE);
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

	private void onUpgradeButtonClick(UIButton p_button)
	{
		SwrveComponent.Instance.SDK.NamedEvent("UpgradeBtnInDashBoard");

		if(string.Empty.Equals(SessionHandler.getInstance().PremiumJson))
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
	
	private void viewPremiumRequestComplete(HttpsWWW p_response)
	{
		Server.init (ZoodlesConstants.getHttpsHost());
		if(null == p_response.error)
		{
			SessionHandler.getInstance ().PremiumJson = p_response.text;
			m_gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName) );
			m_gameController.changeState( ZoodleState.VIEW_PREMIUM );
		}
		else
		{
			setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_11_FAIL),Localization.getString(Localization.TXT_STATE_11_FAIL_DATA));
		}
	}

	private void _getTimeSpendRequestComplete(HttpsWWW p_response)
	{
		m_overallProgressCanvas.setData(MiniJSON.MiniJSON.jsonDecode(p_response.text) as ArrayList);
	}
	
	private UIManager m_uiManager;

	private OverallProgressCanvas m_overallProgressCanvas;
	private CommonDialogCanvas m_commonDialog;
	private UICanvas m_paywallCanvas;
	
	private UIButton m_showProfileButton;
	private UIButton m_closeLeftMenuButton;
	private UIButton m_helpButton;
	private UIButton m_upgradeButton;

	private RequestQueue m_requestQueue;

	private UIButton m_promoteButton;
	
	private UISwipeList m_childrenList;
	

}
