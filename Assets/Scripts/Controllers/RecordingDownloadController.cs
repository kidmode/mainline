using UnityEngine;
using System.Collections;

public class RecordingDownloadController : MonoBehaviour {

	void Awake(){

		InternetPingController.OnInternetConnectionOkay += OnInternetConnectionOkay;

	}

	void OnDisable(){

		InternetPingController.OnInternetConnectionOkay -= OnInternetConnectionOkay;

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnInternetConnectionOkay(){

		Debug.Log("   sdfjasdf asdfsd f  dsf a OnInternetConnectionOkay " );
		     

	}


}
