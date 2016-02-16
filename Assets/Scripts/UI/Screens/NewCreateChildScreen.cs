using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NewCreateChildScreen : MonoBehaviour {

	public GameObject backgroundImageObject;

	// Use this for initialization
	void Start () {

		int newChildInKidMode = PlayerPrefs.GetInt("newChildInKidMode");

		if(newChildInKidMode == 1){

			backgroundImageObject.SetActive(false);

		}
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
