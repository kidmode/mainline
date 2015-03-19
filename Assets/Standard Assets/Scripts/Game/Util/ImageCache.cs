using UnityEngine;
using System.Collections;
using System.IO;

public class ImageCache : Object
{
	public static string getCacheImage(string p_file, string p_contentID)
	{
		if (null == p_contentID)
			return null;
			
		string l_file = _composeFileName(p_file, p_contentID);
		return (File.Exists(l_file) ? "file:///" + l_file : null);
	}
	
	public static void saveCacheImage(string p_file, Texture2D p_image, string p_contentID)
	{
		if (null == p_contentID)
			return;
			
		if (null == p_image)
			return;
		
		if (false == Directory.Exists(IMAGECACHE_PATH))
		{
			Directory.CreateDirectory(IMAGECACHE_PATH);
#if UNITY_IPHONE
			iPhone.SetNoBackupFlag(IMAGECACHE_PATH);
#endif			
		}
	
		string l_file = _composeFileName(p_file, p_contentID);
		File.WriteAllBytes(l_file, p_image.EncodeToPNG());
#if UNITY_IPHONE
		iPhone.SetNoBackupFlag(l_file);
#endif			
	}
	
	public static void deleteCacheImage(string p_file, string p_contentID)
	{
		if (null == p_contentID)
			return;
			
		string l_file = _composeFileName(p_file, p_contentID);
		if (File.Exists(l_file))
			File.Delete(l_file);
	}
	
	private static string _composeFileName(string p_file, string p_contentID)
	{
		return IMAGECACHE_PATH + p_contentID;
	}
	
	private static string IMAGECACHE_PATH	= Application.persistentDataPath + "/ImageCache/";
}