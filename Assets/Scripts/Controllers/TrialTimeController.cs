using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class TrialTimeController : MonoBehaviour {

	public static TrialTimeController Instance;

	public DateTime startTime; 

	private RequestQueue m_queue = null;

	public Text debugTxt;

	void Awake(){

		Instance = this;

	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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

		if(!p_focus){

			focusTestTxt = focusTestTxt + " paused ";

			debugTxt.text = focusTestTxt;

		}

	}

	#region ControlsFromOtherComponents

	public void firstStartTrialTime(){

		setTrialTimeStart();

		saveTrialStartTimeLocal();

	}

	#endregion

}
