using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PDSaveButton : MonoBehaviour {

	[SerializeField]
	private PDControlValueChanged pdControlValueChanged;

	private Button button;

	// Use this for initialization
	void Start () {

		button = gameObject.GetComponent<Button>();

		button.interactable = false;
	
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

		button.interactable = true;

	}

}
