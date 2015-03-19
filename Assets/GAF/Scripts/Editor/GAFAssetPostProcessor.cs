/*
 * File:           GAFAssetPostProcessor.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEditor;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using GAF;
using GAF.Utils;
using GAFEditor.Tracking;

public class GAFAssetPostProcessor : AssetPostprocessor
{
	public void OnPreprocessTexture ()
	{
		GAFPostprocessorHelper.instance.onPreprocessTexture ((TextureImporter)assetImporter);
	}

	public void OnPostprocessTexture (Texture2D _Texture) 
	{
		GAFPostprocessorHelper.instance.onPostprocessTexture (assetPath);
	}

	public override uint GetVersion ()
	{
		return (uint)1;
	}

	public static void OnPostprocessAllAssets(
		  string[] importedAssets
		, string[] deletedAssets
		, string[] movedAssets
		, string[] movedFromAssetPaths)
    {
        foreach (string assetName in importedAssets)
        {
			if (assetName.EndsWith(".gaf"))
            {
				byte [] fileBytes = null;
				using (BinaryReader freader = new BinaryReader(File.OpenRead(assetName)))
				{
					fileBytes = freader.ReadBytes((int)freader.BaseStream.Length);
				}

				if (fileBytes.Length > sizeof(int))
				{
					int header = System.BitConverter.ToInt32(fileBytes.Take(4).ToArray(), 0);
					if (GAFHeader.isCorrectHeader((GAFHeader.CompressionType)header))
					{
						GAFAnimationAsset animationAsset = ScriptableObject.CreateInstance<GAFAnimationAsset>();
						animationAsset = GAFAssetUtils.saveAsset(animationAsset, Path.GetDirectoryName(assetName) + "/" + Path.GetFileNameWithoutExtension(assetName) + ".asset");
						animationAsset.init(fileBytes);

						GAFTracking.sendAssetCreatedRequest(assetName);
					}
				}
			}
        }
    }
}