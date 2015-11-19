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

		if(state == State.STRETCH){

			if(leftArrowTrans.position.x > pointExpandLeft.position.x){

			Rect transRect = mainPanel.GetComponent<RectTransform>().rect;

			mainPanel.GetComponent<RectTransform>().offsetMin = new Vector2(
				mainPanel.GetComponent<RectTransform>().offsetMin.x - moveSpeed * Time.deltaTime, mainPanel.GetComponent<RectTransform>().offsetMin.y);

			}else{

				state = State.NONE;

			}

		}else if(state == State.CONTRACT){
			
			if(leftArrowTrans.position.x < pointContractLeft.position.x){
				
				Rect transRect = mainPanel.GetComponent<RectTransform>().rect;
				
				mainPanel.GetComponent<RectTransform>().offsetMin = new Vector2(
					mainPanel.GetComponent<RectTransform>().offsetMin.x + moveSpeed * Time.deltaTime, mainPanel.GetComponent<RectTransform>().offsetMin.y);
				
			}else{
				
				state = State.NONE;



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
