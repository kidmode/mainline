using UnityEngine;
using System.Collections;
using System;

public class WWWDownloader : MonoBehaviour {

	public Texture2D loadedTexture;

	public string downloadURL;

	public WWW www;

	public event Action OnDownloadDone;

	public enum State{
		START,
		DONE
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

//		Debug.Log(" start download ing ");

		yield return www;

//		Debug.Log(" www returned " + www.error);
//		
		if (www.error == null)
		{

			state = State.DONE;

			if(OnDownloadDone != null)
				OnDownloadDone();

		}


	}
}
