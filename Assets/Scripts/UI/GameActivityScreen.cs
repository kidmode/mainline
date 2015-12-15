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

//	public Image sponsorLogoImage;

	public Image featureClicked_AppIconImage;
	
	public Image featureClicked_AppNameImage;

	//=============================================
	//Feature Reel
	public FeaturedReel featuredReel;

	public GameObject featuredRealPanel;
	
//	public Image featureClicked_SponsorLogoImage;

	void Awake(){

		Instance = this;

	}

	// Use this for initialization
	void Start () {

		scrollRect.onValueChanged.AddListener( onValueChanged ); 

		gameListUpdator = gameObject.GetComponent<ListSizeUpdator>();

//		featuredReel.gameObject.SetActive(true);

		featuredRealPanel.SetActive(false);

//		scrollRect.content

		//For testing
//		featuredReel.startReel(features[0]);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setContentListData(){

		Button[] buttons = contentTrans.GetComponentsInChildren<Button>();

//		Debug.Log(" buttons " + buttons.Length);


	}

	void onValueChanged(Vector2 scrolRectPos){
		
		//		Debug.Log("  scrolRectPos " + scrolRectPos);
		
//		currPos = scrolRectPos;
//		
//		updateChanges (scrolRectPos);


//		Debug.Log(" buttons " + buttons.Length);
		checkFeature();
		
	}


	void checkFeature(){

//		Debug.Log("  checkFeature ");

		Button[] buttons = contentTrans.GetComponentsInChildren<Button>();
		
		int changedPage = 0;

//		gameListUpdator.startStretch();
//
//		return;

		if(features.Length > 0){

			for (int i = 0; i < buttons.Length; i++) {
				
				ListItem item = buttons[i].GetComponent<ListItem>();
				
				if(i % 2 == 1){
					
					if(item.pointRightEnd.gameObject.transform.position.x < scrollRectLeftPoint.position.x){
						
//						Debug.Log(" current over Index  " + item.index);
						
						int currPage = (item.index + 1) / itemCountPerPage;
						
						changedPage = currPage;
						
						
						
						//					return;
						
					}else if(item.pointRightEnd.gameObject.transform.position.x > scrollRectLeftPoint.position.x){
						
//						Debug.Log(" current page Index " + currPageIndex);
						
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

		}else{

			gameListUpdator.startStretch();

			featureSpaceMoveLeftRight.setPosToLeft();

		}

	}


	void onContractDone(){

//		for (int i = 0; i < gameListUpdator.featureGroup.Length; i++) {
//			
//			gameListUpdator.featureGroup[i].SetActive(true);
//			
//		}


		appIconImage.sprite = features[currPageIndex].appIconTexture;

		appNameImage.sprite = features[currPageIndex].appNameTexture;
		
//		sponsorLogoImage.sprite = features[currPageIndex].sponsorLogoTexture;

	}


	void showFeaturedDetails(){

		featureClicked_AppIconImage.sprite = features[currPageIndex].appIconTexture;
		
		featureClicked_AppNameImage.sprite = features[currPageIndex].appNameTexture;


	}

	public void onFeatureIconClicked(){

//		featuredReel.gameObject.SetActive(tr
		featuredRealPanel.gameObject.SetActive(true);

		featuredReel.startReel(features[currPageIndex]);

	}

	public void onDownloadButtonClicked(){

		featuredReel.removeElements();

		featuredRealPanel.gameObject.SetActive(false);

	}

	public void onFeatureReelClose(){
		
		featuredReel.removeElements();
		
		featuredRealPanel.gameObject.SetActive(false);
		
	}


}


[Serializable]
 public class FeatureData{

//	public string companyName;

	public Sprite appIconTexture;

	public Sprite appNameTexture;

	//Not being used right now
	public Sprite sponsorLogoTexture;

	public ShowReelElementData[] ShowReelElementDatas;

}

[Serializable]
public class ShowReelElementData{

	public Sprite texture;

	public bool isVideo;

}



