using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

public class SignInScreen : MonoBehaviour {

	public GameObject SignInCanvas;

//	public GameObject ForgotPasswordCanvas;

	public Text emailText;

	public InputField emailInput;

	private GameController m_gameController;

	public GameObject DialogEmailNotMatch;

	public Text DialogEmailNotMatchTitle;

	public Text DialogEmailNotMatchMessage;

	public GameObject DialogPasswordRecovered;

	public Text DialogPasswordRecoveredTitle;
	
	public Text DialogPasswordRecoveredMessage;

	// Use this for initialization
	void Start () {

		SignInCanvas.gameObject.SetActive(true);

//		ForgotPasswordCanvas.gameObject.SetActive(false);

		Game game = GameObject.FindWithTag("GameController").GetComponent<Game>();
		
		m_gameController = game.gameController;

		DialogEmailNotMatch.SetActive(false);

		DialogPasswordRecovered.SetActive(false);

		setupLocalization();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	private void setupLocalization(){

//		public Text DialogEmailNotMatchTitle;
//		
//		public Text DialogEmailNotMatchMessage;


//		
//		public Text DialogPasswordRecoveredTitle;
//		
//		public Text DialogPasswordRecoveredMessage;

		DialogEmailNotMatchTitle.text = Localization.getString(Localization.TXT_DIALOG_EMAIL_NOT_MATCH_TITLE);
		DialogEmailNotMatchMessage.text = Localization.getString(Localization.TXT_DIALOG_EMAIL_NOT_MATCH_MESSAGE);

		DialogPasswordRecoveredTitle.text = Localization.getString(Localization.TXT_DIALOG_PASSWORD_RECOVERED_TITLE);
		DialogPasswordRecoveredMessage.text = Localization.getString(Localization.TXT_DIALOG_PASSWORD_RECOVERED_MESSAGE);



//		public const string TXT_DIALOG_EMAIL_NOT_MATCH_TITLE	    						= "TXT_DIALOG_EMAIL_NOT_MATCH_TITLE";
//		public const string TXT_DIALOG_EMAIL_NOT_MATCH_MESSAGE	    						= "TXT_DIALOG_EMAIL_NOT_MATCH_MESSAGE";
//		
//		
//		public const string TXT_DIALOG_PASSWORD_RECOVERED_TITLE  						= "TXT_DIALOG_PASSWORD_RECOVERED_TITLE";
//		public const string TXT_DIALOG_PASSWORD_RECOVERED_MESSAGE	    						= "TXT_DIALOG_PASSWORD_RECOVERED_MESSAGE";

	}


	/// <UI interacitons>
	/// 	
	/// </interacitons>
	public void forgotpasswordClicked(){

		if (Application.internetReachability == NetworkReachability.NotReachable 
		    || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected())
		{

			m_gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);

			return;

		}

		bool emailHasPassed = emailPasses();

		if(emailHasPassed){

			//create loading screen

			RequestQueue request = new RequestQueue();

			string l_email = emailInput.text.Trim();
			
			request.add(new GetForGotPasswordRequest(l_email, forgotComplete));
			
			request.request(RequestType.RUSH);

			m_gameController.getUI().createScreen(UIScreen.LOADING_SPINNER_ELEPHANT,false, 7);

		}else{



			DialogEmailNotMatch.SetActive(true);

		}

//		SignInCanvas.gameObject.SetActive(false);
//		
//		ForgotPasswordCanvas.gameObject.SetActive(true);

	}

	public void sendPasswordClcked(){
		
		SignInCanvas.gameObject.SetActive(false);
		
//		ForgotPasswordCanvas.gameObject.SetActive(true);
		
	}

	public void closeDialogPasswordRecovered(){

		DialogPasswordRecovered.SetActive(false);

	}

	public void closeDialogEmailNotMatch(){

		DialogEmailNotMatch.SetActive(false);

	}

	//==			
	/// <summary>
	/// 
	/// </summary>
	/// <returns><c>true</c>, if passes was emailed, <c>false</c> otherwise.</returns>
	private bool emailPasses()
	{

		if(emailInput.text == "" || emailInput.text == " "){

			return false;

		}

		string l_email = emailInput.text.Trim();

		int posAt = l_email.IndexOf("@");

		string checkDotString = l_email.Substring(l_email.Length - 4, 4);

		Debug.Log(" posAt " + posAt +"    checkDotString " + checkDotString);

		return IsMatch(ZoodlesConstants.EMAIL_REGULAR_STRING,l_email.Trim());
	}

	private bool IsMatch(string p_pattern, string p_input)
	{
		if (p_input == null || p_input == "") return false;
		Regex regex = new Regex(p_pattern);
		return regex.IsMatch(p_input);
	}



	private void forgotComplete(HttpsWWW p_response)
	{
		
//		showStatus(p_response.text);
		
		if(p_response.error == null){

			m_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);

			DialogPasswordRecovered.SetActive(true);
			
		}else{

			m_gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 10);

		}
	}

}
