using UnityEngine;
using System.Collections;

public class ShowSpinner : MonoBehaviour {


	public GameObject mSpinnerObj;

	private GameObject mSpinner;
	

	public void showSpinner() {
		Instantiate(mSpinnerObj);
	}

	public void dismissSpinner() {
		mSpinner = GameObject.FindWithTag("Spinner");
		if (mSpinner != null) {
			Destroy (mSpinner);
		} else {
		}
	}
}
