using UnityEngine;
using System;
using System.Collections;


public class PDUpdateComplete : MonoBehaviour {

	public static event Action onUpdateComplete;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void updateComplete(){

		if(onUpdateComplete != null)
			onUpdateComplete();

	}

}
