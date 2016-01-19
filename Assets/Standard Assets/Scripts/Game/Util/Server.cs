using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Net;
using System.Security.Cryptography.X509Certificates;

public enum CallMethod
{
	GET,
	POST,
	PUT,
	DELETE,
	NULL
}

//public delegate void WebCallHandler( WWW p_webCall );
//honda added: for https
public delegate void HttpsWebCallHandler( string status, HttpsWWW p_webCall );
public delegate void StreamBuildHandler( );
//end
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
	
	public static WebRequest request(string requestName, string p_call, Hashtable p_postData,  CallMethod p_callMethod, HttpsWebCallHandler p_callback )
	{
		WebRequest l_webRequest = new WebRequest(requestName, m_instance, p_call, p_postData, p_callMethod, p_callback );
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

		int l_lenght = m_builders.Count;
		if(l_lenght!=0)
		{
			for(int l_i = 0; l_i < l_lenght; l_i ++)
			{
				PostBuilder l_build = m_builders[l_i] as PostBuilder;
				if(l_build.buildFinish)
				{
					m_builders.RemoveAt(l_i);
				}
				else
				{
					l_build.build();
				}
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

	public static void setCer(X509Certificate2 p_adminClient)
	{
		HttpsWWW.adminClient = p_adminClient;
	}

	public static Server getInstance()
	{
		return m_instance;
	}

	public void addbuilder( PostBuilder p_build )
	{
		Server.m_builders.Add(p_build);
	}
	
	public static void removeBuilder( PostBuilder p_build )
	{
		Server.m_builders.Remove(p_build);
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
	public static ArrayList m_builders = new ArrayList();
}

public class PostBuilder
{
	public PostBuilder(Stream p_stream,Byte[] p_bytes,StreamBuildHandler p_handler)
	{
		m_stream = p_stream;
		m_bytes = p_bytes;
		m_handler += p_handler;
		m_index = 0;
		m_sectionLenght = 5;
		m_finish = false;
		m_lastLenght = p_bytes.Length % m_sectionLenght;
		m_lenght = p_bytes.Length / m_sectionLenght + (m_lastLenght == 0 ? -1 : 0);
	}
	
	public void build()
	{
		if(m_index > m_lenght)
		{
			m_finish = true;
			m_handler();
			m_stream.Close();
			return;
		}
		
		if(m_index == m_lenght && m_lastLenght != 0)
		{
			writeStream(m_lastLenght);
		}
		else
		{
			writeStream(m_sectionLenght);
		}
	}
	
	public void writeStream(int p_lenght)
	{
		Byte[] l_bytes = new byte[p_lenght];
		Array.ConstrainedCopy (m_bytes, m_index*m_sectionLenght, l_bytes, 0, p_lenght);
		m_stream.Write (l_bytes, 0, l_bytes.Length);
		m_index ++;
	}
	
	public bool buildFinish
	{
		get{return m_finish;}
	}
	
	private int	   m_sectionLenght;
	private int	   m_lenght;
	private int	   m_lastLenght;
	private int    m_index;
	private bool   m_finish;
	private Stream m_stream;
	private HttpWebRequest m_;
	private byte[] m_bytes;
	private StreamBuildHandler m_handler;
}


