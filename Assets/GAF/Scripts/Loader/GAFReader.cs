/*
 * File:           GAFReader.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

#define GAF_SUPPORT_COMPRESSED

using UnityEngine;
using System.IO;
using System.Collections.Generic;

#if GAF_SUPPORT_COMPRESSED
using Ionic.Zlib;
#endif // GAF_SUPPORT_COMPRESSED

using GAF.Utils;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public class GAFReader 
{
	#region Static

	private const uint TimelinesBaseVersion = 4;

	#endregion // Static

	#region Interface
	
	public void Load(
		  byte [] _AssetData
		, ref GAFAnimationData _SharedData)
	{
#if !UNITY_PRO_LICENSE && GAF_SUPPORT_COMPRESSED && UNITY_EDITOR
		if (PlayerSettings.apiCompatibilityLevel == ApiCompatibilityLevel.NET_2_0_Subset &&
		    Application.platform != RuntimePlatform.WindowsEditor &&
			Application.platform != RuntimePlatform.WindowsPlayer)
		{
			GAFUtils.Warning ("GAF! You are using compressed 'gaf' in free unity. Set API compatibility level as '.NET'!");
		}
#endif // !UNITY_PRO_LICENSE && GAF_SUPPORT_COMPRESSED && UNITY_EDITOR

		GAFHeader header = new GAFHeader ();

		MemoryStream fstream = new MemoryStream(_AssetData);
		using (BinaryReader freader = new BinaryReader(fstream))
		{
			if (freader.BaseStream.Length > GAFHeader.headerDataOffset)
			{
				header.Read(freader);
				if (header.isValid)
				{
					_SharedData = new GAFAnimationData ();

					_SharedData.majorVersion = header.majorVersion;
					_SharedData.minorVersion = header.minorVersion;

					switch(header.compression)
					{
					case GAFHeader.CompressionType.CompressedNone:
						Read(freader, ref _SharedData);
						break;

					case GAFHeader.CompressionType.CompressedZip:
#if GAF_SUPPORT_COMPRESSED
						using (ZlibStream zlibStream = new ZlibStream(fstream, CompressionMode.Decompress))
						{
							byte [] uncompressedBuffer = new byte[header.fileLength];
							zlibStream.Read(uncompressedBuffer, 0, uncompressedBuffer.Length);

							using (BinaryReader reader = new BinaryReader(new MemoryStream(uncompressedBuffer)))
							{
								Read(reader, ref _SharedData);
							}
						}
						break;
#else
						GAFUtils.Assert(false, "GAF. Compressed gaf format is not supported in your plugin!");
						break;
#endif // GAF_SUPPORT_COMPRESSED
					}
				}
			}
		}
	}

	#endregion

	#region Static Interface

	public static Vector2 ReadVector2(BinaryReader _Reader)
	{
		Vector2 retVal = new Vector2();
		retVal.x = _Reader.ReadSingle();
		retVal.y = _Reader.ReadSingle();
		return retVal;
	}

	public static Rect ReadRect(BinaryReader _Reader)
	{
		Rect retVal = new Rect();
		retVal.x 		= _Reader.ReadSingle();
		retVal.y 		= _Reader.ReadSingle();
		retVal.width 	= _Reader.ReadSingle();
		retVal.height	= _Reader.ReadSingle();
		return retVal;
	}

	public static string ReadString(BinaryReader _Reader)
	{
		ushort len = _Reader.ReadUInt16();

		byte [] data = _Reader.ReadBytes(len);

		System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
		return enc.GetString(data);
	}

	public static TagRecord OpenTag(BinaryReader _Reader)
	{
		TagRecord record = new TagRecord();
		
		TagBase.TagType tagType = (TagBase.TagType)_Reader.ReadUInt16 ();
		if (!System.Enum.IsDefined(typeof(TagBase.TagType), tagType))
			tagType = TagBase.TagType.TagInvalid;
		
		record.type 					= tagType;
		record.tagSize 					= _Reader.ReadUInt32();
		record.expectedStreamPosition	= _Reader.BaseStream.Position + record.tagSize;
		
		return record;
	}
	
	public static void CheckTag(TagRecord _Record, BinaryReader _Reader)
	{
		if (_Reader.BaseStream.Position != _Record.expectedStreamPosition)
		{
			GAFUtils.Error(
				"GAFReader::CloseTag - " +
				"Tag " + _Record.type.ToString() + " " +
				"hasn't been correctly read, tag length is not respected. " +
				"Expected " + _Record.expectedStreamPosition + " " +
				"but actually " + _Reader.BaseStream.Position + " !");
		}
	}
	
	public static void CloseTag(TagRecord _Record, BinaryReader _Reader)
	{
		_Reader.BaseStream.Position = _Record.expectedStreamPosition;
	}

	#endregion

	#region Implementation

	private void Read(
		  BinaryReader _GAFFileReader
		, ref GAFAnimationData _SharedData)
	{
		var tagReaders = GetTagsDictionary (_SharedData.majorVersion);

		if (_SharedData.majorVersion >= TimelinesBaseVersion)
		{
			uint scalesCount = _GAFFileReader.ReadUInt32();
			for (uint i = 0; i < scalesCount; ++i)
				_SharedData.scales.Add(_GAFFileReader.ReadSingle());

			uint csfsCount = _GAFFileReader.ReadUInt32();
			for (uint i = 0; i < csfsCount; ++i)
				_SharedData.csfs.Add(_GAFFileReader.ReadSingle());
		}
		else
		{
			uint 		id 				= 0;
			string		linkageName 	= "rootTimeline";
			uint 		framesCount		= (uint)_GAFFileReader.ReadUInt16();
			Rect 		frameSize		= ReadRect(_GAFFileReader);
			Vector2		pivot			= ReadVector2(_GAFFileReader);

			_SharedData.timelines.Add((int)id, new GAFTimelineData(id, linkageName, framesCount, frameSize, pivot));
		}

		while (_GAFFileReader.BaseStream.Position < _GAFFileReader.BaseStream.Length)
		{
			TagRecord record;
			try
			{
				record = OpenTag(_GAFFileReader);
			}
			catch (System.Exception _exception)
			{
				throw new GAFException("GAF! GAFReader::Read - Failed to open tag! Stream position - " + _GAFFileReader.BaseStream.Position.ToString() + "\nException - " + _exception);
			}

			if (record.type != TagBase.TagType.TagInvalid &&
			    tagReaders.ContainsKey(record.type))
			{
				try
				{
					GAFTimelineData data = null;
					tagReaders[record.type].Read(record, _GAFFileReader, ref _SharedData, ref data);
				}
				catch (System.Exception _exception)
				{
					throw new GAFException("GAF! GAFReader::Read - Failed to read tag - " + record.type.ToString() + "\n Exception - " + _exception.ToString(), record);
				}

				CheckTag(record, _GAFFileReader);
			}
			else
			{
				CloseTag(record, _GAFFileReader);
			}
		}

		if (_SharedData != null)
			foreach(var timeline in _SharedData.timelines.Values)
				timeline.sequences.Add(new GAFSequenceData("Default", 1, timeline.framesCount));
	}

	private static Dictionary<TagBase.TagType, TagBase> GetTagsDictionary(uint _MajorVersion)
	{
		var tagReaders = new Dictionary<TagBase.TagType, TagBase>();
		tagReaders.Add(TagBase.TagType.TagDefineAtlas				, new TagDefineAtlas());
		tagReaders.Add(TagBase.TagType.TagDefineAnimationMasks		, new TagDefineAnimationMasks());
		tagReaders.Add(TagBase.TagType.TagDefineAnimationObjects	, new TagDefineAnimationObjects());
		tagReaders.Add(TagBase.TagType.TagDefineAnimationFrames		, new TagDefineAnimationFrames());
		tagReaders.Add(TagBase.TagType.TagDefineNamedParts			, new TagDefineNamedParts());
		tagReaders.Add(TagBase.TagType.TagDefineSequences			, new TagDefineSequences());
		tagReaders.Add(TagBase.TagType.TagDefineStage				, new TagDefineStage());
		tagReaders.Add(TagBase.TagType.TagDefineTimeline			, new TagDefineTimeline());
		tagReaders.Add(TagBase.TagType.TagEnd						, new TagDefineEnd());

		return tagReaders;
	}

	#endregion
}
