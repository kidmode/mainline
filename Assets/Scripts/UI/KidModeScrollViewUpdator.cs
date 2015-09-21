using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;


public class KidModeScrollViewUpdator : MonoBehaviour {
	
	public ScrollRect scrollRect;

	public GameObject scrollArrowLeft;

	public GameObject scrollArrowRight;

	public float arrowShowThreshhold = 0.0003f;

	[SerializeField]
	private int scrollStartContentSize = 6;

	//Icons in the list. This will be set by RegionbaseSate or others
	[SerializeField]
	private int contentSize;

	//Current scroll position
	[SerializeField]
	private Vector2 currPos;


	public static event Action<Vector2> OnScrollViewValueChanged;

	// Use this for initialization
	void Start () {

		scrollRect.onValueChanged.AddListener( onValueChanged ); 

//		getAppButtonList();

	}
	
	// Update is called once per frame
	void Update () {
	
	}



	#region HidingNonOnScreenApps

	public Button[] buttons;

	public void getAppButtonList(){

		buttons = gameObject.GetComponentsInChildren<Button>();

	}



	#endregion



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
		
		if (scrolRectPos.x > 0.0 + arrowShowThreshhold) {
			
			scrollArrowLeft.SetActive(true);
			
		} else {
			
			scrollArrowLeft.SetActive(false);
			
		}
		
		
		if (scrolRectPos.x < 1.0f - arrowShowThreshhold) {
			
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
