using UnityEngine;
using UnityEngine.UI;

public class ToolState : System.Object
{	
	private const int LEFT_MOUSE_BUTTON = 0;

	public virtual void enter(PaintAcitivityController p_paintController) 
	{
		m_mouseStart = Vector2.zero;
		m_mouseEnd = Vector2.zero;
		getTexture(p_paintController);
	}

	public virtual void update(PaintAcitivityController p_paintController, int p_time) 
	{
		calculatePaintingRect(p_paintController);

		Vector2 l_paintingPoint = Vector2.zero;
		if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON))
		{
			if (convertToPaintingCoordinate(Input.mousePosition, out l_paintingPoint))
			{
				m_mouseStart = l_paintingPoint;
				onMouseDown(p_paintController);
				m_mouseLastFrame = m_mouseStart;
			}
			else
			{
				m_mouseStart = Vector2.zero;
				m_mouseLastFrame = Vector2.zero;
			}
		}
		else if (Input.GetMouseButton(LEFT_MOUSE_BUTTON))
		{
			if (Vector2.zero != m_mouseStart)
			{
				convertToPaintingCoordinate(Input.mousePosition, out l_paintingPoint);
				m_mouseEnd = l_paintingPoint;
				onMouseDrag(p_paintController);
				m_mouseLastFrame = m_mouseEnd;
			}
		}
		else if (Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON))
		{
			if (Vector2.zero != m_mouseStart)
			{
				convertToPaintingCoordinate(Input.mousePosition, out l_paintingPoint);
				m_mouseEnd = l_paintingPoint;
				onMouseUp(p_paintController);
			}
			m_mouseLastFrame = Vector2.zero;
		}
	}

	public virtual void exit(PaintAcitivityController p_paintController) 
	{
		m_mouseStart = Vector2.zero;
		m_mouseEnd = Vector2.zero;
	}

	public virtual void onMouseDown(PaintAcitivityController p_paintController) { }
	public virtual void onMouseDrag(PaintAcitivityController p_paintController) { }
	public virtual void onMouseUp(PaintAcitivityController p_paintController) 	{ }

	/*
	 * 	Converts the point to painting area coordinates, with 0,0 being in the upperleft.
	 *  Returns false if the point is not on the canvas.
	 */
	private bool convertToPaintingCoordinate(Vector3 p_mousePoint, out Vector2 p_paintingPoint)
	{
		p_paintingPoint.x = p_mousePoint.x - m_paintingRect.x;
		p_paintingPoint.y = p_mousePoint.y - m_paintingRect.y;
		p_paintingPoint.x = ((p_paintingPoint.x / m_paintingRect.width) * m_paintingTexture.width);
		p_paintingPoint.y = ((p_paintingPoint.y / m_paintingRect.height) * m_paintingTexture.height);
		
		if (p_paintingPoint.x < 0
		    || p_paintingPoint.y < 0
		    || p_paintingPoint.x > m_paintingTexture.width
		    || p_paintingPoint.y > m_paintingTexture.height)
		{
			//Point was outside of texture, clamp to texture
			p_paintingPoint.x = Mathf.Clamp(p_paintingPoint.x, 0, m_paintingTexture.width);
			p_paintingPoint.y = Mathf.Clamp(p_paintingPoint.y, 0, m_paintingTexture.height);
			return false;
		}

		return true;
	}

	private void calculatePaintingRect(PaintAcitivityController p_paintController)
	{
		UIElement l_paintingArea = p_paintController.getPaintingArea();
		RectTransform l_paintingTransform = (RectTransform) l_paintingArea.transform;

		//Get corners for the painting rect tangle in absolute world space
		Vector3[] l_fourCorners = new Vector3[4];
		l_paintingTransform.GetWorldCorners(l_fourCorners);

		//Convert the world points to screen points
		GameObject l_cameraObject = GameObject.Find("CameraView");
		Camera l_camera = l_cameraObject.camera;
		for (int i = 0 ; i < 4 ; ++i)
		{
			Vector3 l_worldPoint = l_fourCorners[i];
			l_fourCorners[i] = l_camera.WorldToScreenPoint(l_worldPoint);
		}

		//Convert the corners into a rectangle
		float l_left = l_fourCorners[0].x;
		float l_right = l_fourCorners[0].x;
		float l_top = l_fourCorners[0].y;
		float l_bottom = l_fourCorners[0].y;
		for (int i = 0; i < 4; ++i)
		{
			Vector3 l_screenPoint = l_fourCorners[i];
			if (l_screenPoint.x < l_left) 	l_left = l_screenPoint.x;
			if (l_screenPoint.x > l_right) 	l_right = l_screenPoint.x;
			if (l_screenPoint.y < l_bottom) l_bottom = l_screenPoint.y;
			if (l_screenPoint.y > l_top) 	l_top = l_screenPoint.y;
		}
		m_paintingRect = new Rect(l_left, l_bottom, l_right - l_left, l_top - l_bottom);
	}

	private void getTexture(PaintAcitivityController p_paintController)
	{
		UIElement l_paintingArea = p_paintController.getPaintingArea();
		GameObject l_gameObject = l_paintingArea.gameObject;
		RawImage l_image = l_gameObject.GetComponent<RawImage>();
		m_paintingTexture = l_image.texture as Texture2D;
	}

	protected Vector2 m_mouseStart;
	protected Vector2 m_mouseLastFrame;
	protected Vector2 m_mouseEnd;
	protected Rect m_paintingRect;
	protected Texture2D m_paintingTexture;
}
