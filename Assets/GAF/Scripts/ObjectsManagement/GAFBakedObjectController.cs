﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GAF
{
	[System.Serializable]
	[AddComponentMenu("")]
	#if UNITY_4_5
	[DisallowMultipleComponent]
	#endif
	[ExecuteInEditMode]
	public class GAFBakedObjectController : GAFBehaviour
	{
		#region Members

		[HideInInspector][SerializeField] private MeshFilter		m_Filter		= null;
		[HideInInspector][SerializeField] private MeshCollider		m_MeshCollider	= null;

		private int				m_ID			= -1;
		private GAFBakedObject	m_BakedObject	= null;

		#endregion // Members

		#region Properties

		public GAFBakedObject bakedObject
		{
			get
			{
				return m_BakedObject;
			}
		}

		public int id
		{
			get
			{
				return m_ID;
			}
		}

		#endregion // Properties

		#region Interface

		public void registerObject(GAFBakedObject _BakedObject)
		{
			m_ID			= _BakedObject.getID();
			m_BakedObject	= _BakedObject;
		}

		public void copyMesh()
		{
			if (m_Filter == null)
			{
				initMesh();

				if (m_MeshCollider == null)
				{
					m_MeshCollider = gameObject.AddComponent<MeshCollider>();
				}
			}
		}

		private void initMesh()
		{
			GAFAtlasElementData element = m_BakedObject.getAtlasElementData();
			GAFTexturesData info		= m_BakedObject.getTexturesData();

			var movieClip = m_BakedObject.getMovieClip();

			if (m_Filter == null)
			{
				m_Filter = gameObject.AddComponent<MeshFilter>();
			}

			float scale			= element.scale * movieClip.settings.pixelsPerUnit;
			float scaledPivotX	= element.pivotX / scale;
			float scaledPivotY	= element.pivotY / scale;
			float scaledWidth	= element.width / scale;
			float scaledHeight	= element.height / scale;

			Vector3[] vertices = new Vector3[4];
			vertices[0] = new Vector3(-scaledPivotX, scaledPivotY - scaledHeight, 0f);
			vertices[1] = new Vector3(-scaledPivotX, scaledPivotY, 0f);
			vertices[2] = new Vector3(-scaledPivotX + scaledWidth, scaledPivotY, 0f);
			vertices[3] = new Vector3(-scaledPivotX + scaledWidth, scaledPivotY - scaledHeight, 0f);

			Texture2D atlasTexture		= movieClip.resource.getTexture(System.IO.Path.GetFileNameWithoutExtension(info.getFileName(movieClip.settings.csf)));
			float scaledElementLeftX	= element.x * movieClip.settings.csf / atlasTexture.width;
			float scaledElementRightX	= (element.x + element.width) * movieClip.settings.csf / atlasTexture.width;
			float scaledElementTopY		= (atlasTexture.height - element.y * movieClip.settings.csf - element.height * movieClip.settings.csf) / atlasTexture.height;
			float scaledElementBottomY	= (atlasTexture.height - element.y * movieClip.settings.csf) / atlasTexture.height;

			Vector2[] uv = new Vector2[vertices.Length];
			uv[0] = new Vector2(scaledElementLeftX, scaledElementTopY);
			uv[1] = new Vector2(scaledElementLeftX, scaledElementBottomY);
			uv[2] = new Vector2(scaledElementRightX, scaledElementBottomY);
			uv[3] = new Vector2(scaledElementRightX, scaledElementTopY);

			Vector3[] normals = new Vector3[vertices.Length];
			normals[0] = new Vector3(0f, 0f, -1f);
			normals[1] = new Vector3(0f, 0f, -1f);
			normals[2] = new Vector3(0f, 0f, -1f);
			normals[3] = new Vector3(0f, 0f, -1f);

			int[] triangles = new int[6];
			triangles[0] = 2;
			triangles[1] = 0;
			triangles[2] = 1;
			triangles[3] = 3;
			triangles[4] = 0;
			triangles[5] = 2;

			Mesh mesh = new Mesh();
			mesh.name = "Element_" + m_BakedObject.getAtlasElementID();

			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.triangles = triangles;
			mesh.normals = normals;

			m_Filter.mesh = mesh;
		}

		#endregion // Interface

		#region MonoBehaviour

		private void Update()
		{
			if (cachedTransform.localPosition != bakedObject.getLocalPosition())
			{
				cachedTransform.localPosition = bakedObject.getLocalPosition();
			}
		}

		#endregion // MonoBehaviour
	}
}