using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UISlider : UIElement
{

	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		m_slider = p_gameObject.GetComponent< Slider >();
		baseElement = m_slider;
		
		DebugUtils.Assert( m_slider != null );
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
		m_slider.transition = p_mode;
	}
	
	public Graphic targetGraphic
	{
		get { return m_slider.targetGraphic; 	}
		set { m_slider.targetGraphic = value; 	}
	}
	
	public SpriteState spriteStates
	{
		get { return m_slider.spriteState; 	}
		set { m_slider.spriteState = value; }
	}
	
	public ColorBlock colorStates
	{
		get { return m_slider.colors; 	}
		set { m_slider.colors = value; 	}
	}
	
	public AnimationTriggers animationStates
	{
		get { return m_slider.animationTriggers; 	}
		set { m_slider.animationTriggers = value;	}
	}
	
	
	
//-- Callbacks --
	
	public void addValueChangedCallback( UnityEngine.Events.UnityAction<float> p_action )
	{
		m_slider.onValueChanged.AddListener( p_action );
	}
	
	public void removeValueChangedCallback( UnityEngine.Events.UnityAction<float> p_action )
	{
		m_slider.onValueChanged.RemoveListener( p_action );
	}
	
	public void removeAllCallbacks()
	{
		m_slider.onValueChanged.RemoveAllListeners();
	}
	

//-- Helpers --
	public float minValue
	{
		get { return m_slider.minValue; 	}
		set { m_slider.minValue = value; 	}
	}
	public float maxValue
	{
		get { return m_slider.maxValue; 	}
		set { m_slider.maxValue = value; 	}
	}
	public float value
	{
		get { return m_slider.value; 	}
		set { m_slider.value = value; 	}
	}
 
	public float normalizedValue
	{
		get { return m_slider.value; 	}
		set { m_slider.value = value; 	}
	}

	public Slider.Direction direction
	{
		get { return m_slider.direction; 	}
		set { m_slider.direction = value;	}
	}

	public bool wholeNumbers 
	{
		get { return m_slider.wholeNumbers; 	}
		set { m_slider.wholeNumbers = value; 	}
	}



	//-------------- Private Implementation------------------
	private Slider m_slider;
}
