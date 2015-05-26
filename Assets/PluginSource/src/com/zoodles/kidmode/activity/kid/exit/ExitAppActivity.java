package com.zoodles.kidmode.activity.kid.exit;

import com.onevcat.uniwebview.AndroidPlugin;
import com.zoodles.kidmode.App;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

public class ExitAppActivity extends Activity
{
	public static String KIDMODE_EXIT_ACTION = "com.zoodles.kidsmode.exit";
	
	public void onCreate(Bundle p_bundle) 
	{
		super.onCreate(p_bundle);		
     	launchDashboard();
//		Log.v("Zoodles", "Zoodles Exit");
 		finish();		
	}	
	
	@Override
	public void onNewIntent(Intent p_intent)
	{
		super.onNewIntent(p_intent);
//		Log.v("Zoodles", "Zoodles Exit");
		launchDashboard();		
	}
	
	private void launchDashboard()
	{
		App l_app = App.instance();
     	Intent l_launchIntent = new Intent();
     	l_launchIntent.setClass(l_app, AndroidPlugin.class);
     	l_launchIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
     	l_launchIntent.setAction(KIDMODE_EXIT_ACTION);
 		startActivity(l_launchIntent);		
	}
}
