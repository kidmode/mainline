using UnityEngine;
using System.Collections;
using System.Net;
using System;
using System.Text;
using System.IO;

public class UploadHelper
{
	string boundary = "----WebKitFormBoundaryE19zNvXGzXaLvS5C";  

	//url for production
	//	string url = "http://" + ZoodlesConstants.ZOODLES_PRODUCTION_HOST;
	string url = ZoodlesConstants.getHttpsHost();

	byte[] bs= new byte[0];

	public UploadHelper(string url)  
	{  
		this.url = ZoodlesConstants.getHttpsHost() + url;
	}

	public byte[] Send()  
	{  
		WebClient myWebClient = new WebClient();  
		myWebClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);  
		EndData();
		_Debug.log (url);
		return myWebClient.UploadData(url,bs);   
	}

	public void SendAsync(UploadDataCompletedEventHandler p_handler)
	{
		WebClient myWebClient = new WebClient();
		myWebClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);  
		EndData();
		_Debug.log (url);
		myWebClient.UploadDataCompleted += p_handler;
		myWebClient.UploadDataAsync(new Uri(url), bs);   
	}

	public void ClearData()  
	{  
		Array.Clear(bs, 0, bs.Length);  
	}

	public void AddTextParameter(string name, string value)  
	{  
		StringBuilder s = new StringBuilder();  
		s.Append("--").Append(boundary).Append("\r\n");  
		s.Append("Content-Disposition:  form-data;  name=\"" + name + "\"\r\n");  
		s.Append("\r\n");  
		s.Append(value).Append("\r\n");  
		AppendString(s.ToString());  
	}

	public void AddFileParameter(string name, FileInfo file)  
	{  
		StringBuilder s = new StringBuilder();  
		s.Append("--").Append(boundary).Append("\r\n");  
		s.Append("Content-Disposition:  form-data;  name=\"" + name + "\";  filename=\"" + file.Name + "\"\r\n");  
		s.Append("Content-Type: " + GetContentType(file) + "\r\n");  
		s.Append("\r\n");  
		AppendString(s.ToString());  
		AppendBytes(GetFileBytes(file));  
		AppendString("\r\n");       
	}

	public void AddByteParameter(string p_name, byte[] p_bytes, string p_extension = "txt")
	{
		StringBuilder s = new StringBuilder();  
		s.Append("--").Append(boundary).Append("\r\n");  
		s.Append("Content-Disposition:  form-data;  name=\"" + p_name + "\";  filename=\"file." + p_extension + "\"\r\n");  
		s.Append("Content-Type: " + p_extension + "\r\n");  
		s.Append("\r\n");
		AppendString(s.ToString());
		AppendBytes(p_bytes);  
		AppendString("\r\n");
	}

	private byte[] GetFileBytes(FileInfo f)  
	{  
		FileStream fs = new FileStream(f.FullName, FileMode.Open);  
		BinaryReader br = new BinaryReader(fs);  
		byte[] b = br.ReadBytes((int)f.Length);  
		br.Close();  
		fs.Close();  
		return b;  
	}

	private string GetContentType(FileInfo f)  
	{  
		string l_extension = f.Extension;
//		if( l_extension.Equals(".wav") )
//		{
//			return "wav";
//		}
//		else
//		{
//			return "txt";
//		}
		return l_extension.TrimStart ('.');
	}

	private void AppendBytes(byte[] bytes)   
	{  
		byte[] newByte = new byte[bs.Length + bytes.Length];  
		Array.Copy(bs, 0, newByte, 0, bs.Length);  
		Array.Copy(bytes, 0, newByte, bs.Length, bytes.Length);  
		bs = newByte;
	}

	private void AppendString(string value)  
	{  
		byte[] bytes = Encoding.UTF8.GetBytes(value);  
		AppendBytes(bytes);  
	}

	private void EndData()  
	{
		StringBuilder s = new StringBuilder();  
		s.Append("--").Append(boundary).Append("--\r\n");  
		s.Append("\r\n");  
		AppendString(s.ToString());  
	}
}
