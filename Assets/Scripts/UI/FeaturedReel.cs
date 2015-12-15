using UnityEngine;
using System.Collections;

public class FeaturedReel : MonoBehaviour {

	public GameObject prefabImageItem;

	public GameObject prefabVideoThumbnailItem;

	public GameObject content;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void startReel(FeatureData featureData){

		for (int i = 0; i < featureData.ShowReelElementDatas.Length; i++) {

			ShowReelElementData element = featureData.ShowReelElementDatas[i];

			if(element.isVideo){

				GameObject createdPrefab = GameObject.Instantiate(prefabVideoThumbnailItem) as GameObject;

				createdPrefab.transform.parent = content.transform;

				createdPrefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

			}else{

				GameObject createdPrefab = GameObject.Instantiate(prefabImageItem) as GameObject;

				createdPrefab.transform.parent = content.transform;

				createdPrefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

			}

		}

	}


}
