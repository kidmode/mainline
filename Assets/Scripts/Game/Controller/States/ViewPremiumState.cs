using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ViewPremiumState : GameState 
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
		p_gameController.getUI().removeScreen( m_premiumCanvas );
		if( null != m_backScreen )
		{
			p_gameController.getUI().removeScreen( m_backScreen );
		}
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

			float l_discount;
			float l_nowMonthPrice = 0;
			float l_nowYearPrice = 0;

			ArrayList l_planList = l_data["plan_info"] as ArrayList;
			Hashtable l_detail = _getPlanDetailByName(l_planList, "Monthly");
			l_nowMonthPrice = float.Parse(l_detail["amount"].ToString());
			l_detail = _getPlanDetailByName(l_planList, "Annual");
			l_nowYearPrice =float.Parse(l_detail["amount"].ToString());
			l_discount = 0.25f;

			l_nowMonthPriceLabel.text = "$" + l_nowMonthPrice.ToString("N");
			l_nowYearPriceLabel.text = "$" + l_nowYearPrice.ToString("N");
			l_MonthDiscountLabel.text = (l_discount*100).ToString() + "%"; 
			l_YealyDiscountLabel.text = (l_discount*100).ToString() + "%"; 
			l_preMonthPriceLabel.text = "$"+ (l_nowMonthPrice /(1- l_discount)).ToString("N");
			l_preYearPriceLabel.text = "$"+ (l_nowYearPrice /(1- l_discount)).ToString("N");
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
		int l_state = m_gameController.getConnectedState ( ZoodleState.VIEW_PREMIUM );
		m_game.gameController.changeState (l_state);
	}

	private void gotoMonthlyPurchase( UIButton p_button )
	{
		SessionHandler.getInstance ().purchaseObject = "Monthly";
		m_game.gameController.changeState (ZoodleState.PLAN_PAYMENT);
	}

	private void gotoYearlyPurchase( UIButton p_button )
	{
		SessionHandler.getInstance ().purchaseObject = "Annual";
		m_game.gameController.changeState (ZoodleState.PLAN_PAYMENT);
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
