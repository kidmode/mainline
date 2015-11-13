using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;


public class GameInfo : System.Object
{
	public WebViewInfo webViewData;
	public AppInfo appData;
	public bool isWebView = true;

	public GameInfo()
	{
		webViewData = new WebViewInfo();
		isWebView = true;
	}

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

public class WebViewInfoStatus
{
	public const int AddItem = 0;
	public const int FromLocal = 1;
	public const int FromGDrive = 2;
	public const int FromServer = 3;
}

public class WebViewInfo : System.Object
{
	public const string DEFAULT_URL = "https://www.youtube.com/embed/v53mhRXXT2g?rel=0";

	public delegate void RetriveIconHandler(Texture2D p_icon);
	
	public Texture2D    icon;
	public string       urlString;
	public WebContent   webData;
	//honda: new property
	public bool 		iconRequested;

	public int infoStatus;

	public WebViewInfo()
	{
		icon = null;
		urlString = null;
		webData = null;
		iconRequested = true;
		infoStatus = WebViewInfoStatus.AddItem;
		iconRequested = true;
	}

	public WebViewInfo(string name, string url, string contentType, int from)
	{
		icon = null;
		urlString = url;
		iconRequested = true;
		webData = new WebContent(name, url, contentType);
		infoStatus = from;
	}

	public WebViewInfo(Texture2D p_icon, WebContent p_content = null, string p_urlString = DEFAULT_URL)
	{
		icon 		= p_icon;
		urlString 	= p_urlString;
		webData     = p_content;
		//honda
		iconRequested = false;
		infoStatus = WebViewInfoStatus.FromServer;
	}

	public void requestIcon()
	{
		//honda: check icon in local folder or not. if YES, get it from local 
		string contentName = "link_" + webData.id + ".png";
		Texture2D texture = ImageCache.getCacheImage(contentName);
		if (texture != null)
		{
			icon = texture;
			if (m_retriveHandler != null)
			{
				m_retriveHandler(icon);
				m_retriveHandler = null;
			}
			iconRequested = true;
			return;
		}
		//end
		//honda comment: if can not get icon from local, do icon from server
		if( webData.icon != null )
		{
			RequestQueue l_queue = new RequestQueue();
			l_queue.add(new ImageRequest("icon", webData.icon, _requestIconComplete));
			l_queue.request(RequestType.RUSH);
			m_queue = l_queue;
			
			iconRequested = true;
		}
		else if( webData.iconMedium != null )
		{
			RequestQueue l_queue = new RequestQueue();
			l_queue.add(new ImageRequest("icon", webData.iconMedium, _requestIconComplete));
			l_queue.request(RequestType.RUSH);
			m_queue = l_queue;
			
			iconRequested = true;
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

			if (webData.category == WebContent.GAME_TYPE || 
			    webData.category == WebContent.VIDEO_TYPE)
			{
				string name = "link_" + webData.id + ".png";
				Debug.Log(name);
				ImageCache.saveCacheImage(name, icon);
			}

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
	private RequestQueue m_trialPlanQueue = null;
	private RequestQueue m_bookQueue = null;

	private int LAYER_GAME = 5;
	private int LAYER_MESSAGE = 6;
	private int LAYER_ERROR = 7;

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
		Fun,
		AppList
	}

	protected enum SubState
	{
		None,
		GO_MAP,
		GO_KIDPROFILE,
		GO_VIDEO,
		GO_GAME,
		GO_BOOKS,
		GO_PAINT,
		GO_APPLIST
	}


	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);

		m_gameController = p_gameController;
		
		m_regionState 	= RegionState.Left;
		m_transitioning = false;
		m_subState = SubState.None;
		m_bookLoaded = false;
		m_linkLoaded = false;

		//create jungle view
		_createViews();

		//set buttons for jungle view
		_setupElements(p_gameController);

		if (m_queue == null)
		{
			m_queue = new RequestQueue();
		}

		if (m_trialPlanQueue == null)
		{
			m_trialPlanQueue = new RequestQueue();
		}


		//honda: check timer
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
		//end
		//honda: current kid drawing list request
		SessionHandler.getInstance().drawingListRequest();
		//end

		//play background music
		SoundManager.getInstance().play("96", 0, 1, "", null, true);

		GAUtil.logScreen("RegionLandingScreen");


//		checkTrialEnd(p_gameController);

	}
	
	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		//set alpha transition from jungle to activity
		checkActivityScreen ();

		//honda: comment out because we remove feature toy box
		//honda: set featured book image
//		if( canSetImage )
//		{
//			if( m_topBook != null && m_topBook.icon != null && null != m_bookActivityCanvas )
//			{
//				UIButton l_featureButton = m_bookActivityCanvas.getView("featureOne") as UIButton;
//				if( l_featureButton != null)
//				{
//					UIImage l_featureImage = l_featureButton.getView("featureOneIcon") as UIImage;
//					l_featureImage.setTexture(m_topBook.icon);
//					canSetImage = false;
//				}
//			}
//		}

		//after complete games and video loading, set contents to WebContentCache
		if( canSetWebContent )
		{
			Debug.Log("~DebugMode~ canSetWebContent");
			canSetWebContent = false;
			_setupWebContentList(SessionHandler.getInstance().webContentList);
		}
		//after complete books loading, set contents to WebContentCache
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
//			case SubState:
//				_changeToPaintActivity(p_gameController);
//				break;
			}

			m_subState = SubState.None;
		}

		//honda comments
		//check whether complete games and videos loading from server or not
		checkIfLinksCacheLoaded(p_gameController);
		//check wheather complete books loading from server or not
		checkIfBooksCacheLoaded(p_gameController);
		//create video, game or book view depending on current activity
		_handleDynamicActivities();
	}

	public void checkIfLinksCacheLoaded(GameController p_gameController)
	{
		if (m_linkLoaded == false)
		{
			Debug.Log("~DebugMode~ checkIfLinksCacheLoaded");
			Game l_game = p_gameController.game;
			User l_user = l_game.user;
			WebContentCache l_cache = l_user.contentCache;
			if (l_cache.isFinishedLoadingWebContent)
			{
				Debug.Log("~DebugMode~ isFinishedLoadingWebContent");
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

		if (m_currentActivityCanvas != null)
			l_ui.removeScreenImmediately(m_currentActivityCanvas);

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

		if( m_foregroundGafGroup != null ){

			if( m_foregroundGafGroup.active){

//				Tweener tw = m_foregroundGafGroup.tweener;
//				
//				tw.addAlphaTrack( 0.0f, 1.0f, 1.0f );

				GAFMovieClip[] gafClips = m_foregroundGafGroup.gameObject.GetComponentsInChildren<GAFMovieClip>();
//
				for (int i = 0; i < gafClips.Length; i++) {
//
//					Debug.Log("gafClips " + gafClips[i].gameObject.name + "   alpha " + gafClips[i].renderer.material.HasProperty("_Alpha") );
//
//					//gafClips[i].renderer.material.
//
//					for (int matIndex = 0; matIndex < gafClips[i].renderer.materials.Length; matIndex++) {
//
//						if(gafClips[i].renderer.materials[matIndex].GetFloat("_Alpha") < 1.0f){
//							
//							gafClips[i].renderer.materials[matIndex].SetFloat("_Alpha", 1.0f);// = 1.0f;
//							
//						}
//
//					}
//
//
				}

			}

		}

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

		if( null != m_regionAppCanvas && null != m_regionAppCanvas.getView("mainPanel") )
		{
			
			if( !m_regionAppCanvas.getView("mainPanel").active){ //m_gameActivityCanvas.getView("mainPanel").alpha < 1.0f &&
				
				m_regionAppCanvas.getView("mainPanel").tweener.addAlphaTrack( 0.0f, 1.0f, 1.0f );
				
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
//		m_regionAppCanvas = l_ui.createScreen(UIScreen.REGION_APP, true, 4);
	}
	
	private void _setupElements(GameController p_gameController)
	{
		m_speechBubble = m_regionLandingCanvas.getView("speechBubble") as UIButton;

		// Sean: vzw
//		m_speechBubble.active = false;
//		m_appSwipeList = m_regionAppCanvas.getView("appScrollView") as UISwipeList;
//
//		this._setupAppContentList();
		// end vzw

		m_mapButton = m_regionLandingCanvas.getView("mapsButton") as UIButton;
		m_mapButton.addClickCallback(onMapButtonClicked);
		m_cornerPosition = m_mapButton.transform.localPosition;
		
		m_backButton = m_regionLandingCanvas.getView("backButton") as UIButton;
		m_backButton.addClickCallback(onBackButtonClicked);

//		m_backButton.transform.localPosition = new Vector3 (-427.2f, 196.9f, 0.0f);
		m_backButtonPosition = m_backButton.transform.localPosition;
		m_backButton.transform.localPosition += new Vector3(0, 200, 0);
	

		m_background = m_regionBackgroundCanvas.getView("background");
		m_foreground = m_regionLandingCanvas.getView("foreground");

		m_appListButton = m_activityPanelCanvas.getView("appListButton") as UIToggle;
		m_appListButton.addValueChangedCallback(appListCallBack);
		m_appListButton.addValueChangedCallback(onActivityToggleClicked);

		appListButtonLockImageIcon = m_activityPanelCanvas.getView("icon_lock") as UIImage;
		//appListCallBack

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

		checkAcitivityActivate();


		m_foregroundGafGroup = m_regionLandingCanvas.getView("gafGroup");
		// Sean: vzw
//		this._setupGafGroup(false);

		m_triggers.Clear();
		// Sean: vzw
//		if (false) {
			m_triggers.Add(new AnimationTrigger(m_regionLandingCanvas.getView("monkeyTrigger") as UIButton, m_regionLandingCanvas.getView("Monkey_Anim") as UIMovieClip));
			m_triggers.Add(new AnimationTrigger(m_regionLandingCanvas.getView("snakeTrigger") as UIButton, m_regionLandingCanvas.getView("Snake_Anim") as UIMovieClip));
			m_triggers.Add(new AnimationTrigger(m_regionLandingCanvas.getView("toucanTrigger") as UIButton, m_regionLandingCanvas.getView("Toucan_Anim") as UIMovieClip));
//		} 
		// vzw end

		UIButton l_butterfly = m_regionLandingCanvas.getView("Butterfly") as UIButton;
		List<Vector3> l_butterflyPosList = new List<Vector3>();
		l_butterflyPosList.Add(l_butterfly.transform.localPosition);
		l_butterflyPosList.Add(l_butterfly.transform.localPosition + new Vector3(1000, 0, 0));
		l_butterfly.tweener.addPositionTrack(l_butterflyPosList, 30.0f, null, Tweener.Style.Standard, true);

		_oscillateLightsDown(m_regionLandingCanvas.getView("light"), Tweener.TargetVar.Rotation);
	}

//	private void _setupGafGroup(bool active)
//	{
//		m_foregroundGafGroup.getView("monkeyTrigger").active = active;
//		m_foregroundGafGroup.getView("snakeTrigger").active = active;
//		m_foregroundGafGroup.getView("toucanTrigger").active = active;
//	}
	
	private void _handleDynamicActivities()
	{
		if (m_createActivity == ActivityType.AppList)
		{
			Debug.Log("spinner: 8. _handleDynamicActivities");
			
			m_regionAppCanvas = m_gameController.getUI().createScreen(UIScreen.REGION_APP, true, LAYER_GAME);

			m_appSwipeList = m_regionAppCanvas.getView("appScrollView") as UISwipeList;

			this._setupAppContentList();

			m_createActivity = ActivityType.None;
			m_currentActivityCanvas = m_regionAppCanvas;
			
//			m_videoSwipeList = m_videoActivityCanvas.getView("allContentScrollView") as UISwipeList;
//			m_videoSwipeList.setData(m_videoViewList);
//			m_videoSwipeList.addClickListener("Prototype", onVideoClicked);
//			
//			m_videoFavorateSwipeList = m_videoActivityCanvas.getView("favorateScrollView") as UISwipeList;
//			m_videoFavorateSwipeList.setData(m_videoFavoritesList);
//			m_videoFavorateSwipeList.addClickListener("Prototype", onVideoClicked);
//			
//			
//			//Get Scroll view updator and set its data so we know its data size
//			KidModeScrollViewUpdator viewUpdator = m_videoSwipeList.gameObject.GetComponent<KidModeScrollViewUpdator>();
//			viewUpdator.setContentDataSize(m_videoViewList.Count);
//			
//			UILabel l_videoInfo = m_videoActivityCanvas.getView("info") as UILabel;
//			l_videoInfo.active = (m_videoSwipeList.getData().Count <= 0);
//			
//			if( true == l_videoInfo.active && m_linkLoaded )
//			{
//				l_videoInfo.text = Localization.getString(Localization.TXT_11_LABEL_INFO);
//			}
//			
//			//honda: comment out because we remove feature toy box
//			//			if (m_videoFeatured != null)
//			//			{
//			//				m_videoFeatured.retriveIcon(setFeatureImageVideo);
//			//			}
//			
//			if( m_videoFavoritesList.Count <= 0 )
//			{
//				UILabel l_favorateInfoLabel = m_videoActivityCanvas.getView("favoriteInfo") as UILabel;
//				l_favorateInfoLabel.text 	= Localization.getString(Localization.TXT_11_LABEL_FAVORITE);
//			}
		}

		if (m_createActivity == ActivityType.Video)
		{
			Debug.Log("~DebugMode~ _handleDynamicActivities");

			m_videoActivityCanvas = m_gameController.getUI().createScreen(UIScreen.VIDEO_ACTIVITY, true, LAYER_GAME);
			m_createActivity = ActivityType.None;
			m_currentActivityCanvas = m_videoActivityCanvas;
			
			m_videoSwipeList = m_videoActivityCanvas.getView("allContentScrollView") as UISwipeList;
			updateVideoList();
//			m_videoSwipeList.setData(m_videoViewList);
//			m_videoSwipeList.addClickListener("RemoveButton", onRemoveButtonClicked);
//			m_videoSwipeList.addClickListener("Prototype", onVideoClicked);
			
			m_videoFavorateSwipeList = m_videoActivityCanvas.getView("favorateScrollView") as UISwipeList;
			m_videoFavorateSwipeList.setData(m_videoFavoritesList);
			m_videoFavorateSwipeList.addClickListener("Prototype", onVideoClicked);

			//Get Scroll view updator
//			KidModeScrollViewUpdator viewUpdator = m_videoSwipeList.gameObject.GetComponent<KidModeScrollViewUpdator>();
//			viewUpdator.setContentDataSize(m_videoViewList.Count);

			UILabel l_videoInfo = m_videoActivityCanvas.getView("info") as UILabel;
			l_videoInfo.active = (m_videoSwipeList.getData().Count <= 0);

			if( true == l_videoInfo.active && m_linkLoaded )
			{
				l_videoInfo.text = Localization.getString(Localization.TXT_11_LABEL_INFO);
			}

			//honda: comment out because we remove feature toy box
//			if (m_videoFeatured != null)
//			{
//				m_videoFeatured.retriveIcon(setFeatureImageVideo);
//			}

			if( m_videoFavoritesList.Count <= 0 )
			{
				UILabel l_favorateInfoLabel = m_videoActivityCanvas.getView("favoriteInfo") as UILabel;
				l_favorateInfoLabel.text 	= Localization.getString(Localization.TXT_11_LABEL_FAVORITE);
			}
		}
		
		if (m_createActivity == ActivityType.Game)
		{
			m_gameActivityCanvas = m_gameController.getUI().createScreen(UIScreen.GAME_ACTIVITY, true, LAYER_GAME);
			m_createActivity = ActivityType.None;
			m_currentActivityCanvas = m_gameActivityCanvas;

			m_gameSwipeList = m_gameActivityCanvas.getView("allContentScrollView") as UISwipeList;
			updateGameList();
//			m_gameSwipeList.setData(m_gameViewList);
//			m_gameSwipeList.addClickListener("Prototype", onGameClicked);

			//Get Scroll view updateor
//			KidModeScrollViewUpdator viewUpdator = m_gameSwipeList.gameObject.GetComponent<KidModeScrollViewUpdator>();
//			viewUpdator.setContentDataSize(m_gameViewList.Count);
			
			m_gameFavorateSwipeList = m_gameActivityCanvas.getView("favorateScrollView") as UISwipeList;
			m_gameFavorateSwipeList.setData(m_gameFavoritesList);
			m_gameFavorateSwipeList.addClickListener("Prototype", onGameClicked);

			UILabel l_gameInfo = m_gameActivityCanvas.getView("info") as UILabel;
			l_gameInfo.active = (m_gameSwipeList.getData().Count <= 0);

			if( true == l_gameInfo.active && m_linkLoaded )
			{
				l_gameInfo.text = Localization.getString(Localization.TXT_12_LABEL_INFO);
			}

			//honda: comment out because we remove feature toy box
//			if (m_gameFeatured != null)
//			{
//				m_gameFeatured.retriveIcon(setFeatureImageGame);
//			}

			if( m_gameFavoritesList.Count <= 0 )
			{
				UILabel l_favorateInfoLabel = m_gameActivityCanvas.getView("favoriteInfo") as UILabel;
				l_favorateInfoLabel.text 	= Localization.getString(Localization.TXT_12_LABEL_FAVORITE);
			}
		}
		
		if (m_createActivity == ActivityType.Books)
		{
			m_messageCanvas = m_gameController.getUI().createScreen(UIScreen.MESSAGE, false, LAYER_MESSAGE);
			m_bookActivityCanvas = m_gameController.getUI().createScreen(UIScreen.BOOK_ACTIVITY, true, LAYER_GAME);
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

			//honda: comment out because we remove feature toy box
//			UIButton l_featureButton = m_bookActivityCanvas.getView("featureOne") as UIButton;
//			l_featureButton.addClickCallback(onFeatureBookClicked);
//			if(m_topBook != null)
//			{
//				if( m_topBook.icon == null )
//				{
//					canSetImage = true;
//				}
//				else
//				{
//					UIImage l_featureImage = l_featureButton.getView("featureOneIcon") as UIImage;
//					l_featureImage.setTexture(m_topBook.icon);
//				}
//			}
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
			m_funActivityCanvas = m_gameController.getUI().createScreen(UIScreen.FUN_ACTIVITY, true, LAYER_GAME);
			m_createActivity = ActivityType.None;
			m_currentActivityCanvas = m_funActivityCanvas;
			
			m_funSwipeList = m_funActivityCanvas.getView("allContentScrollView") as UISwipeList;
//			m_funSwipeList.setData(m_funViewList);
			m_funSwipeList.addClickListener("Prototype", onFunActivityClicked);
			//m_createActivity 		= ActivityType.None;
			//m_gotoPaint 			= true;
		}
	}

	//honda: comment out because we remove feature toy box
//	private void setFeatureImageVideo(Texture2D p_icon)
//	{
//		UIButton l_featureButton = m_videoActivityCanvas.getView("featureOne") as UIButton;
//		if (null != l_featureButton)
//		{
//			l_featureButton.addClickCallback(onFeatureVideoClicked);
//			UIImage l_featureImage = l_featureButton.getView("featureOneIcon") as UIImage;
//			if (null != l_featureImage)
//			{
//				l_featureImage.setTexture(p_icon);
//			}
//		}
//	}

	//honda: comment out because we remove feature toy box
//	private void setFeatureImageGame(Texture2D p_icon)
//	{
//		UIButton l_featureButton = m_gameActivityCanvas.getView("featureOne") as UIButton;
//		if (null != l_featureButton)
//		{
//			l_featureButton.addClickCallback(onFeatureGameClicked);
//			UIImage l_featureImage = l_featureButton.getView("featureOneIcon") as UIImage;
//			if (null != l_featureImage)
//			{
//				l_featureImage.setTexture(p_icon);
//			}
//		}
//	}
	
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


	#region TrialCheck
	private void checkTrialEnd(GameController p_gameController){

		if(TrialTimeController.Instance.isTrialAccount()){

//			

			if (Application.internetReachability == NetworkReachability.NotReachable 
			    || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected()){

				if(TrialTimeController.Instance.isTrialTimeExpired()){
					createTrialEndMessage(p_gameController);
				}

			}else{

				getTrialTimeFromServer();

			}

		}

	}

	private void createTrialEndMessage(GameController p_gameController){

		UIManager l_ui = p_gameController.getUI();
		m_trialMessageCanvas = l_ui.createScreen(UIScreen.TRIAL_MESSAGE, false, 4);
		
		UIElement l_panel = m_trialMessageCanvas.getView( "mainPanel" );
		List<Vector3> l_pointListIn = new List<Vector3>();
		l_pointListIn.Add( l_panel.transform.localPosition );
		l_pointListIn.Add( l_panel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_panel.tweener.addPositionTrack( l_pointListIn, 0f );

		SessionHandler.getInstance().token.setCurrent(false);


		//===================
		//Set up screen
		UIButton m_subscriptionButton = m_trialMessageCanvas.getView ("subscriptionButton") 	as UIButton;
		UIButton m_continueButton	 = m_trialMessageCanvas.getView ("continueButton") 		as UIButton;
		UIButton m_exitButton		 = m_trialMessageCanvas.getView ("exitButton") 			as UIButton;
		UILabel m_messageText		 = m_trialMessageCanvas.getView ("messageText") 		as UILabel;
		UILabel m_continueText		 = m_trialMessageCanvas.getView ("continueText") 		as UILabel;
		
		m_subscriptionButton.addClickCallback ( onSubscriptionClick );
		m_continueButton.addClickCallback ( onContinueClick );
		m_exitButton.addClickCallback ( onContinueClick );

		m_messageText.text = Localization.getString (Localization.TXT_103_LABEL_EXPIRED);
		m_continueText.text = Localization.getString (Localization.TXT_103_BUTTON_NOTHANKS);


	}

	private void onSubscriptionClick(UIButton p_button)
	{
		if(string.Empty.Equals(SessionHandler.getInstance().PremiumJson))
		{
			Server.init (ZoodlesConstants.getHttpsHost());
			m_queue.reset ();
			m_queue.add (new GetPlanDetailsRequest(viewPremiumRequestComplete));
			m_queue.request ( RequestType.SEQUENCE );
		}
		else
		{
			m_gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName) );
			m_gameController.changeState( ZoodleState.VIEW_PREMIUM );

			UIManager l_ui = m_gameController.getUI();
			
			l_ui.removeScreen(m_trialMessageCanvas);
		}
	}

	private void viewPremiumRequestComplete(WWW p_response)
	{
		Server.init (ZoodlesConstants.getHost());
		if(null == p_response.error)
		{

			SessionHandler.getInstance ().PremiumJson = p_response.text;
			m_gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName) );
			m_gameController.changeState( ZoodleState.VIEW_PREMIUM );


			UIManager l_ui = m_gameController.getUI();
			
			l_ui.removeScreen(m_trialMessageCanvas);
		}
		else
		{
			setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_11_FAIL),Localization.getString(Localization.TXT_STATE_11_FAIL_DATA));
		}
	}


	private void onContinueClick(UIButton p_button)
	{

		UIManager l_ui = m_gameController.getUI();

		l_ui.removeScreen(m_trialMessageCanvas);

	}

	public void getTrialTimeFromServer(){
		
		m_trialPlanQueue.reset();
		m_trialPlanQueue.add( new PremiumDetailsRequest( onGetDetailsComplete ) );
		m_trialPlanQueue.request( RequestType.SEQUENCE );
		
	}
	
	private void onGetDetailsComplete(WWW p_response)
	{
		if( null == p_response.error )
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			l_data = (l_data["jsonResponse"] as Hashtable)["response"] as Hashtable;
			
			int trialDaysLeft = (int)((double)l_data["trial_days"]);

			if(trialDaysLeft <= 0){

				createTrialEndMessage(m_gameController);

			}
		}
	}
	#endregion


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

		Debug.Log("######");
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
			p_gameController.connectState (ZoodleState.PAINT_ACTIVITY, ZoodleState.REGION_FUN);
			p_gameController.changeState(ZoodleState.PAINT_ACTIVITY);
		}
		else
		{
			p_gameController.connectState(ZoodleState.PAINT_VIEW, ZoodleState.REGION_FUN);
			p_gameController.changeState(ZoodleState.PAINT_VIEW);
		}
	}

	private void appListCallBack(UIToggle p_element, bool p_toggles)
	{
		if (Application.internetReachability != NetworkReachability.NotReachable 
		    && !KidMode.isAirplaneModeOn() && KidMode.isWifiConnected()
		    && p_toggles == true)
		{
			m_nextActivity = ActivityType.AppList;
//			m_nextActivity = ActivityType.Video;
			SwrveComponent.Instance.SDK.NamedEvent("Tab.AppList");

			Debug.Log("     +++++    appListCallBack ");
		}
	}
	
	private void videoCallback(UIToggle p_element, bool p_toggles)
	{
		if (Application.internetReachability != NetworkReachability.NotReachable 
		    && !KidMode.isAirplaneModeOn() && KidMode.isWifiConnected()
		    && p_toggles == true)
		{
			m_nextActivity = ActivityType.Video;
			SwrveComponent.Instance.SDK.NamedEvent("Tab.VIDEO");
		}
	}
	
	private void gameCallback(UIToggle p_element, bool p_toggles)
	{
		if (Application.internetReachability != NetworkReachability.NotReachable 
		    && !KidMode.isAirplaneModeOn() && KidMode.isWifiConnected()
		    && p_toggles == true)
		{
			m_nextActivity = ActivityType.Game;
			SwrveComponent.Instance.SDK.NamedEvent("Tab.GAME");
		}
	}
	
	private void bookCallback(UIToggle p_element, bool p_toggles)
	{
		if (Application.internetReachability != NetworkReachability.NotReachable 
		    && !KidMode.isAirplaneModeOn() && KidMode.isWifiConnected()
		    && p_toggles == true)
		{
			m_nextActivity = ActivityType.Books;
			SwrveComponent.Instance.SDK.NamedEvent("Tab.BOOK");
		}
	}
	
	private void activityCallback(UIToggle p_element, bool p_toggles)
	{
		if (Application.internetReachability != NetworkReachability.NotReachable 
		    && !KidMode.isAirplaneModeOn() && KidMode.isWifiConnected()
		    && p_toggles == true)
		{
			m_nextActivity = ActivityType.Fun;
			SwrveComponent.Instance.SDK.NamedEvent("Tab.ACTIVITY");
		}
	}
	
	#region Callbacks
	
	private void onFunActivityClicked(UISwipeList p_list, UIButton p_listElement, System.Object p_data, int p_index)
	{
		//honda: if no internet, won't enter drawing section
		if (!showMsgIfNoInternet())
		{
			Drawing l_drawing = (p_data as ActivityInfo).drawing;
			SessionHandler.getInstance().currentDrawing = l_drawing;

			m_subState = SubState.GO_PAINT;
		}
	}

	private void onVideoClicked(UISwipeList p_list, UIButton p_listElement, System.Object p_data, int p_index)
	{
		//honda: if no internet, won't enter video section
		if (!showMsgIfNoInternet())
		{
			WebViewInfo webViewInfo = p_data as WebViewInfo;

			//honda: show add item ui on debug mode 
			if (webViewInfo.infoStatus == WebViewInfoStatus.AddItem)
			{
				m_gameController.getUI().createScreen(UIScreen.ADD_ITEMS_MANUALLY, false, 6);

				AddItemManuallyPopup popup = GameObject.FindWithTag("AddItemManuallyTag").GetComponent<AddItemManuallyPopup>() as AddItemManuallyPopup;
				popup.setPopupType("Video");
				if (popup != null)
				{
					popup.onClick += AddSingleItemButtonClicked;
					popup.onItemsFromGDriveCompleted += AddItemsFromGDrive;
				}

				return;
			}
			//end debug mode

			WebContent l_webContent = webViewInfo.webData;
			SessionHandler.getInstance().currentContent = l_webContent;
			
			m_subState = SubState.GO_VIDEO;
			
			Dictionary<string,string> payload = new Dictionary<string,string>() { {"VideoName", l_webContent.name}};
			SwrveComponent.Instance.SDK.NamedEvent("Video.CLICK",payload);
		}
	}

	//honda: comment out because we remove feature toy box
//	private void onFeatureVideoClicked(UIButton p_button)
//	{
//		SessionHandler.getInstance().currentContent = m_videoFeatured.webData;
//		
//		m_subState = SubState.GO_VIDEO;
//	}

	// Sean: vzw
	private void onAppClicked(UISwipeList p_list, UIButton p_listElement, System.Object p_data, int p_index)
	{
		AppInfo l_app = p_data as AppInfo;

		TimerController.Instance.pauseTimer();
		//tap native app(leave kidmode)
		m_gameController.game.IsNativeAppRunning = true;

		KidMode.startActivity(l_app.packageName);
	}
	// end vzw

	private void onGameClicked(UISwipeList p_list, UIButton p_listElement, System.Object p_data, int p_index)
	{
		//honda: if no internet, won't enter game section
		if(!showMsgIfNoInternet())
		{
			GameInfo l_game = p_data as GameInfo;

			//honda: show add item ui on debug mode 
			if (l_game.webViewData.infoStatus == WebViewInfoStatus.AddItem)
			{
				m_gameController.getUI().createScreen(UIScreen.ADD_ITEMS_MANUALLY, false, 6);
				
				AddItemManuallyPopup popup = GameObject.FindWithTag("AddItemManuallyTag").GetComponent<AddItemManuallyPopup>() as AddItemManuallyPopup;
				popup.setPopupType("Game");
				if (popup != null)
				{
					popup.onClick += AddSingleItemButtonClicked;
					popup.onItemsFromGDriveCompleted += AddItemsFromGDrive;
				}
				
				return;
			}
			//end debug mode

			
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
	}

	//honda: comment out because we remove feature toy box
//	private void onFeatureGameClicked(UIButton p_button)
//	{
//		SessionHandler.getInstance().currentContent = m_gameFeatured.webData;
//		
//		m_subState = SubState.GO_GAME;
//	}

	//honda: comment out because we remove feature toy box
//	private void onFeatureBookClicked(UIButton p_button)
//	{
//		if(m_topBook == null)
//		{
//			return;
//		}
//		if( m_topBook.bookState == BookState.NeedToBuy )
//		{
//			UILabel l_content = m_messageCanvas.getView("messageContent") as UILabel;
//			l_content.text = Localization.getString(Localization.TXT_STATE_REGIONBASE_ASK_BOOK);
//			MessageIn();
//		}
//		if( m_topBook.bookState == BookState.NotRecorded )
//		{
//			SessionHandler.getInstance().currentBook = SessionHandler.getInstance ().bookTable[m_topBook.bookId] as Book;
//			m_subState = SubState.GO_BOOKS;
//		}
//	}
	
	private void onBookClicked(UISwipeList p_list, UIButton p_listElement, System.Object p_data, int p_index)
	{
		//honda: if no internet, won't enter book section
		if(!showMsgIfNoInternet())
		{
			BookInfo l_bookInfo = p_data as BookInfo;

			if( l_bookInfo.bookState == BookState.Recorded )
			{
				SessionHandler.getInstance().currentBook = SessionHandler.getInstance ().bookTable[l_bookInfo.bookId] as Book;
				SessionHandler.getInstance().currentBookReading = SessionHandler.getInstance ().readingTable[l_bookInfo.bookReadingId] as BookReading;
				
				m_subState = SubState.GO_BOOKS;
				Dictionary<string,string> payload = new Dictionary<string,string>() { {"BookName", l_bookInfo.bookName}};
				SwrveComponent.Instance.SDK.NamedEvent("Book.CLICK.RECORDED",payload);
			}

			if( l_bookInfo.bookState == BookState.NeedToBuy )
			{
				UILabel l_content = m_messageCanvas.getView("messageContent") as UILabel;
				l_content.text = Localization.getString(Localization.TXT_STATE_REGIONBASE_ASK_BOOK);
				MessageIn();
				Dictionary<string,string> payload = new Dictionary<string,string>() { {"BookName", l_bookInfo.bookName}};
				SwrveComponent.Instance.SDK.NamedEvent("Book.CLICK.NEEDTOBUY",payload);
			}

			if( l_bookInfo.bookState == BookState.NotRecorded )
			{
				SessionHandler.getInstance().currentBook = SessionHandler.getInstance ().bookTable[l_bookInfo.bookId] as Book;
				m_subState = SubState.GO_BOOKS;
				Dictionary<string,string> payload = new Dictionary<string,string>() { {"BookName", l_bookInfo.bookName}};
				SwrveComponent.Instance.SDK.NamedEvent("Book.CLICK.NORECORDED",payload);
			}
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

		// Sean: vzw
//		m_regionAppCanvas.active = true;
//		m_regionAppCanvas.canvasGroup.interactable = true;
//		m_regionAppCanvas.tweener.addAlphaTrack(0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED, onToLeftRegionTweenAndAppListFadedIn);
		// end vzw
		m_transitioning = false;
		m_activityPanelCanvas.canvasGroup.interactable = true;
		m_cornerProfileCanvas.canvasGroup.interactable = true;



		m_foregroundGafGroup.gameObject.SetActive (true);


		//==============
		//Update corner profile since getting zoodles point could get done when the profile is up already
		//
		CornerProfileCanvas cornerProfile = (CornerProfileCanvas)m_cornerProfileCanvas;

		cornerProfile.refreshInfo ();

			//refreshInfo

		//=----------
		//Check Acitivity Panel
		checkAcitivityActivate();
	}

	// Sean: vzw
	private void onToLeftRegionTweenAndAppListFadedIn( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_transitioning = false;
		m_activityPanelCanvas.canvasGroup.interactable = true;
		m_cornerProfileCanvas.canvasGroup.interactable = true;
	}
	// end vzw


	private void checkAcitivityActivate(){

		Toggle appListToggle = m_appListButton.gameObject.GetComponent<Toggle>();

		if(SessionHandler.getInstance().token.isPremium()){

			appListToggle.interactable = true;

			appListButtonLockImageIcon.gameObject.SetActive(false);

		}else{

			if(TrialTimeController.Instance.isTrialAccount()){

				if(SessionHandler.getInstance().token.isCurrent()){

					appListToggle.interactable = true;

					appListButtonLockImageIcon.gameObject.SetActive(false);

					//---------------
					Color newColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);


					UIImage toggleImageBg = m_appListButton.getView("background") as UIImage;
					Image toggleUIImage = toggleImageBg.gameObject.GetComponent<Image>();
					toggleUIImage.color = newColor;
					
					UIImage toggleIconUIImage = m_appListButton.getView("icon") as UIImage;
					Image toggleIconImage = toggleIconUIImage.gameObject.GetComponent<Image>();
					toggleIconImage.color = newColor;

					//Set lebel color
					UILabel toggUILabel = m_appListButton.getView("titleLabel") as UILabel;
					Text toggleLabelText = toggUILabel.gameObject.GetComponent<Text>();
					toggleLabelText.color = newColor;



				}else{

					appListToggle.interactable = false;

					appListButtonLockImageIcon.gameObject.SetActive(true);

					//---------------
					Color newColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);

					UIImage toggleImageBg = m_appListButton.getView("background") as UIImage;
					Image toggleUIImage = toggleImageBg.gameObject.GetComponent<Image>();
					toggleUIImage.color = newColor;

					UIImage toggleIconUIImage = m_appListButton.getView("icon") as UIImage;
					Image toggleIconImage = toggleIconUIImage.gameObject.GetComponent<Image>();
					toggleIconImage.color = newColor;

					//Set lebel color
					UILabel toggUILabel = m_appListButton.getView("titleLabel") as UILabel;
					Text toggleLabelText = toggUILabel.gameObject.GetComponent<Text>();
					toggleLabelText.color = newColor;

				}

			}else{

				appListToggle.interactable = false;

				appListButtonLockImageIcon.gameObject.SetActive(true);

				//---------------
				Color newColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
				
				UIImage toggleImageBg = m_appListButton.getView("background") as UIImage;
				Image toggleUIImage = toggleImageBg.gameObject.GetComponent<Image>();
				toggleUIImage.color = newColor;
				
				UIImage toggleIconUIImage = m_appListButton.getView("icon") as UIImage;
				Image toggleIconImage = toggleIconUIImage.gameObject.GetComponent<Image>();
				toggleIconImage.color = newColor;

				//Set lebel color
				UILabel toggUILabel = m_appListButton.getView("titleLabel") as UILabel;
				Text toggleLabelText = toggUILabel.gameObject.GetComponent<Text>();
				toggleLabelText.color = newColor;

			}

		}





	}

	private void onActivityToggleClicked(UIToggle p_toggle, bool p_isToggled)
	{
		if ((Application.internetReachability == NetworkReachability.NotReachable 
		     || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected()) 
		    /*&& !p_toggle.name.Equals("booksButton")*/)
		{
			if (!p_isToggled)
				return;

			m_nextActivity = ActivityType.None;
			ActivityPanelCanvas l_panel = m_activityPanelCanvas as ActivityPanelCanvas;
			l_panel.untoggleActivities();

			m_gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6); 

//			setInputEnabled(false);
//			// Honda: Now, when you tap any tab except books tab, it will not do any transition.
//			// Sean: I just need it go back after it reaches the point
//			p_toggle.tweener.addAlphaTrack(1.0f, 1.0f, 0.31f, (UIElement p_element, Tweener.TargetVar p_targetVar) => {
//				ActivityPanelCanvas l_panel = m_activityPanelCanvas as ActivityPanelCanvas;
//				l_panel.untoggleActivities();
//				setInputEnabled(true);
//			});
			return;
		}

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
//			m_regionAppCanvas.canvasGroup.interactable = false;
//			m_regionAppCanvas.active = false;
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

			//honda: comment out because we remove feature toy box
//			if( l_book.isTop )
//			{
//				m_topBook = l_info;
//
//				if( m_topBook.icon == null )
//				{
//					canSetImage = true;
//				}
//			}

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
		List<object> l_list = KidMode.getSelectedAppsSorted();
		foreach (AppInfo l_app in l_list)
		{
			if(! m_appList.Contains(l_app))
				m_appList.Add(l_app);
		}
		#endif

			  
		#if UNITY_EDITOR
		for (int i = 0; i < 60; i++) {

			AppInfo info = new AppInfo();

			for (int j = 0; j < i; j++) {

				info.appName = info.appName + "t";

			}

			info.appName = info.appName + i;

//			info.appName ="test " + i;

			m_appList.Add( info );

		}
		#endif

		m_appSwipeList.setData(m_appList);
		m_appSwipeList.addClickListener("Prototype", onAppClicked);

		KidModeScrollViewUpdator scrollUpdator = m_appSwipeList.gameObject.GetComponent<KidModeScrollViewUpdator>();

		scrollUpdator.getAppButtonList();

	}
	// end vzw

//	private string getLocalContentNmae(WebContent l_content)
//	{
//		if (l_content.category == WebContent.GAME_TYPE || 
//		    l_content.category == WebContent.VIDEO_TYPE)
//		{
//			string file = "link_" + l_content.id + ".png";
//			return file;
//		}
//		return null;
//	}

	private void _setupWebContentList(List<object> p_contentList)
	{
		Debug.Log("~DebugMode~ _setupWebContentList = " + p_contentList.Count);
		//TODO: honda comment: p_contentList could be null and it will cause null reference issue
		if (p_contentList.Count <= 0)
		{
			WebContentCache l_cache = m_gameController.game.user.contentCache;
//			Debug.Log("spinner: 4_1. _setupWebContentList no WebContentLis");
			if( l_cache.loadWebContentFail )
			{
				if (m_videoActivityCanvas != null) {
					UILabel l_videoInfo = m_videoActivityCanvas.getView("info") as UILabel;
					if (null != l_videoInfo)
					{
						l_videoInfo.active = true;
						l_videoInfo.text = Localization.getString(Localization.TXT_STATE_61_FAIL);
					}
				}

				if (m_gameActivityCanvas != null) {
					UILabel l_gameInfo = m_gameActivityCanvas.getView("info") as UILabel;
					if (null != l_gameInfo)
					{
						l_gameInfo.active = true;
						l_gameInfo.text = Localization.getString(Localization.TXT_STATE_61_FAIL);
					}
				}
				return;
			}
		}

//		Debug.Log("spinner: 5. _setupWebContentList clear data");
		m_videoViewList.Clear();
		m_videoFavoritesList.Clear();
		m_gameViewList.Clear();
		m_gameFavoritesList.Clear();

//		#if UNITY_EDITOR
//		for (int i = 0; i < 300; i++) {
//			
//			m_gameViewList.Add( new GameInfo(new AppInfo() ) );
//			
//		}
//
////		m_gameViewList.Add(l_game);
//		#endif

		int l_gameCount = 0;
		int l_videoCount = 0;

		if (p_contentList != null)
		{
			foreach (object o in p_contentList)
			{
				WebContent l_content = o as WebContent;
				
				if (l_content.category == WebContent.VIDEO_TYPE)
				{
					if (l_videoCount++ >= ZoodlesConstants.MAX_VIDEO_CONTENT)
						continue;
					
					string l_url = ZoodlesConstants.YOUTUBE_EMBEDED_URL + l_content.youtubeId +
						ZoodlesConstants.YOUTUBE_NO_RELATED_SUFFEX;

//					string contentName = getLocalContentNmae(l_content);
//					Texture2D texture = ImageCache.getCacheImage(contentName);
					WebViewInfo l_info = new WebViewInfo(null, l_content, l_url);
					
					if (l_content.favorite)
						m_videoFavoritesList.Add(l_info);

					m_videoViewList.Add(l_info);

					//honda: comment out because we remove feature toy box
//					if (l_content.recommend)
//						m_videoFeatured = l_info;
				}
				else if (l_content.category == WebContent.GAME_TYPE)
				{
					if (l_gameCount++ >= ZoodlesConstants.MAX_GAME_CONTENT)
						continue;
					
					string l_url = l_content.url;

//					string contentName = getLocalContentNmae(l_content);
//					Texture2D texture = ImageCache.getCacheImage(contentName);
					WebViewInfo l_info = new WebViewInfo(null, l_content, l_url);

					GameInfo l_game = new GameInfo(l_info);

					if (l_content.favorite)
						m_gameFavoritesList.Add(l_game);


					m_gameViewList.Add(l_game);

					//honda: comment out because we remove feature toy box
//					if (l_content.recommend)
//						m_gameFeatured = l_info;
				}
			}
			Debug.Log("~DebugMode~ m_videoViewList = " + m_videoViewList.Count);
			Debug.Log("~DebugMode~ m_gameViewList = " + m_gameViewList.Count);
			if (m_videoSwipeList != null)
			{
				updateVideoList();
			}
			if (m_gameSwipeList != null)
			{
				updateGameList();
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

			//honda: comment out because we remove feature toy box
//			if (null != m_videoFeatured)
//			{
//				UIButton l_featureButton = m_videoActivityCanvas.getView("featureOne") as UIButton;
//				if (null != l_featureButton)
//				{
//					l_featureButton.addClickCallback(onFeatureVideoClicked);
//					UIImage l_featureImage = l_featureButton.getView("featureOneIcon") as UIImage;
//					if (null != l_featureImage)
//					{
//						m_videoFeatured.retriveIcon(setFeatureImageVideo);
//					}
//				}
//			}
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

			//honda: comment out because we remove feature toy box
//			if (null != m_gameFeatured)
//			{
//				UIButton l_featureButton = m_gameActivityCanvas.getView("featureOne") as UIButton;
//				if (null != l_featureButton)
//				{
//					l_featureButton.addClickCallback(onFeatureGameClicked);
//					UIImage l_featureImage = l_featureButton.getView("featureOneIcon") as UIImage;
//					if (null != l_featureImage)
//					{
//						m_gameFeatured.retriveIcon(setFeatureImageGame);
//					}
//				}
//			}
		}		
		// m_gameSwipeList.setData(m_gameViewList);
		// m_videoSwipeList.setData(m_videoViewList);
	}

	private void _setupDrawingList(List<object> p_contentList)
	{
		m_funViewList.Clear();
		m_funViewList.Add(new ActivityInfo(null));

		if (p_contentList != null)
		{
			foreach (object o in p_contentList)
			{
				ActivityInfo l_info = new ActivityInfo(o as Drawing);
				m_funViewList.Add(l_info);
			}
		}
	}

	private void _cleanUpTextures()
	{
		_cleanUpBookInfo(m_bookSwipeList);
		_cleanUpGameContent(m_gameSwipeList);
		_cleanUpWebContent(m_videoSwipeList);

		// Sean: vzw
//		List<System.Object> data = m_appSwipeList.getData();
//		foreach (System.Object o in data)
//		{
//			AppInfo l_info = o as AppInfo;
//			l_info.dispose();
//		}

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

	private bool showMsgIfNoInternet () 
	{
		if (Application.internetReachability == NetworkReachability.NotReachable 
		    || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected())
		{
			m_gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
			return true;
		}
		else
		{
			return false;
		}
	}
	
	//honda: debug mode
	private void AddSingleItemButtonClicked(List<object> list)
	{
		string contentType = "Video";
		foreach (List<string> item in list)
		{
			//name: item[0]
			//url: item[1]
			//content type: item[2]
			WebViewInfo newInfo = new WebViewInfo(item[0] as string, item[1] as string, item[2] as string, WebViewInfoStatus.FromLocal);
			if ((item[2] as string).Equals("Video"))
			{
				contentType = "Video";
				SessionHandler.getInstance().singleVideoList.Insert(0, newInfo);
			}
			else //if type == "Game"
			{
				contentType = "Game";
				GameInfo gameInfo = new GameInfo(newInfo);
				SessionHandler.getInstance().singleGameList.Insert(0, gameInfo);
			}
		}

		if (contentType.Equals("Video"))
		{
			updateVideoList();
		}
		else // if (contentType.Equals("Game"))
		{
			updateGameList();
		}
	}
	
	private void AddItemsFromGDrive(string jsonData)
	{
		Debug.Log("~DebugMode~ json: " + jsonData);

		ArrayList l_data = MiniJSON.MiniJSON.jsonDecode(jsonData) as ArrayList;
		Debug.Log("~DebugMode~ items count = " + l_data.Count);
		string contentType = "Video";
		Hashtable info = l_data[0] as Hashtable;
		if ((info["type"] as string).Equals("Video"))
		{
			contentType = "Video";
			SessionHandler.getInstance().multipleVideoList.Clear();
		}
		else
		{
			contentType = "Game";
			SessionHandler.getInstance().multipleGameList.Clear();
		}

		foreach (Hashtable item in l_data)
		{
			Debug.Log("~DebugMode~ type: " + item["type"] + " & name: " + item["name"] + "& url: " + item["url"]);

			WebViewInfo newInfo = new WebViewInfo(item["name"] as string, item["url"] as string, item["type"] as string, WebViewInfoStatus.FromGDrive);
			if ((item["type"] as string).Equals("Video"))
			{
				SessionHandler.getInstance().multipleVideoList.Add(newInfo);
			}
			else //if type == "Game"
			{
				GameInfo gameInfo = new GameInfo(newInfo);
				SessionHandler.getInstance().multipleGameList.Add(gameInfo);
			}
		}

		if (contentType.Equals("Video"))
		{
			Debug.Log("~DebugMode~ video list updated");
			updateVideoList();
		}
		else // if (contentType.Equals("Game"))
		{
			updateGameList();
		}
	}

	private void updateVideoList()
	{
		List<object> videoList  = new List<object>();

		videoList.Add(new WebViewInfo());

		//add single item list to video list
		foreach (WebViewInfo info in SessionHandler.getInstance().singleVideoList)
		{
			videoList.Add(info);
		}
		Debug.Log("~DebugMode~ single video list: " + SessionHandler.getInstance().singleVideoList.Count);
		//add multiple item list from google drive to video list
		foreach (WebViewInfo info in SessionHandler.getInstance().multipleVideoList)
		{
			videoList.Add(info);
		}
		Debug.Log("~DebugMode~ multiple video list: " + SessionHandler.getInstance().multipleVideoList.Count);
		//add original list from server to video list
		foreach (WebViewInfo info in m_videoViewList)
		{
			videoList.Add(info);
		}
		Debug.Log("~DebugMode~ video view list: " + m_videoViewList.Count);
		m_videoSwipeList.setData(videoList);
		m_videoSwipeList.addClickListener("Prototype", onVideoClicked);

		KidModeScrollViewUpdator viewUpdator = m_videoSwipeList.gameObject.GetComponent<KidModeScrollViewUpdator>();
		viewUpdator.setContentDataSize(m_videoViewList.Count);
	}

	private void updateGameList()
	{
		List<object> gameList  = new List<object>();
		
		gameList.Add(new GameInfo());
		
		//add single item list to video list
		foreach (GameInfo info in SessionHandler.getInstance().singleGameList)
		{
			gameList.Add(info);
		}
		Debug.Log("~DebugMode~ single game list: " + SessionHandler.getInstance().singleGameList.Count);
		//add multiple item list from google drive to video list
		foreach (GameInfo info in SessionHandler.getInstance().multipleGameList)
		{
			gameList.Add(info);
		}
		Debug.Log("~DebugMode~ multiple game list: " + SessionHandler.getInstance().multipleGameList.Count);
		//add original list from server to video list
		foreach (GameInfo info in m_gameViewList)
		{
			gameList.Add(info);
		}
		Debug.Log("~DebugMode~ game view list: " + m_gameViewList.Count);
		m_gameSwipeList.setData(gameList);
		m_gameSwipeList.addClickListener("Prototype", onGameClicked);
		
		KidModeScrollViewUpdator viewUpdator = m_gameSwipeList.gameObject.GetComponent<KidModeScrollViewUpdator>();
		viewUpdator.setContentDataSize(m_gameViewList.Count);
	}

	private void onRemoveButtonClicked(UISwipeList p_list, UIButton p_listElement, System.Object p_data, int p_index)
	{
		Debug.Log("remove button clicked");
		WebViewInfo info = p_data as WebViewInfo;
		bool isItemRemoved = false;
		string contentType = "Video";
		if (info.webData.gameType == WebContent.LINK_YOUTUBE)
		{
			contentType = "Video";
			if (info.infoStatus == WebViewInfoStatus.FromLocal)
			{
				int index = getListIndex(SessionHandler.getInstance().singleVideoList, info);
				Debug.Log("item index to be deleted on single video list: " + index);
				if (index >= 0)
				{
					SessionHandler.getInstance().singleVideoList.RemoveAt(index);
					isItemRemoved = true;
				}
			}
			else if (info.infoStatus == WebViewInfoStatus.FromGDrive)
			{
				int index = getListIndex(SessionHandler.getInstance().multipleVideoList, info);
				Debug.Log("item index to be deleted on multipla video list: " + index);
				if (index >= 0)
				{
					SessionHandler.getInstance().multipleVideoList.RemoveAt(index);
					isItemRemoved = true;
				}
			}
		}
		else // if info.webData.gameType == WebContent.LINK_HTML
		{
			contentType = "Game";
			if (info.infoStatus == WebViewInfoStatus.FromLocal)
			{
				int index = getListIndex(SessionHandler.getInstance().singleGameList, info);
				Debug.Log("item index to be deleted on single game list: " + index);
				if (index >= 0)
				{
					SessionHandler.getInstance().singleGameList.RemoveAt(index);
					isItemRemoved = true;
				}
			}
			else if (info.infoStatus == WebViewInfoStatus.FromGDrive)
			{
				int index = getListIndex(SessionHandler.getInstance().multipleGameList, info);
				Debug.Log("item index to be deleted on multipla game list: " + index);
				if (index >= 0)
				{
					SessionHandler.getInstance().multipleGameList.RemoveAt(index);
					isItemRemoved = true;
				}
			}
		}

		if (isItemRemoved)
		{
			if (contentType.Equals("Video"))
			{
				updateVideoList();
			}
			else // if (contentType.Equals("Game"))
			{
				updateGameList();
			}
		}
	}

	private int getListIndex(List<object> list, WebViewInfo currentInfo)
	{
		int index = 0;
		bool isFound = false;
		foreach (WebViewInfo info in list)
		{
			if (currentInfo.webData.name != string.Empty && 
			    currentInfo.webData.name.Equals(info.webData.name))
			{

				isFound = true;
				break;
			}
			index++;
		}

		if (isFound)
			return index;
		else
			return -1;
	}
	//end debug mode

	// Sean: vzw
	protected UICanvas m_regionAppCanvas;
	private UISwipeList m_appSwipeList;
	private List<object> m_appList = new List<object>();

	// end vzw

	//Kevin Add new app list Canvas , there is already m_regionAppCanvas but make sure there is no problems so addding a new one
	protected UICanvas m_appListCanvas;
	//Trial time End

	protected UIImage appListButtonLockImageIcon;

	protected UICanvas 	m_regionLandingCanvas;
	protected UICanvas 	m_activityPanelCanvas;
	protected UICanvas 	m_regionBackgroundCanvas;
	protected UICanvas 	m_cornerProfileCanvas;
	
	protected UICanvas 	m_videoActivityCanvas;
	protected UICanvas	m_gameActivityCanvas;
	protected UICanvas	m_bookActivityCanvas;
	protected UICanvas	m_funActivityCanvas;

	protected UICanvas	m_messageCanvas;
	protected UICanvas  m_trialMessageCanvas;

	
	protected UICanvas  m_currentActivityCanvas;
	
	protected UIButton	m_backButton;
	protected UIButton 	m_mapButton;
	protected UIButton  m_speechBubble;
	protected UIButton 	m_profileButton;
	protected UIButton	m_quitMessageButton;

	protected UIToggle  m_appListButton;
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

	//honda: use them for debug app(add videos or games manually)
//	protected List<object> m_singleVideoList      = new List<object>();
//	protected List<object> m_multipleVideoList    = new List<object>();
//	protected List<object> m_singleGameList       = new List<object>();
//	protected List<object> m_multipleGameList     = new List<object>();
	//end
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
