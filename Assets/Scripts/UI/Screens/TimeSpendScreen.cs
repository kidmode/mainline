using UnityEngine;
using System.Collections;

public class TimeSpendScreen : MonoBehaviour {

	private Game game;

	// Use this for initialization
	void Start () {

		game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void promoteSubjectButtonClicked(){

		game.gameController.changeState( ZoodleState.CONTROL_SUBJECT );

	}

}
