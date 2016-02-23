using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BuyGemsState : GameState 
{
	//Public variables
	
	
	//Standard state flow	
	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);

//		KidMode.setKidsModeActive(false);
		KidModeLockController.Instance.swith2DParentMode();
		_init();
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
	}
	
	public override void exit( GameController p_gameController )
	{
		Server.init (ZoodlesConstants.getHttpsHost());
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_buyGemsCanvas );
		p_gameController.getUI().removeScreenImmediately(UIScreen.SPLASH_BACKGROUND);
	}

	//---------------- Private Implementation ----------------------

	private void _init()
	{
#if UNITY_EDITOR
		_setupScreen(m_gameController.getUI());
#elif UNITY_ANDROID
		if (m_inited)
			_setupScreen(m_gameController.getUI());
		else
		{
			_Debug.log("Initialize Billing...");
			AndroidInAppPurchaseManager.ActionBillingSetupFinished += _onBillingSetupFinished;
			AndroidInAppPurchaseManager.instance.addProduct("com.zoodles.v2.good");
			AndroidInAppPurchaseManager.instance.addProduct("com.zoodles.v2.better");
			AndroidInAppPurchaseManager.instance.addProduct("com.zoodles.v2.best");
			AndroidInAppPurchaseManager.instance.loadStore("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA0LIs6pbEcpZg8SNG6CtRIXJP2rJhy2ZrAVWNSHo+9X/PtpprmTI0pk/Vh9uq9GbR9sMfiSnKZEJM78XLVaGwHM7cjcR9lfWVyE1mbpLZweAUo+iPDKPc4kgmd7HqgW27W+ZDISHTVwsU0gwd0syRvNKdfIiv3/3xwl/qdIlkChdQE2dAeWI5pPTsBIrkUkgs6Pwq8VMZStA1qFu0vwWBAt1Tn3pLD4OwGMnoI9q6va6IVgM9tpNOPeAnRwz91RWpjX1LkVjObDWnbt9TpQrsdn9PCb2cOXl8kJMr+KbFEeXjyGRitXf7hiF7nMkYagqwwijxGCHXnl6Tl2uRfYxmpQIDAQAB");
		}
#elif UNITY_IPHONE
		_setupScreen(m_gameController.getUI());
#endif
	}

#if !UNITY_EDITOR && UNITY_ANDROID
	private void _onBillingSetupFinished(BillingResult p_result)
	{
		_Debug.log("_onBillingSetupFinished");

		AndroidInAppPurchaseManager.ActionBillingSetupFinished -= _onBillingSetupFinished;

		if (p_result.isSuccess)
		{
			AndroidInAppPurchaseManager.instance.retrieveProducDetails();
			AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += _onRetrieveProductsFinished;
		}
		else
			_invokeError(Localization.getString(Localization.TXT_STATE_20_ERROR), Localization.getString(Localization.TXT_STATE_20_FAIL_INIT));
	}

	private void _onRetrieveProductsFinished(BillingResult p_result)
	{
		_Debug.log("_onRetrieveProductsFinished");

		AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= _onRetrieveProductsFinished;

		if (p_result.isSuccess)
		{
			AndroidInAppPurchaseManager.ActionProductPurchased += _onProductPurchased;
			AndroidInAppPurchaseManager.ActionProductConsumed += _onProductConsumed;

			_Debug.log("Product count: " + AndroidInAppPurchaseManager.instance.inventory.products.Count);
			foreach (GoogleProductTemplate l_tpl in AndroidInAppPurchaseManager.instance.inventory.products)
			{
				_Debug.log(l_tpl.title);
				_Debug.log(l_tpl.originalJson);
				if (AndroidInAppPurchaseManager.instance.inventory.IsProductPurchased(l_tpl.SKU))
				{
					AndroidInAppPurchaseManager.instance.consume(l_tpl.SKU);
				}
			}

			_setupScreen(m_gameController.getUI());
			m_inited = true;
		}
		else
			_invokeError(Localization.getString(Localization.TXT_STATE_20_ERROR), Localization.getString(Localization.TXT_STATE_20_FAIL_INIT));
	}

	private void _onProductPurchased(BillingResult p_result)
	{
		_Debug.log("_onProductPurchased: " + p_result.message + "(" + p_result.response + ")");


		if (p_result.isSuccess || p_result.response == BillingResponseCodes.BILLING_RESPONSE_RESULT_ITEM_ALREADY_OWNED)
		{
			GooglePurchaseTemplate l_purchase = p_result.purchase;
			AndroidInAppPurchaseManager.instance.consume(l_purchase.SKU);
		}
		else
			_invokeError(Localization.getString(Localization.TXT_STATE_20_ERROR), Localization.getString(Localization.TXT_STATE_20_FAIL_PURCHASE));
	}

	private void _onProductConsumed(BillingResult p_result)
	{
		_Debug.log("_onProductConsumed: " + p_result.message + "(" + p_result.response + ")");

		if (p_result.isSuccess)
		{
			GooglePurchaseTemplate l_purchase = p_result.purchase;
			_Debug.log("---------------Product Info -----------------------");
			_Debug.log("package: " + l_purchase.packageName);
			_Debug.log("orderId: " + l_purchase.orderId);
			_Debug.log("token: " + l_purchase.token);
			_Debug.log("signature: " + l_purchase.signature);
			_Debug.log("payload: " + l_purchase.developerPayload);
			_Debug.log("json: " + l_purchase.originalJson);
			_Debug.log("---------------------------------------------------");


			// Send request to server to validate the receipt and pay the product to the user
			Server.init (ZoodlesConstants.getHttpsHost());
			RequestQueue l_queue = new RequestQueue();
			l_queue.add(new PurchaseGemsRequest(l_purchase.packageName, l_purchase.orderId, l_purchase.SKU, l_purchase.token, l_purchase.developerPayload, _onPurchaseRequestComplete));
			l_queue.request(RequestType.RUSH);
		}
		else
			_invokeError(Localization.getString(Localization.TXT_STATE_20_ERROR), Localization.getString(Localization.TXT_STATE_20_FAIL_CONSUMED));
	}
	#endif
	private void _onPurchaseRequestComplete(HttpsWWW p_response)
	{
		_Debug.log("result: " + p_response.text);


		if(null == p_response.error && !"null".Equals(p_response.text))
		{

			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			l_data = (l_data["jsonResponse"] as Hashtable)["response"] as Hashtable;
			if(null != l_data && l_data.ContainsKey("error") && "false".Equals(l_data["error"].ToString()))
			{
				if(l_data.ContainsKey("gems"))
				{

					int l_gemsCount = int.Parse(l_data["gems"].ToString());
					SessionHandler.getInstance().currentKid.gems = l_gemsCount;
					List<Kid> l_kidList = SessionHandler.getInstance().kidList;
					foreach(Kid l_k in l_kidList)
					{
						l_k.gems = l_gemsCount;
					}
					//swrve start
					string l_gemAmount = m_gameController.board.read("gems") as string;
					string l_gemPrice = m_gameController.board.read("gemsPrice") as string;

					Dictionary<string,string> payload = new Dictionary<string,string>() { {"GemsCount", l_gemAmount.ToString()}};
					SwrveComponent.Instance.SDK.NamedEvent("GemPurchaseSucess",payload);					
					//SwrveComponent.Instance.SDK.Iap(
					//SwrveComponent.Instance.SDK.Purchase("Gems", "usd", int.Parse(l_gemPrice), int.Parse(l_gemAmount));
					//swrve end
					changeToNextState();
				}
				else
				{
					SwrveComponent.Instance.SDK.NamedEvent("GemPurchaseFailure");

					_invokeError(Localization.getString(Localization.TXT_STATE_20_ERROR), Localization.getString(Localization.TXT_STATE_20_FAIL_PURCHASE));
				}
			}
			else
			{
				SwrveComponent.Instance.SDK.NamedEvent("GemPurchaseFailure");

				_invokeError(Localization.getString(Localization.TXT_STATE_20_ERROR), Localization.getString(Localization.TXT_STATE_20_FAIL_PURCHASE));
			}
		}
		else
		{
			SwrveComponent.Instance.SDK.NamedEvent("GemPurchaseFailure");

			_invokeError(Localization.getString(Localization.TXT_STATE_20_ERROR), Localization.getString(Localization.TXT_STATE_20_FAIL_PURCHASE));
		}
	}

	
	private void _setupScreen( UIManager p_uiManager )
	{
		m_buyGemsCanvas = p_uiManager.createScreen( UIScreen.BUY_GEMS, true, 1 );
		m_backScreen = p_uiManager.findScreen( UIScreen.SPLASH_BACKGROUND );
		if (m_backScreen == null)
		{
			m_backScreen = p_uiManager.createScreen(UIScreen.SPLASH_BACKGROUND,true, -1);
			(m_backScreen as SplashBackCanvas).setDown();
		}
		m_backButton = m_buyGemsCanvas.getView ("backButton") as UIButton;
		m_backButton.addClickCallback (goBack);

		m_buyGoodButton = m_buyGemsCanvas.getView ("purchaseGoodButton") as UIButton;
		m_buyGoodButton.addClickCallback (gotoGoodPayment);

		m_buyBetterButton = m_buyGemsCanvas.getView ("purchaseBetterButton") as UIButton;
		m_buyBetterButton.addClickCallback (gotoBetterPayment);

		m_buyBestButton = m_buyGemsCanvas.getView ("purchaseBestButton") as UIButton;
		m_buyBestButton.addClickCallback (gotoBestPayment);

		string l_returnJson = SessionHandler.getInstance ().GemsJson;
		Hashtable l_date = new Hashtable();
		Hashtable l_response = new Hashtable ();	
		
		if(l_returnJson.Length > 0)
		{
			l_date = MiniJSON.MiniJSON.jsonDecode (l_returnJson) as Hashtable;
			Hashtable l_jsonResponse = new Hashtable();

			if(l_date.ContainsKey("jsonResponse"))
				l_jsonResponse = l_date["jsonResponse"] as Hashtable;

			if(l_jsonResponse.ContainsKey("response"))
				l_response = l_jsonResponse["response"] as Hashtable;
		}


		if(l_response.Count != 0)
		{
			UILabel l_goodPlanLabel = m_buyGemsCanvas.getView("goodPrice") as UILabel;
			UILabel l_betterPlanLabel = m_buyGemsCanvas.getView("betterPrice") as UILabel;
			UILabel l_bestPlanLabel = m_buyGemsCanvas.getView("bestPrice") as UILabel;

			Hashtable l_good = l_response["good"] as Hashtable;
			Hashtable l_better = l_response["better"] as Hashtable;
			Hashtable l_best = l_response["best"] as Hashtable;

			l_goodPlanLabel.text = l_good["gems"].ToString() + Localization.getString(Localization.TXT_STATE_20_GEMS) + double.Parse(l_good["amount"].ToString()).ToString("N");
			l_betterPlanLabel.text = l_better["gems"].ToString() + Localization.getString(Localization.TXT_STATE_20_GEMS) + double.Parse(l_better["amount"].ToString()).ToString("N");
			l_bestPlanLabel.text = l_best["gems"].ToString() + Localization.getString(Localization.TXT_STATE_20_GEMS) + double.Parse(l_best["amount"].ToString()).ToString("N");
		}
	}
	
	private void goBack( UIButton p_button )
	{
		SwrveComponent.Instance.SDK.NamedEvent("BackFromBuyGems");

		p_button.removeClickCallback ( goBack );
		int l_state = m_gameController.getConnectedState ( ZoodleState.BUY_GEMS );
		m_gameController.changeState (l_state);
	}

	private void gotoGoodPayment( UIButton p_button )
	{
		string l_returnJson = SessionHandler.getInstance ().GemsJson;
		Hashtable l_data = MiniJSON.MiniJSON.jsonDecode (l_returnJson) as Hashtable;
		Hashtable l_jsonResponse = l_data["jsonResponse"] as Hashtable;
		Hashtable l_response = l_jsonResponse["response"] as Hashtable;
		Hashtable l_good = l_response["good"] as Hashtable;

		m_gameController.board.write("gems", l_good["gems"].ToString());
		m_gameController.board.write("gemsPrice", l_good["amount"].ToString());

		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");
		jc.CallStatic("openGoogleIAP");
		#endif

#if UNITY_EDITOR
		changeToNextState();
#elif UNITY_ANDROID
		AndroidInAppPurchaseManager.instance.purchase("com.zoodles.v2.good");
#endif
	}

	private void gotoBetterPayment( UIButton p_button )
	{
		string l_returnJson = SessionHandler.getInstance ().GemsJson;
		Hashtable l_data = MiniJSON.MiniJSON.jsonDecode (l_returnJson) as Hashtable;
		Hashtable l_jsonResponse = l_data["jsonResponse"] as Hashtable;
		Hashtable l_response = l_jsonResponse["response"] as Hashtable;
		Hashtable l_better = l_response["better"] as Hashtable;
		m_gameController.board.write("gems", l_better["gems"].ToString());
		m_gameController.board.write("gemsPrice", l_better["amount"].ToString());

		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");
		jc.CallStatic("openGoogleIAP");
		#endif
		
#if UNITY_EDITOR
		changeToNextState();
#elif UNITY_ANDROID
		AndroidInAppPurchaseManager.instance.purchase("com.zoodles.v2.better");
#endif
	}

	private void gotoBestPayment( UIButton p_button )
	{
		string l_returnJson = SessionHandler.getInstance ().GemsJson;
		Hashtable l_data = MiniJSON.MiniJSON.jsonDecode (l_returnJson) as Hashtable;
		Hashtable l_jsonResponse = l_data["jsonResponse"] as Hashtable;
		Hashtable l_response = l_jsonResponse["response"] as Hashtable;
		Hashtable l_best = l_response["best"] as Hashtable;
		m_gameController.board.write("gems", l_best["gems"].ToString());
		m_gameController.board.write("gemsPrice", l_best["amount"].ToString());

		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");
		jc.CallStatic("openGoogleIAP");
		#endif
		
#if UNITY_EDITOR
		changeToNextState();
#elif UNITY_ANDROID
		AndroidInAppPurchaseManager.instance.purchase("com.zoodles.v2.best");
#endif
	}

	private void _invokeError(string p_errorTitle, string p_errorMessage)
	{
		int l_state = m_gameController.getConnectedState(ZoodleState.BUY_GEMS);
		m_gameController.connectState(ZoodleState.ERROR_STATE, l_state);
		
		SessionHandler l_handler = SessionHandler.getInstance();
		l_handler.errorName = p_errorTitle;
		l_handler.errorMessage = p_errorMessage;
		m_gameController.changeState(ZoodleState.ERROR_STATE);
	}

	private void changeToNextState()
	{
		int state = m_gameController.getConnectedState(ZoodleState.BUY_GEMS);
		m_gameController.connectState(ZoodleState.BUY_GEMS, state);
		m_gameController.connectState(ZoodleState.PAY_GEMS_CONFIRM, state);
		m_gameController.changeState(ZoodleState.PAY_GEMS_CONFIRM);
	}

	//Private variables
	
	private UICanvas    m_buyGemsCanvas;
	private UICanvas	m_signInButtonBackgroundCanvas;

	private UIButton 	m_gotoDashBoardButton;
	private UICanvas	m_backScreen;

	private UIButton 	m_buyGoodButton;
	private UIButton 	m_buyBetterButton;
	private UIButton 	m_buyBestButton;
	private UIButton 	m_backButton;

#if !UNITY_EDITOR && UNITY_ANDROID
	private bool		m_inited = false;
#endif
}
