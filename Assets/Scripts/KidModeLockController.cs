using UnityEngine;
using System.Collections;
using System;

using UnityEngine.UI;

public class KidModeLockController : MonoBehaviour {

	public static KidModeLockController Instance;
	
	public enum StateHomeLauncher{
		
		KidMode,
		Default
		
	}
	
	public StateHomeLauncher stateHomeLauncher;
	
	public enum StateKidMode{
		
		Parent,
		Kid
		
	}
	
	public StateKidMode stateKidMode;
	
	public bool defaultLauncherChecked = false;

	void Awake(){

		Instance = this;

	}
	
	// Use this for initialization
	void Start () {


		string settingsLauncher = PlayerPrefs.GetString ("settingsLauncher");

		if (settingsLauncher == null || settingsLauncher == "") {

			KidModeLockController.Instance.stateHomeLauncher = StateHomeLauncher.KidMode;

		} else {
		
			KidModeLockController.Instance.stateHomeLauncher = (KidModeLockController.StateHomeLauncher)Enum.Parse (typeof(KidModeLockController.StateHomeLauncher), settingsLauncher);   

		}
		
		checkDefaultLauncherStatus ();
		
	}

	
	// Update is called once per frame
	void Update () {

		
		//		checkDefaultLauncherStatus ();
		
		if (!defaultLauncherChecked) {
			
			checkDefaultLauncherStatus();
			
		}
		
		
		
	}
	
	
	void checkDefaultLauncherStatus(){
		
//				return;
		
//		Debug.Log ("  000000000000000000000000000000000000000  checkDefaultLauncherStatus checkDefaultLauncherStatus     checkDefaultLauncherStatus  ");
		
		if (stateKidMode == StateKidMode.Parent) {
			
//			KidMode.enablePluginComponent ();
			
			
			defaultLauncherChecked = true;
			
			return;
			
		}
		

//		Debug.Log ("   0000000000000000000000000000000000000   isLauncherKidmode  ");

		if (!KidMode.isLauncherKidmode ()) {



			
			KidMode.enablePluginComponent ();


//			Debug.Log ("   0000000000000000000000000000000000000   Open Launcher  ");
			
			KidMode.openLauncherSelector ();

			defaultLauncherChecked = true;
			
		} else {
			
			defaultLauncherChecked = true;
			
		}


		
	}
	
	
	
	
	public void onAndroidPause(){
		
		defaultLauncherChecked = false;
		
	}
	
	public void onAndroidResume(){

//		return;
		
//		checkDefaultLauncherStatus ();
		
		stateChanged ();
		
	}
	
	
	
	
	void stateChanged(){
		
		//		Debug.Log ("  ===================================     stateChanged  ================================ ");
		//================================
		//Default Launcher
		if (stateHomeLauncher == StateHomeLauncher.Default) {
			
			if (stateKidMode == StateKidMode.Parent) {//Parent
				
				KidMode.disablePluginComponent();
				
				KidMode.taskManagerLockFalse();

				KidMode.setKidsModeActive(false);

			}else{//Kid Mode
				
				
				
				KidMode.enablePluginComponent ();
				
				checkDefaultLauncherStatus();
				
				KidMode.taskManagerLockTrue();

				KidMode.setKidsModeActive(true);

//				KidMode.setFullScreen();
				
				// 
				
			}
			
		} else {//Kid Mode Launcher
			
			if (stateKidMode == StateKidMode.Parent) {//Parent
				
				KidMode.enablePluginComponent ();
				
				KidMode.taskManagerLockFalse();

				KidMode.setKidsModeActive(false);
				
			}else{//Kid Mode
				
				KidMode.enablePluginComponent();
				
				checkDefaultLauncherStatus();
				
				KidMode.taskManagerLockTrue();

				KidMode.setKidsModeActive(true);

//				KidMode.setFullScreen();
				
			}
			
			
			
		}
		
		
		
		//===============================
		
	}
	
	
	
	//==========
	//========== UI Checks
	public void swith2DefaultLauncher(){
		
		stateHomeLauncher = StateHomeLauncher.Default;
		
		stateChanged ();
		
	}
	
	public void swith2KidModeLauncher(){
		
		stateHomeLauncher = StateHomeLauncher.KidMode;
		
		stateChanged ();
		
	}
	
	
	//==========
	//========== UI Checks
	public void swith2DParentMode(){
		
		stateKidMode = StateKidMode.Parent;
		
		stateChanged ();
		
	}
	
	public void swith2KidMode(){
		
		stateKidMode = StateKidMode.Kid;
		
		stateChanged ();
		
	}
}
