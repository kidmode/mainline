using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using System.Linq;
using System.Collections.Specialized;

public class HttpsWWW : object 
{
	public HttpsWWW(string p_url)
	{
		MakeRestCall (p_url);
	}

	public HttpsWWW(string p_url,Hashtable p_hashtable)
	{
		MakeRestCall (p_url,p_hashtable);
	}

	public string url
	{
		get { return m_url;   }
		set { m_url = value;  }
	}

	private void MakeUploadCall(string file, string paramName, string contentType, Hashtable param,byte[] buffer)
	{
		Debug.Log(string.Format("Uploading {0} to {1}", file, url));
		string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
		byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
		m_request.ContentType = "multipart/form-data; boundary=" + boundary;
		m_request.Method = "POST";
		m_request.KeepAlive = true;
		m_request.Credentials = System.Net.CredentialCache.DefaultCredentials;
		Stream rs = m_request.GetRequestStream();
		string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
		foreach (DictionaryEntry de in param)
		{
			rs.Write(boundarybytes, 0, boundarybytes.Length);
			string formitem = string.Format(formdataTemplate, de.Key.ToString(), de.Value.ToString());
			Debug.Log(formitem);
			byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
			rs.Write(formitembytes, 0, formitembytes.Length);
		}
		rs.Write(boundarybytes, 0, boundarybytes.Length);
		string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}.{2}\"\r\nContent-Type: {2}\r\n\r\n";
		string header = string.Format(headerTemplate, paramName, file, contentType);
		Debug.Log(header);
		Debug.Log (m_request.Headers.ToString());
		byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
		rs.Write(headerbytes, 0, headerbytes.Length);
		rs.Write(buffer, 0, buffer.Length);
		Debug.Log (m_request.Headers);
		byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
		rs.Write(trailer, 0, trailer.Length);
		rs.Close();
		rs = null;
	}

	private void MakeRestCall(string p_url,Hashtable p_param = null)
	{
		m_url = p_url;
		m_request = (HttpWebRequest)System.Net.WebRequest.Create (p_url);
		m_request.Timeout = 45000;
		ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback (ValidateServerCertificate);
		m_request.AuthenticationLevel = AuthenticationLevel.MutualAuthRequested;

		if(null != adminClient)
		{
			m_request.ClientCertificates.Add (adminClient);
		}

		if(null != p_param)
		{
			string fileName = string.Empty;
			foreach (DictionaryEntry de in p_param)
			{
				if(de.Value.GetType().FullName.Equals("System.Byte[]"))
				{
					fileName = de.Key.ToString();
					break;
				}
			}

			if(fileName.Equals(string.Empty))
			{
				string l_data = string.Empty;
				if( null != p_param && p_param.Keys.Count > 0 )
				{
					int l_count = p_param.Keys.Count - 1;
					
					foreach( string l_key in p_param.Keys )
					{
						l_data += l_key + "=" + p_param[l_key] + (l_count-- > 0 ? "&" : "");
					}
				}
				byte[] bs = Encoding.ASCII.GetBytes(l_data);          
				m_request.Method = "POST";
				m_request.ContentType = "application/x-www-form-urlencoded";
				m_request.ContentLength = bs.Length;

				try
				{
					using (Stream reqStream = m_request.GetRequestStream())
					{
						reqStream.Write(bs, 0, bs.Length);
					}
				}
				catch(WebException e)
				{
					m_errorMessage = e.Message;
					m_finished = true;
				}
			}
			else
			{
				byte[] l_bytes = p_param[fileName] as byte[];
				string l_contentType = p_param["contentType"].ToString();
				p_param.Remove(fileName);
				p_param.Remove("contentType");
				MakeUploadCall(fileName,fileName,l_contentType,p_param,l_bytes);
			}
		}
		try
		{
			m_request.BeginGetResponse (new AsyncCallback (ReadCallback), m_request);
		}
		catch(WebException e)
		{
			m_errorMessage = e.Message;
			m_finished = true;
		}
	}
	
	private void ReadCallback(IAsyncResult asynchronousResult)
	{
		HttpWebRequest l_request = (HttpWebRequest) asynchronousResult.AsyncState;
		try
		{
			HttpWebResponse l_response = (HttpWebResponse) l_request.EndGetResponse(asynchronousResult);

			if (l_response.StatusCode != HttpStatusCode.OK)
			{
				m_errorMessage = l_response.StatusDescription;
			}
			else
			{
				string l_upperUrl = m_url.ToUpper();
				if(l_upperUrl.Contains(".PNG") || l_upperUrl.Contains(".JPG") || l_upperUrl.Contains(".GIF") || 
				   l_upperUrl.Contains("GGPHT") || l_upperUrl.Contains("LH5") || l_upperUrl.Contains("WAV"))
				{
					using (BinaryReader lxBR = new BinaryReader(l_response.GetResponseStream()))
					{
						using (MemoryStream lxMS = new MemoryStream())
						{
							m_lnBuffer = lxBR.ReadBytes(1024);
							while (m_lnBuffer.Length > 0)
							{
								lxMS.Write(m_lnBuffer, 0, m_lnBuffer.Length);
								m_lnBuffer = lxBR.ReadBytes(1024);
							}
							m_lnBuffer = new byte[(int)lxMS.Length];
							lxMS.Position = 0;
							lxMS.Read(m_lnBuffer, 0, m_lnBuffer.Length);
						}
					}
				}
				else
				{
					using (StreamReader streamReader = new StreamReader(l_response.GetResponseStream()))
					{
						m_text = streamReader.ReadToEnd();
					}
				}
				
			}
			m_finished = true;
			l_request.Abort ();
			l_response.Close ();
		}
		catch(WebException e)
		{
			m_errorMessage = e.Message;
			m_finished = true;
		}
	}

	public Texture2D texture
	{
		get
		{
			string l_upperUrl = m_url.ToUpper();
			if(l_upperUrl.Contains(".PNG") || l_upperUrl.Contains(".JPG") || l_upperUrl.Contains(".GIF") || l_upperUrl.Contains("GGPHT") ||  l_upperUrl.Contains("LH5"))
			{
				Texture2D tex = new Texture2D(268,88,TextureFormat.ARGB32, false);
				tex.LoadImage(m_lnBuffer);
				return tex;
			}
			else
			{
				return null;
			}
		}
	}

	public static bool ValidateServerCertificate (
		object sender,
		System.Security.Cryptography.X509Certificates.X509Certificate certificate,
		X509Chain chain,
		System.Net.Security.SslPolicyErrors sslPolicyErrors)
	{
		bool isOk = true;
		// If there are errors in the certificate chain, look at each error to determine the cause.
		if (sslPolicyErrors != SslPolicyErrors.None) {
			for(int i=0; i<chain.ChainStatus.Length; i++) {
				if(chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown) {
					chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
					chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
					chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
					chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
					bool chainIsValid = chain.Build((X509Certificate2)certificate);
					if(!chainIsValid) {
						isOk = false;
					}
				}
			}
		}
		return isOk;
	}

	public string text
	{
		get 
		{
			return m_text;
		}
	}

	public bool isDone
	{
		get 
		{
			return m_finished;
		}
	}

	public string error
	{
		get 
		{
			return m_errorMessage;
		}
	}

	public byte[] bytes
	{
		get
		{
			return m_lnBuffer;
		}
	}

	public void Dispose()
	{
		if (null != m_texture2D)
			GameObject.Destroy (m_texture2D);
		m_request.Abort ();
	}

	private string m_url;
	private string m_text;
	private bool   m_finished;
	private string m_errorMessage;
	private Texture2D m_texture2D = null;
	private byte[] m_lnBuffer;

	private HttpWebRequest m_request;
	public static X509Certificate2 adminClient;
}
