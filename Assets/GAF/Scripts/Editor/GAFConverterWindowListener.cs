/*
 * File:           GAFConverterWindowListener.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System.IO;

using GAF.Utils;
using GAFEditor.Converter.Window;

[InitializeOnLoad]
public static class GAFConverterWindowListener 
{
	static GAFConverterWindowListener()
	{
		GAFConverterWindowEventDispatcher.onCreateMovieClipEvent			+= onCreateMovieClip;
		GAFConverterWindowEventDispatcher.onCreatePrefabEvent				+= onCreatePrefab;
		GAFConverterWindowEventDispatcher.onCreatePrefabPlusInstanceEvent	+= onCreatePrefabPlusInstance;
	}

	private static void onCreateMovieClip(string _AssetPath)
	{
		var assetName	= Path.GetFileNameWithoutExtension(_AssetPath).Replace(" ", "_");
		var assetDir	= "Assets" + Path.GetDirectoryName(_AssetPath).Replace(Application.dataPath, "") + "/";

		var asset = AssetDatabase.LoadAssetAtPath(assetDir + assetName + ".asset", typeof(GAFAnimationAsset)) as GAFAnimationAsset;
		if (!System.Object.Equals(asset, null))
		{
			var movieClipObject = createMovieClip(asset);

			var selected = new List<Object>(Selection.gameObjects);
			selected.Add(movieClipObject);
			Selection.objects = selected.ToArray(); 
		}
		else
		{
			GAFUtils.Log("[GAF] Cannot find asset with path - " + _AssetPath);
		}
	}

	private static void onCreatePrefab(string _AssetPath)
	{
		var assetName = Path.GetFileNameWithoutExtension(_AssetPath).Replace(" ", "_");
		var assetDir = "Assets" + Path.GetDirectoryName(_AssetPath).Replace(Application.dataPath, "") + "/";

		var asset = AssetDatabase.LoadAssetAtPath(assetDir + assetName + ".asset", typeof(GAFAnimationAsset)) as GAFAnimationAsset;
		if (!System.Object.Equals(asset, null))
		{
			var selected = new List<Object>(Selection.gameObjects);

			var prefabPath = assetDir + assetName + ".prefab";
			var existingPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
			if (existingPrefab == null)
			{
				var movieClipObject = createMovieClip(asset);
				var prefab = PrefabUtility.CreateEmptyPrefab(assetDir + assetName + ".prefab");
				prefab = PrefabUtility.ReplacePrefab(movieClipObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
				GameObject.DestroyImmediate(movieClipObject);
				selected.Add(prefab);
			}
			else
			{
				selected.Add(existingPrefab);
			}

			Selection.objects = selected.ToArray();
		}
		else
		{
			GAFUtils.Log("[GAF] Cannot find asset with path - " + _AssetPath);
		}
	}

	private static void onCreatePrefabPlusInstance(string _AssetPath)
	{
		var assetName = Path.GetFileNameWithoutExtension(_AssetPath).Replace(" ", "_");
		var assetDir = "Assets" + Path.GetDirectoryName(_AssetPath).Replace(Application.dataPath, "") + "/";

		var asset = AssetDatabase.LoadAssetAtPath(assetDir + assetName + ".asset", typeof(GAFAnimationAsset)) as GAFAnimationAsset;
		if (!System.Object.Equals(asset, null))
		{
			var selected = new List<Object>(Selection.gameObjects);

			var prefabPath = assetDir + assetName + ".prefab";
			var existingPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
			if (existingPrefab == null)
			{
				var movieClipObject = createMovieClip(asset);
				var prefab = PrefabUtility.CreateEmptyPrefab(assetDir + assetName + ".prefab");
				prefab = PrefabUtility.ReplacePrefab(movieClipObject, prefab, ReplacePrefabOptions.ConnectToPrefab);

				selected.Add(movieClipObject);
				selected.Add(prefab);
			}
			else
			{
				var instance = PrefabUtility.InstantiatePrefab(existingPrefab) as GameObject;
				selected.Add(existingPrefab);
				selected.Add(instance);
			}

			Selection.objects = selected.ToArray();
		}
		else
		{
			GAFUtils.Log("[GAF] Cannot find asset with path - " + _AssetPath);
		}
	}

	private static GameObject createMovieClip(GAFAnimationAsset _Asset)
	{
		var movieClipObject = new GameObject(_Asset.name);
		var movieClip = movieClipObject.AddComponent<GAFMovieClip>();

		movieClip.settings.init(_Asset);
		movieClip.initialize(_Asset, 0);
		movieClip.reload();

		return movieClipObject;
	}
}
