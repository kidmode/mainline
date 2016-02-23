using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class PDControlValueChanged : MonoBehaviour {

	[SerializeField]
	private Slider[] sliders;

	[SerializeField]
	private Toggle[] toggles;

	[SerializeField]
	private bool hasControlValueChanged = false;

	// Use this for initialization
	void Start () {

//		targetSlider.onValueChanged

//		toggleEvent.onValueChanged.AddListener(onValueChanged);



	}

	public void setListeners(){

		for (int i = 0; i < toggles.Length; i++) {
			
			Toggle toggle  = toggles[i];
			
			toggle.onValueChanged.AddListener(onToggleValueChanged);
			
		}
		
		
		for (int i = 0; i < sliders.Length; i++) {
			
			Slider slider  = sliders[i];
			
			slider.onValueChanged.AddListener(onSliderValueChanged);
			
		}


	}

	void OnDisable(){

		for (int i = 0; i < toggles.Length; i++) {
			
			Toggle toggle  = toggles[i];
			
			toggle.onValueChanged.RemoveListener(onToggleValueChanged);
			
		}

		for (int i = 0; i < sliders.Length; i++) {
			
			Slider slider  = sliders[i];
			
			slider.onValueChanged.RemoveListener(onSliderValueChanged);
			
		}


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region ControlValueChanged
	void onToggleValueChanged(bool value){

		hasControlValueChanged = true;

		if(onControlValueChangedTrue != null)
			onControlValueChangedTrue();

	}

	void onSliderValueChanged(float rect){
		
		hasControlValueChanged = true;

		if(onControlValueChangedTrue != null)
			onControlValueChangedTrue();
		
	}
	#endregion

	public event Action onControlValueChangedTrue;
}
