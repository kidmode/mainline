/*
 * File:           GAFSequenceData.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;

public class GAFSequenceData 
{
	#region Members
	
	private string m_Name;
	private uint m_StartFrame;
	private uint m_EndFrame;
	
	#endregion // Members
	
	#region Interface
	
	public GAFSequenceData( string _Name, uint _StartFrame, uint _EndFrame )
	{
		m_Name 			= _Name;
		m_StartFrame 	= _StartFrame;
		m_EndFrame 		= _EndFrame;
	}
	
	#endregion // Interface
	
	#region Properties
	
	public string name
	{
		get
		{
			return m_Name;
		}
	}
	
	public uint startFrame
	{
		get
		{
			return m_StartFrame;
		}
	}
	
	public uint endFrame
	{
		get
		{
			return m_EndFrame;
		}
	}
	
	#endregion // Properties
}
