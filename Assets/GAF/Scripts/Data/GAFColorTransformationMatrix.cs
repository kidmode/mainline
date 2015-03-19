/*
 * File:           GAFColorTransformationMatrix.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;

public enum GAFColorTransformIndex
{
	  GAFCTI_R
	, GAFCTI_G
	, GAFCTI_B
	, GAFCTI_A
	, COUNT
};

public enum GAFFilterType
{
	  GFT_DropShadow = 0
	, GFT_Blur = 1
	, GFT_Glow = 2
	, GFT_ColorMatrix = 6
};

[System.Serializable]
public class GAFColorTransformationMatrix 
{	
	#region Members

	private byte m_MultiplierR;
	private byte m_MultiplierG;
	private byte m_MultiplierB;
	private byte m_MultiplierA;
	private float m_OffsetR;
	private float m_OffsetG;
	private float m_OffsetB;
	private float m_OffsetA;	

	#endregion // Members

	#region Interface 
	
	public GAFColorTransformationMatrix()
	{
		multipliers 	= new Color32((byte)255, (byte)255, (byte)255, (byte)255);
		offsets			= new Vector4(255f, 255f, 255f, 255f);
		m_OffsetA 		= (byte)0;
	}

	public GAFColorTransformationMatrix(Color32 _Multipliers, Vector4 _Offsets)
	{
		multipliers 	= _Multipliers;
		offsets 		= _Offsets;
	}
	
	#endregion // Interface
	
	#region Properties

	public Color32 multipliers
	{
		get
		{			
			return new Color32( m_MultiplierR, m_MultiplierG, m_MultiplierB, m_MultiplierA );
		}
		set
		{
			m_MultiplierR = value.r;
			m_MultiplierG = value.g;
			m_MultiplierB = value.b;
			m_MultiplierA = value.a;
		}
	}

	public Vector4 offsets
	{
		get
		{
			return new Vector4(m_OffsetR, m_OffsetG, m_OffsetB, m_OffsetA);
		}
		set
		{
			m_OffsetR = value.x;
			m_OffsetG = value.y;
			m_OffsetB = value.z;
			m_OffsetA = value.w;
		}
	}

	#endregion // Properties
}
