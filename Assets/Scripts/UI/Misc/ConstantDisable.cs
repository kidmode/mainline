using UnityEngine;
using System.Collections;

public class ConstantDisable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(gameObject.activeSelf){


			if (SessionHandler.getInstance().token.isPremium() == true)
			{
				gameObject.SetActive(false);
			}

		}
	
	}
}
