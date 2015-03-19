using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;



public delegate void ToggleChangedCallback(UIToggle p_toggle, bool p_isOn );


public class UIToggle : UIElement
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		m_toggle = p_gameObject.GetComponent< Toggle >();
		baseElement = m_toggle;
		
		m_toggle.onValueChanged.AddListener( listenerCallbackDispatcher );
		m_callbacks = new List<ToggleChangedCallback>();

		DebugUtils.Assert( m_toggle != null );
	}
	
	public override void update()
	{
		base.update();
	}
	
	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
		removeAllCallbacks();
	}
	
	
	
//-- Transitions --
	
	public void setTransitionMode( Toggle.Transition p_mode )
	{
		m_toggle.transition = p_mode;
	}
	
	public Graphic targetGraphic
	{
		get { return m_toggle.targetGraphic; 	}
		set { m_toggle.targetGraphic = value; 	}
	}

	public SpriteState spriteStates
	{
		get { return m_toggle.spriteState; 	}
		set { m_toggle.spriteState = value; }
	}
	
	public ColorBlock colorStates
	{
		get { return m_toggle.colors; 	}
		set { m_toggle.colors = value; 	}
	}
	
	public AnimationTriggers animationStates
	{
		get { return m_toggle.animationTriggers; 	}
		set { m_toggle.animationTriggers = value;	}
	}

	public bool enabled 
	{
		get { return m_toggle.interactable; }
		set { m_toggle.interactable = value; }
	}

//-- Callbacks --

	public void addValueChangedCallback( ToggleChangedCallback p_action )
	{
		m_callbacks.Add( p_action );
	}
	
	public void removeValueChangedCallback( ToggleChangedCallback p_action)
	{
		m_callbacks.Remove( p_action );
	}
	
	public void removeAllCallbacks()
	{
		m_callbacks.Clear();
	}



//-- Helpers --

	public bool isOn
	{
		get {  return m_toggle.isOn; 	}
		set { m_toggle.isOn = value;	}
	}

	public UIToggleGroup group
	{
		get { return m_group; }
		set
		{
			m_group = value;

			ToggleGroup l_toggleGroup = null;
			if( value != null )
				l_toggleGroup = value.baseElement as ToggleGroup;

			m_toggle.group = l_toggleGroup;
		}
	}
//-------------- Private Implementation------------------

	private void listenerCallbackDispatcher( bool p_toggled )
	{	
		int l_numCallbacks = m_callbacks.Count;
		for (int i = 0; i < l_numCallbacks; ++i)
		{
			ToggleChangedCallback l_callback = m_callbacks[i];
			l_callback( this, p_toggled );
		}
	}

	
	private List<ToggleChangedCallback> m_callbacks;

	private Toggle m_toggle;
	private UIToggleGroup m_group;
}
