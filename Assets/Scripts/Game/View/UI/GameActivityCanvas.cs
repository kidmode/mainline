using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class GameActivityCanvas : UICanvas
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );

        UILabel l_allGamesLabel = getView("allContentLabel") as UILabel;
        l_allGamesLabel.text    = Localization.getString(Localization.TXT_TAB_ALL_GAMES);

        //UILabel l_featuredGameLabel = getView("featuredLabel") as UILabel;
        //l_featuredGameLabel.text    = Localization.getString(Localization.TXT_LABEL_FEATURED_GAMES);

        //UILabel l_gamesLabel    = getView("contentLabel") as UILabel;
        //l_gamesLabel.text       = Localization.getString(Localization.TXT_LABEL_GAMES);

        UILabel l_favorateLabel = getView("favorateLabel") as UILabel;
        l_favorateLabel.text    = Localization.getString(Localization.TXT_TAB_FAVORITES);

        UIToggle l_favorateTab = getView("favorateTab") as UIToggle;
        l_favorateTab.addValueChangedCallback(onFeaturedToggled);

        UIToggle l_allContentTab = getView("allContentTab") as UIToggle;
        l_allContentTab.addValueChangedCallback(onAllToggled);

        m_gameSwipeList = getView("allContentScrollView") as UISwipeList;

        m_gameFavorateSwipeList = getView("favorateScrollView") as UISwipeList;
        m_gameFavorateSwipeList.active = false;

		m_gameFavorateInfo = getView("favoriteInfo") as UILabel;
		m_gameInfo = getView("info") as UILabel;

		_setupList();
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

		GameInfo l_game = p_data as GameInfo;
		DebugUtils.Assert( l_game != null );

        //*Temporary*
        UILabel l_numberLabel = p_element.getView("number") as UILabel;
        DebugUtils.Assert(l_numberLabel != null);

        l_numberLabel.text = (p_index + 1).ToString();

		UIImage l_rawImage = p_element.getView("icon") as UIImage;
		UIImage l_appImage = p_element.getView("appIcon") as UIImage;
		UILabel l_appName = p_element.getView("appName") as UILabel;
		l_appName.active = false;

        if (l_rawImage == null)
            return;

		if( l_game.isWebView )
		{
			WebViewInfo l_info = l_game.webViewData;

			if( l_info.icon == null )
			{
				l_rawImage.setTexture( new Texture2D(1, 1) );
			}
			else
			{
				l_rawImage.setTexture(l_info.icon);
			}

			if (l_appImage != null)
				l_appImage.active = false;
		}
		else
		{
			AppInfo l_info = l_game.appData;
			l_appName.text = l_info.appName;
			l_appName.active = true;

			Vector2 l_textSize = l_appName.calcSize();
			RectTransform l_transform = l_appName.gameObject.GetComponent<RectTransform>();
			float l_scale = Mathf.Min(l_transform.sizeDelta.x / l_textSize.x, 1.0f);
			l_transform.localScale = new Vector3(l_scale, l_scale, 1);
			
			if( l_info.appIcon == null )
			{
				l_appImage.setTexture( new Texture2D(1, 1) );
			}
			else
			{
				l_appImage.setTexture(l_info.appIcon);
			}
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
        m_gameSwipeList.redraw();

        m_gameFavorateSwipeList.setDrawFunction(onListDraw);
        m_gameFavorateSwipeList.redraw();
	}

    private UISwipeList     m_gameSwipeList;
    private UISwipeList     m_gameFavorateSwipeList;
	private UILabel			m_gameFavorateInfo;
	private UILabel			m_gameInfo;

	private List< Button > 	m_buttonList;
}
