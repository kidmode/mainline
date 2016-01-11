using UnityEngine;
using System.Collections;

public class TestPinSetController : MonoBehaviour {

	public int newPin = 6666;

	// Use this for initialization
	void Start () {
	
		Invoke("startSetPin", 3.0f);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void startSetPin(){

		RequestQueue l_request = new RequestQueue ();
		l_request.add ( new SetPinRequest( newPin,  setPinRequestComplete) );
		l_request.request (RequestType.RUSH);

	}

	private void setPinRequestComplete(HttpsWWW p_response)
	{

		if(p_response.error != null){

		}

	}


}
