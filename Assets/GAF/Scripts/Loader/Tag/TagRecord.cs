/*
 * File:           TagRecord.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;

public class TagRecord
{
	#region Members

	private long 				m_ExpectedStreamPos;
	private long 				m_TagSize;
	private TagBase.TagType 	m_TagType;

	#endregion // Members

	#region Properties

	public long expectedStreamPosition
	{
		get
		{
			return m_ExpectedStreamPos;
		}
		set
		{
			m_ExpectedStreamPos = value;
		}
	}
	
	public long tagSize
	{
		get
		{
			return m_TagSize;
		}
		set
		{
			m_TagSize = value;
		}
	}
	
	
	public TagBase.TagType type
	{
		get
		{
			return m_TagType;
		}
		set
		{
			m_TagType = value;
		}
	}

	#endregion // Properties
}
