using UnityEngine;
using System.Collections;

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
		
		//		return;
		
		//		Debug.Log ("   +++++++++++++++++++++++++++++  kidModeNativeCalls.isLauncherKidmode ()  " + kidModeNativeCalls.isLauncherKidmode ());
		
		if (stateKidMode == StateKidMode.Parent) {
			
			KidMode.enablePluginComponent ();
			
			
			defaultLauncherChecked = true;
			
			return;
			
		}
		
		
		if (!KidMode.isLauncherKidmode ()) {
			
			KidMode.enablePluginComponent ();
			
			KidMode.openLauncherSelector ();
			
		} else {
			
			defaultLauncherChecked = true;
			
		}
		
	}
	
	
	
	
	public void onAndroidPause(){
		
		defaultLauncherChecked = false;
		
	}
	
	public void onAndroidResume(){
		
		checkDefaultLauncherStatus ();
		
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
				
			}else{//Kid Mode
				
				
				
				KidMode.enablePluginComponent ();
				
				checkDefaultLauncherStatus();
				
				KidMode.taskManagerLockTrue();
				
				// 
				
			}
			
		} else {//Kid Mode Launcher
			
			if (stateKidMode == StateKidMode.Parent) {//Parent
				
				KidMode.enablePluginComponent ();
				
				KidMode.taskManagerLockFalse();
				
			}else{//Kid Mode
				
				KidMode.enablePluginComponent();
				
				checkDefaultLauncherStatus();
				
				KidMode.taskManagerLockTrue();
				
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
