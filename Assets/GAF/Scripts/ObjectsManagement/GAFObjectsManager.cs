using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GAF
{
	[System.Serializable]
	[AddComponentMenu("")]
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	#if UNITY_4_5
	[DisallowMultipleComponent]
	#endif
	public class GAFObjectsManager : MonoBehaviour
	{
		#region Events

		internal event System.Action onWillRenderObject;

		#endregion // Events

		#region Members

		[HideInInspector][SerializeField] private GAFMovieClip						m_MovieClip		= null;
		[HideInInspector][SerializeField] private List<GAFBakedObject>				m_BakedObjects	= new List<GAFBakedObject>();
		[HideInInspector][SerializeField] private List<GAFObject>					m_Objects		= new List<GAFObject>();
		[HideInInspector][SerializeField] private List<GAFBakedObjectController>	m_Controllers	= new List<GAFBakedObjectController>();
		[HideInInspector][SerializeField] private MeshFilter						m_Filter		= null;
		[HideInInspector][SerializeField] private bool								m_OldMode		= false;

		private Dictionary<int, IGAFObject>	m_AllObjects		= new Dictionary<int, IGAFObject>();
		private GAFRenderProcessor			m_RenderProcessor	= new GAFRenderProcessor();
		 
		#endregion // Members

		#region Properties

		public IEnumerable<IGAFObject> objects
		{
			get
			{
				return oldMode ? m_Objects.Cast<IGAFObject>() : m_BakedObjects.Cast<IGAFObject>();
			}
		}

		public GAFMovieClip movieClip 
		{
			get
			{
				return m_MovieClip;
			}
		}

		public bool oldMode
		{
			get
			{
				return m_OldMode;
			}
		}

		public Dictionary<int, IGAFObject> objectsDict
		{
			get
			{
				if (m_AllObjects == null || m_AllObjects.Count == 0)
				{
					m_AllObjects = new Dictionary<int, IGAFObject>();
					foreach (var _object in m_Objects)
					{
						m_AllObjects.Add(_object.getID(), _object);
					}

					foreach (var _object in m_BakedObjects)
					{
						m_AllObjects.Add(_object.getID(), _object);
					}
				}

				return m_AllObjects;
			}
		}

		#endregion // Properties

		#region Interface

		public void initialize(GAFMovieClip _Player)
		{
			m_MovieClip = _Player;

			m_Filter = GetComponent<MeshFilter>();

			createNewModeObjects();
		}

		public void regroupInOldMode()
		{
			if (!m_OldMode)
			{
				m_RenderProcessor.objectsToRender.Clear();
				m_AllObjects = null;

				for (int i = 0; i < m_BakedObjects.Count; i++)
				{
					if (hasController(m_BakedObjects[i]))
						removeController(m_BakedObjects[i]);
				}

				m_BakedObjects.Clear();

				createOldModeObjects();

				m_OldMode = true;
			}
		}

		public void regroupInNewMode()
		{
			if (m_OldMode)
			{
				m_RenderProcessor.objectsToRender.Clear();
				m_AllObjects = null;

				for (int i = 0; i < m_Objects.Count; i++)
				{
					if (!Application.isPlaying)
					{
						DestroyImmediate(m_Objects[i].gameObject);
						DestroyImmediate(m_Objects[i]);
					}
					else
					{
						Destroy(m_Objects[i].gameObject);
						Destroy(m_Objects[i]);
					}
				}

				m_Objects.Clear();

				createNewModeObjects();

				m_OldMode = false;
			}
		}

		public void reload()
		{
			foreach (var obj in objectsDict.Values)
			{
				obj.reload(m_RenderProcessor);
			}

			m_RenderProcessor.objectsToRender.Clear();

			Mesh mesh = new Mesh();
			mesh.name = name;

			if (m_Filter.sharedMesh != null)
			{
				GameObject.Destroy(m_Filter.sharedMesh);
			}

			m_Filter.sharedMesh = mesh;
		}

		public void addController(int _ID)
		{
			if (!oldMode)
			{
				var bakedObject = m_BakedObjects.Find(obj => obj.getID() == _ID);
				if (bakedObject != null)
				{
					addController(bakedObject);
				}
			}
		}

		public void removeController(int _ID)
		{
			if (!oldMode)
			{
				var bakedObject = m_BakedObjects.Find(obj => obj.getID() == _ID);
				if (bakedObject != null)
				{
					removeController(bakedObject);
				}
			}
		}

		public bool hasController(IGAFObject _Object)
		{
			return oldMode ? false : m_Controllers.Find((controller) => controller.id == _Object.getID()) != null;
		}

		public void clear(bool _DestroyChildren)
		{
			m_OldMode = false;
			for (int i = 0; i < m_Controllers.Count; i++)
			{
				if (Application.isEditor)
					DestroyImmediate(m_Controllers[i]);
				else
					Destroy(m_Controllers[i]);
			}

			if (_DestroyChildren)
			{
				List<GameObject> children = new List<GameObject>();
				foreach (Transform child in transform)
					children.Add(child.gameObject);

				children.ForEach((GameObject child) =>
				{
					if (Application.isPlaying)
						Destroy(child);
					else
						DestroyImmediate(child, true);
				});
			}
			else
			{
				foreach (var obj in m_Objects)
				{
					if (Application.isPlaying)
						Destroy(obj);
					else
						DestroyImmediate(obj, true);
				}
			}

			if (m_AllObjects != null)
			{
				m_AllObjects.Clear();
				m_AllObjects = null;
			}

			m_Controllers.Clear();
			m_Objects.Clear();
			m_BakedObjects.Clear();

			if (m_Filter != null)
			{
				if (m_Filter.sharedMesh != null)
				{
					GameObject.Destroy(m_Filter.sharedMesh);
				}
				GameObject.Destroy(m_Filter);
			}
		}

		public void updateToFrame(List<GAFObjectStateData> _States, bool _Refresh)
		{
			foreach (var state in _States)
			{
				if (objectsDict.ContainsKey((int)state.id))
				{
					objectsDict[(int)state.id].updateToState(state, m_RenderProcessor, _Refresh);
				}
			}

			if (_Refresh)
				m_RenderProcessor.state |= GAFRenderProcessor.MeshState.VertexSet;

			m_RenderProcessor.process(m_MovieClip, m_Filter, renderer);
		}

		#endregion // Interface

		#region MonoBehaviour

		private void OnWillRenderObject()
		{
			if (onWillRenderObject != null)
				onWillRenderObject();
		}

		#endregion // MonoBehaviour

		#region Implementation
		private void createNewModeObjects()
		{
			m_BakedObjects = new List<GAFBakedObject>();

			var objects = movieClip.asset.getObjects(movieClip.timelineID);
			var masks = movieClip.asset.getMasks(movieClip.timelineID);

			for (int i = 0; i < objects.Count; ++i)
			{
				var _object = objects[i];

				var name = getName(_object);

				if (movieClip.asset.maskedObjects.Contains((int)_object.id))
				{
					m_BakedObjects.Add(createBakedObject<GAFBakedObject>(ObjectType.Masked, _object, name));
				}
				else
				{
					m_BakedObjects.Add(createBakedObject<GAFBakedObject>(ObjectType.Simple, _object, name));
				}
			}

			if (masks != null)
			{
				for (int i = 0; i < masks.Count; i++)
				{
					var maskData = masks[i];

					m_BakedObjects.Add(createBakedObject<GAFBakedObject>(ObjectType.Mask, maskData, getName(maskData) + "_mask"));
				}
			}
		}

		private string getName(GAFObjectData _Object)
		{
			var namedParts = movieClip.asset.getNamedParts(movieClip.timelineID);
			var part = namedParts.Find((partData) => partData.objectID == _Object.id);

			return part == null ? _Object.atlasElementID.ToString() + "_" + _Object.id.ToString() : part.name;
		}

		private void createOldModeObjects()
		{
			var objects = movieClip.asset.getObjects(movieClip.timelineID);
			var masks	= movieClip.asset.getMasks(movieClip.timelineID);
			for (int i = 0; i < objects.Count; ++i)
			{
				var _object = objects[i];

				var name = getName(_object);

				if (movieClip.asset.maskedObjects.Contains((int)_object.id))
				{
					m_Objects.Add(createOldModeObject<GAFMaskedObject>(ObjectType.Masked, _object, name));
				}
				else
				{
					m_Objects.Add(createOldModeObject<GAFObject>(ObjectType.Simple, _object, name));
				}
			}

			if (masks != null)
			{
				for (int i = 0; i < masks.Count; ++i)
				{
					var maskData	= masks[i];
					var name		= getName(maskData) + "_mask";

					m_Objects.Add(createOldModeObject<GAFMaskObject>(ObjectType.Mask, maskData, name));
				}
			}
		}

		private T createBakedObject<T>(ObjectType _Type, GAFObjectData _Data, string _Name) where T : GAFBakedObject, new()
		{
			T bakedObject = new T();
			bakedObject.initialize(_Type, movieClip, this, _Name, (int)_Data.id, (int)_Data.atlasElementID);

			return bakedObject;
		}

		private T createOldModeObject<T>(ObjectType _Type, GAFObjectData _Data, string _Name) where T : GAFObject
		{
			var gameObj = new GameObject { name = _Name };
			gameObj.transform.parent			= this.transform;
			gameObj.transform.localScale		= Vector3.one;
			gameObj.transform.localPosition	= Vector3.zero;

			var component = gameObj.AddComponent<T>();
			component.initialize(_Type, movieClip, this, _Name, (int)_Data.id, (int)_Data.atlasElementID);

			return component;
		}

		private void addController(GAFBakedObject _BakedObject)
		{
			var gameObj = new GameObject { name = _BakedObject.getName() };
			gameObj.transform.parent			= this.transform;
			gameObj.transform.localScale		= Vector3.one;
			gameObj.transform.localPosition	= _BakedObject.getLocalPosition();

			var component = gameObj.AddComponent<GAFBakedObjectController>();
			_BakedObject.setController(component);
			m_Controllers.Add(component);
		}

		private void removeController(GAFBakedObject _BakedObject)
		{
			var _object = transform.FindChild(_BakedObject.getName());
			if (!Application.isPlaying)
				DestroyImmediate(_object.gameObject);
			else
				Destroy(_object.gameObject);

			_BakedObject.setController(null);

			m_Controllers.RemoveAll((contoller) => contoller.id == _BakedObject.getID());
		}

		#endregion // Implementation

	}
}