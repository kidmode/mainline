using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InfoScript : MonoBehaviour {

	private Game game;
	private GameObject infoPanel;
	
	void Start () 
	{
		GameObject gameLogic = GameObject.FindWithTag("GameController");
		game = gameLogic.GetComponent<Game>();
		infoPanel = GameObject.FindGameObjectWithTag("InfoPanel");
		infoPanel.SetActive(false);
	}

	public void hideInfoPanel()
	{
		infoPanel.SetActive(false);
	}

	public void showInfoPanel()
	{
		infoPanel.SetActive(true);
		setInfo();
	}

	private void setInfo()
	{
		Text clientIdText = infoPanel.transform.Find("ClientId/ClientIdText").gameObject.GetComponent<Text>();
		Text renewPeriodText = infoPanel.transform.Find("RenewPeriod/RenewPeriodText").gameObject.GetComponent<Text>();
		Text tokenText = infoPanel.transform.Find("Token/TokenText").gameObject.GetComponent<Text>();

		clientIdText.text = SessionHandler.getInstance().clientId.ToString();
		renewPeriodText.text = SessionHandler.getInstance().renewalPeriod.ToString();
		tokenText.text = SessionHandler.getInstance().token.getSecret();
	}


}
