using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Landing Region
public class RegionLandingState : RegionBaseState
{
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );
		
		m_regionState 	= RegionState.Left;

		_setupMainViews( p_gameController );

//		_setupCongratViews ( p_gameController );


	}
	
	private void _setupMainViews( GameController p_gameController )
	{
		m_activityPanelCanvas.tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );
		m_regionLandingCanvas.tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );
		m_regionBackgroundCanvas.tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );
	}

	private void _setupCongratViews(GameController p_gameController)
	{

		UIManager uiManager = p_gameController.getUI();

		if (uiManager.findScreen (UIScreen.CONGRATS_BACKGROUND) == null) {
		
			UICanvas backGround = uiManager.createScreen (UIScreen.CONGRATS_BACKGROUND, false, 6) as UICanvas;

			ShowAndHide show = backGround.getView ("holder").gameObject.GetComponent<ShowAndHide>();

			show.hide();

		}

		if (uiManager.findScreen (UIScreen.CONGRATS) == null){

			UICanvas screen = uiManager.createScreen (UIScreen.CONGRATS, false, 8);

			ShowAndHide show = screen.getView ("holder").gameObject.GetComponent<ShowAndHide>();
			
			show.hide();

		}

	}
}

// Books Activity
public class RegionBooksState : RegionBaseState
{
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );
		
		_setupMainViews( p_gameController );
		
		m_createActivity = ActivityType.Books;
		m_regionState = RegionState.Right;
	}
	
	private void _setupMainViews( GameController p_gameController )
	{
		RectTransform foreRectTransform = m_foreground.gameObject.GetComponent< RectTransform >();
		m_foreground.transform.localPosition = new Vector3( m_foreground.transform.localPosition.x - foreRectTransform.rect.width * FOREGROUND_SPEED, 0.0f, 0.0f );
		
		RectTransform backRectTransform = m_background.gameObject.GetComponent< RectTransform >();
		m_background.transform.localPosition = new Vector3( m_background.transform.localPosition.x - backRectTransform.rect.width * BACKGROUND_SPEED, 0.0f, 0.0f );
		
		m_mapButton.transform.localPosition = m_cornerPosition + new Vector3( 0, 200.0f, 0 );
		
		List< Vector3 > l_backPositions = new List< Vector3 >();
		l_backPositions.Add( m_cornerPosition  + new Vector3( 0, 200.0f, 0 ) );
		l_backPositions.Add( m_cornerPosition );
		m_backButton.tweener.addPositionTrack( l_backPositions, ZoodlesScreenFactory.FADE_SPEED );
		
		m_activityPanelCanvas.canvasGroup.interactable = false;
		m_activityPanelCanvas.alpha = 0.0f;
		m_cornerProfileCanvas.canvasGroup.interactable = false;
		m_cornerProfileCanvas.alpha = 0.0f;

		// Sean: vzw
		m_regionAppCanvas.active = false;

		_Debug.log (m_cornerProfileCanvas.alpha);
	}
}

// Video Activity
public class RegionVideoState : RegionBaseState
{
	public override void enter( GameController p_gameController )
	{
		GameObject gameLogic = GameObject.FindWithTag("GameController");
		gameLogic.GetComponent<Game> ().gameSwitcher (true);

		base.enter( p_gameController );

		
		_setupMainViews( p_gameController );
		
		m_createActivity = ActivityType.Video;
		m_regionState = RegionState.Right;

	}
	
	private void _setupMainViews( GameController p_gameController )
	{
		RectTransform foreRectTransform = m_foreground.gameObject.GetComponent< RectTransform >();
		m_foreground.transform.localPosition = new Vector3( m_foreground.transform.localPosition.x - foreRectTransform.rect.width * FOREGROUND_SPEED, 0.0f, 0.0f );
		
		RectTransform backRectTransform = m_background.gameObject.GetComponent< RectTransform >();
		m_background.transform.localPosition = new Vector3( m_background.transform.localPosition.x - backRectTransform.rect.width * BACKGROUND_SPEED, 0.0f, 0.0f );
		
		m_mapButton.transform.localPosition = m_cornerPosition + new Vector3( 0, 200.0f, 0 );
		
		List< Vector3 > l_backPositions = new List< Vector3 >();
		l_backPositions.Add( m_cornerPosition  + new Vector3( 0, 200.0f, 0 ) );
		l_backPositions.Add( m_cornerPosition );
		m_backButton.tweener.addPositionTrack( l_backPositions, ZoodlesScreenFactory.FADE_SPEED );
		
		m_activityPanelCanvas.canvasGroup.interactable = false;
		m_activityPanelCanvas.alpha = 0.0f;
		m_cornerProfileCanvas.canvasGroup.interactable = false;
		m_cornerProfileCanvas.alpha = 0.0f;

		// Sean: vzw
		m_regionAppCanvas.active = false;

		_Debug.log (m_cornerProfileCanvas.alpha);
	}
}

// Game Activity
public class RegionGameState : RegionBaseState
{
	public override void enter( GameController p_gameController )
	{
		GameObject gameLogic = GameObject.FindWithTag("GameController");
		gameLogic.GetComponent<Game> ().gameSwitcher (true);

		base.enter( p_gameController );
		
		_setupMainViews( p_gameController );
		
		m_createActivity = ActivityType.Game;
		m_regionState = RegionState.Right;
	}
	
	private void _setupMainViews( GameController p_gameController )
	{
		RectTransform foreRectTransform = m_foreground.gameObject.GetComponent< RectTransform >();
		m_foreground.transform.localPosition = new Vector3( m_foreground.transform.localPosition.x - foreRectTransform.rect.width * FOREGROUND_SPEED, 0.0f, 0.0f );
		
		RectTransform backRectTransform = m_background.gameObject.GetComponent< RectTransform >();
		m_background.transform.localPosition = new Vector3( m_background.transform.localPosition.x - backRectTransform.rect.width * BACKGROUND_SPEED, 0.0f, 0.0f );
		
		m_mapButton.transform.localPosition = m_cornerPosition + new Vector3( 0, 200.0f, 0 );
		
		List< Vector3 > l_backPositions = new List< Vector3 >();
		l_backPositions.Add( m_cornerPosition  + new Vector3( 0, 200.0f, 0 ) );
		l_backPositions.Add( m_cornerPosition );
		m_backButton.tweener.addPositionTrack( l_backPositions, ZoodlesScreenFactory.FADE_SPEED );
		
		m_activityPanelCanvas.canvasGroup.interactable = false;
		m_activityPanelCanvas.alpha = 0.0f;
		m_cornerProfileCanvas.canvasGroup.interactable = false;
		m_cornerProfileCanvas.alpha = 0.0f;

		// Sean: vzw
		m_regionAppCanvas.active = false;

		_Debug.log (m_cornerProfileCanvas.alpha);
	}
}

// Fun Activity
public class RegionFunState : RegionBaseState
{
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );
		
		_setupMainViews( p_gameController );
		
		m_createActivity = ActivityType.Fun;
		m_regionState = RegionState.Right;
	}
	
	private void _setupMainViews( GameController p_gameController )
	{
		RectTransform foreRectTransform = m_foreground.gameObject.GetComponent< RectTransform >();
		m_foreground.transform.localPosition = new Vector3( m_foreground.transform.localPosition.x - foreRectTransform.rect.width * FOREGROUND_SPEED, 0.0f, 0.0f );
		
		RectTransform backRectTransform = m_background.gameObject.GetComponent< RectTransform >();
		m_background.transform.localPosition = new Vector3( m_background.transform.localPosition.x - backRectTransform.rect.width * BACKGROUND_SPEED, 0.0f, 0.0f );
		
		m_mapButton.transform.localPosition = m_cornerPosition + new Vector3( 0, 200.0f, 0 );
		
		List< Vector3 > l_backPositions = new List< Vector3 >();
		l_backPositions.Add( m_backButtonPosition  + new Vector3( 0, 200.0f, 0 ) );
		l_backPositions.Add( m_backButtonPosition );
		m_backButton.tweener.addPositionTrack( l_backPositions, ZoodlesScreenFactory.FADE_SPEED );
		
		m_activityPanelCanvas.canvasGroup.interactable = false;
		m_activityPanelCanvas.alpha = 0.0f;
		m_cornerProfileCanvas.canvasGroup.interactable = false;
		m_cornerProfileCanvas.alpha = 0.0f;

		// Sean: vzw
		m_regionAppCanvas.active = false;
	}
}