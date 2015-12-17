using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

//=============================================================================
//Author: Kevin
//
//Date: Dec 2015
//Purpose: A screen script for the Game Activity screen. Different from Game Activity Canvas which is a GCS script
//Manages Featured App and how it shows the Apps Scroll view and how it is shown
//Uses ListSizeUpdator to update the app scroll view.
//The app scroll view should contract or expand depending if there is any Featured app for the current page
//Only 8 per page when there are features. Variable: itemCountPerPage
//
//
//=============================================================================

public class GameActivityScreen : MonoBehaviour {

	//Instance of this Screen. There can be only one at a time
	public static GameActivityScreen Instance;

	//Features Data that are to be loaded into the script. Fill this out by script when getting the Featured app data
	//From Server
	[SerializeField]
	private FeatureData[] features;
	//The Content object that contains all the elemnts ie games and apps
	public Transform contentTrans;
	//Scroll Size for the 
	public ScrollRect scrollRect;
	//This is the position to check which page we are on. currPageIndex
	public Transform scrollRectLeftPoint;
	// Set The number of items per page when there are features
	public int itemCountPerPage = 8;
	//The updator script to update the List Size to expand or contract
	private ListSizeUpdator gameListUpdator;

	//Script that moves the Featured icon Left when the list is expanding or right when the list is contracting
	[SerializeField]
	private UIMoveLeftRight featureSpaceMoveLeftRight;
	//The current Idex fo the page we are on. This calculated using the scrollRectLeftPoint and the elements
	private int currPageIndex = 0;

	//The app icon of the featured app
	public Image appIconImage;
	//The Image for the featured app name
	public Image appNameImage;

	//The same as "appIconImage" but is for the featured Reel
//	public Image featureClicked_AppIconImage;
//	
//	public Image featureClicked_AppNameImage;

	public GameObject featuredRealPanel;

	public FeaturedReel featuredReel;

	//Was here but not used any more. Keep it here just in case Zoodles wants it back again.
//	public Image featureClicked_SponsorLogoImage;

	void Awake(){

		Instance = this;

	}

	// Use this for initialization
	void Start () {

		scrollRect.onValueChanged.AddListener( onValueChanged ); 

		gameListUpdator = gameObject.GetComponent<ListSizeUpdator>();

		featuredRealPanel.SetActive(false);

		//For testing
//		featuredReel.startReel(features[0]);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setContentListData(){

		Button[] buttons = contentTrans.GetComponentsInChildren<Button>();

	}

	//When the scroll of the app and games have changed
	//Then check how we show features
	void onValueChanged(Vector2 scrolRectPos){

		checkFeature();
		
	}


	void checkFeature(){

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
	

	//When featured icon is clicked, Show the featured screen reel
	public void onFeatureIconClicked(){

		featuredRealPanel.gameObject.SetActive(true);

		featuredReel.startReel(features[currPageIndex]);

	}

	//When download button is clicked
	public void onDownloadButtonClicked(){

		featuredReel.removeElements();

		featuredRealPanel.gameObject.SetActive(false);

	}

	//Featured reel closed
	public void onFeatureReelClose(){
		
		featuredReel.removeElements();
		
		featuredRealPanel.gameObject.SetActive(false);
		
	}


}


//======================================================================================================================
//
// Feature Data 
//
//======================================================================================================================
[Serializable]
 public class FeatureData{

//	public string companyName;

	public Sprite appIconTexture;

	public Sprite appNameTexture;

	//Not being used right now
	public Sprite sponsorLogoTexture;

	//The features reel data in an array
	public ShowReelElementData[] ShowReelElementDatas;

}

[Serializable]
public class ShowReelElementData{

	public Sprite texture;

	public bool isVideo;

	public string youtubeLink;

	public string imageLink;

}



