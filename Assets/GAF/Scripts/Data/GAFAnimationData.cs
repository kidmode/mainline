/*
 * File:           GAFAnimationData.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GAFAnimationData
{
	#region Members

	private ushort 								m_MajorVersion 	= 0;
	private ushort 								m_MinorVersion 	= 0;
	private ushort								m_FPS			= 30;
	private Color								m_Color			= Color.white;
	private ushort 								m_Width			= 0;
	private ushort 								m_Height 		= 0;
	private List<float>							m_Scales		= new List<float>();
	private List<float>							m_CSFs			= new List<float>();
	private Dictionary<int, GAFTimelineData>	m_Timelines  	= new Dictionary<int, GAFTimelineData>();

	#endregion // Members

	#region Properties

	public ushort majorVersion
	{
		get
		{
			return m_MajorVersion;
		}

		set
		{
			m_MajorVersion = value;
		}
	}

	public ushort minorVersion
	{
		get
		{
			return m_MinorVersion;
		}
		
		set
		{
			m_MinorVersion = value;
		}
	}

	public ushort fps
	{
		get
		{
			return m_FPS;
		}

		set
		{
			m_FPS = value;
		}
	}

	public Color color
	{
		get
		{
			return m_Color;
		}

		set
		{
			m_Color = value;
		}
	}

	public ushort width
	{
		get
		{
			return m_Width;
		}

		set
		{
			m_Width = value;
		}
	}

	public ushort height
	{
		get
		{
			return m_Height;
		}

		set
		{
			m_Height = value;
		}
	}

	public List<float> scales
	{
		get
		{
			return m_Scales;
		}
	}

	public List<float> csfs
	{
		get
		{
			return m_CSFs;
		}
	}

	public Dictionary<int, GAFTimelineData> timelines
	{
		get
		{
			return m_Timelines;
		}
	}

	public GAFTimelineData rootTimeline
	{
		get
		{
			return m_Timelines[0];
		}
	}

	#endregion // Properties
}
