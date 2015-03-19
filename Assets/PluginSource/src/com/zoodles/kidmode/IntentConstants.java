package com.zoodles.kidmode;


import android.app.Activity;

/**
 * Constants associated with in-app intents.
 * 
 */
public interface IntentConstants {
	
	/**
	 * Activity Intent Extra keys
	 */
	public static final String EXTRA_KID_ID			    = "EXTRA_KID_ID";			// Child ID specified
	public static final String EXTRA_KID			    = "EXTRA_KID";			    // Child specified as part of the intent
	public static final String EXTRA_APP_LIST         = "EXTRA_APP_LIST";         // App lists 
	public static final String EXTRA_ORIENTATION		= "EXTRA_ORIENTATION";		// Tells the Activity which orientation it should use for displaying itself		
	public static final String EXTRA_GAME			    = "EXTRA_GAME";				// Game link to play
	public static final String EXTRA_TAB_ID		        = "EXTRA_TAB_ID";			// Tab that game play screen should open to
	public static final String EXTRA_GAME_SELECTION     = "EXTRA_GAME_SELECTION";	// Selected game, on 'Games' or 'Favorites' screens
	public static final String EXTRA_TAB_SELECTED		= "EXTRA_TAB_SELECTED";		// Set to true when tab is selected
	public static final String EXTRA_PLAY_MAIL_PROMPT	= "EXTRA_PLAY_MAIL_PROMPT";	// Set to true when need to play mail prompt
	public static final String EXTRA_READING		    = "EXTRA_READING";			// Reading to play	
	public static final String EXTRA_MAIL_MESSAGE	    = "EXTRA_MAIL_MESSAGE";		// Mail message to play
	public static final String EXTRA_CONTACT		    = "EXTRA_CONTACT";			// Recipient of new mail message
	public static final String EXTRA_NEW_MAIL_ONLY      = "EXTRA_NEW_MAIL_ONLY";	// Playback of new mail only
	public static final String EXTRA_ART_SELECTION		= "EXTRA_ART_SELECTION";	// Art selection (position) for viewing purpose
	public static final String EXTRA_BILLING_URL        = "EXTRA_BILLING_URL";		// URL for the billing activity
	public static final String EXTRA_CL_FLOW 			= "EXTRA_HBL_FLOW";			// Flag used in HBL setup		
	public static final String EXTRA_CL_PROMPT_STATE	= "EXTRA_HBL_PROMPT_STATE";	// State enum for Child Lock prompts
	public static final String EXTRA_FLAG				= "EXTRA_FLAG";				// Extra for flag
	public static final String EXTRA_TITLE	 			= "EXTRA_TITLE";			// Title string to show
	public static final String EXTRA_DEBUG_LINK_ID		= "EXTRA_DEBUG_LINK_ID";	// Server ID of the link to show
	public static final String EXTRA_SHOW_MENU			= "EXTRA_SHOW_MENU";		// Flag indicating if menu should be shown
	public static final String EXTRA_PLACEMENT 			= "EXTRA_PLACEMENT";		// Tip presentation time (pre or post)
	public static final String EXTRA_NATIVE_APP			= "EXTRA_NATIVE_APP";		// Flag to indicate this intent is associated with native app launch
	public static final String EXTRA_APP_REVIEW			= "EXTRA_NATIVE_APP_REVIEW";// Flag to indicate this intent is associated with native app launch
	public static final String EXTRA_EMAIL_ADDRESS		= "EXTRA_EMAIL_ADDRESS";	// Email address, used to pre-populate a form
	public static final String EXTRA_AUTOCREATE			= "EXTRA_AUTOCREATE";		// Flag automatically create account in FRE
	public static final String EXTRA_IS_NOTIFICATION	= "EXTRA_NOTIFICATION";     // Flag for notification
	public static final String EXTRA_FROM_WHERE			= "EXTRA_FROM_WHERE";		// Tells the activity where it is being launched
	public static final String EXTRA_INTERVAL			= "EXTRA_INTERVAL";			// For time interval
	public static final String EXTRA_INSTALLED_APPS 	= "EXTRA_INSTALLED_APPS";	// For installed apps
	public static final String EXTRA_SUGGESTED_APPS 	= "EXTRA_SUGGESTED_APPS";	// For installed apps
	public static final String EXTRA_WEEKLY_NOTIFI		= "EXTRA_WEEKLY_NOTIFI";	// Flag to indicate this intent is from free app weekly notification
	public static final String EXTRA_LOCATION	        = "EXTRA_LOCATION";			// Subscription Billing Location 
	public static final String EXTRA_APP_EXIT	        = "EXTRA_APP_EXIT";			// Flag telling launcher activity it should exit app
	public static final String EXTRA_FORCE_RELOAD		= "EXTRA_FORECE_RELOAD";   	// Flag telling story book list whether or not should be fetched from server
	public static final String EXTRA_RECORD_GOAL		= "EXTRA_RECORD_GOAL";   	// Experiment goal to be recorded
	public static final String EXTRA_PID				= "EXTRA_PID";				// PID used to kill process
	public static final String EXTRA_DB_ERROR			= "EXTRA_DB_ERROR";			// Indicate whether exception is from DB error
	public static final String EXTRA_EXCEPTION_RPT		= "EXTRA_EXCEPTION_RPT";	// Exception report
	public static final String EXTRA_EXCEPTION_CLS		= "EXTRA_EXCEPTION_CLS";	// Exception class name
	public static final String EXTRA_BOOK_ID			= "EXTRA_BOOK_ID";			// Book id.
	public static final String EXTRA_LINK_ID			= "EXTRA_LINK_ID"; 			// Link id
	public static final String EXTRA_ENTRY_POINT		= "EXTRA_ENTRY_POINT";     	// Entry point
	public static final String EXTRA_AGE_GATE_POST_ACTION	= "EXTRA_AGE_GATE_POST_ACTION";	// Action to perform after age gate
	public static final String EXTRA_ART_CANVAS_WIDTH	= "EXTRA_ART_CANVAS_WIDTH";	// Art canvas width
	public static final String EXTRA_ART_CANVAS_HEIGHT = "EXTRA_ART_CANVAS_HEIGHT"; // Art canvas height
	public static final String EXTRA_ART_STICKER_NAME	= "EXTRA_ART_STICKER_NAME";	// Sticker name
	public static final String EXTRA_ART_STICKER	    = "EXTRA_ART_STICKER";	// Sticker bitmap
	public static final String EXTRA_ART_DRAWING	    = "EXTRA_ART_DRAWING";	// Drawing bitmap

	public static final String EXTRA_PD_OPEN_FEATURE	= "EXTRA_PD_OPEN_FEATURE";  // Feature Pane Parent Dashboard should open to
	public static final String EXTRA_CHILD_ID_ADDED		= "EXTRA_CHILD_ID_ADDED";	// ID of new kid that was added
	public static final String EXTRA_CHILD_ID_SELECTED	= "EXTRA_CHILD_ID_SELECTED";	// ID of kid that was selected
	public static final String EXTRA_FEATURE_SELECTED	= "EXTRA_FEATURE_SELECTED";	// ID of PD feature that was selected
	public static final String EXTRA_FOR_RESULT 		= "EXTRA_FOR_RESULT"; 		// invoked via startActivityForResult

	/// New PD
	public static final String EXTRA_IS_TABLET			= "EXTRA_IS_TABLET";
	
	public static final String EXTRA_ARGUMENTS 			= "EXTRA_ARGUMENTS";		// extra arguments for feature pane
	public static final String EXTRA_CL_FIRST_RUN 		= "EXTRA_FIRST_RUN";		// first-run flag for Child Lock
	/**
	 * Service Intent Extra keys
	 */

	// Video Mail Upload Service
	public static final String EXTRA_PENDING_MAIL 	= "EXTRA_PENDING_MAIL";
	
	// Native App Service
	public static final String CMD_START_APP 			= "CMD_START_APP";
	public static final String CMD_STOP_APP 			= "CMD_STOP_APP";
	public static final String CMD_DIE 					= "CMD_DIE";
	public static final String EXTRA_COMMAND 			= "EXTRA_COMMAND";
	public static final String EXTRA_PACKAGE 			= "EXTRA_PACKAGE";
	public static final String EXTRA_ACTIVITY 			= "EXTRA_ACTIVITY";	
	public static final String EXTRA_PREEMPT_INTENT 	= "EXTRA_PREEMPT_INTENT";
	public static final String EXTRA_PREEMPT_APP		= "EXTRA_PREEMPT_APP";
	public static final String EXTRA_PREEMPT_APP_PKG	= "EXTRA_PREEMPT_APP_PKG";
	public static final String EXTRA_STOP_RECEIVER 		= "EXTRA_STOP_RECEIVER";
	
	// Prefetcher Service
	public static final String EXTRA_FETCHER			= "EXTRA_FETCHER";
	public static final String EXTRA_URL				= "EXTRA_URL";
	public static final String EXTRA_URL_LIST			= "EXTRA_URL_LIST";
	public static final String EXTRA_PRIORITY			= "EXTRA_PRIORITY";
	
	// Timer
	public static final String EXTRA_FROM_CHOOSER		= "EXTRA_FROM_CHOOSER";
	public static final String EXTRA_PENDING_INTENT		= "EXTRA_PENDING_INTENT";
	
	// Art Studio
	public static final String EXTRA_NO_SAVE_OPTION		= "EXTRA_NO_SAVE_OPTION";
	
	/**
	 * Sub-activity request codes
	 */
	public static final int CREATE_KID_ACTIVITY 	      = 1;
	public static final int TAKE_PHOTO_ACTIVITY 	      = 2;
	public static final int PARENT_DASHBOARD_ACTIVITY 	      = 4;
	public static final int PARENT_DASHBOARD_SETTINGS_ACTIVITY 	  = 6;
	public static final int EXIT_ACTIVITY			      = 9;
	public static final int BILLING_ACTIVITY              = 14;
	public static final int KID_FILTER_VIOLENCE_ACTIVITY  = 15;
	public static final int KID_BLOCK_SHOWS_ACTIVITY	  = 16;
	public static final int KID_BLOCK_SITES_ACTIVITY	  = 17;
	public static final int MARKET_ACTIVITY				  = 18;
	public static final int KID_CHILD_LOCK_ACTIVITY		  = 19;
	public static final int PLAYGROUND_ACTIVITY			  = 20;
	public static final int INTRO_UPSELL_ACTIVITY		  = 21;
	public static final int TIP_ACTIVITY		  	  	  = 22;
	public static final int PD_READ_BOOK_ACTIVITY		  = 23;
	public static final int PD_PIN_CODE_ACTIVITY		  = 24;
	public static final int FRE_NATIVE_APPS_ACTIVITY	  = 25;
	public static final int NATIVE_APP_ACTIVITY			  = 26;
	public static final int BOOK_RECORDING_ACTIVITY		  = 27;
	public static final int STORY_BOOK_ACTIVITY			  = 28;
	public static final int VIEW_VIDEO_MAIL_ACTIVITY	  = 29;
	public static final int RECORD_VIDEO_MAIL_ACTIVITY	  = 30;
	public static final int PD_VIDEO_MAIL_ACTIVITY	  	  = 31;
	public static final int EXIT_TIMES_UP_ACTIVITY	  	  = 34;
	public static final int ADD_MORE_TIME_ACTIVITY	  	  = 35;
	public static final int KID_STORY_BOOK_ACTIVITY		  = 36;
	public static final int DEBUG_SETTINGS_ACTIVITY 	  = 37; 				
	public static final int PARENT_PERMISSION_ACTIVITY	  = 37;
	public static final int PARENT_PERMISSION_AUTO_ACTIVITY	  = 38;
	public static final int FRE_AGE_GATE_ACTIVITY_SIGNUP  = 39;
	public static final int FRE_AGE_GATE_ACTIVITY_RETURNING = 40;
	public static final int VERIFY_VPC_ACTIVITY          	= 41;
	public static final int SMART_SELECTION_ACTIVITY		= 42;
	public static final int ART_STICKER	    = 43;
	public static final int KID_BILLING	    = 44;
	public static final int INTRO_FLASH_ACTIVITY			= 45;
	
	// "Tip of the Day" sub-activity codes
	public static final int TIP_ACTIVITY_BASE 					= 100;
	public static final int TIP_HOME_ACTIVITY 					= TIP_ACTIVITY_BASE + 1;
	public static final int TIP_COMPUTER_ACTIVITY 				= TIP_ACTIVITY_BASE + 2;
	public static final int TIP_SETTINGS_ACTIVITY 				= TIP_ACTIVITY_BASE + 3;
	public static final int TIP_PARENT_ACTIVITY 				= TIP_ACTIVITY_BASE + 4;
	public static final int TIP_REVIEW_ACTIVITY 				= TIP_ACTIVITY_BASE + 5;
	public static final int TIP_PROMOTE_SUBJECTS_ACTIVITY 		= TIP_ACTIVITY_BASE + 6;
	public static final int TIP_VIOLENCE_FILTER_ACTIVITY 		= TIP_ACTIVITY_BASE + 7;
	public static final int TIP_PREMIUM_ACTIVITY 				= TIP_ACTIVITY_BASE + 8;
	public static final int TIP_BLOCK_CHARACTERS_ACTIVITY 		= TIP_ACTIVITY_BASE + 9;
	public static final int TIP_ART_ACTIVITY					= TIP_ACTIVITY_BASE + 10;
	public static final int TIP_BOOKS_ACTIVITY					= TIP_ACTIVITY_BASE + 11;
	public static final int TIP_GAMES_ACTIVITY					= TIP_ACTIVITY_BASE + 12;
	public static final int TIP_NATIVE_APPS_ACTIVITY			= TIP_ACTIVITY_BASE + 13;
	public static final int TIP_REPORT_ACTIVITY					= TIP_ACTIVITY_BASE + 14;
	public static final int TIP_ADD_PHOTO_ACTIVITY				= TIP_ACTIVITY_BASE + 15;
	public static final int TIP_RECORD_BOOK_ACTIVITY			= TIP_ACTIVITY_BASE + 16;
	public static final int TIP_APP_RECOMMENDATION_ACTIVITY		= TIP_ACTIVITY_BASE + 17;
	public static final int TIP_OFFLINE_ACTIVITY				= TIP_ACTIVITY_BASE + 18;
	public static final int TIP_YOUTUBE_ACTIVITY				= TIP_ACTIVITY_BASE + 19;
	public static final int TIP_ADOBE_FLASH_ACTIVITY			= TIP_ACTIVITY_BASE + 20;
	public static final int TIP_KID_BROWSER_ACTIVITY			= TIP_ACTIVITY_BASE + 21;
	
	public static final int TIP_ACTIVITY_END = TIP_ACTIVITY_BASE + 99;

	/**
	 * Sub-activity return status codes
	 */
	public static final int RESULT_ERROR  			= Activity.RESULT_FIRST_USER + 5;			// Sub-activity returned an error
	public static final int RESULT_DELETE_CHILD		= Activity.RESULT_FIRST_USER + 15;			// Calling activity should delete the child
	public static final int RESULT_SKIP         	= Activity.RESULT_FIRST_USER + 25;			// video skipped

	public static final int RESULT_CHILD_ADDED		= 55;			// One or more children added
	public static final int RESULT_UP_BUTTON_EXIT	= 56;			// Exit activity via up button in action bar
	public static final int RESULT_CHILD_SELECTED	= 57;			// Exit activity via child selection in action bar
	public static final int RESULT_FEATURE_SELECTED	= 58;			// Exit activity via feature selection in action bar
	
	// exit screen
	public static final int EXIT_GREEN_ARROW    	= Activity.RESULT_FIRST_USER + 26;			// Returned from the 'Z' screen via the green arrow.
	public static final int RESULT_SETTINGS_OK  	= Activity.RESULT_FIRST_USER + 27;

	public static final int RESULT_NEW_VM_RECORDED	= Activity.RESULT_FIRST_USER + 35;			// A new video mail recorded.
	public static final int RESULT_DELETE_VM		= Activity.RESULT_FIRST_USER + 45;			// Calling activity should delete the video mail
	
	// age gate
	public static final int RESULT_AGE_GATE_PASS 	= Activity.RESULT_FIRST_USER + 50;	
	public static final int RESULT_AGE_GATE_FAIL	= Activity.RESULT_FIRST_USER + 51;
	public static final int RESULT_AGE_GATE_FAIL_3	= Activity.RESULT_FIRST_USER + 52;
	
	
	/**
	 * actions used by IntentFilter
	 */
	public static final String ACTION_TIMES_UP = "com.zoodles.kidmode.timesup";
	
}
