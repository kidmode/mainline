using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameInfo : System.Object
{
	public WebViewInfo webViewData;
	public AppInfo appData;
	public bool isWebView = true;

	public GameInfo(WebViewInfo p_webView)
	{
		webViewData = p_webView;
		isWebView = true;
	}

	public GameInfo(AppInfo p_app)
	{
		appData = p_app;
		isWebView = false;
	}
}

public class WebViewInfo : System.Object
{
	public const string DEFAULT_URL = "https://www.youtube.com/embed/v53mhRXXT2g?rel=0";

	public delegate void RetriveIconHandler(Texture2D p_icon);
	
	public Texture2D    icon;
	public string       urlString;
	
	public WebContent   webData;
	
	public WebViewInfo(Texture2D p_icon, WebContent p_content = null, string p_urlString = DEFAULT_URL)
	{
		icon 		= p_icon;
		urlString 	= p_urlString;
		webData     = p_content;

		if (icon == null)
		{
			if( webData.icon != null )
			{
				RequestQueue l_queue = new RequestQueue();
				l_queue.add(new ImageRequest("icon", webData.icon, _requestIconComplete));
				l_queue.request(RequestType.RUSH);
				m_queue = l_queue;
			}
			else if( webData.iconMedium != null )
			{
				RequestQueue l_queue = new RequestQueue();
				l_queue.add(new ImageRequest("icon", webData.iconMedium, _requestIconComplete));
				l_queue.request(RequestType.RUSH);
				m_queue = l_queue;
			}
		}
	}

	public void retriveIcon(RetriveIconHandler p_handler)
	{
		if (icon != null)
			p_handler(icon);
		else
			m_retriveHandler = p_handler;
	}

	private void _requestIconComplete(WWW p_response)
	{
		if (p_response.error == null)
		{
			icon = p_response.texture;
			if (m_retriveHandler != null)
			{
				m_retriveHandler(icon);
				m_retriveHandler = null;
			}
		}

		if (null != m_queue)
		{
			m_queue.dispose();
			m_queue = null;
		}
	}

	public void dispose()
	{
		if (null != m_queue)
		{
			m_queue.dispose();
			m_queue = null;
		}

		if (null != icon)
		{
			GameObject.Destroy(icon);
			icon = null;
		}
	}

	private RequestQueue m_queue = null;
	private RetriveIconHandler m_retriveHandler = null;
}

public class RegionBaseState : GameState
{
	//consts
	protected const float LOADING_WEIGHT	= 1.0f;
	protected const float LOADING_START	= 80.0f;
	
	protected const float PARALLAX_TWEEN_TIME = 1.4f;
	
	protected const float FOREGROUND_SPEED = 0.5f;
	protected const float BACKGROUND_SPEED = 0.13f;

	private RequestQueue m_queue = null;
	private RequestQueue m_bookQueue = null;

	protected enum RegionState
	{
		Left = 0,
		Right
	}
	
	protected enum ActivityType
	{
		None = 0,
		Video,
		Game,
		Books,
		Fun
	}

	protected enum SubState
	{
		None,
		GO_MAP,
		GO_KIDPROFILE,
		GO_VIDEO,
		GO_GAME,
		GO_BOOKS,
		GO_PAINT
	}
	
	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);
		
		m_regionState 	= RegionState.Left;
		m_transitioning = false;
		m_subState = SubState.None;
		m_bookLoaded = false;
		m_linkLoaded = false;
		
		_createViews();
		
		_setupElements();

		if (m_queue == null)
		{
			m_queue = new RequestQueue();
		}

		if (!m_requestStates.ContainsKey("PAINT") || m_requestStates["PAINT"] == true)
		{
			m_queue.add(new DrawingListRequest(_requestDrawingListComplete));
		}

		m_queue.request(RequestType.RUSH);

		SoundManager.getInstance().play("96", 0, 1, "", null, true);

		GAUtil.logScreen("RegionLandingScreen");
	}
	
	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		checkActivityScreen ();

		if( canSetImage )
		{
			if( m_topBook != null && m_topBook.icon != null && null != m_bookActivityCanvas )
			{
				UIButton l_featureButton = m_bookActivityCanvas.getView("featureOne") as UIButton;
				if( l_featureButton != null)
				{
					UIImage l_featureImage = l_featureButton.getView("featureOneIcon") as UIImage;
					l_featureImage.setTexture(m_topBook.icon);
					canSetImage = false;
				}
			}
		}

		if( canSetWebContent )
		{
			canSetWebContent = false;
			_setupWebContentList(SessionHandler.getInstance().webContentList);
		}

		if( canSetBook )
		{
			canSetBook = false;
			_setupBookSwipeLists(SessionHandler.getInstance().bookContentList);	
		}

		int l_numTriggers = m_triggers.Count;
		for (int i = 0; i < l_numTriggers; ++i)
		{
			AnimationTrigger l_trigger = m_triggers[i];
			l_trigger.update(p_time);
		}

		if (m_subState != SubState.None)
		{
			switch (m_subState)
			{
			case SubState.GO_MAP:
				_changeToMaps(p_gameController);
				break;
			case SubState.GO_KIDPROFILE:
				_changeToKidsProfile(p_gameController);
				break;
			case SubState.GO_VIDEO:
				_changeToVideoView(p_gameController);
				break;
			case SubState.GO_GAME:
				_changeToGameView(p_gameController);
				break;
			case SubState.GO_BOOKS:
				_changeToBookReader(p_gameController);
				break;
			case SubState.GO_PAINT:
				_changeToPaintActivity(p_gameController);
				break;
			}

			m_subState = SubState.None;
		}
		
		checkIfLinksCacheLoaded(p_gameController);
		checkIfBooksCacheLoaded(p_gameController);
		_handleDynamicActivities();
	}

	public void checkIfLinksCacheLoaded(GameController p_gameController)
	{
		if (m_linkLoaded == false)
		{
			Game l_game = p_gameController.game;
			User l_user = l_game.user;
			WebContentCache l_cache = l_user.contentCache;
			if (l_cache.isFinishedLoadingWebContent)
			{
				m_linkLoaded = true;
				canSetWebContent = true;
			}
		}
	}

	void checkIfBooksCacheLoaded (GameController p_gameController)
	{
		if (m_bookLoaded == false)
		{
			Game l_game = p_gameController.game;
			User l_user = l_game.user;
			WebContentCache l_cache = l_user.contentCache;
			if (l_cache.isFinishedLoadingBooks)
			{
				m_bookLoaded = true;
				canSetBook = true;
			}
		}
	}
	
	public override void exit(GameController p_gameController)
	{
		base.exit(p_gameController);

		UIManager l_ui = p_gameController.getUI();
		l_ui.removeScreenImmediately(UIScreen.REGION_LANDING);
		l_ui.removeScreenImmediately(UIScreen.REGION_LANDING_BACKGROUND);
		l_ui.removeScreenImmediately(UIScreen.ACTIVITY_PANEL);
		l_ui.removeScreenImmediately(UIScreen.CORNER_PROFILE_INFO);

		// Sean: vzw
		l_ui.removeScreenImmediately(UIScreen.REGION_APP);

		m_topBook = null;
		m_gameFeatured = null;
		m_videoFeatured = null;

		m_gameActivityCanvas = null;
		m_videoActivityCanvas = null;
		m_funActivityCanvas = null;
		m_bookActivityCanvas = null;
		// Sean: vzw
		m_regionAppCanvas = null;

		_clearContentLists();
		
		_cleanUpTextures();

		if (null != m_queue)
		{
			m_queue.dispose();
			m_queue = null;
		}

		if (null != m_bookQueue)
		{
			m_bookQueue.dispose();
			m_bookQueue = null;
		}
	}
	
	//------------------ Private Implementation ----------------------

	private void checkActivityScreen()
	{
		if( null != m_gameActivityCanvas && null != m_gameActivityCanvas.getView("mainPanel") )
		{

			if( !m_gameActivityCanvas.getView("mainPanel").active){

				Tweener tw = m_gameActivityCanvas.getView("mainPanel").tweener;

				tw.addAlphaTrack( 0.0f, 1.0f, 1.0f );
			}

		}

		if( null != m_videoActivityCanvas && null != m_videoActivityCanvas.getView("mainPanel") )
		{

			if( !m_videoActivityCanvas.getView("mainPanel").active){ //m_gameActivityCanvas.getView("mainPanel").alpha < 1.0f &&
				
				m_videoActivityCanvas.getView("mainPanel").tweener.addAlphaTrack( 0.0f, 1.0f, 1.0f );
				
			}
		}

		if( null != m_bookActivityCanvas && null != m_bookActivityCanvas.getView("mainPanel") )
		{

			if( !m_bookActivityCanvas.getView("mainPanel").active){ //m_gameActivityCanvas.getView("mainPanel").alpha < 1.0f &&
				
				m_bookActivityCanvas.getView("mainPanel").tweener.addAlphaTrack( 0.0f, 1.0f, 1.0f );
				
			}
		}

		if( null != m_funActivityCanvas && null != m_funActivityCanvas.getView("mainBack") )
		{

			if( !m_funActivityCanvas.getView("mainBack").active){ //m_gameActivityCanvas.getView("mainPanel").alpha < 1.0f &&
				
				m_funActivityCanvas.getView("mainBack").tweener.addAlphaTrack( 0.0f, 1.0f, 1.0f );
				
			}

		}
	}

	
	private void _createViews()
	{
		UIManager l_ui = m_gameController.getUI();

		m_cornerProfileCanvas = l_ui.createScreen(UIScreen.CORNER_PROFILE_INFO, true, 3);
		m_activityPanelCanvas = l_ui.createScreen(UIScreen.ACTIVITY_PANEL, true, 2);
		m_regionLandingCanvas = l_ui.createScreen(UIScreen.REGION_LANDING, true, 1);
		m_regionBackgroundCanvas = l_ui.createScreen(UIScreen.REGION_LANDING_BACKGROUND, true, 0);

		// Sean: vzw
		m_regionAppCanvas = l_ui.createScreen(UIScreen.REGION_APP, false, 2);
	}
	
	private void _setupElements()
	{
		m_speechBubble = m_regionLandingCanvas.getView("speechBubble") as UIButton;

		// Sean: vzw
		m_speechBubble.active = false;
		m_appSwipeList = m_regionAppCanvas.getView("appScrollView") as UISwipeList;

		this._setupAppContentList();
		// end vzw
		
		m_mapButton = m_regionLandingCanvas.getView("mapsButton") as UIButton;
		m_mapButton.addClickCallback(onMapButtonClicked);
		m_cornerPosition = m_mapButton.transform.localPosition;
		
		m_backButton = m_regionLandingCanvas.getView("backButton") as UIButton;
		m_backButton.addClickCallback(onBackButtonClicked);
		m_backButtonPosition = m_backButton.transform.localPosition;
		m_backButton.transform.localPosition += new Vector3(0, 200, 0);

		
		m_background = m_regionBackgroundCanvas.getView("background");
		m_foreground = m_regionLandingCanvas.getView("foreground");

		m_videoButton = m_activityPanelCanvas.getView("videoButton") as UIToggle;
		m_videoButton.addValueChangedCallback(videoCallback);
		m_videoButton.addValueChangedCallback(onActivityToggleClicked);
		
		m_gamesButton = m_activityPanelCanvas.getView("gamesButton") as UIToggle;
		m_gamesButton.addValueChangedCallback(gameCallback);
		m_gamesButton.addValueChangedCallback(onActivityToggleClicked);
		
		m_booksButton = m_activityPanelCanvas.getView("booksButton") as UIToggle;
		m_booksButton.addValueChangedCallback(bookCallback);
		m_booksButton.addValueChangedCallback(onActivityToggleClicked);
		
		m_activitiesbutton = m_activityPanelCanvas.getView("activitiesButton") as UIToggle;
		m_activitiesbutton.addValueChangedCallback(activityCallback);
		m_activitiesbutton.addValueChangedCallback(onActivityToggleClicked);

		m_profileButton = m_cornerProfileCanvas.getView("profileButton") as UIButton;
		m_profileButton.addClickCallback(onProfileClick);

		m_foregroundGafGroup = m_regionLandingCanvas.getView("gafGroup");
		// Sean: vzw
		this._setupGafGroup(false);

		m_triggers.Clear();
		// Sean: vzw
		if (false) {
			m_triggers.Add(new AnimationTrigger(m_regionLandingCanvas.getView("monkeyTrigger") as UIButton, m_regionLandingCanvas.getView("Monkey_Anim") as UIMovieClip));
			m_triggers.Add(new AnimationTrigger(m_regionLandingCanvas.getView("snakeTrigger") as UIButton, m_regionLandingCanvas.getView("Snake_Anim") as UIMovieClip));
			m_triggers.Add(new AnimationTrigger(m_regionLandingCanvas.getView("toucanTrigger") as UIButton, m_regionLandingCanvas.getView("Toucan_Anim") as UIMovieClip));
		} // vzw end

		UIButton l_butterfly = m_regionLandingCanvas.getView("Butterfly") as UIButton;
		List<Vector3> l_butterflyPosList = new List<Vector3>();
		l_butterflyPosList.Add(l_butterfly.transform.localPosition);
		l_butterflyPosList.Add(l_butterfly.transform.localPosition + new Vector3(1000, 0, 0));
		l_butterfly.tweener.addPositionTrack(l_butterflyPosList, 30.0f, null, Tweener.Style.Standard, true);

		_oscillateLightsDown(m_regionLandingCanvas.getView("light"), Tweener.TargetVar.Rotation);
	}

	private void _setupGafGroup(bool active)
	{
		m_foregroundGafGroup.getView("monkeyTrigger").active = active;
		m_foregroundGafGroup.getView("snakeTrigger").active = active;
		m_foregroundGafGroup.getView("toucanTrigger").active = active;
	}
	
	private void _handleDynamicActivities()
	{
		if (m_createActivity == ActivityType.Video)
		{
			m_videoActivityCanvas = m_gameController.getUI().createScreen(UIScreen.VIDEO_ACTIVITY, true, 4);
			m_createActivity = ActivityType.None;
			m_currentActivityCanvas = m_videoActivityCanvas;
			
			m_videoSwipeList = m_videoActivityCanvas.getView("allContentScrollView") as UISwipeList;
			m_videoSwipeList.setData(m_videoViewList);
			m_videoSwipeList.addClickListener("Prototype", onVideoClicked);
			
			m_videoFavorateSwipeList = m_videoActivityCanvas.getView("favorateScrollView") as UISwipeList;
			m_videoFavorateSwipeList.setData(m_videoFavoritesList);
			m_videoFavorateSwipeList.addClickListener("Prototype", onVideoClicked);


			//Get Scroll view updateor
			KidModeScrollViewUpdator viewUpdator = m_videoSwipeList.gameObject.GetComponent<KidModeScrollViewUpdator>();
			viewUpdator.setContentDataSize(m_videoViewList.Count);

			UILabel l_videoInfo = m_videoActivityCanvas.getView("info") as UILabel;
			l_videoInfo.active = (m_videoSwipeList.getData().Count <= 0);

			if( true == l_videoInfo.active && m_linkLoaded )
			{
				l_videoInfo.text = Localization.getString(Localization.TXT_11_LABEL_INFO);
			}

			if (m_videoFeatured != null)
			{
				m_videoFeatured.retriveIcon(setFeatureImageVideo);
			}

			if( m_videoFavoritesList.Count <= 0 )
			{
				UILabel l_favorateInfoLabel = m_videoActivityCanvas.getView("favoriteInfo") as UILabel;
				l_favorateInfoLabel.text 	= Localization.getString(Localization.TXT_11_LABEL_FAVORITE);
			}
		}
		
		if (m_createActivity == ActivityType.Game)
		{
			m_gameActivityCanvas = m_gameController.getUI().createScreen(UIScreen.GAME_ACTIVITY, true, 4);
			m_createActivity = ActivityType.None;
			m_currentActivityCanvas = m_gameActivityCanvas;

			m_gameSwipeList = m_gameActivityCanvas.getView("allContentScrollView") as UISwipeList;
			m_gameSwipeList.setData(m_gameViewList);
			m_gameSwipeList.addClickListener("Prototype", onGameClicked);


			//Get Scroll view updateor
			KidModeScrollViewUpdator viewUpdator = m_gameSwipeList.gameObject.GetComponent<KidModeScrollViewUpdator>();
			viewUpdator.setContentDataSize(m_gameViewList.Count);
			
			m_gameFavorateSwipeList = m_gameActivityCanvas.getView("favorateScrollView") as UISwipeList;
			m_gameFavorateSwipeList.setData(m_gameFavoritesList);
			m_gameFavorateSwipeList.addClickListener("Prototype", onGameClicked);

			UILabel l_gameInfo = m_gameActivityCanvas.getView("info") as UILabel;
			l_gameInfo.active = (m_gameSwipeList.getData().Count <= 0);

			if( true == l_gameInfo.active && m_linkLoaded )
			{
				l_gameInfo.text = Localization.getString(Localization.TXT_12_LABEL_INFO);
			}

			if (m_gameFeatured != null)
			{
				m_gameFeatured.retriveIcon(setFeatureImageGame);
			}

			if( m_gameFavoritesList.Count <= 0 )
			{
				UILabel l_favorateInfoLabel = m_gameActivityCanvas.getView("favoriteInfo") as UILabel;
				l_favorateInfoLabel.text 	= Localization.getString(Localization.TXT_12_LABEL_FAVORITE);
			}
		}
		
		if (m_createActivity == ActivityType.Books)
		{
			m_messageCanvas = m_gameController.getUI().createScreen(UIScreen.MESSAGE, false, 5);
			m_bookActivityCanvas = m_gameController.getUI().createScreen(UIScreen.BOOK_ACTIVITY, true, 4);
			m_createActivity = ActivityType.None;
			m_currentActivityCanvas = m_bookActivityCanvas;
			
			m_bookSwipeList = m_bookActivityCanvas.getView("allContentScrollView") as UISwipeList;
			m_bookSwipeList.setData(m_bookViewList);
			m_bookSwipeList.addClickListener("Prototype", onBookClicked);
			
			m_bookFavorateSwipeList = m_bookActivityCanvas.getView("favorateScrollView") as UISwipeList;
			m_bookFavorateSwipeList.setData(m_bookFavoritesList);
			m_bookFavorateSwipeList.addClickListener("Prototype", onBookClicked);

			//Get Scroll view updateor
			KidModeScrollViewUpdator viewUpdator = m_bookSwipeList.gameObject.GetComponent<KidModeScrollViewUpdator>();
			viewUpdator.setContentDataSize(m_bookViewList.Count);
			
			UILabel l_bookInfo = m_bookActivityCanvas.getView("info") as UILabel;
			l_bookInfo.active = (m_bookSwipeList.getData().Count <= 0);
			
			if( true == l_bookInfo.active && m_bookLoaded )
			{
				l_bookInfo.text = Localization.getString(Localization.TXT_14_LABEL_INFO);
			}

			UIButton l_featureButton = m_bookActivityCanvas.getView("featureOne") as UIButton;
			l_featureButton.addClickCallback(onFeatureBookClicked);
			if(m_topBook != null)
			{
				if( m_topBook.icon == null )
				{
					canSetImage = true;
				}
				else
				{
					UIImage l_featureImage = l_featureButton.getView("featureOneIcon") as UIImage;
					l_featureImage.setTexture(m_topBook.icon);
				}
			}
			m_quitMessageButton = m_messageCanvas.getView("quitButton") as UIButton;
			m_quitMessageButton.addClickCallback(onQuitMessageButtonClick);

			if( m_bookFavoritesList.Count <= 0 )
			{
				UILabel l_favorateInfoLabel = m_bookActivityCanvas.getView("favoriteInfo") as UILabel;
				l_favorateInfoLabel.text 	= Localization.getString(Localization.TXT_14_LABEL_FAVORITE);
			}
		}
		
		if (m_createActivity == ActivityType.Fun)
		{
			m_funActivityCanvas = m_gameController.getUI().createScreen(UIScreen.FUN_ACTIVITY, true, 4);
			m_createActivity = ActivityType.None;
			m_currentActivityCanvas = m_funActivityCanvas;
			
			m_funSwipeList = m_funActivityCanvas.getView("allContentScrollView") as UISwipeList;
			m_funSwipeList.setData(m_funViewList);
			m_funSwipeList.addClickListener("Prototype", onFunActivityClicked);
			//m_createActivity 		= ActivityType.None;
			//m_gotoPaint 			= true;
		}
	}

	private void setFeatureImageVideo(Texture2D p_icon)
	{
		UIButton l_featureButton = m_videoActivityCanvas.getView("featureOne") as UIButton;
		if (null != l_featureButton)
		{
			l_featureButton.addClickCallback(onFeatureVideoClicked);
			UIImage l_featureImage = l_featureButton.getView("featureOneIcon") as UIImage;
			if (null != l_featureImage)
			{
				l_featureImage.setTexture(p_icon);
			}
		}
	}
	
	private void setFeatureImageGame(Texture2D p_icon)
	{
		UIButton l_featureButton = m_gameActivityCanvas.getView("featureOne") as UIButton;
		if (null != l_featureButton)
		{
			l_featureButton.addClickCallback(onFeatureGameClicked);
			UIImage l_featureImage = l_featureButton.getView("featureOneIcon") as UIImage;
			if (null != l_featureImage)
			{
				l_featureImage.setTexture(p_icon);
			}
		}
	}
	
	private void _clearContentLists()
	{
		_disposeGameInfos(m_gameViewList);
		_disposeGameInfos(m_gameFavoritesList);
		_disposeBookInfos(m_bookViewList);
		_disposeBookInfos(m_bookFavoritesList);
		_disposeWebInfos(m_videoViewList);
		_disposeWebInfos(m_videoFavoritesList);
		_disposeActivityInfos(m_funViewList);

		// Sean: vzw
		if (null != m_appList)
		{
			foreach (AppInfo app in m_appList)
			{
				app.dispose();
			}
		}
		m_appList.Clear();
		// end vzw

		
		m_gameViewList.Clear();
		m_gameFavoritesList.Clear();
		m_bookViewList.Clear();
		m_bookFavoritesList.Clear();
		m_videoViewList.Clear();
		m_videoFavoritesList.Clear();
		m_funViewList.Clear();
	}

	private void _disposeWebInfos(List<object> p_list)
	{
		if (null != p_list)
		{
			int l_numInfos = p_list.Count;
			for (int i = 0; i < l_numInfos; ++i)
			{
				WebViewInfo l_info = p_list[i] as WebViewInfo;
				l_info.dispose();
			}
		}
	}

	private void _disposeGameInfos(List<object> p_list)
	{
		if (null != p_list)
		{
			int l_numInfos = p_list.Count;
			for (int i = 0; i < l_numInfos; ++i)
			{
				GameInfo l_info = p_list[i] as GameInfo;
				if( l_info.isWebView )
				{
					l_info.webViewData.dispose();
				}
				else
				{
					l_info.appData.dispose();
				}
			}
		}
	}
	
	private void _disposeBookInfos(List<object> p_list)
	{
		if (null != p_list)
		{
			int l_numInfos = p_list.Count;
			for (int i = 0; i < l_numInfos; ++i)
			{
				BookInfo l_info = p_list[i] as BookInfo;
				l_info.dispose();
			}
		}
	}
	
	private void _disposeActivityInfos(List<object> p_list)
	{
		int l_numInfos = p_list.Count;
		for (int i = 0; i < l_numInfos; ++i)
		{
			ActivityInfo l_info = p_list[i] as ActivityInfo;
			l_info.dispose();
		}
	}
	
	// private const float MAX_WEB_UPDATE = 2.0f;
	// private float m_updateTimer = 0;
	//
	// private void _updateWebContentInfo()
	// {
	//     if( m_updateTimer > 0 )
	//     {
	//         m_updateTimer -= Time.deltaTime;
	//         return;
	//     }
	//
	//     m_updateTimer = MAX_WEB_UPDATE;
	// }
	
	private void _changeToMaps(GameController p_gameController)
	{
		//m_removeCornerProfile = true;
		
		p_gameController.changeState(ZoodleState.MAP);
		
		m_regionLandingCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);
		m_regionBackgroundCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);
		m_activityPanelCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);
	}

	//added by joshua
	private void _changeToKidsProfile(GameController p_gameController)
	{
		p_gameController.connectState(ZoodleState.KIDS_PROFILE, ZoodleState.REGION_LANDING);
		p_gameController.changeState(ZoodleState.KIDS_PROFILE);
		
		m_regionLandingCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);
		m_regionBackgroundCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);
		m_activityPanelCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);
		m_cornerProfileCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);
	}

	private void _changeToVideoView(GameController p_gameController)
	{
		if (m_currentActivityCanvas != null)
			p_gameController.getUI().removeScreenImmediately(m_currentActivityCanvas);

		p_gameController.changeState(ZoodleState.VIDEO_VIEW);
	}

	private void _changeToGameView(GameController p_gameController)
	{
		if (m_currentActivityCanvas != null)
			p_gameController.getUI().removeScreenImmediately(m_currentActivityCanvas);
		
		p_gameController.changeState(ZoodleState.GAME_VIEW);
	}
	
	private void _changeToBookReader(GameController p_gameController)
	{
		//m_removeCornerProfile = true;
		
		if (m_currentActivityCanvas != null)
			p_gameController.getUI().removeScreenImmediately(m_currentActivityCanvas);
		
		m_regionLandingCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);
		m_regionBackgroundCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);
		m_activityPanelCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);

		p_gameController.connectState(ZoodleState.BOOK_ACTIVITY, ZoodleState.REGION_BOOKS);
		p_gameController.changeState(ZoodleState.BOOK_ACTIVITY);
	}
	
	private void _changeToPaintActivity(GameController p_gameController)
	{
		m_regionLandingCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);
		m_regionBackgroundCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);
		m_activityPanelCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);
		m_cornerProfileCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish);
		
		UIManager l_ui = p_gameController.getUI();
		l_ui.removeScreenImmediately(UIScreen.VIDEO_ACTIVITY);
		l_ui.removeScreenImmediately(UIScreen.GAME_ACTIVITY);
		l_ui.removeScreenImmediately(UIScreen.BOOK_ACTIVITY);
		l_ui.removeScreenImmediately(UIScreen.FUN_ACTIVITY);
		
		m_gameActivityCanvas = null;
		m_videoActivityCanvas = null;
		m_funActivityCanvas = null;
		m_bookActivityCanvas = null;

		l_ui.removeScreen(UIScreen.MESSAGE);
		m_messageCanvas = null;
		
		setInputEnabled(false);

		SessionHandler.getInstance ().IsParent = false;
		if( null == SessionHandler.getInstance().currentDrawing )
		{
			p_gameController.changeState(ZoodleState.PAINT_ACTIVITY);
		}
		else
		{
			p_gameController.connectState(ZoodleState.PAINT_VIEW, ZoodleState.REGION_FUN);
			p_gameController.changeState(ZoodleState.PAINT_VIEW);
		}
	}
	
	private void videoCallback(UIToggle p_element, bool p_toggles)
	{
		m_nextActivity = ActivityType.Video;
		SwrveComponent.Instance.SDK.NamedEvent("Tab.VIDEO");
	}
	
	private void gameCallback(UIToggle p_element, bool p_toggles)
	{
		m_nextActivity = ActivityType.Game;
		SwrveComponent.Instance.SDK.NamedEvent("Tab.GAME");
	}
	
	private void bookCallback(UIToggle p_element, bool p_toggles)
	{
		m_nextActivity = ActivityType.Books;
		SwrveComponent.Instance.SDK.NamedEvent("Tab.BOOK");
	}
	
	private void activityCallback(UIToggle p_element, bool p_toggles)
	{
		m_nextActivity = ActivityType.Fun;
		SwrveComponent.Instance.SDK.NamedEvent("Tab.ACTIVITY");
	}
	
	#region Callbacks
	
	private void onFunActivityClicked(UISwipeList p_list, UIButton p_listElement, System.Object p_data, int p_index)
	{
		Drawing l_drawing = (p_data as ActivityInfo).drawing;
		SessionHandler.getInstance().currentDrawing = l_drawing;

		m_subState = SubState.GO_PAINT;
	}

	private void onVideoClicked(UISwipeList p_list, UIButton p_listElement, System.Object p_data, int p_index)
	{
		WebContent l_webContent = (p_data as WebViewInfo).webData;
		SessionHandler.getInstance().currentContent = l_webContent;

		m_subState = SubState.GO_VIDEO;

		Dictionary<string,string> payload = new Dictionary<string,string>() { {"VideoName", l_webContent.name}};
		SwrveComponent.Instance.SDK.NamedEvent("Video.CLICK",payload);
	}

	private void onFeatureVideoClicked(UIButton p_button)
	{
		SessionHandler.getInstance().currentContent = m_videoFeatured.webData;
		
		m_subState = SubState.GO_VIDEO;
	}

	// Sean: vzw
	private void onAppClicked(UISwipeList p_list, UIButton p_listElement, System.Object p_data, int p_index)
	{
		AppInfo l_app = p_data as AppInfo;
		KidMode.startActivity(l_app.packageName);
	}
	// end vzw

	private void onGameClicked(UISwipeList p_list, UIButton p_listElement, System.Object p_data, int p_index)
	{
		GameInfo l_game = p_data as GameInfo;

		if( l_game.isWebView )
		{
			WebContent l_webContent = l_game.webViewData.webData;
			SessionHandler.getInstance().currentContent = l_webContent;
		
			m_subState = SubState.GO_GAME;
			Dictionary<string,string> payload = new Dictionary<string,string>() { {"GameName", l_webContent.name}};
			SwrveComponent.Instance.SDK.NamedEvent("Game.CLICK",payload);
		}
		else
		{

			KidMode.taskManagerLockFalse();

			KidMode.setKidsModeActive(false);

			KidMode.startActivity(l_game.appData.packageName);
			Dictionary<string,string> payload = new Dictionary<string,string>() { {"GameName", l_game.appData.appName}};
			SwrveComponent.Instance.SDK.NamedEvent("Game.CLICK",payload);
		}
	}

	private void onFeatureGameClicked(UIButton p_button)
	{
		SessionHandler.getInstance().currentContent = m_gameFeatured.webData;
		
		m_subState = SubState.GO_GAME;
	}

	private void onFeatureBookClicked(UIButton p_button)
	{
		if(m_topBook == null)
		{
			return;
		}
		if( m_topBook.bookState == BookState.NeedToBuy )
		{
			UILabel l_content = m_messageCanvas.getView("messageContent") as UILabel;
			l_content.text = Localization.getString(Localization.TXT_STATE_REGIONBASE_ASK_BOOK);
			MessageIn();
		}
		if( m_topBook.bookState == BookState.NotRecorded )
		{
			SessionHandler.getInstance().currentBook = SessionHandler.getInstance ().bookTable[m_topBook.bookId] as Book;
			m_subState = SubState.GO_BOOKS;
		}
	}
	
	private void onBookClicked(UISwipeList p_list, UIButton p_listElement, System.Object p_data, int p_index)
	{
		BookInfo l_bookInfo = p_data as BookInfo;

		if( l_bookInfo.bookState == BookState.Recorded )
		{
			SessionHandler.getInstance().currentBook = SessionHandler.getInstance ().bookTable[l_bookInfo.bookId] as Book;
			SessionHandler.getInstance().currentBookReading = SessionHandler.getInstance ().readingTable[l_bookInfo.bookReadingId] as BookReading;
			
			m_subState = SubState.GO_BOOKS;
		}

		if( l_bookInfo.bookState == BookState.NeedToBuy )
		{
			UILabel l_content = m_messageCanvas.getView("messageContent") as UILabel;
			l_content.text = Localization.getString(Localization.TXT_STATE_REGIONBASE_ASK_BOOK);
			MessageIn();
		}

		if( l_bookInfo.bookState == BookState.NotRecorded )
		{
			SessionHandler.getInstance().currentBook = SessionHandler.getInstance ().bookTable[l_bookInfo.bookId] as Book;
			m_subState = SubState.GO_BOOKS;
		}
	}

	private void _requestDrawingListComplete(WWW p_response)
	{
		if (p_response.error == null)
		{
			List<object> l_contentList = new List<object>();
			List<Drawing> l_list = new List<Drawing> ();
			ArrayList l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as ArrayList;
			foreach (object o in l_data)
			{
				l_contentList.Add(new Drawing(o as Hashtable));
				l_list.Add(new Drawing(o as Hashtable));
			}
			_setupDrawingList(l_contentList);
			SessionHandler.getInstance ().drawingList = l_list;
		}
	}
	
	private void onFadeFinish(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		UICanvas p_canvas = p_element as UICanvas;
		p_canvas.isTransitioning = false;
	}
	
	
	private void onToRightRegionTweenFinished(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		List<Vector3> l_backPositions = new List<Vector3>();
		l_backPositions.Add(m_backButtonPosition + new Vector3(0, 200.0f, 0));
		l_backPositions.Add(m_backButtonPosition);
		m_backButton.tweener.addPositionTrack(l_backPositions, ZoodlesScreenFactory.FADE_SPEED);
		
		m_transitioning = false;
		//m_activityPanelCanvas.canvasGroup.interactable = true;
		
		m_createActivity = m_nextActivity;
		m_nextActivity = ActivityType.None;

		m_foregroundGafGroup.gameObject.SetActive (false);
	}
	
	private void onToLeftRegionTweenFinished(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		List<Vector3> l_mapPositions = new List<Vector3>();
		l_mapPositions.Add(m_cornerPosition - new Vector3(200.0f, 0, 0));
		l_mapPositions.Add(m_cornerPosition);
		m_mapButton.tweener.addPositionTrack(l_mapPositions, ZoodlesScreenFactory.FADE_SPEED);
		
		m_transitioning = false;
		m_activityPanelCanvas.canvasGroup.interactable = true;
		m_cornerProfileCanvas.canvasGroup.interactable = true;

		m_foregroundGafGroup.gameObject.SetActive (true);
	}
	
	private void onActivityToggleClicked(UIToggle p_toggle, bool p_isToggled)
	{
		if (m_transitioning || !p_isToggled)
			return;
		
		if (m_regionState == RegionState.Left)
		{
			m_regionState = RegionState.Right;
			
			RectTransform foreRectTransform = m_foreground.gameObject.GetComponent<RectTransform>();
			List<Vector3> l_posFore = new List<Vector3>();
			l_posFore.Add(m_foreground.transform.localPosition);
			l_posFore.Add(m_foreground.transform.localPosition - new Vector3(foreRectTransform.rect.width * FOREGROUND_SPEED, 0.0f, 0.0f));
			m_foreground.tweener.addPositionTrack(l_posFore, PARALLAX_TWEEN_TIME, onToRightRegionTweenFinished);
			
			RectTransform backRectTransform = m_background.gameObject.GetComponent<RectTransform>();
			List<Vector3> l_posBack = new List<Vector3>();
			l_posBack.Add(m_background.transform.localPosition);
			l_posBack.Add(m_background.transform.localPosition - new Vector3(backRectTransform.rect.width * BACKGROUND_SPEED, 0.0f, 0.0f));
			m_background.tweener.addPositionTrack(l_posBack, PARALLAX_TWEEN_TIME, null);
			
			List<Vector3> l_mapPositions = new List<Vector3>();
			l_mapPositions.Add(m_cornerPosition);
			l_mapPositions.Add(m_cornerPosition - new Vector3(200.0f, 0, 0));
			m_mapButton.tweener.addPositionTrack(l_mapPositions, 0.4f);
			
			m_transitioning = true;
			m_activityPanelCanvas.canvasGroup.interactable = false;
			m_activityPanelCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED);
			m_cornerProfileCanvas.canvasGroup.interactable = false;
			m_cornerProfileCanvas.tweener.addAlphaTrack(1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED);

			// Sean: vzw
			m_regionAppCanvas.canvasGroup.interactable = false;
			m_regionAppCanvas.active = false;
		}
		else
		{
//			_Debug.log("CurrentActivity: " + m_currentActivityCanvas);
			if (m_currentActivityCanvas != null)
				m_gameController.getUI().removeScreenImmediately(m_currentActivityCanvas);
			
			m_createActivity = m_nextActivity;
		}
	}
	
	private void onBackButtonClicked(UIButton p_button)
	{
		if (m_transitioning)
			return;
		
		if (m_regionState == RegionState.Right)
		{
			m_regionState = RegionState.Left;
			
			RectTransform foreRectTransform = m_foreground.gameObject.GetComponent<RectTransform>();
			List<Vector3> l_posFore = new List<Vector3>();
			l_posFore.Add(m_foreground.transform.localPosition);
			l_posFore.Add(m_foreground.transform.localPosition + new Vector3(foreRectTransform.rect.width * FOREGROUND_SPEED, 0.0f, 0.0f));
			m_foreground.tweener.addPositionTrack(l_posFore, PARALLAX_TWEEN_TIME, onToLeftRegionTweenFinished);
			
			RectTransform backRectTransform = m_background.gameObject.GetComponent<RectTransform>();
			List<Vector3> l_posBack = new List<Vector3>();
			l_posBack.Add(m_background.transform.localPosition);
			l_posBack.Add(m_background.transform.localPosition + new Vector3(backRectTransform.rect.width * BACKGROUND_SPEED, 0.0f, 0.0f));
			m_background.tweener.addPositionTrack(l_posBack, PARALLAX_TWEEN_TIME, null);

			List<Vector3> l_backPositions = new List<Vector3>();
			l_backPositions.Add(m_cornerPosition);
			l_backPositions.Add(m_cornerPosition + new Vector3(0, 200.0f, 0));
			m_backButton.tweener.addPositionTrack(l_backPositions, 0.4f);
			
			if (m_currentActivityCanvas != null)
				m_gameController.getUI().removeScreenImmediately(m_currentActivityCanvas);

			m_transitioning = true;
			
			ActivityPanelCanvas l_panel = m_activityPanelCanvas as ActivityPanelCanvas;
			l_panel.untoggleActivities();
			
			m_activityPanelCanvas.canvasGroup.interactable = false;
			m_activityPanelCanvas.tweener.addAlphaTrack(0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED);
			m_cornerProfileCanvas.canvasGroup.interactable = true;
			m_cornerProfileCanvas.tweener.addAlphaTrack(0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED);

			m_regionAppCanvas.active = true;
			m_regionAppCanvas.canvasGroup.interactable = true;

			m_foregroundGafGroup.gameObject.SetActive (true);
		}
	}	
	
	private void onMapButtonClicked(UIButton p_button)
	{
		m_subState = SubState.GO_MAP;
	}	
	
	private void onSpeechClick(UIButton p_button)
	{
		p_button.tweener.addAlphaTrack(1.0f, 0.0f, 1.0f);
		p_button.removeAllCallbacks();
	}
	
	private void onTransitionEnter(UICanvas p_canvas)
	{
		p_canvas.canvasGroup.interactable = false;
	}
	
	private void onTransitionExit(UICanvas p_canvas)
	{
		p_canvas.canvasGroup.interactable = true;
	}

	private void onProfileClick(UIButton p_button)
	{
		m_subState = SubState.GO_KIDPROFILE;
	}

	private void onQuitMessageButtonClick(UIButton p_button)
	{
		MessageOut ();
	}

	private void MessageIn()
	{
		m_gameController.getUI ().changeScreen ( m_messageCanvas, true );
		UIElement l_newPanel = m_messageCanvas.getView ("mainPanel");
		List<Vector3> l_pointListIn = new List<Vector3>();
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
	}

	private void MessageOut()
	{
		m_gameController.getUI ().changeScreen ( m_messageCanvas, false );
		UIElement l_newPanel = m_messageCanvas.getView ("mainPanel");
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( l_newPanel.transform.localPosition );
		l_pointListOut.Add( l_newPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListOut, 0f );
	}
	#endregion
	
	private void setInputEnabled(bool p_isEnabled)
	{
		m_regionLandingCanvas.setInputEnabled(p_isEnabled);
		m_activityPanelCanvas.setInputEnabled(p_isEnabled);
		m_regionBackgroundCanvas.setInputEnabled(p_isEnabled);
		m_cornerProfileCanvas.setInputEnabled(p_isEnabled);
		
		if (m_videoActivityCanvas != null)
			m_videoActivityCanvas.setInputEnabled(p_isEnabled);

		if (m_gameActivityCanvas != null)
			m_gameActivityCanvas.setInputEnabled(p_isEnabled);

		if (m_bookActivityCanvas != null)
			m_bookActivityCanvas.setInputEnabled(p_isEnabled);

		if (m_funActivityCanvas != null)
			m_funActivityCanvas.setInputEnabled(p_isEnabled);
	}
	
	private void _setupBookSwipeLists(List<object> p_contentList)
	{
		if (p_contentList.Count <= 0)
		{
			WebContentCache l_cache = m_gameController.game.user.contentCache;

			if( l_cache.loadBookFail )
			{
				UILabel l_bookInfo = m_bookActivityCanvas.getView("info") as UILabel;
				if (null != l_bookInfo)
				{
					l_bookInfo.active = true;
					l_bookInfo.text = Localization.getString(Localization.TXT_STATE_61_FAIL);
				}
				return;
			}
		}

//		if( ZoodleState.REGION_LANDING != int.Parse(m_gameController.stateName) )
//			return;

		m_bookViewList.Clear();
		m_bookFavoritesList.Clear ();

		int l_bookCount = 0;
		
		foreach (BookInfo l_info in p_contentList)
		{
			if (l_bookCount++ >= ZoodlesConstants.MAX_BOOK_CONTENT)
				continue;

			if (l_info.disposed)
			{
				l_info.reload();
			}

			Book l_book = (Book)SessionHandler.getInstance ().bookTable[l_info.bookId];

			if( l_book.isFavorite )
			{
				m_bookFavoritesList.Add(l_info);
			}

			if( l_book.isTop )
			{
				m_topBook = l_info;

				if( m_topBook.icon == null )
				{
					canSetImage = true;
				}
			}

			m_bookViewList.Add(l_info);
		}

		if( null != m_bookActivityCanvas )
		{
			UILabel l_bookInfo = m_bookActivityCanvas.getView("info") as UILabel;
			if (null != l_bookInfo)
			{
				if( m_bookViewList.Count <= 0 )
				{
					l_bookInfo.active = true;
					l_bookInfo.text = Localization.getString(Localization.TXT_14_LABEL_INFO);
				}
				else
				{
					l_bookInfo.active = false;
				}
			}
		}
	}

	// Sean: vzw

	private void _setupAppContentList()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR

		List<object> l_list = KidMode.getSystemApps();
		foreach (AppInfo l_app in l_list)
		{
			m_appList.Add(l_app);
		}
		#endif

		m_appSwipeList.setData(m_appList);
		m_appSwipeList.addClickListener("Prototype", onAppClicked);
	}
	// end vzw

	private void _setupWebContentList(List<object> p_contentList)
	{
		if (p_contentList.Count <= 0)
		{
			WebContentCache l_cache = m_gameController.game.user.contentCache;
			
			if( l_cache.loadWebContentFail )
			{
				UILabel l_videoInfo = m_videoActivityCanvas.getView("info") as UILabel;
				if (null != l_videoInfo)
				{
					l_videoInfo.active = true;
					l_videoInfo.text = Localization.getString(Localization.TXT_STATE_61_FAIL);
				}

				UILabel l_gameInfo = m_gameActivityCanvas.getView("info") as UILabel;
				if (null != l_gameInfo)
				{
					l_gameInfo.active = true;
					l_gameInfo.text = Localization.getString(Localization.TXT_STATE_61_FAIL);
				}
				return;
			}
		}

		m_videoViewList.Clear();
		m_videoFavoritesList.Clear();
		m_gameViewList.Clear();
		m_gameFavoritesList.Clear();

		int l_gameCount = 0;
		int l_videoCount = 0;

		#if UNITY_ANDROID && !UNITY_EDITOR
		string l_appListJson = PlayerPrefs.GetString( "addedAppList" );
		_Debug.log ( l_appListJson );
		ArrayList l_appNameList = MiniJSON.MiniJSON.jsonDecode( l_appListJson ) as ArrayList;
		if( null != l_appNameList )
		{
			List<System.Object> l_list = KidMode.getLocalApps();
			
			if( l_list != null && l_list.Count > 0)
			{
				foreach(AppInfo l_app in l_list)
				{
					if( l_appNameList.Count > 0 && l_appNameList.Contains(l_app.packageName) )
					{
						GameInfo l_game = new GameInfo(l_app);
						m_gameViewList.Add(l_game);
					}
				}
			}
		}
		l_gameCount += m_gameViewList.Count;
		#endif

		foreach (object o in p_contentList)
		{
			WebContent l_content = o as WebContent;
			
			if (l_content.category == WebContent.VIDEO_TYPE)
			{
				if (l_videoCount++ >= ZoodlesConstants.MAX_VIDEO_CONTENT)
					continue;
				
				string l_url = ZoodlesConstants.YOUTUBE_EMBEDED_URL + l_content.youtubeId +
					ZoodlesConstants.YOUTUBE_NO_RELATED_SUFFEX;
				
				WebViewInfo l_info = new WebViewInfo(null, l_content, l_url);
				
				if (l_content.favorite)
					m_videoFavoritesList.Add(l_info);

				m_videoViewList.Add(l_info);

				if (l_content.recommend)
					m_videoFeatured = l_info;
			}
			else if (l_content.category == WebContent.GAME_TYPE)
			{
				if (l_gameCount++ >= ZoodlesConstants.MAX_GAME_CONTENT)
					continue;
				
				string l_url = l_content.url;
				
				WebViewInfo l_info = new WebViewInfo(null, l_content, l_url);

				GameInfo l_game = new GameInfo(l_info);

				if (l_content.favorite)
					m_gameFavoritesList.Add(l_game);

				m_gameViewList.Add(l_game);

				if (l_content.recommend)
					m_gameFeatured = l_info;
			}
		}

		if( null != m_videoActivityCanvas )
		{
			UILabel l_videoInfo = m_videoActivityCanvas.getView("info") as UILabel;
			if (null != l_videoInfo)
			{
				if( m_videoViewList.Count <= 0 )
				{
					l_videoInfo.active = true;
					l_videoInfo.text = Localization.getString(Localization.TXT_11_LABEL_INFO);
				}
				else
				{
					l_videoInfo.active = false;
				}
			}

			if (null != m_videoFeatured)
			{
				UIButton l_featureButton = m_videoActivityCanvas.getView("featureOne") as UIButton;
				if (null != l_featureButton)
				{
					l_featureButton.addClickCallback(onFeatureVideoClicked);
					UIImage l_featureImage = l_featureButton.getView("featureOneIcon") as UIImage;
					if (null != l_featureImage)
					{
						m_videoFeatured.retriveIcon(setFeatureImageVideo);
					}
				}
			}
		}

		if( null != m_gameActivityCanvas )
		{
			UILabel l_gameInfo = m_gameActivityCanvas.getView("info") as UILabel;
			if (null != l_gameInfo)
			{
				if( m_gameViewList.Count <= 0 )
				{
					l_gameInfo.active = true;
					l_gameInfo.text = Localization.getString(Localization.TXT_12_LABEL_INFO);
				}
				else
				{
					l_gameInfo.active = false;
				}
			}

			if (null != m_gameFeatured)
			{
				UIButton l_featureButton = m_gameActivityCanvas.getView("featureOne") as UIButton;
				if (null != l_featureButton)
				{
					l_featureButton.addClickCallback(onFeatureGameClicked);
					UIImage l_featureImage = l_featureButton.getView("featureOneIcon") as UIImage;
					if (null != l_featureImage)
					{
						m_gameFeatured.retriveIcon(setFeatureImageGame);
					}
				}
			}
		}		
		// m_gameSwipeList.setData(m_gameViewList);
		// m_videoSwipeList.setData(m_videoViewList);
	}

	private void _setupDrawingList(List<object> p_contentList)
	{
		m_funViewList.Clear();
		m_funViewList.Add(new ActivityInfo(null));

		foreach (object o in p_contentList)
		{
			ActivityInfo l_info = new ActivityInfo(o as Drawing);
			m_funViewList.Add(l_info);
		}
	}

	private void _cleanUpTextures()
	{
		_cleanUpBookInfo(m_bookSwipeList);
		_cleanUpGameContent(m_gameSwipeList);
		_cleanUpWebContent(m_videoSwipeList);

		// Sean: vzw
		List<System.Object> data = m_appSwipeList.getData();
		foreach (System.Object o in data)
		{
			AppInfo l_info = o as AppInfo;
			l_info.dispose();
		}

		// end vzw
	}
	
	private void _cleanUpBookInfo(UISwipeList p_swipeList)
	{
		if (p_swipeList == null)
			return;
		
		List<System.Object> l_bookData = p_swipeList.getData();
		foreach (System.Object o in l_bookData)
		{
			BookInfo l_info = o as BookInfo;
			GameObject.Destroy(l_info.icon);
		}
	}

	private void _cleanUpGameContent(UISwipeList p_swipeList)
	{
		if (p_swipeList == null)
			return;
		
		List<System.Object> l_webData = p_swipeList.getData();
		foreach (System.Object o in l_webData)
		{
			GameInfo l_info = o as GameInfo;

			if( l_info.isWebView )
			{				
				GameObject.Destroy(l_info.webViewData.icon);
			}
			else
			{
				GameObject.Destroy(l_info.appData.appIcon);				
			}
		}
	}
	
	private void _cleanUpWebContent(UISwipeList p_swipeList)
	{
		if (p_swipeList == null)
			return;
		
		List<System.Object> l_webData = p_swipeList.getData();
		foreach (System.Object o in l_webData)
		{
			WebViewInfo l_info = o as WebViewInfo;
			GameObject.Destroy(l_info.icon);
		}
	}

	private void _oscillateLightsDown(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		p_element.tweener.addRotationTrack(-2.0f, 2.0f, 5.0f, _oscillateLightsUp);
	}

	private void _oscillateLightsUp(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		p_element.tweener.addRotationTrack(2.0f, -2.0f, 5.0f, _oscillateLightsDown);
	}

	// Sean: vzw
	protected UICanvas m_regionAppCanvas;
	private UISwipeList m_appSwipeList;
	private List<object> m_appList = new List<object>();

	// end vzw


	protected UICanvas 	m_regionLandingCanvas;
	protected UICanvas 	m_activityPanelCanvas;
	protected UICanvas 	m_regionBackgroundCanvas;
	protected UICanvas 	m_cornerProfileCanvas;
	
	protected UICanvas 	m_videoActivityCanvas;
	protected UICanvas	m_gameActivityCanvas;
	protected UICanvas	m_bookActivityCanvas;
	protected UICanvas	m_funActivityCanvas;

	protected UICanvas	m_messageCanvas;
	
	protected UICanvas  m_currentActivityCanvas;
	
	protected UIButton	m_backButton;
	protected UIButton 	m_mapButton;
	protected UIButton  m_speechBubble;
	protected UIButton 	m_profileButton;
	protected UIButton	m_quitMessageButton;
	
	protected UIToggle	m_videoButton;
	protected UIToggle 	m_gamesButton;
	protected UIToggle	m_booksButton;
	protected UIToggle 	m_activitiesbutton;
	
	protected UIToggle  m_favorateTab;
	protected UIToggle  m_allContentTab;
	
	protected UISwipeList m_videoSwipeList;
	protected UISwipeList m_videoFavorateSwipeList;
	protected UISwipeList m_gameSwipeList;
	protected UISwipeList m_gameFavorateSwipeList;
	protected UISwipeList m_bookSwipeList;
	protected UISwipeList m_bookFavorateSwipeList;
	
	protected UISwipeList m_funSwipeList;
	
	protected UIElement 	m_foreground;
	protected UIElement 	m_background;

	protected UIElement m_foregroundGafGroup;

	protected SubState m_subState = SubState.None;

	protected bool canSetImage = false;
	protected bool canSetWebContent = false;
	protected bool canSetBook = false;

	//added by joshua
	protected bool m_gotoKidsProfile = false;

	//private bool m_removeCornerProfile 	= false;
	protected bool m_transitioning 			= false;
	
	//private bool m_createVideoActivity	= false;
	//private bool m_createGameActivity	= false;
	
	protected ActivityType m_createActivity 	= ActivityType.None;
	protected ActivityType m_nextActivity 		= ActivityType.None;
	
	
	protected Vector3 m_cornerPosition;

	protected Vector3 m_backButtonPosition;

	protected RegionState m_regionState;
	
	protected List<object> m_videoViewList        = new List<object>();
	protected List<object> m_videoFavoritesList   = new List<object>();
	protected WebViewInfo  m_videoFeatured		  = null;
	protected List<object> m_gameViewList         = new List<object>();
	protected List<object> m_gameFavoritesList    = new List<object>();
	protected WebViewInfo  m_gameFeatured		  = null;
	protected List<object> m_bookViewList         = new List<object>();
	protected List<object> m_bookFavoritesList    = new List<object>();
	protected List<object> m_funViewList          = new List<object>();
	protected BookInfo m_topBook = null;

	protected List<AnimationTrigger> m_triggers	= new List<AnimationTrigger>();

	protected Dictionary<string, bool> m_requestStates		= new Dictionary<string, bool>();

	protected bool m_bookLoaded = false;
	protected bool m_linkLoaded = false;
}

public class AnimationTrigger
{
	public AnimationTrigger(UIButton p_button, UIMovieClip p_clip)
	{
		m_button = p_button;
		m_clip = p_clip;
		_init();
	}
	
	public void update(int p_alpha)
	{
		if (m_playing)
		{
			m_time += p_alpha;
			if (m_time > m_total)
			{
				m_button.addClickCallback(_trigger);
				m_clip.setSequence("idle", true);
				m_clip.loop = true;
				m_playing = false;
			}
		}
	}
	
	private void _init()
	{
		m_button.addClickCallback(_trigger);
	}
	
	private void _trigger(UIButton p_button)
	{
		m_button.removeClickCallback(_trigger);
		m_clip.setSequence("celebrate", true);
		m_clip.loop = false;
		m_total = (int)(m_clip.duration() * 1000.0f);
		m_time = 0;
		m_playing = true;
	}
	
	private UIButton m_button = null;
	private UIMovieClip m_clip = null;
	private int m_time = 0;
	private int m_total = 0;
	private bool m_playing = false;
}