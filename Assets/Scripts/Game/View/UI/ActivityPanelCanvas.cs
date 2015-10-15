using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class ActivityPanelCanvas : UICanvas
{

	private const float TWEEN_DISTANCE 	= 30.0f;
	private const float TWEEN_TIME 		= 0.3f;


	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		//tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );

		m_appTogle 	= getView( "appListButton" ) as UIToggle;
		m_appTogle.addValueChangedCallback( onToggleChanged );
	
		m_videoToggle 	= getView( "videoButton" ) as UIToggle;
		m_videoToggle.addValueChangedCallback( onToggleChanged );

		m_gamesToggle 	= getView( "gamesButton" ) as UIToggle;
		m_gamesToggle.addValueChangedCallback( onToggleChanged );

		m_booksToggle 	= getView( "booksButton" ) as UIToggle;
		m_booksToggle.addValueChangedCallback( onToggleChanged );

		m_funToggle 	= getView( "activitiesButton" ) as UIToggle;
		m_funToggle.addValueChangedCallback( onToggleChanged );

		m_group 		= getView( "panel" ) as UIToggleGroup;

		m_inactivePosition_Y 	= m_videoToggle.transform.localPosition.y;
		m_activePosition_Y 		= m_inactivePosition_Y - TWEEN_DISTANCE;

		SetupLocalizition();
	}
	
	public override void update()
	{
		base.update();
	}
	
	
	public override void enteringTransition(  )
	{
		base.enteringTransition( );

		//tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
	}
	
	public override void exitingTransition( )
	{
		base.exitingTransition( );
	}
	
	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}
	
	

	public void untoggleActivities()
	{
		if( m_currentToggle == null )
			return;

		m_currentToggle.group 	= null;
		m_currentToggle.isOn 	= false;
		m_currentToggle.group 	= m_group;

		m_currentToggle = null;
	}

//----------------- Private Implementation -------------------

	private void onToggleChanged( UIToggle p_toggle, bool p_isToggled )
	{
		if ((Application.internetReachability == NetworkReachability.NotReachable 
		     || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected()) 
		    /*&& !p_toggle.name.Equals("booksButton")*/)
		{
			m_currentToggle = p_toggle;
			return;
		}

		//If we click on an activity tab that is already selected, don't do anything
		if( m_currentToggle == p_toggle && p_isToggled )
			return;

		if( p_isToggled )
			_toggleOn( p_toggle );
		else
			_toggleOff( p_toggle );
	}

	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private void _toggleOn( UIToggle p_toggle )
	{
		Vector3 l_startPosition	= p_toggle.transform.localPosition;
		l_startPosition.y = m_inactivePosition_Y;

		Vector3 l_finalPosition = p_toggle.transform.localPosition;
		l_finalPosition.y = m_activePosition_Y;
		
		_tweenPosition( p_toggle, l_startPosition, l_finalPosition );
		m_currentToggle = p_toggle;
	}

	private void _toggleOff( UIToggle p_toggle )
	{
		Vector3 l_startPosition	= p_toggle.transform.localPosition;
		l_startPosition.y = m_activePosition_Y;
		
		Vector3 l_finalPosition = p_toggle.transform.localPosition;
		l_finalPosition.y = m_inactivePosition_Y;
		
		_tweenPosition( p_toggle, l_startPosition, l_finalPosition );
		//m_currentToggle = null;
	}
	
	private void _tweenPosition( UIElement p_element, Vector3 p_start, Vector3 p_end )
	{
		List<Vector3> position = new List<Vector3>();
		position.Add( p_start );
		position.Add( p_end );
		p_element.tweener.addPositionTrack( position, TWEEN_TIME );
	}

	private void SetupLocalizition()
    {
		//Kev, new tab
		UILabel l_appLabel    = m_appTogle.getView("titleLabel") as UILabel;
		//End

        UILabel l_videoLabel    = m_videoToggle.getView("titleLabel") as UILabel;
        UILabel l_gamesLabel    = m_gamesToggle.getView("titleLabel") as UILabel;
        UILabel l_booksLabel    = m_booksToggle.getView("titleLabel") as UILabel;
        UILabel l_funLabel      = m_funToggle.getView("titleLabel") as UILabel;
		
		l_gamesLabel.text       = Localization.getString(Localization.TXT_LABEL_GAMES);
		l_videoLabel.text       = Localization.getString(Localization.TXT_LABEL_VIDEOS);
		l_booksLabel.text       = Localization.getString(Localization.TXT_LABEL_BOOKS);
		l_funLabel.text         = Localization.getString(Localization.TXT_LABEL_ACTIVITIES);

		//Kev, new tab
		l_appLabel.text 				= Localization.getString(Localization.TXT_TAB_APPLIST); 
		//End
    }


	private UIToggle m_appTogle;
	private UIToggle m_videoToggle;
	private UIToggle m_gamesToggle;
	private UIToggle m_booksToggle;
	private UIToggle m_funToggle;

	private UIToggleGroup m_group;

	private UIToggle m_currentToggle;

	private float 	m_inactivePosition_Y 	= 0.0f;
	private float 	m_activePosition_Y 		= 0.0f;
}
