using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ListSizeUpdator : MonoBehaviour {

	[SerializeField]
	private GameObject mainPanel;
	[SerializeField]
	private Transform pointExpandLeft;
	[SerializeField]
	private Transform pointContractLeft;
	[SerializeField]
	private Transform leftArrowTrans;
	[SerializeField]
	private float moveSpeed = 3.0f;
	[SerializeField]
	public GameObject[] featureGroup;

	[SerializeField]
	private float contractRectLeft = 104.74f;

	[SerializeField]
	private float expandRectLeft = -80.0f;

	//-80

	public enum State{
		NONE,
		STRETCH,
		CONTRACT
	}
	public State state;


	// Use this for initialization
	void Start () {

		Image pointExpandLeftImage = pointExpandLeft.gameObject.GetComponent<Image>();

		pointExpandLeftImage.enabled = false;

		Image pointContractLeftImage = pointContractLeft.gameObject.GetComponent<Image>();

		pointContractLeftImage.enabled = false;
	
	}
	
	// Update is called once per frame
	void Update () {

//		Debug.Log("   mainPanel.GetComponent<RectTransform>().offsetMin " + mainPanel.GetComponent<RectTransform>().offsetMin.x);

		if(state == State.STRETCH){

			Rect transRect = mainPanel.GetComponent<RectTransform>().rect;

//			if(leftArrowTrans.position.x > pointExpandLeft.position.x){

			if(mainPanel.GetComponent<RectTransform>().offsetMin.x > expandRectLeft){



				mainPanel.GetComponent<RectTransform>().offsetMin = new Vector2(
					mainPanel.GetComponent<RectTransform>().offsetMin.x - moveSpeed * Time.deltaTime, mainPanel.GetComponent<RectTransform>().offsetMin.y);

			}else{

				mainPanel.GetComponent<RectTransform>().offsetMin = new Vector2(
					expandRectLeft, mainPanel.GetComponent<RectTransform>().offsetMin.y);


				state = State.NONE;

			}

		}else if(state == State.CONTRACT){

			Rect transRect = mainPanel.GetComponent<RectTransform>().rect;

//			if(leftArrowTrans.position.x < pointContractLeft.position.x){
			if(mainPanel.GetComponent<RectTransform>().offsetMin.x <  contractRectLeft){
				

				
				mainPanel.GetComponent<RectTransform>().offsetMin = new Vector2(
					mainPanel.GetComponent<RectTransform>().offsetMin.x + moveSpeed * Time.deltaTime, mainPanel.GetComponent<RectTransform>().offsetMin.y);
				
			}else{
				
				state = State.NONE;

				mainPanel.GetComponent<RectTransform>().offsetMin = new Vector2(
					contractRectLeft, mainPanel.GetComponent<RectTransform>().offsetMin.y);

				
				gameObject.SendMessage("onContractDone");
				
			}
			
		}
		//gameObject.GetComponent<RectTransform>().localScale = newScale;
	
	}


	public void startStretch(){

//		for (int i = 0; i < featureGroup.Length; i++) {
//
//			featureGroup[i].SetActive(false);
//
//		}

		state = State.STRETCH;

	}

	public void startContract(){



		state = State.CONTRACT;

	}


}
