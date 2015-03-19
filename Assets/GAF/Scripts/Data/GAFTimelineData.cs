/*
 * File:           GAFTimelineData.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GAFObjectType
{
	  None		= -1
	, Texture 	= 0
	, TextField = 1
	, Timeline	= 2
}

public class GAFTimelineData 
{
	#region Members

	private uint							m_ID			= 0;
	private string							m_LinkageName	= string.Empty;
	private List<GAFAtlasData> 				m_Atlases 		= new List<GAFAtlasData>();
	private List<GAFObjectData>				m_Objects 		= new List<GAFObjectData>();
	private List<GAFObjectData>				m_Masks			= new List<GAFObjectData>();
	private Dictionary<uint, GAFFrameData> 	m_Frames 		= new Dictionary<uint, GAFFrameData>();
	private List<GAFSequenceData>			m_Sequences 	= new List<GAFSequenceData>();
	private List<GAFNamedPartData> 			m_NamedParts 	= new List<GAFNamedPartData>();
	private uint 							m_FramesCount 	= 0;
	private Rect 							m_FrameSize;
	private Vector2 						m_Pivot;

	#endregion // Members

	#region Interface

	public GAFTimelineData(
		  uint 		_ID
		, string 	_LinkageName
		, uint 		_FramesCount
		, Rect 		_FrameSize
		, Vector2	_Pivot)
	{
		m_ID 			= _ID;
		m_LinkageName	= _LinkageName;
		m_FramesCount	= _FramesCount;
		m_FrameSize		= _FrameSize;
		m_Pivot			= _Pivot;
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

	public string linkageName
	{
		get
		{
			return m_LinkageName;
		}
	}

	public List<GAFAtlasData> atlases
	{
		get
		{
			return m_Atlases;
		}
	}
	
	public List<GAFObjectData> objects
	{
		get
		{
			return m_Objects;
		}
	}

	public List<GAFObjectData> masks
	{
		get
		{
			return m_Masks;
		}
	}
	
	public Dictionary<uint, GAFFrameData> frames
	{
		get
		{
			return m_Frames;
		}
	}
	
	public List<GAFSequenceData> sequences
	{
		get
		{
			return m_Sequences;
		}
	}
	
	public List<GAFNamedPartData> namedParts
	{
		get
		{
			return m_NamedParts;
		}
	}
	
	public uint framesCount
	{
		get
		{
			return m_FramesCount;
		}
	}

	public Rect frameSize
	{
		get
		{
			return m_FrameSize;
		}
	}
	
	public Vector2 pivot
	{
		get
		{
			return m_Pivot;
		}
	}
	
	#endregion // Properties
}
