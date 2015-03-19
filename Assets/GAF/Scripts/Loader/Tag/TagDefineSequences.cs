/*
 * File:           TagDefineSequences.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class TagDefineSequences : TagBase 
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
			string id = GAFReader.ReadString(_GAFFileReader);

			ushort start 	= _GAFFileReader.ReadUInt16();
			ushort end 		= _GAFFileReader.ReadUInt16();

			var data = new GAFSequenceData(id, (uint)start, (uint)end);
			if (_CurrentTimeline == null)
				_SharedData.rootTimeline.sequences.Add(data);
			else
				_CurrentTimeline.sequences.Add(data);
		}
	}
}
