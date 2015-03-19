/*
 * File:           GAFAnimationAssetEditor.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(GAFAnimationAsset))]
public class GAFAnimationAssetEditor : Editor
{
	new public GAFAnimationAsset target
	{
		get
		{
			return base.target as GAFAnimationAsset;
		}
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI ();

		GUILayout.Space(5f);
		EditorGUILayout.LabelField("Asset name: " + target.name, EditorStyles.boldLabel);
		if (target.isLoaded)
		{
			EditorGUILayout.BeginVertical(EditorStyles.textField);
			{
				GUILayout.Space(2f);
				EditorGUILayout.LabelField("GAF version: " + target.majorDataVersion.ToString() + "." + target.minorDataVersion.ToString());
				EditorGUILayout.LabelField("Asset version: " + target.assetVersion.ToString());

				GUILayout.Space(5f);
				EditorGUILayout.LabelField("Available atlas scales: " + string.Join(",", target.scales.ConvertAll(scale => scale.ToString()).ToArray()));
				EditorGUILayout.LabelField("Available content scale factors: " + string.Join(",", target.csfs.ConvertAll(csf => csf.ToString()).ToArray()));

				GUILayout.Space(5f);
				EditorGUILayout.LabelField("Batchable objects count: " + target.batchableObjects.Count.ToString());
				EditorGUILayout.LabelField("Masked objects count: " + target.maskedObjects.Count.ToString());
			}
			EditorGUILayout.EndVertical();

			GUILayout.Space(5f);
			EditorGUILayout.LabelField("Timelines: ");
			foreach (var timeline in target.timelines)
			{
				EditorGUILayout.BeginVertical(EditorStyles.textField);
				{
					GUILayout.Space(5f);
					EditorGUILayout.LabelField("ID - " + timeline.id.ToString());
					EditorGUILayout.LabelField("Linkage name - " + timeline.linkageName);
						
					GUILayout.Space(5f);
					EditorGUILayout.LabelField("Frame size: " + timeline.frameSize.ToString());
					EditorGUILayout.LabelField("Pivot: " + timeline.pivot.ToString());
					EditorGUILayout.LabelField("Frames count: " + timeline.framesCount.ToString());
					
					GUILayout.Space(5f);
					EditorGUILayout.LabelField("Available sequences: " + string.Join(",", timeline.sequences.ConvertAll(sequence => sequence.name).ToArray()));
					
					GUILayout.Space(5f);
					EditorGUILayout.LabelField("Objects count: " + timeline.objects.Count.ToString());
					EditorGUILayout.LabelField("Masks count: " + timeline.masks.Count.ToString());
				}
				EditorGUILayout.EndVertical();
			}

			GUILayout.Space(5f);
			if (GUILayout.Button("Add to scene"))
			{
				addToScene();
			}

			GUILayout.Space(5f);
			if (GUILayout.Button("Create prefab"))
			{
				createPrefab();
			}

			GUILayout.Space(5f);
			if (GUILayout.Button("Prefab+instance"))
			{
				createPrefabPlusInstance();
			}

			GUILayout.Space(5f);
			EditorGUILayout.LabelField("Resources: ");
			foreach (var resourcePath in target.resourcesPaths)
			{
				EditorGUILayout.BeginVertical(EditorStyles.textField);
				{
					EditorGUILayout.LabelField(resourcePath);
					var resource = AssetDatabase.LoadAssetAtPath(resourcePath, typeof (GAFTexturesResource)) as GAFTexturesResource;
					if (resource != null)
					{
						List<Texture2D> textures = resource.textures;
						List<Material> materials = resource.materials;
						List<string> missingTexturesNames = resource.missingTextures;

						if (textures.Count > 0)
						{
							GUILayout.Space(3f);
							EditorGUILayout.LabelField("Found textures: ");
							for(int index = 0; index < textures.Count; ++index)
							{
								string path = AssetDatabase.GetAssetPath(textures[index]);

								EditorGUILayout.LabelField("\t" + (index + 1).ToString() + ". " + path);
							}
						}

						if (missingTexturesNames.Count > 0)
						{
							GUILayout.Space(3f);
							EditorGUILayout.LabelField("Missing textures: ");
							for(int index = 0; index < missingTexturesNames.Count; ++index)
							{
								EditorGUILayout.LabelField("\t" + (index + 1).ToString() + ". " + missingTexturesNames[index]);
							}
						}

						if (materials.Count > 0)
						{
							GUILayout.Space(3f);
							EditorGUILayout.LabelField("Shared materials: ");
							for(int index = 0; index < materials.Count; ++index)
							{
								string path = AssetDatabase.GetAssetPath(materials[index]);

								EditorGUILayout.LabelField("\t" + (index + 1).ToString() + ". " + path);
							}
						}
					}
				}
				EditorGUILayout.EndVertical();
			}
		}
		else
		{
			GUILayout.Space(5f);
			EditorGUILayout.LabelField("Asset is not loaded! Please reload asset or reimport '.gaf' file.");
		}

		if (GUILayout.Button("Clean cache"))
		{
			target.cleanUp();
		}

		if (GUILayout.Button("Reload"))
		{
			target.reload();
		}
	}

	private void addToScene()
	{
		createMovieClip();
	}

	private void createPrefab()
	{
		var path = AssetDatabase.GetAssetPath(target);
		path = path.Substring(0, path.Length - name.Length - ".asset".Length);

		var prefabPath = path + name + ".prefab";
		var existingPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
		if (existingPrefab == null)
		{
			var movieClipObject = createMovieClip();
			var prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
			prefab = PrefabUtility.ReplacePrefab(movieClipObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
			GameObject.DestroyImmediate(movieClipObject);
		}
	}

	private void createPrefabPlusInstance()
	{
		var path = AssetDatabase.GetAssetPath(target);
		path = path.Substring(0, path.Length - name.Length - ".asset".Length);

		var prefabPath = path + name + ".prefab";
		var existingPrefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
		if (existingPrefab == null)
		{
			var movieClipObject = createMovieClip();
			var prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
			prefab = PrefabUtility.ReplacePrefab(movieClipObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
		}
		else
		{
			PrefabUtility.InstantiatePrefab(existingPrefab);
		}
	}

	private GameObject createMovieClip()
	{
		var movieClipObject = new GameObject(target.name);
		var movieClip = movieClipObject.AddComponent<GAFMovieClip>();

		movieClip.settings.init(target);
		movieClip.initialize(target, 0);
		movieClip.reload();

		return movieClipObject;
	}
}
