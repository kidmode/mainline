using UnityEngine;
using System.Collections;

public class CongratsState : GameState
{
	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);

		m_time = 0;
		m_clicked = false;
		m_requested = false;

		UIManager l_ui = p_gameController.getUI();

		l_ui.createScreen(UIScreen.CONGRATS_BACKGROUND, false, -1);
//		m_screenBackground = l_ui.findScreen (UIScreen.CONGRATS_BACKGROUND) as UICanvas;

//		m_screenBackground.getView ("holder").gameObject.SetActive (true);
//		ShowAndHide showBackGround = m_screenBackground.getView ("holder").gameObject.GetComponent<ShowAndHide>();
//		
//		showBackGround.show();



		m_screen =  l_ui.createScreen(UIScreen.CONGRATS, false, 1) as UICanvas;//l_ui.findScreen (UIScreen.CONGRATS) as UICanvas;// l_ui.createScreen(UIScreen.CONGRATS, false, 1) as UICanvas;

//		ShowAndHide showScreen = m_screen.getView ("holder").gameObject.GetComponent<ShowAndHide>();
//		
//		showScreen.show();



		UILabel l_info = m_screen.getView("zpInfo") as UILabel;
		l_info.text = Localization.getString(Localization.TXT_STATE_49_ZP);

		UILabel l_pointNumber = m_screen.getView("zpNumberInfo") as UILabel;
		l_pointNumber.text = "5"; 

		UILabel l_requestInfo = m_screen.getView("requestInfo") as UILabel;
		l_requestInfo.text = Localization.getString(Localization.TXT_STATE_49_UPLOADING);

		m_loadingBarImg = m_screen.getView("loadingBarSprite") as UIImage;

		UIButton l_next = m_screen.getView("nextButton") as UIButton;
		l_next.addClickCallback(_click);

		SoundManager.getInstance().playVolume("win", 0, -1, "", null, false, 0.2f);

//		m_screen.getView("holder").tweener.addAlphaTrack( 0.0f, 1.0f, 1.0f );

//		SoundManager.getInstance().play("win", 0, -1, "", null, true);

		m_queue = new RequestQueue();
		m_queue.add(new GetZPs(2, _requestComplete));
		m_queue.request(RequestType.RUSH);

		GAUtil.logScreen("CongratsScreen");
	}

	void startScreen(){



	}

	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		m_time += p_time;
		if (m_time < 1250)
		{
			float l_fillAmount = Mathf.Lerp(0, 1.0f, m_time / 1250.0f);
			m_loadingBarImg.fillAmount = l_fillAmount;
		}
		else
			m_loadingBarImg.fillAmount = 1.0f;

		if( !m_requested )
		{
			m_clicked = false;		
		}

		if (m_requested && m_loadingBarImg.fillAmount == 1.0f) {

//			int l_nextState = p_gameController.getConnectedState(ZoodleState.CONGRATS_STATE);
//			if (l_nextState != -1)
//				p_gameController.changeState(l_nextState);

		}






		if (m_clicked)
		{
			m_clicked = false;
			int l_nextState = p_gameController.getConnectedState(ZoodleState.CONGRATS_STATE);
			if (l_nextState != -1)
				p_gameController.changeState(l_nextState);
		}
	}

	public override void exit(GameController p_gameController)
	{
		UIManager l_ui = p_gameController.getUI();
		l_ui.removeScreen(UIScreen.CONGRATS_BACKGROUND);
		l_ui.removeScreen(UIScreen.CONGRATS);

//		ShowAndHide showBackGround = m_screenBackground.getView ("holder").gameObject.GetComponent<ShowAndHide>();
//		
//		showBackGround.hide();
//
//
//
//		ShowAndHide showScreen = m_screen.getView ("holder").gameObject.GetComponent<ShowAndHide>();
//		
//		showScreen.hide();


		base.exit(p_gameController);
	}

	private void _click(UIButton p_button)
	{
		m_clicked = true;
	}

	private void _requestComplete(WWW p_response)
	{
		if (p_response.error != null)
			m_gameController.changeState(ZoodleState.SERVER_ERROR);
		else
		{
			Hashtable l_jsonResponse = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			if (l_jsonResponse.ContainsKey("jsonResponse"))
			{
				Hashtable l_response = l_jsonResponse["jsonResponse"] as Hashtable;
				if (l_response.ContainsKey("response"))
				{
					Hashtable l_data = l_response["response"] as Hashtable;
					Kid l_kid = SessionHandler.getInstance().currentKid;
					l_kid.level = int.Parse(l_data["level"].ToString());
					l_kid.stars = int.Parse(l_data["zps"].ToString());
				}
			}

			m_requested = true;

			UILabel l_requestInfo = m_screen.getView("requestInfo") as UILabel;
			l_requestInfo.text = Localization.getString(Localization.TXT_STATE_49_CONTINUE);
		}
	}

	private UIImage m_loadingBarImg;
	private int m_time = 0;
	private bool m_clicked = false;
	private bool m_requested = false;
	private RequestQueue m_queue = null;
	private UICanvas m_screen;
	private UICanvas m_screenBackground;
}
