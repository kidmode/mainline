using UnityEngine;
using System.Collections;

public class ZoodlesConstants 
{
    //VPC info
    public const int USER_VPC_NONE = 0;
	public const int USER_VPC_EMAIL = 1;
	public const int USER_VPC_FULL = 2;
	
	//////////////////////////////////////////////////////////////////////
	// URL stuff
	//////////////////////////////////////////////////////////////////////

    public const string QUESTION_MARK                       = "?";
    public const string AMPERSAND                           = "&";
    public const string EQUALS                              = "=";
    public const string SLASH                               = "/";

	//////////////////////////////////////////////////////////////////////
	// WWW Urls
	//////////////////////////////////////////////////////////////////////

	public const string ZOODLES_BILLING_URL                = "/parent/android";
	public const string ZOODLES_UPSELL_URL                 = "/parent/android/upsell";
	public const string ZOODLES_TOS_URL                    = "/home/legal/terms_android";
	public const string ZOODLES_PRIVACY_POLICY_URL         = "/home/legal/privacy";
	
	//////////////////////////////////////////////////////////////////////
	// REST API URLs
	//
	// * All URL strings start with "/"
	// * No URL strings end with "/"
	//////////////////////////////////////////////////////////////////////
	
	public const string REST_BOOKS_URL 				        = "/api/books";
	public const string REST_CLIENT_ID_URL			        = "/api/clients";
	public const string REST_LOCALE_URL			            = "/api/locale";
	public const string REST_KIDS_URL				        = "/api/kids/kid_list";
	public const string REST_KID_URL				        = "/api/kids/kid_show";
	public const string REST_PARENT_URL				        = "/api/parent/kids";
	public const string REST_LOGIN_URL				        = "/api/token";
	public const string REST_NEW_LOGIN_URL				    = "/api/user/signin";
	public const string REST_USER_URL				        = "/api/user/signup";
	public const string REST_USER_EMAIL_URL			        = "/api/emails";
	public const string REST_FORGOT_PASSWORD			    = "/api/user/forgot";
	public const string REST_CRASHES					    = "/api/crashes";
	public const string REST_READING_REQUESTS	            = "/api/reading_requests";
	public const string REST_TIP					        = "/api/html/tips";
	public const string REST_READINGS					    = "/api/readings";
	public const string REST_APP_APPROVED     			    = "/api/apps";
	public const string REST_APP_RECOMMENDATIONS		    = "/api/apps/recommendations";
	public const string REST_APP_RECOMMEND_NEW		  		= "/api/apps/recommended_apps_new";
	public const string REST_APP_REVIEW      			    = "/api/apps/reviews";
	public const string REST_APP_ALLOWED      			    = "/api/apps/allowed";
	public const string REST_EXPERIMENT_URL      		    = "/api/experiments";
	public const string REST_SUBSCRIPTION	      		    = "/api/subscriptions";
	public const string REST_VIEW_GEMS		      		    = "/api/gems_amount/gems_amount";
	public const string REST_SESSION_UPLOAD			        = "/offline/activity";
	public const string REST_TRY_TRIAL				        = "/api/subscriptions/get_trial";
	
	public const string REST_PROMOTED_APPS_SUFFIX           = "/advertiser_apps";
	public const string REST_PROMOTED_APPS_CLICK			= "/api" + REST_PROMOTED_APPS_SUFFIX;
	public const string REST_PROMOTED_APPS_CLICK_SUFFIX	    = "/click";
	public const string REST_LINKS_URL_SUFFIX		        = "/links";
	public const string REST_LINKS_LIST_URL_SUFFIX		    = "/links/link_list";
	public const string REST_TOYBOX_URL_SUFFIX		        = "/links/toybox";
	public const string REST_FAVORITES_URL_SUFFIX	        = "/links/favorite";
	public const string REST_PHOTOS_URL_SUFFIX		        = "/photo";
	public const string REST_READINGS_URL_SUFFIX 	        = "/readings";
	public const string REST_READINGS_ASK_URL_SUFFIX        = "/readings/ask";	
	public const string REST_RELATIVES_URL_SUFFIX 	        = "/relatives";
	public const string REST_SHOWS_SUFFIX			        = "/shows";
	public const string REST_SITES_SUFFIX			        = "/sites";
	public const string REST_DRAWINGS_SUFFIX                = "/drawings";
	public const string REST_DRAWING_FAVORITE_SUFFIX        = "/favorite";
	public const string REST_PURCHASE_SUFFIX                = "/purchase";
	public const string REST_VIDEO_MAILS_URL_SUFFIX 	    = "/video_mails";
	public const string REST_VIDEO_MAILS_READ_SUFFIX        = "/read";
	public const string REST_SNAPSHOT_SUFFIX 			    = "/snapshot";
	public const string REST_VIDEO_MAILS_SENT_SUFFIX	    = "/sent";
	public const string REST_PIN_CODE_SUFFIX	    	    = "/pin";
	public const string REST_HYPOTHESES_SUFFIX			    = "/hypotheses";
	public const string REST_GOAL_SUFFIX      			    = "/goals";
	public const string REST_EMERGENCY_EXIT_SUFFIX 	        = "/pin_lock/emergency_exit";

	public const string VIDSYNC_URL 					    = "/video/videos"; 
	public const string VIDSYNC_STORY_URL 			        = "/video/story"; 
	             
	public const string REST_SUBSCRIPTION_ACTIVATE      	= REST_SUBSCRIPTION + "/activate";
	public const string REST_SUBSCRIPTION_CANCEL	      	= REST_SUBSCRIPTION + "/cancel";
	public const string REST_SUBSCRIPTION_CANCEL_NEW	    = "/api/subscriptions/cancel_subscription";
	            
	public const string REST_TIMER_INFO_SUFFIX			    = "/timer/info";
	public const string REST_TIMER_SET_SUFFIX			    = "/timer/set";
	public const string REST_CANCEL_LOCK 				    = "/api/pin/concel_lock_pin";
	public const string REST_ENABLE_LOCK 				    = "/api/pin/enable_lock_pin";
	public const string REST_UPDATE_SETTING				    = "/api/user_settings/update_settings";
	public const string EDIT_CHILD						    = "/api/kids/update_kid";

	public const string DELETE_CHILD						= "/api/kids/destroy_kid";

	             
	public const string REST_PROMOTE_LANGUAGES_SUFFIX	    = "/languages";
	public const string REST_PROMOTE_LANGUAGES_UPDATE	    = REST_PROMOTE_LANGUAGES_SUFFIX + "/enable";
	public const string REST_VERIFY_VPC_SUFFIX 		        = "/verify";
	public const string REST_AGE_GATE_URL				    = REST_USER_URL + "/age_gate";
	             
	public const string REST_TIME_FOR_SUBJECTS_SUFFIX	    = "/overview/subject_pie";


	public const string NEW_LOGIN_URL	    				= "/api/user/signin";
	public const string NEW_SIGNUP_URL	    				= "/api/user";
	public const string NEW_GREATE_CHILDREN_URL	    		= "/api/kids";

	public const string REST_BUY_BOOK 						= "/api/books/purchase_book";
	public const string SHOW_PLAN_DETAILS					= "/api/user/plan_details";
	public const string UPDATE_NOTIFICATION					= "/api/user_settings/update_settings";
	public const string GET_TOP_RECOMMEND_APP				= "/api/apps/top_app";
	public const string SEND_PIN							= "/api/pin/send_forgot_pin";
	public const string SEND_TELL_FRIEND_EMAIL				= "/api/send_emails/tell_friend";
	public const string CHECK_USERNAME						= "/api/user/check_email";
	//GCS Version uses it
	public const string CLIENTCERTNAME						= "/6453d681f746c56f68123468c8313ffe1612553e";
	//httpskeystore.p12 sh1 hash -> 645594c334480764443c8bcc6ad3f9ce4c8d3315
	public const string CLIENT_CERT							= "/645594c334480764443c8bcc6ad3f9ce4c8d3315";

	public const string REST_GET_FEATURED_CONTENTS			= "/api/feature_contents";

	public const string REST_GET_FEATURED_CONTENTS_EVENTS	= "api/feature_contents/track";

	public const string REST_GET_LOCK_PIN	= "/api/pin/send_forgot_lock_pin";

	///
	//////////////////////////////////////////////////////////////////////
	// REST API Form parameters
	//////////////////////////////////////////////////////////////////////
	
	public const string PARAM_KID_ID						= "kid_id";
	public const string PARAM_LINK_ID						= "link_id";
	public const string PARAM_CLIENT_ID					    = "client_id";
	public const string PARAM_TOKEN						    = "token";
	public const string PARAM_LOGIN						    = "login";
	public const string PARAM_PASSWORD						= "password";
	public const string PARAM_DURATION						= "duration";
	public const string PARAM_CLOSED						= "closed";
	public const string PARAM_BLOCKED						= "blocked";
	public const string PARAM_URL							= "url";
	public const string PARAM_EMAIL						    = "email";
	public const string PARAM_EMAILS						= "emails";
	public const string PARAM_NAME							= "name";
	public const string PARAM_BIRTHDAY						= "birthday";
	public const string PARAM_ALLOW_VIDEO_MAIL				= "allow_video_mail";
	public const string PARAM_RECIPIENT_ID				    = "recipient_id";
	public const string PARAM_BOOK_ID				        = "book_id";
	public const string PARAM_KID_IDS						= "kid_ids";
	public const string PARAM_PICTURE                       = "picture";
	public const string PARAM_CONTENT_TYPE                  = "contentType";

	public const string PARAM_FAMILY						= "family";
	public const string PARAM_OS_VERSION					= "os_version";
	public const string PARAM_FLASH						    = "flash_installed";
	public const string PARAM_DENSITY						= "screen_density";
	public const string PARAM_SIZE 							= "screen_size";
	public const string PARAM_BRAND       				    = "brand";
	public const string PARAM_DEVICE      				    = "device";
	public const string PARAM_MODEL						    = "model";
	public const string PARAM_SCREEN_WIDTH					= "screen_width";
	public const string PARAM_SCREEN_HEIGHT				    = "screen_height";
	public const string PARAM_MANUFACTURER					= "manufacturer";
	public const string PARAM_TABLET						= "is_tablet";
	public const string PARAM_SDK_VERSION					= "sdk_version";
	public const string PARAM_INCREMENTAL_BUILD_VERSION	    = "incremental_build_version";
	public const string PARAM_HBL							= "hbl_enabled";
	public const string PARAM_DURATION_SECONDS 			    = "duration_seconds";
	public const string PARAM_CHANNEL						= "channel";
	public const string PARAM_FIXED						    = "fixed";

	public const string PARAM_MAX_VIOLENCE					= "max_violence";
	public const string PARAM_BLOCKED_SITES				    = "blocked_sites";
	public const string PARAM_BLOCKED_SHOWS				    = "blocked_shows";
	public const string PARAM_PROMOTE_LANGUAGES			    = "languages[]";
	
	public const string PARAM_WEIGHT_COGNITIVE_DEVELOPMENT	= "weight_cognitive_development";
	public const string PARAM_WEIGHT_CREATIVE_DEVELOPMENT	= "weight_creative_development";
	public const string PARAM_WEIGHT_LIFE_SKILLS			= "weight_life_skills";
	public const string PARAM_WEIGHT_MATH					= "weight_math";
	public const string PARAM_WEIGHT_READING				= "weight_reading";
	public const string PARAM_WEIGHT_SCIENCE				= "weight_science";
	public const string PARAM_WEIGHT_SOCIAL_STUDIES		    = "weight_social_studies";
	
	public const string PARAM_HYPOTHESIS					= "hypothesis";

	public const string PARAM_CRASH_DATA					= "crash_data";
	public const string PARAM_APPS							= "apps";
	public const string PARAM_APP_ID						= "advertiser_app_id";
	public const string PARAM_APP_INCLUDE_PROMOS			= "include_promos";
	public const string PARAM_APP_PAGES						= "include_pages";
	public const string PARAM_APP_NAME						= "name";
	public const string PARAM_APP_PACKAGE					= "packages";
	public const string PARAM_APP_PACKAGE_NAME				= "package_name";
	public const string PARAM_KID_AGE						= "age";
	public const string PARAM_PAGE							= "page";
	public const string PARAM_OWNED_REQUEST					= "owned_request";
	public const string PARAM_LANGUAGE						= "language";
	public const string PARAM_COUNTRY						= "country";
	
	public const string PARAM_PLACEMENT					    = "placement";
	public const string PARAM_KNOWN_AS	 					= "known_as";
	public const string PARAM_DRAWING_FAVORITE				= "favorite";

	public const string PARAM_PIN_CODE						= "pin";
	
	public const string PARAM_TRANSACTION_ID				= "transaction_id";
	public const string PARAM_BILLING_ID					= "billing_id";
	public const string PARAM_TERM							= "term";
	
	public const string PARAM_TIMEZONE_OFFSET				= "time_zone_offset";
	public const string PARAM_WEEKDAY_LIMIT				    = "weekday_limit";
	public const string PARAM_WEEKDAY_DISABLED				= "weekday_disabled";
	public const string PARAM_WEEKEND_LIMIT				    = "weekend_limit";
	public const string PARAM_WEEKEND_DISABLED				= "weekend_disabled";
	
	// For Verified Parental Consent (VPC)
	public const string PARAM_CC_NUMBER					    = "card_number";
	public const string PARAM_CC_EXP_MONTH					= "expiration_month";
	public const string PARAM_CC_EXP_YEAR					= "expiration_year";
	public const string PARAM_AGE_GATE 					    = "pass";

	public const string SYSTEM_UI_PACKAGE                   = "com.android.systemui";

    public const string KID_BROWSER_PACKAGE                 = "com.zoodles.gamesplayer";

	public const string FLASH_PACKAGE                 		= "com.adobe.flashplayer";
	
	public const string NEW_PARAM_APP_ID					= "app_id";
	public const string ID									= "id";

	public const string CHILD_LOCK							= "child_lock";
	public const string LOCK_PIN							= "lock_pin";

	public const string INCOMING_CALL						= "incoming_calls";
	public const string TODAY_TIPS							= "today_tips";
	public const string MASTER_VOLUME						= "master_volume";
	public const string MUSIC_VOLUM							= "music_volume";
	public const string EFFECTS_VOLUME						= "effects_volume";
	
	public const string PUSH_NEW_APPS_ADDED					= "push_new_apps_added";
	public const string PUSH_SMART_SELECTION				= "push_smart_selection";
	public const string PUSH_FREE_WEEKLY_APPS				= "push_free_weekly_apps";
	public const string GOOGLE								= "google";
	public const string FROM								= "from";
	public const string TO									= "to";
	public const string OPTIONAL_MESSAGE					= "optional_message";

	public const string CREDIT_CARD_NUMBER					= "creditcard[number]";
	public const string CREDIT_CARD_TYPE					= "creditcard[type]";
	public const string CREDIT_CARD_MONTH					= "creditcard[month]";
	public const string CREDIT_CARD_YEAR					= "creditcard[year]";

	public const string IAB_PACKAGE							= "package_name";
	public const string IAB_ORDER_ID						= "billing_id";
	public const string IAB_SKU								= "product_id";
	public const string IAB_PURCHASE_TOKEN					= "purchase_token";
	public const string IAB_DEVELOPER_PAYLOAD				= "developer_payload";
	
	//////////////////////////////////////////////////////////////////////
	// REST API Form parameter values
	//////////////////////////////////////////////////////////////////////
	
	public const string VALUE_TIP_PRE          =  "preroll";
	public const string VALUE_TIP_POST         = "postroll";

	//////////////////////////////////////////////////////////////////////
	// Database / REST API Refresh Times (in seconds)
	//////////////////////////////////////////////////////////////////////

	public const int TTL_BOOKS                  = 60 * 60 * 24; // 1 day
	public const int TTL_BOOK_READINGS          = 60 * 15;      // 15 minutes
	public const int TTL_RELATIVES              = 60 * 15;      // 15 minutes
	public const int TTL_VIDEO_MAILS            = 60 * 15;      // 15 minutes
	public const int TTL_GAMES 				    = 60 * 60; 		// 1 hour
	public const int TTL_ART 				    = 60 * 60;		// 1 hour 
	public const int TTL_KIDS 				    = 60 * 60;      // 1 hour
	public const int TTL_APPS                   = 60 * 60 * 24 * 7; // 1 week
	public const int TTL_REVIEWS                = 60 * 60 * 24 * 7; // 1 week
	
	//////////////////////////////////////////////////////////////////////
	// Email constants for support/feedback options.
	//////////////////////////////////////////////////////////////////////
	
	public const string ZOODLES_SUPPORT					    = "support";
	public const string ZOODLES_SUPPORT_QUESTION_EMAIL  	= "support+question";
	public const string ZOODLES_SUPPORT_PROBLEM_EMAIL	    = "support+problem";
	public const string ZOODLES_SUPPORT_IDEA_EMAIL		    = "support+idea";
	public const string ZOODLES_SUPPORT_COMPLIMENT_EMAIL    = "support+compliment";
	public const string ZOODLES_SUPPORT_APPS				= "support+apps";
	public const string ZOODLES_EMAIL_DOMAIN 	            = "@zoodles.com";
	
	//////////////////////////////////////////////////////////////////////
	// YouTube Introduction videos.
	//////////////////////////////////////////////////////////////////////
	
	public const string YOUTUBE_INTRO_VIDEO_ID              = "E0RwV5zVuAY";				// Intro video for phone devices
	public const string YOUTUBE_TABLET_INTRO_VIDEO_ID       = "pdlGFLNEGhY";		// Intro video for tablet devices
	public const string ZOODLES_YOUTUBE_API_URL             = "/api/html/youtube";
    public const string YOUTUBE_EMBEDED_URL                 = "https://www.youtube.com/embed/";
    public const string YOUTUBE_NO_RELATED_SUFFEX           = "?rel=0";
	public const string YOUTUBE_DEVELOPER_KEY		        = "AIzaSyCoqlNGHuQHfcUqGc_necDaTNuauoBqtfI"; // Developer API key for accessing native YouTube Player
	//////////////////////////////////////////////////////////////////////
	// Facebook
	//////////////////////////////////////////////////////////////////////

	public const string FACEBOOK_APP_ID                  = "164556459504";

	//////////////////////////////////////////////////////////////////////
	// Facebook
	//////////////////////////////////////////////////////////////////////
	public const string ZPS_LEVEL			 = "level_zps";
	public const string EXPERIENCE_POINTS 	 = "experience_points";
	public const string TAGS 				 = "tags";
	public const string CATEGORIES			 = "categories";
	public const string SUBJECTS			 = "subjects";
	public const string ADD_CHILD_TEXT		 = "Add A Child";
	public const string EMAIL_REGULAR_STRING = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
	public const string DATE_REGULAR_NUMBER  = @"^[0-9]*$";
	public static bool  IS_DEBUG     	     = false;
	
	//////////////////////////////////////////////////////////////////////
	// Other shared constants
	//////////////////////////////////////////////////////////////////////
	public const int SECS_IN_MINUTE  = 60;
	public const int MINUTES_5       = 5        * SECS_IN_MINUTE;
	public const int MINUTES_10      = 10       * SECS_IN_MINUTE;
	public const int MINUTES_20      = 20       * SECS_IN_MINUTE;
	public const int MINUTES_30      = 30       * SECS_IN_MINUTE;
	public const int HOURS_1         = 60       * SECS_IN_MINUTE;
	public const int HOURS_2         = 2 * 60   * SECS_IN_MINUTE;
	public const int HOURS_3         = 3 * 60   * SECS_IN_MINUTE;
	public const int HOURS_4         = 4 * 60   * SECS_IN_MINUTE;

	public const int MAX_KIDS        = 6;
	public const int KID_MIN_CHARS   = 1;
	public const int KID_MAX_CHARS   = 64;

    public const int MAX_BOOK_CONTENT   = 100;
    public const int MAX_GAME_CONTENT   = 100;
    public const int MAX_VIDEO_CONTENT  = 100;

	/////////////////////////////////////////////////////////////////
	// User info
	/////////////////////////////////////////////////////////////////
	public const string USER_NAME			= "UserName";
	public const string USER_PIN			= "UserPin";
	public const string CHILD_LOCK_PASSWORD = "UserLockPassword";
	public const string USER_EXIST			= "UserExist";
	public const string USER_TOKEN			= "UserToken";
	public const string USER_PREMIUM		= "UserPremium";
	public const string USER_TRY			= "UserTry";
	public const string USER_CURRENT		= "UserCurrent";
	public const string USER_VERIFY_BIRTHYEAR = "UserVerifyBirthYear";
	public const string USER_CHILDLOCK_SWITCH = "UserChildLockSwitch";

	/////////////////////////////////////////////////////////////////
	// Symbol
	/////////////////////////////////////////////////////////////////
	public const string BLANK			= " ";
	public const string MIDDLE_LINE		= "-";



	/////////////////////////////////////////////////////////////////
	// Player Prefs for local info saves
	/////////////////////////////////////////////////////////////////
	public const string PLAYERPREF_PREMIMUMTRIAL_START_TIME			= "premiumTrialStartTime";


    public static string getHost()
    {
        return GCS.Environment.getHost(); 
    }

	public static string getHttpsHost()
	{
		return GCS.Environment.getSecureHost(); 
	}
}
