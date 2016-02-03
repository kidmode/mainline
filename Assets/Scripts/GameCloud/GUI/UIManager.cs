using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public delegate void CreateScreenFinishedHandler(UICanvas canvas);

public class UIManager
{
	public class LayerContainer
	{
		public int layerNumber 	= 0;
		public int screenType 	= 0;
		public UICanvas root 	= null;
		public GameObject screenObj = null;
	}

	public const string MAIN_CANVAS_TAG = "MainCanvas";

	public UIManager( Game p_game )
	{
		m_game = p_game;
		m_screenMap = new SortedDictionary<int, LayerContainer>();
		m_activelayerIds = new List<int>();
		m_transitionList = new List<LayerContainer>();
		m_screenStack = new Stack<LayerContainer>();

		m_mainCamera = Camera.main;
	}

	public void setScreenFactory( IScreenFactory p_factory )
	{
		DebugUtils.Assert( p_factory != null );
		m_screenFactory = p_factory;
		m_screenDirectoryMap = m_screenFactory.getDirectoryMap();
		DebugUtils.Assert( m_screenDirectoryMap != null && m_screenDirectoryMap.Count > 0 );
	}

	public void createScreenAsync(int p_newScreen, CreateScreenFinishedHandler handler, bool hasTransition = false, int p_layerNumber = 0) {

		DebugUtils.Assert( m_screenDirectoryMap.ContainsKey( p_newScreen ) );
		DebugUtils.Assert(handler != null);

		string screenPath = m_screenDirectoryMap[p_newScreen];
		AsyncScreenLoader.instance.LoadScreen(screenPath, (object obj) => {
			if (obj == null) {
				_Debug.logError( string.Format( "Could not load canvas at: {0}", screenPath ) );
				handler(null);
				return;
			}

			GameObject l_screen = obj as GameObject;
			GameObject l_screenObj = GameObject.Instantiate(l_screen) as GameObject;

			handler(_setupCanvas(l_screenObj, p_newScreen, hasTransition, p_layerNumber));
		});
	}

	public UICanvas createScreen( int p_newScreen, bool hasTransition = false, int p_layerNumber = 0 )
	{

		if(p_newScreen == 55){

			int test = 1;

		}

		DebugUtils.Assert( m_screenDirectoryMap.ContainsKey( p_newScreen ) );
		GameObject l_screenObj = _createUIScreen( m_screenDirectoryMap[ p_newScreen ] );

		return _setupCanvas(l_screenObj, p_newScreen, hasTransition, p_layerNumber);
	}

	public void removeScreen( UICanvas p_canvas )
	{
		LayerContainer l_container = findContainer( p_canvas );
		
		if( l_container != null )
			removeContainer( l_container );
	}

	public void removeScreen( int p_screenType )
	{
		LayerContainer l_container = findContainer( p_screenType );

		if( l_container != null )
		{
			removeContainer( l_container );
		}
	}

	public void removeScreenImmediately(int p_screenType)
	{
		LayerContainer l_container = findContainer(p_screenType);
		
		if(l_container != null)
		{
			l_container.root.hasTransition = false;
			removeContainer(l_container);
		}
	}

	public void removeScreenImmediately(UICanvas p_canvas)
	{
		LayerContainer l_container = findContainer(p_canvas);
		
		if(l_container != null)
		{
			l_container.root.hasTransition = false;
			removeContainer(l_container);
		}
	}

	public void removeContainer( LayerContainer p_container )
	{
		if(p_container.root.hasTransition)
		{
			m_transitionList.Add(p_container);
			p_container.root.enteringTransition();
		}
		else
		{
			_destroyContainer( p_container );
		}
		m_screenMap[ p_container.layerNumber ] = null;//
		_destroyLayer( p_container.layerNumber );
		_popModelContainer( p_container );
	}

	//Move Layerz
	public void moveScreenToLayer( UICanvas p_canvas, int p_newLayer )
	{
		LayerContainer l_container = findContainer( p_canvas );

		clearLayer( p_newLayer );

		int l_oldLayer = l_container.layerNumber;

		l_container.layerNumber = p_newLayer;
		l_container.root.z 		= p_newLayer;
		l_container.root.sortingOrder = p_newLayer;

		m_screenMap[ p_newLayer ] = l_container;

		_destroyLayer( l_oldLayer );

		if (null != l_container)
		{
			m_activelayerIds.Add(p_newLayer);
			m_activelayerIds.Sort();
		}
	}
	//createCamara
	//getCamera
	//destroyCamera

	public void update()
	{
		//loop through all views transitions and destroy them
		//once they have finished 
		_updateTransitionList();
	
		//loop though all of the layers and update their screens
		_updateScreenMap();
	}


	public void dispose()
	{
		clearAllLayers();
		m_screenMap.Clear();
		m_screenMap = null;
		m_activelayerIds.Clear();
		m_activelayerIds = null;
	}



	public LayerContainer findContainer( UICanvas p_canvas )
	{
		foreach( KeyValuePair< int, LayerContainer > pair in m_screenMap )
		{
			if( pair.Value.root == p_canvas )
				return pair.Value;
		}
		
		return null;
	}

	public LayerContainer findContainer( int p_screenType )
	{
		foreach( KeyValuePair< int, LayerContainer > pair in m_screenMap )
		{
			if( pair.Value.screenType == p_screenType )
				return pair.Value;
		}

		return null;
	}

	public UICanvas findScreen( int p_screenType )
	{
		LayerContainer container = findContainer( p_screenType );
		return ( container != null ) ? container.root : null;
	}

	public UICanvas getScreenInLayer( int p_layerNumber )
	{
		UICanvas l_canvas = null;
		LayerContainer l_container = null;

		if( m_screenMap.TryGetValue( p_layerNumber, out l_container ) )
			l_canvas = l_container.root;

		return l_canvas;
	}

	public void clearLayer( int p_layerNumber )
	{
		if( isLayerEmpty( p_layerNumber ) ) 
			return;

		removeContainer( m_screenMap[ p_layerNumber ] );
	}

	public void clearAllLayers()
	{
		List<int> l_layers = new List<int>();

		foreach(KeyValuePair<int, LayerContainer> l_pair in m_screenMap)
		{
			l_layers.Add(l_pair.Key);
		}

		int l_numLayers = l_layers.Count;
		for (int i = 0; i < l_numLayers; ++i)
		{
			int l_layer = l_layers[i];
			clearLayer( l_layer );
		}

		l_layers.Clear();
		l_layers = null;
	}

	public bool isLayerEmpty( int p_layerNumber )
	{
		return !( 	m_screenMap.ContainsKey( p_layerNumber ) 
		        &&	m_screenMap[ p_layerNumber ] != null );
	}

	public void stopTransitions()
	{
		for( int i = m_transitionList.Count - 1; i >= 0;  --i )
			_cleanUpTransition( i );
	}

	public Game getGame()
	{
		return m_game;
	}

	public void changeScreen( int p_screenType, bool p_model )
	{
		LayerContainer l_container = findContainer( p_screenType );
		if( l_container == null)
			return;

		if( p_model )
			_pushModelContainer( l_container );
		else
			_popModelContainer( l_container );
	}

	public void changeScreen( UICanvas p_screen, bool p_model )
	{
		LayerContainer l_container = findContainer( p_screen );
		if( l_container == null)
			return;

		if( p_model )
			_pushModelContainer( l_container );
		else
			_popModelContainer( l_container );
	}

	public void setScreenEnable( int p_screenType, bool p_enable )
	{
		LayerContainer l_container = findContainer( p_screenType );
		if( l_container == null)
			return;

		_enableGraphicRaycaster (l_container, p_enable);
	}

	public void setScreenEnable( UICanvas p_screen, bool p_enable )
	{
		LayerContainer l_container = findContainer( p_screen );
		if( l_container == null)
			return;
		
		_enableGraphicRaycaster (l_container, p_enable);
	}

//--------------------- Private Implementation ----------------

	
	private Game m_game;
	private SortedDictionary< int, LayerContainer > m_screenMap;
	private Camera	m_mainCamera;
	
	private IScreenFactory m_screenFactory;
	private Dictionary< int, string > m_screenDirectoryMap;
	private List< LayerContainer > m_transitionList;
	private Stack< LayerContainer > m_screenStack;
	private List<int> m_activelayerIds;

	private void _cleanUpTransition( int index )
	{
		_destroyContainer( m_transitionList[ index ] );
		m_transitionList[ index ].root.exitingTransition( );
		m_transitionList.RemoveAt( index );
	}


	private void _updateTransitionList()
	{
		for( int i = m_transitionList.Count - 1; i >= 0;  --i )
		{
			m_transitionList[i].root.update();

			if( !m_transitionList[i].root.isTransitioning )
				_cleanUpTransition( i );
		}
	}

	private void _updateScreenMap()
	{
		int l_numActiveLayers = m_activelayerIds.Count;
		for (int i = 0; i < l_numActiveLayers; ++i)
		{
			int l_layerId = m_activelayerIds[i];
			LayerContainer l_layer = m_screenMap[l_layerId];
			UICanvas l_root = l_layer.root;
			l_root.update();
		}
	}


	private void _destroyLayer( int p_layerNumber )
	{
		m_screenMap.Remove( p_layerNumber );
		if (m_activelayerIds.Contains(p_layerNumber))
	    {
			m_activelayerIds.Remove(p_layerNumber);
		}
	}


	private void _addContainer( LayerContainer p_container )
	{
		DebugUtils.Assert( p_container != null );

		int l_layer = p_container.layerNumber;
		clearLayer( l_layer );

		m_screenMap[ l_layer ] = p_container;
		if (null != p_container)
		{
			m_activelayerIds.Add(l_layer);
			m_activelayerIds.Sort();
		}
	}

	private void _destroyContainer( LayerContainer p_container )
	{
		if( p_container.screenObj != null )
		{
			GameObject.Destroy( p_container.screenObj );
		}

		p_container.root.dispose( true );
		p_container = null;
	}

	private GameObject _createUIScreen( string screenPath )
	{
		GameObject l_screen = Resources.Load< GameObject >( screenPath );
		
		if( l_screen == null )
		{
			_Debug.logError( string.Format( "Could not load canvas at: {0}", screenPath ) );
			return null; 
		}
		
		GameObject l_screenObj = GameObject.Instantiate( l_screen ) as GameObject;

		return l_screenObj;
	}

	private UICanvas _setupCanvas(GameObject p_screenObj, int p_newScreen, bool hasTransition, int p_layerNumber) {
		
		Canvas[] l_canvasArray = p_screenObj.GetComponentsInChildren< Canvas >();
		if( l_canvasArray == null )
			return null;
		
		List<Canvas> l_canvasList = new List<Canvas>( l_canvasArray );
		
		Canvas l_mainCanvas;
		
		if( l_canvasList.Count == 1 )
			l_mainCanvas = l_canvasList[0];
		else
		{
			l_canvasList.ForEach( (a) => a.worldCamera = m_mainCamera );
			l_mainCanvas = l_canvasList.Find( (a) => a.tag == MAIN_CANVAS_TAG );
		}
		
		UICanvas l_root = _createCanvas( p_newScreen, p_layerNumber, l_mainCanvas.gameObject );
		l_root.hasTransition = hasTransition;
		
		LayerContainer l_container = new LayerContainer( );
		l_container.layerNumber = p_layerNumber;
		l_container.screenType	= p_newScreen;
		l_container.root		= l_root;
		l_container.screenObj	= p_screenObj;
		
		_addContainer( l_container );
		
		return l_container.root;
	}

	private UICanvas _createCanvas( int p_screenType, int p_layerNumber, GameObject p_gameObject )
	{
		UICanvas l_root = m_screenFactory.getScreen( p_screenType );
		DebugUtils.Assert( l_root != null );

		l_root.init( p_gameObject );

		l_root.sortingOrder = p_layerNumber;
		l_root.worldCamera	= m_mainCamera;
		l_root.z = p_layerNumber;

		return l_root;
	}

	private void _pushModelContainer( LayerContainer p_container )
	{
		if( m_screenStack.Contains( p_container ) )
			return;
		
		if (m_screenStack.Count > 0)
			_enableGraphicRaycaster( m_screenStack.Peek(), false );
		else
		{
			foreach( KeyValuePair< int, LayerContainer > pair in m_screenMap )
			{
				if( pair.Value != p_container )
					_enableGraphicRaycaster( pair.Value, false );
			}
		}
		m_screenStack.Push( p_container );
	}

	private void _popModelContainer( LayerContainer p_container )
	{
		if( m_screenStack.Count == 0 || m_screenStack.Peek() != p_container )
			return;

		m_screenStack.Pop();
		if( m_screenStack.Count > 0 )
			_enableGraphicRaycaster( m_screenStack.Peek(), true );
		else
		{
			foreach( KeyValuePair< int, LayerContainer > pair in m_screenMap )
			{
				_enableGraphicRaycaster( pair.Value, true );
			}
		}
	}

	private void _enableGraphicRaycaster( LayerContainer p_container, bool p_enabled )
	{
		GraphicRaycaster l_raycaster = p_container.root.graphicRaycaster;
		if (l_raycaster != null)
			l_raycaster.enabled = p_enabled;
	}
}
