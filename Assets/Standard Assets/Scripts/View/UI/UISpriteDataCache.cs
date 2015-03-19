using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISpriteDataCache : object 
{
	//Data store
	private Dictionary<string, Hashtable> m_data;
	private Dictionary<string, SpriteAnimation> m_animations;
	
	public void init()
	{
		m_data = new Dictionary<string, Hashtable>();	
		m_animations = new Dictionary<string, SpriteAnimation>();
	}	
	
	/*
	 * Caches data by asset name -- ignores asset path for caching purposes.
	 */	
	public Hashtable getAsset(string p_asset, string p_assetPath)
	{
		Hashtable l_data;
		bool l_hasData = m_data.TryGetValue(p_asset, out l_data);
		
		if (false == l_hasData)
		{
			TextAsset l_json = Resources.Load(p_assetPath + p_asset) as TextAsset;
			ArrayList l_jsonObject = MiniJSON.MiniJSON.jsonDecode(l_json.text as string) as ArrayList;
			ArrayList l_arrayList = l_jsonObject[0] as ArrayList;		
			l_data = l_arrayList[0] as Hashtable;
			m_data[p_asset] = l_data;
			Resources.UnloadAsset(l_json);
		}
		
		return l_data;
	}	
	
	public SpriteAnimation getAnimation(string p_asset, string p_assetPath)
	{
		SpriteAnimation l_animation;
		bool l_hasData = m_animations.TryGetValue(p_asset, out l_animation);
		
		if (false == l_hasData)
		{
			l_animation = new SpriteAnimation(getAsset(p_asset, p_assetPath));
			m_animations[p_asset] = l_animation;
		}
		
		return l_animation;
	}
	
	//Releasing the data. To be called by release instance.
	public void dispose()
	{
		m_data = null;
	}

	
	
	//Singleton methods
	
	private static UISpriteDataCache s_instance = null;
	
	public static UISpriteDataCache instance()
	{
		if (null == s_instance)
		{
			s_instance = new UISpriteDataCache();
			s_instance.init();
		}
		return s_instance;
	}
	
	public static void releaseInstance()
	{
		if (null != s_instance)
		{
			s_instance.dispose();
			s_instance = null;
		}
	}
}
