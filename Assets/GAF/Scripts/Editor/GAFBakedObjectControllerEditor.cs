using GAF;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GAFBakedObjectController))]
public class GAFBakedObjectControllerEditor : Editor
{
	#region Properties

	new public GAFBakedObjectController target
	{
		get
		{
			return base.target as GAFBakedObjectController;
		}
	}

	#endregion // Properties

	private void OnEnable()
	{
		EditorApplication.update += OnUpdate;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GUILayout.Space(3f);
		var offset = EditorGUILayout.Vector2Field("Position offset: ", target.bakedObject.getPositionOffset());
		if (offset != target.bakedObject.getPositionOffset())
		{
			target.bakedObject.setPositionOffset(offset);
			target.transform.localPosition = target.bakedObject.getLocalPosition();
		}

		GUILayout.Space(2f);
		var visible = EditorGUILayout.Toggle("Visible: ", target.bakedObject.getVisible());
		if (visible != target.bakedObject.getVisible())
		{
			target.bakedObject.setVisible(visible);
		}

		GUILayout.Space(3f);
		EditorGUILayout.LabelField("* Custom material will break dynamic batching!");
		var material = EditorGUILayout.ObjectField("Custom material: ", target.bakedObject.getCustomMaterial(), typeof(Material), false) as Material;
		if (material != target.bakedObject.getCustomMaterial())
		{
			target.bakedObject.setCustomMaterial(material);
		}

		GUILayout.Space(5f);
		if (GUILayout.Button(new GUIContent("Copy mesh")))
		{
			target.copyMesh();
		}
	}

	private void OnUpdate()
	{
		if (target != null)
		{
			if (target.transform.localPosition != target.bakedObject.getLocalPosition())
			{
				target.bakedObject.setPositionOffset((Vector2)(target.transform.localPosition - target.bakedObject.getStatePosition()));
			}
		}
		else
		{
			EditorApplication.update -= OnUpdate;
		}
	}
}

