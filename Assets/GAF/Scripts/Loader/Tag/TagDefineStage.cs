using UnityEngine;
using System.IO;
using System.Collections;

public class TagDefineStage : TagBase 
{
	public override void Read(
		  TagRecord				_Tag
		, BinaryReader 			_GAFFileReader
		, ref GAFAnimationData 	_SharedData
		, ref GAFTimelineData	_CurrentTimeline)
	{
		_SharedData.fps 	= _GAFFileReader.ReadByte ();
		byte a 				= _GAFFileReader.ReadByte ();
		byte r 				= _GAFFileReader.ReadByte ();
		byte g 				= _GAFFileReader.ReadByte ();
		byte b 				= _GAFFileReader.ReadByte ();

		_SharedData.color 	= new Color (r / 255f, g / 255f, b / 255f, a / 255f);
		_SharedData.width	= _GAFFileReader.ReadUInt16();
		_SharedData.height	= _GAFFileReader.ReadUInt16();
	}
}
