using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace GAF
{
	public class GAFRenderProcessor
	{
		#region Enums

		[System.Flags]
		public enum MeshState
		{
			  Null			= 0
			, VertexChange	= 1
			, VertexSet		= 2
		}

		#endregion // Enums

		#region Members

		private static readonly Vector3 normalVector = new Vector3(0, 0, -1f);

		#endregion // Members

		#region Interface

		internal GAFRenderProcessor()
		{
			state = MeshState.Null;
			objectsToRender = new Dictionary<int, GAFBakedObject>();
		}

		internal void process(GAFMovieClip _Clip, MeshFilter _Filter, Renderer _Renderer)
		{			
			if (isStateSet(MeshState.VertexSet))
			{
				resetMesh(_Clip, _Filter, _Renderer);
			}
			else if (isStateSet(MeshState.VertexChange))
			{
				changeMesh(_Filter);
			}

			state = MeshState.Null;
		}

		#endregion // Interface

		#region Properties

		internal Dictionary<int, GAFBakedObject> objectsToRender
		{
			get;
			private set;
		}

		internal List<GAFBakedObject> sortedObjects
		{
			get;
			private set;
		}

		internal MeshState state
		{
			get;
			set;
		}

		#endregion // Properties

		#region Implementation

		private bool isStateSet(MeshState _State)
		{
			return ((state & _State) == _State);
		}

		private void resetMesh(GAFMovieClip _Clip, MeshFilter _Filter, Renderer _Renderer)
		{
			sortedObjects = objectsToRender.Values.ToList();
			sortedObjects.Sort();

			if(_Filter.sharedMesh == null)
				return;

			_Filter.sharedMesh.Clear();

			int capacity = sortedObjects.Count;

 			Vector3[]	vertices	= new Vector3[capacity * 4];
			Vector2[]	uvs			= new Vector2[capacity * 4];
			Color32[]	colors		= new Color32[capacity * 4];
			Vector4[]	tangents	= new Vector4[capacity * 4];
			List<int[]> triangles	= new List<int[]>();
			Material[]	materials	= new Material[capacity];
			Vector3[]	normals		= new Vector3[capacity * 4];

			for (int i = 0; i < normals.Length; i++)
			{
				normals[i] = normalVector;
			}

			_Filter.sharedMesh.subMeshCount = capacity;

			int index = 0;
			int materialIndex = 0;
			foreach (GAFBakedObject obj in sortedObjects)
			{
				obj.getCurrentVertices().CopyTo(vertices, index);
				obj.getUVs().CopyTo(uvs, index);
				obj.getColors().CopyTo(colors, index);
				obj.getColorsShift().CopyTo(tangents, index);

				materials[materialIndex] = obj.getCurrentMaterial();

				triangles.Add(new int[]
				{
 					  2 + index
					, 0 + index
					, 1 + index
					, 3 + index
					, 0 + index
					, 2 + index
				});

				++materialIndex;
				index += 4;
			}

			_Filter.sharedMesh.MarkDynamic();

			_Filter.sharedMesh.vertices = vertices;
			_Filter.sharedMesh.uv		= uvs;
			_Filter.sharedMesh.normals	= normals;
			_Filter.sharedMesh.colors32 = colors;
			_Filter.sharedMesh.tangents = tangents;

			for (int i = 0; i < triangles.Count; i++)
			{
				_Filter.sharedMesh.SetTriangles(triangles[i], i);
			}

			_Renderer.sharedMaterials	= materials;
			_Renderer.castShadows		= false;
			_Renderer.receiveShadows	= false;
			_Renderer.sortingLayerID	= _Clip.settings.spriteLayerID;
			_Renderer.sortingOrder		= _Clip.settings.spriteLayerValue;
		}

		private void changeMesh(MeshFilter _Filter)
		{
			int capacity = sortedObjects.Count;
 			Vector3[] vertices = new Vector3[capacity * 4];
			Color32[] colors = new Color32[capacity * 4];
			Vector4[] tangents = new Vector4[capacity * 4];

			int index = 0;
			foreach (GAFBakedObject obj in sortedObjects)
			{
				obj.getCurrentVertices().CopyTo(vertices, index);
				obj.getColors().CopyTo(colors, index);
				obj.getColorsShift().CopyTo(tangents, index);
				index += 4;
			}

			_Filter.sharedMesh.MarkDynamic();

			_Filter.sharedMesh.vertices = vertices;
			_Filter.sharedMesh.colors32 = colors;
			_Filter.sharedMesh.tangents = tangents;
			_Filter.sharedMesh.RecalculateBounds();
		}

		#endregion // Implementation
	}
}
