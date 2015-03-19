using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class PlanPaymentState : GameState
{
	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);
		
		_setupScreen(p_gameController.getUI());
	}
	
	public override void exit(GameController p_gameController)
	{
		p_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER);
		p_gameController.getUI().removeScreen(m_paymentCanvas);

		base.exit(p_gameController);
	}
	
	//---------------- Private Implementation ----------------------
	
	private void _setupScreen(UIManager p_uiManager)
	{
		m_paymentCanvas = p_uiManager.createScreen(UIScreen.PAYMENT, true, 1);
		
		m_backButton = m_paymentCanvas.getView("exitButton") as UIButton;
		m_backButton.addClickCallback(_goBack);
		
		m_purchaseButton = m_paymentCanvas.getView("purchaseButton") as UIButton;
		m_purchaseButton.addClickCallback(_goPaymentConfirm);
		
		m_topicText = m_paymentCanvas.getView("topicText") as UILabel;
		m_topic = m_paymentCanvas.getView("topic") as UILabel;
		m_prePrice = m_paymentCanvas.getView("prePrice") as UILabel;
		m_discountText = m_paymentCanvas.getView("discount") as UILabel;
		m_nowPrice = m_paymentCanvas.getView("nowPrice") as UILabel;
		m_payable = m_paymentCanvas.getView("payableText") as UILabel;
		m_bestDealImg = m_paymentCanvas.getView("recommendImage") as UIImage;
		m_bestDealImg.active = false;

		string l_purchaseObject = SessionHandler.getInstance().purchaseObject;
		string l_returnJson = SessionHandler.getInstance().PremiumJson;
		ArrayList l_planList = new ArrayList ();
		
		if (l_returnJson.Length > 0)
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(l_returnJson) as Hashtable;
			if (l_data.ContainsKey("plan_info"))
				l_planList = l_data["plan_info"] as ArrayList;
		}
		
		_fillPremiunDate(_getPlanDetailByName(l_planList, l_purchaseObject));

		m_cardNumber = m_paymentCanvas.getView("cardNumber") as UIInputField;
		m_cardNumber.listener.onSelect += _onCardNumberSelect;
		m_cardNumber.listener.onDeselect += _onCardNumberDeselect;
		m_cardMonth = m_paymentCanvas.getView("cardExpiredMonth") as UIInputField;
		m_cardMonth.listener.onSelect += _onCardMonthSelect;
		m_cardMonth.listener.onDeselect += _onCardMonthDeselect;
		m_cardYear = m_paymentCanvas.getView("cardExpiredYear") as UIInputField;
		m_cardYear.listener.onSelect += _onCardYearSelect;
		m_cardYear.listener.onDeselect += _onCardYearDeselect;
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
	
	private void _fillPremiunDate(Hashtable p_data)
	{
		string l_title = p_data["en_name"].ToString();
		if ("Yearly".Equals(l_title))
			m_bestDealImg.active = true;

		float l_nowMonthPrice = float.Parse(p_data["amount"].ToString());

		m_topicText.text = l_title;
		m_topic.text = appendTopicText(l_title);
		float l_discount = 0.0f;
		if (p_data.ContainsKey("discount"))
			l_discount = (float)p_data["discount"];
		else
			l_discount = 0.25f;

		m_prePrice.text = "was $"+ (l_nowMonthPrice / ( 1 - l_discount)).ToString("N") + " now";
		m_nowPrice.text = "$" + l_nowMonthPrice;
		m_discountText.text = (l_discount*100).ToString() + "% off"; 
		m_payable.text = "$" + l_nowMonthPrice;
	}
	
	private string appendTopicText(string p_topic)
	{
		return "You've selected our " + p_topic + " Package.";
	}
	
	private void _goBack(UIButton p_button)
	{
		m_gameController.changeState(ZoodleState.VIEW_PREMIUM);
	}
	
	private void _goPaymentConfirm(UIButton p_button)
	{
		if (m_cardNumber.text == "CC#" || m_cardMonth.text == "Month" || m_cardYear.text == "Year")
			setErrorMessage(m_gameController, "Invalid value", "Your credit card info is incorrect!");
		else
		{
			m_gameController.getUI().removeScreen(m_paymentCanvas);
			m_paymentCanvas = null;
			m_gameController.getUI().createScreen(UIScreen.LOADING_SPINNER);
			string l_purchaseObject = SessionHandler.getInstance().purchaseObject;
			string l_returnJson = SessionHandler.getInstance().PremiumJson;
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(l_returnJson) as Hashtable;
			ArrayList l_planList = l_data["plan_info"] as ArrayList;
			Hashtable l_plan = _getPlanDetailByName(l_planList, l_purchaseObject);

			string l_cardType = CreditCardHelper.parseType(m_cardNumber.text);
			Server.init(ZoodlesConstants.getHttpsHost());
			RequestQueue l_queue = new RequestQueue();
			l_queue.add(new PaymentRequest(l_plan["id"].ToString(), l_cardType, m_cardNumber.text, m_cardMonth.text, m_cardYear.text, _onPaymentComplete));
			l_queue.request(RequestType.RUSH);
		}
	}
	
	private void _onPaymentComplete(WWW p_response)
	{
		Server.init(ZoodlesConstants.getHost());
		if(null == p_response.error)
		{
			Hashtable l_jsonResponse = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			if(l_jsonResponse.ContainsKey("jsonResponse"))
			{
				Hashtable l_response = l_jsonResponse["jsonResponse"] as Hashtable;
				if(l_response.ContainsKey("response"))
				{
					if(null != l_response["response"])
					{
						SessionHandler.getInstance ().token.setTry(true);
						SessionHandler.getInstance ().token.setCurrent(true);
						SessionHandler.getInstance ().token.setPremium(true);
						m_gameController.changeState(ZoodleState.PAY_CONFIRM);
					}
					else
					{
						setErrorMessage(m_gameController, "Invalid value", "Your credit card info is incorrect!");
					}
				}
			}
		}
		else
		{
			setErrorMessage(m_gameController, "Invalid value", "Your credit card info is incorrect!");
		}
	}

	private void _onCardNumberSelect(UIElement p_element)
	{
		if (m_cardNumber.text == "CC#")
			m_cardNumber.text = "";
	}

	private void _onCardNumberDeselect(UIElement p_element)
	{
		if (m_cardNumber.text == "")
			m_cardNumber.text = "CC#";
	}

	private void _onCardMonthSelect(UIElement p_element)
	{
		if (m_cardMonth.text == "Month")
			m_cardMonth.text = "";
	}

	private void _onCardMonthDeselect(UIElement p_element)
	{
		if (m_cardMonth.text == "")
			m_cardMonth.text = "Month";
	}

	private void _onCardYearSelect(UIElement p_element)
	{
		if (m_cardYear.text == "Year")
			m_cardYear.text = "";
	}

	private void _onCardYearDeselect(UIElement p_element)
	{
		if (m_cardYear.text == "")
			m_cardYear.text = "Year";
	}
	
	private UICanvas m_paymentCanvas;
	private UICanvas m_signInButtonBackgroundCanvas;
	
	private UIButton m_purchaseButton;
	private UIButton m_backButton;
	
	private UILabel m_topicText;
	private UILabel m_prePrice;
	private UILabel m_discountText;
	private UILabel m_nowPrice;
	private UILabel m_payable;
	private UIImage m_bestDealImg;
	private UILabel	m_topic;
	private UIInputField m_cardNumber;
	private UIInputField m_cardMonth;
	private UIInputField m_cardYear;
}

