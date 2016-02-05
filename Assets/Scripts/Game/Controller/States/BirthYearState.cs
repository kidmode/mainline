using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BirthYearState : GameState 
{	
	//Public variables
	
	
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );

		_setupScreen( p_gameController.getUI() );

		m_requestQueue = new RequestQueue ();

		if (null == m_postData) 
		{
			m_postData = new Hashtable();
		}

		m_inputNum = "";
		m_starLabel.text = "";
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );

		if (gotoPrevious) 
		{
			gotoPrevious = false;
			if(null == SessionHandler.getInstance().kidList || SessionHandler.getInstance().kidList.Count == 0)
			{
				SessionHandler.getInstance().clearUserData(false);
				p_gameController.changeState(ZoodleState.SET_UP_ACCOUNT);
				return;
			}
			int l_nextState = m_gameController.getConnectedState(ZoodleState.BIRTHYEAR);
			if(GameController.UNDEFINED_STATE != l_nextState)
			{
				if(l_nextState == ZoodleState.INITIALIZE_GAME)
				{
//					KidMode.setKidsModeActive(false);	
					KidModeLockController.Instance.swith2DParentMode();
					PlayerPrefs.Save();
					Application.Quit();
				}
				else if (l_nextState == ZoodleState.MAP)
				{
					p_gameController.changeState(ZoodleState.MAP);
				}
				else if (l_nextState == ZoodleState.REGION_LANDING)
				{
					p_gameController.changeState(ZoodleState.REGION_LANDING);
				}
//				else
//				{
//					p_gameController.changeState(ZoodleState.PROFILE_SELECTION);
//				}
			}
			//honda: if state is undefined, don't do anything
//			else
//			{
//				p_gameController.changeState(ZoodleState.PROFILE_SELECTION);
//			}
		}
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_birthCanvas );
		p_gameController.getUI().removeScreen( UIScreen.COMMON_DIALOG );
	}
	
	
	//---------------- Private Implementation ----------------------
	
	private void _setupScreen( UIManager p_uiManager )
	{
//		int l_previous = m_gameController.getConnectedState(ZoodleState.BIRTHYEAR);
//		if (l_previous == ZoodleState.MAP)
//		{
//			UICanvas l_backScreen = p_uiManager.findScreen(UIScreen.SPLASH_BACKGROUND);
//			if (l_backScreen == null)
//			{
//				l_backScreen = p_uiManager.createScreen(UIScreen.SPLASH_BACKGROUND,true, -1);
//				SplashBackCanvas l_splashBack = l_backScreen as SplashBackCanvas;
//				l_splashBack.setDown();
//			}
//		}

		m_dialogCanvas = p_uiManager.createScreen (UIScreen.COMMON_DIALOG, true, 3) as CommonDialogCanvas;
		m_dialogCanvas.setUIManager (p_uiManager);
		m_dialogCanvas.setUIManager (p_uiManager);
		m_birthCanvas = p_uiManager.createScreen( UIScreen.BIRTHYEAR, true, 1 );
		m_backButton = m_birthCanvas.getView("exitButton") as UIButton;
		m_backButton.addClickCallback (toBack);

		//honda
		if (m_gameController.game.isFotaBroadcast)
		{
			Debug.Log("@@@@@@@@fota disable back button");
			m_backButton.active = false;
		}
		//end

		// Sean: vzw
		// if this app is A launcher, we should go back to nowhere
		int l_nextState = m_gameController.getConnectedState(ZoodleState.BIRTHYEAR);
		// re-set it because I took it.
		m_gameController.connectState(ZoodleState.BIRTHYEAR, l_nextState);
		if (l_nextState == ZoodleState.INITIALIZE_GAME) {
			m_backButton.active = false;
		}
		// end: vzw


		List<Vector3> l_posList = new List<Vector3>();
		l_posList.Add( m_backButton.transform.localPosition + new Vector3( -100, 0, 0 ) );
		l_posList.Add( m_backButton.transform.localPosition );
		m_backButton.tweener.addPositionTrack( l_posList, 1.0f, null, Tweener.Style.QuadOutReverse );
		m_closeDialog = m_dialogCanvas.getView ("closeMark") as UIButton;
		m_closeDialogButton = m_dialogCanvas.getView ("closeButton") as UIButton;
		m_closeDialogButton.active = true;
		m_panel = m_birthCanvas.getView("panel") as UIElement;
		m_title = m_birthCanvas.getView("topicTextGroup") as UIElement;
		m_title.tweener.addAlphaTrack (0.0f, 1.0f, 1.0f,null);
		m_panel.tweener.addAlphaTrack (0.0f, 1.0f, 1.0f,onTitleTweenFinish);

		UILabel l_commonDialogTopic = m_dialogCanvas.getView ("dialogText") as UILabel;
		l_commonDialogTopic.text = Localization.getString(Localization.TXT_STATE_12_PIN_TITLE);
		UILabel l_commonDialogContent = m_dialogCanvas.getView ("contentText") as UILabel;
		l_commonDialogContent.text = Localization.getString(Localization.TXT_STATE_12_PIN_CONTENT);

		m_forgotButton = m_birthCanvas.getView("forgotButtonArea").getView("forgotButton") as UIButton;
		m_forgotButton.active = false;
		m_forgotButton.addClickCallback (startGetParentGateLockRequest);
		m_starLabel = m_birthCanvas.getView("panel").getView ("birthArea").getView ("starText") as UILabel;
		m_noticeText = m_birthCanvas.getView ("noticeText") as UILabel;
		ArrayList l_numBtnArray = m_birthCanvas.getView ("panel").getView ("numBoard").getViewsByTag ("numBtn");
		int l_count = l_numBtnArray.Count;
		for(int l_i = 0; l_i < l_count; l_i++)
		{
			(l_numBtnArray[l_i] as UIButton).addClickCallback(clickNumButton);
 		}

		if(!SessionHandler.getInstance().verifyBirth)
		{
			m_noticeText.text = Localization.getString(Localization.TXT_STATE_12_PIN_NOTICE);
		}

		m_deleteButton = m_birthCanvas.getView("deleteButton") as UIButton;
		m_deleteButton.addClickCallback(deleteNumber);
	}

	private void closeDialog(UIButton p_button)
	{
		m_gameController.getUI ().changeScreen (UIScreen.COMMON_DIALOG,false);
		m_forgotButton.addClickCallback (startGetParentGateLockRequest);
		m_closeDialog.removeAllCallbacks ();
		m_closeDialogButton.removeAllCallbacks ();
		m_dialogCanvas.setOutPosition ();


	}

	//Kev
	private void startGetParentGateLockRequest(UIButton button){

		if (m_gameController.game.checkInternet()) 
		{

			m_gameController.getUI().createScreen(UIScreen.LOADING_SPINNER_ELEPHANT, false, 10);

			m_requestQueue.reset ();
			GetLockPinRequest getLockPinRequest = new GetLockPinRequest(SessionHandler.getInstance().username, sendPinRequestComplete);//new SendPinRequest(sendPinRequestComplete);
			getLockPinRequest.timeoutHandler = serverRequestTimeout;
			m_requestQueue.add(getLockPinRequest);
			m_requestQueue.request();

		}

	}
	//End

	private void showDialog(UIButton p_button)
	{
		if (m_gameController.game.checkInternet()) 
		{
			m_forgotButton.removeAllCallbacks ();
			m_closeDialog.addClickCallback (closeDialog);
			m_closeDialogButton.addClickCallback (closeDialog);

//			m_requestQueue.reset ();
//			SendPinRequest sendPinRequest = new SendPinRequest(sendPinRequestComplete);
//			sendPinRequest.timeoutHandler = serverRequestTimeout;
//			m_requestQueue.add(sendPinRequest);
//			m_requestQueue.request();

		}
	}

	private void sendPinRequestComplete(HttpsWWW p_response)
	{

		Hashtable table = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;

		bool hasError = (bool)table[ "error" ];

		if(!hasError)
		{
//			m_gameController.getUI ().changeScreen (UIScreen.COMMON_DIALOG,true);
			m_dialogCanvas.setOriginalPosition();

			//Kev
			m_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);

			showDialog(null);
		}
		else
		{

			m_requestQueue.reset ();
			SendPinRequest forgotMainPinRequest = new SendPinRequest(forgotMainPinComplete);//new SendPinRequest(sendPinRequestComplete);
			forgotMainPinRequest.timeoutHandler = serverRequestTimeout;
			m_requestQueue.add(forgotMainPinRequest);
			m_requestQueue.request();
			
		}
	}
	
	
	private void forgotMainPinComplete(HttpsWWW p_response)
	{
		
		if(p_response.error == null){

			m_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);

			m_dialogCanvas.setOriginalPosition();
			
			showDialog(null);
			
		}else{

			m_forgotButton.addClickCallback(startGetParentGateLockRequest);
			m_gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);

		}
		
	}

	private void serverRequestTimeout()
	{
		m_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);
		m_birthCanvas.active = true;
		m_forgotButton.addClickCallback(startGetParentGateLockRequest);
	}
	
	private void onTitleTweenFinish( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_forgotButton.tweener.addAlphaTrack (0.0f, 1.0f, 0.5f);
	}

	private void toBack( UIButton p_button )
	{
		gotoPrevious = true;
	}

	private void clickNumButton( UIButton p_button )
	{
		m_inputNum = m_inputNum + p_button.name.Substring (3);
		m_starLabel.text = m_starLabel.text + "* ";

		if(m_inputNum.Length == 4)
		{
			if(SessionHandler.getInstance().verifyBirth)
			{
				int l_inputNum = int.Parse(m_inputNum);
				
				if(0!= SessionHandler.getInstance().pin && l_inputNum == SessionHandler.getInstance().pin)
				{
					changeStateAfterVerify();
				}
				else
				{
					setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_12_INCORRECT),Localization.getString(Localization.TXT_STATE_12_INCORRECT_YEAR));
				}
			}
			else
			{
				if(m_inputNum.Equals(SessionHandler.getInstance().childLockPassword))
				{
					changeStateAfterVerify();
				}
				else
				{
					setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_12_INCORRECT),Localization.getString(Localization.TXT_STATE_12_INCORRECT_PIN));
				}
			}

			m_inputNum = "";
			m_starLabel.text = "";
		}
	}

	private void changeStateAfterVerify()
	{
		int l_previous = m_gameController.getConnectedState(ZoodleState.BIRTHYEAR);
		Debug.Log("BirthYearState -> previous? " + l_previous);

		if (m_gameController.game.isFotaBroadcast)
		{
			Debug.Log("@@@@@@@@fota parent gate verified");
			KidMode.broadcastKidmodeFota();
			UICanvas l_backScreen = m_gameController.getUI().findScreen(UIScreen.SPLASH_BACKGROUND);
			if (l_backScreen != null)
			{
				m_gameController.getUI().removeScreenImmediately(UIScreen.SPLASH_BACKGROUND);
			}
			m_gameController.changeState(l_previous);
			m_gameController.game.isFotaBroadcast = false;
			return;
		}

		if(GameController.UNDEFINED_STATE != l_previous)
		{
			if(l_previous == ZoodleState.PROFILE_SELECTION)
			{
//				KidMode.setKidsModeActive(false);	
				KidModeLockController.Instance.swith2DParentMode();
				PlayerPrefs.Save();
				Application.Quit();
			}
			else if(l_previous == ZoodleState.INITIALIZE_GAME)
			{
				m_gameController.changeState(ZoodleState.SIGN_IN_CACHE);
			}
			else if (l_previous == ZoodleState.MAP)
			{
				KidMode.broadcastCurrentMode("ParentMode");
				//honda: unload map if needed
				UIManager manager = m_gameController.getUI();
				MapCanvas mcanvas = manager.findScreen(UIScreen.MAP) as MapCanvas;
				if (mcanvas != null)
					manager.removeScreenImmediately(mcanvas);
				SplashBackCanvas sbcanvas = manager.findScreen(UIScreen.SPLASH_BACKGROUND) as SplashBackCanvas;
				if (sbcanvas != null)
					manager.removeScreenImmediately(sbcanvas);

				m_gameController.changeState(ZoodleState.PROFILE_SELECTION);
			}
			else if (l_previous == ZoodleState.REGION_LANDING)
			{
				KidMode.broadcastCurrentMode("ParentMode");
				//honda: unload map if needed
				UIManager manager = m_gameController.getUI();
				MapCanvas mcanvas = manager.findScreen(UIScreen.MAP) as MapCanvas;
				if (mcanvas != null)
					manager.removeScreenImmediately(mcanvas);
				SplashBackCanvas sbcanvas = manager.findScreen(UIScreen.SPLASH_BACKGROUND) as SplashBackCanvas;
				if (sbcanvas != null)
					manager.removeScreenImmediately(sbcanvas);

				//reset timer if kid is going to overview app state on parent dashboard
				TimerController.Instance.resetKidTimer();
				m_gameController.changeState(ZoodleState.OVERVIEW_APP);
			}
			else
			{
				m_gameController.changeState(ZoodleState.PROFILE_SELECTION);
			}
		}
		else
		{
//			UIManager manager = m_gameController.getUI();
//			SplashBackCanvas sbcanvas = manager.findScreen(UIScreen.SPLASH_BACKGROUND) as SplashBackCanvas;
//			if (sbcanvas != null)
//				manager.removeScreenImmediately(sbcanvas);
			
			m_gameController.changeState(ZoodleState.PROFILE_SELECTION);
		}
	}

	private void deleteNumber( UIButton p_button )
	{
		if (m_inputNum.Length > 0)
		{
			m_inputNum = m_inputNum.Substring(0, m_inputNum.Length - 1);
			m_starLabel.text = m_starLabel.text.Substring(0, m_inputNum.Length * 2);
		}
	}

	//Private variables
	
	private UIElement	m_title;
	private UIElement 	m_panel;
	private UIButton 	m_backButton;
	private UIButton 	m_forgotButton;
	private UILabel 	m_starLabel;
	private UILabel		m_noticeText;
	private UIButton	m_deleteButton;
	private string 		m_inputNum;
	private UIButton	m_closeDialog;
	private UIButton	m_closeDialogButton;

	private UICanvas    m_birthCanvas;
	private CommonDialogCanvas m_dialogCanvas;
	private UICanvas	m_signInButtonBackgroundCanvas;
	private RequestQueue m_requestQueue;

	private Hashtable 	m_postData;

	private bool 		gotoPrevious = false;

	public 	bool		isFotaBroadcast = false; 
}