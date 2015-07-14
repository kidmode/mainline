using UnityEngine;
using System.Collections;

public class ScreenLockTest : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		firstUpdateTime = 1000;
		KidMode.requestHomeButton();


	}

	private float firstUpdateTime = 0;

	// Update is called once per frame
	void Update () 
	{
		firstUpdateTime = firstUpdateTime - 1;
		if (firstUpdateTime < 0)
		{
//			lockTask();
//			KidMode.requestHomeButton();
			firstUpdateTime = 1000000;
			_Debug.log("first frame calls");
		}

//		updateLockTaskOnResume();
	}

	void OnApplicationFocus(bool focusStatus)
	{
//		_Debug.log("on application focus " + focusStatus);
	}

//	private bool updateLock = false;
	void OnApplicationPause(bool pauseStatus) 
	{
//		_Debug.log("on application pause " + pauseStatus);
		if (pauseStatus == false)
		{
//			updateLock = true;
//			updateLockTaskOnResume();
		}
	}

	public void updateLockTaskOnResume()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		jo.Call("updateLockTaskOnResume"); 
		#endif
	}

	public void lockTask()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
//		jo.Call("lockTask"); 
		object[] l_args = new object[] {true};
		jo.Call("_setKidsModeActive", l_args); 
		#endif
	}

	public void unlockTask()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
//		jo.Call("unlockTask"); 
		object[] l_args = new object[] {false};
		jo.Call("_setKidsModeActive", l_args); 
		#endif
	}
	
	public void getInstalledApps()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		jo.Call("getInstalledApps"); 
		#endif
	}

	public void startTempleRun()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		jo.Call("startTempleRun"); 
		#endif
	}

	public void startTempleRunGeneric()
	{
		GameObject gameLogic = GameObject.FindWithTag("GameController");

		#if UNITY_ANDROID && !UNITY_EDITOR
		string l_packageName = "com.imangi.templerun2";
		string l_activityName = "com.prime31.UnityPlayerProxyActivity";

		gameLogic.GetComponent<Game> ().IsReLaunch = 1;

		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		
		object[] l_params = new object[2];
		l_params[0] = l_packageName;
		l_params[1] = l_activityName;
		
		jo.Call("startApp", l_params); 

		#endif
	}

	public void killTempleRun()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		jo.Call("killTempleRun"); 
		#endif
	}

	public void listRunningApps()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 
		jo.Call("listRunningApps"); 
		#endif
	}
}
