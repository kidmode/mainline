/*
 * File:			GAFAssetUtils.cs
 * Version:			3.7.1
 * Last changed:	2014/7/25 16:04
 * Author:			Alexey_Nikitin
 * Copyright:		© Catalyst Apps
 * Project:			GAFEditor
 */

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace GAF.Utils
{
	public static class GAFAssetUtils
	{
		public static T saveAsset<T>(T _Asset, string _Path) where T : UnityEngine.Object
		{
			T oldAsset = AssetDatabase.LoadAssetAtPath(_Path, typeof(T)) as T;
			if (System.Object.Equals(oldAsset, null) && System.Object.Equals(_Asset, null))
			{
				return null;
			}
			else if (!System.Object.Equals(oldAsset, null) && System.Object.Equals(_Asset, null))
			{
				return oldAsset;
			}
			else if (!System.Object.Equals(oldAsset, null) && !System.Object.Equals(_Asset, null))
			{
				EditorUtility.CopySerialized(_Asset, oldAsset);
				EditorUtility.SetDirty(oldAsset);
				return oldAsset;
			}
			else
			{
				AssetDatabase.CreateAsset(_Asset, _Path);
				return _Asset;
			}
		}

		public static List<T> findAssetsAtPath<T>(string _Path, string _Pattern) where T : UnityEngine.Object
		{
			string dataPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
			List<T> result = new List<T>();
			string[] files = Directory.GetFiles(dataPath + _Path, _Pattern);
			foreach (string file in files)
			{
				string path = "Assets" + file.Remove(0, Application.dataPath.Length);
				T asset = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
				if (asset != null)
					result.Add(asset);
			}

			return result;
		}

		public static bool isAssetAvailable<T>(T _Asset) where T : UnityEngine.Object
		{
			string assetsPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
			if (!System.Object.Equals(_Asset, null))
			{
				var path = AssetDatabase.GetAssetPath(_Asset);
				var fullPath = assetsPath + path;
				return !string.IsNullOrEmpty(path) && System.IO.File.Exists(fullPath);
			}

			return false;
		}

		public static bool checkAssets<T>(ref List<T> _Assets) where T : UnityEngine.Object
		{
			bool correct = true;
			int i = 0;
			foreach (var asset in _Assets)
			{
				if (!System.Object.Equals(asset, null))
				{
					if (!isAssetAvailable(asset))
					{
						_Assets[i] = null;
						correct = false;
					}
				}
				++i;
			}

			_Assets.RemoveAll(asset => System.Object.Equals(asset, null));

			return correct;
		}
	}
}

#endif // UNITY_EDITOR

