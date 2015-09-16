using UnityEngine;
using System.Collections;

public class RegionAppScreen : MonoBehaviour {

	public GameObject SpeechBoxHey;

	public GameObject SpeechBoxRecommendApps;

	public GameObject MonkeyImage;

	public GameObject MonkeyImageHighlight;

	public int ShowHeyChance = 50;

	public float ShowHeyTime = 3.0f;

	public enum State{
		Wating,
		SpeechHey,
		SpeechRecommend,
		ToParentGate
	}

	public State state;

	// Use this for initialization
	void Start () {

		InvokeRepeating("checkSpeechBoxHey", ShowHeyTime, ShowHeyTime);

		SpeechBoxHey.SetActive(false);

		SpeechBoxRecommendApps.SetActive(false);

		MonkeyImageHighlight.SetActive(false);

		MonkeyImage.SetActive(true);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void checkSpeechBoxHey(){

		int rndSpeech = Random.Range(0, 100);

		if(ShowHeyChance > rndSpeech){

			state = State.SpeechHey;

			SpeechBoxHey.SetActive(true);

			SpeechBoxRecommendApps.SetActive(false);

		}

	}

	public void monkeyClick(){

		Debug.Log("  monkeyClick  ");

		if(state != State.SpeechRecommend){

			CancelInvoke("checkSpeechBoxHey");

			SpeechBoxRecommendApps.SetActive(true);

			SpeechBoxHey.SetActive(false);

			state = State.SpeechRecommend;

		}else if(state == State.SpeechRecommend){

			MonkeyImageHighlight.SetActive(true);
			
			MonkeyImage.SetActive(false);

			Invoke("parentGateThenRecommendedApps", 0.4f);

//			parentGateThenRecommendedApps();

		}

	}

	private void parentGateThenRecommendedApps(){

		Game game = GameObject.FindWithTag("GameController").GetComponent<Game>();

		game.callParentGate();

	}


}
