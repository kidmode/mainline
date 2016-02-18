

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;


public class KidModeScrollViewUpdatorVertical : MonoBehaviour {
	
	public ScrollRect scrollRect;
	
	public GameObject scrollArrowUp;
	
	public GameObject scrollArrowDown;
	
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
		
				Debug.Log("  scrolRectPos " + scrolRectPos);
		
		currPos = scrolRectPos;
		
		updateChanges (scrolRectPos);
		
	}
	
	void updateChanges(Vector2 scrolRectPos){
		
//		if (contentSize <= scrollStartContentSize) {
//			
//			scrollArrowLeft.SetActive (false);
//			
//			scrollArrowRight.SetActive (false);
//
//			//			return;
//			
//		}
		
		if (scrolRectPos.y > 0.0 + arrowShowThreshhold) {
			
			scrollArrowDown.SetActive(true);
			
		} else {
			
			scrollArrowDown.SetActive(false);
			
		}
		
		
		if (scrolRectPos.y < 1.0f - arrowShowThreshhold) {
			
			scrollArrowUp.SetActive(true);
			
		} else {
			
			scrollArrowUp.SetActive(false);
			
		}
		
	}
	
	
	
//	void OnDisable(){
//		
//		if (contentSize <= scrollStartContentSize) {
//			
//			scrollArrowLeft.SetActive (false);
//			
//			scrollArrowRight.SetActive (false);
//			
//		} else {
//			
//			scrollArrowLeft.SetActive (false);
//			
//			scrollArrowRight.SetActive (false);
//			
//		}
//		
//	}
//	
//	void OnEnable(){
//		
//		if (contentSize <= scrollStartContentSize) {
//			
//			scrollArrowLeft.SetActive (false);
//			
//			scrollArrowRight.SetActive (false);
//			
//		} else {
//			
//			//			Debug.Log ("  OnEnable ");
//			
//			scrollArrowLeft.SetActive (true);
//			
//			scrollArrowRight.SetActive (true);
//			
//		}
//		
//		updateChanges (currPos);
//		
//	}
	
	public void setContentDataSize(int size){
		
		contentSize = size;
		
	}
	
}
