using UnityEngine;
using System.Collections;
using System;

public class TutorialController : MonoBehaviour {

	public static TutorialController Instance;

	[SerializeField]
	private TutorialSequence[] tutPopupSequence;

	[SerializeField]
	private Game gameLogic;

	[SerializeField]
	private TutorialSequence currSequence;

	void Awake(){

		Instance = this;

	}
	//

	// Use this for initialization
	void Start () {

//		showTutorial (TutorialSequenceName.VIOLENCE_LEVEL);

//		clearTutorialPlayerPref (); //FIXME // emove this latter

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void showTutorial(TutorialSequenceName sequenceName){

		TutorialSequence sequence = findTutorialSequence (sequenceName);

		if (sequence != null) {



			int prefSetting = PlayerPrefs.GetInt(sequenceName.ToString());

			if(prefSetting == 0){

				currSequence = sequence;

				Debug.LogWarning(" 000000000000000000000000000000 show it " + currSequence.ToString() );

				UIManager uiManager = gameLogic.gameController.getUI();

				uiManager.createScreen(currSequence.screenID, false, 6);

				PlayerPrefs.SetInt(sequenceName.ToString(), 1);


			}


		}

	}


	public void sequenceScreenFinished(){

		UIManager uiManager = gameLogic.gameController.getUI();
		
		uiManager.removeScreen(currSequence.screenID);

	}


	TutorialSequence findTutorialSequence(TutorialSequenceName sequenceName){

		for (int i = 0; i < tutPopupSequence.Length; i++) {

			TutorialSequence sequence = tutPopupSequence[i];

			if(sequence.sequenceName == sequenceName){

				return sequence;

			}

		}

		return null;

	}

	void clearTutorialPlayerPref(){

		for (int i = 0; i < tutPopupSequence.Length; i++) {
			
			TutorialSequence sequence = tutPopupSequence [i];

			PlayerPrefs.SetInt(sequence.sequenceName.ToString(), 0);

		}

	}

}


[Serializable]
public class TutorialSequence{

	public TutorialSequenceName sequenceName;

	//public string prefSaveName;
	public int screenID;

}


public enum TutorialSequenceName{
	
	Add_YOUR_APP,
	VIOLENCE_LEVEL
	
	
}


//
//[Serializable]
//public class TutorialSequenceName
//{
//	public const int Add_YOUR_APP      = 0;
//	public const int VIOLENCE_LEVEL    = 1;
//
//}
