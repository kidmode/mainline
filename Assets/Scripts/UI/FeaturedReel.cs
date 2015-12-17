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

	private FeatureData featureData;

	private string featureButtonName = "FeatureThumb";

	// Use this for initialization
	void Start () {


	
	}

	void OnDisable(){



	}
	
	// Update is called once per frame
	void Update () {

			
	
	}

	public void OnVideoClosed(){

		content.SetActive(true);

	}


	//Start placing elemtns to the scroll view
	public void startReel(FeatureData featureData){

		content.SetActive(true);

		Game.OnVideoClosed += OnVideoClosed;

		this.featureData = featureData;

		for (int i = 0; i < featureData.ShowReelElementDatas.Length; i++) {

			ShowReelElementData element = featureData.ShowReelElementDatas[i];

			GameObject createdPrefab;

			if(element.isVideo){

				createdPrefab = GameObject.Instantiate(prefabVideoThumbnailItem) as GameObject;

//				createdPrefab.transform.parent = content.transform;

				createdPrefab.transform.SetParent( content.transform, false);

				createdPrefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

				Image icon = createdPrefab.GetComponentInChildren<Image>();

				icon.sprite = element.texture;

			}else{

				createdPrefab = GameObject.Instantiate(prefabImageItem) as GameObject;

//				createdPrefab.transform.parent = content.transform;

				createdPrefab.transform.SetParent( content.transform, false);

				createdPrefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

				Image icon = createdPrefab.GetComponentInChildren<Image>();
				
				icon.sprite = element.texture;

			}

			Button btnComp = createdPrefab.GetComponent<Button>();

			createdPrefab.name = featureButtonName + "_" + i.ToString();

			btnComp.onClick.AddListener ( () => featureReelCallBackTest(btnComp) ) ;

//			btnComp.onClick.AddListener(featureReelCallBack);

		}

		// Show featured App icons that are the same as the Game Acitivty Screen
		appIconImage.sprite = featureData.appIconTexture;
		
		appNameImage.sprite = featureData.appNameTexture;

	}
	
	public void featureReelCallBackTest(Button button)
	{
		
		Debug.Log("   featureReelCallBackTest " + button.gameObject.name.Substring(button.gameObject.name.Length - 1));

		int index = int.Parse( button.gameObject.name.Substring(button.gameObject.name.Length - 1)) ;

		if(featureData.ShowReelElementDatas[index].isVideo){


			#if UNITY_ANDROID && !UNITY_EDITOR
			
			AndroidJavaClass jc = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");
			
			jc.CallStatic("startPlayYoutube", featureData.ShowReelElementDatas[index].youtubeLink);

			content.SetActive(false);
			
			#endif

		}
		
	}



	//Clears all the child elements of the content Object
	public void removeElements(){

		Game.OnVideoClosed -= OnVideoClosed;

		LayoutElement[] layoutElements = content.GetComponentsInChildren<LayoutElement>();

		for (int i = 0; i < layoutElements.Length; i++) {

			GameObject.Destroy(layoutElements[i].gameObject);

		}

	}

}
