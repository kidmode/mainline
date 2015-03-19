package com.zoodles.kidmode;

/**
 * Constants used for interfacing with Zoodles servers.
 * 
 */
public interface ZoodlesConstants {

	//////////////////////////////////////////////////////////////////////
	// Server hosts
	//////////////////////////////////////////////////////////////////////
	
	public static final String ZOODLES_STAGING_HOST				= "staging.zoodles.com";
	public static final String ZOODLES_PRODUCTION_HOST			= "www.zoodles.com";
	public static final String ZOODLES_PRODUCTION_VIDSYNC_HOST	= "vidbalancer.zoodles.com";
	public static final String ZOODLES_STAGING_VIDSYNC_HOST		= "vidbalancer-staging.zoodles.com";
	public static final String LIKE_US_URL						= "http://m.facebook.com/zoodles";
	public static final String HTTP								= "http";
	public static final String HTTPS							= "https";
	public static final int    HTTP_PORT						= 80;
	public static final int    HTTPS_PORT						= 443;
	
	//////////////////////////////////////////////////////////////////////
	// URL stuff
	//////////////////////////////////////////////////////////////////////

	public static final String QUESTION_MARK                     = "?";
	public static final String AMPERSAND                         = "&";
	public static final String EQUALS                            = "=";
	public static final String SLASH                             = "/";
		
	//////////////////////////////////////////////////////////////////////
	// WWW Urls
	//////////////////////////////////////////////////////////////////////

	public static final String ZOODLES_BILLING_URL               = "/parent/android";
	public static final String ZOODLES_UPSELL_URL                = "/parent/android/upsell";
	public static final String ZOODLES_TOS_URL        			 = "/home/legal/terms_android";
	public static final String ZOODLES_PRIVACY_POLICY_URL		 = "/home/legal/privacy";
	
	//////////////////////////////////////////////////////////////////////
	// REST API URLs
	//
	// * All URL strings start with "/"
	// * No URL strings end with "/"
	//////////////////////////////////////////////////////////////////////
	
	public static final String REST_BOOKS_URL 				    = "/api/books";
	public static final String REST_CLIENT_ID_URL			    = "/api/clients";
	public static final String REST_LOCALE_URL			    	= "/api/locale";
	public static final String REST_KIDS_URL				    = "/api/kids";
	public static final String REST_PARENT_URL					= "/api/parent/kids";
	public static final String REST_LOGIN_URL				    = "/api/token";
	public static final String REST_USER_URL				    = "/api/user";
	public static final String REST_USER_EMAIL_URL				= "/api/emails";
	public static final String REST_FORGOT_PASSWORD				= "/api/user/forgot";
	public static final String REST_CRASHES						= "/api/crashes";
	public static final String REST_READING_REQUESTS	        = "/api/reading_requests";
	public static final String REST_TIP							= "/api/html/tips";
	public static final String REST_READINGS					= "/api/readings";
	public static final String REST_APP_APPROVED     			= "/api/apps";
	public static final String REST_APP_RECOMMENDATIONS			= "/api/apps/recommendations";
	public static final String REST_APP_REVIEW      			= "/api/apps/reviews";
	public static final String REST_APP_ALLOWED      			= "/api/apps/allowed";
	public static final String REST_EXPERIMENT_URL      		= "/api/experiments";
	public static final String REST_SUBSCRIPTION	      		= "/api/subscriptions";
	public static final String REST_SESSION_UPLOAD			    = "/offline/activity";
	
	public static final String REST_PROMOTED_APPS_SUFFIX		= "/advertiser_apps";
	public static final String REST_PROMOTED_APPS_CLICK			= "/api" + REST_PROMOTED_APPS_SUFFIX;
	public static final String REST_PROMOTED_APPS_CLICK_SUFFIX	= "/click";
	public static final String REST_LINKS_URL_SUFFIX		    = "/links";
	public static final String REST_TOYBOX_URL_SUFFIX		    = "/links/toybox";
	public static final String REST_FAVORITES_URL_SUFFIX	    = "/links/favorite";
	public static final String REST_PHOTOS_URL_SUFFIX		    = "/photo";
	public static final String REST_READINGS_URL_SUFFIX 	    = "/readings";
	public static final String REST_READINGS_ASK_URL_SUFFIX     = "/readings/ask";	
	public static final String REST_RELATIVES_URL_SUFFIX 	    = "/relatives";
	public static final String REST_SHOWS_SUFFIX			    = "/shows";
	public static final String REST_SITES_SUFFIX			    = "/sites";
	public static final String REST_DRAWINGS_SUFFIX             = "/drawings";
	public static final String REST_DRAWING_FAVORITE_SUFFIX     = "/favorite";
	public static final String REST_PURCHASE_SUFFIX             = "/purchase";
	public static final String REST_VIDEO_MAILS_URL_SUFFIX 	    = "/video_mails";
	public static final String REST_VIDEO_MAILS_READ_SUFFIX     = "/read";
	public static final String REST_SNAPSHOT_SUFFIX 			= "/snapshot";
	public static final String REST_VIDEO_MAILS_SENT_SUFFIX	    = "/sent";
	public static final String REST_PIN_CODE_SUFFIX	    		= "/pin";
	public static final String REST_HYPOTHESES_SUFFIX			= "/hypotheses";
	public static final String REST_GOAL_SUFFIX      			= "/goals";
	public static final String REST_EMERGENCY_EXIT_SUFFIX 		= "/pin_lock/emergency_exit";

	public static final String VIDSYNC_URL 					    = "/video/videos"; 
	public static final String VIDSYNC_STORY_URL 			    = "/video/story"; 
	
	public static final String REST_SUBSCRIPTION_PLAN	      	= "/api/subscription_plans";
	public static final String REST_SUBSCRIPTION_ACTIVATE      	= REST_SUBSCRIPTION + "/activate";
	public static final String REST_SUBSCRIPTION_CANCEL	      	= REST_SUBSCRIPTION + "/cancel";
	
	public static final String REST_TIMER_INFO_SUFFIX			= "/timer/info";
	public static final String REST_TIMER_SET_SUFFIX			= "/timer/set";
	
	public static final String REST_PROMOTE_LANGUAGES_SUFFIX	= "/languages";
	public static final String REST_PROMOTE_LANGUAGES_UPDATE	= REST_PROMOTE_LANGUAGES_SUFFIX + "/enable";
	public static final String REST_VERIFY_VPC_SUFFIX 			= "/verify";
	public static final String REST_AGE_GATE_URL				= REST_USER_URL + "/age_gate";
	
	public static final String REST_TIME_FOR_SUBJECTS_SUFFIX	= "/overview/subject_pie";
	
	//////////////////////////////////////////////////////////////////////
	// REST API Form parameters
	//////////////////////////////////////////////////////////////////////
	
	public static final String PARAM_KID_ID							= "kid_id";
	public static final String PARAM_LINK_ID						= "link_id";
	public static final String PARAM_CLIENT_ID						= "client_id";
	public static final String PARAM_TOKEN							= "token";
	public static final String PARAM_LOGIN							= "login";
	public static final String PARAM_PASSWORD						= "password";
	public static final String PARAM_DURATION						= "duration";
	public static final String PARAM_CLOSED							= "closed";
	public static final String PARAM_BLOCKED						= "blocked";
	public static final String PARAM_URL							= "url";
	public static final String PARAM_EMAIL							= "email";
	public static final String PARAM_EMAILS							= "emails";
	public static final String PARAM_NAME							= "name";
	public static final String PARAM_BIRTHDAY						= "birthday";
	public static final String PARAM_ALLOW_VIDEO_MAIL				= "allow_video_mail";
	public static final String PARAM_RECIPIENT_ID				    = "recipient_id";
	public static final String PARAM_BOOK_ID				        = "book_id";
	public static final String PARAM_KID_IDS						= "kid_ids";
	
	public static final String PARAM_FAMILY							= "family";
	public static final String PARAM_OS_VERSION						= "os_version";
	public static final String PARAM_FLASH							= "flash_installed";
	public static final String PARAM_DENSITY						= "screen_density";
	public static final String PARAM_SIZE							= "screen_size";
	public static final String PARAM_BRAND       				    = "brand";
	public static final String PARAM_DEVICE      				    = "device";
	public static final String PARAM_MODEL							= "model";
	public static final String PARAM_SCREEN_WIDTH					= "screen_width";
	public static final String PARAM_SCREEN_HEIGHT					= "screen_height";
	public static final String PARAM_MANUFACTURER					= "manufacturer";
	public static final String PARAM_TABLET							= "is_tablet";
	public static final String PARAM_SDK_VERSION					= "sdk_version";
	public static final String PARAM_INCREMENTAL_BUILD_VERSION		= "incremental_build_version";
	public static final String PARAM_HBL							= "hbl_enabled";
	public static final String PARAM_DURATION_SECONDS 				= "duration_seconds";
	public static final String PARAM_CHANNEL						= "channel";
	public static final String PARAM_FIXED							= "fixed";

	public static final String PARAM_MAX_VIOLENCE					= "max_violence";
	public static final String PARAM_BLOCKED_SITES					= "blocked_sites";
	public static final String PARAM_BLOCKED_SHOWS					= "blocked_shows";
	public static final String PARAM_PROMOTE_LANGUAGES				= "languages[]";
	
	public static final String PARAM_WEIGHT_COGNITIVE_DEVELOPMENT	= "weight_cognitive_development";
	public static final String PARAM_WEIGHT_CREATIVE_DEVELOPMENT	= "weight_creative_development";
	public static final String PARAM_WEIGHT_LIFE_SKILLS				= "weight_life_skills";
	public static final String PARAM_WEIGHT_MATH					= "weight_math";
	public static final String PARAM_WEIGHT_READING					= "weight_reading";
	public static final String PARAM_WEIGHT_SCIENCE					= "weight_science";
	public static final String PARAM_WEIGHT_SOCIAL_STUDIES			= "weight_social_studies";
	
	public static final String PARAM_HYPOTHESIS						= "hypothesis";

	public static final String PARAM_CRASH_DATA						= "crash_data";
	public static final String PARAM_APPS							= "apps";
	public static final String PARAM_APP_ID							= "advertiser_app_id";
	public static final String PARAM_APP_INCLUDE_PROMOS				= "include_promos";
	public static final String PARAM_APP_NAME						= "name";
	public static final String PARAM_APP_PACKAGE					= "packages";
	public static final String PARAM_APP_PACKAGE_NAME				= "package_name";
	public static final String PARAM_KID_AGE						= "age";
	public static final String PARAM_LANGUAGE						= "language";
	public static final String PARAM_COUNTRY						= "country";
	
	public static final String PARAM_PLACEMENT						= "placement";
	public static final String PARAM_KNOWN_AS	 					= "known_as";
	public static final String PARAM_DRAWING_FAVORITE				= "favorite";

	public static final String PARAM_PIN_CODE						= "pin";
	
	public static final String PARAM_TRANSACTION_ID					= "transaction_id";
	public static final String PARAM_BILLING_ID						= "billing_id";
	public static final String PARAM_TERM							= "term";
	
	public static final String PARAM_TIMEZONE_OFFSET				= "time_zone_offset";
	public static final String PARAM_WEEKDAY_LIMIT					= "weekday_limit";
	public static final String PARAM_WEEKDAY_DISABLED				= "weekday_disabled";
	public static final String PARAM_WEEKEND_LIMIT					= "weekend_limit";
	public static final String PARAM_WEEKEND_DISABLED				= "weekend_disabled";
	
	// For Verified Parental Consent (VPC)
	public static final String PARAM_CC_NUMBER						= "card_number";
	public static final String PARAM_CC_EXP_MONTH					= "expiration_month";
	public static final String PARAM_CC_EXP_YEAR					= "expiration_year";
	public static final String PARAM_AGE_GATE 						= "pass";

	public static final String SYSTEM_UI_PACKAGE = "com.android.systemui";

	public static final String KID_BROWSER_PACKAGE = "com.zoodles.gamesplayer";

	//////////////////////////////////////////////////////////////////////
	// REST API Form parameter values
	//////////////////////////////////////////////////////////////////////
	
	public static final String VALUE_TIP_PRE =  "preroll";
	public static final String VALUE_TIP_POST = "postroll";

	//////////////////////////////////////////////////////////////////////
	// Database / REST API Refresh Times (in seconds)
	//////////////////////////////////////////////////////////////////////

	public static final int TTL_BOOKS               = 60 * 60 * 24; // 1 day
	public static final int TTL_BOOK_READINGS       = 60 * 15;      // 15 minutes
	public static final int TTL_RELATIVES           = 60 * 15;      // 15 minutes
	public static final int TTL_VIDEO_MAILS         = 60 * 15;      // 15 minutes
	public static final int TTL_GAMES 				= 60 * 60; 		// 1 hour
	public static final int TTL_ART 				= 60 * 60;		// 1 hour 
	public static final int TTL_KIDS 				= 60 * 60;      // 1 hour
	public static final int TTL_APPS               	= 60 * 60 * 24 * 7; // 1 week
	public static final int TTL_REVIEWS             = 60 * 60 * 24 * 7; // 1 week
	
	//////////////////////////////////////////////////////////////////////
	// Email constants for support/feedback options.
	//////////////////////////////////////////////////////////////////////
	
	public static final String ZOODLES_SUPPORT					= "support";
	public static final String ZOODLES_SUPPORT_QUESTION_EMAIL  	= "support+question";
	public static final String ZOODLES_SUPPORT_PROBLEM_EMAIL	= "support+problem";
	public static final String ZOODLES_SUPPORT_IDEA_EMAIL		= "support+idea";
	public static final String ZOODLES_SUPPORT_COMPLIMENT_EMAIL = "support+compliment";
	public static final String ZOODLES_SUPPORT_APPS				= "support+apps";
	public static final String ZOODLES_EMAIL_DOMAIN 	= "@zoodles.com";
	
	//////////////////////////////////////////////////////////////////////
	// YouTube Introduction videos.
	//////////////////////////////////////////////////////////////////////
	
	public static final String YOUTUBE_INTRO_VIDEO_ID  = "E0RwV5zVuAY";				// Intro video for phone devices
	public static final String YOUTUBE_TABLET_INTRO_VIDEO_ID  = "pdlGFLNEGhY";		// Intro video for tablet devices
	public static final String ZOODLES_YOUTUBE_API_URL = "/api/html/youtube";
	public static final String YOUTUBE_DEVELOPER_KEY		= "AIzaSyCoqlNGHuQHfcUqGc_necDaTNuauoBqtfI"; // Developer API key for accessing native YouTube Player
	//////////////////////////////////////////////////////////////////////
	// Facebook
	//////////////////////////////////////////////////////////////////////

	public static final String FACEBOOK_APP_ID        = "164556459504";
	
	//////////////////////////////////////////////////////////////////////
	// Other shared constants
	//////////////////////////////////////////////////////////////////////
	public static final int SECS_IN_MINUTE = 60;
	public static final int MINUTES_5 = 5 * SECS_IN_MINUTE;
	public static final int MINUTES_10 = 10 * SECS_IN_MINUTE;
	public static final int MINUTES_20 = 20 * SECS_IN_MINUTE;
	public static final int MINUTES_30 = 30 * SECS_IN_MINUTE;
	public static final int HOURS_1 = 60 * SECS_IN_MINUTE;
	public static final int HOURS_2 = 2 * 60 * SECS_IN_MINUTE;
	public static final int HOURS_3 = 3 * 60 * SECS_IN_MINUTE;
	public static final int HOURS_4 = 4 * 60 * SECS_IN_MINUTE;

	public static final int MAX_KIDS = 6;
	public static final int KID_MIN_CHARS = 1;
	public static final int KID_MAX_CHARS = 64;
}
