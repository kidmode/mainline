package com.zoodles.kidmode.util;

import android.app.Activity;
import android.app.ActivityManager;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.os.Handler;
import android.util.Log;
import android.view.WindowManager;

import com.zoodles.kidmode.App;
import com.zoodles.kidmode.ZoodlesConstants;
import com.zoodles.kidmode.features.DeviceInfo;

import java.lang.reflect.Method;
import java.util.List;

/**
 * Created by steve on 12/12/13.
 */
public class ScreenLock {
	// HTC's private window Flag. Prevent showing transient status bar.
	private static final int FLAG_FORBID_TRANSIENT_STATUS_BAR = 0x80000000;

	private static final String TAG = "ScreenLock";
	private final Context context;

	public ScreenLock(Context context) {
		this.context = context;
	}

	public void handleAppResume(boolean resume) {
		if (!resume) {
			windowCloseHandler.postDelayed(recentAppCloseRunnable, 10);
		} else {
			
		}
	}

	private Handler windowCloseHandler = new Handler();
	private Runnable recentAppCloseRunnable = new Runnable() {
		@Override
		public void run() {
			ActivityManager am = (ActivityManager) context
					.getSystemService(Context.ACTIVITY_SERVICE);
			List<ActivityManager.RunningTaskInfo> runningTaskInfo = am
					.getRunningTasks(2);

			ComponentName cn = runningTaskInfo.get(0).topActivity;
//			Log.d(TAG, "class " + cn.getClassName());

			if (cn != null
					&& cn.getPackageName().equals(
							ZoodlesConstants.SYSTEM_UI_PACKAGE)) {
//				Log.d(TAG, "System UI class " + cn.getClassName());
				// If we are on Honeycomb or higher, we can bring the app toward
				// to recents
				if (android.os.Build.VERSION.SDK_INT >= Build.VERSION_CODES.HONEYCOMB) {
					am.moveTaskToFront(runningTaskInfo.get(1).id,
							ActivityManager.MOVE_TASK_WITH_HOME);
				} else {
					// Otherwise we have to toogle it, It may not work on some
					// HTC Device
					Intent closeRecents = new Intent(
							"com.android.systemui.recent.action.TOGGLE_RECENTS");
					closeRecents.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK
							| Intent.FLAG_ACTIVITY_EXCLUDE_FROM_RECENTS);
					ComponentName recents = new ComponentName(
							"com.android.systemui",
							"com.android.systemui.recent.RecentsActivity");
					closeRecents.setComponent(recents);
					context.startActivity(closeRecents);
				}
			}
		}
	};

	public void hideStatusBarForHTC(Activity activity) {
		/*
		 * android.os.Build.MANUFACTURER doesnt work on some* HTC devices, so we
		 * need to make sure: 1) Only apply for Kitkat 2) Only has Sense
		 */
		DeviceInfo di = App.instance().deviceInfo();
		if (di.getSDKVersion() == 19 && di.hasHTCSoftware(activity)) {
			// Add FLAG_FORBID_TRANSIENT_STATUS_BAR to current window to prevent
			// showing transient status bar.
			activity.getWindow().addFlags(
					WindowManager.LayoutParams.FLAG_FULLSCREEN
							| FLAG_FORBID_TRANSIENT_STATUS_BAR);
		}
	}

	public void closeStatusBar(Activity activity) {
		try {
			Object service = activity.getSystemService("statusbar");
			Class<?> statusbarManager = Class
					.forName("android.app.StatusBarManager");
			Method collapse = null;
			if (android.os.Build.VERSION.SDK_INT <= Build.VERSION_CODES.JELLY_BEAN) {
				collapse = statusbarManager.getMethod("collapse");
			} else {
				collapse = statusbarManager.getMethod("collapsePanels");
			}
			collapse.setAccessible(true);
			collapse.invoke(service);
		} catch (Exception ex) {
			ex.printStackTrace();
		}
	}
}
