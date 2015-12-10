/*
 * File:           GAFActionData.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GAFActionData 
{
	#region Enums

	public enum ActionType
	{
		  None			= -1
		, Stop 			= 0
		, Play 			= 1
		, GotoAndStop 	= 2
		, GotoAndPlay	= 3
		, DispatchEvent	= 4
	}

	#endregion // Enums

	private ActionType 		m_Type			= ActionType.None;
	private List<string> 	m_Parameters	= new List<string>();

	#region Members

	#endregion // Members

	#region Interface

	public GAFActionData(ActionType _Type)
	{
		m_Type = _Type;
	}

	#endregion // Interface

	#region Properties

	public ActionType type
	{
		get
		{
			return m_Type;
		}
	}

	public List<string> parameters
	{
		get
		{
			return m_Parameters;
		}
	}

	#endregion // Properties
}
