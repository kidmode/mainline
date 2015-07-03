using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextLocalize : MonoBehaviour {

	[SerializeField]
	private string localizationKey;

	// Use this for initialization
	void Start () {

		Text displayText = gameObject.GetComponent<Text>();

		displayText.text = Localization.getString(localizationKey);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
