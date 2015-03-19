package com.zoodles.kidmode.provider.constants;

import android.net.Uri;

/**
 * Those constants are going to be shared with external applications.
 * 
 * WARNING: Change with caution
 */
public interface SettingsProviderConstants
{

	// ////////////////////////////////////////////////////////////////////////
	// Base URI
	// ////////////////////////////////////////////////////////////////////////

	public static final String AUTHORITY = "com.zoodles.kidmode.provider.settings";
	public static final String PATH_SETTINGS_BASE = "settings";

	/**
	 * Base URI to this ContentProvider
	 */
	public static final Uri CONTENT_URI = Uri.parse("content://" + AUTHORITY + "/" + PATH_SETTINGS_BASE);

	// ////////////////////////////////////////////////////////////////////////
	// Settings
	// ////////////////////////////////////////////////////////////////////////

	/**
	 * Setting to allow/disallow incoming calls. Append to CONTENT_URI via
	 * ContentUris.withAppendedId
	 */
	public static final int CODE_ALLOW_INCOMING_CALL = 100;

	/**
	 * Setting for exit action. Append to CONTENT_URI via
	 * ContentUris.withAppendedId
	 */
	public static final int CODE_EXIT_ACTION = 110;

	/**
	 * Values for the "Exit Action" setting
	 */
	public static final int EXIT_ACTION_DRAW_Z = 11;
	public static final int EXIT_ACTION_BIRTH_YEAR = 22;
	public static final int EXIT_ACTION_PIN_CODE = 33;

	/**
	 * Values for PIN code
	 */
	public static final int EXIT_PIN_CODE_UNSPECIFIED = -1;

	// ////////////////////////////////////////////////////////////////////////
	// Result
	// ////////////////////////////////////////////////////////////////////////

	/**
	 * Column in the Cursor returned by query
	 */
	public static final String COLUMN_VALUE = "prefs_value";
	public static final String COLUMN_PIN_CODE = "prefs_pin_code"; // 0 through
																	// 9999
																	// allowed,
																	// or
																	// EXIT_PIN_CODE_UNSPECIFIED

	/**
	 * MIME type of returned result
	 */
	public static final String TYPE_KIDMODE_SETTING = "vnd.android.cursor.dir/vnd.zoodles.kidmode.setting";
}
