using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorPicker : System.Object
{
	private const int LEFT_MOUSE_BUTTON = 0;
	private const float REFERENCE_PICKER_WIDTH = 130;

	public ColorPicker() {}

	public void setVerticalSlider(UISlider p_slider)
	{
		m_verticalSlider = p_slider;
	}

	public void setColorCircle(UIImage p_colorCircle)
	{
		m_colorCircle = p_colorCircle;
		m_colorPicker = (UIImage)m_colorCircle.getView("Picker");
		m_isInitialized = false;
		m_color = new HSBColor(Color.blue);
	}

	public Color getCurrentColor()
	{
		return m_color.ToColor();
	}

	public void update()
	{
		if (null == m_colorCircle) 
		{
			return;
		}

		calculateRect();

		if (false == m_isInitialized)
		{
			calculatePickerColor();
		}
		
		Vector2 l_paintingPoint = Vector2.zero;
		if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON))
		{
			if (convertToPaintingCoordinate(Input.mousePosition, out l_paintingPoint))
			{
				m_mouseStart = l_paintingPoint;
				onMouseDown();
				m_mouseLastFrame = m_mouseStart;
			}
			else
			{
				m_mouseStart = Vector2.zero;
			}
		}
		else if (Input.GetMouseButton(LEFT_MOUSE_BUTTON))
		{
			if (Vector2.zero != m_mouseStart)
			{
				convertToPaintingCoordinate(Input.mousePosition, out l_paintingPoint);
				m_mouseEnd = l_paintingPoint;
				onMouseDrag();
				m_mouseLastFrame = m_mouseEnd;
			}
		}
		else if (Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON))
		{
			if (Vector2.zero != m_mouseStart)
			{
				convertToPaintingCoordinate(Input.mousePosition, out l_paintingPoint);
				m_mouseEnd = l_paintingPoint;
				onMouseUp();
			}
		}

		if (null != m_verticalSlider)
		{
			m_color.b = 1;
			Image l_background = m_verticalSlider.gameObject.GetComponent<Image>();
			l_background.color = getCurrentColor();
			m_color.b = m_verticalSlider.value;	

			Color l_pickerColor = Color.white * m_color.b;
			l_pickerColor.a = 1;
			m_colorCircle.color = l_pickerColor;
		}
	}

	public void onMouseDown()
	{
		updateLocation();
	}

	public void onMouseDrag()
	{
		updateLocation();
	}

	public void onMouseUp()
	{
		updateLocation();
	}

	private void calculatePickerColor()
	{

		RectTransform l_rectTransform = (RectTransform) m_colorPicker.transform;
		Vector3 l_pickerPosition = l_rectTransform.localPosition;
		Vector2 l_inputPosition = new Vector2(l_pickerPosition.x, l_pickerPosition.y);
		
		float l_screenFactor = m_colorRect.width / REFERENCE_PICKER_WIDTH;
		float l_screenToTextureX = (m_circleTexture.width / m_colorRect.width) * l_screenFactor;
		float l_screenToTextureY = (m_circleTexture.height / m_colorRect.height) * l_screenFactor;
		Vector2 l_scaleVector = new Vector2(l_screenToTextureX, l_screenToTextureY);
		l_inputPosition.Scale(l_scaleVector);

		l_inputPosition.x = l_inputPosition.x + (m_circleTexture.width * 0.5f);
		l_inputPosition.y = l_inputPosition.y + (m_circleTexture.height * 0.5f);

		m_mouseEnd = l_inputPosition;
		updateLocation();
		m_isInitialized = true;
	}

	private void updateLocation()
	{
		Vector2 l_center = new Vector2(0f, 0f);
		l_center.x = m_circleTexture.width * 0.5f;
		l_center.y = m_circleTexture.height * 0.5f;

		Vector2 l_inputVector = new Vector2(0f, 0f);
		l_inputVector.x = m_mouseEnd.x - l_center.x;
		l_inputVector.y = m_mouseEnd.y - l_center.y;

		float l_hyp = Mathf.Sqrt((l_inputVector.x * l_inputVector.x) + (l_inputVector.y * l_inputVector.y));
		if (l_hyp <= l_center.x)
		{
			l_hyp = Mathf.Clamp(l_hyp, 0, l_center.x);
			float l_a = Vector3.Angle(new Vector3(1, 0, 0), l_inputVector);

			if (l_inputVector.y < 0)
			{
				l_a = 360 - l_a;
			}

			m_color.h = l_a / 360;
			m_color.s = l_hyp / l_center.x;

			movePicker(l_inputVector);
		}
	}

	private void movePicker(Vector2 p_inputVector)
	{
		//Convert from texture space to screen space
		float l_screenFactor = REFERENCE_PICKER_WIDTH / m_colorRect.width;
		float l_textureToScreenSpaceX = (m_colorRect.width / m_circleTexture.width) * l_screenFactor;
		float l_textureToScreenSpaceY = (m_colorRect.height / m_circleTexture.height) * l_screenFactor;
		Vector2 l_scaleVector = new Vector2(l_textureToScreenSpaceX, l_textureToScreenSpaceY);
		p_inputVector.Scale(l_scaleVector);

		//Apply position to picker
		RectTransform l_transform = (RectTransform) m_colorPicker.transform;
		l_transform.localPosition = new Vector3(p_inputVector.x, p_inputVector.y, 0);
	}

	private void calculateRect()
	{
		GameObject l_circleGameObject = m_colorCircle.gameObject;
		Image l_circleImage = l_circleGameObject.GetComponent<Image>();
		m_circleTexture = l_circleImage.sprite.texture;
		RectTransform l_transform = l_circleImage.rectTransform;
		
		//Get corners for the painting rect tangle in absolute world space
		Vector3[] l_fourCorners = new Vector3[4];
		l_transform.GetWorldCorners(l_fourCorners);
		
		//Convert the world points to screen points
		GameObject l_cameraObject = GameObject.Find("CameraView");
		Camera l_camera = l_cameraObject.GetComponent<Camera>();
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
		
		m_colorRect = new Rect(l_left, l_bottom, l_right - l_left, l_top - l_bottom);
		m_center = new Vector2(m_colorRect.x + (m_colorRect.width * 0.5f), m_colorRect.y + (m_colorRect.height * 0.5f));
	}

	private bool convertToPaintingCoordinate(Vector3 p_mousePoint, out Vector2 p_paintingPoint)
	{
		p_paintingPoint.x = p_mousePoint.x - m_colorRect.x;
		p_paintingPoint.y = p_mousePoint.y - m_colorRect.y;
		p_paintingPoint.x = ((p_paintingPoint.x / m_colorRect.width) * m_circleTexture.width);
		p_paintingPoint.y = ((p_paintingPoint.y / m_colorRect.height) * m_circleTexture.height);
		
		if (p_paintingPoint.x < 0
		    || p_paintingPoint.y < 0
		    || p_paintingPoint.x > m_circleTexture.width
		    || p_paintingPoint.y > m_circleTexture.height)
		{
			//Point was outside of texture, clamp to texture
			p_paintingPoint.x = Mathf.Clamp(p_paintingPoint.x, 0, m_circleTexture.width);
			p_paintingPoint.y = Mathf.Clamp(p_paintingPoint.y, 0, m_circleTexture.height);
			return false;
		}
		
		return true;
	}

	protected Vector2 m_mouseStart;
	protected Vector2 m_mouseLastFrame;
	protected Vector2 m_mouseEnd;

	protected Rect m_colorRect;
	protected Vector2 m_center;
	protected Texture2D m_circleTexture;
	protected HSBColor m_color;

	protected UIImage m_colorCircle;
	protected UIImage m_colorPicker;
	protected UISlider m_verticalSlider;

	protected bool m_isInitialized;
}
