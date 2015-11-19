using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class GameActivityScreen : MonoBehaviour {

	public static GameActivityScreen Instance;

	[SerializeField]
	private FeatureData[] features;
	
	public Transform contentTrans;
	
	public ScrollRect scrollRect;

	public Transform scrollRectLeftPoint;

	void Awake(){

		Instance = this;

	}

	// Use this for initialization
	void Start () {

		scrollRect.onValueChanged.AddListener( onValueChanged ); 

//		scrollRect.content
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setContentListData(){

		Button[] buttons = contentTrans.GetComponentsInChildren<Button>();

		Debug.Log(" buttons " + buttons.Length);


	}

	void onValueChanged(Vector2 scrolRectPos){
		
		//		Debug.Log("  scrolRectPos " + scrolRectPos);
		
//		currPos = scrolRectPos;
//		
//		updateChanges (scrolRectPos);

		Button[] buttons = contentTrans.GetComponentsInChildren<Button>();

		for (int i = 0; i < buttons.Length; i++) {

			ListItem item = buttons[i].GetComponent<ListItem>();

		}
//		Debug.Log(" buttons " + buttons.Length);
		
	}

}


[Serializable]
 class FeatureData{

//	public string companyName;

	public Texture2D appIconTexture;

	public Texture2D appNameTexture;

	public Texture2D sponsorLogoTexture;

}
