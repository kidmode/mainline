using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KidModeScrollViewUpdator : MonoBehaviour {
	
	public ScrollRect scrollRect;

	public GameObject scrollArrowLeft;

	public GameObject scrollArrowRight;

	[SerializeField]
	private int scrollStartContentSize = 6;

	//Icons in the list. This will be set by RegionbaseSate or others
	[SerializeField]
	private int contentSize;

	//Current scroll position
	[SerializeField]
	private Vector2 currPos;

	// Use this for initialization
	void Start () {

		scrollRect.onValueChanged.AddListener( onValueChanged ); 

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void onValueChanged(Vector2 scrolRectPos){

		currPos = scrolRectPos;

		updateChanges (scrolRectPos);

	}

	void updateChanges(Vector2 scrolRectPos){

		if (contentSize <= scrollStartContentSize) {
			
			scrollArrowLeft.SetActive (false);
			
			scrollArrowRight.SetActive (false);
			
//			return;
			
		}
		
		if (scrolRectPos.x > 0.0f) {
			
			scrollArrowLeft.SetActive(true);
			
		} else {
			
			scrollArrowLeft.SetActive(false);
			
		}
		
		
		if (scrolRectPos.x < 1.0f) {
			
			scrollArrowRight.SetActive(true);
			
		} else {
			
			scrollArrowRight.SetActive(false);
			
		}

	}
	


	void OnDisable(){

		if (contentSize <= scrollStartContentSize) {

			scrollArrowLeft.SetActive (false);
			
			scrollArrowRight.SetActive (false);
			
		} else {

			scrollArrowLeft.SetActive (false);

			scrollArrowRight.SetActive (false);

		}

	}

	void OnEnable(){

		if (contentSize <= scrollStartContentSize) {

			scrollArrowLeft.SetActive (false);
			
			scrollArrowRight.SetActive (false);

		} else {

//			Debug.Log ("  OnEnable ");

			scrollArrowLeft.SetActive (true);

			scrollArrowRight.SetActive (true);

		}

		updateChanges (currPos);

	}

	public void setContentDataSize(int size){

		contentSize = size;

	}

}
