/*
 * File:           GAFHeader.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using System.IO;
using UnityEngine;

public class GAFHeader 
{
	#region Enums

	public enum CompressionType
	{
		  CompressedNone 		= 0x00474146	// GAF
		, CompressedZip 		= 0x00474143,  	// GAC
	};

	#endregion // Enums
	
	#region Members
	
	private CompressionType	m_Compression;
	private int 			m_FileLength;
	private	byte 			m_MajorVersion;
	private	byte 			m_MinorVersion;
	
	#endregion

	#region Interface

	public void Read(BinaryReader _Reader)
	{
		m_Compression = (CompressionType)_Reader.ReadInt32();
		if (isValid)
		{
			m_MajorVersion 	= _Reader.ReadByte();
			m_MinorVersion	= _Reader.ReadByte();
			m_FileLength	= (int)	 _Reader.ReadUInt32();
		}
	}

	public bool isValid
	{
		get
		{
			return isCorrectHeader(m_Compression);
		}
	}

	public CompressionType compression
	{
		get
		{
			return m_Compression;
		}
	}

	public ushort majorVersion
	{
		get
		{
			return System.Convert.ToUInt16(m_MajorVersion);
		}
	}

	public ushort minorVersion
	{
		get
		{
			return System.Convert.ToUInt16(m_MinorVersion);
		}
	}

	public uint fileLength
	{
		get
		{
			return (uint)m_FileLength;
		}
	}

	public static int headerDataOffset
	{
		get
		{
			return sizeof(CompressionType) + sizeof(int) + 2 * sizeof(byte);
		}
	}

	public static bool isCorrectHeader(CompressionType _Compression)
	{
		return System.Enum.IsDefined (typeof(CompressionType), _Compression);
	}

	#endregion
}
