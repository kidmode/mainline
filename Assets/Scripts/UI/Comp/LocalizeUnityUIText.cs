using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class LocalizeUnityUIText : MonoBehaviour {

	public LocalizationProcess[] localizeProcesses;

	// Use this for initialization
	void Start () {

		updateLocalText();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void updateLocalText(){

		//
		for (int i = 0; i < localizeProcesses.Length; i++) {

			LocalizationProcess process = localizeProcesses[i];

			process.uText.text = Localization.getString(process.localizeTag);

		}

	}

}

[Serializable]
public class LocalizationProcess{

	public Text uText;

	public string localizeTag;

}
