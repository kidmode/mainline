/*
 * File:           GAFSystem.cs
 * Version:        3.7.1
 * Last changed:   Date: 2014/06/26
 * Author:         Alexey Nikitin
 * Copyright:      © Catalyst Apps
 * Product:        GAF Animation Player
 */

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class GAFPostprocessorHelper
{	
	#region Members
	
	private static GAFPostprocessorHelper m_Instance = null;

	private List<string> m_BusyTextures = new List<string>();
	
	#endregion // Members
	
	#region Interface
	
	public void onPreprocessTexture(TextureImporter _Importer)
	{
		if (on_preprocess_texture != null)
		{
			on_preprocess_texture(_Importer);
		}
	}
	
	public void onPostprocessTexture(string _TextureAssetPath)
	{
		if (on_postprocess_texture != null)
		{
			Texture2D texture = AssetDatabase.LoadAssetAtPath(_TextureAssetPath, typeof(Texture2D)) as Texture2D;
			on_postprocess_texture(texture);
		}
	}
	
	public void onResourceBecomeReady(GAFTexturesResource _Resource)
	{
		if (on_resource_become_ready != null)
		{
			on_resource_become_ready(_Resource);
		}
	}
	
	#endregion // Interface
	
	#region Properties
	
	public static GAFPostprocessorHelper instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = new GAFPostprocessorHelper();
			}
			
			return m_Instance;
		}
	}


	public List<string> busyTextures
	{
		get { return m_BusyTextures; }
		set { m_BusyTextures = value; }
	}
	
	#endregion // Properties
	
	#region Events
	
	public delegate void GAFPreprocessTexture(TextureImporter _Importer); 
	public event GAFPreprocessTexture on_preprocess_texture;
	
	public delegate void GAFPostprocessTexture(Texture2D _Texture);
	public event GAFPostprocessTexture on_postprocess_texture;
	
	public delegate void GAFResourceReady(GAFTexturesResource _Resource);
	public event GAFResourceReady on_resource_become_ready;
	
	#endregion // Events
}

#endif // UNITY_EDITOR
