using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class FeaturedReel : MonoBehaviour {

	public GameObject prefabImageItem;

	public GameObject prefabVideoThumbnailItem;

	public GameObject content;

	public Image appIconImage;
	
	public Image appNameImage;

	//Private
//	private RectTransform featurelRect;

	// Use this for initialization
	void Start () {

//		featurelRect = gameObject.GetComponent<RectTransform>();
//
//		Debug.LogError("  featurelRect " + featurelRect.position);
//
//		Debug.LogError("  featurelRect offsetMax offsetMin " + featurelRect.offsetMax + "    " + featurelRect.offsetMin);
//
////		featurelRect.offsetMin
//
////		featurelRect.sizeDelta = new Vector2(200.0f, 200.0f);	
//
//		Debug.LogError("  featurelRect after width " + featurelRect.position);
//
//		Debug.LogError("  featurelRect offsetMax offsetMin " + featurelRect.offsetMax + "    " + featurelRect.offsetMin);
	
	}
	
	// Update is called once per frame
	void Update () {

			
	
	}

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

	public void removeElements(){

		LayoutElement[] layoutElements = content.GetComponentsInChildren<LayoutElement>();

		for (int i = 0; i < layoutElements.Length; i++) {

			GameObject.Destroy(layoutElements[i].gameObject);

		}

	}

}
