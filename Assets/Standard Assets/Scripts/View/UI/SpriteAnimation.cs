using UnityEngine;
using System.Collections;

public class SpriteAnimation : object
{
	private static string ASSET_PATH = "Animations/";

	public Hashtable images;
	public ArrayList textures;
	public ArrayList frames;
	
	public double x;
	public double y;
	public double width;
	public double height;
	public string name;
	public bool loop;
	
	public SpriteAnimation(Hashtable p_json) 
	{
		images = new Hashtable();
		frames = new ArrayList();
		textures = new ArrayList();
	
		x = double.Parse(p_json["x"].ToString());
		y = double.Parse(p_json["y"].ToString());
		width = double.Parse(p_json["width"].ToString());
		height = double.Parse(p_json["height"].ToString());
		name = p_json["name"] as string;
		loop = bool.Parse(p_json["loop"].ToString());
		
		parseImages(p_json["images"] as ArrayList);
		parseFiles(p_json["files"] as ArrayList);
		parseFrames(p_json["frames"] as ArrayList);
	}		
	
	private void parseFrames(ArrayList p_json) 
	{
		Hashtable l_frames = new Hashtable();
		foreach (Hashtable l_frame in p_json)
		{
			int l_frameId = Caster.toInt(l_frame["frameId"]); 
			l_frames[l_frameId] = l_frame;
		}
		
		for (int i = 0; i < p_json.Count; ++i)
		{
			Hashtable l_frame = l_frames[i + 1] as Hashtable;
			int l_duration = Caster.toInt(l_frame["duration"]);
			int l_imageId = Caster.toInt(l_frame["imageId"]);
			for (int j = 0; j < l_duration; ++j)
			{
				frames.Add(l_imageId);				
			}
		}			
	}
	
	private void parseFiles(ArrayList p_json) 
	{
		for (int i = 0; i < p_json.Count; ++i)
		{
			textures.Add(null);
		}
	
		foreach (Hashtable l_file in p_json)
		{
			int l_fileId = Caster.toInt(l_file["fileId"]);
			l_fileId = l_fileId - 1;
			textures[l_fileId] = Resources.Load(ASSET_PATH + l_file["name"]);
		}
	}
	
	private void parseImages(ArrayList p_json) 
	{
		foreach (Hashtable l_image in p_json)
		{
			SpriteImage l_spriteImage  = new SpriteImage(l_image);
			images[l_spriteImage.imageId] = l_spriteImage;
		}			
	}
	
	public void dispose() 
	{	
		frames = null;
		images = null;	
		textures = null;	
	}

}