/*
 * File:           TagDefineAnimationFrames.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class TagDefineAnimationFrames : TagBase 
{
	public override void Read(
		  TagRecord				_Tag
		, BinaryReader 			_GAFFileReader
		, ref GAFAnimationData 	_SharedData
		, ref GAFTimelineData	_CurrentTimeline)
	{
		uint framesCount = _GAFFileReader.ReadUInt32();
		for (uint i = 0; i < framesCount; ++i)
		{
			uint frameNumber = _GAFFileReader.ReadUInt32();
			GAFFrameData frame = new GAFFrameData(frameNumber);
			
			uint statesCount = _GAFFileReader.ReadUInt32();
			for (uint j = 0; j < statesCount; ++j)
			{
				frame.addState(ExctractState(_GAFFileReader, _SharedData.rootTimeline));
			}
			
			_SharedData.rootTimeline.frames.Add(frame.frameNumber, frame);
		}
	}

	private GAFObjectStateData ExctractState(BinaryReader _Reader, GAFTimelineData _Timeline)
	{
		bool hasColorTransform 	= System.Convert.ToBoolean(_Reader.ReadByte());
		bool hasMasks			= System.Convert.ToBoolean(_Reader.ReadByte());
		bool hasEffect			= System.Convert.ToBoolean(_Reader.ReadByte());

		GAFObjectStateData state = new GAFObjectStateData(_Reader.ReadUInt32());

		state.zOrder	= -(_Reader.ReadInt32()) + _Timeline.objects.Count;
		state.alpha		= _Reader.ReadSingle();
		state.a 		= _Reader.ReadSingle();
		state.b 		= _Reader.ReadSingle();
		state.c 		= _Reader.ReadSingle();
		state.d 		= _Reader.ReadSingle();
		state.tX 		= _Reader.ReadSingle();
		state.tY 		= _Reader.ReadSingle();
		
		if (hasColorTransform)
		{
			float [] ctxMul = new float[(int)GAFColorTransformIndex.COUNT];
			float [] ctxOff = new float[(int)GAFColorTransformIndex.COUNT];

			ctxMul[(int)GAFColorTransformIndex.GAFCTI_A] = state.alpha;
			ctxOff[(int)GAFColorTransformIndex.GAFCTI_A] = _Reader.ReadSingle();
			
			ctxMul[(int)GAFColorTransformIndex.GAFCTI_R] = _Reader.ReadSingle();
			ctxOff[(int)GAFColorTransformIndex.GAFCTI_R] = _Reader.ReadSingle();
			
			ctxMul[(int)GAFColorTransformIndex.GAFCTI_G] = _Reader.ReadSingle();
			ctxOff[(int)GAFColorTransformIndex.GAFCTI_G] = _Reader.ReadSingle();
			
			ctxMul[(int)GAFColorTransformIndex.GAFCTI_B] = _Reader.ReadSingle();
			ctxOff[(int)GAFColorTransformIndex.GAFCTI_B] = _Reader.ReadSingle();

			Color colorMult = new Color(
				  ctxMul[(int)GAFColorTransformIndex.GAFCTI_R]
				, ctxMul[(int)GAFColorTransformIndex.GAFCTI_G]
				, ctxMul[(int)GAFColorTransformIndex.GAFCTI_B]
				, ctxMul[(int)GAFColorTransformIndex.GAFCTI_A]);

			Color colorOff = new Color(
				  ctxOff[(int)GAFColorTransformIndex.GAFCTI_R]
				, ctxOff[(int)GAFColorTransformIndex.GAFCTI_G]
				, ctxOff[(int)GAFColorTransformIndex.GAFCTI_B]
				, ctxOff[(int)GAFColorTransformIndex.GAFCTI_A]);

			state.colorMatrix = new GAFColorTransformationMatrix(colorMult, colorOff);
		}
		
		if (hasEffect)
		{
			byte effectsCount = _Reader.ReadByte();
			for (byte e = 0; e < effectsCount; ++e)
			{
				// skip all
				GAFFilterType type = (GAFFilterType)_Reader.ReadUInt32();
				switch(type)
				{
				case GAFFilterType.GFT_DropShadow: 	_Reader.BaseStream.Position += sizeof(uint) + 5 * sizeof(float) + 2 * sizeof(byte); break;
				case GAFFilterType.GFT_Blur: 		_Reader.BaseStream.Position += 2 * sizeof(float); 									break;
				case GAFFilterType.GFT_Glow: 		_Reader.BaseStream.Position += sizeof(uint) + 3 * sizeof(float) + 2 * sizeof(byte); break;
				case GAFFilterType.GFT_ColorMatrix: _Reader.BaseStream.Position += 20 * sizeof(float); 									break;
				}
			}
		}
		
		if (hasMasks)
		{
			state.maskID = (int)_Reader.ReadUInt32();
		}
		
		return state;
	}
}
