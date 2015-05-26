using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawing : System.Object
{
	public Drawing(Hashtable p_data = null)
	{
		if (p_data != null)
			_fromHashtable(p_data);
	}

	public string id { get { return m_id; } }
	public string mediumUrl { get { return m_mediumUrl; } }
	public string largeUrl { get { return m_largeUrl; } }
	public bool favorite { get { return m_favorite; } }
	public Texture2D largeIcon {get{return m_largeIcon;} set{ m_largeIcon = value;}}

	private void _fromHashtable(Hashtable p_data)
	{
		m_id = p_data["id"].ToString();
		m_mediumUrl = p_data["medium_url"].ToString();
		m_largeUrl = p_data["large_url"].ToString();
		m_favorite = bool.Parse(p_data["favorite"].ToString());
	}

	public void dispose ()
	{
		if (m_largeIcon != null)
		{
			GameObject.Destroy(m_largeIcon);
		}
		
		m_largeIcon = null;
	}

	private string m_id;
	private string m_mediumUrl;
	private string m_largeUrl;
	private bool m_favorite;

	private Texture2D m_largeIcon;
}

