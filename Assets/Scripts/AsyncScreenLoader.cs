using UnityEngine;
using System.Collections;

public delegate void RequestFinishedHandler(object obj);

public class AsyncScreenLoader : MonoBehaviour {

	static public AsyncScreenLoader instance;
	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadScreen(string resPath, RequestFinishedHandler handler) {
		DebugUtils.Assert(handler != null);
		StartCoroutine(_loadScreen(resPath, handler));
	}

	IEnumerator _loadScreen(string resPath, RequestFinishedHandler handler) {

		ResourceRequest rr = Resources.LoadAsync<GameObject>(resPath);

		if (!rr.isDone) {
			yield return new WaitForSeconds(0.5f);
		}

		yield return rr;

		handler(rr.asset);
	}
}
