using UnityEngine;
using System.Collections;

public class StringManipulator : MonoBehaviour {

	public static int maxLength = 25; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public static string getShortenOneLineAppName(string appName){

		if(appName.Length > maxLength){

			string returnName = appName.Substring(0, maxLength - 3) + "...";

			return returnName;

		}else {

			return appName;

		}

	}
}
