using UnityEngine;
using System.Collections;
using System.IO;

public class DebugController : MonoBehaviour {

	public GameObject debugControlsPanel;

	// Use this for initialization
	void Start () {

		//debugControlsPanel

#if UNITY_ANDROID && !UNITY_EDITOR

		if(debugControlsPanel != null){

			debugControlsPanel.SetActive(false);


		}

#endif
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void clearLocalData(){

		PlayerPrefs.DeleteAll();

		File.Delete( Application.persistentDataPath + "/kidList.txt");
		
		File.Delete( Application.persistentDataPath + "/kidList_temp.txt");

		File.Delete( Application.persistentDataPath + "/kidList_backup.txt");

	}




}
