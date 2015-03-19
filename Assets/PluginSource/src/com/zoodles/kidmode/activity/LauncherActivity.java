package com.zoodles.kidmode.activity;

import com.onevcat.uniwebview.AndroidPlugin;
import com.zoodles.kidmode.App;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;

public class LauncherActivity extends Activity
{
	public static String KIDMODE_ACTION = "com.zoodles.kidsmode";
	
	public void onCreate(Bundle p_bundle) 
	{
		super.onCreate(p_bundle);		
     	launchKidmode();
		Log.v("Zoodles", "Zoodles Launcher");
 		finish();		
	}	
	
	private void launchKidmode()
	{
		App l_app = App.instance();
     	Intent l_launchIntent = new Intent();
     	l_launchIntent.setClass(l_app, AndroidPlugin.class);
     	l_launchIntent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
     	l_launchIntent.setAction(KIDMODE_ACTION);
 		startActivity(l_launchIntent);		
	}
	
}
