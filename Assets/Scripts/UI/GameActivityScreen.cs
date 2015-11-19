using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class GameActivityScreen : MonoBehaviour {

	public static GameActivityScreen Instance;

	void Awake(){

		Instance = this;

	}

	[SerializeField]
	private FeatureData[] features;

	public Transform contentTrans;
	

	// Use this for initialization
	void Start () {

//		scrollRect.content
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setContentListData(){

		Button[] buttons = contentTrans.GetComponentsInChildren<Button>();

		Debug.Log(" buttons " + buttons.Length);


	}

}


[Serializable]
 class FeatureData{

//	public string companyName;

	public Texture2D appIconTexture;

	public Texture2D appNameTexture;

	public Texture2D sponsorLogoTexture;

}
