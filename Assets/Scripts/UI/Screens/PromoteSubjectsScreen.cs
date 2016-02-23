using UnityEngine;
using System.Collections;

public class PromoteSubjectsScreen : MonoBehaviour {

	private Game game;

	[SerializeField]
	private ScreenController bottomSaveButtonsControl;

	// Use this for initialization
	void Start () {

		game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();

		if( SessionHandler.getInstance().token.isPremium() ){
			
			bottomSaveButtonsControl.showScreenAtIndex(0);
			
		}else{
			
			bottomSaveButtonsControl.showScreenAtIndex(1);
			
		}
	
	}

	
	// Update is called once per frame
	void Update () {
	
	}

	public void viewSubjectReportButtonClicked(){
		
		game.gameController.changeState( ZoodleState.OVERVIEW_TIMESPENT );
		
	}

	public void goPremiumButtonClicked(){
		
		game.gameController.connectState( ZoodleState.VIEW_PREMIUM, ZoodleState.CONTROL_SUBJECT );
		
		game.gameController.changeState( ZoodleState.VIEW_PREMIUM );
	}


}
