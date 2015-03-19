using UnityEngine;
using System;

public class WebViewState : GameState
{
	protected enum SubState
	{
		NONE,
		GO_CONGRATS
	}

	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);

		SoundManager.getInstance().stopMusic();

		m_subState = SubState.NONE;

		UICanvas l_screen = p_gameController.getUI().createScreen(UIScreen.WEBVIEW);
		UIButton l_button = l_screen.getView("backButton") as UIButton;
		l_button.addClickCallback(_clickBack);

		_setupWebView("Prefabs/Web/YoutubeWebview", l_screen.scaleFactor);
		m_webView.Load(SessionHandler.getInstance().currentContent.url);
		m_webView.Show();

		m_linkId = SessionHandler.getInstance().currentContent.id;
		m_duration = 0;

		GAUtil.logScreen("WebViewScreen");
	}

	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		m_duration += p_time;
//		if (m_subState != SubState.NONE)
//		{
//			switch (m_subState)
//			{
//			case SubState.GO_CONGRATS:
//				p_gameController.connectState(ZoodleState.CONGRATS_STATE, ZoodleState.REGION_LANDING);
//				p_gameController.changeState(ZoodleState.CONGRATS_STATE);
//				break;
//			}
//
//			m_subState = SubState.NONE;
//		}
	}

	public override void exit(GameController p_gameController)
	{
		GameObject.Destroy(m_webObj);
		m_webObj = null;
		m_webView = null;

		p_gameController.getUI().removeScreenImmediately(UIScreen.WEBVIEW);

		new VisitLinkTrackRequest().send(m_linkId, (int)Math.Ceiling(m_duration * 0.001));

		base.exit(p_gameController);
	}

	public override bool handleMessage(GameController p_gameController, int p_type, string p_string)
	{
		if (p_type == 1)
		{
			if (m_webView != null)
				m_webView.Reload();
		}
		
		return true;
	}

	private void _setupWebView(string p_webViewPrefab, float p_scale)
	{
		GameObject l_webViewPrefab = Resources.Load(p_webViewPrefab) as GameObject;
		if (l_webViewPrefab == null)// || Application.platform != RuntimePlatform.Android )
			return;
		
		m_webObj = GameObject.Instantiate(l_webViewPrefab) as GameObject;
		m_webView = m_webObj.GetComponent<UniWebView>();
		if (m_webView == null)
			return;

		//m_webView.insets = new UniWebViewEdgeInsets((int)(90.0f * p_scale), 0, 0, 0);
		m_webView.insets = new UniWebViewEdgeInsets(0, 0, 0, 0);
		m_webView.SetUseWideViewPort(false);
		m_webView.OnReceivedKeyCode += _onBackKeyCode;
		m_webView.OnWebViewShouldClose 	+= _onShouldCloseView;
		m_webView.backButtonEnable = true;
	}
	
	private void _onBackKeyCode(UniWebView p_view, int p_keyCode)
	{
		const int kAndroidBackButtonCode = 4;
		if (p_keyCode != kAndroidBackButtonCode) 
			return;
		
		p_view.Hide();
		p_view.CleanCache();
		p_view.Load("about:blank");
		
		m_subState = SubState.GO_CONGRATS;
	}
	
	private bool _onShouldCloseView(UniWebView p_webView)
	{
		m_subState = SubState.GO_CONGRATS;
		return false;
	}

	private void _clickBack(UIButton p_button)
	{
		m_subState = SubState.GO_CONGRATS;
	}

	private GameObject	m_webObj;
	private UniWebView	m_webView;
	protected SubState m_subState = SubState.NONE;
	private int m_linkId = -1;
	protected int m_duration = 0;
}

public class VideoViewState : WebViewState
{
	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		if (m_subState != SubState.NONE)
		{
			switch (m_subState)
			{
			case SubState.GO_CONGRATS:
				p_gameController.connectState(ZoodleState.CONGRATS_STATE, ZoodleState.REGION_VIDEO);
				p_gameController.changeState(ZoodleState.CONGRATS_STATE);
				break;
			}
			
			m_subState = SubState.NONE;
		}
	}

	public override void exit(GameController p_gameController)
	{
		GAUtil.logVisit("Video", m_duration);

		base.exit(p_gameController);
	}
}

public class GameViewState : WebViewState
{
	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		if (m_subState != SubState.NONE)
		{
			switch (m_subState)
			{
			case SubState.GO_CONGRATS:
				p_gameController.connectState(ZoodleState.CONGRATS_STATE, ZoodleState.REGION_GAME);
				p_gameController.changeState(ZoodleState.CONGRATS_STATE);
				break;
			}
			
			m_subState = SubState.NONE;
		}
	}

	public override void exit(GameController p_gameController)
	{
		GAUtil.logVisit("Game", m_duration);

		base.exit(p_gameController);
	}
}

