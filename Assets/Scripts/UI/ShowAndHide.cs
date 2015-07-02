using UnityEngine;
using System.Collections;

public class ShowAndHide : MonoBehaviour {

	public RectTransform hideTrans;

	public RectTransform showTrans;

	// Use this for initialization
	void Start () {

//		show ();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void hide(){

		RectTransform currentRectTrans = gameObject.GetComponent<RectTransform>();

		currentRectTrans.localPosition = hideTrans.localPosition;//new Vector2 (-2000.0f, 0.0f);// = hideTrans;

	}

	public void show(){

		RectTransform currentRectTrans = gameObject.GetComponent<RectTransform>();
		
		currentRectTrans.localPosition = showTrans.localPosition;


	}
}
