/*
 * File:           TagDefineAtlas.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class TagDefineAtlas : TagBase 
{
	public override void Read(
		  TagRecord				_Tag
		, BinaryReader 			_GAFFileReader
		, ref GAFAnimationData 	_SharedData
		, ref GAFTimelineData	_CurrentTimeline)
	{
		float scale = _GAFFileReader.ReadSingle();
		if (!_SharedData.scales.Contains(scale))
			_SharedData.scales.Add (scale);

		Dictionary<uint, GAFTexturesData> 		texturesInfos	= new Dictionary<uint, GAFTexturesData>();
		Dictionary<uint, GAFAtlasElementData>	atlasElements	= new Dictionary<uint, GAFAtlasElementData>();

		byte atlasesCount = _GAFFileReader.ReadByte();
		for (byte i = 0; i < atlasesCount; ++i)
		{	
			uint id 			= _GAFFileReader.ReadUInt32();
			byte sourcesCount 	= _GAFFileReader.ReadByte();

			Dictionary<float, string> files = new Dictionary<float, string>();
			for (byte j = 0; j < sourcesCount; ++j)
			{
				string filename = GAFReader.ReadString(_GAFFileReader);
				float csf		= _GAFFileReader.ReadSingle();

				if (!_SharedData.csfs.Contains(csf))
					_SharedData.csfs.Add (csf);

				files.Add(csf, filename);
			}

			texturesInfos.Add(id, new GAFTexturesData(id, files));
		}

		uint elementsCount = _GAFFileReader.ReadUInt32();
		for (uint i = 0; i < elementsCount; ++i)
		{
			Vector2 pivotPoint 		= GAFReader.ReadVector2(_GAFFileReader);
			Vector2 origin			= GAFReader.ReadVector2(_GAFFileReader);
			float 	elementScale	= _GAFFileReader.ReadSingle();
			float 	width			= _GAFFileReader.ReadSingle();
			float 	height			= _GAFFileReader.ReadSingle();
			uint 	atlasIdx		= _GAFFileReader.ReadUInt32();
			uint 	elementAtlasIdx	= _GAFFileReader.ReadUInt32();

			atlasElements.Add(elementAtlasIdx, new GAFAtlasElementData(
				  elementAtlasIdx
				, pivotPoint.x
				, pivotPoint.y
				, origin.x
				, origin.y
				, width
				, height
				, atlasIdx
				, elementScale
				, new Rect(0, 0, 0, 0)));
		}

		_SharedData.rootTimeline.atlases.Add (new GAFAtlasData (scale, texturesInfos, atlasElements));
	}
}
