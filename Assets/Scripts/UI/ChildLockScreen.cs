using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChildLockScreen : MonoBehaviour {

	public GameObject dialogChildLockChangeOK;

	public GameObject dialogChildLockChangeError;

	public GameObject dialogChildLockChangeHelp;

	//Localization
	public Text[] childLockChangeTitles;

	public Text dialogChildLockChangeHelpTxt1;

	public Text dialogChildLockChangeHelpTxt2;

	public Text childLockHelpButtonText;

	public Text dialogChildLockErrorTxt1;
	
	public Text dialogChildLockErrorTxt2;

	public Text dialogChildLockOkayTxt;

	public Text[] childLockCloseButtonTexts;

	public Text childLockScreenText1;
	public Text childLockScreenText2;
	public Text childLockScreenSaveButton;

	// Use this for initialization
	void Start () {
	
		setupLocalization();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void setupLocalization(){

		for (int i = 0; i < childLockChangeTitles.Length; i++) {

			childLockChangeTitles[i].text =  Localization.getString(Localization.TXT_STATE_31_PIN_CHANGE_TITLE);
		}

		dialogChildLockChangeHelpTxt1.text =  Localization.getString(Localization.TXT_STATE_31_PIN);

		dialogChildLockChangeHelpTxt2.text =  Localization.getString(Localization.TXT_STATE_31_PIN_LINE2);

		childLockHelpButtonText.text = Localization.getString(Localization.TXT_STATE_31_PIN_CHANGE_HELP_BUTTON_TEXT);
		//TXT_STATE_31_PIN_CHANGE_HELP_BUTTON_TEXT

		dialogChildLockErrorTxt1.text = Localization.getString(Localization.TXT_STATE_31_PIN_CHANGE_ERROR_MSSG1);

		dialogChildLockErrorTxt2.text = Localization.getString(Localization.TXT_STATE_31_PIN_CHANGE_ERROR_MSSG2);

		dialogChildLockOkayTxt.text = Localization.getString(Localization.TXT_STATE_31_PIN_CHANGE_OKAY_MSSG1);

		for (int i = 0; i < childLockCloseButtonTexts.Length; i++) {
			
			childLockCloseButtonTexts[i].text =  Localization.getString(Localization.TXT_STATE_31_PIN_CHANGE_MSSG_CLOSE);
		}

		childLockScreenText1.text = Localization.getString(Localization.TXT_CHILD_LOCK_CODE_1);
		childLockScreenText2.text = Localization.getString(Localization.TXT_CHILD_LOCK_CODE_2);
		childLockScreenSaveButton.text = Localization.getString(Localization.TXT_CHILD_LOCK_CODE_SAVE);
	}

	public void closeDialogChildLockChangeOK(){

		dialogChildLockChangeOK.gameObject.SetActive(false);

	}

	public void closeDialogChildLockChangeError(){

		dialogChildLockChangeError.gameObject.SetActive(false);
		
	}

	public void closeDialogChildLockChangeHelp(){

		dialogChildLockChangeHelp.gameObject.SetActive(false);
		
	}

}
