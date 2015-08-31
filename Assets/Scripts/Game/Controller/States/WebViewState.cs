using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class WebViewState : GameState
{
	protected enum SubState
	{
		NONE,
		No_Points,
		GO_CONGRATS
	}

	private RequestQueue 	m_requestQueue;

	string m_text = "";

	public override void enter(GameController p_gameController) {
		base.enter(p_gameController);
		SoundManager.getInstance().stopMusic();
		m_subState = SubState.NONE;
		string l_url = getURL();
		
		PointSystemController.Instance.setPointOK (PointSystemController.PointRewardState.No_Point);
		
		m_isLoaded = false;
		
		m_linkId = SessionHandler.getInstance().currentContent.id;
		m_duration = 0;


		m_requestQueue = new RequestQueue();

		m_requestQueue.reset ();
		m_requestQueue.add( new LinkVisitRequest( m_linkId ) );
		m_requestQueue.request (RequestType.RUSH);

		#if UNITY_ANDROID && !UNITY_EDITOR
		
		AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");
		
		jc.CallStatic("startPlayYoutube", l_url);

		#endif

		p_gameController.game.gameSwitcher (false);
	}

//	public override void enter(GameController p_gameController)
//	{
//		base.enter(p_gameController);
//
//		SoundManager.getInstance().stopMusic();
//
//		m_subState = SubState.NONE;
//
//		UICanvas l_screen = p_gameController.getUI().createScreen(UIScreen.WEBVIEW);
//		UIButton l_button = l_screen.getView("backButton") as UIButton;
//		l_button.addClickCallback(_clickBack);
//
//		m_inset = calculateInset(l_button);
//		TextAsset l_asset = Resources.Load( "Data/removeFullScreenButton" ) as TextAsset;
//		m_text = (l_asset).text;
//		string l_url = getURL();
//
////		Debug.Log ("                          enter       ===========================      l_url " + l_url);
//
//		PointSystemController.Instance.setPointOK (PointSystemController.PointRewardState.No_Point);
//
//		m_isLoaded = false;
//		_setupWebView("Prefabs/Web/YoutubeWebview", m_inset);
//		m_webView.OnLoadComplete += HandleOnLoadComplete;
//		m_webView.OnLoadBegin += HandleOnLoadBegin;
//		m_webView.OnReceivedKeyCode += HandelOnReceivedKeyCode;
//		m_webView.Load(l_url);
//		m_webView.Show();
//
//		m_linkId = SessionHandler.getInstance().currentContent.id;
//		m_duration = 0;
//
//		GAUtil.logScreen("WebViewScreen");
//	}

	void HandleOnLoadBegin (UniWebView webView, string loadingUrl)
	{
		string l_url = getURL();

		if (loadingUrl != l_url)
		{
			webView.Stop();
//			m_webView.Load(l_url);
		}
	}

	private string getURL()
	{
		string l_url = SessionHandler.getInstance().currentContent.url;

		int l_indexOfParams = l_url.LastIndexOf("&");
		if (l_indexOfParams != -1)
		{
			l_url = l_url.Substring(0, l_indexOfParams);
		}
		//Process youtube to add parameters
		if (l_url.Contains("youtube"))
		{
			if (l_url.Contains("?"))
			{
				l_url = l_url + "&fs=0&modestbranding=1&rel=0&showinfo=1&controls=1&cc_load_policy=1";
			}
			else
			{
				l_url = l_url + "?fs=0&modestbranding=1&rel=0&showinfo=1&controls=1&cc_load_policy=1";
			}
		}

		return l_url;
	}

	public void HandleOnLoadComplete (UniWebView webView, bool success, string errorMessage)
	{

		if (success) {

			PointSystemController.Instance.startPointSystemTimer();

		}

		m_webView.AddJavaScript (m_text);
		m_webView.EvaluatingJavaScript ("disableFullScreen()");
		m_isLoaded = true;
	}

	public static void HandleOnLoadComplete ()
	{
		
		PointSystemController.Instance.startPointSystemTimer();

		m_isLoaded = true;
	}



	public void HandelOnReceivedKeyCode(UniWebView webView, int keyCode){

		Debug.Log ("        HandelOnReceivedKeyCode   )000000000000000000  " + keyCode);


	}
	

	private float calculateInset(UIElement p_topBar)
	{
		RectTransform l_transform = (RectTransform) p_topBar.transform;		
		float l_scale = p_topBar.canvas.scaleFactor;
		float l_offset = Screen.height - l_transform.position.y;
		float l_height = l_transform.rect.height * l_transform.transform.localScale.y * l_scale;
		float l_inset = l_height + (2 * l_offset);
		return l_inset;
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
			{
				if (m_isLoaded)
				{
					KidMode.showWebViews();
				}
				else
				{
					m_webView.Reload();
				}
			}
		}
		return true;
	}

	private void _setupWebView(string p_webViewPrefab, float p_inset)
	{
		GameObject l_webViewPrefab = Resources.Load(p_webViewPrefab) as GameObject;
		if (l_webViewPrefab == null)// || Application.platform != RuntimePlatform.Android )
			return;
		
		m_webObj = GameObject.Instantiate(l_webViewPrefab) as GameObject;
		m_webView = m_webObj.GetComponent<UniWebView>();
		if (m_webView == null)
			return;

		//m_webView.insets = new UniWebViewEdgeInsets((int)(90.0f * p_scale), 0, 0, 0);
		m_webView.insets = new UniWebViewEdgeInsets(0, (int)p_inset, 0, 0);
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

		if (PointSystemController.Instance.pointSystemState () == PointSystemController.PointRewardState.OK
		    && !TimerController.Instance.timesUp) 
		{

			m_subState = SubState.GO_CONGRATS;

		} else {

			PointSystemController.Instance.stopPointSystemTimer();

			m_subState = SubState.No_Points;

		}
	}
	
	private bool _onShouldCloseView(UniWebView p_webView)
	{
		if (PointSystemController.Instance.pointSystemState () == PointSystemController.PointRewardState.OK) {
			
			m_subState = SubState.GO_CONGRATS;
			
		} else {
			
			PointSystemController.Instance.stopPointSystemTimer();
			
			m_subState = SubState.No_Points;
			
		}
		return false;
	}

	private void _clickBack(UIButton p_button)
	{
		if (PointSystemController.Instance.pointSystemState () == PointSystemController.PointRewardState.OK) {
			
			m_subState = SubState.GO_CONGRATS;
			
		} else {
			
			PointSystemController.Instance.stopPointSystemTimer();
			
			m_subState = SubState.No_Points;
			
		}
	}

	public static void _clickBackBtn()
	{
		if (PointSystemController.Instance.pointSystemState () == PointSystemController.PointRewardState.OK
		    && !TimerController.Instance.timesUp) 
		{
			
			m_subState = SubState.GO_CONGRATS;
			
		} else {
			
			PointSystemController.Instance.stopPointSystemTimer();
			
			m_subState = SubState.No_Points;
			
		}
	}

	private GameObject	m_webObj;
	protected UniWebView m_webView;
	protected static SubState m_subState = SubState.NONE;
	private static bool m_isLoaded;
	private int m_linkId = -1;
	protected int m_duration = 0;

	protected float m_inset;
}

public class VideoViewState : WebViewState
{
	public override void enter(GameController p_gameController)
	{

		base.enter(p_gameController);


	}
	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		if (m_subState != SubState.NONE)
		{
			switch (m_subState)
			{
			case SubState.GO_CONGRATS:
				// Don't show the point pop up
//				p_gameController.connectState(ZoodleState.CONGRATS_STATE, ZoodleState.REGION_VIDEO);
//				p_gameController.changeState(ZoodleState.CONGRATS_STATE);
				p_gameController.changeState(ZoodleState.REGION_VIDEO);
				SessionHandler.getInstance().getPoints();
				break;

			case SubState.No_Points:
				p_gameController.changeState(ZoodleState.REGION_VIDEO);
				break;

			}

			
			m_subState = SubState.NONE;
		}
	}

	public override void exit(GameController p_gameController)
	{
		GAUtil.logVisit("Video", m_duration);
		// add this later
//		int videotime = (int)Math.Ceiling(m_duration * 0.001);
//		Dictionary<string,string> payload = new Dictionary<string,string>() { {"Duration", videotime.ToString()}};
		SwrveComponent.Instance.SDK.NamedEvent("Video.end");

		SessionHandler.getInstance ().currentKid.videoWatchedCount = SessionHandler.getInstance ().currentKid.videoWatchedCount + 1;

		ArrayList l_list = new ArrayList();
		foreach (Kid k in SessionHandler.getInstance ().kidList) {
			l_list.Add(k.toHashTable());
		}
		String encodedString = MiniJSON.MiniJSON.jsonEncode(l_list);
		SessionHandler.SaveKidList(encodedString);


		base.exit(p_gameController);
	}
}

public class GameViewState : WebViewState
{
	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);
		
		Screen.autorotateToPortrait = true;
		Screen.autorotateToPortraitUpsideDown = true;
		Screen.orientation = ScreenOrientation.AutoRotation;

		m_webView.InsetsForScreenOreitation += setInsetsScreenOrientation;
	}

	UniWebViewEdgeInsets setInsetsScreenOrientation(UniWebView webView, UniWebViewOrientation orientation)
	{
		Debug.Log("orientation: " + orientation);
		if (orientation == UniWebViewOrientation.Portrait)
		{
			return new UniWebViewEdgeInsets((int)m_inset, 0, 0, 0);
		}
		else
		{
			return new UniWebViewEdgeInsets(0, (int)m_inset, 0, 0);
		}
	}

	IEnumerator WaitAndPrint(GameController p_gameController) {
		if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown) {
			Screen.orientation = ScreenOrientation.Landscape;
			yield return new WaitForSeconds (0.5F);
		} else
			yield return null;
		p_gameController.changeState(ZoodleState.REGION_GAME);
	}

	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		if (m_subState != SubState.NONE)
		{
			switch (m_subState)
			{
			case SubState.GO_CONGRATS:
//				p_gameController.connectState(ZoodleState.CONGRATS_STATE, ZoodleState.REGION_GAME);
//				p_gameController.changeState(ZoodleState.CONGRATS_STATE);
				p_gameController.game.StartCoroutine(WaitAndPrint(p_gameController));
				SessionHandler.getInstance().getPoints();
				break;
				
			case SubState.No_Points:
				p_gameController.game.StartCoroutine(WaitAndPrint(p_gameController));
				p_gameController.changeState(ZoodleState.REGION_GAME);
				break;
			}
			
			m_subState = SubState.NONE;
		}
	}

	public override void exit(GameController p_gameController)
	{
		GAUtil.logVisit("Game", m_duration);

		int gametime = (int)Math.Ceiling(m_duration * 0.001);
		Dictionary<string,string> payload = new Dictionary<string,string>() { {"Duration", gametime.ToString()}};
		if (gametime > 120)
		{
			SwrveComponent.Instance.SDK.NamedEvent("Game.DURATION.>2mins",payload);
		}
		else
		{
			SwrveComponent.Instance.SDK.NamedEvent("Game.DURATION.<2mins",payload);
		}

		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.orientation = ScreenOrientation.Landscape;
		Screen.orientation = ScreenOrientation.AutoRotation;



		SessionHandler.getInstance ().currentKid.gamePlayedCount = SessionHandler.getInstance ().currentKid.gamePlayedCount + 1;

		ArrayList l_list = new ArrayList();
		foreach (Kid k in SessionHandler.getInstance ().kidList) {
			l_list.Add(k.toHashTable());
		}
		String encodedString = MiniJSON.MiniJSON.jsonEncode(l_list);
		SessionHandler.SaveKidList(encodedString);



		base.exit(p_gameController);
	}
}

