using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class CallServerState : GameState 
{
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );

		m_gameController = p_gameController;

		m_changeState = false;
		m_changeErrorState = false;
		m_webRequests = new List<WebRequest> ();
		errorMessage = "";
		m_callCount = 100;
		_setupScreen( p_gameController.getUI() );
		m_session = SessionHandler.getInstance ();
		sendCall ();
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );

		if(m_changeState || m_callCount == 0)
		{
			int l_lastState = m_gameController.getConnectedState(ZoodleState.CALL_SERVER);
			if(l_lastState != ZoodleState.NO_STATE)
			{
				m_gameController.changeState(l_lastState);
			}
		}
		if(m_changeErrorState)
		{
			setErrorMessage(p_gameController,"Fail",errorMessage);
		}
	}

	private void sendCall()
	{
		if(null != SessionHandler.getInstance().callList)
		{
			List<SingleCall> l_calls = SessionHandler.getInstance().callList;
			m_callCount =  l_calls.Count;
			for (int l_i = 0; l_i <m_callCount; l_i++)
			{
				SingleCall l_call = l_calls[l_i];
				m_webRequests.Add( Server.request (l_call.callPath,l_call.callData,l_call.callMethod,onSignCallReturn));
			}
		}
		else if(!("".Equals(m_session.url)))
		{
			m_webRequest = Server.request (m_session.url,m_session.parameter,m_session.callMethod,onCallReturn);
		}
	}

	private static void onCallReturn(WWW p_webCall)
	{
		if(null != p_webCall.error)
		{
			_Debug.logError(p_webCall.error);
			errorMessage = p_webCall.error;
			m_changeErrorState = true;
		}
		else
		{
			_Debug.log(p_webCall.text);
			SessionHandler.getInstance().returnJson = p_webCall.text;
			m_changeState = true;
		}
	}

	private static void onSignCallReturn(WWW p_webCall)
	{
		if(null != p_webCall.error)
		{
			_Debug.logError(p_webCall.error);
			errorMessage = p_webCall.error;
			m_changeErrorState = true;
		}
		else
		{
			_Debug.log(p_webCall.text);
		}
		m_callCount --;

	//	p_webCall.Dispose ();
	}

	public override void exit( GameController p_gameController )
	{
		m_gameController = null;
		p_gameController.getUI().removeScreen( m_errorCanvas );
		m_errorCanvas = null;
		Server.init (ZoodlesConstants.getHost());

		if(null != m_webRequest)
		{
			Server.removeCall(m_webRequest);
		}

		int l_count = m_webRequests.Count;
		for(int l_i = 0; l_i <l_count; l_i++)
		{
			Server.removeCall(m_webRequests[l_i]);
		}

		SessionHandler.getInstance ().callList = null;
		base.exit( p_gameController );	
	}
	
	
	//---------------- Private Implementation ----------------------
	
	
	
	private void _setupScreen( UIManager p_uiManager )
	{
		m_errorCanvas = p_uiManager.findScreen( UIScreen.ERROR ) as ErrorCanvas;
		if( m_errorCanvas == null )
			m_errorCanvas = p_uiManager.createScreen( UIScreen.ERROR, false, 5 ) as ErrorCanvas;

		UILabel l_errorName = m_errorCanvas.getView ("errorNameLabel") as UILabel;
		l_errorName.text = "Loading";

		UILabel l_errorMessage = m_errorCanvas.getView ("errorLabel") as UILabel;
		l_errorMessage.text = "Please wait..";

		UIImage l_background = m_errorCanvas.getView ("bodyBG") as UIImage;
		l_background.tweener.addAlphaTrack(0, 1, 0.1f);

		UIButton l_quitButton = m_errorCanvas.getView ("quitButton") as UIButton;
		l_quitButton.active = false;
	}

	private void onExitClicked(UIButton p_button)
	{
		p_button.removeClickCallback(onExitClicked);
		m_changeState = true;
	}
	
	public static bool m_changeState;
	public static bool m_changeErrorState;
	public static string errorMessage;
	private WebRequest m_webRequest;
	private List<WebRequest> m_webRequests;
	public static int m_callCount;
	private SessionHandler m_session;
	private ErrorCanvas m_errorCanvas;
}
