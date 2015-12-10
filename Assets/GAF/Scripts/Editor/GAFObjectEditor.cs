using UnityEditor;
using UnityEngine;
using System.Collections;

namespace GAF
{
	[CustomEditor(typeof(GAFObject))]
	public class GAFObjectEditor : Editor
	{
		new public GAFObject target
		{
			get
			{
				return (GAFObject)base.target;
			}
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUILayout.Space(3f);
			var offset = EditorGUILayout.Vector2Field("Position offset: ", target.getPositionOffset());
			if (offset != target.getPositionOffset())
			{
				target.setPositionOffset(offset);
				target.transform.localPosition = target.getLocalPosition();
			}

			GUILayout.Space(2f);
			var visible = EditorGUILayout.Toggle("Visible: ", target.getVisible());
			if (visible != target.getVisible())
			{
				target.setVisible(visible);
			}

			GUILayout.Space(3f);
			EditorGUILayout.LabelField("* Custom material will break dynamic batching!");
			var material = EditorGUILayout.ObjectField("Custom material: ", target.getCustomMaterial(), typeof(Material), false) as Material;
			if (material != target.getCustomMaterial())
			{
				target.setCustomMaterial(material);
			}
		}

		private void OnEnable()
		{
			EditorApplication.update += OnUpdate;
		}

		private void OnUpdate()
		{
			if (target != null)
			{
				if (target.transform.localPosition != target.getLocalPosition())
				{
					target.setPositionOffset((Vector2)(target.transform.localPosition - target.getStatePosition()));
				}
			}
			else
			{
				EditorApplication.update -= OnUpdate;
			}
		}
	}
}
