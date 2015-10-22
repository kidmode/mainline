using UnityEngine;
using System.Collections;

public class TutorialScreen : MonoBehaviour {

	public GameObject[] sequencePanels;

	[SerializeField]
	private int currIndex;

	// Use this for initialization
	void Start () {

		currIndex = 0;

		showPanel (currIndex);
	
	}
	
	// Update is called once per frame
	void Update () {

//		if(Input.GetMouseButtonDown(0)){
//
//			if(currIndex >= sequencePanels.Length - 1){
//
//				TutorialController.Instance.sequenceScreenFinished();
//
//				return;
//
//			}
//
//			currIndex++;
//
//			showPanel(currIndex);
//
//		}
	
	}

	public void showNextIndexPanel(){

		if(currIndex >= sequencePanels.Length - 1){

			if(TutorialController.Instance != null)
				TutorialController.Instance.sequenceScreenFinished();

			return;

		}

		currIndex++;

		showPanel(currIndex);

	}

	public void showPanel(int index){

		hideAll ();

		sequencePanels [index].SetActive (true);

	}



	void hideAll(){

		for (int i = 0; i < sequencePanels.Length; i++) {

			sequencePanels[i].SetActive(false);
				
		}

	}

	public void closeTutorialScreen(){

		if(TutorialController.Instance != null)
			TutorialController.Instance.sequenceScreenFinished();
		//SwrveComponent.Instance.SDK.NamedEvent("Tutorial.skip");
	}

}
