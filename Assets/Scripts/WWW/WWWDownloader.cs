using UnityEngine;
using System.Collections;
using System;

//=============================================================================
//Author: Kevin
//
//Date: Dec 2015
//Purpose: 
//Use the WWW to download data/ or anything from url
//once it is done, event OnDownloadDone is called
//and then we could get texture, data etc
//
//=============================================================================

public class WWWDownloader : MonoBehaviour {

	//the url for the www
	public string downloadURL;
	//the www object
	public WWW www;

	//Action event when the download is done 
	public event Action OnDownloadDone;

	public enum State{
		START,//STARTed downloading
		DONE//Done downloading
	}

	public State state;

	// Use this for initialization
	void Start () {


	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void startWWWDownload(string downloadURL){

		this.downloadURL = downloadURL;

		StartCoroutine(DownloadImage(downloadURL));

	}

	private IEnumerator DownloadImage(string downloadURL)
	{

		state = State.START;

		www = new WWW(downloadURL);

		yield return www;
//		
		if (www.error == null)
		{

			state = State.DONE;

			if(OnDownloadDone != null)
				OnDownloadDone();

		}


	}
}
