using UnityEngine;
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
		
		m_uiManager.removeScreen( UIScreen.COMMON_DIALOG );
		
		m_uiManager.removeScreen( UIScreen.ADD_SITE );
		m_uiManager.removeScreen( UIScreen.PAYWALL );
	}

	private void _setupScreen( GameController p_gameController )
	{
		m_commonDialog 				= m_uiManager.createScreen( UIScreen.COMMON_DIALOG, true, 5 ) 			as CommonDialogCanvas;
		m_commonDialog.setUIManager (p_gameController.getUI());

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


	private UISwipeList 	m_childrenList;


	private UIButton 		m_helpButton;
	private CommonDialogCanvas m_commonDialog;

	private UICanvas 		m_paywallCanvas;
	private UIButton 		m_upgradeButton;


	//site part
	private AddSiteCanvas 	m_addSiteCanvas;

	private UISwipeList 	m_siteSwipeList;
	private List<SiteInfo> 	m_siteInfoList;
	private UIButton 		m_searchButton;
	private UILabel 		m_inputLabel;
	private string 			m_siteUpdateList;
}
