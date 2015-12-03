using UnityEngine;
using System.Collections;

public class BuildSettings : MonoBehaviour {

	public static BuildSettings Instance;

	public string[] sceneNames;

	void Awake(){

		Instance = this;

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
