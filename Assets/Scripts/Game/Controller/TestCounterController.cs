using UnityEngine;
using System.Collections;
using System;

public class TestCounterController : MonoBehaviour {

	private RequestQueue 	m_requestQueue;

	// Use this for initialization
	void Start () {
	
		m_requestQueue = new RequestQueue();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateKidCounters(){

//		SessionHandler.getInstance ().currentKid.videoWatchedCount = SessionHandler.getInstance ().currentKid.videoWatchedCount + 1;


		SessionHandler.getInstance ().currentKid.gamePlayedCount = SessionHandler.getInstance ().currentKid.gamePlayedCount + 1;

		ArrayList l_list = new ArrayList();
		foreach (Kid k in SessionHandler.getInstance ().kidList) {
			l_list.Add(k.toHashTable());
		}
		String encodedString = MiniJSON.MiniJSON.jsonEncode(l_list);
		SessionHandler.SaveKidList(encodedString);

//		Hashtable l_param = new Hashtable ();
//		l_param ["videos_watched_count"] 					= 1;// SessionHandler.getInstance ().currentKid.videoWatchedCount.ToString()	;
//		l_param ["games_played_count"] 					= 2;
//		l_param [ZoodlesConstants.PARAM_MAX_VIOLENCE] = 1;

		//l_param ["video_watched_count"] 
		//SessionHandler.getInstance ().currentKid.weightMath 				= Mathf.CeilToInt (m_mathSlider.value);
		
//		m_requestQueue.reset ();
//		m_requestQueue.add( new SetKidProfileCountersRequest( l_param ) );
//		m_requestQueue.request (RequestType.RUSH);

		m_requestQueue.reset ();
		m_requestQueue.add( new LinkVisitRequest( 23963 ) );
		m_requestQueue.request (RequestType.RUSH);

	}


}
