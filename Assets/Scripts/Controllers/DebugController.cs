using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

//=============================================================================
//Author: Kevin
//
//Date: NA
//Purpose: 
//Use this to remove Player prefs in Editor or write some other actions for easy testing
//A panel will show or hide. There are buttons for the actions
//
//=============================================================================
public class DebugController : MonoBehaviour {

	public Text txtStatus;

	//The panel to show the controls such as buttons and texts
	public GameObject debugControlsPanel;

	public static DebugController Instance;

	public bool showPanelOnAndroid = false;

	void Awake(){

		Instance = this;

	}

	// Use this for initialization
	void Start () {

		//debugControlsPanel

		if(!showPanelOnAndroid){

	#if UNITY_ANDROID && !UNITY_EDITOR

			if(debugControlsPanel != null){

				debugControlsPanel.SetActive(false);


			}

	#endif

		}


	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void clearLocalData(){

		PlayerPrefs.DeleteAll();

		File.Delete( Application.persistentDataPath + "/kidList.txt");
		
		File.Delete( Application.persistentDataPath + "/kidList_temp.txt");

		File.Delete( Application.persistentDataPath + "/kidList_backup.txt");

	}


	public void showStatus(string statusText){

		txtStatus.text = statusText;

	}

	public void loadVideoList(){

		ToyboxRandomizeController.Instance.getVideoList();

	}

	public void fogotPassword(){

		RequestQueue request = new RequestQueue();

		request.add(new GetForGotPasswordRequest("uncagedgaming@gmail.com", forgotComplete));

		request.request(RequestType.RUSH);

	}

	private void forgotComplete(HttpsWWW p_response)
	{

		showStatus(p_response.text);

		if(p_response.error == null){
			
		}
	}


	//=================== = = == = = == = = == = == = = == = == = = = = =
	public void getFeaturedContent(){
		
		RequestQueue request = new RequestQueue();
		
		request.add(new GetFeatureContentsRequest(getFeaturedContentComplete));
		
		request.request(RequestType.RUSH);
		
	}

	private void getFeaturedContentComplete(HttpsWWW p_response)
	{
		
		showStatus(p_response.text);
		
		if(p_response.error == null){
			
		}
	}

	//=================== = = == = = == = = == = == = = == = == = = = = =
	public void getLockPin(){
		
		RequestQueue request = new RequestQueue();
		
		request.add(new GetLockPinRequest(SessionHandler.getInstance().username, getLockPinCompelete));
		
		request.request(RequestType.RUSH);
		
	}
	
	private void getLockPinCompelete(HttpsWWW p_response)
	{
		
		showStatus(p_response.text);
		
		if(p_response.error == null){
			
		}
	}


}
