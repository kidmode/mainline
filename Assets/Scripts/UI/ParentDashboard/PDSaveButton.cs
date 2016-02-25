using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PDSaveButton : MonoBehaviour {

	[SerializeField]
	private PDControlValueChanged pdControlValueChanged;

	private Button button;

	[SerializeField]
	private Text interactiveText;

	[SerializeField]
	private Text uninteractiveText;



	// Use this for initialization
	void Start () {

		button = gameObject.GetComponent<Button>();

		setButtonNotInteractive();
	
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

		setButtonInteractive();

	}

	void onUpdateComplete(){

		if(button!= null)
			setButtonNotInteractive();

	}


	void setButtonInteractive(){

		button.interactable = true;

		if(interactiveText != null)
			interactiveText.gameObject.SetActive(true);

		if(interactiveText != null)
			uninteractiveText.gameObject.SetActive(false);

	}

	void setButtonNotInteractive(){

		button.interactable = false;

		if(interactiveText != null)
			interactiveText.gameObject.SetActive(false);

		if(interactiveText != null)
			uninteractiveText.gameObject.SetActive(true);

	}

}
