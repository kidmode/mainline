using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TutorialHighlight : MonoBehaviour {

	[SerializeField]
	private bool updateHighlightLocationalPanels; 

	public RectTransform transRight;

	public RectTransform transLeft;

	// Use this for initialization
	void Start () {
	
	}


	// Update is called once per frame
	void Update () {

//		Debug.Log("  TutorialHighLight " );

		if(updateHighlightLocationalPanels){

			updateHighlightLocationalPanels = false;

			checkPanels();
//

//			hightlightRect.
//
		}
	
	}


	void checkPanels(){

		checkLeftPanel();

		checkRightPanel();

	}

	void checkLeftPanel(){

		RectTransform hightlightRect = gameObject.GetComponent<RectTransform>();
		
		float hightlightRectWidth = hightlightRect.offsetMax.x - hightlightRect.offsetMin.x;
		
		float hightlightRectHeight = hightlightRect.offsetMax.y - hightlightRect.offsetMin.y;

		
		float transLeftWidth = transLeft.offsetMax.x - transLeft.offsetMin.x;
		
		float transLeftHeight = transLeft.offsetMax.y - transLeft.offsetMin.y;
		
		
		//Set the height of the right Trans
		//			transRight.offsetMax = new Vector2(transRight.offsetMax.x, hightlightRectHeight);
		transLeft.sizeDelta = new Vector2(transLeftWidth, hightlightRectHeight);
		
		//Set local pos so it will always be centered
		
		Vector3 localPos = transLeft.localPosition;
		
		localPos.x = -1.0f * (transLeftWidth/2 + hightlightRectWidth/2);
		
		localPos.y = 0.0f;
		
		localPos.z = 0.0f;
		
		transLeft.localPosition = localPos;

	}

	void checkRightPanel(){

		Debug.Log(" +++++++ + + + ++ + + + + + ++ + +   TutorialHighLight +++++ ++ + ++ + + + + + + + + + + + ++ + + + + + + + ++" );
		//

		
		RectTransform hightlightRect = gameObject.GetComponent<RectTransform>();
		
		Debug.Log("  hightlightRect pos " + hightlightRect.position );
		
		Debug.Log("  hightlightRect sizeDelta " + hightlightRect.sizeDelta );
		
		Debug.Log("  hightlightRect offsetMax " + hightlightRect.offsetMax );
		
		Debug.Log("  hightlightRect offsetMin " + hightlightRect.offsetMin );
		
		float hightlightRectWidth = hightlightRect.offsetMax.x - hightlightRect.offsetMin.x;
		
		float hightlightRectHeight = hightlightRect.offsetMax.y - hightlightRect.offsetMin.y;
		
		Debug.Log(" ______ _ __ _ _ ___ _ ____ __ _ _ _ _ "  );
		
		Debug.Log("  transRight pos " + transRight.position );
		
		Debug.Log("  transRight sizeDelta " + transRight.sizeDelta );
		
		Debug.Log("  transRight localPos " + transRight.localPosition );
		
		Debug.Log("  transRight offsetMax " + transRight.offsetMax );
		
		Debug.Log("  transRight offsetMin " + transRight.offsetMin );
		
		float transRightWidth = transRight.offsetMax.x - transRight.offsetMin.x;
		
		float transRightHeight = transRight.offsetMax.y - transRight.offsetMin.y;
		
		
		//Set the height of the right Trans
		//			transRight.offsetMax = new Vector2(transRight.offsetMax.x, hightlightRectHeight);
		transRight.sizeDelta = new Vector2(transRightWidth, hightlightRectHeight);
		
		//Set local pos so it will always be centered
		
		Vector3 localPos = transRight.localPosition;
		
		localPos.x = transRightWidth/2 + hightlightRectWidth/2;
		
		localPos.y = 0.0f;
		
		localPos.z = 0.0f;
		
		transRight.localPosition = localPos;

	}
}
