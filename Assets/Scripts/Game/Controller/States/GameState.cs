using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class GameState
{
	public virtual void enter( GameController p_gameController )
	{
		m_gameController = p_gameController;
		if (p_gameController != null)
		{
			Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n" +
			          "game state: " + p_gameController.stateName +
			          "\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
		}
	}
	
	public virtual void update( GameController p_gameController, int p_time ) 
	{
	}
	
	public virtual void lateUpdate( GameController p_gameController )
	{
	}
	
	public virtual void exit( GameController p_gameController )
	{
	}
	
	public virtual bool handleMessage( GameController p_gameController, int p_type, string p_string )
	{
		return true;
	}

	// this should only be overwritten for the error state (if it isn't it can create an infinite loop when trying to close the pop up)
	protected virtual void setErrorMessage(GameController p_gameController, string p_errorName, string p_errorMessage)
	{
		int l_thisState = int.Parse(p_gameController.stateName);

		if(ZoodleState.CALL_SERVER == l_thisState)
		{
			l_thisState = SessionHandler.getInstance().invokeCallServerState;
		}

		p_gameController.connectState(ZoodleState.ERROR_STATE, l_thisState);

		SessionHandler l_handler = SessionHandler.getInstance();

		l_handler.errorName 	= p_errorName;
		l_handler.errorMessage 	= p_errorMessage;

		p_gameController.changeState(ZoodleState.ERROR_STATE);
	}

	protected virtual void sendCall(GameController p_gameController, Hashtable p_parameter, string p_url,CallMethod p_callMethod,int p_state)
	{
		int l_thisState = int.Parse(p_gameController.stateName);

		p_gameController.connectState(ZoodleState.CALL_SERVER, p_state);
		
		SessionHandler l_handler = SessionHandler.getInstance();
		
		l_handler.parameter 	= p_parameter;
		l_handler.url 			= p_url;
		l_handler.callMethod 	= p_callMethod;
		l_handler.invokeCallServerState	= l_thisState;

		
		p_gameController.changeState(ZoodleState.CALL_SERVER);
	}

	protected GameController m_gameController = null;
}

public class SingleCall
{
	private string m_callPath;
	private Hashtable m_callData;
	private CallMethod m_callMethod;

	public SingleCall(string p_callPath, Hashtable p_callDate,CallMethod p_callMethod)
	{
		m_callPath = p_callPath;
		m_callData = p_callDate;
		m_callMethod = p_callMethod;
	}
	
	public string callPath
	{
		get {   return m_callPath;   }
		set {   m_callPath = value;  }
	}

	public Hashtable callData
	{
		get {   return m_callData;   }
		set {   m_callData = value;  }
	}

	public CallMethod callMethod
	{
		get {   return m_callMethod;   }
		set {   m_callMethod = value;  }
	}
}

public class CreditCardHelper
{
	public static string parseType(string p_cardNumber)
	{
		p_cardNumber = new Regex(@"[^0-9]+").Replace(p_cardNumber, "");
		if (new Regex(@"^4[0-9]{12}(?:[0-9]{3})?").IsMatch(p_cardNumber))
			return "visa";
		else if (new Regex(@"^5[1-5][0-9]{14}").IsMatch(p_cardNumber))
			return "master";
		else if (new Regex(@"^3[47][0-9]{13}").IsMatch(p_cardNumber))
			return "american_express";
		else if (new Regex(@"^6(?:011|5[0-9]{2})[0-9]{12}").IsMatch(p_cardNumber))
			return "discover";
		else
			return string.Empty;
	}
}

public class GameStateBoard
{
	public void write(string p_key, object p_value)
	{
		m_storage.Add(p_key, p_value);
	}

	public object read(string p_key)
	{
		object l_value = m_storage[p_key];
		m_storage.Remove(p_key);
		return l_value;
	}

	private Hashtable m_storage = new Hashtable();
}