using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class PDHelpButton : MonoBehaviour {

	[SerializeField]
	private HelpInfo[] helpInfo;

	[SerializeField]
	private Button button;

	[SerializeField]
	private Behaviour[] behaviours;

	private Game game;

	private CommonDialogCanvas helpDialogCanvas;

	// Use this for initialization
	void Start () {

		game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();

		disableButton();
	
	}

	void OnEnable(){

		GameController.onStateChanged += onStateChanged;

	}

	void OnDisable(){

		GameController.onStateChanged -= onStateChanged;

	}

	void disableButton(){

		for (int i = 0; i < behaviours.Length; i++) {

			behaviours[i].enabled = false;

		}

	}

	void enableButton(){
		
		for (int i = 0; i < behaviours.Length; i++) {
			
			behaviours[i].enabled = true;
			
		}
		
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void helpButtonClicked(){

		helpDialogCanvas = game.gameController.getUI().createScreen( UIScreen.COMMON_DIALOG, true, 15 )  as CommonDialogCanvas;
		
		helpDialogCanvas.setUIManager (game.gameController.getUI());

		helpDialogCanvas.setOriginalPosition ();
		UIButton l_closeButton = helpDialogCanvas.getView ("closeMark") as UIButton;
		
		UILabel l_titleLabel = helpDialogCanvas.getView ("dialogText") as UILabel;
		UILabel l_contentLabel = helpDialogCanvas.getView ("contentText") as UILabel;
		l_titleLabel.text = Localization.getString(Localization.TXT_STATE_48_HELP_TITLE);
		l_contentLabel.text = Localization.getString(Localization.TXT_STATE_48_HELP_CONTENT);
		
		l_closeButton.addClickCallback (onCloseDialogButtonClick);

	}

	private void onCloseDialogButtonClick(UIButton button){

		helpDialogCanvas.setOutPosition ();

	}

	#region Event
	//Events
	void onStateChanged(GameState state, int stateType){

		Debug.LogWarning("   stateType  = == = = = = = = = " + stateType);

		if(hasStateHelpButton(stateType)){

			enableButton();

		}else{

			disableButton();

		}

	}

	//End
	#endregion

	private bool hasStateHelpButton(int stateType){

		for (int i = 0; i < helpInfo.Length; i++) {

			HelpInfo info = helpInfo[i];

			if(info.stateType == stateType)

				return true;

		}

		return false;

	}

}

[Serializable]
public class HelpInfo{

	public int stateType;


}


