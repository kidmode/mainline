package com.zoodles.kidmode.provider;

import static com.zoodles.kidmode.provider.constants.SettingsProviderConstants.AUTHORITY;
import static com.zoodles.kidmode.provider.constants.SettingsProviderConstants.CODE_ALLOW_INCOMING_CALL;
import static com.zoodles.kidmode.provider.constants.SettingsProviderConstants.CODE_EXIT_ACTION;
import static com.zoodles.kidmode.provider.constants.SettingsProviderConstants.COLUMN_PIN_CODE;
import static com.zoodles.kidmode.provider.constants.SettingsProviderConstants.COLUMN_VALUE;
import static com.zoodles.kidmode.provider.constants.SettingsProviderConstants.EXIT_ACTION_BIRTH_YEAR;
import static com.zoodles.kidmode.provider.constants.SettingsProviderConstants.EXIT_ACTION_DRAW_Z;
import static com.zoodles.kidmode.provider.constants.SettingsProviderConstants.EXIT_ACTION_PIN_CODE;
import static com.zoodles.kidmode.provider.constants.SettingsProviderConstants.EXIT_PIN_CODE_UNSPECIFIED;
import static com.zoodles.kidmode.provider.constants.SettingsProviderConstants.PATH_SETTINGS_BASE;
import static com.zoodles.kidmode.provider.constants.SettingsProviderConstants.TYPE_KIDMODE_SETTING;
import android.content.ContentProvider;
import android.content.ContentValues;
import android.content.UriMatcher;
import android.database.Cursor;
import android.database.MatrixCursor;
import android.net.Uri;
import android.util.Log;
import android.util.SparseIntArray;

import com.zoodles.kidmode.App;
import com.zoodles.kidmode.Preferences;
//import com.zoodles.kidmode.model.gateway.User;

/**
 * Allow third-party app to access Kid Mode settings:
 * 
 * Current available settings: Allow incoming call, exit actions.
 * 
 */
public class SettingsProvider extends ContentProvider{

	private static final String TAG = "SettingsProvider";
	
	protected static final int SETTINGS = 1;
	
	protected static final SparseIntArray sExitActionCodeMap;
	
	protected static final UriMatcher sUriMatcher;
	
	static {
		sUriMatcher = new UriMatcher( UriMatcher.NO_MATCH );
		sUriMatcher.addURI( AUTHORITY, PATH_SETTINGS_BASE + "/" + CODE_ALLOW_INCOMING_CALL, CODE_ALLOW_INCOMING_CALL );
		sUriMatcher.addURI( AUTHORITY, PATH_SETTINGS_BASE + "/" + CODE_EXIT_ACTION, CODE_EXIT_ACTION );
		
		// Map external exit action codes to internal use exit action codes, so change of exposed exit action or internal exit action
		// code won't affect each other.
		sExitActionCodeMap = new SparseIntArray( 5 );
		sExitActionCodeMap.put( EXIT_ACTION_DRAW_Z, Preferences.PREFS_EXIT_ACTION_DRAW_Z );
		sExitActionCodeMap.put( EXIT_ACTION_BIRTH_YEAR, Preferences.PREFS_EXIT_ACTION_BIRTH_YEAR );
		sExitActionCodeMap.put( EXIT_ACTION_PIN_CODE, Preferences.PREFS_EXIT_ACTION_PIN_CODE );
	}

	private App mApp;
	
	@Override
	public boolean onCreate() {
		mApp = App.getInstance(getContext());
		return true;
	}

	@Override
	public String getType( Uri uri ) {
		switch( sUriMatcher.match( uri ) ){
		case CODE_ALLOW_INCOMING_CALL:
		case CODE_EXIT_ACTION:
			return TYPE_KIDMODE_SETTING;
		default:
			return null;
		}
	}
	
	@Override
	public Cursor query( Uri uri, String[] projection, String selection, String[] selectionArgs, String sortOrder ) {
		MatrixCursor c = new MatrixCursor(new String[] {COLUMN_VALUE, COLUMN_PIN_CODE});
		Preferences prefs = mApp.preferences();
		switch( sUriMatcher.match( uri ) ){
		case CODE_ALLOW_INCOMING_CALL:
			Log.d(TAG, "get value for allow incoming call" );
			boolean b = prefs.getSharedValue( Preferences.PREFS_KEY_ALLOW_INCOMING_CALLS, true );
			int value = b ? 1 : 0;
			c.addRow( new Integer[]{ value, EXIT_PIN_CODE_UNSPECIFIED } ); 
			break;
		case CODE_EXIT_ACTION:
			Log.d(TAG, "get value for exit action");
			int internalExitActionCode = prefs.getSharedValue( Preferences.PREFS_KEY_EXIT_ACTION, Preferences.DEFAULT_EXIT_ACTION );
			int resultCode = obtainExternalExitActionCode( internalExitActionCode );
			
			int pinCode = EXIT_PIN_CODE_UNSPECIFIED;
			if ( internalExitActionCode == Preferences.PREFS_EXIT_ACTION_PIN_CODE ) {
				pinCode = prefs.exitPinCode();
			}
			c.addRow( new Integer[]{ resultCode, pinCode } );			
			break;
		default:
			throw new IllegalArgumentException( "Invalid uri" ); 
		}
		return c;
	}

	@Override
	public int update( Uri uri, ContentValues values, String selection, String[] selectionArgs ) {
		int value = values.getAsInteger(COLUMN_VALUE);
		int result = 0;
		switch( sUriMatcher.match( uri ) ){
		case CODE_ALLOW_INCOMING_CALL:
			setAllowIncomingCall( value );
			result=1;
			break;
		case CODE_EXIT_ACTION:
			Integer pinCode = values.getAsInteger(COLUMN_PIN_CODE);
			result = setExitAction( value, pinCode == null ? EXIT_PIN_CODE_UNSPECIFIED : pinCode );
			break;
		default:
			throw new IllegalArgumentException( "Invalid uri" );
		}
		return result;
		
	}
	
	/**
	 * Get the external exit action code that is mapped to internal exit action code
	 * 
	 * @param internalCode
	 * @return
	 */
	protected int obtainExternalExitActionCode( int internalCode ){
		int index = sExitActionCodeMap.indexOfValue( internalCode );
		if( index >= 0 ){ 
			return sExitActionCodeMap.keyAt( index );
		} else {
			return EXIT_ACTION_DRAW_Z;
		}
	}
	
	/**
	 * Check the valid exit action code for phone.
	 * 
	 * @param internalExitActionCode
	 * @return True if it is valid, false otherwise.
	 */
	private boolean isValidExitActionCode( int internalExitActionCode ) {
		 return sExitActionCodeMap.indexOfValue( internalExitActionCode ) >= 0;
	}
	
	private void setAllowIncomingCall( int value ){
		boolean b = value > 0 ? true : false;
		Preferences prefs = mApp.preferences();
		prefs.enableIncomingCalls( b );
		prefs.editSharedValue( Preferences.PREFS_KEY_ALLOW_INCOMING_CALLS, b, true );
	}
	
	/**
	 * @param externalCode
	 * @param pinCode		Used only when action is pin code, ignored  otherwise.
	 * 						0 through 9999, or EXIT_PIN_CODE_UNSPECIFIED if not specified.
	 * @return 1 for success, 0 for failure
	 */
	private int setExitAction( int externalCode, int pinCode ){
		Preferences prefs = mApp.preferences();
		int internalCode = sExitActionCodeMap.get( externalCode );
		
		if ( !isValidExitActionCode( internalCode ) ) { return 0; }

		if ( internalCode == Preferences.PREFS_EXIT_ACTION_PIN_CODE ) {
			if ( pinCode > 9999 ) { 
				// reject -- invalid pin code specified
				return 0;
			}	
//			User user = mApp.sessionHandler().getUser();
//			if ( user == null || !user.hasPINLock() ) {
//				// can't set PIN code - user not logged in, or not premium
//				return 0;
//			}
			if ( pinCode >= 0 ) {
				// valid PIN code was specified - set it
				prefs.setExitPinCode( pinCode );
			}
		} 
		prefs.setExitAction( internalCode );	
		
		return 1;
	}
	
	@Override
	public int delete( Uri uri, String selection, String[] selectionArgs ) {
		// Not allow to delete
		return 0;
	}

	@Override
	public Uri insert( Uri uri, ContentValues values ) {
		// Not allow to insert any data to Kid Mode
		return null;
	}
}
