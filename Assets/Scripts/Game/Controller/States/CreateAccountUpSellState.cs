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
		if (string.Empty.Equals(SessionHandler.getInstance().PremiumJson))
		{
			Server.init(ZoodlesConstants.getHttpsHost());
			m_queue.add(new GetPlanDetailsRequest(_getPlanDetailsComplete));
			m_queue.request();
		}
		else
			_setupScreen(p_gameController.getUI());
	}
	
	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		if (m_subState != SubState.NONE)
		{
			switch (m_subState)
			{
			case SubState.GO_TRIAL:
				if (m_cardNumber.text == "CC#" || m_cardMonth.text == "Month" || m_cardYear.text == "Year")
					setErrorMessage(m_gameController, "Invalid value", "Your credit card info is incorrect!");
				else
				{
					p_gameController.getUI().removeScreen(m_payConfirmCanvas);
					m_payConfirmCanvas = null;
					p_gameController.getUI().createScreen(UIScreen.LOADING_SPINNER);
					Server.init(ZoodlesConstants.getHttpsHost());
					m_queue.add(new PaymentRequest("-1", CreditCardHelper.parseType(m_cardNumber.text), m_cardNumber.text, m_cardMonth.text, m_cardYear.text, _onPaymentComplete));
					m_queue.request(RequestType.RUSH);
				}
				break;
			case SubState.GO_REFUSE:
				if (null != SessionHandler.getInstance().kidList && SessionHandler.getInstance().kidList.Count > 0)
				{
					int l_state = p_gameController.getConnectedState( ZoodleState.SIGN_UP_UPSELL );
					if( l_state != -999 && l_state != -1 )
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
					p_gameController.changeState(ZoodleState.CREATE_CHILD);
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
		p_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER);
		p_gameController.getUI().removeScreen(m_payConfirmCanvas);

		base.exit(p_gameController);
	}

	//---------------- Private Implementation ----------------------
	
	private void _setupScreen(UIManager p_uiManager)
	{	
		m_payConfirmCanvas = p_uiManager.createScreen(UIScreen.SIGIN_UP_UPSELL, true, 1);

		m_backButton = m_payConfirmCanvas.getView("backButton") as UIButton;
		m_backButton.addClickCallback(goBack);

		string l_returnJson = SessionHandler.getInstance().PremiumJson;
		Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(l_returnJson) as Hashtable;
		ArrayList l_planList = l_data["plan_info"] as ArrayList;
		Hashtable l_plan = _getPlanDetailByName(l_planList, "Monthly");

		UILabel l_info = m_payConfirmCanvas.getView("info") as UILabel;
		l_info.text = string.Format("By continuing you agree to our Terms of Service and Privacy Policy.\nAfter the trail ends, your card will be charged ${0} monthly.", l_plan["amount"]);

		m_cardNumber = m_payConfirmCanvas.getView("cardNumber") as UIInputField;
		m_cardNumber.listener.onSelect += _onCardNumberSelect;
		m_cardNumber.listener.onDeselect += _onCardNumberDeselect;
		m_cardMonth = m_payConfirmCanvas.getView("cardExpiredMonth") as UIInputField;
		m_cardMonth.listener.onSelect += _onCardMonthSelect;
		m_cardMonth.listener.onDeselect += _onCardMonthSelect;
		m_cardYear = m_payConfirmCanvas.getView("cardExpiredYear") as UIInputField;
		m_cardYear.listener.onSelect += _onCardYearSelect;
		m_cardYear.listener.onDeselect += _onCardYearDeselect;

		m_startFreeTrial = m_payConfirmCanvas.getView("purchaseButton") as UIButton;
		m_startFreeTrial.addClickCallback(startFreeTrial);
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

	private void _getPlanDetailsComplete(WWW p_response)
	{
		Server.init(ZoodlesConstants.getHost());
		if (null == p_response.error)
		{
			SessionHandler.getInstance().PremiumJson = p_response.text;
			_setupScreen(m_gameController.getUI());
		}
		else
			setErrorMessage(m_gameController, "Fail", "Get data failed, please try it again.");
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

	//Private variables
	
	private UICanvas    m_payConfirmCanvas;

	private UIButton 	m_backButton;
	private UIButton 	m_startFreeTrial;
	private UIButton 	m_refuseButton;

	private UIInputField 	m_cardNumber;
	private UIInputField 	m_cardMonth;
	private UIInputField	m_cardYear;

	private RequestQueue m_queue = new RequestQueue();
	private SubState 	m_subState = SubState.NONE;
}
