using UnityEngine;
using System;
using System.Collections;

public enum CallMethod
{
	GET,
	POST,
	PUT,
	DELETE,
	NULL
}

public delegate void WebCallHandler( WWW p_webCall );

public class Server : object 
{
//	public static var MAIN_URL:String = "http://98.158.181.169:8080/kiosk/restful/";

    private Server( string p_host )
    {
        m_url = p_host;
    }
   
    public static void init( string p_host )
    {
       // if (m_instance == null)
            m_instance = new Server( p_host );
    }

    public string url
    {
        get { return m_instance.m_url; }
        set { m_instance.m_url = WWW.EscapeURL(value); }
    }



    public static WebRequest request(string p_call, Hashtable p_postData,  CallMethod p_callMethod, WebCallHandler p_callback )
	{
        WebRequest l_webRequest = new WebRequest( m_instance, p_call, p_postData, p_callMethod, p_callback );
        m_instance.addCall( l_webRequest );
		
		return l_webRequest;
	}

	private static int MAX_CONNECTIONS = 4;
	public static void update( float p_deltaTime )
	{
		int i = m_instance.m_calls.Count - 1;
		if (i >= MAX_CONNECTIONS) i = MAX_CONNECTIONS - 1;
		for ( ; i >= 0; i--)
		{
            WebRequest l_call = m_instance.m_calls[i] as WebRequest;
            l_call.update(p_deltaTime);

			if (l_call.isDestroyed)
			{
				Server.removeCall(l_call);
			}
		}
	}
	
	public void addCall( WebRequest p_request )
	{
		m_instance.m_calls.Add(p_request);;
	}
	
	public static void removeCall( WebRequest p_request )
	{
        m_instance.m_calls.Remove(p_request);
	}
	
	
	//private IEnumerator webCall( string p_call, Hashtable p_postData, WebCallHandler p_callback )
	//{
	//	string l_url = url + p_call;
	//	WWWForm l_params = new WWWForm();
	//	
	//	foreach( string key in p_postData.Keys )
	//	{
	//		if( p_postData[key].GetType() == typeof(int) )
	//			l_params.AddField( key, Mathf.CeilToInt( (int)p_postData[key] ) );
	//		if( p_postData[key].GetType() == typeof(String) )
	//			l_params.AddField( key, p_postData[key] as String );
	//	}
    //
    //    WWW l_serverRequest = new WWW( l_url, l_params );
    //    yield return l_serverRequest;
	//	
	//	p_callback( l_serverRequest );
	//}


    private ArrayList m_calls = new ArrayList();
    private string m_url;
    private static Server m_instance;
}
