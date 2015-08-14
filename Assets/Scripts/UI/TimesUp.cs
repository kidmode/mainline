using UnityEngine;
using System.Collections;

public class TimesUp : MonoBehaviour {

//	private Game game;
	
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void removeTimesUpScreen()
	{
		TimerController.Instance.removeTimesUpScreen();
	}
}
