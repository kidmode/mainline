/*
 * File:           GAFTexturesData.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections.Generic;

public class GAFTexturesData
{
	#region Members

	private uint 	 					m_ID;
	private Dictionary<float, string> 	m_Files;
	
	#endregion // Members
	
	#region Interface
	
	public GAFTexturesData(uint _ID, Dictionary<float, string> _Files)
	{
		m_ID 	= _ID;
		m_Files = _Files;
	}
	
	public string getFileName(float _CSF)
	{
		if (m_Files.ContainsKey(_CSF))
			return m_Files[_CSF];
		else
			return string.Empty;
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

	public Dictionary<float, string> files
	{
		get
		{
			return m_Files;
		}
	}
	
	#endregion // Properties
}
