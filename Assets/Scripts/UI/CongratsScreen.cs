using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CongratsScreen : MonoBehaviour {

//	public Image

	private GameObject gameLogic;

	[SerializeField]
	private GameObject zoodlePointRequestPrefab;



	//
	//=============================== 
	//Moved the animation from CongratState to screen sprcific
	private UIImage m_loadingBarImg;

	private int m_time;

	private UICanvas congratsScreen;

	private UICanvas congratsScreenBackGround;

	// Use this for initialization
	void Start () {

		gameLogic = GameObject.Find ("GameLogic");

		Game game = gameLogic.GetComponent<Game>();
//		
		UIManager uiManager = game.gameController.getUI();

		congratsScreen = uiManager.findScreen (UIScreen.CONGRATS);

		congratsScreenBackGround = uiManager.findScreen (UIScreen.CONGRATS_BACKGROUND);

		GameObject.Instantiate (zoodlePointRequestPrefab);

		Invoke ("startCongratsScreen", 0.6f);
//
//		m_loadingBarImg = congratsScreen.getView("loadingBarSprite") as UIImage;
	
	}

	void startCongratsScreen(){

//		return;
		congratsScreen.getView ("holder").gameObject.SetActive (true);
//
//		congratsScreen.gameObject.SetActive (true);
//
		congratsScreenBackGround.getView ("holder").gameObject.SetActive (true);
//		
//		congratsScreen.gameObject.SetActive (true);

	}
	
	// Update is called once per frame
	void Update () {

//		int p_time = (int)(Time.deltaTime * 1000.0f);
//
//		m_time += p_time;
//		if (m_time < 1250)
//		{
//			float l_fillAmount = Mathf.Lerp(0, 1.0f, m_time / 1250.0f);
//			m_loadingBarImg.fillAmount = l_fillAmount;
//		}
//		else
//			m_loadingBarImg.fillAmount = 1.0f;

//	
	}

	public void closeCongratsScreen(){

		return;

		Game game = gameLogic.GetComponent<Game>();

		UIManager l_ui = game.gameController.getUI();

		l_ui.removeScreen(UIScreen.CONGRATS_BACKGROUND);

		l_ui.removeScreen(UIScreen.CONGRATS);

	}


}
