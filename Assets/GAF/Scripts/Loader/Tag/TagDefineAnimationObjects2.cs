/*
 * File:           TagDefineAnimationObjects2.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections;
using System.IO;

public class TagDefineAnimationObjects2 : TagBase 
{
	public override void Read(
		  TagRecord				_Tag
		, BinaryReader 			_GAFFileReader
		, ref GAFAnimationData 	_SharedData
		, ref GAFTimelineData	_CurrentTimeline)
	{
		uint count = _GAFFileReader.ReadUInt32();
		
		for (uint i = 0; i < count; ++i)
		{
			uint 			objectId 			= _GAFFileReader.ReadUInt32();
			uint 			elementAtlasIdRef	= _GAFFileReader.ReadUInt32();
			GAFObjectType	type				= (GAFObjectType)_GAFFileReader.ReadUInt16();

			_CurrentTimeline.objects.Add(new GAFObjectData(objectId, elementAtlasIdRef, type));
		}
	}
}
