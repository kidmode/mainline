﻿using UnityEngine;
using System.Collections;

public class TutClick2NextPage : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetMouseButtonDown(0)){

			if(TutorialController.Instance != null)
				TutorialController.Instance.showNextPage();

		}
		
	}
}
