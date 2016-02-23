using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PDLockIcon : MonoBehaviour {
	
	[SerializeField]
	private PDControlValueChanged pdControlValueChanged;
	
	private Image iconImage;
	
	// Use this for initialization
	void Start () {
		
		iconImage = gameObject.GetComponent<Image>();

		iconImage.enabled = false;
//		Image.Equals/
		
	}
	
	void OnEnable(){
		
		pdControlValueChanged.onControlValueChangedTrue += onControlValueChangedTrue;
		
		PDUpdateComplete.onUpdateComplete += onUpdateComplete;
		
	}
	
	void OnDisable(){
		
		pdControlValueChanged.onControlValueChangedTrue -= onControlValueChangedTrue;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void onControlValueChangedTrue(){

		if( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() )
		{

			iconImage.enabled = false;

		}else{

			iconImage.enabled = true;

		}
		
	}
	
	void onUpdateComplete(){
		
		if(iconImage!= null)
			iconImage.enabled = false;
		
	}
	
}
