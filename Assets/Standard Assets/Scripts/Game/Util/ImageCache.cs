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
		Debug.Log ("@@@@@@@@@@@ Application.persistentDataPath = " + Application.persistentDataPath);
		string file = ImageCache.getCacheImagePath(p_file);
		if (file == null)
			return null;

		byte[] bytes = File.ReadAllBytes(file); 

		string[] splitArray = p_file.Split('.');
		string fileExt = splitArray[splitArray.Length-1];
		TextureFormat texFormat = TextureFormat.ARGB32;
		if (fileExt.Equals("jpg"))
			texFormat = TextureFormat.RGB24;

		Texture2D texture = new Texture2D(1, 1, texFormat, false);
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

		string[] splitArray = p_file.Split('.');
		string fileExt = splitArray[splitArray.Length-1];

		byte[] bytes;
		if (fileExt.Equals("png"))
			bytes = p_image.EncodeToPNG();
		else //if (fileExt.Equals("jpg"))
			bytes = p_image.EncodeToJPG(100);

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