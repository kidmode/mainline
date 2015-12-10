/*
 * File:           GAFObjectStateData.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;

[System.Serializable]
public class GAFObjectStateData 
{	
	#region Members	
	
	private int 	m_ID 				= 0;
	private int 	m_ZOrder			= 0;
	private float 	m_A					= 0f;
	private float 	m_B					= 0f;
	private float 	m_C					= 0f;
	private float  	m_D					= 0f;
	private float 	m_Tx				= 0f;
	private float 	m_Ty				= 0f;
	private float 	m_Alpha				= 0f;
	private int 	m_MaskID 			= -1;
	private float 	m_HorizontalBlur 	= 0f;
	private float  	m_VerticalBlur 		= 0f;
	private GAFColorTransformationMatrix m_ColorTransformationMatrix;
	
	#endregion // Members
	
	#region Interface
	
	public GAFObjectStateData(uint _ID)
	{
		m_ID = (int)_ID;
	}
	
	#endregion // Interface
	
	#region Properties
	
	public uint id
	{
		get
		{
			return (uint)m_ID;
		}
	}
	
	public int zOrder
	{
		get
		{
			return m_ZOrder;
		}
		
		set
		{
			m_ZOrder = value;
		}
	}
	
	public float a
	{
		get
		{
			return m_A;
		}
		
		set
		{
			m_A = value;
		}
	}
	
	public float b
	{
		get
		{
			return m_B;
		}
		
		set
		{
			m_B = value;
		}
	}
	
	public float c
	{
		get
		{
			return m_C;
		}
		
		set
		{
			m_C = value;
		}
	}
	
	public float d
	{
		get
		{
			return m_D;
		}
		set
		{
			m_D = value;
		}
	}
	
	public float tX
	{
		get
		{
			return m_Tx;
		}
		
		set
		{
			m_Tx = value;
		}
	}
	
	public float tY
	{
		get
		{
			return m_Ty;
		}
		
		set
		{
			m_Ty = value;
		}
	}
	
	public float alpha
	{
		get
		{
			return m_Alpha;
		}
		
		set
		{
			m_Alpha = value;
		}
	}
	
	public int maskID
	{
		get
		{
			return m_MaskID;
		}
		
		set
		{
			m_MaskID = value;
		}
	}
	
	public float horizontalBlur
	{
		get
		{
			return m_HorizontalBlur;
		}
		
		set
		{
			m_HorizontalBlur = value;
		}
	}
	
	public float verticalBlur
	{
		get
		{
			return m_VerticalBlur;
		}
		
		set
		{
			m_VerticalBlur = value;
		}
	}
	
	public GAFColorTransformationMatrix colorMatrix
	{
		get
		{
			return m_ColorTransformationMatrix;
		}
		set
		{
			m_ColorTransformationMatrix = value;
		}
	}
	
	#endregion // Properties
}
