using UnityEngine;
using System.Collections;

public class NavigationBarControl : MonoBehaviour {

	#if UNITY_ANDROID
	const int SYSTEM_UI_FLAG_IMMERSIVE = 2048;
	const int SYSTEM_UI_FLAG_IMMERSIVE_STICKY = 4096;
	const int SYSTEM_UI_FLAG_HIDE_NAVIGATION = 2;
	const int SYSTEM_UI_FLAG_FULLSCREEN = 4;
	const int SYSTEM_UI_FLAG_LAYOUT_STABLE = 256;
	const int SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION = 512;
	const int SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN = 1024;

	bool mIsKeyboardShow	=	false;

	AndroidJavaObject decorView;

	void Start() {
		TurnImmersiveModeOn();

		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow");
		decorView = window.Call<AndroidJavaObject>("getDecorView");

		mIsKeyboardShow = false;


	}
	
	void Update() {

		if(TouchScreenKeyboard.visible && !mIsKeyboardShow) {
			mIsKeyboardShow	= true;
			TurnImmersiveModeOff();;
		}
		else if(!TouchScreenKeyboard.visible && mIsKeyboardShow) {
			mIsKeyboardShow = false;
			TurnImmersiveModeOn();
		}
	}


	public void TurnImmersiveModeOn()
	{
		using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) { 
			AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
			activity.CallStatic("showFullScreen"); 
		}
	}
	
	void TurnImmersiveModeOff() {
		decorView.Call("setSystemUiVisibility",
		               SYSTEM_UI_FLAG_LAYOUT_STABLE | SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION | SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN);
	}

	
	#endif
}
