using UnityEngine;
using System.Collections;

public class SpriteImage : object
{
	public int fileId;
	public int width;
	public int height;
	public int imageId;
	public int offsetX;
	public int offsetY;
	public double anchorX;
	public double anchorY;
	
	public SpriteImage(Hashtable p_imageData) 
	{
		fileId = Caster.toInt(p_imageData["fileId"]);
		width = Caster.toInt(p_imageData["width"]);
		height = Caster.toInt(p_imageData["height"]);
		imageId = Caster.toInt(p_imageData["imageId"]);
		offsetX = Caster.toInt(p_imageData["offsetX"]);
		offsetY = Caster.toInt(p_imageData["offsetY"]);
		anchorX = Caster.toDouble(p_imageData["anchorX"]);
		anchorY = Caster.toDouble(p_imageData["anchorY"]);	
	}
	
}