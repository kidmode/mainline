using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Cleaned by Kevin
//BTW previous code suck 
//from around 18 variables to 3!!!!

public class OverviewTimeSpentState : GameState {
	
	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);
		
		m_uiManager = m_gameController.getUI();
		m_requestQueue = new RequestQueue ();
		
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

		m_uiManager.removeScreen( UIScreen.TIME_SPEND );
	}
	
	//----------------- Private Implementation -------------------
	
	private void _setupScreen( GameController p_gameController )
	{
		m_timeSpendCanvas 	= m_uiManager.createScreen( UIScreen.TIME_SPEND, true, 1 ) as TimeSpendCanvas;
	}
	
	private void _setupElment()
	{
		
		UIElement l_newPanel = m_timeSpendCanvas.getView ("mainPanel");
//		List<Vector3> l_pointListIn = new List<Vector3>();
//		l_pointListIn.Add( l_newPanel.transform.localPosition );
//		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 830, 0 ));
//		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
//		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, 0.5f);


//		if( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() )
//		{
		m_requestQueue.reset();
		m_requestQueue.add( new GetTimeSpendRequest(_getTimeSpendRequestComplete) );
		m_requestQueue.request( RequestType.RUSH );
//		}

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

	private void _getTimeSpendRequestComplete(HttpsWWW p_response)
	{
		m_timeSpendCanvas.setData(MiniJSON.MiniJSON.jsonDecode(p_response.text) as ArrayList);
	}

	private UIManager m_uiManager;

	private TimeSpendCanvas m_timeSpendCanvas;
	
	private RequestQueue m_requestQueue;
}
