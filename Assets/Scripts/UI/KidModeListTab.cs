using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class KidModeListTab : MonoBehaviour {

	public GameObject onText;

	public GameObject offText;

	public Toggle toggle;

	// Use this for initialization
	void Start () {

		toggle.onValueChanged.AddListener( onValueChanged ); 
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onValueChanged(bool changed){

		if (changed) {

			onText.SetActive(true);

			offText.SetActive(false);

		} else {

			onText.SetActive(false);

			offText.SetActive(true);

		}

	}


}
