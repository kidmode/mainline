using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TagDefineTimeline : TagBase 
{
	public override void Read(
		  TagRecord				_Tag
		, BinaryReader 			_GAFFileReader
		, ref GAFAnimationData 	_SharedData
		, ref GAFTimelineData	_RootTimeline)
	{
		uint 		id 				= _GAFFileReader.ReadUInt32();
		uint 		framesCount		= _GAFFileReader.ReadUInt32();
		Rect 		frameSize		= GAFReader.ReadRect(_GAFFileReader);
		Vector2		pivot			= GAFReader.ReadVector2(_GAFFileReader);
		byte		hasLinkage		= _GAFFileReader.ReadByte();
		string		linkageName 	= string.Empty;

		if (hasLinkage == 1)
			linkageName = GAFReader.ReadString(_GAFFileReader);

		var timeline = new GAFTimelineData (id, linkageName, framesCount, frameSize, pivot);
		_SharedData.timelines.Add ((int)id, timeline);

		var tagReaders = GetTagsDictionary ();
		while (_GAFFileReader.BaseStream.Position < _Tag.expectedStreamPosition)
		{
			TagRecord record;
			try
			{
				record = GAFReader.OpenTag(_GAFFileReader);
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
					tagReaders[record.type].Read(record, _GAFFileReader, ref _SharedData, ref timeline);
				}
				catch (System.Exception _exception)
				{
					throw new GAFException("GAF! GAFReader::Read - Failed to read tag - " + record.type.ToString() + "\n Exception - " + _exception.ToString(), record);
				}
				
				GAFReader.CheckTag(record, _GAFFileReader);
			}
			else
			{
				GAFReader.CloseTag(record, _GAFFileReader);
			}
		}
	}

	private static Dictionary<TagBase.TagType, TagBase> GetTagsDictionary()
	{
		var tagReaders = new Dictionary<TagBase.TagType, TagBase>();
		tagReaders.Add(TagBase.TagType.TagDefineNamedParts			, new TagDefineNamedParts());
		tagReaders.Add(TagBase.TagType.TagDefineSequences			, new TagDefineSequences());
		//tagReaders.Add(TagBase.TagType.TagDefineTextFields		, new TagDefineTextFields ());
		tagReaders.Add(TagBase.TagType.TagDefineAtlas2				, new TagDefineAtlas2());
		tagReaders.Add(TagBase.TagType.TagDefineStage				, new TagDefineStage());
		tagReaders.Add(TagBase.TagType.TagDefineAnimationObjects2	, new TagDefineAnimationObjects2());
		tagReaders.Add(TagBase.TagType.TagDefineAnimationMasks2		, new TagDefineAnimationMasks2());
		tagReaders.Add(TagBase.TagType.TagDefineAnimationFrames2	, new TagDefineAnimationFrames2());
		tagReaders.Add(TagBase.TagType.TagEnd						, new TagDefineEnd());
		return tagReaders;
	}
}
