using UnityEngine;
using System.Collections;

public class PremiumEligibleState : GameState {

	UICanvas mPremiumEligibleCanvas;

	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);
		_setupScreen( p_gameController.getUI() );
	}

	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );

		p_gameController.getUI().removeScreenImmediately(mPremiumEligibleCanvas);
	}

	private void _setupScreen(UIManager p_uiManager)
	{
		SplashBackCanvas bgCanvas = p_uiManager.findScreen( UIScreen.SPLASH_BACKGROUND ) as SplashBackCanvas;
		if( bgCanvas == null )
			bgCanvas = p_uiManager.createScreen(UIScreen.SPLASH_BACKGROUND, true, -1) as SplashBackCanvas;

		mPremiumEligibleCanvas = p_uiManager.createScreen( UIScreen.PREMIUM_ELIGIBLE, false , 2 );
		UIButton l_continueButton = mPremiumEligibleCanvas.getView("continueButton") as UIButton;
		UIButton l_exitButton = mPremiumEligibleCanvas.getView("exitButton") as UIButton;
		l_continueButton.addClickCallback( onContinueClick );
		l_exitButton.addClickCallback( onContinueClick );
		UILabel l_message = mPremiumEligibleCanvas.getView("messageText") as UILabel;
		
		string l_deviceName = SessionHandler.getInstance().deviceName;
		int l_renewalPeriod = SessionHandler.getInstance().renewalPeriod;
		
		string l_messageText = string.Format( Localization.getString (Localization.TXT_105_LABEL_CONTENT_NOTICE), l_deviceName, l_renewalPeriod);
		l_message.text = l_messageText;

		bgCanvas.setDown();
	}

	private void onContinueClick(UIButton p_button)
	{
		m_gameController.changeState(ZoodleState.SET_UP_ACCOUNT);
	}

}
