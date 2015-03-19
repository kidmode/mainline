using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PaintActivityState : GameState 
{

	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);

		_setupScreen(p_gameController.getUI());

		Drawing l_drawing = SessionHandler.getInstance().currentDrawing;
		if (l_drawing == null)
		{
			UIElement l_area = m_activityCanvas.getView("PaintingArea");
			RectTransform l_transform = (RectTransform)l_area.transform;
			Texture2D l_texture = new Texture2D((int)l_transform.rect.width, (int)l_transform.rect.height, TextureFormat.ARGB32, false, true);
			Color[] l_pixels = l_texture.GetPixels();//Get all pixels
			int l_numPixels = l_pixels.Length;
			Color l_white = Color.white;
			for (int i = 0; i < l_numPixels; ++i)
			{
				l_pixels[i] = l_white;
			}
			l_texture.SetPixels(l_pixels);
			l_texture.Apply();
			_setupCanvas(p_gameController, l_texture);
		}
		else
		{
			RequestQueue l_queue = new RequestQueue();
			l_queue.add(new ImageRequest("texture", l_drawing.largeUrl, (WWW p_response)=>
			{
				_setupCanvas(p_gameController, p_response.texture);
			}));
			l_queue.request(RequestType.RUSH);
		}

		GAUtil.logScreen("PaintActivityScreen");
	}
	
	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		if (m_paintController != null)
			m_paintController.update(p_gameController);
	}
	
	public override void exit(GameController p_gameController)
	{
		base.exit(p_gameController);	

		m_uiManager.removeScreen(m_activityCanvas);
		m_activityCanvas = null;
		m_uiManager = null;
		m_paintController = null;
	}
	
	
	//---------------- Private Implementation ----------------------
	
	
	
	private void _setupScreen(UIManager p_uiManager)
	{
		m_uiManager = p_uiManager;		
		m_activityCanvas = p_uiManager.createScreen(UIScreen.PAINT_ACTIVITY, false, 10);
		m_activityCanvas.tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );
	}
	
	private void _setupCanvas(GameController p_gameController, Texture2D p_texture)
	{
		m_paintController = new PaintAcitivityController(p_gameController, m_activityCanvas, p_texture);
		
		UIElement l_toolPanel = m_activityCanvas.getView("ToolPanel");
		
		RectTransform l_rectTransform = (RectTransform)l_toolPanel.transform;
		List<Vector3> l_goals = new List<Vector3>();
		l_goals.Add(l_rectTransform.localPosition + new Vector3( -l_rectTransform.rect.width, 0, 0 ) );
		l_goals.Add(l_rectTransform.localPosition + new Vector3( -l_rectTransform.rect.width, 0, 0 ) );
		l_goals.Add(l_rectTransform.localPosition);
		l_toolPanel.tweener.addPositionTrack(l_goals, 1f, null, Tweener.Style.Standard, false);
		
		l_rectTransform.localPosition = new Vector3 (-l_rectTransform.rect.width, l_rectTransform.localPosition.y, l_rectTransform.localPosition.z);
	}

	//Private variables
	private UICanvas m_activityCanvas;
	private UIManager m_uiManager;
	private PaintAcitivityController m_paintController;
}
