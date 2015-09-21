using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PointerCheck : MonoBehaviour {
	
	public InputField input;
	
//	public Text clickInfoText;
//
//	public Text clickInfoText1;

	public GameObject checkMoveObject;

	public GameObject checkMoveObjectFake;

	public TouchScreenKeyboard keyboard;

//	public Vector3 checkMoveObjectFocasLocalPos;
//
//	public Vector3 checkMoveObjectInitLocalPos;

//	public RectTransform localRect;
	
	// Use this for initialization
	void Start () {

		checkMoveObjectFake.SetActive(false);
		
//		clickInfoText.text = "no text";
		
	}
	
	// Update is called once per frame
	void Update () {

//		if(TouchScre
		if(TouchScreenKeyboard.visible){

//			clickInfoText1.text = "visible";

			checkMoveObjectFake.SetActive(true);
			
			checkMoveObject.SetActive(false);

		}else {

//			clickInfoText1.text = "NOTNOTNONTNO   ";

			checkMoveObjectFake.SetActive(false);
			
			checkMoveObject.SetActive(true);

		}
	}

	
//	public void OnPointerClick(PointerEventData data)
//	{
//		Debug.Log("Button OnPointerClick  "+data.pointerId+" up");
//
//		checkMoveObjectFake.SetActive(true);
//
//		checkMoveObject.SetActive(false);
//
////		checkMoveObjectInitLocalPos = checkMoveObject.transform.localPosition;
////
////		checkMoveObject.transform.localPosition = checkMoveObjectFocasLocalPos;
//		
//		clickInfoText.text = "Inputfiled OnPointerClick " + gameObject.name;
//	}
//
//	public void OnPointerExit(PointerEventData data)
//	{
//		Debug.Log("Button OnPointerExit  "+data.pointerId+" up");
//
//		return;
//
//		checkMoveObjectFake.SetActive(false);
//		
//		checkMoveObject.SetActive(true);
////
////		checkMoveObject.transform.localPosition = checkMoveObjectInitLocalPos;
////		
//		clickInfoText.text = "Inputfiled OnPointerExit " + gameObject.name;
//	}

	
	
}