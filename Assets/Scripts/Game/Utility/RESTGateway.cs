using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MiniJSON;


public delegate void OnLoginHandle( string username, string password, bool p_error );
public delegate void OnListRequestHandle( List<object> p_contentList, bool p_error );
//public delegate void OnListRequestHandle( List<object> p_contentList, bool p_error );
public delegate void OnImageRequestHandle( Texture2D p_texture );

/**
 * Constants used for interfacing with Zoodles servers.
 * 
 */

public class RESTGateway 
{
	private enum RequestType
    {
        BookList = 0,
        KidList,
        WebContentList,
        Image
    }

    class RequestGroup
    {
        public RequestType type = RequestType.BookList;
        public OnListRequestHandle contentList = null;
    }

    public static string buildUrl( string p_apiCall, string p_prefix = "" )
    {
        string l_hostServer = ZoodlesConstants.getHttpsHost();

        string url = p_prefix + l_hostServer + p_apiCall;
        return url;
    }
	
	// this should only be overwritten for the error state (if it isn't it can create an infinite loop when trying to close the pop up)
	public static void setErrorMessage(GameController p_gameController, string p_errorName, string p_errorMessage)
	{
		int l_thisState = int.Parse(p_gameController.stateName);
		
		p_gameController.connectState(ZoodleState.ERROR_STATE, l_thisState);
		
		SessionHandler l_handler = SessionHandler.getInstance();
		
		l_handler.errorName 	= p_errorName;
		l_handler.errorMessage 	= p_errorMessage;
		
		p_gameController.changeState(ZoodleState.ERROR_STATE);
	}

/*	public static IEnumerator setBirthYear( int p_birthYear )
	{

	}
*/

 //----------------------------------- Private Implementation ---------------------------
 
    private static int _getTimeZoneOffset()
    {
        System.TimeZone l_timeZone = System.TimeZone.CurrentTimeZone;
        System.TimeSpan l_timeSpan = l_timeZone.GetUtcOffset( System.DateTime.Today );

        return l_timeSpan.Seconds;
    }
	
    private static Game getGame()
    {
        if (m_gameInstance == null)
            m_gameInstance = GameObject.FindObjectOfType<Game>();

        return m_gameInstance;
    }
    private static Game m_gameInstance = null;
//    private static List<WWW> m_requestList = new List<WWW>();
}
