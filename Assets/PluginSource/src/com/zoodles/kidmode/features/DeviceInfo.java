package com.zoodles.kidmode.features;

import java.util.Locale;

import android.content.Context;
import android.content.pm.FeatureInfo;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager;
import android.content.res.Configuration;
import android.graphics.Point;
import android.hardware.Camera;
import android.net.ConnectivityManager;
import android.net.NetworkInfo;
import android.os.Build;
import android.util.DisplayMetrics;
import android.view.Display;
import android.view.WindowManager;

import com.zoodles.kidmode.ZoodlesConstants;

/**
 * Utility class for performing device inventory.
 */
public class DeviceInfo {
	
	//////////////////////////////////////////////////////////////////////////
	// Constants
	//////////////////////////////////////////////////////////////////////////
	
	protected static final String FLASHPLAYER_PKG = "com.adobe.flashplayer";
	private static final String ANDROID = "android";
	private static final double REST_SIZE_MEASURE = 4.0;
	private static final String REST_SIZE_SMALL = "small";
	private static final String REST_SIZE_LARGE = "large";
	
	//////////////////////////////////////////////////////////////////////////
	// Data
	//////////////////////////////////////////////////////////////////////////
	
	private String mAppVersion;
//	private boolean mFlashInstalled = false;

	private float mRelativeDensity;		// System's scaling factor : 1.0, 1.5, .75 etc
	private int mDensity;				// DPI
	private double mDiagonalSize;		// inches
	private int mScreenClass;			// Screen size (small, normal, large, xlarge), as determined by constants in Configuration class
	private int mDensityClass;			// Density classification (ldpi, mdpi, hdpi, xhdpi)
	private int mWidth, mHeight;		// Display dimensions in Pixels
	private Context ctx;

	//////////////////////////////////////////////////////////////////////////
	// Constructor
	//////////////////////////////////////////////////////////////////////////

	// Default constructor is private - do not use
	private DeviceInfo() {
		;
	}
	
	public DeviceInfo( Context ctx ) {
		initializeWithContext(ctx);
	}

	//////////////////////////////////////////////////////////////////////////
	// Initialization
	//////////////////////////////////////////////////////////////////////////
	
	private void initializeWithContext(Context ctx) {
		this.ctx = ctx;
		WindowManager wm = ((WindowManager) ctx.getSystemService(Context.WINDOW_SERVICE));
		Display display = wm.getDefaultDisplay(); 
		
		// Screen size in pixels
		if ( getSDKVersion() < 13 ) {
			mWidth = display.getWidth();
			mHeight = display.getHeight();
		} else {
			Point size = new Point();
			display.getSize( size );
			mWidth = size.x;
			mHeight = size.y;
		}
				
		// Screen size category as determined by Android framework
		int screenLayout = ctx.getResources().getConfiguration().screenLayout;
		mScreenClass = screenLayout & Configuration.SCREENLAYOUT_SIZE_MASK;
		
		// Approximate screen density, as the Android framework uses for choosing
		// layouts and assets.  Note that this may not be the same as the actual screen
		// density.  Some devices (the Samsung Galaxy Tab 7) will actually lie when reporting
		// this value, in order to get high-density assets and layouts even though the
		// Galaxy Tab has a medium-density screen.
		mDensity = 120;
		DisplayMetrics metrics = new DisplayMetrics();
		display.getMetrics(metrics);
		switch (metrics.densityDpi) {
		case DisplayMetrics.DENSITY_HIGH:
			mDensity = 240;
			break;
		case DisplayMetrics.DENSITY_MEDIUM:
			mDensity = 160;
			break;
		case DisplayMetrics.DENSITY_LOW:
			mDensity = 120;
			break;
		}
		mDensityClass = metrics.densityDpi;

		// Screen physical diagonal size in inches, rounded to one place after decimal (e.g. '3.7', '10.1')
		double xdpi = metrics.xdpi;
		if ( xdpi < 1.0 ) {
			// Guard against divide-by-zero, possible with lazy device manufacturers who set these fields incorrectly
			// Set the density to our best guess.
			xdpi = mDensity;
		}
		double ydpi = metrics.ydpi;
		if ( ydpi < 1.0 ) {
			ydpi = mDensity;
		}
		
		double physicalWidth = ((double) metrics.widthPixels) / xdpi;
		double physicalHeight = ((double) metrics.heightPixels) / ydpi;
		mDiagonalSize = Math.floor( (Math.sqrt(Math.pow(physicalWidth, 2) + Math.pow(physicalHeight, 2)) * 10.0 + 0.5) ) / 10.0;

		mRelativeDensity = metrics.density;
		
		// Test for presence/absence of Flash
		PackageManager pm = ctx.getPackageManager();
		
		// Get App Version
		try {
			PackageInfo pkgInfo = pm.getPackageInfo( ctx.getPackageName(), 0);
			mAppVersion = pkgInfo.versionName;
		} catch ( Exception ignore ) {;}
	}
	
	//////////////////////////////////////////////////////////////////////////
	// System attributes
	//////////////////////////////////////////////////////////////////////////

	/**
	 * Returns current version number of the Kid Mode application.
	 * 
	 * @return
	 */
	public String getAppVersion() {
		return mAppVersion;
	}

	public String getFamily() {
		return ANDROID;
	}

	public String getRelease() {
		return Build.VERSION.RELEASE;
	}
	
	public String getIncrementalBuildVersion() {
		return Build.VERSION.INCREMENTAL;
	}
	
	/**
	 * Returns SDK version number.  See the app manifest
	 * for an Android version number decoder ring.
	 * @return
	 */
	public int getSDKVersion() {
		return Build.VERSION.SDK_INT;
	}

	public String getBrand() {
		return Build.BRAND;
	}

	public String getManufacturer() {
		return Build.MANUFACTURER;
	}
	
	public String getModel() {
		return Build.MODEL;
	}

	public String getID() {
		return Build.ID;
	}

	public String getHost() {
		return Build.HOST;
	}

	public String getDevice() {
		return Build.DEVICE;
	}

	public String getCodename() {
		return Build.VERSION.CODENAME;
	}
	
	//////////////////////////////////////////////////////////////////////////
	// Screen Metrics
	//////////////////////////////////////////////////////////////////////////

	/**
	 * Returns screen width, in pixels.
	 * 
	 * @return
	 */
	public int getWidth() {
		return mWidth;
	}

	/**
	 * Returns screen height, in pixels.
	 * 
	 * @return
	 */
	public int getHeight() {
		return mHeight;
	}
	
	/**
	 * Returns screen density, in DPI.
	 * 
	 * @return
	 */
	public int getDensity() {
		return mDensity;
	}
	
	/**
	 * Returns screen density classification ( ldpi, mdpi, hdpi or xhdpi ).
	 * See DisplayMetrics.DENSITY_HIGH and friends for values this returns.
	 * 
	 * @return
	 */
	public int getDensityClass() {
		return mDensityClass;
	}
	
	/**
	 * Returns screen size classification ( small, normal, large, xlarge )
	 * See Configuration.SCREENLAYOUT_SIZE_LARGE and friends for values this returns.
	 * 
	 * @return
	 */
	public int getScreenClass() {
		return mScreenClass;
	}
	
	/**
	 * Returns the relative screen density, from DisplayMetrics.
	 * 
	 * @see convertDpToPixel
	 * @see convertPixelToDp 
	 * 
	 * @return
	 */
	public float getRelativeDensity() {
		return mRelativeDensity;
	}
	
	/**
	 * Returns empirically calculated screen diagonal size, in inches.
	 * @return
	 */
	public double getDiagonalSize() {
		return mDiagonalSize;
	}
	
	//////////////////////////////////////////////////////////////////////////
	// Helpers
	//////////////////////////////////////////////////////////////////////////
	
	/**
	 * Convert density independent pixel to actual device pixel
	 * 
	 * @param dp
	 */
	public int convertDpToPixel( int dp ) {
		return ( int ) ( dp * getRelativeDensity() + 0.5 );
	}
	
	/**
	 * Convert actual device pixel to density independent pixel
	 * 
	 * @param pixel
	 * @return
	 */
	public int convertPixelToDp( int pixel ) {
		return ( int ) ( pixel / getRelativeDensity() + 0.5 );
	}
	
	//////////////////////////////////////////////////////////////////////////
	// Native YouTube Player
	//////////////////////////////////////////////////////////////////////////
	
	/**
	 * Return TRUE only if YouTube player is up-to-date.
	 * @return
	 */
	public boolean supportYouTubePlayer() {
		return true;
//		YouTubeInitializationResult result = YouTubeApiServiceUtil.isYouTubeApiServiceAvailable( App.instance() );
//		return result == YouTubeInitializationResult.SUCCESS;
	}
	
	
	/**
	 * Return TRUE when meet below conditions:<br>
	 * <br>
	 * 1. YouTube player is installed on the devices and it is up-to-date.
	 * <br><br>
	 * <b>OR</b>
	 * <br><br>
	 * 1. The device has to be running at least ICS ( 4.0 );<br>
	 * 2. YouTube app is missing, OR YouTube app is disable, OR YouTube app is invalid.
	 * 
	 * @return
	 */
	public boolean supportYouTubeVideo(){
		return true;
//		YouTubeInitializationResult result = YouTubeApiServiceUtil.isYouTubeApiServiceAvailable( App.instance() );
//
//		if( result == YouTubeInitializationResult.SUCCESS ){
//			return true;
//		}
//		
//		return ( result == YouTubeInitializationResult.SERVICE_DISABLED ||
//				result == YouTubeInitializationResult.SERVICE_INVALID ||
//				result == YouTubeInitializationResult.SERVICE_MISSING ) && supportsHTML5Video();
	}
	
	public boolean isYouTubePlayerUpdateRequired(){
		return false;
//		YouTubeInitializationResult result = YouTubeApiServiceUtil.isYouTubeApiServiceAvailable( App.instance() );
//		return result == YouTubeInitializationResult.SERVICE_VERSION_UPDATE_REQUIRED;
	}
	
	//////////////////////////////////////////////////////////////////////////
	// Supported Features
	//////////////////////////////////////////////////////////////////////////
	
	/**
	 * Returns TRUE if Flash is installed on the device.
	 * 
	 * @return
	 */
	public boolean hasFlashInstalled() {
		PackageManager pm = ctx.getPackageManager();
		try {
			pm.getPackageInfo(FLASHPLAYER_PKG, 0);
			return true;
		} catch (Exception ignore) {
			;
		}
		return false;
	}
	
	/**
	 * Returns TRUE if device supports home button blocking.  In general,
	 * this is nearly every device, except for those on a blacklist.
	 * 
	 * @return
	 */
	public boolean supportsChildLock() {
		if( isKindleFireAndRunningICS() ){ return false; }
		return true;
	}
	
	/**
	 * Returns TRUE if Video Mail is supported on the current device,
	 * from a hardware perspective.  (There may be other reasons why
	 * Video Mail may not be supported, such as locale, etc).
	 * 
	 * Use this call in preference to hasFrontFacingCamera(). This
	 * call provides a debug short-circuit, as well as a wrapper that
	 * allows for device blacklisting if necessary.
	 * 
	 * @see hasFrontFacingCamera
	 */
	public boolean supportsVideoMail() {
//		if (App.instance().isDebug()) return true;	
		return hasFrontFacingCamera();
	}
	
	/**
	 * Return TRUE when the device
	 * 1. is ICS ( SDK 14 ) device;
	 * 2. has problem with flash ( include flash not come preload ), and is ICS device;
	 * 
	 * @return
	 */
    public boolean supportsHTML5Video() {
        return hasICS() || isKindleFireAndRunningICS();
    }
    
	//////////////////////////////////////////////////////////////////////////
	// Device Capabilities (Hardware)
	//////////////////////////////////////////////////////////////////////////
	
    /**
     * Return TRUE if device sdk version is greater than 10.
     * @return
     */
    public boolean hasHoneyComb() {
    	return getSDKVersion() > 10;
    }
    
    public boolean hasICS(){
    	return getSDKVersion() > 14;
    }
    
    /**
     * Return TRUE if the device sdk version is greater than 15.
     * @return
     */
    public boolean hasJellyBean() {
    	return getSDKVersion() > 15;
    }
    
	/**
	 * Returns TRUE if the current device has a front-facing camera
	 * (that we support).
	 */
	public boolean hasFrontFacingCamera() {
		if ( getSDKVersion() < 9 ) { 
			// Device running Froyo (Android 2.2)
			// Handle some devices with proprietary camera API
			if ( isGalaxyTab7() ) 	{ return true; }
			if ( isDellStreak7() )	{ return true; }
			return false;
		}
			
		// Gingerbread (2.3) or later - use the official Android API
		int cameraCount = 0;
		// Surround this block of code with a try/catch because we were seeing some crashes
		// in the crash log for the Samsung Captivate running 2.2 ( SDK Version 8 ). Possibly
		// a defect with Samsung.
		try{
			Camera.CameraInfo cameraInfo = new Camera.CameraInfo();
			cameraCount = Camera.getNumberOfCameras();
			for ( int camIdx = 0; camIdx < cameraCount; camIdx++ ) {
				Camera.getCameraInfo( camIdx, cameraInfo );
				if ( cameraInfo.facing == Camera.CameraInfo.CAMERA_FACING_FRONT ) {
					return true;
				}
			}
		}
		catch( NoClassDefFoundError ex ) { return false; }
		// Some Galaxy Nexus phones have problem with camera, this will
		// make sure the phone does not crash.
		catch( RuntimeException ignore) { return false; } 
															
		return false;
	}
	
	/**
	 * Returns TRUE if the system is currently connected to the Internet
	 * via WiFi.  Returns FALSE if connected via another medium (3G) or
	 * there is no connection.
	 * 
	 * @return
	 */
	public boolean hasWiFiConnection(final Context ctx) {
		ConnectivityManager conn = 
				(ConnectivityManager) ctx.getSystemService(Context.CONNECTIVITY_SERVICE);
		NetworkInfo activeNetwork = conn.getActiveNetworkInfo();
		if (activeNetwork != null && activeNetwork.isConnected()
				&& activeNetwork.getType() == ConnectivityManager.TYPE_WIFI) {
			return true;
		}
		return false;
	}
	
	/**
	 * Returns TRUE if the system is currently connected to the Internet
	 * via Cellular 3g/4g.  Returns FALSE if connected via another medium (wifi) or
	 * there is no connection.
	 * 
	 * @return
	 */
	public boolean hasCellularConnection(final Context ctx) {
		ConnectivityManager conn = 
				(ConnectivityManager) ctx.getSystemService(Context.CONNECTIVITY_SERVICE);
		NetworkInfo activeNetwork = conn.getActiveNetworkInfo();
		if (activeNetwork != null && activeNetwork.isConnected()
				&& ( activeNetwork.getType() == ConnectivityManager.TYPE_MOBILE ||
				     activeNetwork.getType() == ConnectivityManager.TYPE_WIMAX) ) { 
			return true;
		}
		return false;
	}
	
	/**
	 * Returns TRUE if the device has any sort of active network connection.
	 * Returns FALSE if the device has no connection.
	 * 
	 * @param ctx
	 * @return
	 */
	public boolean hasNetworkConnection(final Context ctx) {
		ConnectivityManager conn = 
				(ConnectivityManager) ctx.getSystemService(Context.CONNECTIVITY_SERVICE);
		NetworkInfo activeNetwork = conn.getActiveNetworkInfo();
		if (activeNetwork != null && activeNetwork.isConnected()) { 
			return true;
		}
		return false;
	}
	
	/**
	 * Returns TRUE if this device has a cell radio AND is capable of placing voice calls.
	 * @return
	 */
	public boolean hasPhone( final Context ctx ) {
		PackageManager pm = ctx.getPackageManager();
		return pm.hasSystemFeature( PackageManager.FEATURE_TELEPHONY );
	}
	
	/**
	 * Returns TRUE if device is considered a "Tablet".
	 * 
	 * We define a "tablet" as a device with at least "large" screen size, and
	 * screen resolution greater than 800x480 when it is not ldpi.
	 * 
	 * For ldpi, return true when screen size is equal to or bigger than large.
	 * 
	 * In general, Activity classes are discouraged from writing if/else code to
	 * perform tablet-specific functions.
	 * 
	 * Instead, we use a pattern of Activity classes specific to phone or tablet interfaces.
	 * 
	 * @return
	 */
	public boolean isTablet() {
		// Must have a Large or Xlarge screen
		if (!(mScreenClass == Configuration.SCREENLAYOUT_SIZE_LARGE) && !(mScreenClass == Configuration.SCREENLAYOUT_SIZE_XLARGE )) return false;
		
		// If medium or high density, screen resolution must be greater than 800x480
		if ( mDensityClass != DisplayMetrics.DENSITY_LOW ) { 
			return ( ( getWidth() > 800 && getHeight() > 480 ) || ( getHeight() > 800 && getWidth() > 480 ) );
		}
			
		return true;
	}
	
	// Return the language being used by the device
	public String getLanguage() {
		return Locale.getDefault().getLanguage();
	}
	
	//////////////////////////////////////////////////////////////////////////
	// Specific Device Identification -
	//
	// Usually used for blacklisting application features, but sometimes
	// used for other purposes (for instance, in a factory method to
	// choose a bespoke implementation for a given device).
	//
	// Be sure you have a good reason to use one of these methods.  And
	// always hide a device-specific check behind an API call or other
	// abstraction (such as a factory method or one of the capability-oriented
	// helpers in this class), to keep mainline business logic free from
	// device-specific code.
	//////////////////////////////////////////////////////////////////////////
	
	/**
	 * Returns TRUE if the current device is a Samsung Galaxy Tab 7 (international version).
	 * 
	 * @return
	 */
	public boolean isInternationalGalaxyTab() {
		if ( "samsung".equalsIgnoreCase(Build.MANUFACTURER) && "GT-P1000".equals(Build.DEVICE) ) {
			return true;
		}
		return false;
	}
	
	/**
	 * Return TRUE if the current device is a Samsung Galaxy Tab 7.
	 * 
	 * @return
	 */
	public boolean isGalaxyTab7() {
		if ( !"samsung".equalsIgnoreCase(Build.MANUFACTURER) ) {
			return false;
		}
		
		// http://www.samsunghub.com/2010/09/06/samsung-galaxy-tabs-model-numbers-for-different-operators/
		String[] models = { "SPH-P100",			// Sprint
							"SCH-I800",			// Verizon
							"SGH-T849",			// T-Mobile (different than web article, but determined by directly inspecting a T-Mobile Tab)
							"SGH-I987",			// AT&T
							"GT-P1000",			// International
							"SHW-M180S",		// SKT
							"SC-01C",			// NTT DoCoMo
		};
		
		for ( int i = 0; i < models.length; i++ ) {
			if ( models[i].equals(Build.DEVICE) ) {
				return true;
			}
		}
		
		return false;
	}
	
	/**
	 * Return TRUE if the current device is a Dell Streak 7.
	 * 
	 * @return
	 */
	public boolean isDellStreak7() {
		if ( "Dell Inc.".equalsIgnoreCase(Build.MANUFACTURER) && "streak7".equalsIgnoreCase(Build.DEVICE)) {
			return true;
		}
		return false;
	}
	
	/**
	 * Return TRUE if the current device is a Motorola RAZR.
	 */
	public boolean isMotoRAZR() {
		if ( "motorola".equalsIgnoreCase(Build.MANUFACTURER) && 
				Build.DEVICE != null && Build.DEVICE.contains("spyder") ) {
			return true;
		}
		return false;
	}
	
	/**
	 * Return TRUE if the current device is a Motorola Bionic.
	 */
	public boolean isMotoBionic() {
		if ( "motorola".equalsIgnoreCase(Build.MANUFACTURER) && 
				Build.DEVICE != null && Build.DEVICE.contains("targa") ) {
			return true;
		}
		return false;
	}
	
	/**
	 * Return TRUE if the device is made by HTC.
	 * 
	 * @return
	 */
	public boolean isHTC() {
		if ( "HTC".equalsIgnoreCase(Build.MANUFACTURER) ) {
			return true;
		}
		return false;
	}
	
	/**
	 * Return TRUE if the device is made by Pandigital.
	 * 
	 * @return
	 */
	public boolean isPandigital() {
		if ( "Pandigital".equalsIgnoreCase(Build.MANUFACTURER) ) {
			return true;
		}
		return false;
	}
	
	/**
	 * Return TRUE if the device is made by Ematic.
	 * 
	 * @return
	 */
	public boolean isEmatic() {
		if ( "ematic".equalsIgnoreCase(Build.MANUFACTURER) ) {
			return true;
		}
		return false;
	}
	
	/**
	 * Return true if the device is Kindle Fire devices that running ICS and onward
	 * Those devices are: Kindle Fire HD 7, Kindle Fire HD 8.9, and Kindle Fire 2. 
	 * 
	 * @return
	 */
	public boolean isKindleFireAndRunningICS() {
		return "Amazon".equalsIgnoreCase( Build.MANUFACTURER ) && getSDKVersion() >= 14;
	}
	
	/**
	 * Return string used by REST URL. If mDiagonalSize bigger than 4 inches, it is large, 
	 * otherwise it is small
	 * @return
	 */
	public String getScreenSizeForREST() {
		if (mDiagonalSize > REST_SIZE_MEASURE) {
			return REST_SIZE_LARGE;
		} else {
			return REST_SIZE_SMALL;
		}
	}

	public boolean hasHTCSoftware(Context context) {
		FeatureInfo[] systemAvailableFeatures = getSystemAvailableFeatures(context);
		for (FeatureInfo fi : systemAvailableFeatures) {
			if (fi.name != null && fi.name.startsWith("com.htc.software")) {
				return true;
			}
		}
		return false;
	}

	public FeatureInfo[] getSystemAvailableFeatures(Context context) {
		FeatureInfo[] features = context.getPackageManager().getSystemAvailableFeatures();
		return features;
	}

	public boolean hasKidBrowserInstalled(Context context){
		PackageManager pm = context.getPackageManager();
		try {
			pm.getPackageInfo(ZoodlesConstants.KID_BROWSER_PACKAGE, 0);
			return true;
		} catch ( Exception ignore ) {;}
		return false;
	}

	public boolean isFromKitKat() {
		return getSDKVersion() >= 19;
	}

	public boolean canPlayFlash(Context context) {
		//Has flash and below 4.4
		if(hasFlashInstalled() && !isFromKitKat()){
			return true;
		}
		//4.4 and above and has flash, kid browser
		if(hasKidBrowserInstalled(context) && hasFlashInstalled() && isFromKitKat()){
			return true;
		}
		return false;
	}

	public boolean needInstallKidBrowser(Context context) {
		//From android 4.4 and doesnt have KidBrowser installed
		if(isFromKitKat() && !hasKidBrowserInstalled(context)) {
			return true;
		}
		return false;
	}
}
