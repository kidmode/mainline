using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class GameActivityCanvas : UICanvas
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
//		getView("mainPanel").tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );

        UIToggle l_favorateTab = getView("favorateTab") as UIToggle;
        l_favorateTab.addValueChangedCallback(onFeaturedToggled);

        UIToggle l_allContentTab = getView("allContentTab") as UIToggle;
        l_allContentTab.addValueChangedCallback(onAllToggled);

        m_gameSwipeList = getView("allContentScrollView") as UISwipeList;

        m_gameFavorateSwipeList = getView("favorateScrollView") as UISwipeList;
        m_gameFavorateSwipeList.active = false;

		m_gameFavorateInfo = getView("favoriteInfo") as UILabel;
		m_gameInfo = getView("info") as UILabel;

		m_emptyTexture = new Texture2D (1, 1);
		m_emptyTexture.SetPixel(0, 0, new Color(0.88f, 0.88f, 0.88f, 1.0f));
		m_emptyTexture.Apply();

		_setupList();
		
		SetupLocalizition ();
		setScrollView ();
	}
	
	public override void enteringTransition(  )
	{
		base.enteringTransition( );
		
		tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
	}
	
	public override void exitingTransition( )
	{
		base.exitingTransition( );
	}

	//----------------- Private Implementation -------------------
	
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}
	
//    private void onAssessmentClicked( UIButton p_button )
//    {
//		SessionHandler l_session = SessionHandler.getInstance();
//        string l_baseURL = "http://dev.zoodles.com/assessments/Game1/index.html";
//
//		//Create kid param
//		Kid l_kid = l_session.currentKid;
//		string l_kidID = "kid_id=" + l_kid.id;
//
//		//Create token param
//		Token l_tokenObject = l_session.token; //why?
//		string l_tokenSecret = l_tokenObject.getSecret();
//		string l_token = "token=" + l_tokenSecret;
//
//		//Put url and params together
//		string l_url = string.Format("{0}?{1}&{2}", l_baseURL, l_kidID, l_token);
//
//		_playGame(l_url);
//    }

    private void onFeaturedToggled(UIToggle p_toggle, bool p_isOn)
    {
        if (p_isOn)
        {
			moveTotal = 0;
			move = 0;
			isFavor = true;
			m_favorcontentPanel.localPosition = new Vector3 (0, 0, 0);
            m_gameSwipeList.active = false;
	        m_gameFavorateSwipeList.active = true;
			m_gameFavorateInfo.active = (m_gameFavorateSwipeList.getData().Count <= 0);
			m_gameInfo.active = false;
        }
    }

    private void onAllToggled(UIToggle p_toggle, bool p_isOn)
    {
        if (p_isOn)
        {
			moveTotal = 0;
			move = 0;
			isFavor = false;
			m_contentPanel.localPosition = new Vector3 (0, 0, 0);
            m_gameSwipeList.active = true;
            m_gameFavorateSwipeList.active = false;
			m_gameFavorateInfo.active = false;
			m_gameInfo.active = (m_gameSwipeList.getData().Count <= 0);
        }
    }

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UIButton l_button = p_element as UIButton;
		DebugUtils.Assert( l_button != null );

		AppInfo appInfo = p_data as AppInfo;

		GameInfo l_game = p_data as GameInfo;



		UILabel l_numberLabel = p_element.getView("number") as UILabel;
		DebugUtils.Assert(l_numberLabel != null);
		
		UIImage l_rawImage = p_element.getView("icon") as UIImage;
		UIImage l_appImage = p_element.getView("appIcon") as UIImage;
		UILabel l_appName = p_element.getView("appName") as UILabel;

		UIImage l_backgroundImage = p_element.getView("background") as UIImage;
		//honda: we don't use shadow anymore, we use panel's shadow
		UIImage l_shadowImage = p_element.getView("shadow") as UIImage;
		l_shadowImage.active  = false;
		UIImage l_panel = p_element.getView("Panel") as UIImage;
		UIImage l_PanelShadow = p_element.getView("PanelShadow") as UIImage;
		Mask mask = l_panel.gameObject.GetComponent<Mask>();

		//==========
		//Update List item so we could get the index latter on other scripts
		ListItem item = p_element.transform.gameObject.GetComponent<ListItem>();
		item.index = p_index;

		//PanelShadow

		if(l_game != null)
		{
//		DebugUtils.Assert( l_game != null );

			//honda: Debug mode
			UILabel title = p_element.getView("AddTitle") as UILabel;
			UIImage plus = p_element.getView("Plus") as UIImage;
			UILabel name = p_element.getView("NameTitle") as UILabel;
			UIButton removeBtn = p_element.getView("RemoveButton") as UIButton;
			if (l_game.webViewData.infoStatus == WebViewInfoStatus.AddItem)
			{
				mask.enabled = true;

				title.active = true;
				plus.active = true;
				name.active = false;
				removeBtn.active = false;
				removeBtn.enabled = false;
				l_rawImage.setTexture( m_emptyTexture );
				l_appImage.active = false;
				l_appName.active = false;
				l_numberLabel.active = false;
				return;
			}
			else if (l_game.webViewData.infoStatus == WebViewInfoStatus.FromLocal || l_game.webViewData.infoStatus == WebViewInfoStatus.FromGDrive)
			{
				mask.enabled = true;

				title.active = false;
				plus.active = false;
				name.active = false;
//				name.text = l_game.webViewData.webData.name;
				removeBtn.active = false;//true;
				removeBtn.enabled = false;//true;
				l_rawImage.setTexture( m_emptyTexture );
				l_appImage.active = false;
				l_appName.gameObject.transform.parent = p_element.gameObject.transform;
				l_appName.active = true;
				l_appName.text = l_game.webViewData.webData.name;
				l_numberLabel.active = false;
				return;
			}
			else
			{
				title.active = false;
				plus.active = false;
				name.active = false;
				removeBtn.active = false;
				removeBtn.enabled = false;
				l_numberLabel.active = false;
			}
			//end debug mode


	//        l_numberLabel.text = (p_index + 1).ToString();
			if (l_appImage != null)
				l_appImage.active = false;
		
			if (l_rawImage == null)
	            return;

			if( l_game.isWebView )
			{
				WebViewInfo l_info = l_game.webViewData;

				//honda comment: requestIcon check icon from local or server
				if( !l_info.iconRequested )
				{
					l_info.requestIcon();
				}

				l_appName.active = true;
				l_appName.text = StringManipulator.getShortenOneLineAppName( l_game.webViewData.webData.name );

				if( l_info.icon == null )
				{
					l_rawImage.setTexture( m_emptyTexture );

				}
				else if (l_info.icon != null)
				{
					l_rawImage.setTexture(l_info.icon);
					l_rawImage.active = true;
					if (l_appImage != null)
						l_appImage.active = false;
				}

				//===================
				mask.enabled = true;

				//Need to move the app icon info to the child of the 
//				l_appName.gameObject.transform.parent = p_element.gameObject.transform;

				l_appName.gameObject.transform.SetParent(p_element.gameObject.transform, true);

				l_PanelShadow.active = true;
			}
		}
		else
		{
			AppInfo l_info =  p_data as AppInfo;//l_game.appData;
			l_appName.text = StringManipulator.getShortenOneLineAppName( l_info.appName );
			if (l_appName.active == false)
			{
				l_appName.active = true;
			}

			Vector2 l_textSize = l_appName.calcSize();
			RectTransform l_transform = l_appName.gameObject.GetComponent<RectTransform>();
//			float l_scale = Mathf.Min(l_transform.sizeDelta.x / l_textSize.x, 1.0f);
//			l_transform.localScale = new Vector3(l_scale, l_scale, 1);
			
			if( l_info.appIcon == null )
			{
				l_appImage.setTexture( m_emptyTexture );
				l_rawImage.active = false;
			}
			else if (l_info.appIcon != null)
			{
				l_appImage.setTexture(l_info.appIcon);
				l_appImage.active = true;
				l_rawImage.active = false;
			}

			//==========================================================================================
			//Turn off the mask
			mask.enabled = false;

			//===================
			//Need to move the app icon info to the child of the 
//			l_appImage.gameObject.transform.parent = p_element.gameObject.transform;// l_panel.gameObject.transform;
//			l_appName.gameObject.transform.parent = p_element.gameObject.transform;
			l_appImage.gameObject.transform.SetParent(p_element.gameObject.transform, true);
			l_appName.gameObject.transform.SetParent(p_element.gameObject.transform, true);


			//Turn off the background image
			l_backgroundImage.gameObject.SetActive(false);
			l_backgroundImage.active = false;
			//Turn off the panel shadow
			l_PanelShadow.gameObject.SetActive(false);
			l_panel.active = false;
		}
	}

	private void _setupList()
	{
        //List<System.Object> l_infoData = new List<System.Object>();
        //l_infoData.Add( new WebViewInfo( null, "http://www.zoodles.com/games/SeahorseJump/index.html"));
		//l_infoData.Add( new WebViewInfo( null, "http://www.zoodles.com/games/Woblox/index.html" ) );
		//l_infoData.Add( new WebViewInfo( null, "http://www.zoodles.com/games/KidsShapesLearningPuzzle/index.html" ) );
		//l_infoData.Add( new WebViewInfo( null, "http://www.zoodles.com/games/MathGame/index.html" ) );
		//l_infoData.Add( new WebViewInfo( null, "http://www.zoodles.com/games/circus/index.html" ) );
		//l_infoData.Add( new WebViewInfo( null, "http://www.zoodles.com/games/colorBall/index.html" ) );
		//l_infoData.Add( new WebViewInfo( null, "http://www.tinglygames.com/games/monstersnake/" ) );
        //l_infoData.Add( new WebViewInfo( null, "http://www.zoodles.com/games/numbersGarden/index.html"));
		//
		//l_swipe.setData( l_infoData );
        m_gameSwipeList.setDrawFunction(onListDraw);
        m_gameFavorateSwipeList.setDrawFunction(onListDraw);
	}

	private void SetupLocalizition()
	{		
		UILabel l_allGamesLabel 	= getView("allContentLabel") as UILabel;
		UILabel l_favorateLabel 	= getView("favorateLabel") as UILabel;
		UILabel l_allContentLabelOff   = getView("allContentLabelOff") as UILabel;
		UILabel l_favorateLabelOff 	= getView("favorateLabelOff") as UILabel;
		UILabel l_infoLabel 		= getView("info") as UILabel;
		UILabel l_favorateInfoLabel = getView("favoriteInfo") as UILabel;
		UILabel l_headerLabel 		= getView("header") as UILabel;

		l_allGamesLabel.text    	= Localization.getString(Localization.TXT_TAB_ALL_GAMES);
		l_favorateLabel.text 		= Localization.getString(Localization.TXT_TAB_FAVORITES);
		l_allContentLabelOff.text = l_allGamesLabel.text;
		l_favorateLabelOff.text = l_favorateLabel.text;
//		l_infoLabel.text 			= Localization.getString(Localization.TXT_12_LABEL_INFO);
//		l_favorateInfoLabel.text 	= Localization.getString(Localization.TXT_12_LABEL_FAVORITE);
		l_infoLabel.text 			= Localization.getString(Localization.TXT_LABEL_LOADING);
		l_favorateInfoLabel.text 	= Localization.getString(Localization.TXT_LABEL_LOADING);
		l_headerLabel.text 			= Localization.getString(Localization.TXT_12_LABEL_HEADER);
	}


	public override void update()
	{
		base.update();
		
		if (mMovingOffset == 0) {
			if(isFavor)
				move = m_favorcontentPanel.localPosition.x;
			else
				move = m_contentPanel.localPosition.x;
			moveTotal = move;
		}
		if (mMovingOffset > 0) {
			move += mMovingOffset * MOVESPEED;
			if (move <= moveTotal) {
				if(isFavor)
					m_favorcontentPanel.localPosition = new Vector3 (move, 0, 0);
				else
					m_contentPanel.localPosition = new Vector3 (move, 0, 0);
			}
			else {
				if(!m_leftButton.active) {
					mMovingOffset = 0;
					if(isFavor)
						moveTotal = m_favorcontentPanel.localPosition.x;
					else
						moveTotal = m_contentPanel.localPosition.x;
				}
				else {
					moveTotal += (mMovingOffset * OFFSET);
					mMovingOffset = 0;
				}
				
			}
		} else if (mMovingOffset < 0) {
			
			move += mMovingOffset * MOVESPEED;
			if (move > moveTotal) {
				if(isFavor)
					m_favorcontentPanel.localPosition = new Vector3 (move, 0, 0);
				else
					m_contentPanel.localPosition = new Vector3 (move, 0, 0);
			}
			else {
				if(!m_rightButton.active) {
					mMovingOffset = 0;
					if(isFavor)
						moveTotal = m_favorcontentPanel.localPosition.x;
					else
						moveTotal = m_contentPanel.localPosition.x;
				}
				else {
					moveTotal += (mMovingOffset * OFFSET);
					mMovingOffset = 0;
				}
				
			}
		}
		
	}
	
	public void onLeftButtonClick( UIButton p_button )
	{

		Debug.Log("   < - -- - - - - ----- - -- -  -- -   onLeftButtonClick    " );

		mMovingOffset = 1;
		if (move == moveTotal) {
			moveTotal = move + mMovingOffset * OFFSET;
		}
	}
	
	public void onRightButtonClick( UIButton p_button )
	{

		Debug.Log("  onRightButtonClick   -- - - - - - - - - -  - ->" );

		mMovingOffset = -1;
		if (moveTotal == 0) {
			if(isFavor)
				move = m_favorcontentPanel.localPosition.x;
			else
				move = m_contentPanel.localPosition.x;
			moveTotal = move + mMovingOffset * OFFSET;
		} else if (move == moveTotal) {
			moveTotal = move + mMovingOffset * OFFSET;
		}
	}
	
	public void onLeftFavorButtonClick( UIButton p_button )
	{
		mMovingOffset = 1;
		if (move == moveTotal) {
			moveTotal = move + mMovingOffset * OFFSET;
		}
	}
	
	public void onRightFavorButtonClick( UIButton p_button )
	{
		mMovingOffset = -1;
		if (moveTotal == 0) {
			if(isFavor)
				move = m_favorcontentPanel.localPosition.x;
			else
				move = m_contentPanel.localPosition.x;
			moveTotal = move + mMovingOffset * OFFSET;
		} else if (move == moveTotal) {
			moveTotal = move + mMovingOffset * OFFSET;
		}
	}
	
	void setScrollView() {
		
		m_contentPanel = getView ("allContentScrollView").getChildAt(0).gameObject.GetComponent<RectTransform>();
		m_favorcontentPanel = getView ("favorateScrollView").getChildAt(0).gameObject.GetComponent<RectTransform>();
		
		m_leftButton = getView( "allContentArrowLeft" ) as UIButton;
		m_rightButton = getView( "allContentArrowRight" ) as UIButton;
		m_favorleftButton = getView( "favorateScrollArrowLeft" ) as UIButton;
		m_favorrightButton = getView( "favorateScrollArrowRight" ) as UIButton;
		
		m_leftButton.addClickCallback( onLeftButtonClick );
		m_rightButton.addClickCallback( onRightButtonClick );
		
		m_favorleftButton.addClickCallback( onLeftFavorButtonClick );
		m_favorrightButton.addClickCallback( onRightFavorButtonClick );
	}


    private UISwipeList     m_gameSwipeList;
    private UISwipeList     m_gameFavorateSwipeList;
	private UILabel			m_gameFavorateInfo;
	private UILabel			m_gameInfo;

	private List< Button > 	m_buttonList;

	private Texture2D m_emptyTexture;

	private RectTransform m_contentPanel;
	private RectTransform m_favorcontentPanel;

	private UIButton m_leftButton;
	private UIButton m_rightButton;
	private UIButton m_favorleftButton;
	private UIButton m_favorrightButton;
	
	private float OFFSET = 500;
	private float MOVESPEED = 30;
	private float mMovingOffset = 0;
	private float move = 0;
	private float moveTotal;
	private bool isFavor = false; // true: favor false: all

}
