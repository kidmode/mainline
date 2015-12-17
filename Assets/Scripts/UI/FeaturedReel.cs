using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//=============================================================================
//Author: Kevin
//
//Manages the Screenshots and videos when the featured icon is clicked.
//
//
//=============================================================================

public class FeaturedReel : MonoBehaviour {

	//Prefab for the image Item
	public GameObject prefabImageItem;
	//Prefab for hte video Thumbnail item
	public GameObject prefabVideoThumbnailItem;
	//Content object that contains all the elements of the scroll view
	public GameObject content;

	//App Icon Image for the featured app
	public Image appIconImage;
	//App name Image for the featured app
	public Image appNameImage;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

			
	
	}

	//Start placing elemtns to the scroll view
	public void startReel(FeatureData featureData){

		for (int i = 0; i < featureData.ShowReelElementDatas.Length; i++) {

			ShowReelElementData element = featureData.ShowReelElementDatas[i];

			if(element.isVideo){

				GameObject createdPrefab = GameObject.Instantiate(prefabVideoThumbnailItem) as GameObject;

//				createdPrefab.transform.parent = content.transform;

				createdPrefab.transform.SetParent( content.transform, false);

				createdPrefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

				Image icon = createdPrefab.GetComponentInChildren<Image>();

				icon.sprite = element.texture;

			}else{

				GameObject createdPrefab = GameObject.Instantiate(prefabImageItem) as GameObject;

//				createdPrefab.transform.parent = content.transform;

				createdPrefab.transform.SetParent( content.transform, false);

				createdPrefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

				Image icon = createdPrefab.GetComponentInChildren<Image>();
				
				icon.sprite = element.texture;

			}

		}

		// Show featured App icons that are the same as the Game Acitivty Screen
		appIconImage.sprite = featureData.appIconTexture;
		
		appNameImage.sprite = featureData.appNameTexture;

	}

	//Clears all the child elements of the content Object
	public void removeElements(){

		LayoutElement[] layoutElements = content.GetComponentsInChildren<LayoutElement>();

		for (int i = 0; i < layoutElements.Length; i++) {

			GameObject.Destroy(layoutElements[i].gameObject);

		}

	}

}
