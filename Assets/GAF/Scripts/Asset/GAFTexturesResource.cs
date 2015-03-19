/*
 * File:           GAFTexturesResource.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

using UnityEngine;
using GAF;
using GAF.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

[System.Serializable]
public class GAFTexturesResource : ScriptableObject
{
	#region Static

	private static readonly string m_sBatchableObjectShaderPath = "GAF/GAFObjectsGroup";
	private static readonly string m_sMaskedObjectShaderPath 	= "GAF/GAFMaskedObject";
	
	#endregion // Static

	#region Members

	private GAFAnimationAsset supressAsset() { return m_Asset; }

	[HideInInspector][SerializeField] private GAFAnimationAsset	m_Asset				= null;		
	[HideInInspector][SerializeField] private float 			m_Scale				= 1f;
	[HideInInspector][SerializeField] private float 			m_CSF				= 1f;
	[HideInInspector][SerializeField] private List<string> 		m_TexturesNames 	= new List<string>();
	[HideInInspector][SerializeField] private List<Texture2D> 	m_Textures 			= new List<Texture2D>();
	[HideInInspector][SerializeField] private List<Material> 	m_SharedMaterials 	= new List<Material>();

	#endregion // Members

	#region Interface

	public Texture2D getTexture(string _Name)
	{
#if UNITY_EDITOR
		GAFAssetUtils.checkAssets(ref m_Textures);
#endif // UNITY_EDITOR

		return m_Textures.Find (texture => texture.name == _Name);
	}

	public void dispose()
	{
		for (int i = m_Textures.Count - 1; i >= 0; --i)
		{
			Texture2D l_texture = m_Textures[i];
			Resources.UnloadAsset(l_texture);
		}
	}

	public Material getMaterial(GAFAnimationAsset _Asset, uint _ObjectID, string _TextureName)
	{
		if (!System.Object.Equals(_Asset,null))
		{
			Material material = null;
			if (_Asset.maskedObjects.Contains((int)_ObjectID))
			{
				material 			= new Material(Shader.Find(m_sMaskedObjectShaderPath));
				material.color 			= new Color(1f, 1f, 1f, 1f);
				material.mainTexture	= getTexture(_TextureName);
				return material;
			}
			else if (_Asset.batchableObjects.Contains((int)_ObjectID))
			{
#if UNITY_EDITOR
				if (!GAFAssetUtils.checkAssets(ref m_SharedMaterials) ||
				    m_SharedMaterials.Find(_material => _material.name == _TextureName) == null)
				{
					createSharedData();
					AssetDatabase.SaveAssets();
				}
#endif // UNITY_EDITOR

				material = m_SharedMaterials.Find(_material => _material.name == _TextureName);
				if (material != null)
				{
					material.shader = Shader.Find(m_sBatchableObjectShaderPath);
#if UNITY_EDITOR
					EditorUtility.SetDirty(material);
#endif // UNITY_EDITOR
				}

				return material;
			}
		}

		return null;
	}

	#endregion // Interface

	#region Properties

	public bool isReady
	{
		get
		{
			return missingTextures.Count == 0;
		}
	}

	public float scale
	{
		get
		{
			return m_Scale;
		}
	}

	public float csf
	{
		get
		{
			return m_CSF;
		}
	}

	public List<string> texturesNames
	{
		get
		{
			return m_TexturesNames;
		}
	}

	public List<string> missingTextures
	{
		get
		{
#if UNITY_EDITOR
			GAFAssetUtils.checkAssets(ref m_Textures);
#endif // UNITY_EDITOR

			List<string> missing = new List<string>();
			foreach(var name in m_TexturesNames)
			{
				if (m_Textures.Find(texture => texture.name == name) == null)
					missing.Add(name);
			}

			return missing;
		}
	}

	public List<Texture2D> textures
	{
		get
		{
#if UNITY_EDITOR
			GAFAssetUtils.checkAssets(ref m_Textures);
#endif // UNITY_EDITOR

			return m_Textures;
		}
	}

	public List<Material> materials
	{
		get
		{
#if UNITY_EDITOR
			GAFAssetUtils.checkAssets(ref m_SharedMaterials);
#endif // UNITY_EDITOR
			
			return m_SharedMaterials;
		}
	}

	#endregion // Properties

#if UNITY_EDITOR

	#region EditorInterface

	public void init(GAFAnimationAsset _Asset, List<string> _Names, float _Scale, float _CSF)
	{
		if (this != null &&
		    !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(this)))
		{
			string path = AssetDatabase.GetAssetPath (_Asset);
			if (!string.IsNullOrEmpty(path))
			{
				m_Asset				= _Asset;
				m_TexturesNames 	= _Names;
				m_Scale				= _Scale;
				m_CSF				= _CSF;

				processTextures();
			}
		}
	}

	#endregion // EditorInterface

	#region EditorImplementation

	private void processTextures()
	{
		if (isAvailable)
		{
			string dirName = Path.GetDirectoryName(assetPath);
			var textures = GAFAssetUtils.findAssetsAtPath<Texture2D>(dirName, "*.png");
			foreach(var texture in textures)
			{	
				if (!string.IsNullOrEmpty(m_TexturesNames.Find(name => name == texture.name)))
				{	
					string texturePath = AssetDatabase.GetAssetPath(texture);
					TextureImporter textureImporter = AssetImporter.GetAtPath( texturePath ) as TextureImporter;
					if (hasCorrectImportSettings(textureImporter))
					{
						addTexture(texture);
					}
					else if (!GAFPostprocessorHelper.instance.busyTextures.Contains(textureImporter.assetPath))
					{
						reimportTexture(textureImporter);
					}
				}
			}
			
			if (missingTextures.Count == 0)
			{
				createSharedData();
			}

			AssetDatabase.SaveAssets();
		}
	}

	private void reimportTexture(TextureImporter _Importer)
	{
		if (isAvailable)
		{
			GAFPostprocessorHelper.instance.busyTextures.Add(_Importer.assetPath);

			_Importer.textureType 			= TextureImporterType.Advanced;
			_Importer.npotScale 			= TextureImporterNPOTScale.None;
			_Importer.mipmapEnabled 		= false;
			_Importer.maxTextureSize 		= 4096;
			_Importer.alphaIsTransparency	= true;
			_Importer.isReadable			= true;

			if (m_Asset.hasMasks)
			{
				_Importer.textureFormat = TextureImporterFormat.ARGB32;
			}
			else
			{
#if UNITY_EDITOR_OSX
				_Importer.textureFormat = TextureImporterFormat.PVRTC_RGBA4;
#else
				_Importer.textureFormat = TextureImporterFormat.ARGB32;
#endif // UNITY_EDITOR_OSX
			}
			
			TextureImporterSettings st = new TextureImporterSettings();
			_Importer.ReadTextureSettings( st );
			st.wrapMode = TextureWrapMode.Clamp;
			_Importer.SetTextureSettings( st );

			AssetDatabase.ImportAsset( _Importer.assetPath, ImportAssetOptions.ForceUpdate );
		}
	}
	
	private bool hasCorrectImportSettings(TextureImporter _Importer)
	{
		if (isAvailable)
		{
			TextureImporterFormat correctFormat = TextureImporterFormat.ARGB32;
			if (!m_Asset.hasMasks)
			{
#if UNITY_EDITOR_OSX
				correctFormat = TextureImporterFormat.PVRTC_RGBA4;
#else
				correctFormat = TextureImporterFormat.ARGB32;
#endif // UNITY_EDITOR_OSX
			}

			return 	_Importer.textureType 			== TextureImporterType.Advanced &&
					_Importer.npotScale 			== TextureImporterNPOTScale.None &&
					_Importer.mipmapEnabled 		== false &&
					_Importer.isReadable 			== true &&
					_Importer.maxTextureSize 		== 4096 &&
					_Importer.alphaIsTransparency	== true &&
					_Importer.textureFormat 		== correctFormat;
		}
		else
		{
			return false;
		}
	}

	private bool containsTexture(Texture2D _Texture)
	{
		GAFAssetUtils.checkAssets(ref m_Textures);
		
		return m_Textures.Contains (_Texture) || m_Textures.Find(texture => texture.name == _Texture.name) != null;
	}
	
	private void addTexture(Texture2D _Texture)
	{
		if (isAvailable)
		{
			if (!string.IsNullOrEmpty(m_TexturesNames.Find(textureName => textureName == _Texture.name)) &&
			    !containsTexture(_Texture))
			{
				m_Textures.Add(_Texture);
				EditorUtility.SetDirty (this);
			}
		}
	}

	private void createSharedData()
	{
		if (isAvailable)
		{
			if (m_Asset.batchableObjects.Count > 0)
			{
				m_SharedMaterials.Clear();
				
				foreach(var texture in m_Textures)
				{
					Material material 		= new Material(Shader.Find(m_sBatchableObjectShaderPath));
					material.color 			= new Color(1f, 1f, 1f, 1f);
					material.mainTexture	= texture;

					string path = Path.GetDirectoryName(assetPath) + "/" + texture.name + ".mat";
					material = GAFAssetUtils.saveAsset(material, path);
					
					m_SharedMaterials.Add(material);
				}

				EditorUtility.SetDirty (this);
			}
		}
	}

	#endregion // EditorImplementation

	#region ScriptableObject

	private void OnEnable()
	{
		GAFPostprocessorHelper.instance.on_preprocess_texture += onPreprocessTexture;
	}

	private void OnDisable()
	{
		GAFPostprocessorHelper.instance.on_preprocess_texture -= onPreprocessTexture;
	}

	#endregion // ScriptableObject

	#region Callbacks
	
	private void onPreprocessTexture(TextureImporter _Importer)
	{
		if (isAvailable)
		{
			List<string> missing = missingTextures;
			if (missing.Count > 0)
			{
				string assetDir = Path.GetDirectoryName(assetPath) + "/";
				if (!string.IsNullOrEmpty(missingTextures.Find(name => assetDir + name + ".png" == _Importer.assetPath)))
				{
					if (hasCorrectImportSettings(_Importer))
					{
						GAFPostprocessorHelper.instance.busyTextures.Remove(_Importer.assetPath);

						Texture2D texture = AssetDatabase.LoadAssetAtPath(_Importer.assetPath, typeof(Texture2D)) as Texture2D;
						addTexture(texture);
						
						if (missingTextures.Count == 0)
						{
							createSharedData();
							GAFPostprocessorHelper.instance.onResourceBecomeReady(this);
						}

						AssetDatabase.SaveAssets();
					}
					else if (!GAFPostprocessorHelper.instance.busyTextures.Contains(_Importer.assetPath))
					{
						reimportTexture(_Importer);
					}
				}
			}
		}
	}

	#endregion // Callbacks

	#region Editor Properties
	
	private bool isAvailable
	{
		get
		{
			if (!System.Object.Equals(m_Asset, null))
			{
				string assetsPath   = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
				string resourcePath = AssetDatabase.GetAssetPath (this);
				
				return  !string.IsNullOrEmpty (resourcePath) &&
						File.Exists (assetsPath + assetPath) &&
						File.Exists (assetsPath + resourcePath);
			}
			else
			{
				return false;
			}
		}
	}
	
	private string assetPath
	{
		get
		{
			return !System.Object.Equals(m_Asset, null) ? AssetDatabase.GetAssetPath(m_Asset) : string.Empty;
		}
	}

	#endregion //  Editor Properties

#endif // UNITY_EDITOR
}
