/*
 * File:           GAFAtlasData.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GAFAtlasData
{
	#region Members
	
	private float 									m_Scale;
	private Dictionary<uint, GAFTexturesData> 		m_TexturesData	= null;
	private Dictionary<uint, GAFAtlasElementData>	m_Elements 		= null;
	
	#endregion
	
	#region Interface
	
	public GAFAtlasData(
		  float _Scale
		, Dictionary<uint, GAFTexturesData> 	_TexturesData
		, Dictionary<uint, GAFAtlasElementData> _Elements)
	{
		m_Scale 		= _Scale;
		m_TexturesData 	= _TexturesData;
		m_Elements 		= _Elements;
	}

	public GAFTexturesData getAtlas(uint _AtlasID)
	{
		if (m_TexturesData.ContainsKey(_AtlasID))
			return m_TexturesData[_AtlasID];
		else
			return null;
	}

	public GAFAtlasElementData getElement(uint _ElementID)
	{
		if (m_Elements.ContainsKey(_ElementID))
			return m_Elements[_ElementID];
		else
			return null;
	}
	
	#endregion // Interface
	
	#region Properties
	
	public float scale
	{
		get
		{
			return m_Scale;
		}
	}

	public Dictionary<uint, GAFTexturesData> texturesData
	{
		get
		{
			return m_TexturesData;
		}
	}

	#endregion // Properties
}
