using UnityEngine;
using System.Collections;

//=============================================================================
//Author: Kevin
//
//Date: NA
//Purpose: The activity buttons in Jungle needs to srink or expand in size when the screen is narrow or wide
//
//=============================================================================

public class ActivityPanelScreenUpdator : MonoBehaviour {

	//Variables for the acitvity panel
	public Transform mainPanelLeftExtreme;

	public Transform mainPanelRightExtreme;


	public Transform leftPanelRightExtreme;

	public Transform rightPanelLeftExtreme;

	//The range for it to be in position
	public float checkRange = 12.0f;

	public enum State{

		CHECK,
		OKAY

	}

	public State state;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(state == State.CHECK){

			if(mainPanelRightExtreme.position.x > rightPanelLeftExtreme.position.x){

				float rightScale = getRightScale();

				RectTransform rectTrans = gameObject.GetComponent<RectTransform>();
				
				Vector3 newScale = rectTrans.localScale * rightScale;
				
				gameObject.GetComponent<RectTransform>().localScale = newScale;

			}else if(mainPanelLeftExtreme.position.x < leftPanelRightExtreme.position.x ){
				
				float leftScale = getLeftScale();
				
				RectTransform rectTrans = gameObject.GetComponent<RectTransform>();
				
				Vector3 newScale = rectTrans.localScale * leftScale;
				
				gameObject.GetComponent<RectTransform>().localScale = newScale;
				
			}else if(mainPanelRightExtreme.position.x < rightPanelLeftExtreme.position.x - checkRange){ //=================== if too small for the right side
				
				float rightScale = getRightScale();
				
				RectTransform rectTrans = gameObject.GetComponent<RectTransform>();
				
				Vector3 newScale = rectTrans.localScale * rightScale;
				
				gameObject.GetComponent<RectTransform>().localScale = newScale;
				
			}else if(mainPanelLeftExtreme.position.x < leftPanelRightExtreme.position.x &&  mainPanelRightExtreme.position.x > rightPanelLeftExtreme.position.x){

				float scale;

				float leftScale = getLeftScale();

				float rightScale = getRightScale();

				if(leftScale < rightScale){

					scale = leftScale;

				}else{

					scale = rightScale;

				}



				RectTransform rectTrans = gameObject.GetComponent<RectTransform>();

				Vector3 newScale = rectTrans.localScale * scale;

				gameObject.GetComponent<RectTransform>().localScale = newScale;

			}else{

			}

		}
	
	}


	private float getLeftScale(){

		float leftXToMainPanel = Mathf.Abs(mainPanelLeftExtreme.position.x - gameObject.transform.position.x);
		
		float leftPanelRightXToMainPanel = Mathf.Abs(leftPanelRightExtreme.position.x - gameObject.transform.position.x);
		
		float leftScale = leftPanelRightXToMainPanel / leftXToMainPanel;

		return leftScale;

	}

	private float getRightScale(){
		
		float rightXToMainPanel = Mathf.Abs(mainPanelRightExtreme.position.x - gameObject.transform.position.x);
		
		float rightPanelLeftToMainPanel = Mathf.Abs(rightPanelLeftExtreme.position.x - gameObject.transform.position.x);
		
		float rightScale = rightPanelLeftToMainPanel / rightXToMainPanel ;

		return rightScale;
		
	}
}
