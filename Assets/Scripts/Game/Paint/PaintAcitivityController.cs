using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class PaintAcitivityController : System.Object 
{
	public PaintAcitivityController(GameController p_gameController, UIElement p_canvas, Texture2D p_texture)
	{
		m_gameController = p_gameController;
		m_canvas = p_canvas;
		m_paintTexture = p_texture;
		m_isFinished = false;
		setup();
	}

	public void setup()
	{
		m_toolButtons = new List<UIButton>();
		m_toolPanelMap = new Dictionary<UIElement, UIElement>();
		m_toolStateMap = new Dictionary<UIElement, ToolState>();

		//Add tools to maps
		addTool("Brush", new BrushTool());
//		addTool("Line", new LineTool());
		addTool("Eraser", new EraserTool());
//		addTool("Stamp", new StampTool());

		//Setup brush size Controller
		setupBrushSizeController();

		//Setup palette controller
		setupPaletteController();

		//Setup Painting Area
		setupPaintingArea();

		//start texture manager
		m_textureManager = new TextureManager(m_gameController, m_paintTexture);
		//m_textureManager.reset();

		//Add button logic to system buttons
		addSystemButtons();

		//Set first tool as current state
		changeToolState(m_toolButtons[0]);	
	}

	public void update(GameController p_gameController)
	{
		m_toolState.update(this, 0);
		m_textureManager.update();

		if(m_isFinished)
		{

			dispose();


			if (PointSystemController.Instance.pointSystemState () == PointSystemController.PointRewardState.OK) {
				
				p_gameController.connectState(ZoodleState.CONGRATS_STATE, ZoodleState.REGION_FUN);
				p_gameController.changeState(ZoodleState.CONGRATS_STATE);

			} else {

//				int l_nextState = p_gameController.getConnectedState(ZoodleState.REGION_FUN);
				
				PointSystemController.Instance.stopPointSystemTimer();
				
				p_gameController.changeState( ZoodleState.REGION_FUN );
				
			}
		}
	}

	public void dispose()
	{
		m_toolPanelMap.Clear();
		m_toolPanelMap = null;

		m_textureManager.dispose();

		m_canvas = null;
		m_toolState = null;
		m_textureManager = null;
	}

	public UIElement getPaintingArea()
	{
		return m_paintingArea;
	}

	public BrushSizeController getBrushSizeController()
	{
		return m_brushSizeController;
	}

	public PaletteController getPaletteController()
	{
		return m_paletteController;
	}

	public float getBrushSize()
	{
		return m_brushSizeController.getCurrentSize();
	}

	public Color getBrushColor()
	{
		return  m_paletteController.getCurrentColor();
	}

	public TextureManager getTextureManager ()
	{
		return m_textureManager;
	}

	public UIElement getCanvas()
	{
		return m_canvas;
	}

	private void addTool(string p_toolName, ToolState p_state)
	{
		//Convert toolname into the name of the button and panel
		string l_toolButtonName = p_toolName + "Button";

		//Get panel and button
		UIButton l_button = (UIButton)m_canvas.getView(l_toolButtonName);

		//Add mapping from button to state
		m_toolStateMap[l_button] = p_state;

		//Add callback to swap states
		l_button.addClickCallback(changeToolState);

		//Add tool to list of buttons
		m_toolButtons.Add(l_button);
	}

	private void changeToolState (UIButton p_button)
	{
		ToolState l_nextState = m_toolStateMap[p_button];

		if (null != m_toolState)
		{
			m_toolState.exit(this);
		}

		if (m_activeTool != null)
		{
			m_activeTool.enabled = true;
		}


		m_toolState = l_nextState;
		m_activeTool = p_button;

		if (m_activeTool != null)
		{
			m_activeTool.enabled = false;
		}

		if (null != m_toolState)
		{
			m_toolState.enter(this);
		}
	}

	private void setupPaintingArea()
	{
		//Get paint area
		m_paintingArea = m_canvas.getView("PaintingArea");		

		//Get paint area size
//		RectTransform l_transform = (RectTransform)m_paintingArea.transform;
//		Rect l_rect = l_transform.rect;
//		int l_width = (int)l_rect.width;
//		int l_height= (int)l_rect.height; 

		//Create new texture
//		m_paintTexture = new Texture2D(l_width, l_height, TextureFormat.ARGB32, false, true);

		//assign texture to paint area
		GameObject l_gameObject = m_paintingArea.gameObject;
		RawImage l_rawImage = l_gameObject.GetComponent<RawImage>();
		l_rawImage.texture = m_paintTexture;
	}

	private void addSystemButtons()
	{
		addSystemButton("SaveAndExitButton", saveAndExit);
		addSystemButton("UndoButton", undo);
	}

	private void saveAndExit(UIButton p_button)
	{
		save(p_button);
		exit(p_button);
	}

	private void addSystemButton(string p_buttonName, ButtonClickCallback m_callback)
	{
		UIButton l_button = (UIButton) m_canvas.getView(p_buttonName);
		l_button.addClickCallback(m_callback);
	}

	private void save(UIButton p_button)
	{
		m_textureManager.save();
	}

	private void load(UIButton p_button)
	{
		m_textureManager.pushUndoPoint();
		m_textureManager.load();
	}

	private void reset(UIButton p_button)
	{
		m_textureManager.pushUndoPoint();
		m_textureManager.reset();
	}

	private void undo(UIButton p_button)
	{
		m_textureManager.undo();
	}

	private void exit(UIButton p_button)
	{
		m_isFinished = true;
	}

	private void setupBrushSizeController()
	{
		m_brushSizeController = new BrushSizeController();

		Dictionary<String, float> l_buttonData = new Dictionary<String, float>();
		l_buttonData["SmallSizeButton"] = 10.0f;
		l_buttonData["MediumSizeButton"] = 20.0f;
		l_buttonData["LargeSizeButton"] = 30.0f;

		String[] l_keys = new string[l_buttonData.Count];
		l_buttonData.Keys.CopyTo(l_keys, 0);
		int l_numKeys = l_keys.Length;

		for (int i = 0; i < l_numKeys; ++i)
		{
			string l_buttonName = l_keys[i];
			UIButton l_button = m_canvas.getView(l_buttonName) as UIButton;
			float l_size = l_buttonData[l_buttonName];
			m_brushSizeController.registerButton(l_button, l_size);
		}
	}

	private void setupPaletteController()
	{
		UIElement m_palette = m_canvas.getView("ColorPalette");
		m_paletteController = new PaletteController();
		m_paletteController.registerButtons(m_palette);
	}

	private GameController m_gameController;
	private BrushSizeController m_brushSizeController;
	private PaletteController m_paletteController;
	private UIButton m_activeTool;

	private UIElement m_canvas;
	private UIElement m_paintingArea;
	private ToolState m_toolState;
	private Texture2D m_paintTexture;
	private TextureManager m_textureManager;
	private List<UIButton> m_toolButtons;
	private bool m_isFinished;
	private Dictionary<UIElement, UIElement> m_toolPanelMap;
	private Dictionary<UIElement, ToolState> m_toolStateMap;
}
