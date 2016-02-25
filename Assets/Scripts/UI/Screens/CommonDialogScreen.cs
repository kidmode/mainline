using UnityEngine;
using System.Collections;

public class CommonDialogScreen : MonoBehaviour {

	private Game game;

	// Use this for initialization
	void Start () {

		game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void removeCommonDialogScreen(){

	}

}
