using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

public class OverviewBookState : GameState
{
	//--------------------Public Interface -----------------------
	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);

		m_uiManager = m_gameController.getUI();
		m_requestQueue = new RequestQueue ();

		_setupScreen( p_gameController );
		_setupElment();
	}

	public override void update (GameController p_gameController, int p_time)
	{
		base.update (p_gameController, p_time);
		if(isLoadBook)
		{
			if(null != SessionHandler.getInstance().bookList)
			{
				isLoadBook = false;
				_setupRecommendedBookCanvas();
			}
		}
		m_gemCountLabelInBook.text = SessionHandler.getInstance ().currentKid.gems.ToString();
	}

	public override void exit (GameController p_gameController)
	{
		base.exit (p_gameController);
		m_uiManager.removeScreen( UIScreen.CONFIRM_DIALOG );
		m_uiManager.removeScreen( UIScreen.BOOK_LIST );
		m_uiManager.removeScreen( UIScreen.LEFT_MENU );
		m_uiManager.removeScreen( UIScreen.RECOMMENDED_BOOK );
		m_uiManager.removeScreen( UIScreen.DASHBOARD_CONTROLLER );
		m_uiManager.removeScreen( UIScreen.DASHBOARD_COMMON );
		m_uiManager.removeScreen( UIScreen.COMMON_DIALOG );

	}

	//----------------- Private Implementation -------------------

	private void _setupScreen( GameController p_gameController )
	{
		m_commonDialog = m_uiManager.createScreen( UIScreen.COMMON_DIALOG, true, 18 )  as CommonDialogCanvas;
		m_commonDialog.setUIManager (p_gameController.getUI());
		m_confirmDialogCanvas 	= m_uiManager.createScreen( UIScreen.CONFIRM_DIALOG, false, 15 );
		m_bookListCanvas 	= m_uiManager.createScreen( UIScreen.BOOK_LIST, false, 12 );
		m_leftMenuCanvas = m_uiManager.createScreen( UIScreen.LEFT_MENU, true, 10 )  as LeftMenuCanvas;
		m_dashboardControllerCanvas = m_uiManager.createScreen( UIScreen.DASHBOARD_CONTROLLER, false, 9 ) as DashBoardControllerCanvas;
		m_recommendedBookCanvas	= m_uiManager.createScreen( UIScreen.RECOMMENDED_BOOK, true, 2 );
		m_dashboardCommonCanvas = m_uiManager.createScreen( UIScreen.DASHBOARD_COMMON, true, 0 );
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_recommendedBookCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 830, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
		
		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED);
		if(null == SessionHandler.getInstance().bookList)
		{
			m_requestQueue.reset ();
			m_requestQueue.add(new GetBookRequest(_requestBookListComplete));
			m_requestQueue.request();
			isLoadBook = true;
		}
	}

	private void _setupElment()
	{

		m_helpButton = m_recommendedBookCanvas.getView ("helpButton") as UIButton;
		m_helpButton.addClickCallback (onHelpButtonClick);

		m_leftButton = m_dashboardControllerCanvas.getView( "leftButton" ) as UIButton;
		m_leftButton.addClickCallback( onLeftButtonClick );
		
		m_rightButton = m_dashboardControllerCanvas.getView( "rightButton" ) as UIButton;
		m_rightButton.addClickCallback( onRightButtonClick );

		m_dashboardControllerCanvas.setupDotList( 7 );
		m_dashboardControllerCanvas.setCurrentIndex( 4 );
		m_gemCountLabelInBook = m_recommendedBookCanvas.getView ("gemCountText") as UILabel;
		m_moreBookButton = m_recommendedBookCanvas.getView( "bookListButton" ) as UIButton;
		m_moreBookButton.addClickCallback( onMoreBookButtonClick );
		m_exitBookListButton = m_bookListCanvas.getView( "exitButton" ) as UIButton;
		m_exitBookListButton.addClickCallback( onExitBookListButtonClick );
		m_costArea = m_confirmDialogCanvas.getView("costArea");
		m_needMoreArea = m_confirmDialogCanvas.getView("needMoreArea");
		m_exitConfirmDialogButton = m_confirmDialogCanvas.getView ("exitButton") as UIButton;
		m_exitConfirmDialogButton.addClickCallback (onExitConfiemDialogButtonClick);
		m_cancelBuyButton = m_confirmDialogCanvas.getView ("cancelButton") as UIButton;
		m_cancelBuyButton.addClickCallback (onExitConfiemDialogButtonClick);
		m_confirmBuyButton = m_confirmDialogCanvas.getView ("confirmButton") as UIButton;
		m_confirmBuyButton.addClickCallback (onConfiemButtonClick);
		
		List<object> l_list = new List<object> ();
		m_bookList = m_bookListCanvas.getView ("bookSwipeList") as UISwipeList;
		m_bookList.setData (l_list);

		m_buyGemsButton = m_confirmDialogCanvas.getView("buyGemsButton") as UIButton;
		m_buyGemsButton.addClickCallback(buyGems);
		m_costArea = m_confirmDialogCanvas.getView("costArea");
		m_needMoreArea = m_confirmDialogCanvas.getView("needMoreArea");
		
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
		
		m_overviewButton = m_dashboardCommonCanvas.getView ("overviewButton") as UIButton;
		m_overviewButton.enabled = false;
		
		m_controlsButton = m_dashboardCommonCanvas.getView ("controlButton") as UIButton;
		m_controlsButton.addClickCallback (goToControls);
		
		m_statChartButton = m_dashboardCommonCanvas.getView ("starButton") as UIButton;
		m_statChartButton.addClickCallback (goToStarChart);
		
		m_childrenList = m_leftMenuCanvas.getView ("childSwipeList") as UISwipeList;
		m_childrenList.addClickListener ("Prototype",onSelectThisChild);

		m_tryPremiumButton = m_leftMenuCanvas.getView ("premiumButton") as UIButton;
		m_buyGemsButton = m_leftMenuCanvas.getView ("buyGemsButton") as UIButton;
		m_buyGemsButtonOnConfirm = m_confirmDialogCanvas.getView ("buyGemsButton") as UIButton;
		m_tryPremiumButton.addClickCallback (toPremiumScreen);
		m_buyGemsButton.addClickCallback (toBuyGemsScreen);
		m_buyGemsButtonOnConfirm.addClickCallback (toBuyGemsScreen);

		m_moreBookButton.active = false;

		if( null != SessionHandler.getInstance().bookList)
		{
			_setupRecommendedBookCanvas();
		}
	}

	private void onHelpButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks ();
		m_commonDialog.setOriginalPosition ();
		UIButton l_closeButton = m_commonDialog.getView ("closeMark") as UIButton;
		
		UILabel l_titleLabel = m_commonDialog.getView ("dialogText") as UILabel;
		UILabel l_contentLabel = m_commonDialog.getView ("contentText") as UILabel;
		l_titleLabel.text = "Recommended Books";
		l_contentLabel.text = "Foster a love of reading in your child by selecting recommended digital storybooks.";

		l_closeButton.addClickCallback (onCloseDialogButtonClick);
	}
	
	private void onCloseDialogButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		m_commonDialog.setOutPosition ();
		m_helpButton.addClickCallback (onHelpButtonClick);
	}

	private void buyGems( UIButton p_button )
	{
		m_costArea.active = true;
		m_needMoreArea.active = false;

		gotoGetGems ();

//		m_gameController.connectState ( ZoodleState.BUY_GEMS, int.Parse(m_gameController.stateName) );
//		m_gameController.changeState (ZoodleState.BUY_GEMS);
	}

	private void _setupRecommendedBookCanvas()
	{	
		m_requestQueue.reset ();
		
		UILabel l_bookCountLabel = m_bookListCanvas.getView ("bookCountText") as UILabel;
		List<UIElement> l_canvasList = new List<UIElement> ();
		UIElement l_book1 = m_recommendedBookCanvas.getView ("bookOne") as UIElement;
		UIElement l_book2 = m_recommendedBookCanvas.getView ("bookTwo") as UIElement;
		UIElement l_book3 = m_recommendedBookCanvas.getView ("bookThree") as UIElement;
		UIElement l_book4 = m_recommendedBookCanvas.getView ("bookFour") as UIElement;
		
		l_canvasList.Add (l_book1);
		l_canvasList.Add (l_book2);
		l_canvasList.Add (l_book3);
		l_canvasList.Add (l_book4);
		List<Book> l_list = SessionHandler.getInstance ().bookList;

		l_bookCountLabel.text = l_list.Count.ToString ();

		int l_count = l_list.Count >= 4 ? 4 : l_list.Count;
		for(int l_i = 0; l_i < l_count; l_i++)
		{
			UIElement l_element = l_canvasList[l_i];
			
			Book l_book = l_list[l_i];
			
			_setupSignleBook(l_element,l_book);
			UIButton l_buyButton = l_element.getView ("buyBookButton") as UIButton;
			l_buyButton.addClickCallback(onClickBuyBookButton);
			
			if(null == l_book.icon)
				downLoadBookIcon( l_book, l_element );
			else
			{
				UIImage l_image = l_element.getView("bookImage") as UIImage;
				l_image.setTexture(l_book.icon);
			}
			l_element.active = true;
		}
		
		m_requestQueue.request ();
		m_moreBookButton.active = true;
	}

	private void _setupSignleBook(UIElement p_element, Book p_book)
	{
		UILabel l_bookName = p_element.getView ("bookNameText") as UILabel;
		l_bookName.text = p_book.title;
		UILabel l_gemsPrice = p_element.getView ("priceText") as UILabel;
		l_gemsPrice.text = p_book.gems.ToString();
		
		UIImage l_lockImage = p_element.getView ("lockImage") as UIImage;
		UIButton l_buyButton = p_element.getView ("buyBookButton") as UIButton;
		UILabel l_unlockLabel = p_element.getView ("unlockText") as UILabel;

		Token l_token = SessionHandler.getInstance ().token;

		if( l_token.isPremium() || l_token.isCurrent() || p_book.gems == 0 || p_book.owned)
		{
			l_lockImage.active = false;
			l_buyButton.active = false;
			l_unlockLabel.text = "Unlocked";
		}
		else
		{
			l_lockImage.active = true;
			l_buyButton.active = true;
			l_unlockLabel.text = "Unlock this book";
		}
	}

	private void onExitConfiemDialogButtonClick( UIButton p_button )
	{
		m_uiManager.changeScreen (UIScreen.CONFIRM_DIALOG,false);
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_currentPanel = m_confirmDialogCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_currentPanel.transform.localPosition );
		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	private void onClickBuyBookButton(UIButton p_button)
	{
		m_wantedBook = null;
		List<Book> l_bookList = SessionHandler.getInstance ().bookList;
		switch(p_button.parent.parent.name)
		{
		case "bookOne":
			m_wantedBook = l_bookList[0];
			break;
		case "bookTwo":
			m_wantedBook = l_bookList[1];
			break;
		case "bookThree":
			m_wantedBook = l_bookList[2];
			break;
		case "bookFour":
			m_wantedBook = l_bookList[3];
			break;
		}
		m_clickedBuyButton = p_button;
		confirmBuyBook (m_wantedBook);
	}
	
	private void onBookClick(UISwipeList p_list, UIButton p_listElement, System.Object p_data, int p_index)
	{
		m_selectedElement = null;
		switch(p_index)
		{
		case 0:
			m_selectedElement = m_recommendedBookCanvas.getView("bookOne") as UIElement;
			break;
		case 1:
			m_selectedElement = m_recommendedBookCanvas.getView("bookTwo") as UIElement;
			break;
		case 2:
			m_selectedElement = m_recommendedBookCanvas.getView("bookThree") as UIElement;
			break;
		case 3:
			m_selectedElement = m_recommendedBookCanvas.getView("bookFour") as UIElement;
			break;
		default:
			break;
		}
		m_wantedBook = (Book)p_data;
		m_clickedBuyButton = p_listElement;
		confirmBuyBook (m_wantedBook);
	}

	private void onMoreBookButtonClick( UIButton p_button )
	{
		m_uiManager.changeScreen (UIScreen.BOOK_LIST,true);
		bookListOpen = true;
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_bookListCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f,onBookPositionTrackFinish );
	}

	private void onBookPositionTrackFinish(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		m_requestQueue.reset ();
		if(null == m_bookList)
			m_bookList = m_bookListCanvas.getView ("bookSwipeList") as UISwipeList;
		List<Book> l_bookList = SessionHandler.getInstance ().bookList;
		if(null != l_bookList)
		{
			List< System.Object > infoData = new List< System.Object >();
			int l_count = l_bookList.Count;
			for(int l_i =0; l_i < l_count; l_i++ )
			{
				infoData.Add(l_bookList[l_i]);
				if(null == l_bookList[l_i].icon)
					downLoadBookIcon( l_bookList[l_i], null );
			}
			m_requestQueue.request ();
			m_bookList.setData( infoData );
			m_bookList.setDrawFunction( onBookListDraw );
			m_bookList.redraw();
		}
		m_bookList.addClickListener ("buyBookButton", onBookClick);
	}

	private void onBookListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		Book l_book = (Book)p_data;
		_setupSignleBook (p_element, (Book)p_data);
		if(null != l_book.icon)
		{
			UIImage l_image = p_element.getView("bookImage") as UIImage;
			l_image.setTexture(l_book.icon);
		}
	}

	private void onExitBookListButtonClick( UIButton p_button )
	{
		m_uiManager.changeScreen (UIScreen.BOOK_LIST,false);
		bookListOpen = false;
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_currentPanel = m_bookListCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_currentPanel.transform.localPosition );
		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );
	}


	private void onConfiemButtonClick( UIButton p_button )
	{
		if(null != m_wantedBook && null != m_clickedBuyButton)
		{
			if(SessionHandler.getInstance().currentKid.gems >= m_wantedBook.gems)
				sendBuyBookRequest ();
			else
			{
				UILabel l_costGems = m_confirmDialogCanvas.getView("costPriceText") as UILabel;
				UILabel l_needGems = m_confirmDialogCanvas.getView("needPriceText") as UILabel;
				l_costGems.text = m_wantedBook.gems.ToString ();
				l_needGems.text = (m_wantedBook.gems - SessionHandler.getInstance().currentKid.gems).ToString ();
				UILabel l_titleLabel = m_confirmDialogCanvas.getView("titleText") as UILabel;
				l_titleLabel.text = "Need More Gems!";
				UILabel l_notice1label = m_confirmDialogCanvas.getView("noticeText1") as UILabel;
				l_notice1label.text = "You need more gems to purchase:";
				m_costArea.active = false;
				m_needMoreArea.active = true;
			}
		}
	}

	private void downLoadBookIcon( Book p_book, UIElement p_element )
	{
		m_requestQueue.add (new BookIconRequest( p_book, p_element ));
	}
	
	public void confirmBuyBook(Book p_book)
	{
		if(bookListOpen)
			m_uiManager.changeScreen (UIScreen.BOOK_LIST,false);
		m_uiManager.changeScreen (UIScreen.CONFIRM_DIALOG,true);
		m_costArea.active = true;
		m_needMoreArea.active = false;
		
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_confirmDialogCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f);
		
		UILabel l_titleLabel = l_newPanel.getView("titleText") as UILabel;
		l_titleLabel.text = "Confirm Purchase";
		UILabel l_notice1label = l_newPanel.getView("noticeText1") as UILabel;
		l_notice1label.text = "You are purchasing:";
		UILabel l_notice1label2 = l_newPanel.getView("noticeText2") as UILabel;
		l_notice1label2.text = p_book.title;
		UILabel l_priceLabel = l_newPanel.getView("priceText") as UILabel;
		l_priceLabel.text = p_book.gems.ToString();
	}
	
	private void sendBuyBookRequest()
	{
		m_requestQueue.reset ();
		m_requestQueue.add (new BuyBookRequest (m_wantedBook, m_clickedBuyButton, m_confirmDialogCanvas ,m_selectedElement));
		m_requestQueue.request ();
		m_uiManager.changeScreen (UIScreen.CONFIRM_DIALOG,false);
	}

	//-------------------------------------------------------------------------------------------------------------------------------------------------//

	private void toShowMenu(UIButton p_button)
	{
		if(canMoveLeftMenu)
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
	
	private void toChildMode(UIButton p_button)
	{
		m_gameController.changeState (ZoodleState.PROFILE_SELECTION);
	}
	
	private void toSettingScreen(UIButton p_button)
	{
		p_button.removeClickCallback (toSettingScreen);
		m_gameController.changeState (ZoodleState.SETTING_STATE);
	}
	
	private void goToControls( UIButton p_button )
	{
		m_gameController.changeState (ZoodleState.CONTROL_SUBJECT);
	}
	
	private void goToStarChart( UIButton p_button )
	{
		m_gameController.changeState (ZoodleState.DASHBOARD_STAR_CHART);
	}
	
	private void onSelectThisChild(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		Kid l_kid = p_data as Kid;
		if (ZoodlesConstants.ADD_CHILD_TEXT.Equals (l_kid.name))
		{
			SessionHandler.getInstance().CreateChild = true;
			m_gameController.connectState(ZoodleState.CREATE_CHILD,int.Parse(m_gameController.stateName));
			m_gameController.changeState (ZoodleState.CREATE_CHILD);
		}
		else
		{
			List<Kid> l_kidList = SessionHandler.getInstance().kidList;
			SessionHandler.getInstance().currentKid = l_kidList[p_index-1];
			m_gameController.changeState(ZoodleState.OVERVIEW_INFO);
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
	
	private void onLeftButtonClick( UIButton p_button )
	{
		m_gameController.changeState( ZoodleState.OVERVIEW_APP );
	}
	
	private void onRightButtonClick( UIButton p_button )
	{
		m_gameController.changeState( ZoodleState.OVERVIEW_READING );
	}
	
	private void goToChildLock(UIButton p_button)
	{
		m_gameController.changeState (ZoodleState.CHILD_LOCK_STATE);
	}

	private void toPremiumScreen(UIButton p_button)
	{
		m_gameController.connectState (ZoodleState.SIGN_IN_UPSELL, int.Parse(m_gameController.stateName));
		m_gameController.changeState (ZoodleState.SIGN_IN_UPSELL);
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
			setErrorMessage(m_gameController,"fail","Get date failed please try it again.");
		}
	}

	private void _requestBookListComplete(WWW p_response)
	{
		string l_string = "";
		
		l_string = UnicodeDecoder.Unicode(p_response.text);
		l_string = UnicodeDecoder.UnicodeToChinese(l_string);
		l_string = UnicodeDecoder.CoverHtmlLabel(l_string);
		
		ArrayList l_data = MiniJSON.MiniJSON.jsonDecode (l_string) as ArrayList;
		int l_dataCount = l_data.Count;
		List<Book> l_list = new List<Book> ();
		for(int l_i = 0; l_i < l_dataCount; l_i++)
		{
			Hashtable l_table = l_data[l_i] as Hashtable;
			Book l_book = new Book(l_table);
			l_list.Add(l_book);
		}
		SessionHandler.getInstance ().bookList = l_list;

		
		m_moreBookButton = m_recommendedBookCanvas.getView( "bookListButton" ) as UIButton;

		if( m_moreBookButton != null )
		{
			m_moreBookButton.active = true;
		}
	}
	//-------------------------------------------------------------------------------------------------------------------------------------------------//

	private DashBoardControllerCanvas m_dashboardControllerCanvas;
	private UICanvas 				  m_dashboardCommonCanvas;
	private LeftMenuCanvas 			  m_leftMenuCanvas;
	private UICanvas				  m_recommendedBookCanvas;
	private UICanvas				  m_bookListCanvas;
	private CommonDialogCanvas 		  m_commonDialog;
	private UICanvas 				  m_confirmDialogCanvas;
	private UILabel 				  m_gemCountLabel;
	private UIManager 				  m_uiManager;
	private UIButton 				  m_moreBookButton;
	private UIButton				  m_exitBookListButton;
	private UIButton				  m_exitBookDetailsButton;
	private UILabel 			      m_gemCountLabelInBook;
	private Book	 			      m_wantedBook;
	private UISwipeList 			  m_bookList;
	private bool 					  isLoadBook = false;
	private UIElement  				  m_costArea;
	private UIButton 			      m_buyGemsButton;
	private UIButton 			      m_buyGemsButtonOnConfirm;
	private UIElement 				  m_needMoreArea;
	private UIButton 			      m_clickedBuyButton;
	private UIElement 			   	  m_selectedElement;
	private UIButton 				  m_exitConfirmDialogButton;
	private UIButton 			      m_cancelBuyButton;
	private UIButton 				  m_confirmBuyButton;
	private UIButton 				  m_leftSideMenuButton;
	private UIButton    			  m_showProfileButton;
	private UIButton 				  m_closeLeftMenuButton;
	private UIButton 				  m_childModeButton;
	private UIButton 				  m_settingButton;
	private UISwipeList				  m_childrenList;
	private RequestQueue 			  m_requestQueue;
	private UIButton				  m_leftButton;
	private UIButton 				  m_helpButton;
	private UIButton				  m_rightButton;
	private UIElement 				  m_menu;
	private UIButton				  m_overviewButton;
	private UIButton 				  m_controlsButton;
	private UIButton				  m_statChartButton;
	private UIButton 				  m_tryPremiumButton;
	private bool 					  canMoveLeftMenu = true;
	private bool 					  bookListOpen = false;
}