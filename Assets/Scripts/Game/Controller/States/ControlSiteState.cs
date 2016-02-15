﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlSiteState : GameState
{
	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);
		
		m_uiManager = m_gameController.getUI();
		m_requestQueue = new RequestQueue();
		_setupScreen( p_gameController );
		_setupElment();
	}
	
	public override void update (GameController p_gameController, int p_time)
	{
		base.update (p_gameController, p_time);
	}
	
	public override void exit (GameController p_gameController)
	{
		checkRequest ();
		
		base.exit (p_gameController);
		
		m_uiManager.removeScreen( UIScreen.LEFT_MENU );
		m_uiManager.removeScreen( UIScreen.COMMON_DIALOG );
		
		m_uiManager.removeScreen( UIScreen.ADD_SITE );
		m_uiManager.removeScreen( UIScreen.PAYWALL );
	}

	private void _setupScreen( GameController p_gameController )
	{
		m_commonDialog 				= m_uiManager.createScreen( UIScreen.COMMON_DIALOG, true, 5 ) 			as CommonDialogCanvas;
		m_commonDialog.setUIManager (p_gameController.getUI());
		m_leftMenuCanvas 			= m_uiManager.createScreen( UIScreen.LEFT_MENU, true, 4 ) 				as LeftMenuCanvas;

		if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
		{
			m_paywallCanvas = m_uiManager.createScreen( UIScreen.PAYWALL, false, 2 );
			m_upgradeButton = m_paywallCanvas.getView( "upgradeButton" ) as UIButton;
			m_upgradeButton.addClickCallback( onUpgradeButtonClick );
		}

		m_addSiteCanvas 			= m_uiManager.createScreen( UIScreen.ADD_SITE, true, 1 ) 				as AddSiteCanvas;

		if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
		{
			m_uiManager.setScreenEnable( UIScreen.ADD_SITE, false );
		}
	}

	private void _setupElment()
	{

		m_helpButton = m_addSiteCanvas.getView ("helpButton") as UIButton;
		m_helpButton.addClickCallback (onHelpButtonClick);

		
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_addSiteCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, 830, 0 ));
		l_newPanel.tweener.addPositionTrack( l_pointListIn, 0f );
		l_newPanel.tweener.addAlphaTrack( 0.0f, 1.0f, 0.5f, onShowFinish, Tweener.Style.Standard, false);
		
		m_menu = m_leftMenuCanvas.getView ("LeftMenu") as UIElement;
		m_showProfileButton = m_menu.getView ("profileButton") as UIButton;
		m_showProfileButton.addClickCallback (toShowAllChilren);
		
		m_closeLeftMenuButton = m_leftMenuCanvas.getView ("closeButton") as UIButton;
		m_settingButton = 		m_leftMenuCanvas.getView ("settingButton") 	as UIButton;
		m_childrenList = 		m_leftMenuCanvas.getView ("childSwipeList") as UISwipeList;
		m_tryPremiumButton = 	m_leftMenuCanvas.getView ("premiumButton") 	as UIButton;
		m_buyGemsButton = 		m_leftMenuCanvas.getView ("buyGemsButton") 	as UIButton;
		m_closeLeftMenuButton.	addClickCallback (onCloseMenu);
		m_settingButton.		addClickCallback (toSettingScreen);
		m_childrenList.			addClickListener ("Prototype",onSelectThisChild);
		m_tryPremiumButton.		addClickCallback (toPremiumScreen);
		m_buyGemsButton.		addClickCallback (toBuyGemsScreen);
		


		//site part
		m_searchButton = 		m_addSiteCanvas.getView ( "searchButton" ) 	as UIButton;
		m_inputLabel = 			m_addSiteCanvas.getView ( "inputText" ) 	as UILabel;
		m_siteSwipeList = 		m_addSiteCanvas.getView ( "siteSwipeList" ) as UISwipeList;
		m_searchButton.			addClickCallback ( onSearchButtonClicked );
		m_siteSwipeList.		addClickListener ( "controlButton", m_addSiteCanvas.onButtonClicked );
		m_siteSwipeList.		addClickListener ( "controlButton", onSiteButtonClicked );
		m_siteSwipeList.active = false;
	}

	private void checkRequest()
	{
		if( m_isValueChanged )
		{
			m_isValueChanged = false;
			updateAddSite();
		}
	}

	private void onHelpButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks ();
		m_commonDialog.setOriginalPosition ();
		UIButton l_closeButton = m_commonDialog.getView ("closeMark") as UIButton;
		
		UILabel l_titleLabel = m_commonDialog.getView ("dialogText") as UILabel;
		UILabel l_contentLabel = m_commonDialog.getView ("contentText") as UILabel;
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_60_HELP_TITLE);
		l_contentLabel.text = Localization.getString(Localization.TXT_STATE_60_HELP_CONTENT);

		l_closeButton.addClickCallback (onCloseDialogButtonClick);
	}
	
	private void onCloseDialogButtonClick(UIButton p_button)
	{
		p_button.removeAllCallbacks();
		m_commonDialog.setOutPosition ();
		m_helpButton.addClickCallback (onHelpButtonClick);
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

			if( !SessionHandler.getInstance().token.isPremium() && !SessionHandler.getInstance().token.isCurrent() )
			{
				m_uiManager.setScreenEnable( UIScreen.ADD_SITE, false );
			}
		}
	}
	
	private void onCloseMenuTweenFinished( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		canMoveLeftMenu = true;
	}

	private void toSettingScreen(UIButton p_button)
	{
		p_button.removeClickCallback (toSettingScreen);
		m_gameController.changeState (ZoodleState.SETTING_STATE);
	}

	private void onSelectThisChild(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		Kid l_kid = p_data as Kid;
		if (ZoodlesConstants.ADD_CHILD_TEXT.Equals (l_kid.name))
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
		gotoGetGems ();
	}
	
	private void gotoGetGems()
	{	
		string l_returnJson = SessionHandler.getInstance ().returnJson;
		
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
	
	private void viewGemsRequestComplete(HttpsWWW p_response)
	{
		Server.init (ZoodlesConstants.getHttpsHost());
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
	
	private void toShowMenuTweenFinished( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		canMoveLeftMenu = true;
	}

	private void onSearchButtonClicked( UIButton p_button )
	{
		string l_keyWords = m_inputLabel.text;
		
		if( l_keyWords.Equals(string.Empty) )
		{
			List<object> l_allData = new List<object>();
			foreach( SiteInfo l_site in m_siteInfoList )
			{
				l_allData.Add( l_site );
			}
			
			m_addSiteCanvas.setSearchData( l_allData );
			return;
		}
		
		List<object> l_searchData = new List<object>();
		
		foreach( SiteInfo l_site in m_siteInfoList )
		{
			if( l_site.name.Contains(l_keyWords) )
			{
				l_searchData.Add( l_site );
			}
		}
		
		m_addSiteCanvas.setSearchData( l_searchData );
	}

	private void onSiteButtonClicked(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
//		p_list.removeClickListener ( "controlButton", onSiteButtonClicked );s
		SiteInfo l_siteInfo = p_data as SiteInfo;
		
		for( int i = 0; i < m_siteInfoList.Count; i++ )
		{
			if( l_siteInfo.id == m_siteInfoList[i].id )
			{
				m_siteInfoList[i].isBlocked = !m_siteInfoList[i].isBlocked;
			}
		}
		
		m_isValueChanged = true;
	}
	
	private void onShowFinish(UIElement p_element, Tweener.TargetVar p_target)
	{
		if( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() )
		{
			m_requestQueue.reset ();
			m_requestQueue.add(new GetSiteListRequest( _getSiteListRequestComplete ) );
			m_requestQueue.request( RequestType.SEQUENCE );
		}
		else
		{
			m_siteSwipeList.setData( new List<object>() );
		}
	}

	private void _getSiteListRequestComplete(HttpsWWW p_response)
	{
		ArrayList l_jsonList = MiniJSON.MiniJSON.jsonDecode (p_response.text) as ArrayList;
		
		List<object> l_dataList = new List<object> ();
		m_siteInfoList = new List<SiteInfo>();
		
		foreach( Hashtable l_data in l_jsonList )
		{
			SiteInfo l_siteInfo = new SiteInfo();
			l_siteInfo.id = int.Parse( l_data["id"].ToString() );
			l_siteInfo.name = l_data["name"].ToString();
			l_siteInfo.isBlocked = l_data["blocked"] == null ? false : true;
			l_dataList.Add(l_siteInfo);
			
			SiteInfo l_siteInfoListData = new SiteInfo();
			l_siteInfoListData.id = l_siteInfo.id;
			l_siteInfoListData.name = l_siteInfo.name;
			l_siteInfoListData.isBlocked = l_siteInfo.isBlocked;
			m_siteInfoList.Add(l_siteInfoListData);
		}
		
		m_addSiteCanvas.setData( l_dataList );
//		m_siteSwipeList.removeClickListener ( "controlButton", onSiteButtonClicked );
		m_siteSwipeList.active = true;
	}

	private void updateAddSite()
	{
		_Debug.log ( "update site" );
		m_siteUpdateList = "";
		foreach( SiteInfo l_site in m_siteInfoList )
		{
			if( l_site.isBlocked )
			{
				m_siteUpdateList += l_site.id.ToString();
				m_siteUpdateList += ",";
			}
		}
		m_siteUpdateList = m_siteUpdateList.TrimEnd( ',' );

		m_requestQueue.reset ();
		m_requestQueue.add( new SetSiteListRequest(m_siteUpdateList) );
		m_requestQueue.request( RequestType.SEQUENCE );
	}

	private void onUpgradeButtonClick(UIButton p_button)
	{
		SwrveComponent.Instance.SDK.NamedEvent("UpgradeBtnInDashBoard");

		if(string.Empty.Equals(SessionHandler.getInstance().PremiumJson))
		{
			Server.init (ZoodlesConstants.getHttpsHost());
			m_requestQueue.reset ();
			m_requestQueue.add (new GetPlanDetailsRequest(viewPremiumRequestComplete));
			m_requestQueue.request ();
		}
		else
		{
			m_gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName) );
			m_gameController.changeState( ZoodleState.VIEW_PREMIUM );
		}
	}
	
	private void viewPremiumRequestComplete(HttpsWWW p_response)
	{
		Server.init (ZoodlesConstants.getHttpsHost());
		if(null == p_response.error)
		{
			SessionHandler.getInstance ().PremiumJson = p_response.text;
			m_gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName) );
			m_gameController.changeState( ZoodleState.VIEW_PREMIUM );
		}
		else
		{
			setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_11_FAIL),Localization.getString(Localization.TXT_STATE_11_FAIL_DATA));
		}
	}

	private UIManager 		m_uiManager;
	private RequestQueue 	m_requestQueue;
	private bool 			m_isValueChanged = false;


	private LeftMenuCanvas 	m_leftMenuCanvas;
	private UISwipeList 	m_childrenList;


	private UIButton 		m_helpButton;
	private CommonDialogCanvas m_commonDialog;

	private UICanvas 		m_paywallCanvas;
	private UIButton 		m_upgradeButton;
	
	//top bar part
	private UIButton 		m_tryPremiumButton;
	private UIButton 		m_buyGemsButton;
	private UIButton 		m_showProfileButton;
	private UIButton		m_closeLeftMenuButton;
	private UIButton	    m_settingButton;
	private UIElement 		m_menu;
	private bool 			canMoveLeftMenu = true;

	//site part
	private AddSiteCanvas 	m_addSiteCanvas;

	private UISwipeList 	m_siteSwipeList;
	private List<SiteInfo> 	m_siteInfoList;
	private UIButton 		m_searchButton;
	private UILabel 		m_inputLabel;
	private string 			m_siteUpdateList;
}
