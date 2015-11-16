using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapState : GameState
{
	//consts
	private const float LOADING_WEIGHT	= 1.0f;
	private const float LOADING_START	= 80.0f;

	enum SubState
	{
		NONE,
		GO_PROFILE,
		GO_REGION,
		GO_KIDPROFILE,
		//added by honda
		ADD_BIRTHYEAR,
	}
	//--------------------Public Interface -----------------------
	
	public override void enter(GameController p_gameController)
	{

		base.enter(p_gameController);

//		AndroidNativeUtility.ShowPreloader(Localization.getString (Localization.TXT_LOADING_TITLE), Localization.getString (Localization.TXT_LOADING_MESSAGE));

		
//		Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.InversedLarge);
//		Handheld.StartActivityIndicator();
		KidMode.showProgressBar ();

		//honda: debug mode
		SessionHandler.getInstance().singleGameList.Clear();
		SessionHandler.getInstance().singleVideoList.Clear();
		SessionHandler.getInstance().multipleGameList.Clear();
		SessionHandler.getInstance().multipleVideoList.Clear();
		//end debug mode

		KidMode.systemGC ();
		_setupMap(p_gameController);
		_setupKidProfile(p_gameController);
		m_subState = SubState.NONE;

		if (!TimerController.Instance.isRunning && !TimerController.Instance.timesUp)
		{
			TimerController.Instance.setKidTimer(SessionHandler.getInstance().currentKid.id, 
			                                     SessionHandler.getInstance().currentKid.timeLimits,
			                                     SessionHandler.getInstance().currentKid.timeLeft);
			TimerController.Instance.startTimer();
			SessionHandler.getInstance().currentKid.lastPlay = System.DateTime.Now.ToString();
		}
		else if (TimerController.Instance.timesUp)
		{
			TimerController.Instance.timesUp = false;
		}

		SoundManager.getInstance().play("96", 0, 1, "", null, true);

		GAUtil.logScreen("MapScreen");

//		Handheld.StopActivityIndicator();
//		AndroidNativeUtility.HidePreloader();

		KidMode.dismissProgressBar ();
	}
	
	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		if (m_subState != SubState.NONE)
		{
			switch (m_subState)
			{
			case SubState.GO_PROFILE:
				_changeToProfile(p_gameController);
				break;
			case SubState.GO_REGION:
				_changeToRegionLanding(p_gameController);
				break;
			case SubState.GO_KIDPROFILE:
				_changeToKidsProfile(p_gameController);
				break;
			case SubState.ADD_BIRTHYEAR:
				_changeToBirthYear(p_gameController);
				break;
			}

			m_subState = SubState.NONE;
		}
	}
	
	public override void exit(GameController p_gameController)
	{
        UIManager l_ui = p_gameController.getUI();

		//honda
		//DO NOT remove map, but cache map temporarily
		m_mapCanvas.active = false;
		//not hide map, remove map to have more space
//		l_ui.removeScreenImmediately(UIScreen.MAP);

        l_ui.removeScreen(UIScreen.CORNER_PROFILE_INFO);
		//if( m_removeCornerProfile )
		//	p_gameController.getUI().removeScreen( UIScreen.CORNER_PROFILE_INFO );

		base.exit(p_gameController);
	}

	//------------------ Private Implementation ----------------------

	private void onTransitionDone()
	{
		m_entranceButton.enabled = true;
	}

	private void _setupMap(GameController p_gameController)
	{
		UIManager l_ui = p_gameController.getUI();

		SplashBackCanvas sbcanvas = l_ui.findScreen(UIScreen.SPLASH_BACKGROUND) as SplashBackCanvas;
		if (sbcanvas != null) {
			l_ui.removeScreenImmediately(sbcanvas);
		}

		m_mapCanvas = l_ui.findScreen(UIScreen.MAP);
		if (m_mapCanvas != null)
		{
			m_mapCanvas.active = true;
			m_mapCanvas.tweener.addAlphaTrack(0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED);

			// Sean: vzw
			m_entranceButton.enabled = false;
			m_entranceButton.tweener.addAlphaTrack(0.0f, 1.0f,
			                                       ZoodlesScreenFactory.FADE_SPEED,
			                                       (UIElement p_element, Tweener.TargetVar p_targetVar) => { onTransitionDone(); });
			// end vzw
			return;
		}

//		GameObject obj = Resources.Load<GameObject>("Prefabs/Screens/LoadingScreen");
//		GameObject ins = GameObject.Instantiate(obj) as GameObject;

//		l_ui.createScreenAsync(UIScreen.MAP, (UICanvas canvas) => {

			m_mapCanvas = l_ui.createScreen(UIScreen.MAP, true, 10);
//			GameObject.DestroyImmediate(ins);


//			m_mapCanvas = canvas;

			m_mapCanvas.enterTransitionEvent += onTransitionEnter;
			m_mapCanvas.exitTransitionEvent += onTransitionExit;
			
			

			m_entranceButton = m_mapCanvas.getView("entranceButton") as UIButton;
			m_entranceButton.addClickCallback(onBackClicked);
			// Sean: vzw
			m_entranceButton.enabled = false;
			m_entranceButton.tweener.addAlphaTrack(0.0f, 1.0f,
			                                       ZoodlesScreenFactory.FADE_SPEED,
			                                       (UIElement p_element, Tweener.TargetVar p_targetVar) => { onTransitionDone(); });
			// end vzw
			
			UIButton l_jungleButton = m_mapCanvas.getView("jungleButton") as UIButton;
			l_jungleButton.addClickCallback(onJungleClicked);
			
			//		UIButton l_savannaButton = m_mapCanvas.getView("savannaButton") as UIButton;
			
			UIButton l_welcomeButton = m_mapCanvas.getView("infoPanel") as UIButton;
			l_welcomeButton.addClickCallback(onSpeechClick);
			
			UIImage l_arrow	= m_mapCanvas.getView( "activeArrow" ) as UIImage;
			List<Vector3> l_arrowPosList = new List<Vector3>();
			l_arrowPosList.Add(l_arrow.transform.localPosition);
			l_arrowPosList.Add(l_arrow.transform.localPosition + new Vector3(0, 50, 0));
			l_arrowPosList.Add(l_arrow.transform.localPosition);
			l_arrow.tweener.addPositionTrack(l_arrowPosList, 1.0f, null, Tweener.Style.Standard, true);

//		}, true, 10);
	}

	private void _setupKidProfile(GameController p_gameController)
	{
		UIManager l_ui = p_gameController.getUI();
		m_cornerProfileCanvas = l_ui.createScreen(UIScreen.CORNER_PROFILE_INFO, false, 11);
		
		UIButton l_profilebutton = m_cornerProfileCanvas.getView("profileButton") as UIButton;
		l_profilebutton.addClickCallback(onProfileButtonClick);		
	}

	private void _changeToProfile(GameController p_gameController)
	{
		p_gameController.changeState(ZoodleState.PROFILE_SELECTION);
			
		m_mapCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);
	}

	private void _changeToRegionLanding(GameController p_gameController)
	{
		p_gameController.changeState(ZoodleState.REGION_LANDING);

		m_mapCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);
	}

	//added by joshua
	private void _changeToKidsProfile(GameController p_gameController)
	{
		p_gameController.connectState(ZoodleState.KIDS_PROFILE, ZoodleState.MAP);
		p_gameController.changeState(ZoodleState.KIDS_PROFILE);
		
		m_mapCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);
	}

	//added by honda
	private void _changeToBirthYear(GameController p_gameController)
	{
		UIManager l_ui = p_gameController.getUI();
		UICanvas l_backScreen = l_ui.findScreen(UIScreen.SPLASH_BACKGROUND);
		if (l_backScreen == null)
		{
			l_backScreen = l_ui.createScreen(UIScreen.SPLASH_BACKGROUND,true, -1);
			SplashBackCanvas l_splashBack = l_backScreen as SplashBackCanvas;
			l_splashBack.setDown();
		}

		p_gameController.connectState(ZoodleState.BIRTHYEAR, ZoodleState.MAP);
		p_gameController.changeState(ZoodleState.BIRTHYEAR);
	}

	//Listeners	
	private void onFadeFinish(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		UICanvas p_canvas = p_element as UICanvas;
		p_canvas.active = false;
		p_canvas.isTransitioning = false;
	}

	private void onBackClicked(UIButton p_button)
	{
		m_subState = SubState.ADD_BIRTHYEAR;//SubState.GO_PROFILE;
	}

	private void onJungleClicked(UIButton p_button)
	{
		m_subState = SubState.GO_REGION;
		SwrveComponent.Instance.SDK.NamedEvent("ENTER_JUNGLE");
	}	

	private void onSpeechClick(UIButton p_button)
	{
		p_button.tweener.addAlphaTrack(1.0f, 0.0f, 1.0f);
		p_button.removeAllCallbacks();
	}

	private void onTransitionEnter(UICanvas p_canvas)
	{
		p_canvas.graphicRaycaster.enabled = false;
	}
	
	private void onTransitionExit(UICanvas p_canvas)
	{
		p_canvas.graphicRaycaster.enabled = true;
	}

	//added by joshua
	private void onProfileButtonClick(UIButton p_button)
	{
		m_subState = SubState.GO_KIDPROFILE;
	}

	private UIButton m_entranceButton;
	private UICanvas m_mapCanvas;
	private UICanvas m_cornerProfileCanvas;
	private SubState m_subState = SubState.NONE;
}
