using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//===================================================================================================
//Overview for Art 
//===================================================================================================

public class OverviewArtState : GameState {
	
	public override void enter (GameController p_gameController)
	{

		base.enter (p_gameController);

		game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();

		_setupDrawingData();

		m_uiManager = m_gameController.getUI();
		m_requestQueue = new RequestQueue ();
		
		_setupScreen( p_gameController );
		_setupElment();

		SessionHandler.getInstance().onUpdateDrawingList += onSessionDrawingListUpdated;

	}



	public void onSessionDrawingListUpdated(){

//		Debug.LogWarning("    onSessionDrawingListUpdated   " );
//
//		for(int l_i = 0; l_i < l_canvasList.Count; l_i++)
//		{
//
//			UIButton l_element = l_canvasList[l_i] as UIButton;
//			//			Drawing l_drawing = l_list[l_i];
//			
//			//get activity info again so I could request Image only; thats the only it will work so . .. . . .. .
//			ActivityInfo info  = funActivityList[l_i] as ActivityInfo;
//			
//			info.forceRequestIcon();
//			
//		}

	}

	public override void update (GameController p_gameController, int p_time)
	{

//		base.update (p_gameController, p_time);
//
//		if(isLoadDrawing)
//		{
//			if(null != SessionHandler.getInstance().drawingList)
//			{
//				isLoadDrawing = false;
//
//				if( SessionHandler.getInstance().drawingList.Count > 0 )
//				{					
////					_setupArtGalleryCanvas();
//				}
//				else
//				{
////					UILabel l_loading = m_artGalleryCanvas.getView ("loadingText") as UILabel;
////					l_loading.text = Localization.getString(Localization.TXT_STATE_48_EMPTY);
//				}
//			}
//		}



//
//		for(int l_i = 0; l_i < funActivityList.Count; l_i++)
//		{
//
//			if(l_i < l_canvasList.Count){
//
//				UIButton l_element = l_canvasList[l_i] as UIButton;
//	//			Drawing l_drawing = l_list[l_i];
//				
//				//get activity info again so I could request Image only; thats the only it will work so . .. . . .. .
//				ActivityInfo info  = funActivityList[l_i] as ActivityInfo;
//				
//	//			Debug.LogWarning(" l i " + l_i + "    info  " + info.icon); 
//				
//				if( null != info.icon )
//				{
//					UIImage l_image = l_element.getView("artImage") as UIImage;
//					l_image.setTexture(info.icon);
//
//				}
//			}
//
//		}


	}
	
	public override void exit (GameController p_gameController)
	{
		base.exit (p_gameController);
		
//		m_uiManager.removeScreen( UIScreen.ART_GALLERY );
		m_uiManager.removeScreen( UIScreen.ART_LIST );
		m_uiManager.removeScreen( UIScreen.COMMON_DIALOG );

		SessionHandler.getInstance().onUpdateDrawingList -= onSessionDrawingListUpdated;
	}
	
	//----------------- Private Implementation -------------------


	//Set datas for drawing thumbnails and such
	private void _setupDrawingData(){

		
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

	}
	
	private void _setupScreen( GameController p_gameController )
	{
		m_commonDialog = m_uiManager.createScreen( UIScreen.COMMON_DIALOG, true, 15 )  as CommonDialogCanvas;
	
		m_commonDialog.setUIManager (p_gameController.getUI());

		m_artListCanvas 	= m_uiManager.createScreen( UIScreen.ART_LIST, false, 4 );

		if( null != m_artListCanvas && null != m_artListCanvas.getView("mainPanel") )
		{
			
			if( !m_artListCanvas.getView("mainPanel").active){
				
				Tweener tw = m_artListCanvas.getView("mainPanel").tweener;
				
				tw.addAlphaTrack( 0.0f, 1.0f, 1.0f );
			}
			
		}

		//FAde in the art list Canvas so there is no jump when it comes in
		//m_artListCanvas

//		m_artGalleryCanvas 	= m_uiManager.createScreen( UIScreen.ART_GALLERY, true, 1 );

	}
	
	private void _setupElment()
	{

//		m_helpButton = m_artGalleryCanvas.getView ("helpButton") as UIButton;

//		m_helpButton = m_artListCanvas.getView ("helpButton") as UIButton;
//		m_helpButton.addClickCallback (onHelpButtonClick);
		//m_artGalleryCanvas
		
//		UIElement l_newPanel = m_artGalleryCanvas.getView ("mainPanel");
//		List<Vector3> l_pointListIn = new List<Vector3>();
//		l_pointListIn.Add( l_newPanel.transform.localPosition );
//		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 491, 0 ));
////		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
//		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, 0.5f);



//		m_moreArtButton = m_artGalleryCanvas.getView( "artListButton" ) as UIButton;
//		m_moreArtButton.addClickCallback( onMoreArtButtonClick );
//		m_moreArtButton.active = false;

//		m_exitArtListButton = m_artListCanvas.getView( "exitButton" ) as UIButton;
//		m_exitArtListButton.addClickCallback( onExitArtListButtonClick );

		//Create an empty list for set up swipeList.
		List<System.Object> l_list = new List<System.Object>();
		m_drawingList = m_artListCanvas.getView ("artSwipeList") as UISwipeList;
		m_drawingList.setData (l_list);


		if( null != SessionHandler.getInstance().drawingList)
		{



//			if( SessionHandler.getInstance().drawingList.Count > 0 )
//			{					
//				_setupArtGalleryCanvas();
//			}
//			else
//			{
//				UILabel l_loading = m_artGalleryCanvas.getView ("loadingText") as UILabel;
//				l_loading.text = Localization.getString(Localization.TXT_STATE_48_EMPTY);
//			}

			showArtList();

		}
		else
		{
			loadDrawingList();
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
	


	private void showArtList(){

//		m_uiManager.changeScreen ( m_artListCanvas, true );
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_artListCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
//		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f,onArtPositionTrackFinish );

	}


	private void onMoreArtButtonClick( UIButton p_button )
	{

//		game.setPDMenuBarVisible(false, false);

		showArtList();

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

	//This is old ones . . . . the six original thumbnails
	//so remove them since no use for now
//	private void onArtButtonClick( UIButton p_button )
//	{
//		switch( p_button.name )
//		{
//		case "artOne" :
//			SessionHandler.getInstance().currentDrawing = SessionHandler.getInstance().drawingList[0];
//			break;
//		case "artTwo" :
//			SessionHandler.getInstance().currentDrawing = SessionHandler.getInstance().drawingList[1];
//			break;
//		case "artThree" :
//			SessionHandler.getInstance().currentDrawing = SessionHandler.getInstance().drawingList[2];
//			break;
//		case "artFour" :
//			SessionHandler.getInstance().currentDrawing = SessionHandler.getInstance().drawingList[3];
//			break;
//		case "artFive" :
//			SessionHandler.getInstance().currentDrawing = SessionHandler.getInstance().drawingList[4];
//			break;
//		case "artSix" :
//			SessionHandler.getInstance().currentDrawing = SessionHandler.getInstance().drawingList[5];
//			break;
//		}
//		
//		SessionHandler.getInstance ().IsParent = true;
//
//		m_gameController.connectState ( ZoodleState.PAINT_VIEW, ZoodleState.OVERVIEW_ART );
//		m_gameController.changeState (ZoodleState.PAINT_VIEW);
//	}

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
//		return;
//		return;

		//Kevin, some how the drawing icon at index 0 get changed some how .. . . .  so solving this this way for now
		//Best to find the cause of this
		//Even every index is return without drawing, the first thumbnail still get changed some how , some where . . .. . .
		if(p_index == 0){

			UIImage l_image = p_element.getView("artImage") as UIImage;
			
			ActivityInfo info = funActivityList[p_index] as ActivityInfo;

			l_image.setTexture(info.drawing.largeIcon);
			
			if(info.icon != null){
				l_image.setTexture(info.icon);
			}else{

				if(!info.IsRequested)
					info.forceRequestIcon();

			}
			
			return;
			
		}

		Drawing l_drawing = (Drawing)p_data;
		if(null != l_drawing.largeIcon)
		{
			UIImage l_image = p_element.getView("artImage") as UIImage;
			l_image.setTexture(l_drawing.largeIcon);
		}else{

			List<Drawing> l_drawingList = SessionHandler.getInstance ().drawingList;


		}
	}

//	private void onExitArtListButtonClick( UIButton p_button )
//	{
//
//		game.setPDMenuBarVisible(true, false);
//
//		m_uiManager.changeScreen ( m_artListCanvas, false );
//		List<Vector3> l_pointListOut = new List<Vector3>();
//		UIElement l_currentPanel = m_artListCanvas.getView ("mainPanel");
//		l_pointListOut.Add( l_currentPanel.transform.localPosition );
//		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
//		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );
//
//	}

//	private void _setupArtGalleryCanvas()
//	{
//		UILabel l_loading = m_artGalleryCanvas.getView ("loadingText") as UILabel;
//		l_loading.active = false;
//
//		m_requestQueue.reset ();
//		
//
//		l_canvasList = new List<UIElement> ();
//		UIElement l_art1 = m_artGalleryCanvas.getView( "artOne" );
//		UIElement l_art2 = m_artGalleryCanvas.getView( "artTwo" );
//		UIElement l_art3 = m_artGalleryCanvas.getView( "artThree" );
//		UIElement l_art4 = m_artGalleryCanvas.getView( "artFour" );
//		UIElement l_art5 = m_artGalleryCanvas.getView( "artFive" );
//		UIElement l_art6 = m_artGalleryCanvas.getView( "artSix" );
//		
//		l_canvasList.Add (l_art1);
//		l_canvasList.Add (l_art2);
//		l_canvasList.Add (l_art3);
//		l_canvasList.Add (l_art4);
//		l_canvasList.Add (l_art5);
//		l_canvasList.Add (l_art6);
		
//		List<Drawing> l_list = SessionHandler.getInstance ().drawingList;
//
//
//		int l_count = l_list.Count >= 6 ? 6 : l_list.Count;
//
//		if( 0 == l_count )
//		{
//			return;
//		}
//
//		m_moreArtButton.active = true;
//
//
//		
//		for(int l_i = 0; l_i < l_count; l_i++)
//		{
//			UIButton l_element = l_canvasList[l_i] as UIButton;
//			Drawing l_drawing = l_list[l_i];
//
////			//get activity info again so I could request Image only; thats the only it will work so . .. . . .. .
////			ActivityInfo info  = funActivityList[l_i] as ActivityInfo;
////
////			Debug.LogWarning(" l i " + l_i + "    info  " + info.icon); 
////
////			if( null != info.icon )
////			{
////				UIImage l_image = l_element.getView("artImage") as UIImage;
////				l_image.setTexture(info.icon);
////
////				Debug.LogWarning( "  ****** texture okay ");
////			}
////			else
////			{
////				info.requestIcon();
////
////				Debug.LogWarning( "  ****** texture request ");
////			}
//
//
//			if(null == l_drawing.largeIcon)
//				downLoadDrawing(l_element,l_drawing);
//			else
//			{
//				UIImage l_image = l_element.getView("artImage") as UIImage;
//				l_image.setTexture(l_drawing.largeIcon);
////
////				ActivityInfo info = funActivityList[l_i] as ActivityInfo;
////				info.requestIcon();
//				//l_drawing.m
////				delayedDownload(l_element,l_drawing);
//			}
//			l_element.active = true;
//			l_element.addClickCallback( onArtButtonClick );
//		}
//		
//		m_requestQueue.request ();
//	}


//
//	private IEnumerator delayedDownload(UIButton l_element, Drawing l_drawing) {
//		yield return new WaitForSeconds(2f);
//		downLoadDrawing(l_element,l_drawing);
//	}



	private void loadDrawingList()
	{
		m_requestQueue.reset ();
		m_requestQueue.add(new GetDrawingRequest(loadDrawingListComplete));
		m_requestQueue.request();
		isLoadDrawing = true;
	}

	private void loadDrawingListComplete(HttpsWWW p_response)
	{

		ArrayList l_data = MiniJSON.MiniJSON.jsonDecode (p_response.text) as ArrayList;
		int l_dataCount = l_data.Count;
		List<Drawing> l_list = new List<Drawing> ();

		for(int l_i = 0; l_i < l_dataCount; l_i++)
		{
			Hashtable l_table = l_data[l_i] as Hashtable;
			Drawing l_drawing = new Drawing(l_table);
			l_list.Add(l_drawing);
		}

		SessionHandler.getInstance ().drawingList = l_list;

		_setupDrawingData();

		showArtList();

	}


	


	private UIManager m_uiManager;
//	private DashBoardControllerCanvas m_dashboardControllerCanvas;
//	private UICanvas m_artGalleryCanvas;
	private UICanvas m_artListCanvas;
//	private UICanvas m_dashboardCommonCanvas;
//	private LeftMenuCanvas m_leftMenuCanvas;
	private CommonDialogCanvas m_commonDialog;

	
	private UIButton m_moreArtButton;
//	private UIButton m_exitArtListButton;
//	private UIButton m_helpButton;
	
	private UISwipeList m_childrenList;
	
	private UIElement 	m_menu;


	private bool		isLoadDrawing = false;
	
	private UISwipeList m_drawingList;
	
	private RequestQueue m_requestQueue;

	private ArrayList funActivityList;
	private List<UIElement> l_canvasList;

	private Game game;

}