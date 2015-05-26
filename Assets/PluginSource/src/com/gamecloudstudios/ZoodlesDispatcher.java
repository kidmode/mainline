package com.gamecloudstudios;

import static com.zoodles.kidmode.IntentConstants.EXTRA_ACTIVITY;
import static com.zoodles.kidmode.IntentConstants.EXTRA_PACKAGE;

import com.onevcat.uniwebview.AndroidPlugin;
import com.zoodles.kidmode.App;
import com.zoodles.kidmode.IntentConstants;
import com.zoodles.kidmode.service.NativeAppService;

import android.app.Activity;
import android.app.PendingIntent;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

public class ZoodlesDispatcher extends Activity 
{
	public static String DISPATCHER_PACKAGE = "DISPATCHER_PACKAGE";
	public static String DISPATCHER_ACTIVITY = "DISPATCHER_ACTIVITY";	
	public static Activity main;
	
	@Override
	public void onCreate(Bundle p_bundle) 
	{
		super.onCreate(p_bundle);
		
		Intent l_intent = this.getIntent();
		String l_packageName = l_intent.getStringExtra( DISPATCHER_PACKAGE );
		String l_activityName = l_intent.getStringExtra( DISPATCHER_ACTIVITY );
		
		Intent playgroundIntent = new Intent().setClass( this, this.getClass() );
		playgroundIntent.putExtra( IntentConstants.EXTRA_NATIVE_APP, true );
		playgroundIntent.addFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);
		PendingIntent pi = PendingIntent.getActivity( this.getApplicationContext(), 0, playgroundIntent, 0 );

//		Log.v("Zoodles", "starting " + l_packageName + " " + l_activityName);
		
		NativeAppService.startApp(this, 0, l_packageName, l_activityName, pi, null);
		
//		Log.v("Zoodles", "Dispatching");
	}
	
	@Override
	public void onPause()
	{
		super.onPause();
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
	public void onWindowFocusChanged(boolean hasFocus) 
	{
//		Log.v("Zoodles", "window focus changed");
	}

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
	
	private void launchDashboard()
	{
		App l_app = App.instance();
     	Intent l_launchIntent = new Intent();
     	l_launchIntent.setClass(l_app, AndroidPlugin.class);
     	l_launchIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
 		startActivity(l_launchIntent);		
	}
}
