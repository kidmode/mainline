using UnityEngine;
using System.Collections;

public class RegionAppScreen : MonoBehaviour {

	public GameObject SpeechBoxHey;

	public GameObject SpeechBoxRecommendApps;

	public GameObject MonkeyImage;

	public GameObject MonkeyImageHighlight;

	public int ShowHeyChance = 50;

	public float ShowHeyTime = 3.0f;

	public float removeSpeechTime = 5.0f;

	public enum State{
		Wating,
		SpeechHey,
		SpeechRecommend,
		ToParentGate
	}

	public State state;

	// Use this for initialization
	void Start () {

//		InvokeRepeating("checkSpeechBoxHey", ShowHeyTime, ShowHeyTime);

		state = State.Wating;

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

//			CancelInvoke("resetSpeech");
			
			Invoke("resetSpeech", removeSpeechTime);

		}

	}

	public void monkeyClick(){

		Debug.Log("  monkeyClick  ");

		if(state != State.ToParentGate){

			state = State.ToParentGate;
			
			MonkeyImageHighlight.SetActive(true);
			
			MonkeyImage.SetActive(false);
			
			Invoke("parentGateThenRecommendedApps", 0.4f);
			
			//			parentGateThenRecommendedApps();
			
		}


//
//		if(state != State.SpeechRecommend){
//
//			CancelInvoke("resetSpeech");
//
//
//			CancelInvoke("checkSpeechBoxHey");
//
//			SpeechBoxRecommendApps.SetActive(true);
//
//			SpeechBoxHey.SetActive(false);
//
//			state = State.SpeechRecommend;
//
////			
//
//			Invoke("resetSpeech", removeSpeechTime);
//
//		}else if(state == State.SpeechRecommend){
//
//			MonkeyImageHighlight.SetActive(true);
//			
//			MonkeyImage.SetActive(false);
//
//			Invoke("parentGateThenRecommendedApps", 0.4f);
//
////			parentGateThenRecommendedApps();
//
//		}

	}

	private void resetSpeech(){

		state = State.Wating;

		SpeechBoxRecommendApps.SetActive(false);

		SpeechBoxHey.SetActive(false);

		CancelInvoke("checkSpeechBoxHey");

	}

	private void parentGateThenRecommendedApps(){

		Game game = GameObject.FindWithTag("GameController").GetComponent<Game>();

		game.callParentGate();

	}


}
