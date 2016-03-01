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

	[SerializeField]
	private string currStateName;

	private Game game;

	void Awake(){

		Instance = this;

		//make sure the menu bar is visible whenever we enter the state
		game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();

		currStateName = game.gameController.stateName;

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

		currStateName = game.gameController.state.ToString();
	
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
}
