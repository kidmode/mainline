using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PaymentState : GameState 
{
	//Public variables
	
	
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );
		
		m_game = p_gameController.game;

		_setupScreen( p_gameController.getUI() );

	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_paymentCanvas );
	}

	//---------------- Private Implementation ----------------------
	
	private void _setupScreen( UIManager p_uiManager )
	{
		m_paymentCanvas = p_uiManager.createScreen( UIScreen.PAYMENT, true, 1 );

		m_backButton = m_paymentCanvas.getView ("exitButton") as UIButton;
		m_backButton.addClickCallback (goBack);

		m_purchaseButton = m_paymentCanvas.getView ("purchaseButton") as UIButton;
		m_purchaseButton.addClickCallback (goPaymentConfirm);

		m_topicText = m_paymentCanvas.getView ("topicText") as UILabel;
		m_topic 	= m_paymentCanvas.getView ("topic") as UILabel;
		m_prePrice = m_paymentCanvas.getView ("prePrice") as UILabel;
		m_discountText = m_paymentCanvas.getView ("discount") as UILabel;
		m_nowPrice = m_paymentCanvas.getView ("nowPrice") as UILabel;
		m_payable = m_paymentCanvas.getView ("payableText") as UILabel;
		m_bestDealImg = m_paymentCanvas.getView ("recommendImage") as UIImage;
		m_bestDealImg.active = false;
		string l_purchaseObject = SessionHandler.getInstance ().purchaseObject;

		string l_returnJson = SessionHandler.getInstance ().PremiumJson;
		string l_gemsJson = SessionHandler.getInstance ().GemsJson;
		Hashtable l_date = new Hashtable();
		Hashtable l_response = new Hashtable ();
		Hashtable l_planList = new Hashtable ();

		if(l_returnJson.Length > 0)
		{
			l_date = MiniJSON.MiniJSON.jsonDecode (l_returnJson) as Hashtable;

			if(l_date.ContainsKey("subscription_plans"))
				l_planList = l_date["subscription_plans"] as Hashtable;
		}

		if(l_gemsJson.Length > 0)
		{
			l_date = MiniJSON.MiniJSON.jsonDecode (l_gemsJson) as Hashtable;
			Hashtable l_jsonResponse = new Hashtable();

			if(l_date.ContainsKey("jsonResponse"))
				l_jsonResponse = l_date["jsonResponse"] as Hashtable;
			
			if(l_jsonResponse.ContainsKey("response"))
				l_response = l_jsonResponse["response"] as Hashtable;
		}

		switch(l_purchaseObject)
		{
			case "Monthly":
				fillPremiunDate(l_planList,Localization.getString(Localization.TXT_STATE_21_MONTH));
				break;
			case "Yearly":
				fillPremiunDate(l_planList,Localization.getString(Localization.TXT_STATE_21_YEAR));
				break;
			case "good":
				Hashtable l_good = l_response["good"] as Hashtable;
				fillDate(l_good,Localization.getString(Localization.TXT_STATE_21_GOOD));
				break;
			case "better":
				Hashtable l_better = l_response["better"] as Hashtable;
				fillDate(l_better,Localization.getString(Localization.TXT_STATE_21_BETTER));
				break;
			case "best":
				Hashtable l_best = l_response["best"] as Hashtable;
				fillDate(l_best,Localization.getString(Localization.TXT_STATE_21_BEST));
				break;
			default:
				break;
		}
	}

	private void fillDate(Hashtable p_date,string p_title)
	{
		if(Localization.getString(Localization.TXT_STATE_21_BEST).Equals(p_title))
		{
			m_bestDealImg.active =true;
		}
		m_topicText.text = p_title;
		m_topic.text = appendTopicText (p_title);
		m_prePrice.text = p_date["gems"].ToString() + Localization.getString(Localization.TXT_STATE_21_GEMS);
		m_nowPrice.text = Localization.getString(Localization.TXT_STATE_21_MONEY) + p_date["amount"].ToString();
		m_discountText.text = "";
		m_payable.text = Localization.getString(Localization.TXT_STATE_21_MONEY) + p_date["amount"].ToString();;
	}

	private void fillPremiunDate(Hashtable p_date,string p_title)
	{
		float l_nowMonthPrice;
		if(Localization.getString(Localization.TXT_STATE_21_YEAR).Equals(p_title))
		{
			m_bestDealImg.active =true;
			l_nowMonthPrice =float.Parse( p_date["Annual"].ToString());
		}
		else
		{
			m_bestDealImg.active =false;
			l_nowMonthPrice =float.Parse( p_date[p_title].ToString());
		}
		m_topicText.text = p_title;
		m_topic.text = appendTopicText (p_title);
		float l_discount = 0.0f;
		if(p_date.ContainsKey("discount"))
		{
			l_discount = (float) p_date["discount"];
		}
		else
		{
			l_discount = 0.25f;
		}

		m_prePrice.text = string.Format(Localization.getString(Localization.TXT_STATE_21_WAS), (l_nowMonthPrice / ( 1 - l_discount)).ToString("N") );
		m_nowPrice.text = Localization.getString(Localization.TXT_STATE_21_MONEY) + l_nowMonthPrice;
		m_discountText.text = (l_discount*100).ToString() + Localization.getString(Localization.TXT_STATE_21_OFF); 
		m_payable.text = Localization.getString(Localization.TXT_STATE_21_MONEY) + l_nowMonthPrice;
	}

	private string appendTopicText(string l_topic)
	{
		return string.Format(Localization.getString(Localization.TXT_STATE_21_PACKAGE), l_topic );
	}
	
	private void goBack( UIButton p_button )
	{
		string l_purchaseObject = SessionHandler.getInstance ().purchaseObject;
		if("Monthly".Equals(l_purchaseObject) || "Yearly".Equals(l_purchaseObject))
		{
			m_gameController.changeState( ZoodleState.VIEW_PREMIUM );
		}
		else
		{	
			m_gameController.changeState (ZoodleState.BUY_GEMS);
		}
	}

	private void goPaymentConfirm( UIButton p_button )
	{
		m_game.gameController.changeState (ZoodleState.PAY_CONFIRM);
	}

//	//http://loopthetube.com#-GakoQ5_at8&start=0.707&end=63.331
//	private void _loadTestWebpage( string p_webPage )
//	{
//		m_webView = GetComponent< UniWebView >();
//		//m_webView to some event of UniWebView
//		m_webView.OnLoadComplete += OnLoadComplete;
//		//m_webView.OnReceivedMessage += OnReceivedMessage;
//		//m_webView.OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;
//		
//		//Almost full screen but 5 point gap in each edge.
//		m_webView.insets = new UniWebViewEdgeInsets(-10,0,0,0);
//		//Set a url string to load
//		m_webView.url = p_webPage;
//		m_webView.toolBarShow = false;
//		m_webView.Load();
//	}
//	
//	
//	//The listening method of OnLoadComplete method.
//	void OnLoadComplete(UniWebView webView, bool success, string errorMessage) 
//	{
//		if (success) 
//		{
//			//Great, everything goes well. Show the web view now.
//			webView.Show();
//		} 
//		else 
//		{
//			//Oops, something wrong.
//			Debug.LogError("Something wrong in webview loading: " + errorMessage);
//		}
//	}
//	
//	private UniWebView m_webView;

	//Private variables
	
	private UICanvas    m_paymentCanvas;
	private UICanvas	m_signInButtonBackgroundCanvas;

	private UIButton 	m_purchaseButton;
	private UIButton 	m_backButton;
	
	private UILabel m_topicText;
	private UILabel m_prePrice;
	private UILabel m_discountText;
	private UILabel m_nowPrice;
	private UILabel m_payable;
	private UIImage m_bestDealImg;
	private UILabel	m_topic;
	
	private Game 		m_game;
}
