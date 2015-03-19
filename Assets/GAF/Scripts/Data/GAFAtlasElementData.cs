/*
 * File:           GAFAtlasElementData.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;

public class GAFAtlasElementData
{
	#region Members
	
	private uint 	m_ID;
	private float 	m_PivotX;
	private float 	m_PivotY;
	private float 	m_X;
	private float 	m_Y;
	private float 	m_Width;
	private float 	m_Height;
	private uint 	m_AtlasID;
	private float 	m_Scale;
	private Rect 	m_Scale9GridRect;
	
	#endregion
	
	#region Interface
	
	public GAFAtlasElementData(
		  uint 	_ID
		, float _PivotX
		, float _PivotY
		, float _X
		, float _Y
		, float _Width
		, float	_Height
		, uint 	_AtlasID
		, float _Scale
		, Rect	_Scale9GridRect)
	{
		m_ID 				= _ID;
		m_PivotX 			= _PivotX;
		m_PivotY 			= _PivotY;
		m_X 				= _X;
		m_Y 				= _Y;
		m_Width 			= _Width;
		m_Height 			= _Height;
		m_AtlasID 			= _AtlasID;
		m_Scale 			= _Scale;
		m_Scale9GridRect	= _Scale9GridRect;
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
	
	public float pivotX
	{
		get
		{
			return m_PivotX;	
		}
	}
	
	public float pivotY
	{
		get
		{
			return m_PivotY;	
		}
	}
	
	public float x
	{
		get
		{
			return m_X;	
		}
	}
	
	public float y
	{
		get
		{
			return m_Y;
		}
	}
	
	public float width
	{
		get
		{
			return m_Width;
		}
	}
	
	public float height
	{
		get
		{
			return m_Height;
		}
	}
	
	public uint atlasID
	{
		get
		{
			return m_AtlasID;
		}
	}
	
	public float scale
	{
		get
		{
			return m_Scale;
		}
	}

	public Rect scale9GridRect
	{
		get
		{
			return m_Scale9GridRect;
		}
	}

	#endregion // Properties
}
