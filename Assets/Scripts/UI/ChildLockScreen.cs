using UnityEngine;
using System.Collections;

public class ChildLockScreen : MonoBehaviour {

	public GameObject dialogChildLockChangeOK;

	public GameObject dialogChildLockChangeError;

	public GameObject dialogChildLockChangeHelp;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void closeDialogChildLockChangeOK(){

		dialogChildLockChangeOK.gameObject.SetActive(false);

	}

	public void closeDialogChildLockChangeError(){

		dialogChildLockChangeError.gameObject.SetActive(false);
		
	}

	public void closeDialogChildLockChangeHelp(){

		dialogChildLockChangeHelp.gameObject.SetActive(false);
		
	}

}
