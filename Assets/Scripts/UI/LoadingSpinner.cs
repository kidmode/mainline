using UnityEngine;
using System.Collections;

public class LoadingSpinner : MonoBehaviour {

	public GameObject mLoadingRing;
	private int mSpeed = -400;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		mLoadingRing.transform.Rotate(0, 0, (mSpeed * Time.deltaTime) % 360);
	}
}
