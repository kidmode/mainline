using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController 
{
	private GameState m_state;
	private string m_stateName;
	
	private Dictionary< int, GameState > m_states;
	private Dictionary< int, int > 	m_connectStates;
	
	private Game m_game;	
	private UIManager m_uiManager;
	private IGameStateFactory m_gameStateFactory;
	
	public GameController( Game p_game, IScreenFactory p_screenFactory, 
	                      IGameStateFactory p_gameStateFactory ) 
	{
		m_game = p_game;
		m_uiManager = new UIManager( p_game );
		m_uiManager.setScreenFactory( p_screenFactory );

		string l_host = ZoodlesConstants.getHost();
		Server.init( l_host );
		//Create states table
		m_states        = new Dictionary< int, GameState >();
		m_connectStates = new Dictionary< int, int >();
		
		m_gameStateFactory = p_gameStateFactory;
		m_gameStateFactory.addStates( this );
		changeState( m_gameStateFactory.initialState );
	}
	
	public void addState( int p_stateName, GameState p_gameState )
	{
		DebugUtils.Assert( m_states != null );
		
		if( m_states.ContainsKey( p_stateName ) )
			m_states[ p_stateName ].exit( this );
		
		m_states[ p_stateName ] = p_gameState;
	}
	
	public void update( int p_time ) 
	{
		if (null != m_state)
		{
			m_state.update( this, p_time );
		}
		
		if( m_uiManager != null )
			m_uiManager.update();
	}
	
	public void changeState( int p_stateType )
	{
		if (null != m_state)
			m_state.exit(this);

		m_state = m_states[ p_stateType ];
		m_stateName = p_stateType.ToString();

		if (null != m_state)
			m_state.enter(this);
	}
	
	public void changeState( int p_stateType, bool p_checkState )
	{
		int l_state = -1;
		if( p_checkState )
		{
			l_state = getConnectedState( p_stateType );
			if( l_state < 0 )
			{
				changeState( l_state );
				return;
			}
		}
		
		changeState( p_stateType );
	}
	
	public GameState state
	{
		get { return m_state; }
	}
	
	public string stateName
	{
		get { return m_stateName; }
	}
	
	public Game game
	{
		get { return m_game; }
	}
	
	public UIManager getUI()
	{
		return m_uiManager;
	}
	
	public void connectState( int p_stateName, int p_nextState )
	{
		m_connectStates[p_stateName] = p_nextState;
	}
	
	public int getConnectedState( int p_stateType )
	{
		if (!m_connectStates.ContainsKey (p_stateType))
			return -999;
		else
		{
			int l_state = m_connectStates[ p_stateType ];
			m_connectStates[ p_stateType ] = -1;
			return l_state;
		}
	}
	
	public bool handleMessage( int p_type, string p_string )
	{
		return m_state.handleMessage(this, p_type, p_string);
	}		
}