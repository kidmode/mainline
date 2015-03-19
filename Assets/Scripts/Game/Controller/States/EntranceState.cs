using UnityEngine;
using System.Collections;

public class EntranceState : GameState
{

	//consts
	private const float LOADING_WEIGHT	= 1.0f;
	private const float LOADING_START	= 80.0f;
	
	//--------------------Public Interface -----------------------
	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );

        UIManager l_ui = p_gameController.getUI();

        m_entranceCanvas = l_ui.createScreen(UIScreen.ENTRANCE, true);
		m_entranceCanvas.enterTransitionEvent 	+= onTransitionEnter;
		m_entranceCanvas.exitTransitionEvent 	+= onTransitionExit;
		m_entranceCanvas.tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );

        m_cornerProfileCanvas = l_ui.findScreen(UIScreen.CORNER_PROFILE_INFO);
		if( m_cornerProfileCanvas == null )
		{
            m_cornerProfileCanvas = l_ui.createScreen(UIScreen.CORNER_PROFILE_INFO, true, 2);
		}


        UILabel l_newsFeedLabel = m_entranceCanvas.getView("newsLabel") as UILabel;
        l_newsFeedLabel.text = Localization.getString(Localization.TXT_LABEL_NEWS_FEED);

		UIButton l_profilesButton = m_entranceCanvas.getView( "profilesButton" ) as UIButton;
		l_profilesButton.addClickCallback( onBackClicked );

        UILabel l_profileLabel = l_profilesButton.getView("btnText") as UILabel;
        l_profileLabel.text = Localization.getString(Localization.TXT_BUTTON_PROFILES);

        UIButton l_mapsButton = m_entranceCanvas.getView("mapsButton") as UIButton;
        l_mapsButton.addClickCallback(onMapsClicked);

        UILabel l_mapsLabel = l_mapsButton.getView("btnText") as UILabel;
        l_mapsLabel.text = Localization.getString(Localization.TXT_BUTTON_MAPS);

        UIButton l_welcomeButton = m_entranceCanvas.getView("lionSpeechBubble") as UIButton;
        l_welcomeButton.addClickCallback(onInfoClicked);

        UILabel l_welcomeTitleLabel = l_welcomeButton.getView("header") as UILabel;
        l_welcomeTitleLabel.text = Localization.getString(Localization.TXT_LABEL_WELCOME_BACK);

        UILabel l_welcomeBodyLabel = l_welcomeButton.getView("body") as UILabel;
        l_welcomeBodyLabel.text = Localization.getString(Localization.TXT_LABEL_WELCOME_INFO);

		//added by joshua
		m_profileButton = m_cornerProfileCanvas.getView( "profileButton" ) as UIButton;
		m_profileButton.addClickCallback( onProfileClick );
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );		
		

		if (m_confirmBack)
			_gotoProfileSelectionState( p_gameController );

		if( m_gotoMaps )
			_gotoMapsState( p_gameController );

		//added by joshua
		if (m_gotoKidsProfile)
			_changeToKidsProfile( p_gameController );
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );

        UIManager l_ui = p_gameController.getUI();

        l_ui.removeScreen(UIScreen.ENTRANCE);

		if( m_removeCornerProfile )
            l_ui.removeScreen(UIScreen.CORNER_PROFILE_INFO);
	}
	
	
//------------------------ Private Implementation ---------------------------
//---------------------------------------------------------------------------

	private void _gotoProfileSelectionState( GameController p_gameController )
	{
		m_removeCornerProfile = true;
		
		p_gameController.changeState( ZoodleState.PROFILE_SELECTION );
		
		m_entranceCanvas.tweener.addAlphaTrack(      1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
		m_cornerProfileCanvas.tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
		
		m_confirmBack = false;
	}

	private void _gotoMapsState( GameController p_gameController )
	{
		m_removeCornerProfile = false;
		
		p_gameController.changeState( ZoodleState.MAP );
		m_entranceCanvas.tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
		
		m_gotoMaps = false;
	}

	//added by joshua
	private void _changeToKidsProfile( GameController p_gameController )
	{
		m_removeCornerProfile = true;

		p_gameController.connectState(ZoodleState.KIDS_PROFILE, ZoodleState.ENTRANCE);
		p_gameController.changeState( ZoodleState.KIDS_PROFILE );
		
		m_entranceCanvas.tweener.addAlphaTrack(      1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
		m_cornerProfileCanvas.tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
		
		m_gotoKidsProfile = false;
	}

//Listeners	
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		UICanvas p_canvas = p_element as UICanvas;
		p_canvas.isTransitioning = false;
	}
	

	private void onBackClicked( UIButton p_button )
	{
		m_confirmBack = true;
	}

	private void onMapsClicked( UIButton p_button )
	{
		m_gotoMaps = true;
	}	

	private void onInfoClicked( UIButton p_button )
	{
		p_button.tweener.addAlphaTrack( 1.0f, 0.0f, 1.0f );
		p_button.removeAllCallbacks();
	}

	private void onTransitionEnter( UICanvas p_canvas )
	{
		p_canvas.graphicRaycaster.enabled = false;
	}
	
	private void onTransitionExit( UICanvas p_canvas )
	{
		p_canvas.graphicRaycaster.enabled = true;
	}

	//added
	private void onProfileClick( UIButton p_button )
	{
		m_gotoKidsProfile = true;
	}


	private UICanvas m_entranceCanvas;
	private UICanvas m_cornerProfileCanvas;

	//added by joshua
	private UIButton m_profileButton;

	private bool m_confirmBack 	= false;
	private bool m_gotoMaps		= false;
	private bool m_removeCornerProfile = false;

	//added by joshua
	private bool m_gotoKidsProfile = false;
}
