using UnityEngine;
using System.Collections;

public class TimeLimitsCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		
		_setupElement();
	}
	
	public override void update ()
	{
		base.update ();
	}
	
	public override void dispose (bool p_deep)
	{
		base.dispose (p_deep);
	}
	
	public override void enteringTransition ()
	{
		base.enteringTransition ();
		tweener.addAlphaTrack( 1.0f, 0.0f, 0.0f, onFadeFinish );
	}
	
	public override void exitingTransition ()
	{
		base.exitingTransition ();
	}
	
	public void setTimeLimits( Hashtable p_hashTable )
	{
		DebugUtils.Assert( p_hashTable != null);
		
		if( 0 == p_hashTable.Count )
			return;
		
		m_data = p_hashTable;
		_setupData();
	}
	
	//------------------ Private Implementation --------------------
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}
	
	private void _setupElement( )
	{
		m_weekGroup 			= getView( "weekGroup" ) as UIToggleGroup;
		m_weekThirtyMin 		= m_weekGroup.getView( "thirtyMinToggle" )	 as UIToggle;
		m_weekOneHour 			= m_weekGroup.getView( "oneHourToggle" )	 as UIToggle;
		m_weekTwoHours 			= m_weekGroup.getView( "twoHoursToggle" )	 as UIToggle;
		m_weekFourHours 		= m_weekGroup.getView( "fourHoursToggle" )	 as UIToggle;
		m_weekUnlimited 		= m_weekGroup.getView( "unlimitedToggle" )	 as UIToggle;
		
		m_weekendGroup 			= getView( "weekendGroup" ) as UIToggleGroup;
		m_weekendThirtyMin 		= m_weekendGroup.getView( "thirtyMinToggle" )	 as UIToggle;
		m_weekendOneHour 		= m_weekendGroup.getView( "oneHourToggle" )		 as UIToggle;
		m_weekendTwoHours 		= m_weekendGroup.getView( "twoHoursToggle" )	 as UIToggle;
		m_weekendFourHours 		= m_weekendGroup.getView( "fourHoursToggle" )	 as UIToggle;
		m_weekendUnlimited 		= m_weekendGroup.getView( "unlimitedToggle" )	 as UIToggle;
	}
	
	private void _setupData()
	{	
		if( (bool)m_data[ "weekday_disabled" ] )
		{
			m_weekUnlimited.isOn = true;
		}
		else
		{
			switch( m_data[ "weekday_limit" ].ToString() )
			{
			case "30" :
				m_weekThirtyMin.isOn = true;
				break;
			case "60" :
				m_weekOneHour.isOn = true;
				break;
			case "120" :
				m_weekTwoHours.isOn = true;
				break;
			case "240" :
				m_weekFourHours.isOn = true;
				break;
			default :
				break;
			}
		}

		if( (bool)m_data[ "weekend_disabled" ] )
		{
			m_weekendUnlimited.isOn = true;
		}
		else
		{
			switch( m_data[ "weekend_limit" ].ToString() )
			{
			case "30" :
				m_weekendThirtyMin.isOn = true;
				break;
			case "60" :
				m_weekendOneHour.isOn = true;
				break;
			case "120" :
				m_weekendTwoHours.isOn = true;
				break;
			case "240" :
				m_weekendFourHours.isOn = true;
				break;
			default :
				break;
			}
		}
	}
	
	private UIToggleGroup m_weekGroup;
	private UIToggle m_weekThirtyMin;
	private UIToggle m_weekOneHour;
	private UIToggle m_weekTwoHours;
	private UIToggle m_weekFourHours;
	private UIToggle m_weekUnlimited;
	
	private UIToggleGroup m_weekendGroup;
	private UIToggle m_weekendThirtyMin;
	private UIToggle m_weekendOneHour;
	private UIToggle m_weekendTwoHours;
	private UIToggle m_weekendFourHours;
	private UIToggle m_weekendUnlimited;
	
	private Hashtable m_data;
}
