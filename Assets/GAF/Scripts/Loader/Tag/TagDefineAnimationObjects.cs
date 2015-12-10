/*
 * File:           TagDefineAnimationObjects.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class TagDefineAnimationObjects : TagBase 
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
			uint objectId 			= _GAFFileReader.ReadUInt32();
			uint elementAtlasIdRef	= _GAFFileReader.ReadUInt32();

			_SharedData.rootTimeline.objects.Add(new GAFObjectData(objectId, elementAtlasIdRef, GAFObjectType.Texture));
		}
	}
}
