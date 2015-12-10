/*
 * File:           TagDefineNamedParts.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class TagDefineNamedParts : TagBase 
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
			uint 	objectIdRef = _GAFFileReader.ReadUInt32();
			string 	name		= GAFReader.ReadString(_GAFFileReader);

			var data = new GAFNamedPartData(objectIdRef, name);
			if (_CurrentTimeline == null)
				_SharedData.rootTimeline.namedParts.Add(data);
			else
				_CurrentTimeline.namedParts.Add(data);
		}
	}
}
