package com.zoodles.kidmode;

import com.zoodles.kidmode.util.ScreenLock;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;

import android.util.Log;
import android.view.Window;
import android.view.WindowManager;

import com.onevcat.uniwebview.AndroidPlugin;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerNativeActivity;

/**
 * Activity invoked when Home button is pressed.
 * This occurs when Kid Mode is the default responder of the Home intent.
 */
public class ZoodlesHome extends Activity {

	/**
	 * Tag used for logging errors.
	 */
	private static final String TAG = "ZoodlesHome";
	public static final String DIRECT_TO_KID_CHOOSER = "DIRECT_TO_KID_CHOOSER";

	@Override
	public void onCreate(Bundle icicle) {
		super.onCreate(icicle);
//		launchDashboard();		
//  		finish();		
  		
//		requestWindowFeature(Window.FEATURE_NO_TITLE);
		// Set up full screen
//		getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN,
//				WindowManager.LayoutParams.FLAG_FULLSCREEN);
//		new ScreenLock(this).hideStatusBarForHTC(this);

		
/*		
		boolean inChildLockFlow = i.getBooleanExtra( IntentConstants.EXTRA_CL_FLOW, false );
		boolean firstRun = i.getBooleanExtra( IntentConstants.EXTRA_CL_FIRST_RUN, false);
		boolean redirectToKidChooser = i.getBooleanExtra( DIRECT_TO_KID_CHOOSER, false);
		int flag = i.getIntExtra( IntentConstants.EXTRA_FLAG, 0 );

		if (redirectToKidChooser) {
			Intent kidChooserIntent = new Intent().setClass(this, KidChooserActivity.class);
			startActivity(kidChooserIntent);
			finish();
			return;
		}
		ChildLock childLock = App.instance().childLock();
		
		// We can divide invocations of this Activity into three use cases:
		//
		//    1) In Child Lock flow - this activity has been invoked while the user
		//       is attempting to lock the Home button. In this case, we are
		//       being invoked immediately after the user has made a choice from
		//       the Android intent resolution dialog.
		//
		//    2) Synthetic home intent - for example, issued by the system during
		//       power up to launch the system home screen.
		//
		//    3) All other times - the child inadvertently hits Home during gameplay.
		//
		// 	  Basically, we do nothing for case (3).  For case (1), we need to route
		//    the user to the next step in the Child Lock setup flow.  For case (2),
		//    we invoke LauncherActivity, as if the user has tapped Kid Mode's icon
		//    in the real system launcher.
		
		if ( inChildLockFlow ) {

			// One of the following scenarios can be the case, depending upon how 
			// successful the user was at navigating the Intent resolution dialog.
			//
			// 1) Success - We are the default responder to Home app.
			//
			// 2) Failure - The user selected Kid Mode, but didn't mark us default.
			//    That's a bummer, but we can deal with it.
			//
			// 3) Failure - the user hits the Back button at the Intent resolution
			//    dialog.  The ChildLockActivity deals with this case.
			//
			// 4) Failure (not recoverable) - The user chose another app such as the
			//    system Home app.  The flow is broken, Kid Mode is no longer in
			//    front of the user, and the system will never return here - it will
			//    launch that app instead.
			//
			// 5) Failure (not recoverable) - Like (4) but in addition the user has
			//    assigned the Home button default to another app. This is the worst
			//    failure possible, and will require ChildLockActivity to lead the
			//    user through a recovery process via the system Settings app.
			// 
			// We (can only) handle scenarios (1) and (2) below.
			
			int prompt = childLock.isDefault( App.instance() ) ? 
						ChildLockFragment.PROMPT_DEFAULT : 				// 1) Success
						ChildLockFragment.PROMPT_FORGOT_CHECKBOX;		// 2) Failure - user forgot to check the 'default' checkbox.		
			relaunchChildLockActivity( prompt, flag, firstRun );
		}
		
		// If this home intent was not invoked while in the kid experience
		// (for instance, as part of system boot), head to the front door of
		// the app.
		if ( !App.instance().childLock().inChildLock() ) {
			LauncherActivity.launch( this );
		} 
		
		//If user doesnt intent to exit the app, we just go to our Kid Chooser as a dashboard
		boolean exitToSystem = getIntent().getBooleanExtra(ExitHelper.EXIT_THIS_LAUNCHER_INTENT, false);
		if(!exitToSystem){
			KidChooserActivity.launchFromHome(this);
		}
		finish();
*/
	}
	
	private void launchDashboard()
	{
		App l_app = App.instance();
     	Intent l_launchIntent = new Intent();
     	l_launchIntent.setClass(l_app, AndroidPlugin.class);
     	l_launchIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
 		startActivity(l_launchIntent);		
	}
	
	
	@Override
	public void onResume()
	{
		super.onResume();
	}
	@Override
	public void onRestart()
	{
		super.onRestart();
	}
	
	@Override
	public void onWindowFocusChanged(boolean hasFocus) {
		new ScreenLock(this).closeStatusBar(this);
	}

/*
	private void relaunchChildLockActivity( int prompt, int flag, boolean firstRun ) 
	{
		if ( firstRun ) {
			Log.d( TAG, "First run flow, launching back into IntroChildLockActivity" );
			IntroChildLockActivity.launch( this, prompt, flag );
		} else {
			Log.d( TAG, "Launching back into Parent Dashboard" );
			Bundle arguments = ChildLockFragment.buildArguments(flag, prompt);
			NewSuperParentDashboardActivity.launch( this, Feature.CHILD_LOCK, -1, arguments );
		}
	}
*/
	
	public static void launch(Activity activity) 
	{
//		Log.v("Zoodles", "launch");
/*		
 		if (App.instance().childLock().inChildLock()) {
			Intent startMain = new Intent(Intent.ACTION_MAIN);
			startMain.addCategory(Intent.CATEGORY_HOME);
			startMain.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
			startMain.putExtra(DIRECT_TO_KID_CHOOSER, true);
			App.instance().getApplicationContext().startActivity(startMain);
		} else {
			AndroidPlugin.launch(activity);
		}
*/
	}

}
