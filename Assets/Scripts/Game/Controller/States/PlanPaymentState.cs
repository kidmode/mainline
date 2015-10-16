using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlanPaymentState : GameState
{
	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);
		
		_setupScreen(p_gameController.getUI());
	}
	
	public override void exit(GameController p_gameController)
	{
		p_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);
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
		
		m_cardMonth.text = Localization.getString( Localization.TXT_23_LABEL_MONTH );
		m_cardYear.text = Localization.getString( Localization.TXT_23_LABEL_YEAR );

		m_monthComBox = m_paymentCanvas.getView ("monthChcekbox") as UIComboBox;
		m_yearComBox = m_paymentCanvas.getView ("yearChcekbox") as UIComboBox;
		
		List<object> l_monthDate = new List<object> ();
		List<object> l_yearDate = new List<object> ();
		l_monthDate.Add (new ComboBoxData("01",1));
		l_monthDate.Add (new ComboBoxData("02",2));
		l_monthDate.Add (new ComboBoxData("03",3));
		l_monthDate.Add (new ComboBoxData("04",4));
		l_monthDate.Add (new ComboBoxData("05",5));
		l_monthDate.Add (new ComboBoxData("06",6));
		l_monthDate.Add (new ComboBoxData("07",7));
		l_monthDate.Add (new ComboBoxData("08",8));
		l_monthDate.Add (new ComboBoxData("09",9));
		l_monthDate.Add (new ComboBoxData("10",10));
		l_monthDate.Add (new ComboBoxData("11",11));
		l_monthDate.Add (new ComboBoxData("12",12));
		m_monthComBox.setSwipeListDate (l_monthDate);
		int l_nowYear = System.DateTime.Now.Year;
		int l_year = 0;
		for (int l_i=0; l_i<8; l_i++) 
		{
			l_year = l_nowYear + l_i;
			l_yearDate.Add(new ComboBoxData(l_year.ToString(),l_year));
		}
		m_yearComBox.setSwipeListDate (l_yearDate);
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

		//m_prePrice.text = "was $"+ (l_nowMonthPrice / ( 1 - l_discount)).ToString("N") + " now";
		m_prePrice.text = string.Format(Localization.getString( Localization.TXT_STATE_65_DISCOUNT ),(l_nowMonthPrice / ( 1 - l_discount)).ToString("N"));
		m_nowPrice.text = "$" + l_nowMonthPrice;
		m_discountText.text = (l_discount*100).ToString() + Localization.getString( Localization.TXT_STATE_65_OFF); 
		m_payable.text = "$" + l_nowMonthPrice;
	}
	
	private string appendTopicText(string p_topic)
	{
		//return "You've selected our " + p_topic + " Package.";
		return string.Format (Localization.getString( Localization.TXT_STATE_65_SELECT_PACKAGE ),p_topic);
	}
	
	private void _goBack(UIButton p_button)
	{
		m_gameController.changeState(ZoodleState.VIEW_PREMIUM);
	}
	
	private void _goPaymentConfirm(UIButton p_button)
	{
		int l_selectYear = int.Parse(m_yearComBox.currentData.entryValue.ToString());
		int l_selectMonth = int.Parse(m_monthComBox.currentData.entryValue.ToString());
		if(l_selectYear == System.DateTime.Now.Year && l_selectMonth < System.DateTime.Now.Month)
		{
			setErrorMessage(m_gameController, Localization.getString(Localization.TXT_STATE_39_INVAILD), Localization.getString(Localization.TXT_8_DATE_VERIFIY));
		}
		else
		{
			if (m_cardNumber.text == Localization.getString( Localization.TXT_STATE_65_CC))
				setErrorMessage(m_gameController, Localization.getString( Localization.TXT_STATE_65_INVALID_VALUE), Localization.getString( Localization.TXT_STATE_65_EMPTY));
			else
			{
				m_gameController.getUI().removeScreen(m_paymentCanvas);
				m_paymentCanvas = null;
				m_gameController.getUI().createScreen(UIScreen.LOADING_SPINNER_ELEPHANT);
				string l_purchaseObject = SessionHandler.getInstance().purchaseObject;
				string l_returnJson = SessionHandler.getInstance().PremiumJson;
				Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(l_returnJson) as Hashtable;
				ArrayList l_planList = l_data["plan_info"] as ArrayList;
				Hashtable l_plan = _getPlanDetailByName(l_planList, l_purchaseObject);

				string l_cardType = CreditCardHelper.parseType(m_cardNumber.text);
				if(string.Empty.Equals(l_cardType))
				{
					setErrorMessage(m_gameController, Localization.getString( Localization.TXT_STATE_65_INVALID_VALUE), Localization.getString( Localization.TXT_STATE_65_INCORRECT));
					return;
				}
				Server.init(ZoodlesConstants.getHttpsHost());
				RequestQueue l_queue = new RequestQueue();
				l_queue.add(new PaymentRequest(l_plan["id"].ToString(), l_cardType, m_cardNumber.text, m_monthComBox.currentData.entryValue.ToString(), m_yearComBox.currentData.entryValue.ToString(), _onPaymentComplete));
				l_queue.request(RequestType.RUSH);
			}
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
						setErrorMessage(m_gameController, Localization.getString( Localization.TXT_STATE_65_INVALID_VALUE), Localization.getString( Localization.TXT_STATE_65_INCORRECT));
					}
				}
			}
		}
		else
		{
			setErrorMessage(m_gameController, Localization.getString( Localization.TXT_STATE_65_INVALID_VALUE), Localization.getString( Localization.TXT_STATE_65_INCORRECT));
		}
	}

	private void _onCardNumberSelect(UIElement p_element)
	{
		if (m_cardNumber.text == Localization.getString( Localization.TXT_STATE_65_CC))
			m_cardNumber.text = "";
	}

	private void _onCardNumberDeselect(UIElement p_element)
	{
		if (m_cardNumber.text == "")
			m_cardNumber.text = Localization.getString( Localization.TXT_STATE_65_CC);
	}

	private void _onCardMonthSelect(UIElement p_element)
	{
		if (m_cardMonth.text == Localization.getString( Localization.TXT_23_LABEL_MONTH))
			m_cardMonth.text = "";
	}

	private void _onCardMonthDeselect(UIElement p_element)
	{
		if (m_cardMonth.text == "")
			m_cardMonth.text = Localization.getString( Localization.TXT_23_LABEL_MONTH);
	}

	private void _onCardYearSelect(UIElement p_element)
	{
		if (m_cardYear.text == Localization.getString( Localization.TXT_23_LABEL_YEAR))
			m_cardYear.text = "";
	}

	private void _onCardYearDeselect(UIElement p_element)
	{
		if (m_cardYear.text == "")
			m_cardYear.text = Localization.getString( Localization.TXT_23_LABEL_YEAR);
	}
	
	private UICanvas m_paymentCanvas;
	private UICanvas m_signInButtonBackgroundCanvas;
	
	private UIButton m_purchaseButton;
	private UIButton m_backButton;
	
	private UIComboBox  m_yearComBox;
	private UIComboBox  m_monthComBox;
	
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

