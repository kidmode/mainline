/*
 * File:           GAFFrameEvent.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;

public class GAFFrameEvent
{
	#region Members

	private System.Action<GAFMovieClip>			m_Callback	= null;
	private string								m_ID		= string.Empty;

	#endregion Members

	#region Interface

	public GAFFrameEvent(System.Action<GAFMovieClip> _Callback)
	{
		m_Callback 	= _Callback;
		m_ID 		= System.Guid.NewGuid().ToString();
	}
	
	public string id
	{
		get
		{
			return m_ID;
		}
	}
	
	public void trigger(GAFMovieClip _Clip)
	{
		m_Callback (_Clip);
	}

	#endregion // Interface
}
