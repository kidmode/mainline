/*
 * File:           GAFNamedPartData.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;

public class GAFNamedPartData 
{
	#region Members
	
	private uint 	m_ObjectID;
	private string 	m_Name;
	
	#endregion // Members
	
	#region Interface
	
	public GAFNamedPartData( uint _ObjectID, string _Name )
	{
		m_ObjectID 	= _ObjectID;
		m_Name 		= _Name;
	}
	
	#endregion // Interface
	
	#region Properties
	
	public uint objectID
	{
		get
		{
			return m_ObjectID;
		}
	}
	
	public string name
	{
		get
		{
			return m_Name;
		}
	}
	
	#endregion // Properties
}
