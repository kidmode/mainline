using UnityEngine;
using System.Collections;

public class ActivityPanelScreenUpdator : MonoBehaviour {


	public Transform mainPanelLeftExtreme;

	public Transform mainPanelRightExtreme;


	public Transform leftPanelRightExtreme;

	public Transform rightPanelLeftExtreme;


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

//				RectTransformExtensions.SetSize();

//				gameObject.transform.localScale = gameObject.transform.localScale * scale;

//				gameObject.rec
//
//				state = State.OKAY;

//				Debug.Log(" we are overlapping " + mainPanelLeftExtreme.position.x + "   "  + 
//				          leftPanelRightExtreme.position.x + "   scale " + scale + "  GetSize " + RectTransformExtensions.GetSize(gameObject.GetComponent<RectTransform>()));

			}else{

//				Debug.Log(" we are okay " + mainPanelLeftExtreme.position.x);

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
