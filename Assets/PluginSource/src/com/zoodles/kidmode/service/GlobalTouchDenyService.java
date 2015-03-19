package com.zoodles.kidmode.service;

import android.app.ActivityManager;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.graphics.PixelFormat;
import android.os.*;
import android.util.Log;
import android.view.*;
import android.view.View.OnTouchListener;
import android.widget.LinearLayout;
import android.widget.LinearLayout.LayoutParams;

import java.util.ArrayList;
import java.util.List;

import com.onevcat.uniwebview.AndroidPlugin;
import com.zoodles.kidmode.App;

public class GlobalTouchDenyService extends Service implements OnTouchListener {

	private static final long DELAY = 100;
	private static final long WATCHER_INTERVAL = 1000;
	private static final String TAG = "GlobalTouchDenyService";
	private static final String ZOODLES_PACKAGE = "com.zoodles.kidmode";
	private static final String PLUGIN_PACKAGE = "com.onevcat.uniwebview";
	// window manager
	private WindowManager mWindowManager;
	// linear layout will use to detect touch event
	private LinearLayout touchLayout;
	private int retry;
	private final int MAX_RETRY = 5;
	private boolean started;
	private Object lock = new Object();
	private Object watcherLock = new Object();
	private WindowManager.LayoutParams mParams;
	private List<String> whiteLists = new ArrayList<String>();
	private List<String> watcherList = new ArrayList<String>();
	private HandlerThread watcherThread;
	private Handler watcherHandler;

	@Override
	public IBinder onBind(Intent arg0) {
		return mBinder;
	}

	// Binder given to clients
	private final IBinder mBinder = new LocalBinder();

	/**
	 * This method start a looper thread to check if something else not in
	 * whitelist trying to launch
	 */
	public void startWatcherTouchDenyService() {
		synchronized (watcherLock) {
			if (started) {
				return;
			}

			watcherHandler.sendEmptyMessage(0);
			Log.v("Zoodles", "Mesage for watcher thread looper to start.");

		}
	}

	public void stopTouchDenyWatcher() {
		synchronized (watcherLock) {
			watcherHandler.removeMessages(0);
			for (String pkg : watcherList) {
				// NativeAppService.stopApp( getApplicationContext(), pkg, null
				// );
			}
		}
	}

	/**
	 * Class used for the client Binder. Because we know this service always
	 * runs in the same process as its clients, we don't need to deal with IPC.
	 */
	public class LocalBinder extends Binder {
		public GlobalTouchDenyService getService() {
			// Return this instance of LocalService so clients can call public
			// methods
			return GlobalTouchDenyService.this;
		}
	}

	public void startTouchDenyService() {
		synchronized (lock) 
		{
			if (started) 
			{
				Log.v("Zoodles", "Already Started");
				return;
			}
			
			retry = 0;
			final ActivityManager activityManager = (ActivityManager) getSystemService(Context.ACTIVITY_SERVICE);
			final List<ActivityManager.RunningTaskInfo> recentTasks = activityManager.getRunningTasks(3);
			String topPackageName = recentTasks.get(0).baseActivity.getPackageName();

			Log.d(TAG, "Top Package Name " + topPackageName);
			if ((recentTasks != null 
					&& recentTasks.size() > 0 
					&& topPackageName.equals(getPackageName()))
				|| whiteLists.contains(topPackageName)) 
			{
				Log.v("Zoodles", "retry  blocking task");
				retryBlockingTask();
				return;
			}

			Log.v("Zoodles", "add block view");
			addBlockView();
		}
	}

	public void stopTouchDenyService() {
		synchronized (lock) {
			if (mWindowManager != null && (touchLayout != null) && (started)) {
				handler.removeMessages(0);
				mWindowManager.removeView(touchLayout);
				started = false;

			}

		}
	}

	Handler handler = new Handler() {
		@Override
		public void handleMessage(Message msg) {
			super.handleMessage(msg);

			final ActivityManager activityManager = (ActivityManager) getSystemService(Context.ACTIVITY_SERVICE);
			final List<ActivityManager.RunningTaskInfo> recentTasks = activityManager.getRunningTasks(3);
			String topPackageName = recentTasks.get(0).baseActivity.getPackageName();
			Log.v(TAG, "In Handler Top Package Name " + topPackageName);

			if ((recentTasks != null 
					&& recentTasks.size() > 0 
					&& topPackageName.equals(getPackageName()))
				|| whiteLists.contains(topPackageName)) 
			{
				if (retry < MAX_RETRY) 
				{
					retry++;
					handler.sendEmptyMessageDelayed(0, DELAY);
				} 
				else 
				{
					handler.removeMessages(0);
					App.instance().stopLockScreenService();
				}
				return;
			}

			addBlockView();
			handler.removeMessages(0);
		}
	};

	@Override
	public void onCreate() {
		super.onCreate();
		whiteLists.add("com.htc.powersavinglauncher");
		whiteLists.add(ZOODLES_PACKAGE);
		whiteLists.add(PLUGIN_PACKAGE);
		whiteLists.add(this.getPackageName());

		Log.v("Zoodles2", "package name:" + this.getPackageName());

		watcherList.add(PLUGIN_PACKAGE);

		// create linear layout
		touchLayout = new LinearLayout(this);
		// set layout full screen
		LayoutParams lp = new LayoutParams(LayoutParams.MATCH_PARENT,
				LayoutParams.MATCH_PARENT);
		touchLayout.setLayoutParams(lp);
		// set color if you want layout visible on screen
		// set on touch listener
		touchLayout.setOnTouchListener(this);
		touchLayout.addView(getWarningView(), lp);
		// fetch window manager object
		mWindowManager = (WindowManager) getSystemService(WINDOW_SERVICE);
		// set layout parameter of window manager
		mParams = new WindowManager.LayoutParams(
				WindowManager.LayoutParams.MATCH_PARENT,
				WindowManager.LayoutParams.MATCH_PARENT, // height is equal to
															// full screen
				WindowManager.LayoutParams.TYPE_PHONE, // Type Ohone, These are
														// non-application
														// windows providing
														// user interaction with
														// the phone (in
														// particular incoming
														// calls).
				WindowManager.LayoutParams.FLAG_ALT_FOCUSABLE_IM,
				PixelFormat.TRANSLUCENT);
		mParams.gravity = Gravity.CENTER | Gravity.CENTER;

		watcherThread = new HandlerThread("watcherThread");
		watcherThread.start();

		Log.v("Zoodles", "Watcher thread started");

		watcherHandler = new Handler(watcherThread.getLooper())
		{
			@Override
			public void handleMessage(Message msg)
			{
				super.handleMessage(msg);
				synchronized (watcherLock)
				{
					Log.v("Zoodles", "watcher");
					final ActivityManager activityManager = (ActivityManager) getSystemService(Context.ACTIVITY_SERVICE);
					final List<ActivityManager.RunningTaskInfo> recentTasks = activityManager.getRunningTasks(3);
					String topPackageName = recentTasks.get(0).baseActivity.getPackageName();
					watcherHandler.sendEmptyMessageDelayed(0, WATCHER_INTERVAL);

					if (!whiteLists.contains(topPackageName) && !watcherList.contains(topPackageName))
					{
						addBlockView();
					}
				}
			}
		};

	}

	private void retryBlockingTask() {
		handler.sendEmptyMessage(0);
	}

	private void addBlockView() {

		if (!started) {
			Log.v("Zoodles", "Add block view thing");
			mWindowManager.addView(touchLayout, mParams);
		}
		started = true;
	}

	private View getWarningView() {
		LayoutInflater inflater = (LayoutInflater) getSystemService(Context.LAYOUT_INFLATER_SERVICE);
		int l_resource = this.getResources().getIdentifier("overlay_blocking_view", "layout", this.getPackageName());
		View view = inflater.inflate(l_resource, null);
		return view;
	}

	@Override
	public void onDestroy() {
		stopTouchDenyService();
		super.onDestroy();
	}

	@Override
	public boolean onTouch(View v, MotionEvent event) 
	{
		stopTouchDenyService();
		stopTouchDenyWatcher();

		Intent startMain = new Intent(Intent.ACTION_MAIN);
		startMain.addCategory(Intent.CATEGORY_HOME);
		startMain.addFlags(Intent.FLAG_ACTIVITY_NO_ANIMATION);
		startMain.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		startActivity(startMain);
		return true;
	}

	public LinearLayout getInterceptedScreenLayout() {
		return touchLayout;
	}

}
