using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;


public enum WebRequestStatus
{
	InProgress,
	Finished,
	Error,
	Timeout
}

public class WebRequest : object 
{
	private Server m_server;
	private string m_call = null;

	private WWWForm m_form;
    private Hashtable m_param;

    private WebCallHandler m_callback;
	private float m_time;
	private WWW m_www;
	
	private int m_retryLimit = 10;
//	private int m_retries = 0;
	private float m_timeout = 60.0f;
	
	private int m_status = (int)WebRequestStatus.InProgress;

    private CallMethod m_callMethod;


    public WebRequest(Server p_server, string p_call, Hashtable p_postData, CallMethod p_callMethod, WebCallHandler p_callback )
	{
		m_server        = p_server;
		m_call          = p_call;
		m_callback      = p_callback;
		
		m_time          = 0;

        m_param         = p_postData;
        m_callMethod    = p_callMethod;

		if( m_callMethod == CallMethod.GET )
			_setupGET();
        else if( m_callMethod == CallMethod.POST )
            _setupPOST();
	}
	
	public void update( float p_time )
	{
		if (m_call == null)
			return;

		m_time += p_time;
		
		if( null == m_www )
		{
			string l_url = "";
			if( m_call.StartsWith( "@absolute:" ) )
				l_url = m_call.Substring( "@absolute:".Length );
			else if( m_call.StartsWith( "http") || m_call.StartsWith( "www" ) )
				l_url = m_call;
			else
				l_url = m_server.url + m_call;
#if UNITY_EDITOR
			Debug.Log(l_url);
#endif

			if( m_form != null )
				m_www = new WWW( l_url, m_form );
            else
                m_www = new WWW( l_url );
		}
		else
		{
			if( m_www.isDone )
			{
				if( m_www.error != null )
				{
					#if UNITY_EDITOR
					Debug.LogError( m_www.url + ":" + m_www.error );
					#endif
					m_status = (int)WebRequestStatus.Error;
				}
				else
				{
					#if UNITY_EDITOR
					Debug.Log(m_www.text);
					#endif
					m_status = (int)WebRequestStatus.Finished;
				}
				m_callback( m_www );
				m_server.removeCall( this );
			}
		}
		
		if( m_time > m_timeout )
			m_status = (int)WebRequestStatus.Timeout;
	}	
	
	public int status
	{
		get
		{
			return m_status;
		}
	}
	
	public int retryLimit
	{
		set 
		{
			m_retryLimit = value;
		}
		get
		{
			return m_retryLimit;
		}
	}
	
	public float timeout
	{
		set 
		{
			m_timeout = value;
		}
	}



//---------------------- Private Implementation --------------------
	private void _setupGET()
	{
		if( null != m_param && m_param.Keys.Count > 0 )
		{
			int l_count = m_param.Keys.Count - 1;

			if(m_call.Contains("?"))
				m_call += "&";
			else
				m_call += "?";

			foreach( string l_key in m_param.Keys )
			{
				m_call += l_key + "=" + m_param[l_key] + (l_count-- > 0 ? "&" : "");
			}
		}
	}

    private void _setupPOST(  )
    {
        if( null != m_param )
        {
            m_form = new WWWForm();
            foreach (string key in m_param.Keys)
            {
                if (m_param[key].GetType() == typeof(int))
                    m_form.AddField(key, Mathf.CeilToInt( float.Parse( m_param[key].ToString() ) ));
                else if (m_param[key].GetType() == typeof(String))
                    m_form.AddField(key, m_param[key] as String);
				else if (m_param[key].GetType() == typeof(byte[]))
					m_form.AddBinaryData(key, m_param[key] as byte[]);
            }
        }
    }

	public void destroy()
	{
		if(null != m_www)
		{
			m_www.Dispose();
			m_call = null;
		}
	}
    //private Dictionary< string, string > _setupGET( )
    //{
    //    Dictionary<string, string> l_dictionary = new Dictionary<string, string>();
    //   
    //    foreach (string key in m_param.Keys)
    //    {
    //        if (m_param[key].GetType() == typeof(int))
    //        {
    //            int l_value = Mathf.CeilToInt(float.Parse(m_param[key].ToString()));
    //            l_dictionary.Add(key, l_value.ToString());
    //        }
    //
    //        if (m_param[key].GetType() == typeof(string))
    //            l_dictionary.Add(key, m_param[key] as string);
    //    }
    //
    //    return l_dictionary;
    //}

	
	/// <summary>
	/// This method is only for web call by put and delete method.(p_data format "a=1&b=2&c=3")
	/// </summary>
	/// <returns>result by server</returns>
	/// <param name="p_uri">P_uri.</param>
	/// <param name="p_data">P_data.</param>
	/// <param name="p_callMethod">P_call method.</param>
	public static string CallServerByPutOrDeleteMethod(string p_uri, string p_data, CallMethod p_callMethod)
	{
		//
		string l_method = p_callMethod == CallMethod.DELETE ? "DELETE" : "PUT";
		HttpWebRequest l_request = (HttpWebRequest)System.Net.WebRequest.Create(p_uri);
		Debug.Log (p_uri + " " + l_method);
		l_request.Method = l_method;
		l_request.Credentials = CredentialCache.DefaultCredentials;
		l_request.ContentLength = p_data.Length;
		l_request.ContentType =  "application/x-www-form-urlencoded";

		Stream l_requestStream = l_request.GetRequestStream();          
		l_requestStream.Write( System.Text.Encoding.UTF8.GetBytes(p_data), 0, p_data.Length);           
		l_requestStream.Close();

		Stream l_responseStream =  l_request.GetResponse().GetResponseStream();

		StreamReader l_responseReader = new StreamReader (l_responseStream,System.Text.Encoding.GetEncoding ("utf-8"));	
		string l_stringResponse = l_responseReader.ReadToEnd(); 
		l_responseStream.Close ();

		return null == l_stringResponse||string.Empty == l_stringResponse?"{}":l_stringResponse;
	}

}
