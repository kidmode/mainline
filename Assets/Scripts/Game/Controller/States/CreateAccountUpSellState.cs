using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CreateAccountUpSellState : GameState 
{
	enum SubState
	{
		NONE,
		GO_TRIAL,
		GO_REFUSE,
		GO_CONGRATS
	}

	//Standard state flow	
	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);

		m_queue.reset();
		m_firstUse = string.Empty.Equals (SessionHandler.getInstance ().token.getSecret ());
		if(m_firstUse)
		{
			_setupScreen(m_gameController.getUI());
		}
		else
		{
			if (string.Empty.Equals(SessionHandler.getInstance().PremiumJson))
			{
				Server.init(ZoodlesConstants.getHttpsHost());
				m_queue.add(new GetPlanDetailsRequest(_getPlanDetailsComplete));
				m_queue.request();
			}
			else
				_setupScreen(p_gameController.getUI());
		}

	}
	
	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		if (m_subState != SubState.NONE)
		{
			switch (m_subState)
			{
			case SubState.GO_TRIAL:
				int l_selectYear = int.Parse(m_yearComBox.currentData.entryValue.ToString());
				int l_selectMonth = int.Parse(m_monthComBox.currentData.entryValue.ToString());
				if(l_selectYear == System.DateTime.Now.Year && l_selectMonth < System.DateTime.Now.Month)
				{
					setErrorMessage(m_gameController, Localization.getString(Localization.TXT_STATE_39_INVAILD), Localization.getString(Localization.TXT_8_DATE_VERIFIY));
				}
				else
				{
					if (m_cardNumber.text == Localization.getString(Localization.TXT_STATE_39_CC) || string.Empty.Equals(CreditCardHelper.parseType(m_cardNumber.text)))
						setErrorMessage(m_gameController, Localization.getString(Localization.TXT_STATE_39_INVAILD), Localization.getString(Localization.TXT_STATE_65_EMPTY));
					else
					{
						if(m_firstUse)
						{
							SessionHandler.getInstance().creditCardNum = m_cardNumber.text;
							SessionHandler.getInstance().cardYear = m_yearComBox.currentData.entryValue.ToString();
							SessionHandler.getInstance().cardMonth = m_monthComBox.currentData.entryValue.ToString();
							m_gameController.changeState(ZoodleState.SET_UP_ACCOUNT);
						}
						else
						{
							p_gameController.getUI().removeScreen(m_payConfirmCanvas);
							m_payConfirmCanvas = null;
							p_gameController.getUI().createScreen(UIScreen.LOADING_SPINNER);
							Server.init(ZoodlesConstants.getHttpsHost());
							m_queue.add(new PaymentRequest("-1", CreditCardHelper.parseType(m_cardNumber.text), m_cardNumber.text, m_monthComBox.currentData.entryValue.ToString(), m_yearComBox.currentData.entryValue.ToString(), _onPaymentComplete));
							m_queue.request(RequestType.RUSH);
						}
					}
				}

				break;
			case SubState.GO_REFUSE:
				if ((null != SessionHandler.getInstance().kidList && SessionHandler.getInstance().kidList.Count > 0) || m_firstUse)
				{
					int l_state = p_gameController.getConnectedState( ZoodleState.SIGN_UP_UPSELL );
					if( l_state != GameController.UNDEFINED_STATE && l_state != -1 )
					{
						p_gameController.changeState(l_state);
					}
					else
					{
						p_gameController.changeState(ZoodleState.PROFILE_SELECTION);
					}
				}
				else
				{
					p_gameController.changeState(ZoodleState.CREATE_CHILD_NEW);
				}
				break;
			case SubState.GO_CONGRATS:
				p_gameController.changeState(ZoodleState.UPSELL_CONGRATURATION);
				break;
			}
			
			m_subState = SubState.NONE;
		}
	}
	
	public override void exit(GameController p_gameController)
	{
		p_gameController.getUI().removeScreenImmediately(UIScreen.LOADING_SPINNER);
		p_gameController.getUI().removeScreenImmediately(m_payConfirmCanvas);

		base.exit(p_gameController);
	}

	//---------------- Private Implementation ----------------------
	
	private void _setupScreen(UIManager p_uiManager)
	{
		m_payConfirmCanvas = p_uiManager.createScreen(UIScreen.SIGIN_UP_UPSELL, true, 1);

		m_backButton = m_payConfirmCanvas.getView("backButton") as UIButton;
		m_backButton.addClickCallback(goBack);

		m_thanksButton = m_payConfirmCanvas.getView ( "thanksButton" ) as UIButton;
		m_thanksButton.addClickCallback ( onThanksClick );
		if( m_firstUse )
		{
			m_thanksButton.active = true;
		}

		Hashtable l_plan = new Hashtable ();
		if(!m_firstUse)
		{
			string l_returnJson = SessionHandler.getInstance().PremiumJson;
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(l_returnJson) as Hashtable;
			ArrayList l_planList = l_data["plan_info"] as ArrayList;
			l_plan = _getPlanDetailByName(l_planList, "Monthly");
		}

		UILabel l_info = m_payConfirmCanvas.getView("info") as UILabel;

		if(!m_firstUse)
		{
			l_info.text = string.Format(Localization.getString(Localization.TXT_65_LABEL_INFO), l_plan["amount"]);
		}
		else
		{
			l_info.active = false;
		}

		m_cardNumber = m_payConfirmCanvas.getView("cardNumber") as UIInputField;
		m_cardNumber.listener.onSelect += _onCardNumberSelect;
		m_cardNumber.listener.onDeselect += _onCardNumberDeselect;
		m_cardMonth = m_payConfirmCanvas.getView("cardExpiredMonth") as UIInputField;
		m_cardMonth.listener.onSelect += _onCardMonthSelect;
		m_cardMonth.listener.onDeselect += _onCardMonthDeselect;
		m_cardYear = m_payConfirmCanvas.getView("cardExpiredYear") as UIInputField;
		m_cardYear.listener.onSelect += _onCardYearSelect;
		m_cardYear.listener.onDeselect += _onCardYearDeselect;
		m_cardMonth.text = Localization.getString( Localization.TXT_23_LABEL_MONTH );
		m_cardYear.text = Localization.getString( Localization.TXT_23_LABEL_YEAR );

		m_startFreeTrial = m_payConfirmCanvas.getView("purchaseButton") as UIButton;
		m_startFreeTrial.addClickCallback(startFreeTrial);

		m_monthComBox = m_payConfirmCanvas.getView ("monthChcekbox") as UIComboBox;
		m_yearComBox = m_payConfirmCanvas.getView ("yearChcekbox") as UIComboBox;

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

	private void onThanksClick( UIButton p_button )
	{
		//TODO create free account
		p_button.removeClickCallback ( onThanksClick );
		int l_state = m_gameController.getConnectedState (ZoodleState.SIGN_UP_UPSELL);
		if(ZoodleState.SIGN_IN_UPSELL == l_state)
			m_gameController.changeState ( l_state );
		else
			m_gameController.changeState ( ZoodleState.SET_UP_ACCOUNT );
	}

	private void goBack(UIButton p_button)
	{
		m_subState = SubState.GO_REFUSE;
	}

	private void startFreeTrial(UIButton p_button)
	{
		m_subState = SubState.GO_TRIAL;
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
						SessionHandler.getInstance ().token.setPremium(false);
						m_subState = SubState.GO_CONGRATS;
					}
					else
					{
						setErrorMessage(m_gameController, Localization.getString(Localization.TXT_STATE_39_INVAILD), Localization.getString(Localization.TXT_STATE_39_CREDIT));
					}
				}
			}
		}
		else
		{
			setErrorMessage(m_gameController, Localization.getString(Localization.TXT_STATE_39_INVAILD), Localization.getString(Localization.TXT_STATE_39_CREDIT));
		}
	}

	private void _getPlanDetailsComplete(WWW p_response)
	{
		Server.init(ZoodlesConstants.getHost());
		if (null == p_response.error)
		{
			SessionHandler.getInstance().PremiumJson = p_response.text;
			_setupScreen(m_gameController.getUI());
		}
		else
			setErrorMessage(m_gameController, Localization.getString(Localization.TXT_STATE_11_FAIL), Localization.getString(Localization.TXT_STATE_11_FAIL_DATA));
	}

	private void _onCardNumberSelect(UIElement p_element)
	{
		if (m_cardNumber.text == Localization.getString(Localization.TXT_STATE_39_CC))
			m_cardNumber.text = "";
	}
	
	private void _onCardNumberDeselect(UIElement p_element)
	{
		if (m_cardNumber.text == "")
			m_cardNumber.text = Localization.getString(Localization.TXT_STATE_39_CC);
	}
	
	private void _onCardMonthSelect(UIElement p_element)
	{
		if (m_cardMonth.text == Localization.getString(Localization.TXT_STATE_39_MONTH))
			m_cardMonth.text = "";
	}
	
	private void _onCardMonthDeselect(UIElement p_element)
	{
		if (m_cardMonth.text == "")
			m_cardMonth.text = Localization.getString(Localization.TXT_STATE_39_MONTH);
	}
	
	private void _onCardYearSelect(UIElement p_element)
	{
		if (m_cardYear.text == Localization.getString(Localization.TXT_STATE_39_YEAR))
			m_cardYear.text = "";
	}
	
	private void _onCardYearDeselect(UIElement p_element)
	{
		if (m_cardYear.text == "")
			m_cardYear.text = Localization.getString(Localization.TXT_STATE_39_YEAR);
	}

	//Private variables
	
	private UICanvas    m_payConfirmCanvas;
	private bool 		m_firstUse;
	private UIButton 	m_backButton;
	private UIButton 	m_startFreeTrial;
	private UIButton 	m_refuseButton;
	private UIButton 	m_thanksButton;
	private UIComboBox  m_yearComBox;
	private UIComboBox  m_monthComBox;

	private UIInputField 	m_cardNumber;
	private UIInputField 	m_cardMonth;
	private UIInputField	m_cardYear;

	private RequestQueue m_queue = new RequestQueue();
	private SubState 	m_subState = SubState.NONE;
}
