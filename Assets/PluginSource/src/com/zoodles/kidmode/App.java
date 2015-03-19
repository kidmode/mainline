package com.zoodles.kidmode;

import android.app.Application;
import android.content.*;
import android.graphics.Bitmap;
import android.graphics.PixelFormat;

import android.os.IBinder;
import android.util.Log;

import com.zoodles.kidmode.features.DeviceInfo;
import com.zoodles.kidmode.service.GlobalTouchDenyService;
import com.zoodles.kidmode.util.ChildLock;

/**
 * Subclass of Android Application object to manage application globals.
 * 
 */
public class App extends Application {
	private static final String TAG = "AppTag";

	//////////////////////////////////////////////////////////////////////
	// Constants
	//////////////////////////////////////////////////////////////////////
	
	/**
	 * XML schema for custom attributes in custom view classes.
	 * 
	 * http://stackoverflow.com/questions/6811431/android-custom-attributes-on-normal-views
	 * https://gist.github.com/1105281
	 */
	public static final String KID_MODE_XMLNS = "http://com.zoodles.kidmode/schema";
		
	//////////////////////////////////////////////////////////////////////
	// Singleton
	//////////////////////////////////////////////////////////////////////

	protected static App sInstance = null;
	public    static App instance() { return sInstance; }

	protected DeviceInfo mDeviceInfo;
	protected ChildLock mChildLock;
	protected Thread.UncaughtExceptionHandler mSystemExceptionHandler;
	protected ScreenReceiver mScreenReceiver;
	protected Preferences mPrefs;
	private Intent globalTouchDenyIntent;
	private boolean pauseLockScreenService = false;
	//////////////////////////////////////////////////////////////////////
	// App State
	//////////////////////////////////////////////////////////////////////
	
	/**
	 * If we altered the volume settings on launch, store the original
	 * volume here so that we can restore it on exit.
	 */
	protected int mSavedVolume;

	//////////////////////////////////////////////////////////////////////
	// App Lifecycle
	//////////////////////////////////////////////////////////////////////


	@Override
	public void onCreate() {
		super.onCreate();	
		setInstance(this);
		Log.d(TAG, "onCreate()");
	}

	public static synchronized App getInstance(Context context) {
		Log.d(TAG, "getInstance(Context context)");
		if (sInstance == null) {
			App appObj = (App)context.getApplicationContext();
			setInstance(appObj);
		}
		return sInstance;
	}
	
	private static synchronized void setInstance(App appObj) {
		if (sInstance == null) {
			sInstance = appObj;
			sInstance.initializeInstance();
		}
	}
	
	protected void initializeInstance() {
		mSavedVolume = -1;
		
		// Listen for screen on/off events so we don't play audio when the screen is
		// off or locked
        mScreenReceiver = new ScreenReceiver();
        IntentFilter filter = new IntentFilter(Intent.ACTION_SCREEN_ON);
        filter.addAction(Intent.ACTION_SCREEN_OFF);
        registerReceiver(mScreenReceiver, filter);

//		globalTouchDenyIntent = new Intent(this.getApplicationContext(),GlobalTouchDenyService.class);
//
//		bindService(globalTouchDenyIntent, mConnection, Context.BIND_AUTO_CREATE);
//		startService(globalTouchDenyIntent);
		
//		Log.v("Zoodles", "starting binding globaltouchdenyintent");
	}

	private GlobalTouchDenyService mTouchDenyService;
	private boolean mTouchDenyBound;
	/**
	 * Defines callbacks for service binding, passed to bindService()
	 */
	private ServiceConnection mConnection = new ServiceConnection() {

		@Override
		public void onServiceConnected(ComponentName className,
		                               IBinder service) {
			// We've bound to LocalService, cast the IBinder and get LocalService instance
			GlobalTouchDenyService.LocalBinder binder = (GlobalTouchDenyService.LocalBinder) service;
			if(binder==null){
				return;
			}
			mTouchDenyService = binder.getService();
			mTouchDenyBound = true;
		}

		@Override
		public void onServiceDisconnected(ComponentName arg0) {
			mTouchDenyBound = false;
		}
	};

	public void startWatcherLockScreenService() 
	{
		Log.v("Zoodles", "startLockSCreenServiceCheck: " + mChildLock.inChildLock() + " " + (!pauseLockScreenService));
		if (mChildLock.inChildLock() && !pauseLockScreenService) 
		{
			Log.v("Zoodles", "In child lock and pause screen service confirmed");
			synchronized (globalTouchDenyIntent)
			{
				Log.v("Zoodles", "Touch deny intent synchronized");
				if(mTouchDenyBound)
				{
					Log.d(TAG, "startWatcherLockScreenService");
					mTouchDenyService.startWatcherTouchDenyService();
				}
			}
		}
	}

	public void stopWatcherLockScreenService() 
	{
		Log.v("Zoodles", "Stopping the watcher4' " + !pauseLockScreenService + " " + mChildLock.inChildLock());
		if (mChildLock.inChildLock() && !pauseLockScreenService) 
		{
			Log.v("Zoodles", "Stopping the watcher3'");
			synchronized (globalTouchDenyIntent)
			{
				Log.v("Zoodles", "Stopping the watcher2'");
				if(mTouchDenyBound)
				{
					Log.d(TAG, "stopWatcherLockScreenService");
					Log.v("Zoodles", "Stopping the watcher'");
					mTouchDenyService.stopTouchDenyWatcher();
				}
			}
		}
	}

	/**
	 * Start service that attaches transparent lock UI
	 */
	public void startLockScreenService() {
		if (mChildLock.inChildLock() && !pauseLockScreenService) 
		{
			synchronized (globalTouchDenyIntent)
			{
				if(mTouchDenyBound)
				{
					Log.v("Zoodles", "startLockScreenService: ");
					mTouchDenyService.startTouchDenyService();
				}
			}
		}
	}

	/**
	 * Temporary pause lock service so that KID can play some games
	 */
	public void pauseLockScreenService() {
		if (mChildLock.inChildLock()) {
			Log.v("Zoodles", "pauseLockScreenService ");
			pauseLockScreenService = true;
		}
	}

	/**
	 * Stop service that attaches transparent lock UI
	 */
	public void stopLockScreenService() {
		Log.d(TAG, "stopLockScreenService: ");
		pauseLockScreenService = false;
		synchronized (globalTouchDenyIntent){
			if(mTouchDenyBound){
				Log.v("Zoodles", "StopLockScreenService ");
				mTouchDenyService.stopTouchDenyService();
			}
		}
	}

	//////////////////////////////////////////////////////////////////////
	// Utility Accessors
	//////////////////////////////////////////////////////////////////////
	
	/**
	 * Return device info object.  This contains version and system information.
	 * @return
	 */
	public DeviceInfo deviceInfo() {
		if ( mDeviceInfo == null ) {
			setupDeviceInfo();
		}
		return mDeviceInfo;
	}

	public void setDeviceInfo(DeviceInfo deviceInfo){
		mDeviceInfo = deviceInfo;
	}
	
	protected void setupDeviceInfo() {
		mDeviceInfo = new DeviceInfo( this );
	}
	
	public ChildLock childLock() {
		if (mChildLock == null) {
			mChildLock = new ChildLock();
		}
		return mChildLock;
	}
	
	public int getDefaultPixelFormat() {
		return PixelFormat.RGB_565;
	}
	
	public Bitmap.Config getDefaultBitmapConfig() {
		return Bitmap.Config.ARGB_8888;
	}
	
	public Thread.UncaughtExceptionHandler getSystemExceptionHandler() {
		return mSystemExceptionHandler;
	}
		
	public ScreenReceiver screenReceiver() {
		return mScreenReceiver;
	}
	
	/**
	 * This class is used to detect screen change events and prevent the App from playing audio
	 * when the screen is off. 
	 */
	public static class ScreenReceiver extends BroadcastReceiver {
		
		private boolean mScreenLocked;
		private OnScreenStateChangeListener mListener;
		
		public ScreenReceiver()   { mScreenLocked = false; }
		
		public void setOnScreenStateChangeListener( OnScreenStateChangeListener listener ){
			mListener = listener;
		}
		
		public boolean screenLocked() { return mScreenLocked; }
		
		public void setScreenLock(boolean screenLock) { mScreenLocked = screenLock; }
		
	    @Override
	    public void onReceive( Context context, Intent intent ) {
	    	mScreenLocked = intent.getAction().equals( Intent.ACTION_SCREEN_OFF );
	    	Log.d( TAG, "mScreenLocked: " + ( mScreenLocked ? "TRUE" : "FALSE" ) );
	    	
	    	// Don't check for pre-emptions when the screen is off or lockscreen inactive
	    	if ( mScreenLocked ) {
	    		Log.d( TAG, "ScreenReceiver: Stopping pre-emption watch." );
	    		App.instance().childLock().stopWatchForPreemption();
	    	}
	    	
	    	if( mListener != null ){ mListener.onScreenStateChange( mScreenLocked ); }
	    }
	    
	    /**
	     * Implement this to listener to screen state change (on/off),
	     * 
	     * Note: keep long operation out of this listener.
	     */
		public static interface OnScreenStateChangeListener {
			
			public void onScreenStateChange( boolean screenLocked );
			
		}
	}

	@Override
	public void onTerminate() {
		super.onTerminate();
		stopLockScreenService();
		unbindService(mConnection);
		stopService(globalTouchDenyIntent);
	}
	
	public synchronized Preferences preferences() { 
		if( mPrefs == null ){
			mPrefs = new Preferences( this );
		}
		return mPrefs; 
	}	
}