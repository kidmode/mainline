using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;


public class TrialTimeController : MonoBehaviour {

	public static TrialTimeController Instance;

	public DateTime startTime; 

	[SerializeField]
	public int[] checkTrialStates;// Only when in these states, the trial message will be checked

	private RequestQueue m_queue = null;

	public Text debugTxt;

	private Game game;

	private UICanvas m_trialMessageCanvas;

	public enum CheckState{
		CHECKED,
		NOT_CHECKED
	}

	public CheckState checkState;

	void Awake(){

		Instance = this;

		checkState = CheckState.NOT_CHECKED;

		m_queue = new RequestQueue();

		game = GameObject.FindWithTag("GameController").GetComponent<Game>();

	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

//		Debug.Log(" game.gameController.stateName " + game.gameController.stateName);
	
	}


	#region ServerInfo
	public void getTrialTimeFromServer(){

		m_queue.reset();
		m_queue.add( new PremiumDetailsRequest( onGetDetailsComplete ) );
		m_queue.request( RequestType.SEQUENCE );

	}

	private void onGetDetailsComplete(WWW p_response)
	{
		if( null == p_response.error )
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			l_data = (l_data["jsonResponse"] as Hashtable)["response"] as Hashtable;
			
			int trialDaysLeft = (int)((double)l_data["trial_days"]);

			if(trialDaysLeft <= 0){
				
				createTrialEndMessage(game.gameController);

				checkState = CheckState.CHECKED;
				
			}
		}
	}

	#endregion

	public bool isTrialAccount(){

		if(!SessionHandler.getInstance().token.isPremium()){

			if(SessionHandler.getInstance().token.isTried() ){

				return true;

			}

		}

		return false;

	}

	public void setTrialTimeStart(){

		startTime = DateTime.Now;

	}

	public void checkTrialTimeDifference(){

		loadStartTimeLocal();

		DateTime currentTime = DateTime.Now;

		Debug.Log("  currentTime - startTime).TotalMilliseconds " + (currentTime - startTime).TotalMilliseconds);

		if(debugTxt != null)
			debugTxt.text = "  currentTime - startTime).TotalMilliseconds " + (currentTime - startTime).TotalMilliseconds;



		//SessionHandler

//		if ((currentTime - startTime).TotalMilliseconds < 30){
//
//		}

	}

	public void uiTestStartTrialMessage(){

		createTrialEndMessage(game.gameController);

	}

	public void checkToken(){

		Debug.Log("SessionHandler.getInstance().token.isPremium() " + SessionHandler.getInstance().token.isPremium());
		
		Debug.Log("SessionHandler.getInstance().token.isTried() " + SessionHandler.getInstance().token.isTried());
		
		Debug.Log("SessionHandler.getInstance().token.isCurrent() " + SessionHandler.getInstance().token.isCurrent());

	}


	public bool isTrialTimeExpired(){

		DateTime currentTime = DateTime.Now;

		if ((currentTime - startTime).TotalDays > 7 ){

			return true;

		}

		return false;

	}

	public void saveTrialStartTimeLocal(){

		PlayerPrefs.SetString(ZoodlesConstants.PLAYERPREF_PREMIMUMTRIAL_START_TIME, startTime.ToString());

	}

	public void loadStartTimeLocal(){

		string startTimeString = PlayerPrefs.GetString(ZoodlesConstants.PLAYERPREF_PREMIMUMTRIAL_START_TIME);

		startTime = DateTime.Parse(startTimeString);

		Debug.Log("  saved start time " + startTime.ToString());

		if(debugTxt != null)
			debugTxt.text = "  saved start time " + startTime.ToString();

	}


	private string focusTestTxt;
	public void OnApplicationFocus(bool p_focus)
	{

		if (p_focus)
		{

			Debug.Log("  p_focus " + p_focus);

			if(isTrialAccount()){

				if(debugTxt != null)
					debugTxt.text = " is Trial = true ";

			}

		}

		focusTestTxt = focusTestTxt + " focused ";

		debugTxt.text = focusTestTxt;

	}

	void OnApplicationPause(bool p_focus) {



	}


	public void androidExit(){
			
		focusTestTxt = focusTestTxt + " paused ";
		
		debugTxt.text = focusTestTxt;

		checkState = CheckState.NOT_CHECKED;

	}

	public void androidEnter(){

		if(checkState == CheckState.NOT_CHECKED){

			for (int i = 0; i < checkTrialStates.Length; i++) {

				if(checkTrialStates[i] == int.Parse(game.gameController.stateName)){

					checkTrialEnd(game.gameController);
					
					focusTestTxt = focusTestTxt + " focused ";
					
					debugTxt.text = focusTestTxt;


				}

			}


		}

	}

	public void changeStateCheck(){

		if(checkState == CheckState.NOT_CHECKED){
			
			for (int i = 0; i < checkTrialStates.Length; i++) {
				
				if(checkTrialStates[i] == int.Parse(game.gameController.stateName)){
					
					checkTrialEnd(game.gameController);
					
					focusTestTxt = focusTestTxt + " focused ";
					
					debugTxt.text = focusTestTxt;
					
					
				}
				
			}
			
			
		}

	}

	#region ControlsFromOtherComponents

	public void firstStartTrialTime(){

		setTrialTimeStart();

		saveTrialStartTimeLocal();

	}

	#endregion

	private void checkTrialEnd(GameController p_gameController){
		
		if(isTrialAccount()){

			if (Application.internetReachability == NetworkReachability.NotReachable 
			    || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected()){
				
				if(TrialTimeController.Instance.isTrialTimeExpired()){

					createTrialEndMessage(p_gameController);

					checkState = CheckState.CHECKED;

				}
				
			}else{
				//Seems like the tokens will be updated when the account is expried
//				getTrialTimeFromServer();

				if(!SessionHandler.getInstance().token.isCurrent()){

					createTrialEndMessage(p_gameController);
					
					checkState = CheckState.CHECKED;

				}

				
			}
			
		}
		
	}
	
	private void createTrialEndMessage(GameController p_gameController){
		
		UIManager l_ui = p_gameController.getUI();
		m_trialMessageCanvas = l_ui.createScreen(UIScreen.TRIAL_MESSAGE, false, 4);
		
		UIElement l_panel = m_trialMessageCanvas.getView( "mainPanel" );
		List<Vector3> l_pointListIn = new List<Vector3>();
		l_pointListIn.Add( l_panel.transform.localPosition );
		l_pointListIn.Add( l_panel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_panel.tweener.addPositionTrack( l_pointListIn, 0f );
		
		SessionHandler.getInstance().token.setCurrent(false);
		
		
		//===================
		//Set up screen
		UIButton m_subscriptionButton = m_trialMessageCanvas.getView ("subscriptionButton") 	as UIButton;
		UIButton m_continueButton	 = m_trialMessageCanvas.getView ("continueButton") 		as UIButton;
		UIButton m_exitButton		 = m_trialMessageCanvas.getView ("exitButton") 			as UIButton;
		UILabel m_messageText		 = m_trialMessageCanvas.getView ("messageText") 		as UILabel;
		UILabel m_continueText		 = m_trialMessageCanvas.getView ("continueText") 		as UILabel;
		
		m_subscriptionButton.addClickCallback ( onSubscriptionClick );
		m_continueButton.addClickCallback ( onContinueClick );
		m_exitButton.addClickCallback ( onContinueClick );
		
		m_messageText.text = Localization.getString (Localization.TXT_103_LABEL_EXPIRED);
		m_continueText.text = Localization.getString (Localization.TXT_103_BUTTON_NOTHANKS);
		
		
	}
	
	private void onSubscriptionClick(UIButton p_button)
	{
		if(string.Empty.Equals(SessionHandler.getInstance().PremiumJson))
		{
			Server.init (ZoodlesConstants.getHttpsHost());
			m_queue.reset ();
			m_queue.add (new GetPlanDetailsRequest(viewPremiumRequestComplete));
			m_queue.request ( RequestType.SEQUENCE );
		}
		else
		{
			game.gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(game.gameController.stateName) );
			game.gameController.changeState( ZoodleState.VIEW_PREMIUM );
			
			UIManager l_ui = game.gameController.getUI();
			
			l_ui.removeScreen(m_trialMessageCanvas);
		}
	}
	
	private void viewPremiumRequestComplete(WWW p_response)
	{
		Server.init (ZoodlesConstants.getHost());
		if(null == p_response.error)
		{
			SessionHandler.getInstance ().PremiumJson = p_response.text;
			game.gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(game.gameController.stateName) );
			game.gameController.changeState( ZoodleState.VIEW_PREMIUM );
			
			
			UIManager l_ui = game.gameController.getUI();
			
			l_ui.removeScreen(m_trialMessageCanvas);
		}
		else
		{
			setErrorMessage(game.gameController,Localization.getString(Localization.TXT_STATE_11_FAIL),Localization.getString(Localization.TXT_STATE_11_FAIL_DATA));
		}
	}
	
	
	private void onContinueClick(UIButton p_button)
	{
		
		UIManager l_ui = game.gameController.getUI();
		
		l_ui.removeScreen(m_trialMessageCanvas);
		
	}

	protected void setErrorMessage(GameController p_gameController, string p_errorName, string p_errorMessage)
	{
		int l_thisState = int.Parse(p_gameController.stateName);
		
		if(ZoodleState.CALL_SERVER == l_thisState)
		{
			l_thisState = SessionHandler.getInstance().invokeCallServerState;
		}
		
		p_gameController.connectState(ZoodleState.ERROR_STATE, l_thisState);
		
		SessionHandler l_handler = SessionHandler.getInstance();
		
		l_handler.errorName 	= p_errorName;
		l_handler.errorMessage 	= p_errorMessage;
		
		p_gameController.changeState(ZoodleState.ERROR_STATE);
	}
	
//	public void getTrialTimeFromServer(){
//		
//		m_trialPlanQueue.reset();
//		m_trialPlanQueue.add( new PremiumDetailsRequest( onGetDetailsComplete ) );
//		m_trialPlanQueue.request( RequestType.SEQUENCE );
//		
//	}
	
//	private void onGetDetailsComplete(WWW p_response)
//	{
//		if( null == p_response.error )
//		{
//			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
//			l_data = (l_data["jsonResponse"] as Hashtable)["response"] as Hashtable;
//			
//			int trialDaysLeft = (int)((double)l_data["trial_days"]);
//			
//			if(trialDaysLeft <= 0){
//				
//				createTrialEndMessage(m_gameController);
//				
//			}
//		}
//	}

}
