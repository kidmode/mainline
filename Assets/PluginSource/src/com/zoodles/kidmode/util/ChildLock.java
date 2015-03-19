package com.zoodles.kidmode.util;

import java.util.ArrayList;
import java.util.List;

import android.app.Activity;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.telephony.PhoneStateListener;
import android.telephony.TelephonyManager;
import android.util.Log;

import com.zoodles.kidmode.App;
import com.zoodles.kidmode.ZoodlesHome;

/**
 * Class maintaining current state of the "Child Lock" feature, both internally
 * and system-wide.
 * 
 */
public class ChildLock {

	private PackageManager mPm = null;
	private ResolveInfo mInfo = null;

	private static final String TAG = "ChildLock";

	/**
	 * Global flag that activities can use to check to see if they are being
	 * paused as a result of an in-application Intent, versus something else on
	 * the system removing them from the top of the activity stack.
	 */
	protected boolean mPreemptionFlag;

	/**
	 * Global flag, set to TRUE when the device is in the middle of a phone
	 * call. Used to allow phone calls through when pre-emption checking is
	 * enabled.
	 */
	protected boolean mInCall;

	/**
	 * Global flag, used to indicate that Kid Mode is in the middle of a play
	 * session with Child Lock activated
	 */
	protected boolean mInChildLock;

	/**
	 * Retry count, used to count the number of times parent tries to set Child
	 * Lock before successfully completing the task.
	 */
	protected int mAttemptCount;

	/**
	 * Flag to indicate whether allowed App is running on the front or not. When
	 * allowed App is running, we should stop preemption action, even if Kid
	 * Mode playground activity's onStart() is called temporarily
	 * 
	 */
	protected boolean mInAppRunning;

	/**
	 * Used to detect incoming phone calls.
	 */
	private IncomingCallListener mPhoneListener;

	// ////////////////////////////////////////////////////////////////////////
	// Public
	// ////////////////////////////////////////////////////////////////////////

	public ChildLock() {
		mInCall = false;
		mInChildLock = false;
		mPreemptionFlag = false;
		mPhoneListener = null;
		clearAttemptCount();
	}

	// ////////////////////////////////////////////////////////////////////
	// Activity relauncher support
	// ////////////////////////////////////////////////////////////////////

	/**
	 * Invoked before launching another Kid Mode Activity. Disables pre-emption
	 * checking for the next activity.
	 */
	public void stopWatchForPreemption() {
		mPreemptionFlag = false;
	}

	/**
	 * Invoked when a Kid Mode Activity is successfully started. Allows
	 * detection of a new activity that was not launched by Kid Mode.
	 */
	public void startWatchForPreemption() {
		mPreemptionFlag = true;
	}

	/**
	 * Invoked when an allowed app is going to run, so that we don't need to do
	 * preemption protection
	 */
	public void allowedAppIsRunning() 
	{
		mInAppRunning = true;
	}

	/**
	 * Invoked when an allowed app cannot be on the front any more, so that we
	 * need to do preemption protection
	 */
	public void allowedAppStopped() {
		mInAppRunning = false;
	}

	public boolean isAllowedAppRunning() {
		return mInAppRunning;
	}

	/**
	 * Returns TRUE if we are in a pre-emption check state. Whether or not Child
	 * Lock is currently active is not considered.
	 * 
	 * @return
	 */
	public boolean isPreempted() 
	{
		return mPreemptionFlag && !mInCall && !mInAppRunning;
	}

	public boolean preemptionFlag() {
		return mPreemptionFlag;
	}

	/**
	 * Set a flag indicating we are in a phone call. This can be used to
	 * short-circuit pre-emption checks.
	 * 
	 * @param call
	 */
	public void inCall(boolean call) 
	{
		mInCall = call;
	}

	public boolean callInProgress() {
		return mInCall;
	}

	// ////////////////////////////////////////////////////////////////////
	// Child Lock flag
	// ////////////////////////////////////////////////////////////////////

	/**
	 * Returns TRUE if the Child Lock is currently active (home button is
	 * actively grabbed)
	 */
	public boolean inChildLock() {
		return mInChildLock;
	}

	public void setInChildLock(boolean k) {
		mInChildLock = k;
	}

	// ////////////////////////////////////////////////////////////////////
	// Child Lock attempt counter
	// ////////////////////////////////////////////////////////////////////

	public void incrementAttemptCount() {
		mAttemptCount++;
	}

	public int getAttemptCount() {
		return mAttemptCount;
	}

	public void clearAttemptCount() {
		mAttemptCount = 0;
	}

	// ////////////////////////////////////////////////////////////////////
	// Home Button Locking Support
	// ////////////////////////////////////////////////////////////////////

	/*
	 * Most public API calls take the application Context as a parameter. In
	 * actual use, this is the application instance. This is done to avoid
	 * holding dangling references to the application, and creating memory
	 * leaks.
	 */

	/**
	 * Checks if ZoodlesHome activity has taken over as the default for Home
	 * 
	 * @return true, if Kid Mode is now home app, false if not.
	 * 
	 *         initializes
	 */
	public boolean isDefault(Context appContext) {
		ensurePackageManager(appContext);
		String appName = getHomeAppName(appContext);
		if (appName == null) {
			return false;
		}
		return appName.contains(getZoodlesAppName(appContext));
	}

	/**
	 * Decide whether or not Kid Mode is currently the default home launcher (
	 * hold the sticky bit ).
	 * 
	 * Original post:
	 * http://stackoverflow.com/questions/8299427/how-to-check-if-
	 * my-application-is-the-default-launcher
	 * 
	 * @return True is Kid Mode is currently the default home launcher, false
	 *         otherwise.
	 */
	public boolean isDefaultHomeLauncher(Context appContext) {
		ensurePackageManager(appContext);

		final IntentFilter filter = new IntentFilter(Intent.ACTION_MAIN);
		filter.addCategory(Intent.CATEGORY_HOME);

		List<IntentFilter> filters = new ArrayList<IntentFilter>();
		filters.add(filter);

		final String kidModePackageName = appContext.getPackageName();
		List<ComponentName> activities = new ArrayList<ComponentName>();

		mPm.getPreferredActivities(filters, activities, kidModePackageName);

		for (ComponentName activity : activities) {
			if (kidModePackageName.equals(activity.getPackageName())) {
				return true;
			}
		}

		return false;
	}

	/**
	 * Checks if there are multiple choices for home button intent
	 * 
	 * @return TRUE if there are multiple choices, FALSE if not.
	 */
	public boolean hasDifferentDefault(Context appContext) {
		return (!isDefault(appContext) && !hasMultipleChoices());
	}

	/**
	 * Register this application as a respondent of the Home button intent.
	 */
	public void setupHomeReplacement(App appContext) {
		ensurePackageManager(appContext);
		ComponentName component = new ComponentName(
				appContext.getPackageName(), ZoodlesHome.class.getName());
		Log.v(TAG, "Enabling Zoodles HOME");
		mPm.setComponentEnabledSetting(component,
				PackageManager.COMPONENT_ENABLED_STATE_ENABLED,
				PackageManager.DONT_KILL_APP);

		try {
			//
			// We need to check to see if we have Home here, but doing so causes
			// a
			// bizarre side effect (crash?) that sends us back to the app
			// launcher if we
			// invoke isDefault...
			// See mods in
			// https://github.com/zoodles/android/commit/a286e9b597601a9bab528ca21539dd34ab2ed613#KidMode
			// that got us into this mess.
			//
			setInChildLock(true);
			listenForIncomingCalls();
		} catch (Exception e) {
			// It's very unlikely something went wrong, but if it did, try
			// undoing it.
			// (There is a bug on Samsung Galaxy S devices running 2.1, which
			// will cause the
			// Home Intent resolution to toss a NPE. At that point, the device
			// is bricked.
			// This is an attempt to defuse the issue.)
			Log.v("Zoodles", "Error found + " + e.getMessage());
			try {
				removeHomeReplacement(appContext);
			} catch (Exception ignore) {
				;
			}
			setInChildLock(false);
			stopListeningForIncomingCalls();
		}
	}

	/**
	 * De-register this application as a respondent of the Home button intent.
	 */
	public void removeHomeReplacement(App appContext) {
		ensurePackageManager(appContext);
		ComponentName component = new ComponentName(
				appContext.getPackageName(), ZoodlesHome.class.getName());
		Log.v(TAG, "Disabling Zoodles HOME");
		mPm.setComponentEnabledSetting(component,
				PackageManager.COMPONENT_ENABLED_STATE_DISABLED,
				PackageManager.DONT_KILL_APP);

		setInChildLock(false);
		stopListeningForIncomingCalls();
	}

	/**
	 * Get all apps that respond to the home button intent.
	 * 
	 * @return
	 */
	public List<ResolveInfo> getAllHomeButtonApps(Context appContext) {
		ensurePackageManager(appContext);
		List<ResolveInfo> responders = mPm.queryIntentActivities(
				buildHomeIntent(), PackageManager.MATCH_DEFAULT_ONLY);
		return responders;
	}

	/**
	 * Send an intent to launch the Home app. If this app is registered as a
	 * Home app, this should either invoke our Home activity, or cause the
	 * system to post a resolver dialog to the user.
	 * 
	 * @param activity
	 *            Launching Activity.
	 * @param inSignup
	 *            TRUE if we are launched as part of the signup flow.
	 */
	public void sendHomeIntent(Activity activity, int flag) {
		Intent i = buildHomeIntent();
		// Log.d(TAG, "Sending Home intent");
		// i.putExtra( IntentConstants.EXTRA_CL_FLOW, true );
		// i.putExtra( IntentConstants.EXTRA_FLAG, flag );
		//
		// if ( (flag & ChildLockFragment.FLAG_FIRST_RUN) != 0 ) {
		// i.putExtra( IntentConstants.EXTRA_CL_FIRST_RUN, true );
		// }
		activity.startActivity(i);
	}

	/**
	 * Send a home intent as a way of exiting the app. Not intended for use in
	 * the Child Lock flow.
	 * 
	 * @param activity
	 */
	public void sendHomeIntent(Activity activity) {
		Intent i = buildHomeIntent();
		Log.d(TAG, "Sending Home intent");
		activity.startActivity(i);
	}

	// ////////////////////////////////////////////////////////////////////////
	// Private
	// ////////////////////////////////////////////////////////////////////////

	// ////////////////////////////////////////////////////////////////////
	// Incoming Calls
	// ////////////////////////////////////////////////////////////////////

	/**
	 * Watch for and allow incoming calls when Child Lock is enabled
	 */
	public void listenForIncomingCalls() {
		App app = App.instance();

		// remove previous listener
		if (mPhoneListener != null) {
			TelephonyManager phoneMgr = (TelephonyManager) app
					.getSystemService(Context.TELEPHONY_SERVICE);
			phoneMgr.listen(mPhoneListener, PhoneStateListener.LISTEN_NONE);

			mPhoneListener = null;

			inCall(false);
		}

		mPhoneListener = new IncomingCallListener();
		TelephonyManager phoneMgr = (TelephonyManager) app
				.getSystemService(Context.TELEPHONY_SERVICE);
		phoneMgr.listen(mPhoneListener, PhoneStateListener.LISTEN_CALL_STATE);
	}

	/**
	 * Stop watching for incoming calls when Child Lock is disabled
	 */
	protected void stopListeningForIncomingCalls() {
		if (mPhoneListener == null) {
			return;
		} // not registered

		App app = App.instance();
		mPhoneListener = null;
		TelephonyManager phoneMgr = (TelephonyManager) app
				.getSystemService(Context.TELEPHONY_SERVICE);
		phoneMgr.listen(mPhoneListener, PhoneStateListener.LISTEN_NONE);
	}

	private class IncomingCallListener extends PhoneStateListener {
		@Override
		public void onCallStateChanged(int state, String incomingNumber) {
			if (state != TelephonyManager.CALL_STATE_IDLE) {
//				App app = App.instance();
				if (state == TelephonyManager.CALL_STATE_RINGING) {
//					app.pauseLockScreenService();
				} else {
//					app.stopLockScreenService();
				}
				inCall(true);
			}
		}
	}

	// ////////////////////////////////////////////////////////////////////
	// Home Intent
	// ////////////////////////////////////////////////////////////////////

	/**
	 * Checks if there are multiple choices for home button intent
	 * 
	 * @return true, if there are multiple choices, false if not.
	 */
	private boolean hasMultipleChoices() {
		String homeAppName = getHomeActivityName();
		return homeAppName != null && homeAppName.contains("ResolverActivity");
	}

	/**
	 * Returns the name of the app that currently owns home.
	 * 
	 * @return
	 */
	private String getHomeAppName(Context appContext) {
		getHomeButtonInfo(appContext);
		if (mInfo == null || mInfo.activityInfo == null) {
			return null;
		}
		CharSequence appName = mPm
				.getApplicationLabel(mInfo.activityInfo.applicationInfo);
		Log.d(TAG, "Current home app is: " + appName);

		return appName.toString();
	}

	/**
	 * Get information about the current default owner of the home button.
	 */
	protected ResolveInfo getHomeButtonInfo(Context appContext) {
		ensurePackageManager(appContext);
		mInfo = mPm.resolveActivity(buildHomeIntent(),
				PackageManager.MATCH_DEFAULT_ONLY);
		return mInfo;
	}

	/**
	 * Returns the name of the activity that currently owns home.
	 * 
	 * @return
	 */
	private String getHomeActivityName() {
		if (mInfo == null || mInfo.activityInfo == null) {
			return null;
		}

		CharSequence activityName = mInfo.activityInfo.toString();
		Log.d(TAG, "Current home activity is: " + activityName);

		return activityName.toString();
	}

	/**
	 * Returns the name of this application.
	 * 
	 * @return
	 */
	private String getZoodlesAppName(Context appContext) {
		CharSequence appName = mPm.getApplicationLabel(appContext
				.getApplicationInfo());
		if (appName == null) {
			return null;
		}
		return appName.toString();
	}

	/**
	 * Construct an intent for launching the home app.
	 * 
	 * @return
	 */
	private Intent buildHomeIntent() {
		Intent i = new Intent();
		i.setAction(Intent.ACTION_MAIN);
		i.addCategory(Intent.CATEGORY_HOME);
		return i;
	}

	/**
	 * Initialize our instance of the PackageManager on demand.
	 * 
	 * @param applicationContext
	 */
	private void ensurePackageManager(Context appContext) {
		if (mPm == null) {
			mPm = appContext.getPackageManager();
		}
	}
}