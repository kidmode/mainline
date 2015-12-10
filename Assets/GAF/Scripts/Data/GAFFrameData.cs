/*
 * File:           GAFFrameData.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GAFFrameData 
{	
	#region Members
	
	private uint 									m_FrameNumber;
	private Dictionary<uint, GAFObjectStateData> 	m_States;
	
	#endregion // Members
	
	#region Interface
	
	public GAFFrameData( uint _FrameNumber )
	{
		m_FrameNumber = _FrameNumber;
		
		m_States = new Dictionary<uint, GAFObjectStateData>();
	}	
	
	public void addState( GAFObjectStateData state )
	{
		m_States.Add(state.id, state);
	}
	
	#endregion // Interface
	
	#region Properties
	
	public uint frameNumber
	{
		get
		{
			return m_FrameNumber;
		}
	}
	
	public Dictionary<uint, GAFObjectStateData> states
	{
		get
		{
			return m_States;
		}
		
		set
		{
			m_States = value;
		}
	}
	
	#endregion // Properties
}
