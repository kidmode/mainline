using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ControlViolenceState : GameState
{
	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);
		
		m_uiManager = m_gameController.getUI();
		m_requestQueue = new RequestQueue();
		_setupScreen( p_gameController );
		_setupElment();

		//make sure the menu bar is visible whenever we enter the state
		Game game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
		
		game.setPDMenuBarVisible(true, false);

//		TutorialController.Instance.showTutorial(TutorialSequenceName.VIOLENCE_LEVEL);
		if(TutorialController.Instance != null)
			TutorialController.Instance.showNextPage();

	}
	
	public override void update (GameController p_gameController, int p_time)
	{
		base.update (p_gameController, p_time);
	}
	
	public override void exit (GameController p_gameController)
	{
		base.exit (p_gameController);

		m_uiManager.removeScreen( UIScreen.VIOLENCE_FILTERS );
	}

	private void _setupScreen( GameController p_gameController )
	{
		m_violenceFiltersCanvas 	= m_uiManager.createScreen( UIScreen.VIOLENCE_FILTERS, true, 1 );
	}

	private void _setupElment()
	{
		//New Save Button
		mSaveButton = m_violenceFiltersCanvas.getView ("saveButton") as UIButton;
		mSaveButton.addClickCallback (checkRequest);

		
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_violenceFiltersCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, saveMessageDisplacement, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, 0.5f);

		
		//violence part
		m_levelZeroToggle 	= m_violenceFiltersCanvas.getView( "levelZeroToggle" ) 	as UIToggle;
		m_levelOneToggle 	= m_violenceFiltersCanvas.getView( "levelOneToggle" ) 	as UIToggle;
		m_levelTwoToggle 	= m_violenceFiltersCanvas.getView( "levelTwoToggle" ) 	as UIToggle;
		m_levelThreeToggle 	= m_violenceFiltersCanvas.getView( "levelThreeToggle" ) as UIToggle;
		m_levelFourToggle 	= m_violenceFiltersCanvas.getView( "levelFourToggle" ) 	as UIToggle;
		
		m_levelZeroToggle.addValueChangedCallback ( onViolenceChanged );
		m_levelOneToggle.addValueChangedCallback ( onViolenceChanged );
		m_levelTwoToggle.addValueChangedCallback ( onViolenceChanged );
		m_levelThreeToggle.addValueChangedCallback ( onViolenceChanged );
		m_levelFourToggle.addValueChangedCallback ( onViolenceChanged );

		setEventLisenters();
	}

	private void setEventLisenters(){

		//set up value changed events
		PDControlValueChanged valueChanged = m_violenceFiltersCanvas.gameObject.transform.parent.gameObject.GetComponent<PDControlValueChanged>();
		
		valueChanged.setListeners();

	}

	private void checkRequest(UIButton p_button)
	{
		if (checkInternet() == false)
			return;


		if( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() )
		{
			
			if( m_isValueChanged )
			{
				
				m_isValueChanged = false;
				
				updateViolenceFilters();
				
			}
			
			
		}else{
			
			m_gameController.connectState( ZoodleState.VIEW_PREMIUM, ZoodleState.CONTROL_VIOLENCE );
			
			m_gameController.changeState( ZoodleState.VIEW_PREMIUM );
			
			Game game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
			
			game.setPDMenuBarVisible(false, false);
			
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

	private void onViolenceChanged( UIToggle p_toggle, bool p_bool )
	{
		m_isValueChanged = true;


	}

	private void updateViolenceFilters ()
	{
		Hashtable l_param = new Hashtable ();
		if( m_levelZeroToggle.isOn )
		{
			l_param [ZoodlesConstants.PARAM_MAX_VIOLENCE] = 0;
		}
		if( m_levelOneToggle.isOn )
		{
			l_param [ZoodlesConstants.PARAM_MAX_VIOLENCE] = 1;
		}
		if( m_levelTwoToggle.isOn )
		{
			l_param [ZoodlesConstants.PARAM_MAX_VIOLENCE] = 2;
		}
		if( m_levelThreeToggle.isOn )
		{
			l_param [ZoodlesConstants.PARAM_MAX_VIOLENCE] = 3;
		}
		if( m_levelFourToggle.isOn )
		{
			l_param [ZoodlesConstants.PARAM_MAX_VIOLENCE] = 4;
		}

		m_requestQueue.reset ();
		m_requestQueue.add( new SetSubjectsRequest( l_param , updateViolenceSettingComplete) );
		m_requestQueue.request (RequestType.SEQUENCE);

		m_uiManager.createScreen(UIScreen.LOADING_SPINNER_ELEPHANT, false, 10);

	}

	private void updateViolenceSettingComplete(HttpsWWW p_response)
	{

		UICanvas messageCanvas = m_uiManager.createScreen(UIScreen.PD_MESSAGE, false, 20);

		UIButton messageCloseButton = messageCanvas.getView("quitButton") as UIButton;

		messageCloseButton.addClickCallback(closePDMessage);


		if(p_response.error == null){

			m_uiManager.removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);

		}else{

			m_uiManager.createScreen(UIScreen.ERROR_MESSAGE, false, 20);

		}



	}

	private void closePDMessage(UIButton button){

		m_uiManager.removeScreen(UIScreen.PD_MESSAGE);

	}

	private UIManager 		m_uiManager;
	private RequestQueue 	m_requestQueue;
	private bool 			m_isValueChanged = false;
	
	private UISwipeList 	m_childrenList;

	//violence part
	private UICanvas 		m_violenceFiltersCanvas;

	private UIToggle 		m_levelZeroToggle;
	private UIToggle 		m_levelOneToggle;
	private UIToggle 		m_levelTwoToggle;
	private UIToggle 		m_levelThreeToggle;
	private UIToggle 		m_levelFourToggle;

	//Kevin
	private float saveMessageDisplacement = 1092.0f;

	//Kevin - Save Button
	UIButton mSaveButton;

}
