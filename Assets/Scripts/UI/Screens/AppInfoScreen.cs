using UnityEngine;
using System.Collections;

public class AppInfoScreen : MonoBehaviour {

	[SerializeField]
	private ScreenController leftPartControl;

	private RequestQueue m_queue = null;

	private Game game;

	// Use this for initialization
	void Start () {

		game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();

		game.setPDMenuBarVisible(true, false);

		m_queue = new RequestQueue();

//		showPlanDetails();

		if( SessionHandler.getInstance().token.isPremium() ){

			leftPartControl.showScreenAtIndex(0);

		}else{

			leftPartControl.showScreenAtIndex(1);

		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region ButtonEvents
	public void contactUsButtonClicked(){

		Debug.Log(" app contactUsButtonClicked clicked " ); 

	}

	public void buygGemsButtonClicked(){
		game.gameController.connectState( ZoodleState.BUY_GEMS, ZoodleState.SETTING_STATE );
		
		game.gameController.changeState( ZoodleState.BUY_GEMS );
		
		game.setPDMenuBarVisible(false, false);
		
	}


	public void subscribeButtonClicked(){
		
		game.gameController.connectState( ZoodleState.VIEW_PREMIUM, ZoodleState.SETTING_STATE );

		game.gameController.changeState( ZoodleState.VIEW_PREMIUM );

		game.setPDMenuBarVisible(false, false);
		
	}
	#endregion

	#region ServerInfo
	private void getPlanDetails(){
		
		m_queue.reset();
		m_queue.add( new PremiumDetailsRequest( onGetDetailsComplete ) );
		m_queue.request( RequestType.SEQUENCE );
		
	}

	private void onGetDetailsComplete(HttpsWWW p_response)
	{
		if( null == p_response.error )
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			l_data = (l_data["jsonResponse"] as Hashtable)["response"] as Hashtable;
			
			int trialDaysLeft = (int)((double)l_data["trial_days"]);

			Debug.LogWarning(" = = = = = = = == = = = = = == = = = ");

			foreach(DictionaryEntry entry in l_data){

				Debug.LogWarning("  KEY: " + entry.Key + " Value: " + entry.Value);

			}
			

		}
	}


	//ShowPlanDetailsRequest

	private void showPlanDetails(){
		
		m_queue.reset();
		m_queue.add( new ShowPlanDetailsRequest( onShowPlanDetailsComplete ) );
		m_queue.request( RequestType.SEQUENCE );
		
	}
	
	private void onShowPlanDetailsComplete(HttpsWWW p_response)
	{
		if( null == p_response.error )
		{

			Debug.LogWarning(" = = = = = = = == = = = = = == = = = " + p_response.text);
			
		}
	}
	#endregion

}
