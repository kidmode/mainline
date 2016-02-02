using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ViewPremiumState : GameState 
{
	//Public variables

	private UIManager m_uiManager;
	
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );
		SwrveComponent.Instance.SDK.NamedEvent("ViewPremiumPlan");

		m_game = p_gameController.game;
		m_uiManager = p_gameController.getUI ();

		_setupScreen( p_gameController.getUI() );

		//Set events
		Game.OnPaymentSuccess += OnGameScriptPaymentSuccess;

	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_premiumCanvas );
		if( null != m_backScreen )
		{
			p_gameController.getUI().removeScreen( m_backScreen );
		}

		//remove event call backs
		Game.OnPaymentSuccess -= OnGameScriptPaymentSuccess;

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
		m_premiumCanvas = p_uiManager.createScreen( UIScreen.VIEW_PREMIUM, true, 1 );

		m_backButton = m_premiumCanvas.getView ("exitButton") as UIButton;
		m_backButton.addClickCallback (backToUpsell);

		m_monthPurchaseButton = m_premiumCanvas.getView ("monthlyPurchaseButton") as UIButton;
		m_monthPurchaseButton.addClickCallback (gotoMonthlyPurchase);

		
		string l_returnJson = SessionHandler.getInstance ().PremiumJson;

		Hashtable l_data = null;

		if(l_returnJson.Length > 0)
			l_data = MiniJSON.MiniJSON.jsonDecode (l_returnJson) as Hashtable;
//		m_transparentImage = m_premiumCanvas.getView ("topImage") as UIImage;
		if(null != l_data && l_data.ContainsKey("plan_info"))
		{
			m_yearlyPurchaseButton = m_premiumCanvas.getView ("yearlyPurchaseButton") as UIButton;
			m_yearlyPurchaseButton.addClickCallback (gotoYearlyPurchase);
			
			UILabel l_nowMonthPriceLabel = m_premiumCanvas.getView("monthCard").getView ("nowPrice") as UILabel;
			UILabel l_nowYearPriceLabel = m_premiumCanvas.getView("yearlyCard").getView ("nowPrice") as UILabel;
			
			UILabel l_preMonthPriceLabel = m_premiumCanvas.getView("monthCard").getView ("prePrice") as UILabel;
			UILabel l_preYearPriceLabel = m_premiumCanvas.getView("yearlyCard").getView ("prePrice") as UILabel;
			
			UILabel l_MonthDiscountLabel = m_premiumCanvas.getView("monthCard").getView ("discount").getView("Text") as UILabel;
			UILabel l_YealyDiscountLabel = m_premiumCanvas.getView("yearlyCard").getView ("discount").getView("Text") as UILabel;

			float l_monthDiscount;
			float l_yealyDiscount;
			float l_nowMonthPrice = 0;
			float l_nowYearPrice = 0;

			ArrayList l_planList = l_data["plan_info"] as ArrayList;
			Hashtable l_detail = _getPlanDetailByName(l_planList, "Monthly");
			l_nowMonthPrice = float.Parse(l_detail["amount"].ToString());
			l_detail = _getPlanDetailByName(l_planList, "Annual");
			l_nowYearPrice =float.Parse(l_detail["amount"].ToString());

			l_monthDiscount = 0.20f;
			l_yealyDiscount = 0.50f;
			
			l_nowMonthPriceLabel.text = "$" + l_nowMonthPrice.ToString("N");
			l_nowYearPriceLabel.text = "$" + l_nowYearPrice.ToString("N");
			l_MonthDiscountLabel.text = (l_monthDiscount*100).ToString() + "%";
			l_YealyDiscountLabel.text = (l_yealyDiscount*100).ToString() + "%"; 
			l_preMonthPriceLabel.text = "$"+ (l_nowMonthPrice /(1- l_monthDiscount)).ToString("N");
			l_preYearPriceLabel.text = "$"+ (l_nowYearPrice /(1- l_yealyDiscount)).ToString("N");
		}
		else
		{
			m_premiumCanvas.active = false;
		}
	}

	private Hashtable _getPlanDetailByName(ArrayList p_plans, string p_planName)
	{
		Hashtable l_plan = null;
		for (int i = 0; i < p_plans.Count; ++i)
		{
			l_plan = p_plans[i] as Hashtable;
			if (p_planName.Equals(l_plan["en_name"].ToString()))
				return l_plan;
		}

		return null;
	}

	private void backToUpsell( UIButton p_button )
	{
		p_button.removeClickCallback (backToUpsell);
		int l_state = m_gameController.getConnectedState ( ZoodleState.VIEW_PREMIUM );
		if (l_state == ZoodleState.SIGN_IN)
		{
			m_game.gameController.changeState(ZoodleState.PROFILE_SELECTION);
		}
		else
		{
			m_game.gameController.changeState(l_state);
		}

	}

	private void gotoMonthlyPurchase( UIButton p_button )
	{
		SwrveComponent.Instance.SDK.NamedEvent("GotoMonthlyPurchase");

//		SessionHandler.getInstance ().purchaseObject = "Monthly";
//		m_game.gameController.changeState (ZoodleState.PLAN_PAYMENT);

		SessionHandler.getInstance ().purchaseObject = "Monthly";
		string l_purchaseObject = "Monthly";
		string l_returnJson = SessionHandler.getInstance().PremiumJson;
		Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(l_returnJson) as Hashtable;
		ArrayList l_planList = l_data["plan_info"] as ArrayList;
		Hashtable l_plan = _getPlanDetailByName(l_planList, l_purchaseObject);
		
		
		_setupWebview (GCS.Environment.getSecureHost() + "/payments/premium?client_id=" + SessionHandler.getInstance().clientId +
		                            "&token=" + SessionHandler.getInstance().token.getSecret() +
		                            "&plan_id=" + l_plan["id"].ToString());
	}

	private void OnGameScriptPaymentSuccess(){
		
		SessionHandler.getInstance ().token.setTry(true);
		SessionHandler.getInstance ().token.setCurrent(true);
		SessionHandler.getInstance ().token.setPremium(true);
		
		_closeWebview ();
		
		//Payment done and First started Trial account
		//Now sets local playerprefs
		TrialTimeController.Instance.firstStartTrialTime();
		
		if(SessionHandler.getInstance ().token.isTried())
		{
			SwrveComponent.Instance.SDK.NamedEvent("FreeTrialToPremium");
		} else
		{
			SwrveComponent.Instance.SDK.NamedEvent("PremiumWithoutTrial");
		}

		m_gameController.changeState(ZoodleState.PAY_CONFIRM);
	}

	private void _setupWebview(string p_url)
	{
		UICanvas l_screen = m_uiManager.createScreen(UIScreen.CREDIT_CARD_WEBVIEW, false, 5);
		m_uiManager.changeScreen(l_screen, true);
		UIButton l_confirm = l_screen.getView("quitButton") as UIButton;
		l_confirm.addClickCallback(_onConfirmButtonClick);
		UniWebView l_webView = l_screen.gameObject.GetComponentInChildren<UniWebView>();
		l_webView.insets = new UniWebViewEdgeInsets((int)(110.0f * l_screen.scaleFactor), (int)(110.0f * l_screen.scaleFactor), (int)(110.0f * l_screen.scaleFactor), (int)(110.0f * l_screen.scaleFactor));
		l_webView.OnReceivedKeyCode += _onBackKeyCode;
		l_webView.OnWebViewShouldClose += _onShouldCloseView;
		l_webView.OnLoadComplete += loadComplete;
		l_webView.SetShowSpinnerWhenLoading(true);
		l_webView.Load(p_url);
	}

	private void _onConfirmButtonClick(UIButton p_button)
	{
		
		_closeWebview();
	}


	private void _onBackKeyCode(UniWebView p_view, int p_keyCode)
	{
		_closeWebview();
	}
	
	private bool _onShouldCloseView(UniWebView p_webView)
	{
		_closeWebview();
		return true;
	}

	private void quitNoInternetButtonClicked()
	{
		ErrorMessage error = GameObject.FindWithTag("ErrorMessageTag").GetComponent<ErrorMessage>() as ErrorMessage;
		if (error != null)
			error.onClick -= quitNoInternetButtonClicked;
	}


	private void loadComplete(UniWebView webView, bool success, string errorMessage)
	{
		if (!success)
		{
			Debug.Log("Uniwebview load failed error message: " + errorMessage);
			_closeWebview();
			m_uiManager.createScreen(UIScreen.ERROR_MESSAGE, false, 15);
			ErrorMessage error = GameObject.FindWithTag("ErrorMessageTag").GetComponent<ErrorMessage>() as ErrorMessage;
			if (error != null)
				error.onClick += quitNoInternetButtonClicked;
		}
		else
		{
			webView.Show();
		}
	}

	private void gotoYearlyPurchase( UIButton p_button )
	{
		SwrveComponent.Instance.SDK.NamedEvent("gotoYearlyPurchase");

//		SessionHandler.getInstance ().purchaseObject = "Annual";
//		m_game.gameController.changeState (ZoodleState.PLAN_PAYMENT);

		SessionHandler.getInstance ().purchaseObject = "Annual";
		string l_purchaseObject = "Annual";
		string l_returnJson = SessionHandler.getInstance().PremiumJson;
		Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(l_returnJson) as Hashtable;
		ArrayList l_planList = l_data["plan_info"] as ArrayList;
		Hashtable l_plan = _getPlanDetailByName(l_planList, l_purchaseObject);
		
		
		_setupWebview (GCS.Environment.getSecureHost() + "/payments/premium?client_id=" + SessionHandler.getInstance().clientId +
		               "&token=" + SessionHandler.getInstance().token.getSecret() +
		               "&plan_id=" + l_plan["id"].ToString());
	}

	private void _closeWebview()
	{
		UICanvas l_screen = m_uiManager.findScreen(UIScreen.CREDIT_CARD_WEBVIEW);
		UniWebView l_webView = l_screen.gameObject.GetComponentInChildren<UniWebView>();
		l_webView.Hide();
		l_webView.CleanCache();
		l_webView.OnReceivedKeyCode -= _onBackKeyCode;
		l_webView.OnWebViewShouldClose -= _onShouldCloseView;
		l_webView.OnLoadComplete -= loadComplete;
		m_uiManager.removeScreenImmediately(l_screen);
	}

	//Private variables
	
	private UICanvas    m_premiumCanvas;
	private UICanvas	m_backScreen;

	private UIButton 	m_monthPurchaseButton;
	private UIButton 	m_yearlyPurchaseButton;
	private UIButton 	m_backButton;
//	private UIImage		m_transparentImage;

	private Game 		m_game;
}
