using UnityEngine;
using System.Collections;

public class RegionAppScreen : MonoBehaviour {

	public GameObject SpeechBoxHey;

	public GameObject SpeechBoxRecommendApps;

	public int ShowHeyChance = 50;

	public float ShowHeyTime = 3.0f;

	// Use this for initialization
	void Start () {

		InvokeRepeating("checkSpeechBoxHey", ShowHeyTime, ShowHeyTime);

		SpeechBoxHey.SetActive(false);

		SpeechBoxRecommendApps.SetActive(false);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void checkSpeechBoxHey(){

		int rndSpeech = Random.Range(0, 100);

		if(ShowHeyChance > rndSpeech){

			SpeechBoxHey.SetActive(true);

		}

	}


}
