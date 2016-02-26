﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ControlTimeState : GameState
{
	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);
		
		m_uiManager = m_gameController.getUI();
		m_requestQueue = new RequestQueue();
		_setupScreen( p_gameController );
		_setupElment();

		if(TutorialController.Instance != null)
			TutorialController.Instance.showNextPage();


		Game game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
		
		game.setPDMenuBarVisible(true, false);

	}
	
	public override void update (GameController p_gameController, int p_time)
	{
		base.update (p_gameController, p_time);
	}
	
	public override void exit (GameController p_gameController)
	{
		base.exit (p_gameController);

		m_uiManager.removeScreen( UIScreen.TIME_LIMITS );
	}

	private void _setupScreen( GameController p_gameController )
	{
		m_timeLimitsCanvas = m_uiManager.createScreen( UIScreen.TIME_LIMITS, true, 1 ) as TimeLimitsCanvas;
	}

	private void _setupElment()
	{
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_timeLimitsCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, mainPanelOffset, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, 0.5f);
		
		//time part
		UIToggleGroup l_weekGroup= m_timeLimitsCanvas.getView( "weekGroup" ) as UIToggleGroup;
		m_weekThirtyMin 		= l_weekGroup.getView( "thirtyMinToggle" )	 as UIToggle;
		m_weekOneHour 			= l_weekGroup.getView( "oneHourToggle" )	 as UIToggle;
		m_weekTwoHours 			= l_weekGroup.getView( "twoHoursToggle" )	 as UIToggle;
		m_weekFourHours 		= l_weekGroup.getView( "fourHoursToggle" )	 as UIToggle;
		m_weekUnlimited 		= l_weekGroup.getView( "unlimitedToggle" )	 as UIToggle;
		
		UIToggleGroup l_weekendGroup= m_timeLimitsCanvas.getView( "weekendGroup" ) as UIToggleGroup;
		m_weekendThirtyMin 		= l_weekendGroup.getView( "thirtyMinToggle" )	 as UIToggle;
		m_weekendOneHour 		= l_weekendGroup.getView( "oneHourToggle" )		 as UIToggle;
		m_weekendTwoHours 		= l_weekendGroup.getView( "twoHoursToggle" )	 as UIToggle;
		m_weekendFourHours 		= l_weekendGroup.getView( "fourHoursToggle" )	 as UIToggle;
		m_weekendUnlimited 		= l_weekendGroup.getView( "unlimitedToggle" )	 as UIToggle;
		
		m_weekThirtyMin.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekOneHour.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekTwoHours.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekFourHours.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekUnlimited.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekendThirtyMin.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekendOneHour.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekendTwoHours.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekendFourHours.addValueChangedCallback ( onTimeLimitsChanged );
		m_weekendUnlimited.addValueChangedCallback ( onTimeLimitsChanged );

		if( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() )
		{
			UIElement l_panel = m_timeLimitsCanvas.getView("panel");
			l_panel.active = false;
			m_requestQueue.reset ();
			m_requestQueue.add (new GetTimeLimitsRequest(_getTimeLimitRequestComplete));
			m_requestQueue.request( RequestType.SEQUENCE );
		}else{

			setValueChangedEvent();

		}

		//New Save Button
		mSaveButton = m_timeLimitsCanvas.getView ("saveButton") as UIButton;
		mSaveButton.addClickCallback (checkRequest);
	}

	private void checkRequest(UIButton button)
	{
		if (checkInternet() == false)
			return;

		if( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() )
		{

			if( m_isValueChanged )
			{

				m_isValueChanged = false;

				updateTimeLimits();

			}

		}else{

			m_gameController.connectState( ZoodleState.VIEW_PREMIUM, ZoodleState.CONTROL_TIME );
			
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

	private void onTimeLimitsChanged( UIToggle p_toggle, bool p_bool )
	{

		m_isValueChanged = true;

		if( onControlValueChanged != null){

			onControlValueChanged(m_isValueChanged);

		}

	}

	private void _getTimeLimitRequestComplete(HttpsWWW p_response)
	{
		Hashtable table = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
		m_timeLimitsCanvas.setTimeLimits(table);
		UIElement l_panel = m_timeLimitsCanvas.getView("panel");

		if( null == l_panel )
		{
			return;
		}

		l_panel.active = true;
		m_isValueChanged = false;


		setValueChangedEvent();

	}

	private void setValueChangedEvent(){

		//set up value changed events
		PDControlValueChanged valueChanged = m_timeLimitsCanvas.gameObject.transform.parent.gameObject.GetComponent<PDControlValueChanged>();
		
		valueChanged.setListeners();

	}

	//honda: update TimeLimits to currentkid and save kidinfo to local
	private void updateTimeLimits ()
	{
		Hashtable l_param = new Hashtable ();
		l_param [ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance ().token.getSecret ();

		int weekdayTimeLimits = -1;
		int weekendTimeLimits = -1;
		if( m_weekUnlimited.isOn )
		{
			l_param[ZoodlesConstants.PARAM_WEEKDAY_DISABLED] = "true";
			l_param[ZoodlesConstants.PARAM_WEEKDAY_LIMIT] = "-1";
			weekdayTimeLimits = -1;
		}
		else
		{
			l_param[ZoodlesConstants.PARAM_WEEKDAY_DISABLED] = "false";
			if( m_weekThirtyMin.isOn )
				weekdayTimeLimits = 30;
			else if( m_weekOneHour.isOn )
				weekdayTimeLimits = 60;
			else if( m_weekTwoHours.isOn )
				weekdayTimeLimits = 120;
			else if( m_weekFourHours.isOn )
				weekdayTimeLimits = 240;
			l_param[ZoodlesConstants.PARAM_WEEKDAY_LIMIT] = weekdayTimeLimits.ToString();
		}
		
		if( m_weekendUnlimited.isOn )
		{
			l_param[ZoodlesConstants.PARAM_WEEKEND_DISABLED] = "true";
			l_param[ZoodlesConstants.PARAM_WEEKEND_LIMIT] = "-1";
			weekendTimeLimits = -1;
		}
		else
		{
			l_param[ZoodlesConstants.PARAM_WEEKEND_DISABLED] = "false";
			if( m_weekendThirtyMin.isOn )
				weekendTimeLimits = 30;
			if( m_weekendOneHour.isOn )
				weekendTimeLimits = 60;
			if( m_weekendTwoHours.isOn )
				weekendTimeLimits = 120;
			if( m_weekendFourHours.isOn )
				weekendTimeLimits = 240;
			l_param[ZoodlesConstants.PARAM_WEEKEND_LIMIT] = weekendTimeLimits.ToString();
		}
		SessionHandler.getInstance().currentKid.updateTimeLimitsInfo(weekdayTimeLimits, weekendTimeLimits);

		foreach(DictionaryEntry item in l_param)
		{
			Debug.Log("time limit param: " + item.Key + "->" + item.Value);
		}

		m_requestQueue.reset ();
		m_requestQueue.add ( new SetTimeLimitsRequest(l_param, _setTimeLimitRequestComplete));
		m_requestQueue.request (RequestType.SEQUENCE);

		m_uiManager.createScreen(UIScreen.LOADING_SPINNER_ELEPHANT, false, 10);

	}

	private void _setTimeLimitRequestComplete(HttpsWWW p_response)
	{

		UICanvas messageCanvas = m_uiManager.createScreen(UIScreen.PD_MESSAGE, false, 20);
		
		UIButton messageCloseButton = messageCanvas.getView("quitButton") as UIButton;
		
		messageCloseButton.addClickCallback(closePDMessage);

		if (p_response.error == null)
		{

			Debug.Log(p_response.text);

			m_uiManager.removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);

		}
		else
		{

			Debug.Log(p_response.error);

			m_uiManager.createScreen(UIScreen.ERROR_MESSAGE, false, 20);

		}


		if( onControlValueChanged != null){
			
			onControlValueChanged(m_isValueChanged);
			
		}

	}

	private void closePDMessage(UIButton button){
		
		m_uiManager.removeScreen(UIScreen.PD_MESSAGE);
		
	}
	
	private UIManager 		m_uiManager;
	private RequestQueue 	m_requestQueue;
	private bool 			m_isValueChanged = false;

	private UISwipeList 	m_childrenList;
	
	private UIButton 		m_leftButton;
	private UIButton 		m_rightButton;
	
	// time part
	private TimeLimitsCanvas m_timeLimitsCanvas;

	private UIToggle m_weekThirtyMin;
	private UIToggle m_weekOneHour;
	private UIToggle m_weekTwoHours;
	private UIToggle m_weekFourHours;
	private UIToggle m_weekUnlimited;
	
	private UIToggle m_weekendThirtyMin;
	private UIToggle m_weekendOneHour;
	private UIToggle m_weekendTwoHours;
	private UIToggle m_weekendFourHours;
	private UIToggle m_weekendUnlimited;
	
	//Kevin
	private float mainPanelOffset = 1124.25f;

	//Kevin - event when control values changed to true 
	public static event Action<bool>	onControlValueChanged;

	//Kevin - Save Button
	UIButton mSaveButton;

}
