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

		_init();
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_buyGemsCanvas );
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
			AndroidInAppPurchaseManager.instance.loadStore();
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
			_invokeError("Error", "Initialize store failed. Please try again.");
	}

	private void _onRetrieveProductsFinished(BillingResult p_result)
	{
		_Debug.log("_onRetrieveProductsFinished");

		AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= _onRetrieveProductsFinished;

		if (p_result.isSuccess)
		{
			_Debug.log("Product count: " + AndroidInAppPurchaseManager.instance.inventory.products.Count);
			foreach (GoogleProductTemplate l_tpl in AndroidInAppPurchaseManager.instance.inventory.products)
			{
				_Debug.log(l_tpl.title);
				_Debug.log(l_tpl.originalJson);
			}

			AndroidInAppPurchaseManager.ActionProductPurchased += _onProductPurchased;
			AndroidInAppPurchaseManager.ActionProductConsumed += _onProductConsumed;
			_setupScreen(m_gameController.getUI());
			m_inited = true;
		}
		else
			_invokeError("Error", "Initialize store failed. Please try again.");
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
			_invokeError("Error", "Purchase product failed. Please try again later.");
	}

	private void _onProductConsumed(BillingResult p_result)
	{
		_Debug.log("_onProductConsumed: " + p_result.message + "(" + p_result.response + ")");

		if (p_result.isSuccess)
		{
			GooglePurchaseTemplate l_purchase = p_result.purchase;
			_Debug.log(l_purchase.packageName);
			_Debug.log(l_purchase.signature);
			_Debug.log(l_purchase.originalJson);
			// TODO: Send request to server to validate the receipt and pay the product to the user
			m_gameController.changeState(ZoodleState.PAY_CONFIRM);
		}
		else
			_invokeError("Error", "Consumed product failed. Please try again later.");
	}
#endif
	
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

			l_goodPlanLabel.text = l_good["gems"].ToString() + " Gems for $" + double.Parse(l_good["amount"].ToString()).ToString("N");
			l_betterPlanLabel.text = l_better["gems"].ToString() + " Gems for $" + double.Parse(l_better["amount"].ToString()).ToString("N");
			l_bestPlanLabel.text = l_best["gems"].ToString() + " Gems for $" + double.Parse(l_best["amount"].ToString()).ToString("N");
		}
	}
	
	private void goBack( UIButton p_button )
	{
		int l_state = m_gameController.getConnectedState ( ZoodleState.BUY_GEMS );
		m_gameController.changeState (l_state);
	}

	private void gotoGoodPayment( UIButton p_button )
	{
#if UNITY_EDITOR
		m_gameController.changeState(ZoodleState.PAY_CONFIRM);
#elif UNITY_ANDROID
		AndroidInAppPurchaseManager.instance.purchase("org.bestlogic.purchasez.item01");
#endif
	}

	private void gotoBetterPayment( UIButton p_button )
	{
#if UNITY_EDITOR
		m_gameController.changeState(ZoodleState.PAY_CONFIRM);
#elif UNITY_ANDROID
		AndroidInAppPurchaseManager.instance.purchase("org.bestlogic.purchasez.item02");
#endif
	}

	private void gotoBestPayment( UIButton p_button )
	{
#if UNITY_EDITOR
		m_gameController.changeState(ZoodleState.PAY_CONFIRM);
#elif UNITY_ANDROID
		AndroidInAppPurchaseManager.instance.purchase("android.test.purchased");
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
