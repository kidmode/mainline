using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;

public class ImageCache : Object
{
	public static Texture2D getCacheImage(string p_file)
	{
		if (null == p_file)
			return null;

		string file = ImageCache.getCacheImagePath(p_file);
		if (file == null)
			return null;

		byte[] bytes = File.ReadAllBytes(file); 
		Texture2D texture = new Texture2D(1, 1);
		texture.LoadImage(bytes);
		return texture;
	}

	public static string getCacheImagePath(string p_file)
	{
		if (null == p_file)
			return null;
			
		string l_file = _composeFileName(p_file);
		return (File.Exists(l_file) ? l_file : null);
	}
	
	public static void saveCacheImage(string p_file, Texture2D p_image)
	{
		if (null == p_file)
			return;
			
		if (null == p_image)
			return;
		
		if (false == Directory.Exists(IMAGECACHE_PATH))
		{
			Directory.CreateDirectory(IMAGECACHE_PATH);		
		}

		if (ImageCache.getCacheImagePath(p_file) != null)
		{
			ImageCache.deleteCacheImage(p_file);
		}
	
		string l_file = _composeFileName(p_file);
		byte[] bytes = p_image.EncodeToPNG();

		Thread thread = new Thread(() => saveImageThread(l_file, bytes));
		thread.Start();
	}

	private static void saveImageThread(string filePath, byte[] bytes)
	{
		File.WriteAllBytes(filePath, bytes);
		Debug.Log(filePath + " save image successfully.");
	}
	
	public static void deleteCacheImage(string p_file)
	{
		if (null == p_file)
			return;
			
		string l_file = _composeFileName(p_file);
		if (File.Exists(l_file))
			File.Delete(l_file);
	}
	
	private static string _composeFileName(string p_file)
	{
		return IMAGECACHE_PATH + p_file;
	}
	
	private static string IMAGECACHE_PATH	= Application.persistentDataPath + "/ImageCache/";
}