using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UpsellSplashState : GameState 
{
	//Public variables
	
	
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );
		
		m_requestQueue = new RequestQueue ();

		_setupScreen( p_gameController.getUI() );

	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_upsellSplashCanvas );
	}

	//---------------- Private Implementation ----------------------
	
	private void _setupScreen( UIManager p_uiManager )
	{

		m_backScreen = p_uiManager.findScreen( UIScreen.SPLASH_BACKGROUND );
		if (m_backScreen == null)
		{
			m_backScreen = p_uiManager.createScreen(UIScreen.SPLASH_BACKGROUND,true, -1);
			(m_backScreen as SplashBackCanvas).setDown();
		}

		m_upsellSplashCanvas = p_uiManager.createScreen( UIScreen.UPSELL_SPLASH, true, 1 );

		m_gotoDashBoardButton = m_upsellSplashCanvas.getView ("continueButton") as UIButton;
		m_gotoDashBoardButton.addClickCallback (gotoDashBoard);

		m_viewPlanButton = m_upsellSplashCanvas.getView ("viewPlanButton") as UIButton;
		m_viewPlanButton.addClickCallback (gotoViewPlan);

		m_buyGemsButton = m_upsellSplashCanvas.getView ("getGemsButton") as UIButton;
		m_buyGemsButton.addClickCallback (gotoGetGems);

		m_profileButton = m_upsellSplashCanvas.getView ("profilesButton") as UIButton;
		m_profileButton.addClickCallback (gotoProfileSelectScreen);
	}

	private void gotoDashBoard( UIButton p_button )
	{
		if(SessionHandler.getInstance().childLockSwitch)
		{
			if (0 != SessionHandler.getInstance ().pin) 
			{
				m_gameController.changeState (ZoodleState.BIRTHYEAR);
			}
			else
			{
				setErrorMessage(m_gameController,"error","Pin inexistence. Please reLogin.");
			}
		}
		else
		{
			m_gameController.changeState (ZoodleState.OVERVIEW_INFO);
		}
	}

	private void gotoViewPlan( UIButton p_button )
	{
		p_button.removeClickCallback (gotoViewPlan);
		string l_returnJson = SessionHandler.getInstance ().PremiumJson;
		
		if(l_returnJson.Length > 0)
		{
			Hashtable l_date = MiniJSON.MiniJSON.jsonDecode (l_returnJson) as Hashtable;
			if(l_date.ContainsKey("subscription_plans"))
			{
				m_gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName) );
				m_gameController.changeState( ZoodleState.VIEW_PREMIUM );
			}
			else
			{
				Server.init (ZoodlesConstants.getHttpsHost());
				m_requestQueue.reset();
				m_requestQueue.add(new GetPlanDetailsRequest(getViewPremiumComplete));
				m_requestQueue.request();
			}
		}
		else
		{
			Server.init (ZoodlesConstants.getHttpsHost());
			m_requestQueue.reset();
			m_requestQueue.add(new GetPlanDetailsRequest(getViewPremiumComplete));
			m_requestQueue.request();
		}
	}

	private void gotoGetGems( UIButton p_button )
	{	
		p_button.removeClickCallback (gotoGetGems);
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
				Server.init (ZoodlesConstants.getHttpsHost());
				m_requestQueue.reset();
				m_requestQueue.add(new ViewGemsRequest(getViewGemsComplete));
				m_requestQueue.request();
			}
		}
		else
		{
			Server.init (ZoodlesConstants.getHttpsHost());
			m_requestQueue.reset();
			m_requestQueue.add(new ViewGemsRequest(getViewGemsComplete));
			m_requestQueue.request();
		}
	}

	private void getViewPremiumComplete(WWW p_response)
	{
		Server.init (ZoodlesConstants.getHost());
		if(null != p_response.error)
		{
			_Debug.logError(p_response.error);
		}
		else
		{
			SessionHandler.getInstance().PremiumJson = p_response.text;
			m_gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName) );
			m_gameController.changeState( ZoodleState.VIEW_PREMIUM );
		}
	}

	private void getViewGemsComplete(WWW p_response)
	{
		Server.init (ZoodlesConstants.getHost());
		if(null != p_response.error)
		{
			_Debug.logError(p_response.error);
		}
		else
		{
			SessionHandler.getInstance().GemsJson = p_response.text;
			m_gameController.connectState( ZoodleState.BUY_GEMS, int.Parse(m_gameController.stateName) );
			m_gameController.changeState(ZoodleState.BUY_GEMS);
		}
	}

	private void gotoProfileSelectScreen( UIButton p_button )
	{
		p_button.removeClickCallback (gotoProfileSelectScreen);
		m_gameController.changeState (ZoodleState.PROFILE_SELECTION);
	}

	//Private variables

	
	private UICanvas    m_upsellSplashCanvas;
	private UICanvas	m_backScreen;

	private UIButton 	m_gotoDashBoardButton;
	private UIButton 	m_viewPlanButton;
	private UIButton 	m_buyGemsButton;
	private UIButton 	m_profileButton;
	private RequestQueue m_requestQueue;
}
