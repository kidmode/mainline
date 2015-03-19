using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIToggleGroup : UIElement
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		m_toggleGroup = p_gameObject.GetComponent< ToggleGroup >();
		baseElement = m_toggleGroup;
		
		DebugUtils.Assert( m_toggleGroup != null );
	}
	
	public override void update()
	{
		base.update();
	}
	
	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
		//m_toggleGroup.
	}

	//------------------ Private Implementation --------------------
	private ToggleGroup m_toggleGroup;
}
