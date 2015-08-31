using UnityEngine;
using System.Collections;

public class BrushTool : ToolState
{
	public override void enter( PaintAcitivityController p_paintController ) 
	{
		base.enter(p_paintController);



		m_size = 1;
		m_color = Color.blue;
		m_hardness = 50;
	}

	public override void update( PaintAcitivityController p_paintController, int p_time ) 
	{
		base.update(p_paintController, p_time);
	}

	public override void exit( PaintAcitivityController p_paintController ) 
	{
		base.exit(p_paintController);
	}

	public override void onMouseDown(PaintAcitivityController p_paintController)
	{
		m_hasMarkedUndoPoint = false;
		
		if (false == m_hasMarkedUndoPoint )
			//		    && m_mouseLastFrame != m_mouseEnd)
		{
			p_paintController.getTextureManager().pushUndoPoint();
			m_hasMarkedUndoPoint = true;
		}
		
		m_size = p_paintController.getBrushSize();
		m_color = p_paintController.getBrushColor();
		m_hardness = m_size * 5;
		
		//		Debug.Log("m_mouseStart m_mouseStart " + m_mouseStart);
		if(m_mouseStart != Vector2.zero)
			paintLine (m_mouseStart, m_mouseStart + new Vector2( 0.1f, 0.1f), m_size, m_color, m_hardness);
	}

	public override void onMouseDrag(PaintAcitivityController p_paintController)
	{
		if (false == m_hasMarkedUndoPoint
		    && m_mouseLastFrame != m_mouseEnd)
		{
			p_paintController.getTextureManager().pushUndoPoint();
			m_hasMarkedUndoPoint = true;
		}

		m_size = p_paintController.getBrushSize();
		m_color = p_paintController.getBrushColor();
		m_hardness = m_size * 5;

		paintLine (m_mouseLastFrame, m_mouseEnd, m_size, m_color, m_hardness);
	}

	private void paintLine (Vector2 p_start, Vector2 p_end, float p_radius, Color p_color, float p_hardness) 
	{
		int l_startX =  (int)Mathf.Clamp (Mathf.Min (p_start.x, p_end.x) - p_radius, 0, m_paintingTexture.width);
		int l_startY =  (int)Mathf.Clamp (Mathf.Min (p_start.y, p_end.y) - p_radius, 0, m_paintingTexture.height);
		int l_endX = (int)Mathf.Clamp (Mathf.Max (p_start.x, p_end.x) + p_radius, 0, m_paintingTexture.width);
		int l_endY = (int)Mathf.Clamp (Mathf.Max (p_start.y, p_end.y) + p_radius, 0, m_paintingTexture.height);

		int l_width = l_endX - l_startX;
		int l_height = l_endY - l_startY;

		float l_radiusPlusOne = p_radius + 1;
		float l_radiusPlusOneSquared = l_radiusPlusOne * l_radiusPlusOne;

		Vector2 l_start = new Vector2(l_startX, l_startY);
		Color[] l_pixels = m_paintingTexture.GetPixels(l_startX, l_startY, l_width, l_height);
		for (int y = 0; y < l_height; y++) 
		{
			for (int x = 0; x < l_width; x++) 
			{
				Vector2 l_point = new Vector2(x,y) + l_start;
				Vector2 l_center = l_point + new Vector2(0.5f,0.5f);
				float l_distance = (l_center - Mathfx.NearestPointStrict(p_start, p_end, l_center)).sqrMagnitude;
				if (l_distance > l_radiusPlusOneSquared) 
				{
					continue;
				}

				l_distance = Mathfx.GaussFalloff(Mathf.Sqrt(l_distance), p_radius) * p_hardness;
				Color l_color;
				if (l_distance > 0) 
				{
					l_color = Color.Lerp(l_pixels[y*l_width+x], p_color, l_distance);
				} 
				else 
				{
					l_color = l_pixels[y*l_width+x];
				}
				
				l_pixels[y*l_width+x] = l_color;
			}
		}

		m_paintingTexture.SetPixels (l_startX, l_startY, l_width, l_height, l_pixels);
		m_paintingTexture.Apply();
	}

	private float m_size;
	private float m_hardness;
	private Color m_color;

	private bool m_hasMarkedUndoPoint;
}
