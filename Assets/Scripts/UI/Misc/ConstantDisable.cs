using UnityEngine;
using System.Collections;

public class ConstantDisable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(gameObject.activeSelf){

			gameObject.SetActive(false);

		}
	
	}
}
