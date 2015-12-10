/*
 * File:           GAFObjectData.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;

public class GAFObjectData 
{	
	#region Members
	
	private uint 			m_ID				= 0;
	private uint 			m_AtlasElementID	= 0;
	private	GAFObjectType	m_ObjectType		= GAFObjectType.None;
	
	#endregion // Members
	
	#region Interface 
	
	public GAFObjectData(uint _ID, uint _AtlasElementID, GAFObjectType _Type)
	{
		m_ID 				= _ID;
		m_AtlasElementID 	= _AtlasElementID;
		m_ObjectType		= _Type;
	}
	
	#endregion // Interface
	
	#region Properties
	
	public uint id
	{
		get
		{
			return m_ID;
		}
	}
	
	public uint atlasElementID
	{
		get
		{
			return m_AtlasElementID;
		}
	}

	public GAFObjectType type
	{
		get
		{
			return m_ObjectType;
		}
	}

	#endregion // Properties
}
