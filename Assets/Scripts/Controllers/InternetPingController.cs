using UnityEngine;
using System;
using System.Collections;

public class InternetPingController : MonoBehaviour {

	public static InternetPingController Instance;

	public string checkURL;

	public enum StateURL{
		CHECKING,
		NOTREACHABLE,
		OKAY

	}

	public StateURL stateURL;

	public StateURL prevState;

	//=======+===========================================
	//Events
	//=================================================
	public static event Action OnInternetConnectionOkay;

	void Awake(){

		prevState = StateURL.OKAY;

		Instance = this;

	}

	// Use this for initialization
	void Start () {

//		checkURLState();

		InvokeRepeating("checkURLState", 0.0f, 2.0f);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void checkURLState(){

		StartCoroutine( checkURLReachability(checkURL));

	}

	private IEnumerator checkURLReachability(string l_url)
	{

		//used to now if we have just got internet connection
		StateURL tempPrevState = prevState;

		if(stateURL != StateURL.CHECKING)
			prevState = stateURL;

		stateURL = StateURL.CHECKING;

		RequestQueue l_queue = new RequestQueue();
		WWW l_www = new WWW (l_url);
		yield return l_www;

//		Debug.LogError(" l_www " + l_www.error);

		if(l_www.error == null){



			if(tempPrevState == StateURL.NOTREACHABLE && stateURL != StateURL.OKAY){

				if(OnInternetConnectionOkay != null){

					OnInternetConnectionOkay();

				}

			}

			stateURL = StateURL.OKAY;

		}else{

			stateURL = StateURL.NOTREACHABLE;

		}

	}
}
