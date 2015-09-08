using UnityEngine;
using System.Collections;

public class LoadingIconAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		elapsedTime += Time.deltaTime;
		float scale = 1.5f + Mathf.Sin(elapsedTime) * 0.5f;
		Vector3 vec = new Vector3(scale, scale, scale);
		transform.localScale = vec;
	}

	private float elapsedTime = 0.0f;
}
