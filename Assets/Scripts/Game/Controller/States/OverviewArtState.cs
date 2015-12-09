using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverviewArtState : GameState {
	
	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);


		List<Drawing> l_list = SessionHandler.getInstance ().drawingList;	
		
		//=================
		//Write all drawing infos to FunActivityInfo List
		funActivityList = new ArrayList();
		if(l_list != null){
			for(int l_i = 0; l_i < l_list.Count; l_i++)
			{
				
				Drawing l_drawing = l_list[l_i];
				
				ActivityInfo info = new ActivityInfo(l_drawing);
				
				funActivityList.Add(info);
				
			}
		}

		
		m_uiManager = m_gameController.getUI();
		m_requestQueue = new RequestQueue ();
		
		_setupScreen( p_gameController );
		_setupElment();

		SessionHandler.getInstance().onUpdateDrawingList += onSessionDrawingListUpdated;
	}


	public void onSessionDrawingListUpdated(){

		Debug.LogWarning("    onSessionDrawingListUpdated   " );

		for(int l_i = 0; l_i < l_canvasList.Count; l_i++)
		{
			UIButton l_element = l_canvasList[l_i] as UIButton;
			//			Drawing l_drawing = l_list[l_i];
			
			//get activity info again so I could request Image only; thats the only it will work so . .. . . .. .
			ActivityInfo info  = funActivityList[l_i] as ActivityInfo;
			
			info.forceRequestIcon();
			
		}

	}

	public override void update (GameController p_gameController, int p_time)
	{
		base.update (p_gameController, p_time);

		if(isLoadDrawing)
		{
			if(null != SessionHandler.getInstance().drawingList)
			{
				isLoadDrawing = false;

				if( SessionHandler.getInstance().drawingList.Count > 0 )
				{					
					_setupArtGalleryCanvas();
				}
				else
				{
					UILabel l_loading = m_artGalleryCanvas.getView ("loadingText") as UILabel;
					l_loading.text = Localization.getString(Localization.TXT_STATE_48_EMPTY);
				}
			}
		}




		for(int l_i = 0; l_i < funActivityList.Count; l_i++)
		{

			if(l_i < l_canvasList.Count){

				UIButton l_element = l_canvasList[l_i] as UIButton;
	//			Drawing l_drawing = l_list[l_i];
				
				//get activity info again so I could request Image only; thats the only it will work so . .. . . .. .
				ActivityInfo info  = funActivityList[l_i] as ActivityInfo;
				
	//			Debug.LogWarning(" l i " + l_i + "    info  " + info.icon); 
				
				if( null != info.icon )
				{
					UIImage l_image = l_element.getView("artImage") as UIImage;
					l_image.setTexture(info.icon);

				}
			}

		}

//		Debug.Log("   SessionHandler.getInstance().IsDrawingUpdated " + SessionHandler.getInstance().IsDrawingUpdated);
//
//		List<Drawing> l_list = SessionHandler.getInstance ().drawingList;
//		for(int l_i = 0; l_i < l_canvasList.Count; l_i++)
//		{
//			UIButton l_element = l_canvasList[l_i] as UIButton;
//			Drawing l_drawing = l_list[l_i];
//			
//			
//			if(null == l_drawing.largeIcon){
////				downLoadDrawing(l_element,l_drawing);
//			}else
//			{
//				UIImage l_image = l_element.getView("artImage") as UIImage;
//				l_image.setTexture(l_drawing.largeIcon);
//			}
//			l_element.active = true;
//			l_element.addClickCallback( onArtButtonClick );
//		}

	}
	
	public override void exit (GameController p_gameController)
	{
		base.exit (p_gameController);
		
		m_uiManager.removeScreen( UIScreen.DASHBOARD_CONTROLLER );
		m_uiManager.removeScreen( UIScreen.DASHBOARD_COMMON );
		m_uiManager.removeScreen( UIScreen.LEFT_MENU );
		m_uiManager.removeScreen( UIScreen.ART_GALLERY );
		m_uiManager.removeScreen( UIScreen.ART_LIST );
		m_uiManager.removeScreen( UIScreen.COMMON_DIALOG );

		SessionHandler.getInstance().onUpdateDrawingList -= onSessionDrawingListUpdated;
	}
	
	//----------------- Private Implementation -------------------
	
	private void _setupScreen( GameController p_gameController )
	{
		m_commonDialog = m_uiManager.createScreen( UIScreen.COMMON_DIALOG, true, 5 )  as CommonDialogCanvas;
	
		m_commonDialog.setUIManager (p_gameController.getUI());

		m_artListCanvas 	= m_uiManager.createScreen( UIScreen.ART_LIST, false, 4 );

		m_leftMenuCanvas = m_uiManager.createScreen( UIScreen.LEFT_MENU, true, 3 )  as LeftMenuCanvas;;
		
		m_dashboardControllerCanvas = m_uiManager.createScreen( UIScreen.DASHBOARD_CONTROLLER, false, 2 ) as DashBoardControllerCanvas;

		m_artGalleryCanvas 	= m_uiManager.createScreen( UIScreen.ART_GALLERY, true, 1 );
		
		m_dashboardCommonCanvas = m_uiManager.createScreen( UIScreen.DASHBOARD_COMMON, true, 0 );
	}
	
	private void _setupElment()
	{
		m_leftButton = m_dashboardControllerCanvas.getView( "leftButton" ) as UIButton;
		m_leftButton.addClickCallback( onLeftButtonClick );

		m_helpButton = m_artGalleryCanvas.getView ("helpButton") as UIButton;
		m_helpButton.addClickCallback (onHelpButtonClick);

		m_rightButton = m_dashboardControllerCanvas.getView( "rightButton" ) as UIButton;
		m_rightButton.addClickCallback( onRightButtonClick );

		m_dashboardControllerCanvas.setupDotList( 6 );
		m_dashboardControllerCanvas.setCurrentIndex( 5 );
		
		UIElement l_newPanel = m_artGalleryCanvas.getView ("mainPanel");
		List<Vector3> l_pointListIn = new List<Vector3>();
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 830, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, 0.5f);
		
		m_menu = m_leftMenuCanvas.getView ("LeftMenu") as UIElement;
		m_leftSideMenuButton = m_dashboardCommonCanvas.getView ("menuButton") as UIButton;
		m_leftSideMenuButton.addClickCallback (toShowMenu);
		m_showProfileButton = m_menu.getView ("profileButton") as UIButton;
		m_showProfileButton.addClickCallback (toShowAllChilren);
		
		m_closeLeftMenuButton = m_leftMenuCanvas.getView ("closeButton") as UIButton;
		m_closeLeftMenuButton.addClickCallback (onCloseMenu);
		m_childModeButton = m_dashboardCommonCanvas.getView ("childModelButton") as UIButton;
		m_childModeButton.addClickCallback (toChildMode);
		
		m_settingButton = m_leftMenuCanvas.getView ("settingButton") as UIButton;
		m_settingButton.addClickCallback (toSettingScreen);

		m_appsButton = m_dashboardCommonCanvas.getView ("appsButton") as UIButton;
		m_appsButton.addClickCallback(goToAddApps);

		m_overviewButton = m_dashboardCommonCanvas.getView ("overviewButton") as UIButton;
		m_overviewButton.enabled = false;
		
		m_controlsButton = m_dashboardCommonCanvas.getView ("controlButton") as UIButton;
		m_controlsButton.addClickCallback (goToControls);
		
		m_statChartButton = m_dashboardCommonCanvas.getView ("starButton") as UIButton;
		m_statChartButton.addClickCallback (goToStarChart);
		
		m_childrenList = m_leftMenuCanvas.getView ("childSwipeList") as UISwipeList;
		m_childrenList.addClickListener ("Prototype",onSelectThisChild);

		m_moreArtButton = m_artGalleryCanvas.getView( "artListButton" ) as UIButton;
		m_moreArtButton.addClickCallback( onMoreArtButtonClick );
		m_moreArtButton.active = false;

		m_exitArtListButton = m_artListCanvas.getView( "exitButton" ) as UIButton;
		m_exitArtListButton.addClickCallback( onExitArtListButtonClick );

		//Create an empty list for set up swipeList.
		List<System.Object> l_list = new List<System.Object>();
		m_drawingList = m_artListCanvas.getView ("artSwipeList") as UISwipeList;
		m_drawingList.setData (l_list);

		m_tryPremiumButton = m_leftMenuCanvas.getView ("premiumButton") as UIButton;
		m_buyGemsButton = m_leftMenuCanvas.getView ("buyGemsButton") as UIButton;
		m_tryPremiumButton.addClickCallback (toPremiumScreen);
		m_buyGemsButton.addClickCallback (toBuyGemsScreen);


//		loadDrawingList

		if( null != SessionHandler.getInstance().drawingList)
		{
			if( SessionHandler.getInstance().drawingList.Count > 0 )
			{					
				_setupArtGalleryCanvas();
			}
			else
			{
				UILabel l_loading = m_artGalleryCanvas.getView ("loadingText") as UILabel;
				l_loading.text = Localization.getString(Localization.TXT_STATE_48_EMPTY);
			}
		}
		else
		{
			loadDrawingList();
		}
	}


	private void onHelpButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks ();
		m_commonDialog.setOriginalPosition ();
		UIButton l_closeButton = m_commonDialog.getView ("closeMark") as UIButton;
		
		UILabel l_titleLabel = m_commonDialog.getView ("dialogText") as UILabel;
		UILabel l_contentLabel = m_commonDialog.getView ("contentText") as UILabel;
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_48_HELP_TITLE);
		l_contentLabel.text = Localization.getString(Localization.TXT_STATE_48_HELP_CONTENT);

		l_closeButton.addClickCallback (onCloseDialogButtonClick);
	}
	
	private void onCloseDialogButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		m_commonDialog.setOutPosition ();
		m_helpButton.addClickCallback (onHelpButtonClick);
	}
	
	private void onLeftButtonClick( UIButton p_button )
	{
		if (checkInternet())
		{
//			m_gameController.changeState (ZoodleState.OVERVIEW_BOOK);
			m_gameController.changeState (ZoodleState.OVERVIEW_READING);
		}
	}
	
	private void onRightButtonClick( UIButton p_button )
	{
		return;
	}	

	private void goToAddApps( UIButton p_button )
	{
		m_gameController.changeState (ZoodleState.CONTROL_APP);
	}
	
	private void goToControls( UIButton p_button )
	{
		if (checkInternet())
		{
			m_gameController.changeState (ZoodleState.CONTROL_SUBJECT);
		}
	}

	private bool checkInternet()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable 
		    || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected())
		{
			m_gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
			
			ErrorMessage error = GameObject.FindWithTag("ErrorMessageTag").GetComponent<ErrorMessage>() as ErrorMessage;
			if (error != null)
				error.onClick += onClickExit;
			
			return false;
		}
		return true;
	}

	private void onClickExit()
	{
		ErrorMessage error = GameObject.FindWithTag("ErrorMessageTag").GetComponent<ErrorMessage>() as ErrorMessage;
		error.onClick -= onClickExit;;
		m_gameController.changeState (ZoodleState.CONTROL_APP);
	}
	
	private void goToStarChart( UIButton p_button )
	{
		m_gameController.changeState (ZoodleState.DASHBOARD_STAR_CHART);
	}
	
	private void goToChildLock(UIButton p_button)
	{
		m_gameController.changeState (ZoodleState.CHILD_LOCK_STATE);
	}
	
	private void toSettingScreen(UIButton p_button)
	{
		if (checkInternet())
		{
			p_button.removeClickCallback (toSettingScreen);
			m_gameController.changeState (ZoodleState.SETTING_STATE);
		}
	}
	
	private void onCloseMenu(UIButton p_button)
	{
		if(canMoveLeftMenu)
		{
			m_uiManager.changeScreen(UIScreen.LEFT_MENU,false);
			Vector3 l_position = m_menu.transform.localPosition;
			
			List<Vector3> l_posList = new List<Vector3> ();
			l_posList.Add (l_position);
			l_posList.Add (l_position + new Vector3 (-200, 0, 0));
			m_menu.tweener.addPositionTrack (l_posList, m_leftMenuCanvas.displaySpeed, onCloseMenuTweenFinished, Tweener.Style.QuadOutReverse);
			canMoveLeftMenu = false;
		}
	}
	
	private void onCloseMenuTweenFinished( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		canMoveLeftMenu = true;
	}
	
	private void toShowMenuTweenFinished( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		canMoveLeftMenu = true;
	}
	
	private void toChildMode(UIButton p_button)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if (KidMode.isHomeLauncherKidMode ()) {
			
			m_gameController.changeState (ZoodleState.PROFILE_SELECTION);
			
		} else {
			
			KidMode.enablePluginComponent();
			
			KidMode.openLauncherSelector ();
			
		}
		#else
		m_gameController.changeState (ZoodleState.PROFILE_SELECTION);
		#endif
	}
	
	private void toShowMenu(UIButton p_button)
	{
		if(canMoveLeftMenu && checkInternet())
		{
			m_uiManager.changeScreen(UIScreen.LEFT_MENU,true);
			Vector3 l_position = m_menu.transform.localPosition;
			List<Vector3> l_posList = new List<Vector3> ();
			l_posList.Add (l_position);
			l_posList.Add (l_position + new Vector3 (200, 0, 0));
			m_menu.tweener.addPositionTrack (l_posList, m_leftMenuCanvas.displaySpeed, toShowMenuTweenFinished, Tweener.Style.QuadOutReverse);
			canMoveLeftMenu = false;
		}
	}
	
	private void toShowAllChilren(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		m_leftMenuCanvas.showKids (addButtonClickCall);
	}
	
	private void addButtonClickCall( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_showProfileButton.addClickCallback (toShowAllChilren);
	}
	
	private void onSelectThisChild(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		if (checkInternet() == false)
			return;

		Kid l_kid = p_data as Kid;
		if (Localization.getString(Localization.TXT_86_BUTTON_ADD_CHILD).Equals (l_kid.name))
		{
			SessionHandler.getInstance().CreateChild = true;
			m_gameController.connectState(ZoodleState.CREATE_CHILD_NEW,int.Parse(m_gameController.stateName));
			m_gameController.changeState (ZoodleState.CREATE_CHILD_NEW);
		}
		else
		{
			List<Kid> l_kidList = SessionHandler.getInstance().kidList;
			SessionHandler.getInstance().currentKid = l_kidList[p_index-1];
			m_gameController.changeState(ZoodleState.OVERVIEW_INFO);
		}
	}

	private void onMoreArtButtonClick( UIButton p_button )
	{
		m_uiManager.changeScreen ( m_artListCanvas, true );
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_artListCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f,onArtPositionTrackFinish );
	}

	private void onArtPositionTrackFinish(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		m_requestQueue.reset ();
		if(null == m_drawingList)
			m_drawingList = m_artListCanvas.getView("artSwipeList") as UISwipeList;
		List<Drawing> l_drawingList = SessionHandler.getInstance ().drawingList;
		if(null != l_drawingList)
		{
			List<object> infoData = new List<object>();
			int l_count = l_drawingList.Count;
			for(int l_i =0; l_i < l_count; l_i++ )
			{
				infoData.Add(l_drawingList[l_i]);
				if(null == l_drawingList[l_i].largeIcon)
					downLoadDrawing(p_element, l_drawingList[l_i]);
			}


//			m_funViewList.Clear();
//			m_funViewList.Add(new ActivityInfo(null));
//			
//			if (l_drawingList != null)
//			{
//				foreach (Drawing drawing in l_drawingList)
//				{
//					ActivityInfo l_info = new ActivityInfo(drawing);
//					m_funViewList.Add(l_info);
//				}
//			}


			m_requestQueue.request ();
			m_drawingList.setData( infoData );
			m_drawingList.setDrawFunction( onDrawingListDraw );
			m_drawingList.redraw();
//			m_drawingList.removeClickListener( "Prototype", onArtClick );
			m_drawingList.addClickListener( "Prototype", onArtClick );
		}
	}

	private void onArtButtonClick( UIButton p_button )
	{
		switch( p_button.name )
		{
		case "artOne" :
			SessionHandler.getInstance().currentDrawing = SessionHandler.getInstance().drawingList[0];
			break;
		case "artTwo" :
			SessionHandler.getInstance().currentDrawing = SessionHandler.getInstance().drawingList[1];
			break;
		case "artThree" :
			SessionHandler.getInstance().currentDrawing = SessionHandler.getInstance().drawingList[2];
			break;
		case "artFour" :
			SessionHandler.getInstance().currentDrawing = SessionHandler.getInstance().drawingList[3];
			break;
		case "artFive" :
			SessionHandler.getInstance().currentDrawing = SessionHandler.getInstance().drawingList[4];
			break;
		case "artSix" :
			SessionHandler.getInstance().currentDrawing = SessionHandler.getInstance().drawingList[5];
			break;
		}
		
		SessionHandler.getInstance ().IsParent = true;

		m_gameController.connectState ( ZoodleState.PAINT_VIEW, ZoodleState.OVERVIEW_ART );
		m_gameController.changeState (ZoodleState.PAINT_VIEW);
	}

	private void onArtClick( UISwipeList p_list, UIElement p_element, object p_data, int p_index )
	{
		Drawing l_drawing = p_data as Drawing;
		SessionHandler.getInstance ().currentDrawing = l_drawing;
		SessionHandler.getInstance ().IsParent = true;

		m_gameController.connectState ( ZoodleState.PAINT_VIEW, ZoodleState.OVERVIEW_ART );
		m_gameController.changeState (ZoodleState.PAINT_VIEW);
	}

	private void downLoadDrawing(UIElement p_element, Drawing p_drawing)
	{
		m_requestQueue.add (new DrawingRequest(p_drawing,p_element));
	}

	private void onDrawingListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		Drawing l_drawing = (Drawing)p_data;
		if(null != l_drawing.largeIcon)
		{
			UIImage l_image = p_element.getView("artImage") as UIImage;
			l_image.setTexture(l_drawing.largeIcon);
		}
	}

	private void onExitArtListButtonClick( UIButton p_button )
	{
		m_uiManager.changeScreen ( m_artListCanvas, false );
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_currentPanel = m_artListCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_currentPanel.transform.localPosition );
		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	private void _setupArtGalleryCanvas()
	{
		UILabel l_loading = m_artGalleryCanvas.getView ("loadingText") as UILabel;
		l_loading.active = false;

		m_requestQueue.reset ();
		
		UILabel l_artCountLabel = m_artListCanvas.getView ("artCountText") as UILabel;

		l_canvasList = new List<UIElement> ();
		UIElement l_art1 = m_artGalleryCanvas.getView( "artOne" );
		UIElement l_art2 = m_artGalleryCanvas.getView( "artTwo" );
		UIElement l_art3 = m_artGalleryCanvas.getView( "artThree" );
		UIElement l_art4 = m_artGalleryCanvas.getView( "artFour" );
		UIElement l_art5 = m_artGalleryCanvas.getView( "artFive" );
		UIElement l_art6 = m_artGalleryCanvas.getView( "artSix" );
		
		l_canvasList.Add (l_art1);
		l_canvasList.Add (l_art2);
		l_canvasList.Add (l_art3);
		l_canvasList.Add (l_art4);
		l_canvasList.Add (l_art5);
		l_canvasList.Add (l_art6);
		
		List<Drawing> l_list = SessionHandler.getInstance ().drawingList;

		l_artCountLabel.text = l_list.Count.ToString ();

		int l_count = l_list.Count >= 6 ? 6 : l_list.Count;

		if( 0 == l_count )
		{
			return;
		}

		m_moreArtButton.active = true;


		
		for(int l_i = 0; l_i < l_count; l_i++)
		{
			UIButton l_element = l_canvasList[l_i] as UIButton;
			Drawing l_drawing = l_list[l_i];

//			//get activity info again so I could request Image only; thats the only it will work so . .. . . .. .
//			ActivityInfo info  = funActivityList[l_i] as ActivityInfo;
//
//			Debug.LogWarning(" l i " + l_i + "    info  " + info.icon); 
//
//			if( null != info.icon )
//			{
//				UIImage l_image = l_element.getView("artImage") as UIImage;
//				l_image.setTexture(info.icon);
//
//				Debug.LogWarning( "  ****** texture okay ");
//			}
//			else
//			{
//				info.requestIcon();
//
//				Debug.LogWarning( "  ****** texture request ");
//			}


			if(null == l_drawing.largeIcon)
				downLoadDrawing(l_element,l_drawing);
			else
			{
				UIImage l_image = l_element.getView("artImage") as UIImage;
				l_image.setTexture(l_drawing.largeIcon);
//
//				ActivityInfo info = funActivityList[l_i] as ActivityInfo;
//				info.requestIcon();
				//l_drawing.m
//				delayedDownload(l_element,l_drawing);
			}
			l_element.active = true;
			l_element.addClickCallback( onArtButtonClick );
		}
		
		m_requestQueue.request ();
	}



	private IEnumerator delayedDownload(UIButton l_element, Drawing l_drawing) {
		yield return new WaitForSeconds(2f);
		downLoadDrawing(l_element,l_drawing);
	}



	private void loadDrawingList()
	{
		m_requestQueue.reset ();
		m_requestQueue.add(new GetDrawingRequest());
		m_requestQueue.request();
		isLoadDrawing = true;
	}

	private void toPremiumScreen(UIButton p_button)
	{
		if (LocalSetting.find("User").getBool("UserTry",true))
		{
			if(!SessionHandler.getInstance().token.isCurrent())
			{
				m_gameController.connectState (ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName));
				m_gameController.changeState (ZoodleState.VIEW_PREMIUM);	
			}
		}
		else
		{
			m_gameController.connectState (ZoodleState.SIGN_IN_UPSELL, int.Parse(m_gameController.stateName));
			m_gameController.changeState (ZoodleState.SIGN_IN_UPSELL);
		}
	}
	
	private void toBuyGemsScreen(UIButton p_button)
	{
		//m_game.gameController.changeState(ZoodleState.BUY_GEMS);
		gotoGetGems ();
	}
	
	private void gotoGetGems()
	{	
		string l_returnJson = SessionHandler.getInstance ().GemsJson;
		
		if(l_returnJson.Length > 0)
		{
			Hashtable l_date = MiniJSON.MiniJSON.jsonDecode (l_returnJson) as Hashtable;
			if(l_date.ContainsKey("jsonResponse"))
			{
				m_gameController.connectState( ZoodleState.BUY_GEMS, int.Parse(m_gameController.stateName) );
				m_gameController.changeState (ZoodleState.BUY_GEMS);
			}
			else
			{
				//sendCall (m_game.gameController,null,"/api/gems_amount/gems_amount?" + ZoodlesConstants.PARAM_TOKEN + "=" + SessionHandler.getInstance ().token.getSecret (),CallMethod.GET,ZoodleState.BUY_GEMS);
				Server.init (ZoodlesConstants.getHttpsHost());
				m_requestQueue.reset ();
				m_requestQueue.add (new ViewGemsRequest(viewGemsRequestComplete));
				m_requestQueue.request ();
			}
		}
		else
		{
			Server.init (ZoodlesConstants.getHttpsHost());
			m_requestQueue.reset ();
			m_requestQueue.add (new ViewGemsRequest(viewGemsRequestComplete));
			m_requestQueue.request ();
			//sendCall (m_game.gameController,null,"/api/gems_amount/gems_amount?" + ZoodlesConstants.PARAM_TOKEN + "=" + SessionHandler.getInstance ().token.getSecret (),CallMethod.GET,ZoodleState.BUY_GEMS);
		}
	}

	private void viewGemsRequestComplete(WWW p_response)
	{
		Server.init (ZoodlesConstants.getHost());
		if(p_response.error == null)
		{
			SessionHandler.getInstance ().GemsJson = p_response.text;
			m_gameController.connectState( ZoodleState.BUY_GEMS, int.Parse(m_gameController.stateName) );
			m_gameController.changeState (ZoodleState.BUY_GEMS);
		}
		else
		{
			setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_11_FAIL),Localization.getString(Localization.TXT_STATE_11_FAIL_DATA));
		}
	}

	private UIManager m_uiManager;
	private DashBoardControllerCanvas m_dashboardControllerCanvas;
	private UICanvas m_artGalleryCanvas;
	private UICanvas m_artListCanvas;
	private UICanvas m_dashboardCommonCanvas;
	private LeftMenuCanvas m_leftMenuCanvas;
	private CommonDialogCanvas m_commonDialog;
	
	private UIButton m_leftButton;
	private UIButton m_rightButton;
	private UIButton m_leftSideMenuButton;
	private UIButton m_showProfileButton;
	private UIButton m_closeLeftMenuButton;
	private UIButton m_childModeButton;
	private UIButton m_settingButton;

	private UIButton m_appsButton;
	private UIButton m_overviewButton;
	private UIButton m_controlsButton;
	private UIButton m_statChartButton;
	private UIButton m_tryPremiumButton;
	private UIButton m_buyGemsButton;
	
	private UIButton m_moreArtButton;
	private UIButton m_exitArtListButton;
	private UIButton m_helpButton;
	
	private UISwipeList m_childrenList;
	
	private UIElement 	m_menu;
	
	private bool 		canMoveLeftMenu = true;
	private bool		isLoadDrawing = false;
	
	private UISwipeList m_drawingList;
	
	private RequestQueue m_requestQueue;

	private ArrayList funActivityList;
	private List<UIElement> l_canvasList;
	private List<object> 	m_funViewList = new List<object>();
}