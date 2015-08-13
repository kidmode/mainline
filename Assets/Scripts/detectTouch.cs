using UnityEngine;
using System.Collections;

public class detectTouch : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	private const int COUNT_DOWN = 200;
	private int touchCountDown = COUNT_DOWN;
	void FixedUpdate() {
		if (Input.touchCount > 0) {
			this.touchCountDown = COUNT_DOWN;
			#if UNITY_ANDROID && !UNITY_EDITOR
			Application.targetFrameRate = 60;
			#endif
		} else {
			--this.touchCountDown;
			if (this.touchCountDown < 0) {
				this.touchCountDown = 0;
				#if UNITY_ANDROID && !UNITY_EDITOR
				Application.targetFrameRate = 10;
				#endif
			}
		}
	}	
}
