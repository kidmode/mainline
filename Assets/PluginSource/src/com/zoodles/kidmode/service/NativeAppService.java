package com.zoodles.kidmode.service;

import static com.zoodles.kidmode.IntentConstants.*;

import com.onevcat.uniwebview.AndroidPlugin;
import com.zoodles.kidmode.App;
import com.zoodles.kidmode.IntentConstants;
import com.zoodles.kidmode.model.content.InstalledApp;
import com.zoodles.kidmode.util.ChildLock;

import java.util.List;
import java.util.concurrent.ScheduledThreadPoolExecutor;
import java.util.concurrent.TimeUnit;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.ActivityManager;
import android.app.PendingIntent;
import android.app.PendingIntent.CanceledException;
import android.app.Service;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.content.pm.PackageManager.NameNotFoundException;
import android.os.IBinder;
import android.os.Process;
import android.os.ResultReceiver;
import android.util.Log;

/**
 * Service that launches and terminates native apps.  Also provides
 * a background facility to monitor native apps and detect app launches
 * outside the Kid Mode app.  Including this functionality as a running
 * Service provides a side benefit: Android will try very hard to keep
 * Kid Mode alive, and kill our app only as a last resort.
 * 
 * This Service accepts three commands via the Intent extra
 * EXTRA_COMMAND:
 * 
 * CMD_START_APP	Launch a native app and watches in the background to
 * 					ensure it remains the frontmost app.  Takes as
 * 					Extra parameters the kid ID, the name of the package
 * 					and activity to launch, and a PendingIntent to invoke
 * 					if the app falls off the top of the activity stack.
 *					The background task remains active until the native
 *					app is preempted, or it is stopped explicitly by
 *					another Intent from the Activity.
 * 
 * CMD_STOP_APP		Stop watching a previously launched native app,
 * 					and kill it. Takes as Extra parameters the name of
 * 					the package to kill, and a ResultReceiver which is
 * 					invoked after the native app process is dead. The
 * 					background monitoring thread is guaranteed to be
 * 					dead when the ResultReceiver is invoked.
 * 
 * CMD_DIE			Stops watching any native app, kills background
 * 					monitoring thread. Takes no Extra parameters. At
 *                  completion, the service exits.
 * 
 */
@SuppressLint("NewApi")
public class NativeAppService extends Service {
	
	private final static String TAG = "NativeAppService";

	//////////////////////////////////////////////////////////////////////////
	// Constants
	//////////////////////////////////////////////////////////////////////////

	/**
	 * Status codes for return values
	 */
	public static final int RESULT_OK = 1;
	
	//////////////////////////////////////////////////////////////////////////
	// Background tasks
	//////////////////////////////////////////////////////////////////////////	
	
	protected Watchdog mWatchdog;
	
	protected ScheduledThreadPoolExecutor mScheduledExcutor = new ScheduledThreadPoolExecutor(1);
	//////////////////////////////////////////////////////////////////////////
	// Launchers
	//////////////////////////////////////////////////////////////////////////
	
	public static void startApp( Context context, int kidId, String packageName, 
			String activityName, PendingIntent pi, ResultReceiver receiver ) {
		Intent i = new Intent().setClass( context, NativeAppService.class );
		i.putExtra( EXTRA_COMMAND, CMD_START_APP );
		i.putExtra( EXTRA_KID_ID, kidId );
		i.putExtra( EXTRA_PACKAGE, packageName );
		i.putExtra( EXTRA_ACTIVITY, activityName );
		i.putExtra( EXTRA_PREEMPT_INTENT, pi );
		i.putExtra( EXTRA_STOP_RECEIVER, receiver );
		context.startService(i);
	}
	
	public static void stopApp( Context context, String packageName, ResultReceiver receiver ) {
		Intent i = new Intent().setClass( context, NativeAppService.class );
		i.putExtra( EXTRA_COMMAND, CMD_STOP_APP );
		i.putExtra( EXTRA_PACKAGE, packageName );
		i.putExtra( EXTRA_STOP_RECEIVER, receiver);
		
		context.startService( i );
	}
	
	public static void die( Context context ) {
		Intent i = new Intent().setClass( context, NativeAppService.class );
		i.putExtra( EXTRA_COMMAND, CMD_DIE );

		context.startService( i );
	}

	//////////////////////////////////////////////////////////////////////////
	// Service Lifecycle
	//////////////////////////////////////////////////////////////////////////
	
	@Override
	public void onCreate() {
	}
	
	@Override
	public int onStartCommand( Intent intent, int flags, int startId ) {
		String cmd = intent.getStringExtra( EXTRA_COMMAND );
		
		String packageName = intent.getStringExtra( EXTRA_PACKAGE );
		String activityName = intent.getStringExtra( EXTRA_ACTIVITY );
		int kidId = intent.getIntExtra( EXTRA_KID_ID, -1 ); 
		PendingIntent preemptIntent = intent.getParcelableExtra( EXTRA_PREEMPT_INTENT );
		ResultReceiver receiver = intent.getParcelableExtra( EXTRA_STOP_RECEIVER );		
		startApp( kidId, packageName, activityName, preemptIntent, receiver );
		
//		if ( CMD_START_APP.equals(cmd) ) {
//			String packageName = intent.getStringExtra( EXTRA_PACKAGE );
//			String activityName = intent.getStringExtra( EXTRA_ACTIVITY );
//			int kidId = intent.getIntExtra( EXTRA_KID_ID, -1 ); 
//			PendingIntent preemptIntent = intent.getParcelableExtra( EXTRA_PREEMPT_INTENT );
//			ResultReceiver receiver = intent.getParcelableExtra( EXTRA_STOP_RECEIVER );
//			
//			Log.d( TAG, "Command: Start " + packageName );
//			if ( kidId > 0 && packageName != null && preemptIntent != null ) {
//				startApp( kidId, packageName, activityName, preemptIntent, receiver );
//			}
//		} else if ( CMD_STOP_APP.equals(cmd) ) {
//			String packageName = intent.getStringExtra( EXTRA_PACKAGE );
//			ResultReceiver receiver = intent.getParcelableExtra( EXTRA_STOP_RECEIVER );
//
//			Log.d( TAG, "Command: Stop " + packageName );
//			if ( packageName != null ) {		
//				stopApp( packageName, receiver );
//			}
//		} else {
//			Log.d( TAG, "Command: Die" );
//			die();
//		}
//		
		return Service.START_NOT_STICKY;
	}
	
	@Override
	public void onDestroy() {
		super.onDestroy();
		killWatchdogThread();
	}

	//////////////////////////////////////////////////////////////////////////
	// Service API
	//////////////////////////////////////////////////////////////////////////	
	
	/**
	 * Launch native app, and start a background task to keep an eye on it.
	 * 
	 * @param packageName
	 * @param preemptIntent
	 */
	public void startApp(int kidId, String packageName, String activityName, PendingIntent preemptIntent, ResultReceiver receiver) 
	{
//		// Kill any previous watchdog thread
		killWatchdogThread();
//		
//		// Launch the game
		Intent i = null;
		try {
			i = new Intent( Intent.ACTION_MAIN );
			i.addCategory( Intent.CATEGORY_LAUNCHER );
			i.setComponent( new ComponentName( packageName, activityName ) );
			i.addFlags( Intent.FLAG_ACTIVITY_NEW_TASK );
//			i.addFlags( Intent.FLAG_ACTIVITY_CLEAR_TASK );
//			i.addFlags( Intent.FLAG_ACTIVITY_EXCLUDE_FROM_RECENTS );
//			i.addFlags( Intent.FLAG_ACTIVITY_MULTIPLE_TASK );
			((App) getApplicationContext()).childLock().allowedAppIsRunning();
//			Log.d( TAG, "starting activity for " + packageName );
			startActivity( i );
		} catch( Exception e ) {
//			Log.e( TAG, "Error launching package: " + packageName, e );
		}
//		
		startWatchdogThread( kidId, packageName, preemptIntent, receiver );
	}
	
	/**
	 * Stop watching native app, and kill its process.
	 * 
	 * @param packageName
	 * @param receiver 			invoked after native app is dead
	 */
	public void stopApp( String packageName, ResultReceiver receiver ) {
		// Kill off the game at some point in the near future
		mScheduledExcutor.schedule(new KillGameTask(packageName, receiver), 1, TimeUnit.SECONDS);

		// Ensure the watchdog thread is dead
		killWatchdogThread();
	}
	
	/**
	 * Kills service on request from Activity.
	 */
	public void die() {
		killWatchdogThread();
		stopSelf();
	}
	
	//////////////////////////////////////////////////////////////////////////
	// Watchdog Thread
	//////////////////////////////////////////////////////////////////////////
	
	/**
	 * Thread to check Android Task/Activity stack and make sure child hasn't
	 * busted out to another app.
	 */
	protected class Watchdog extends Thread {
		
		//////////////////////////////////////////////////////////////////////////
		// Parameters
		//////////////////////////////////////////////////////////////////////////
		
		private int mKidId;
		private String mPackageName;
		private PendingIntent mPreemptIntent;
		private ResultReceiver mReceiver;
		
		private boolean mWasTop;
		
		//////////////////////////////////////////////////////////////////////////
		// State
		//////////////////////////////////////////////////////////////////////////
		
		private int mTaskId;
		private int mKidModeTaskId;
		private List<InstalledApp> approvedApps;

		//////////////////////////////////////////////////////////////////////////
		// Constructor
		//////////////////////////////////////////////////////////////////////////
		
		public Watchdog( int kidId, String packageName, PendingIntent preemptIntent, ResultReceiver receiver ) {
			super();
			mTaskId = -1;
			mKidModeTaskId = -1;
			approvedApps = null;

			mKidId = kidId;
			mPackageName = packageName;
			mPreemptIntent = preemptIntent;
			mReceiver = receiver;
			
			mWasTop = false;
		}
		
		//////////////////////////////////////////////////////////////////////////
		// Thread Body
		//////////////////////////////////////////////////////////////////////////
		
		private boolean wasFront = false;
		private int m_time = 0;
		@Override
		public void run() 
		{
			while(!shouldExit()) 
			{
				try 
				{
					sleepTask();
				} 
				catch (InterruptedException e)
				{ 
					break; 
				}			
				
				boolean l_isKidmodeTopTask = isKidmodeTopTask();
				boolean l_isExternalAppTopTask = isExternalAppTopTask();
				boolean l_finished = false;
				
				if (!l_isExternalAppTopTask
					&& !l_isKidmodeTopTask)
				{
//					Log.v("Zoodles", "We aren't foreground anymore mPackageName:" + mPackageName);
					((App) getApplicationContext()).childLock().allowedAppStopped();
					forceKidModeToTop( mPreemptIntent, "" );
					mWasTop = true;
				}						
				
				if (mWasTop)
				{
					if (l_isKidmodeTopTask)
					{
						if (isKidmodeRunningInFront())
						{
							if (wasFront)
							{
								try
								{
									l_finished = true;
									Log.v("Zoodles", "Service Finished");
								}
								catch(Exception e) {}
							}
							wasFront = true;
						}
					}
				}
//				
				if (l_finished)
				{
					break;
				}				
			}
			
//			try {
//				App app = (App) getApplicationContext();
//				ActivityManager activityManager = (ActivityManager) 
//					getApplicationContext().getSystemService(ACTIVITY_SERVICE);
//				
//				boolean gameAppeared = false;
//				
//				mTaskId = findTaskId( mPackageName );
//				mKidModeTaskId = findTaskId( app.getPackageName() );
//				
//				loadApprovedApps();
//	
//				// Look once per second to see if another app has popped to the top.
//				while ( !shouldExit() ) {
//					Log.d( TAG, "Tick" );
//	
//					try {
//						sleepTask();
//					} catch ( InterruptedException e ) { break; }				
//	
//					if ( shouldExit() ) break;
//					
//					// Look to see if the topmost task has changed.
//					List<ActivityManager.RunningTaskInfo> runningTasks = activityManager.getRunningTasks(1);
//					if ( runningTasks == null || runningTasks.size() == 0 ) {
//						// Bombproof against an empty response from the OS
//						continue;
//					}
//					
//					ActivityManager.RunningTaskInfo topTask = activityManager.getRunningTasks(1).get(0);
//					String topPackageName = topTask.topActivity.getPackageName();
//					if ( topTask.id != mKidModeTaskId && !packageApproved( topPackageName ) ) {
//						Log.d( TAG, "The topmost task is something other than the game or Kid Mode, and isn't whitelisted." );
//						forceKidModeToTop( mPreemptIntent, topPackageName );
//						break;
//					}
//					
//					if ( shouldExit() ) break;
//	
//					// Address a corner case - Game is slow to load and has not yet appeared, before
//					// this thread has had a chance to start inspecting task stack.
//					if ( topTask.id == mTaskId ) {
//						gameAppeared = true;
//					} 
//					
//					if (mTaskId == -1 && !gameAppeared) {
//						//when we cannot find mTaskId from base activity, and game has not appeared yet
//						ActivityManager.RunningTaskInfo taskWithTopActivityMatch = taskForPackageOnTop(mPackageName);
//						if (taskWithTopActivityMatch != null) {
//							gameAppeared = true;
//						}
//					}
//					
//					// Exit thread if Kid Mode becomes visible once again, after game has
//					// initially appeared.
//					if ( topTask.id == mKidModeTaskId && gameAppeared ) {
//						break;
//					}
//					
//					if ( shouldExit() ) break;
//					
//					// Enforce maximum volume for native app
//					//AudioUtils.enforceMaxVolume( app, app.preferences().maxVolume() );
//				}
//			} finally {
//				//when watch dog ends, allowed apps ends
//				((App) getApplicationContext()).childLock().allowedAppStopped();
//				Log.d( TAG, "Watch dog ends, signal allowed app ends" );
//				
//				// in case PlaygroundActivity#onResume() is called before allowdApp flag turns off,
//				// kill the background package one more time
//				// Also, double killing in PlaygroundActivity#onResume() make sure that process goes to 
//				// background when it is killed.
//				mScheduledExcutor.schedule(new KillGameTask(mPackageName, mReceiver), 1, TimeUnit.SECONDS);
//			}
		}
		
		public boolean isExternalAppTopTask()
		{
			ActivityManager activityManager = (ActivityManager) getApplicationContext().getSystemService(ACTIVITY_SERVICE);
			List<ActivityManager.RunningAppProcessInfo> processes = activityManager.getRunningAppProcesses();
			
			for ( ActivityManager.RunningAppProcessInfo process : processes ) 
			{
				if (process.processName.contains(mPackageName))
				{
					if (process.importance == process.IMPORTANCE_FOREGROUND)
					{
						return true;
					}						
				}
			}			
			return false;
		}
		
		public boolean isKidmodeTopTask()
		{
			ActivityManager activityManager = (ActivityManager) getApplicationContext().getSystemService(ACTIVITY_SERVICE);
			List<ActivityManager.RunningAppProcessInfo> processes = activityManager.getRunningAppProcesses();
			
			for ( ActivityManager.RunningAppProcessInfo process : processes ) 
			{
				if (process.processName.contains("com.zoodles.kidmode"))
				{
					if (process.importance == process.IMPORTANCE_FOREGROUND)
					{
						return true;
					}
				}
			}
			return false;
		}
		
		//////////////////////////////////////////////////////////////////////////
		// Thread Control
		//////////////////////////////////////////////////////////////////////////
		
		public boolean isKidmodeRunningInFront()
		{
			Activity l_currentActivity = com.unity3d.player.UnityPlayer.currentActivity;
			AndroidPlugin l_plugin = (AndroidPlugin) l_currentActivity;
			if (null != l_plugin)
			{
				if (l_plugin.hasWindowFocus())
				{
					return l_plugin.isInForeground();
				}
			}
			return false;
		}
		
		public boolean shouldExit() {
			return Thread.currentThread().isInterrupted();
		}
		
		//////////////////////////////////////////////////////////////////////////
		// Task Management
		//////////////////////////////////////////////////////////////////////////
		
		/**
		 * Get a list of the approved apps for the current child.
		 */
		private void loadApprovedApps() {
//			if ( mKidId > 0 ) {
//				approvedApps = App.instance().database().
//						getGamesTable().findNativeGamesByKidId( mKidId );
//			}
		}
			
		/**
		 * Force the Kid Mode Playground Activity to top of activity stack.
		 */
		private void forceKidModeToTop( PendingIntent preemptIntent, String offendingPackageName ) {

			// Get the human-readable name of the offending package.
			CharSequence label = null;
			PackageManager pm = getPackageManager();
			ApplicationInfo info = null;
			try {
				info = pm.getApplicationInfo( offendingPackageName, 0 );
				if ( info != null ) {
					label = pm.getApplicationLabel( info );
				}
			} catch ( NameNotFoundException ignore ) {;}
			
			try {
//				Log.d( TAG, "Pushing Kid Mode to top." );
				Intent i = new Intent();
				i.putExtra( IntentConstants.EXTRA_PREEMPT_APP, label );
				i.putExtra( IntentConstants.EXTRA_PREEMPT_APP_PKG, offendingPackageName );
				preemptIntent.send( NativeAppService.this, 0, i );
				
				ActivityManager activityManager = (ActivityManager) getApplicationContext().getSystemService(ACTIVITY_SERVICE);
				int taskId = findTaskId( "com.zoodles.kidmode" );				
				activityManager.moveTaskToFront(taskId, ActivityManager.MOVE_TASK_WITH_HOME);
				
			} catch ( Exception ignore ) {;}
		}
		
		/**
		 * Returns TRUE if the package is one that was approved by parent.
		 * 
		 * @param packageName
		 * @return
		 */
		private boolean packageApproved( String packageName ) {
			return false;
			
//			if ( packageName == null ) return false;
//			if ( mPackageName == null ) return true;
//			
//			App application = ((App) getApplicationContext());
//			Preferences prefs = application.preferences();
//
//			// A package we launched
//			if ( packageName.equals(mPackageName) ) {
//				return true;
//			}
//			
//			// Kid Mode itself
//			if ( application.getPackageName().startsWith( mPackageName ) ) {
//				return true;
//			}
//			
//			// If the user wants incoming calls, exclude any dialer apps.
//			if ( prefs.incomingCallsEnabled() ) {
//				PackageManager pm = application.getPackageManager();
//				PackageInfo info;
//				try {
//					info = pm.getPackageInfo(packageName, PackageManager.GET_PERMISSIONS);
//					String[] requestedPermissions = info.requestedPermissions;
//					
//					if ( requestedPermissions != null ) {
//						int permissionCount = 0;						
//						for ( int i = 0; i < requestedPermissions.length; i++ ) {
//							if ( requestedPermissions[i].equals("android.permission.CALL_PHONE")) {
//								permissionCount++;
//							}
//							if ( requestedPermissions[i].equals("android.permission.CALL_PRIVILEGED")) {
//								permissionCount++;
//							}							
//							if ( requestedPermissions[i].equals("android.permission.READ_PHONE_STATE")) {
//								permissionCount++;
//							}
//							if ( requestedPermissions[i].equals("android.permission.MODIFY_PHONE_STATE")) {
//								permissionCount++;
//							}
//							if ( requestedPermissions[i].equals("android.permission.RECORD_AUDIO")) {
//								permissionCount++;
//							}
//						}
//						if ( permissionCount == 5 ) { return true; }
//					}
//					
//				} catch (NameNotFoundException ignore) {;}
//			}
//			
//			// Check against the parent's whitelist
//			if ( approvedApps == null ) { return false; }
//			for ( int i = 0; i < approvedApps.size(); i++ ) {
//				NativeApp app = approvedApps.get(i);
//				String appPackageName = app.getPackage();
//				if ( ((appPackageName == null) ? false : appPackageName.equals(packageName)) ) {
//					return true;
//				}
//			}
//			
//			return false;
		}
	}
	
	protected class KillGameTask extends Thread {
		private String mPackageName;
		private ResultReceiver mReceiver;
		
		public KillGameTask(String packageName, ResultReceiver receiver) {
			mPackageName = packageName;
			mReceiver = receiver;
		}
		
		@Override
		public void run() {
			killBackgroundGames(mPackageName, mReceiver);
		}
	}
		
	//////////////////////////////////////////////////////////////////////////
	// Helpers
	//////////////////////////////////////////////////////////////////////////
	
	/**
	 * Find the task ID for the named package
	 */
	private int findTaskId( String packageName ) {
		ActivityManager.RunningTaskInfo task = taskForPackageOnBase( packageName );
		if ( task != null ) {
			return task.id;
		}
		
		return -1;
	}
	
	/**
	 * Return the task record that has base activity matches packageName 
	 * 
	 * @param packageName
	 * @return
	 */
	private ActivityManager.RunningTaskInfo taskForPackageOnBase( String packageName ) {
		return taskForPackage(packageName, true);
	}
	
	/**
	 * Return the task record that has top activity matches packageName 
	 * 
	 * @param packageName
	 * @return
	 */
	private ActivityManager.RunningTaskInfo taskForPackageOnTop( String packageName ) {
		return taskForPackage(packageName, false);
	}
	
	/**
	 * Return the task record matching packageName either on its base activity or top activity
	 * @param packageName package name to match
	 * @param isOnBase true when match with base activity; false when match with top activity
	 * @return Task info object
	 */
	private ActivityManager.RunningTaskInfo taskForPackage( String packageName, boolean isOnBase) {
		if ( packageName == null ) return null;

		ActivityManager activityManager = (ActivityManager) getApplicationContext().getSystemService(ACTIVITY_SERVICE);
		List<ActivityManager.RunningTaskInfo> tasks = activityManager.getRunningTasks(10);
		if ( tasks == null ) { return null; }

		for ( ActivityManager.RunningTaskInfo task : tasks ) {
			ComponentName activityName = isOnBase ? task.baseActivity : task.topActivity;
			String taskPackageName = activityName.getPackageName();
			if ( taskPackageName != null && taskPackageName.startsWith( packageName ) ) {
				return task;
			}
		}
		
		return null;		
	}
	
	
	/**
	 * Return the process ID corresponding to the native app we just launched
	 */
	private int findNativeAppProcessId( String packageName ) {
		if ( packageName == null ) return -1;

		ActivityManager activityManager = (ActivityManager) getApplicationContext().getSystemService(ACTIVITY_SERVICE);
		List<ActivityManager.RunningAppProcessInfo> processes = activityManager.getRunningAppProcesses();
		if ( processes == null ) { return -1; }
		
		for ( ActivityManager.RunningAppProcessInfo process : processes ) {
			String [] packages = process.pkgList;
			for ( int i = 0; i < packages.length; i++ ) {
				if ( packages[i].startsWith( packageName ) ) {
					return process.pid;
				}
			}
		}
		
		return -1;
	}
	
	/**
	 * Launch the watchdog thread
	 * 
	 * @param kidId
	 * @param packageName
	 * @param preemptIntent
	 */
	private void startWatchdogThread( int kidId, String packageName, PendingIntent preemptIntent, ResultReceiver receiver ) {
		mWatchdog = new Watchdog( kidId, packageName, preemptIntent, receiver );
		mWatchdog.start();
	}
	
	/**
	 * Kills the watchdog task and waits around to make sure the sucker is dead
	 */
	private void killWatchdogThread() {
		if ( mWatchdog != null && mWatchdog.isAlive() ) {
			if ( !mWatchdog.isInterrupted() ) {
				mWatchdog.interrupt();
			}
			try {
				// Wait for it to die, die, die
				mWatchdog.join();
			} catch ( InterruptedException ignore ) {;}
		}
		mWatchdog = null;
	}
	
	/**
	 * May be overridden by test cases
	 * @throws InterruptedException
	 */
	protected void sleepTask() throws InterruptedException {
		Thread.sleep( 1000 );
	}
	
	/**
	 * Boilerplate.
	 */
	@Override
	public IBinder onBind(Intent arg0) {
		return null;
	}
	
	private void killBackgroundGames(String packageName, ResultReceiver receiver) {
		murderAllGameProcesses( packageName );
		notifyActivityGameIsDead( receiver );		
	}

	//////////////////////////////////////////////////////////////////////////
	// Helpers
	//////////////////////////////////////////////////////////////////////////	
	
	/**
	 * Find and Kill all the processes in the Native App's task.  At least, all
	 * the ones that we can find out about.
	 */
	private void murderAllGameProcesses( String packageName ) {
		ActivityManager.RunningTaskInfo task = taskForPackageOnBase( packageName );
		if ( task == null ) {
			task = taskForPackageOnTop( packageName );
		}
		if ( task == null ) { return; }
		
		if ( task.baseActivity != null ) {
//			Log.d( TAG, "Killing base activity: " +  task.baseActivity.getPackageName() );
			murderGameProcess( task.baseActivity.getPackageName() );
		}
		if ( task.topActivity != null ) {
//			Log.d( TAG, "Killing top activity: " +  task.topActivity.getPackageName() );
			murderGameProcess( task.topActivity.getPackageName() );
		}
	}
	
	/**
	 * Kill the process for the named package.
	 * 
	 * NOTE: Will work only if the game is running completely in the background.
	 * 
	 * See:
	 * http://stackoverflow.com/questions/4503277/ android-close-other-apps
	 * http://stackoverflow.com/questions/3826374/difference-between-a-task-killer-killing-an-app-and-the-android-os-killing-an-app
	 * (Note answers on both by hackbod, aka Dianne Hackborn of Google Android team)
	 * http://stackoverflow.com/questions/4154210/android-how-to-kill-programmatically-the-camera-activity-running-in-background
	 */
	private void murderGameProcess( String packageName ) {
		ActivityManager activityManager = (ActivityManager) getApplicationContext().getSystemService(ACTIVITY_SERVICE);
		int processId = findNativeAppProcessId( packageName );
		if ( processId > 0 ) {
//			Log.d( TAG, "Killing process ID for package: " +  processId + " " +  packageName );
			activityManager.killBackgroundProcesses( packageName );
			Process.sendSignal( processId, android.os.Process.SIGNAL_KILL );
		}
	}
	
	/**
	 * Tell the Activity that the game is dead.
	 */
	private void notifyActivityGameIsDead( ResultReceiver receiver ) {
		if ( receiver == null ) return;
		
//		Log.d( TAG, "notifying activity game is dead." );
		receiver.send( RESULT_OK, null );
	}
}
