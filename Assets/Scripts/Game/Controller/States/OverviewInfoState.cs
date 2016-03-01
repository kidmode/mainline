using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverviewInfoState : GameState {

	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);

		m_uiManager = m_gameController.getUI();
		m_requestQueue = new RequestQueue ();
		UICanvas l_backScreen = m_uiManager.findScreen( UIScreen.SPLASH_BACKGROUND );
		if (l_backScreen != null)
		{
			m_uiManager.removeScreenImmediately(UIScreen.SPLASH_BACKGROUND);
		}
		m_app = SessionHandler.getInstance ().currentKid.topRecommendedApp;
		canLoadTopRecommandApp = true;
		m_getTopRecommendAppRequestQueue = new RequestQueue ();
		_setupScreen( p_gameController );
		_setupElment();
		turnOffChildLock();

		if(TutorialController.Instance != null)
			TutorialController.Instance.showNextPage();
		SwrveComponent.Instance.SDK.NamedEvent("Parent_Dashboard.overview");

	}

	private void turnOffChildLock()
	{
//		KidMode.setKidsModeActive(false);	
		KidModeLockController.Instance.swith2DParentMode();
	}
	//private float l_time = 0.0f;
	public override void update (GameController p_gameController, int p_time)
	{
		base.update (p_gameController, p_time);
		//l_time += Time.deltaTime;
		if(null != SessionHandler.getInstance().currentKid.topRecommendedApp && canLoadTopRecommandApp)
		{
			canLoadTopRecommandApp = false;
			m_app = SessionHandler.getInstance().currentKid.topRecommendedApp;
			if( 0 != m_app.id )
			{
				m_topRecommendAppArea.active = true;
				_setupSignleApp(m_topRecommendAppArea,m_app);
				loadAppDetail();
			}
			else
			{
				m_topRecommendAppArea.active = false;
				
				UILabel l_emptyText = m_dashboardInfoCanvas.getView("emptyText") as UILabel;
				l_emptyText.text = Localization.getString(Localization.TXT_STATE_50_EMPTY);
			}

		}
	}

	public override void exit (GameController p_gameController)
	{
		base.exit (p_gameController);
		m_requestQueue.dispose ();
		m_uiManager.removeScreen( UIScreen.DASHBOARD_INFO );
		m_uiManager.removeScreen( UIScreen.CONFIRM_DIALOG );
		m_uiManager.removeScreen( UIScreen.APP_DETAILS );
	}

	//----------------- Private Implementation -------------------

	private void _setupScreen( GameController p_gameController )
	{
		m_confirmDialogCanvas 	= m_uiManager.createScreen( UIScreen.CONFIRM_DIALOG, false, 15 ) as ConfirmDialogCanvas;

		m_appDetailsCanvas 	= m_uiManager.createScreen( UIScreen.APP_DETAILS, false, 14 ) as AppDetailsCanvas;

		m_dashboardInfoCanvas 	= m_uiManager.createScreen( UIScreen.DASHBOARD_INFO, true, 1 );

	}

	private void _setupElment()
	{

		m_exitAppDetailsButton = m_appDetailsCanvas.getView( "exitButton" ) as UIButton;
		m_exitAppDetailsButton.addClickCallback( onExitAppDetailsButtonClick );

		m_editProfileButton = m_dashboardInfoCanvas.getView ("editProfileButton") as UIButton;
		m_editProfileButton.addClickCallback (editProfile);

		UIElement l_newPanel = m_dashboardInfoCanvas.getView ("mainPanel");
		List<Vector3> l_pointListIn = new List<Vector3>();

		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, 0.5f);


		//confirm dialog items
		m_exitConfirmDialogButton = m_confirmDialogCanvas.getView ("exitButton") as UIButton;
		m_exitConfirmDialogButton.addClickCallback (onExitConfiemDialogButtonClick);
		
		m_costArea = m_confirmDialogCanvas.getView("costArea");
		m_cancelBuyButton = m_confirmDialogCanvas.getView ("cancelButton") as UIButton;
		m_cancelBuyButton.addClickCallback (onExitConfiemDialogButtonClick);
		m_confirmBuyButton = m_confirmDialogCanvas.getView ("confirmButton") as UIButton;
		m_confirmBuyButton.addClickCallback (onConfiemButtonClick);
		
		m_needMoreArea = m_confirmDialogCanvas.getView("needMoreArea");
		m_buyGemsButtonOnConfirm = m_confirmDialogCanvas.getView ("buyGemsButton") as UIButton;
		m_buyGemsButtonOnConfirm.addClickCallback (toBuyGemsScreen);
		//end of confirm dialog items

		m_topRecommendAppArea = m_dashboardInfoCanvas.getView ("app") as UIButton;
		m_topRecommendAppArea.addClickCallback (showAppDetail);

		if (null == m_app)
		{
			m_topRecommendAppArea.active = false;

			UILabel l_emptyText = m_dashboardInfoCanvas.getView("emptyText") as UILabel;
			l_emptyText.text = Localization.getString(Localization.TXT_LABEL_LOADING);
			if(SessionHandler.getInstance().appRequest.isCompleted())
			{
				m_getTopRecommendAppRequestQueue.reset();
				m_getTopRecommendAppRequestQueue.add(new GetTopRecommandRequest(ZoodlesConstants.GOOGLE,SessionHandler.getInstance().currentKid,getTopRecommendRequestComplete));
				m_getTopRecommendAppRequestQueue.request(RequestType.SEQUENCE);
			}
		}

		//Get the list
		List<Book> l_list = SessionHandler.getInstance().bookList;

		if(l_list == null){


			m_requestQueue.reset();
			m_requestQueue.add(new GetBookRequest("false",_requestBookListComplete));
			m_requestQueue.request();

		}else if(l_list.Count > 0){

			bookList = l_list;
			drawRecommendedBookInfo();

		}

	}

	private void drawRecommendedBookInfo(){

		List<Book> l_list = bookList;

		m_selectedElement = m_dashboardInfoCanvas.getView ("bookOne") as UIElement;
		
		curretRecommendedBookIndex = Random.Range(0, l_list.Count);
		
		Book l_book = l_list[curretRecommendedBookIndex];
		
		_setupSignleBook(m_selectedElement,l_book);
		UIButton l_buyButton = m_selectedElement.getView ("buyBookButton") as UIButton;
		l_buyButton.addClickCallback(onClickBuyBookButton);
		UIButton bookButton = m_selectedElement.getView ("BookButton") as UIButton;
		bookButton.addClickCallback(onClickBuyBookButton);
//		UIButton l_openBook = m_selectedElement as UIButton;
//		l_openBook.addClickCallback(onOpenBookClick);
		if(null == l_book.icon)
			downLoadBookIcon( l_book, m_selectedElement );
		else
		{
			UIImage l_image = m_selectedElement.getView("BookImage") as UIImage;
			l_image.setTexture(l_book.icon);
		}
		m_selectedElement.active = true;
	}


	private void onOpenBookClick(UIButton p_button)
	{
		Book l_book = new Book();

		l_book = bookList[curretRecommendedBookIndex];

		if (null == l_book)
			return;
		else
		{
			if(l_book.owned)
			{
//				showLoadingDialog();

				m_uiManager.createScreen(UIScreen.LOADING_SPINNER_ELEPHANT);

				m_requestQueue.reset();
				m_requestQueue.add(new GetBookByIdRequest(l_book.id,getBookRequestComplete));
				m_requestQueue.request();
			}
			else
			{
//				m_uiManager.changeScreen(UIScreen.BOOK_LIST,false);
//				m_bookDetailsCanvas.setOriginalPosition();
//				m_bookDetailsCanvas.setAuthor(l_book.author);
//				m_bookDetailsCanvas.setBookName(l_book.title);
//				m_bookDetailsCanvas.setIllustrator(l_book.illustrator);
//				m_bookDetailsCanvas.setBookIcon(l_book.icon);
//				UIButton l_closeButton = m_bookDetailsCanvas.getView("closeMark") as UIButton;
//				l_closeButton.addClickCallback(closeBookDetails);
			}
		}
	}

	private void getBookRequestComplete(HttpsWWW p_response)
	{
		if(null == p_response.error)
		{
			string l_string = "";
			
			l_string = UnicodeDecoder.Unicode(p_response.text);
			l_string = UnicodeDecoder.UnicodeToChinese(l_string);
			l_string = UnicodeDecoder.CoverHtmlLabel(l_string);
			Hashtable l_jsonResponse = MiniJSON.MiniJSON.jsonDecode(l_string) as Hashtable;
			Book l_book = new Book(l_jsonResponse);
			SessionHandler.getInstance().currentBook = l_book;
			
			m_gameController.connectState(ZoodleState.BOOK_ACTIVITY, ZoodleState.OVERVIEW_INFO);

			m_gameController.changeState(ZoodleState.BOOK_ACTIVITY);
		}

		m_uiManager.removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);
	}

	//Once the requset book list is complete, update it in the SessionHandler
	//Then draw the reocmmmend book item
	private void _requestBookListComplete(HttpsWWW p_response)
	{
		if(null == p_response.error)
		{
			string l_string = "";
			
			l_string = UnicodeDecoder.Unicode(p_response.text);
			l_string = UnicodeDecoder.UnicodeToChinese(l_string);
			l_string = UnicodeDecoder.CoverHtmlLabel(l_string);
			Hashtable l_jsonResponse = MiniJSON.MiniJSON.jsonDecode(l_string) as Hashtable;
			if(l_jsonResponse.ContainsKey("jsonResponse"))
			{
				Hashtable l_response = l_jsonResponse["jsonResponse"] as Hashtable;
				if(l_response.ContainsKey("response"))
				{
					Hashtable l_bookData = l_response["response"] as Hashtable;
					ArrayList l_own = l_bookData["owned"] as ArrayList;
					ArrayList l_unown = l_bookData["unowned"] as ArrayList;
					int l_dataCount = l_own.Count;
					List<Book> l_list = new List<Book> ();
					for(int l_i = 0; l_i < l_dataCount; l_i++)
					{
						Hashtable l_table = l_own[l_i] as Hashtable;
						Book l_book = new Book(l_table);
						l_list.Add(l_book);
					}
					l_dataCount = l_unown.Count;
					for(int l_i = 0; l_i < l_dataCount; l_i++)
					{
						Hashtable l_table = l_unown[l_i] as Hashtable;
						Book l_book = new Book(l_table);
						l_list.Add(l_book);
					}
					bookList = l_list;

					drawRecommendedBookInfo();
				}
			}
			//SessionHandler.getInstance ().getBookIcon();
		}
	}

	private void onClickBuyBookButton(UIButton p_button)
	{
		m_wantedBook = null;

		m_wantedBook = bookList[curretRecommendedBookIndex];

		m_clickedBuyButton = p_button;
		confirmBuyBook(m_wantedBook);
	}


	private void downLoadBookIcon( Book p_book, UIElement p_element )
	{

		m_requestQueue.reset ();

		m_requestQueue.add (new BookIconRequest( p_book, p_element ));

		m_requestQueue.request();

	}

	public void confirmBuyBook(Book p_book)
	{
		UILabel l_titleLabel = m_confirmDialogCanvas.getView("titleText") as UILabel;
		//enough gems to buy the item
		if (SessionHandler.getInstance().currentKid.gems >= p_book.gems)
		{
			m_costArea.active = true;
			m_needMoreArea.active = false;
			
			l_titleLabel.text = Localization.getString(Localization.TXT_STATE_45_CONFIRM);
			UILabel nameLabel = m_costArea.getView("ItemName") as UILabel;
			nameLabel.text = p_book.title;
			if (p_book.icon != null)
			{
				UIImage image = m_costArea.getView("ItemImage") as UIImage;
				image.setTexture(p_book.icon);
			}
			UILabel currentGemsLabel = m_costArea.getView("CurrentGems") as UILabel;
			currentGemsLabel.text = SessionHandler.getInstance().currentKid.gems.ToString("N0");
			UILabel l_priceLabel = m_costArea.getView("priceText") as UILabel;
			l_priceLabel.text = p_book.gems.ToString("N0");
		}
		//not enough gems to buy items
		else
		{
			m_costArea.active = false;
			m_needMoreArea.active = true;
			
			l_titleLabel.text = Localization.getString(Localization.TXT_STATE_45_GEM_TITLE);
			UILabel currentGemsLabel = m_needMoreArea.getView("CurrentGems") as UILabel;
			currentGemsLabel.text = SessionHandler.getInstance().currentKid.gems.ToString("N0");
			UILabel nameLabel = m_needMoreArea.getView("ItemName") as UILabel;
			nameLabel.text = p_book.title;
			UILabel l_notice3_2 = m_needMoreArea.getView("noticeText3") as UILabel;
			l_notice3_2.text = string.Format(Localization.getString(Localization.TXT_75_LABEL_ITEM_COST), "book");
			UILabel l_priceLabel = m_needMoreArea.getView("costPriceText") as UILabel;
			l_priceLabel.text = p_book.gems.ToString();
		}

		m_uiManager.changeScreen(UIScreen.CONFIRM_DIALOG, true);
		m_confirmDialogCanvas.moveInDialog();
	}

	private void setGemsOnCostAreaOfConfirmDialog(string name, int gems)
	{
		m_costArea.active = true;
		m_needMoreArea.active = false;
		
		UIElement l_newPanel = m_confirmDialogCanvas.getView ("mainPanel");
		UILabel l_titleLabel = l_newPanel.getView("titleText") as UILabel;
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_45_CONFIRM);
		UILabel l_notice1label = l_newPanel.getView("noticeText1") as UILabel;
		l_notice1label.text = Localization.getString(Localization.TXT_STATE_45_PURCHASE);
		UILabel l_notice1label2 = l_newPanel.getView("noticeText2") as UILabel;
		l_notice1label2.text = name;
		UILabel l_priceLabel = l_newPanel.getView("priceText") as UILabel;
		l_priceLabel.text = gems.ToString();
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
			l_unlockLabel.text = Localization.getString (Localization.TXT_69_LABEL_UNLOCKED);
		}
		else
		{
//			l_lockImage.active = true;
			l_buyButton.active = true;
			l_unlockLabel.text = Localization.getString (Localization.TXT_69_LABEL_UNLOCK);
		}
	}

	private void editProfile(UIButton p_button)
	{
		if (checkInternet()) 
		{
			SessionHandler.getInstance ().inputedChildName = SessionHandler.getInstance ().currentKid.name;
			SessionHandler.getInstance ().inputedbirthday = SessionHandler.getInstance ().currentKid.birthday;
			SessionHandler.getInstance ().selectAvatar = string.Empty;
			SessionHandler.getInstance ().CreateChild = false;
			m_gameController.changeState (ZoodleState.CREATE_CHILD);
		}
	}

	private void onConfiemButtonClick( UIButton p_button )
	{
		if (null != m_wantedBook && null != m_clickedBuyButton)
		{
			if(SessionHandler.getInstance().currentKid.gems >= m_wantedBook.gems)
				sendBuyBookRequest();
		}
	}

	private void setGemsOnNeedMoreAreaOfConfirmDialog(int gems)
	{
		UILabel l_costGems = m_confirmDialogCanvas.getView("costPriceText") as UILabel;
		UILabel l_needGems = m_confirmDialogCanvas.getView("needPriceText") as UILabel;
		l_costGems.text = gems.ToString();
		l_needGems.text = (gems - SessionHandler.getInstance().currentKid.gems).ToString();
		UILabel l_titleLabel = m_confirmDialogCanvas.getView("titleText") as UILabel;
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_45_GEM_TITLE);
		UILabel l_notice1label = m_confirmDialogCanvas.getView("noticeText1") as UILabel;
		l_notice1label.text = Localization.getString(Localization.TXT_STATE_45_GEM_INFO);
		m_costArea.active = false;
		m_needMoreArea.active = true;
	}
	
	private void sendBuyBookRequest()
	{
		m_requestQueue.reset ();
		m_requestQueue.add (new BuyBookRequest (m_wantedBook, m_clickedBuyButton, m_confirmDialogCanvas ,m_selectedElement));
		m_requestQueue.request ();
		m_uiManager.changeScreen(UIScreen.CONFIRM_DIALOG, false);
	}

	private void installApp( string p_packageName )
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass l_jcPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject l_joActivity = l_jcPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		
		AndroidJavaClass jc_uri = new AndroidJavaClass("android.net.Uri");
		
		AndroidJavaObject l_uri = jc_uri.CallStatic<AndroidJavaObject>("parse", string.Format("market://details?id={0}", p_packageName));
		
		AndroidJavaClass jc_intent = new AndroidJavaClass("android.content.Intent");
		
		AndroidJavaObject jo_view = jc_intent.GetStatic<AndroidJavaObject>("ACTION_VIEW");
		
		AndroidJavaObject jo_intent = new AndroidJavaObject("android.content.Intent", jo_view, l_uri);

		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");
		jc.CallStatic("installRecommand");
		
		AndroidJavaObject jo_chooser = jc_intent.CallStatic<AndroidJavaObject>("createChooser", jo_intent, Localization.getString(Localization.TXT_STATE_45_MARKET));
		
		l_joActivity.Call("startActivity", jo_chooser );
		
		GAUtil.logInstallApp(p_packageName);
		#endif
	}

	private void onExitConfiemDialogButtonClick( UIButton p_button )
	{
		m_uiManager.changeScreen(UIScreen.CONFIRM_DIALOG,false);
		m_confirmDialogCanvas.moveOutDialog();
	}

	private void loadAppDetail()
	{
		if(null == m_appDetailsCanvas)
		{
			return;
		}
		UILabel l_appCostText = m_appDetailsCanvas.getView("appCostText") as UILabel;
		UIButton l_buyAppButton = m_appDetailsCanvas.getView("buyAppButton") as UIButton;
		UILabel l_appFreeText = m_appDetailsCanvas.getView("appFreeText") as UILabel;
		
		if(m_app.gems == 0)
		{
			l_appCostText.active = false;
			l_buyAppButton.active = false;
			l_appFreeText.active = true;
		}
		else
		{
			if(m_app.own)
			{
				l_appCostText.active = false;
				l_buyAppButton.active = false;
				l_appFreeText.active = false;
			}
			else if(!m_app.own)
			{
				l_appFreeText.active = false;
				l_appCostText.active = true;
				l_buyAppButton.active = true;
			}
		}
		
		Token l_token = SessionHandler.getInstance ().token;
		if( l_token.isPremium() || l_token.isCurrent() )
		{
			l_appCostText.active = false;
			l_appFreeText.active = false;
			l_buyAppButton.active = true;
			UILabel l_text = l_buyAppButton.getView( "Text" ) as UILabel;
			l_text.text = Localization.getString(Localization.TXT_STATE_45_INSTALL);
		}
		
		resetSubjectColor ();
		_setupSignleApp (m_appDetailsCanvas,m_app);
		
		UILabel l_description = m_appDetailsCanvas.getView("appDescriptionText") as UILabel;
		UILabel l_violence = m_appDetailsCanvas.getView("violenceLevelText") as UILabel;
		UILabel l_age = m_appDetailsCanvas.getView("ageText") as UILabel;
		UIImage l_icon = m_appDetailsCanvas.getView ("appImage") as UIImage;
		UIButton l_buyButton = m_appDetailsCanvas.getView ("buyAppButton") as UIButton;
		l_buyButton.removeClickCallback (buyApp);
		l_buyButton.addClickCallback (buyApp);
		
		l_description.text = m_app.description;
		l_violence.text = m_app.violence.ToString();
		l_age.text = m_app.ageMin.ToString() +"-"+ m_app.ageMax.ToString();
		if(null != m_app.icon)
		{
			l_icon.setTexture(m_app.icon);
		}
	}

	private void showAppDetail(UIButton p_button)
	{
		isAppDetailsScreen = true;
		m_uiManager.changeScreen(UIScreen.APP_DETAILS, true);
		m_appDetailsCanvas.moveInDialog();
	}

	private void buyApp(UIButton p_button)
	{
		Token l_token = SessionHandler.getInstance ().token;
		if( l_token.isPremium() || l_token.isCurrent() )
		{
			installApp(m_app.packageName);
		}
	}

	private void resetSubjectColor()
	{
		UIElement l_readingColor = m_appDetailsCanvas.getView ("readingColor") as UIElement;
		l_readingColor.active = false;
		
		UIElement l_scienceColor = m_appDetailsCanvas.getView ("scienceColor") as UIElement;
		l_scienceColor.active = false;
		
		UIElement l_socialColor = m_appDetailsCanvas.getView ("socialColor") as UIElement;
		l_socialColor.active = false;
		
		UIElement l_cognitiveColor = m_appDetailsCanvas.getView ("cognitiveColor") as UIElement;
		l_cognitiveColor.active = false;
		
		UIElement l_creativeColor = m_appDetailsCanvas.getView ("creativeColor") as UIElement;
		l_creativeColor.active = false;
		
		UIElement l_lifeSkillsColor = m_appDetailsCanvas.getView ("lifeSkillsColor") as UIElement;
		l_lifeSkillsColor.active = false;
		
		UIElement l_mathColor = m_appDetailsCanvas.getView ("mathColor") as UIElement;
		l_mathColor.active = false;
	}

	private void iconRequestComplete(HttpsWWW p_response)
	{
		if(null != p_response.error)
		{
			_Debug.log(p_response);
		}
		else
		{
			App l_app = SessionHandler.getInstance().currentKid.topRecommendedApp;
			l_app.iconDownload = true;
			l_app.icon = p_response.texture;
			if(null != m_topRecommendAppArea)
			{
				UIImage l_image = m_topRecommendAppArea.getView("appImage") as UIImage;
				
				if( null == l_image )
				{
					return;
				}
				
				l_image.setTexture(p_response.texture);
			}

			if(null != m_appDetailsCanvas)
			{
				UIImage l_image = m_appDetailsCanvas.getView("appImage") as UIImage;
				
				if( null == l_image )
				{
					return;
				}
				
				l_image.setTexture(p_response.texture);
			}

		}
	}

	private void _setupSignleApp(UIElement p_element, App p_app)
	{
		UILabel l_appName = p_element.getView ("appNameText") as UILabel;
		if(null != l_appName)
			l_appName.text = p_app.name;
		UILabel l_appCostText = p_element.getView("appCostText") as UILabel;
		UILabel l_appFreeText = p_element.getView("appFreeText") as UILabel;
		UILabel l_sponsoredText = p_element.getView("sponsoredText") as UILabel;
		if(null == p_app.icon)
		{
			m_getTopRecommendAppRequestQueue.reset();
			m_getTopRecommendAppRequestQueue.add(new IconRequest(p_app,p_element,iconRequestComplete));
			m_getTopRecommendAppRequestQueue.request();
		}
		else
		{
			UIImage l_image = p_element.getView("appImage") as UIImage;
			
			if( null == l_image )
			{
				return;
			}
			
			l_image.setTexture(p_app.icon);
		}
		if(p_app.gems == 0)
		{
			if(null != l_appCostText)
				l_appCostText.active = false;
			if(null != l_appFreeText)
				l_appFreeText.active = true;
			if(null != l_sponsoredText)
				l_sponsoredText.active = true;
		}
		else
		{
			if(p_app.own)
			{
				if(null != l_appCostText)
					l_appCostText.active = false;
				if(null != l_appFreeText)
					l_appFreeText.active = false;
				if(null != l_sponsoredText)
					l_sponsoredText.active = false;
			}
			else if(!p_app.own)
			{
				if(null != l_appFreeText)
					l_appFreeText.active = false;
				if(null != l_sponsoredText)
					l_sponsoredText.active = false;
				if(null != l_appCostText)
				{
					l_appCostText.active = true;
					l_appCostText.text = p_app.gems.ToString();
				}
			}
		}
		
		Token l_token = SessionHandler.getInstance ().token;
		if( l_token.isPremium() || l_token.isCurrent() )
		{
			if(null != l_appCostText)
				l_appCostText.active = false;
			if(null != l_appFreeText)
				l_appFreeText.active = false;
		}

		Dictionary< string, int > l_subjects = p_app.subjects;
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_MATH))
		{
			UIElement l_mathColor = p_element.getView ("mathColor") as UIElement;
			if(null != l_mathColor)
			l_mathColor.active = true;
		}
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_LANGUAGE))
		{
			UIElement l_readingColor = p_element.getView ("readingColor") as UIElement;
			if(null != l_readingColor)
			l_readingColor.active = true;
		}
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_SCIENCE))
		{
			UIElement l_scienceColor = p_element.getView ("scienceColor") as UIElement;
			if(null != l_scienceColor)
				l_scienceColor.active = true;
		}
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_SOCIAL_SCIENCE))
		{
			UIElement l_socialColor = p_element.getView ("socialColor") as UIElement;
			if(null != l_socialColor)
			l_socialColor.active = true;
		}
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_CONITIVE_DEVELOPMENT))
		{
			UIElement l_cognitiveColor = p_element.getView ("cognitiveColor") as UIElement;
			if(null != l_cognitiveColor)
			l_cognitiveColor.active = true;
		}
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_CREATIVE_DEVELOPMENT))
		{
			UIElement l_creativeColor = p_element.getView ("creativeColor") as UIElement;
			if(null != l_creativeColor)
			l_creativeColor.active = true;
			
		}
		
		if(l_subjects.ContainsKey(AppTable.COLUMN_LIFE_SKILLS))
		{
			UIElement l_lifeSkillsColor = p_element.getView ("lifeSkillsColor") as UIElement;
			if(null != l_lifeSkillsColor)
			l_lifeSkillsColor.active = true;
		}

		if(null != p_element && null != p_element.gameObject)
			p_element.active = true;
	}

	private void getTopRecommendRequestComplete(HttpsWWW p_response)
	{
		if(null == p_response.error && !"null".Equals(p_response.text))
		{
			string l_string = UnicodeDecoder.Unicode(p_response.text);
			l_string = UnicodeDecoder.UnicodeToChinese(l_string);
			Hashtable l_hashTable = MiniJSON.MiniJSON.jsonDecode(l_string) as Hashtable;
			App l_app = new App(l_hashTable);
			if(l_hashTable.ContainsKey("owned"))
			{
				l_app.own = (bool)l_hashTable["owned"];
			}
			SessionHandler.getInstance().currentKid.topRecommendedApp = l_app;
			m_app = l_app;
		}
	}

	private void onExitAppDetailsButtonClick( UIButton p_button )
	{
		isAppDetailsScreen = false;
		m_uiManager.changeScreen (UIScreen.APP_DETAILS, false);
		m_appDetailsCanvas.moveOutDialog();
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
	
	private void toBuyGemsScreen(UIButton p_button)
	{
		SwrveComponent.Instance.SDK.NamedEvent("GotoGetGemsFromLeftMenu");

		gotoGetGems();
	}
	
	private void gotoGetGems()
	{	
		string l_returnJson = SessionHandler.getInstance ().GemsJson;
		
		if(l_returnJson.Length > 0)
		{
			Hashtable l_date = MiniJSON.MiniJSON.jsonDecode (l_returnJson) as Hashtable;
			if(l_date.ContainsKey("jsonResponse"))
			{
				m_uiManager.changeScreen(UIScreen.CONFIRM_DIALOG, false);
				m_gameController.connectState( ZoodleState.BUY_GEMS, int.Parse(m_gameController.stateName) );
				m_gameController.changeState (ZoodleState.BUY_GEMS);
			}
			else
			{
				doViewGemsRequest();
			}
		}
		else
		{
			doViewGemsRequest();
		}
	}

	private void doViewGemsRequest()
	{
		Server.init(ZoodlesConstants.getHttpsHost());
		m_requestQueue.reset();
		m_requestQueue.add(new ViewGemsRequest(viewGemsRequestComplete));
		m_requestQueue.request();
	}
	
	private void viewGemsRequestComplete(HttpsWWW p_response)
	{
		Server.init (ZoodlesConstants.getHttpsHost());
		if(p_response.error == null)
		{
			m_uiManager.changeScreen(UIScreen.CONFIRM_DIALOG, false);
			SessionHandler.getInstance ().GemsJson = p_response.text;
			m_gameController.connectState( ZoodleState.BUY_GEMS, int.Parse(m_gameController.stateName) );
			m_gameController.changeState (ZoodleState.BUY_GEMS);
		}
		else
		{
			m_uiManager.changeScreen(UIScreen.CONFIRM_DIALOG, false);
			m_confirmDialogCanvas.moveOutDialog();
			setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_11_FAIL),Localization.getString(Localization.TXT_STATE_11_FAIL_DATA));
		}
	}

	private UIManager m_uiManager;
	private App m_app;

	private UICanvas m_dashboardInfoCanvas;
	private AppDetailsCanvas m_appDetailsCanvas;
	private ConfirmDialogCanvas	m_confirmDialogCanvas;

	private UIElement m_costArea;
	private UIButton m_buyGemsButton;
	private UIButton m_buyGemsButtonOnConfirm;
	private UIElement m_needMoreArea;
	private UIButton m_exitConfirmDialogButton;
	private UIButton m_cancelBuyButton;
	private UIButton m_confirmBuyButton;

	private UIButton m_exitAppDetailsButton;

	private UIButton m_editProfileButton;

	private UIButton m_topRecommendAppArea;
	private RequestQueue m_requestQueue;
	private RequestQueue m_getTopRecommendAppRequestQueue;
	
	private bool 		canLoadTopRecommandApp;


	//Kevin
	//Added index for current recommended book
	private int curretRecommendedBookIndex;
	//Current book that is recommended and clicked
	private Book	 			      m_wantedBook;
	//Buy button for the book
	private UIButton 			      m_clickedBuyButton;
	//Used to call Game script functions
//	private Game game;
	private List<Book> bookList;
	//selected top recommended book ui element
	//currently, only have one top recommended book ui element
	private UIElement m_selectedElement;

	private bool isAppDetailsScreen = false;
}
