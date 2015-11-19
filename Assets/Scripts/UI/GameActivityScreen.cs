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

	public int itemCountPerPage = 8;

	private ListSizeUpdator gameListUpdator;

	[SerializeField]
	private UIMoveLeftRight featureSpaceMoveLeftRight;



	private int currPageIndex = 0;

	public Image appIconImage;

	public Image appNameImage;

	public Image sponsorLogoImage;

	void Awake(){

		Instance = this;

	}

	// Use this for initialization
	void Start () {

		scrollRect.onValueChanged.AddListener( onValueChanged ); 

		gameListUpdator = gameObject.GetComponent<ListSizeUpdator>();

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

		int changedPage = 0;

		for (int i = 0; i < buttons.Length; i++) {

			ListItem item = buttons[i].GetComponent<ListItem>();

			if(i % 2 == 1){

				if(item.pointRightEnd.gameObject.transform.position.x < scrollRectLeftPoint.position.x){

					Debug.Log(" current over Index  " + item.index);

					int currPage = (item.index + 1) / itemCountPerPage;

					changedPage = currPage;



//					return;

				}else if(item.pointRightEnd.gameObject.transform.position.x > scrollRectLeftPoint.position.x){

					Debug.Log(" current page Index " + currPageIndex);

					if(currPageIndex != changedPage){

						currPageIndex = changedPage;
					
						if(currPageIndex > features.Length - 1){
							
							gameListUpdator.startStretch();

							featureSpaceMoveLeftRight.startMoveLeft();
							
						}else{
							
							gameListUpdator.startContract();

							featureSpaceMoveLeftRight.startMoveRight();
							
						}

					}

					return;

				}

			}

		}
//		Debug.Log(" buttons " + buttons.Length);
		
	}


	void onContractDone(){

//		for (int i = 0; i < gameListUpdator.featureGroup.Length; i++) {
//			
//			gameListUpdator.featureGroup[i].SetActive(true);
//			
//		}


		appIconImage.sprite = features[currPageIndex].appIconTexture;

		appNameImage.sprite = features[currPageIndex].appNameTexture;
		
		sponsorLogoImage.sprite = features[currPageIndex].sponsorLogoTexture;

	}

}


[Serializable]
 class FeatureData{

//	public string companyName;

	public Sprite appIconTexture;

	public Sprite appNameTexture;

	public Sprite sponsorLogoTexture;

}
