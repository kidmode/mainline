using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InitializeGameState : GameState 
{
	//consts
	private const float LOADING_WEIGHT 	= 1;
	private const float LOADING_START 	= 100;
	
	//Standard state flow	
	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);

		m_gotoLogin = false;
		m_time = 0;

		m_request = new RequestQueue ();
		m_request.add ( new ClientIdRequest(getClientIdComplete) );
		m_request.add ( new CheckFreePremiumRequest(getCheckComplete) );
		m_request.request ( RequestType.SEQUENCE );
		_setupScreen(p_gameController.getUI());
		m_loadingLabel.tweener.addAlphaTrack(1.0f, 0.0f, 1.0f, onLoadingTweenFinish);

		GAUtil.logScreen("SplashScreen");
	}
	
	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		m_time += p_time;
		if (m_time < 1250)
		{
			float l_fillAmount = Mathf.Lerp(0, 1.0f, m_time / 1250.0f);
			if(null != m_loadingBarImg)
				m_loadingBarImg.fillAmount = l_fillAmount;
		}
		else if (m_loadingTween == false)
		{
			m_loadingTween = true;
			if(null != m_loadingBarImg)
				m_loadingBarImg.fillAmount = 1.0f;
		}
		
		if (m_gotoLogin)
		{
			if( SessionHandler.getInstance().token.isExist() )
			{
				if(SessionHandler.getInstance().childLockSwitch)
				{
					p_gameController.connectState(ZoodleState.BIRTHYEAR,int.Parse(m_gameController.stateName));
					p_gameController.changeState(ZoodleState.BIRTHYEAR);
				}
				else
				{
					p_gameController.changeState(ZoodleState.SIGN_IN_CACHE);
				}
			}
			else
			{
				p_gameController.changeState(ZoodleState.CREATE_ACCOUNT_SELECTION);
			}
			m_gotoLogin = false;
		}
	}
	
	public override void exit(GameController p_gameController)
	{
		base.exit(p_gameController);

		p_gameController.getUI().removeScreen(m_splashForeCanvas);
	}

	//---------------- Private Implementation ----------------------
	
	private void _setupScreen(UIManager p_uiManager)
	{
		m_splashBackCanvas = p_uiManager.findScreen(UIScreen.SPLASH_BACKGROUND) as SplashBackCanvas;
		if (m_splashBackCanvas == null)
			m_splashBackCanvas = p_uiManager.createScreen(UIScreen.SPLASH_BACKGROUND, true, -1) as SplashBackCanvas;

		m_splashForeCanvas = p_uiManager.createScreen(
			( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() ) ? UIScreen.SPLASH_PREMIUM : UIScreen.SPLASH,
			true,
			1);
		m_splashForeCanvas.enterTransitionEvent += onTransitionEnter;
		m_splashForeCanvas.exitTransitionEvent 	+= onTransitionExit;

		m_tapContinueButton = m_splashForeCanvas.getView("nextButton") as UIButton;
		m_tapContinueButton.addClickCallback(onNextClicked);
        m_tapContinueButton.active = false;

		m_loadingBarImg = m_splashForeCanvas.getView("loadingBarSprite") as UIImage;
		m_loadingLabel = m_splashForeCanvas.getView("loadingLabel") as UILabel;

        Vector3 l_position = m_loadingLabel.transform.localPosition;

		List<Vector3> l_posList = new List<Vector3>();
        l_posList.Add(l_position + new Vector3(150, 50, 0));
        l_posList.Add(l_position + new Vector3(-10, 20, 0));
        l_posList.Add(l_position + new Vector3(-2, 5, 0));
        l_posList.Add(l_position);
		m_loadingLabel.tweener.addPositionTrack(l_posList, 1.0f, null, Tweener.Style.QuadOutReverse);

		SessionHandler.getInstance().flashInstall = KidMode.hasFlashInstalled();
	}

//Listeners
	private void onNextClicked(UIButton p_button)
	{
		m_gotoLogin = true;
        m_splashBackCanvas.transitionDown(2.5f);
	}	

	private void onTransitionEnter(UICanvas p_canvas)
	{
		p_canvas.graphicRaycaster.enabled = false;
		//m_uiManager.moveScreenToLayer( p_canvas, -1 );
	}
		
	private void onTransitionExit(UICanvas p_canvas)
	{
		p_canvas.graphicRaycaster.enabled = true;
	}

	private void onFadeFinish(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		UICanvas p_canvas = p_element as UICanvas;
		p_canvas.isTransitioning = false;
	}

	private void onLoadingTweenFinish(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		m_tapContinueButton.active = true;
		m_tapContinueButton.tweener.addAlphaTrack(0.0f, 1.0f, 0.5f);
	}

	private void getClientIdComplete(WWW p_response)
	{
		if(p_response.error == null)
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			SessionHandler.getInstance ().clientId = l_data.ContainsKey("id") ? double.Parse(l_data["id"].ToString()) : -1;
//			SessionHandler.getInstance ().clientId = 26;
		}
		else
		{
			if (!SessionHandler.getInstance().token.isExist()) //cynthia
			{
				m_request.reset();
				//cynthia vzw
				Game game = GameObject.Find("GameLogic").GetComponent<Game>();
				game.gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
				//setErrorMessage( m_gameController, Localization.getString(Localization.TXT_STATE_0_FAIL), Localization.getString(Localization.TXT_STATE_0_FAIL_MESSAGE) );
				//vzw end
			}
		}
	}

	private void getCheckComplete(WWW p_response)
	{
		if(p_response.error == null)
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			l_data = (l_data["jsonResponse"] as Hashtable)["response"] as Hashtable;
			
			SessionHandler.getInstance().renewalPeriod = (int)((double)l_data["renewal_period"]);

			m_loadingLabel.tweener.addAlphaTrack(1.0f, 0.0f, 1.0f, onLoadingTweenFinish);
		}
		else
		{
			if (!SessionHandler.getInstance().token.isExist()) //cynthia
			{
				m_request.reset();
				//cynthia vzw
				Game game = GameObject.Find("GameLogic").GetComponent<Game>();
				game.gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
				//setErrorMessage( m_gameController, Localization.getString(Localization.TXT_STATE_0_FAIL), Localization.getString(Localization.TXT_STATE_0_FAIL_MESSAGE) );
				//vzw end
			} 
		}
	}
	
	//Private variables
	private bool m_gotoLogin;
	private int m_time = 0;
	private bool m_loadingTween = false;

    private UILabel     m_welcomeLabel;
    private UILabel     m_byLabel;
	private UILabel		m_loadingLabel;
	private UIImage 	m_loadingBarImg;
	private UIButton 	m_tapContinueButton;

	private SplashBackCanvas	m_splashBackCanvas;
	private UICanvas			m_splashForeCanvas;

	private RequestQueue m_request;
}
