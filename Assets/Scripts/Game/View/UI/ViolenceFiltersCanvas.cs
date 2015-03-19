using UnityEngine;
using System.Collections;

public class ViolenceFiltersCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);

		if( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() )
		{
			_setupElement();
			_setupData();
		}
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
	
	//------------------ Private Implementation --------------------
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}
	
	private void _setupElement()
	{
		m_levelZeroToggle 	= getView( "levelZeroToggle" )	 as UIToggle;
		m_levelOneToggle 	= getView( "levelOneToggle" )	 as UIToggle;
		m_levelTwoToggle 	= getView( "levelTwoToggle" )	 as UIToggle;
		m_levelThreeToggle 	= getView( "levelThreeToggle" )	 as UIToggle;
		m_levelFourToggle 	= getView( "levelFourToggle" )	 as UIToggle;
	}

	private void _setupData()
	{
		if (SessionHandler.getInstance ().kidList.Count > 0 && SessionHandler.getInstance ().currentKid == null )
		{
			SessionHandler.getInstance ().currentKid = SessionHandler.getInstance ().kidList [0];
		}
		Kid l_kid = SessionHandler.getInstance ().currentKid;

		switch( l_kid.maxViolence )
		{
			case ViolenceRating.NoViolence:
				m_levelZeroToggle.isOn = true;
				break;
			case ViolenceRating.ViolentInnuendos:
				m_levelOneToggle.isOn = true;
				break;
			case ViolenceRating.ExplosionsButNoVisibleWeapons:
				m_levelTwoToggle.isOn = true;
				break;
			case ViolenceRating.VisibleWeapons:
				m_levelThreeToggle.isOn = true;
				break;
			case ViolenceRating.SimulatedPhysicalViolence:
				m_levelFourToggle.isOn = true;
				break;
		}
	}
	
	private UIToggle m_levelZeroToggle;
	private UIToggle m_levelOneToggle;
	private UIToggle m_levelTwoToggle;
	private UIToggle m_levelThreeToggle;
	private UIToggle m_levelFourToggle;
}
