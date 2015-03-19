/*
 * File:           TagBase.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.Collections.Generic;
using System.IO;

public abstract class TagBase 
{
	#region enums

	public enum TagType
	{
		  TagInvalid					= -1
		, TagEnd 						= 0
		, TagDefineAtlas 				= 1
		, TagDefineAnimationMasks 		= 2
		, TagDefineAnimationObjects 	= 3
		, TagDefineAnimationFrames 		= 4
		, TagDefineNamedParts 			= 5
		, TagDefineSequences 			= 6
		, TagDefineTextFields 			= 7
		, TagDefineAtlas2 				= 8
		, TagDefineStage 				= 9
		, TagDefineAnimationObjects2	= 10
		, TagDefineAnimationMasks2		= 11
		, TagDefineAnimationFrames2		= 12
		, TagDefineTimeline				= 13
	}

	#endregion

	#region interface

	public abstract void Read(
		  TagRecord				_Tag
		, BinaryReader 			_GAFFileReader
		, ref GAFAnimationData 	_SharedData
		, ref GAFTimelineData	_CurrentTimeline);

	#endregion
}
