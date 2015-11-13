using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIScreen
{
	public const int NO_SCREEN 					= 0;
	public const int SPLASH 					= 1;
	public const int PROFILE_SELECTION 			= 2;
	public const int ENTRANCE 					= 3;
	public const int SPLASH_BACKGROUND 			= 4;
	public const int LOADING_ENTRANCE 			= 5;
	public const int CORNER_PROFILE_INFO 		= 6;
	public const int MAP 						= 7;
	public const int REGION_LANDING 			= 8;
	public const int REGION_LANDING_BACKGROUND 	= 9;
	public const int ACTIVITY_PANEL			 	= 10;
	public const int VIDEO_ACTIVITY				= 11;
	public const int GAME_ACTIVITY				= 12;
	public const int ZOODLES_INTRO				= 13;
	public const int BOOK_ACTIVITY				= 14;
	public const int PAINT_ACTIVITY				= 15;
	public const int BOOK_READER				= 16;
    public const int BOOK_PAGE                  = 17;
	public const int CREATE_ACCOUNT_SELECTION	= 18;
	public const int CREATE_ACCOUNT_BACKGROUND	= 19;
	public const int SET_ACCOUNT				= 20;
	public const int SIGN_IN					= 21;
	public const int BIRTHYEAR					= 22;
	public const int CREATE_CHILDPROFILE		= 23;
	public const int SELECT_AVATAR				= 24;
	public const int SET_BIRTHYEAR				= 25;
	public const int DASHBOARD_COMMON 			= 26;
	public const int APP_INFO		 			= 27;
	public const int LEFT_MENU 					= 28;

	public const int PLAN_DEATILS				= 30;
	public const int CANCEL_SUB					= 31;
	public const int ERROR						= 32;
    public const int FUN_ACTIVITY               = 33;
	public const int UPSELL_SPLASH 				= 34;
	public const int VIEW_PREMIUM 				= 35;
	public const int BUY_GEMS	 				= 36;
	public const int PAYMENT 	 				= 37;
	public const int PAY_CONFIRM 	 			= 38;
	public const int THANK		 	 			= 39;
	public const int SIGN_OUT		 	 		= 40;
	public const int SENT_FEED_BACK	 	 		= 41;
	public const int HOME_RESET					= 42;
	public const int HOME_REQUEST				= 43;
	public const int CHILD_LOCK_SCREEN			= 44;
	public const int NOTIFICATION				= 45;
	public const int SETTING_COMMON				= 46;
	public const int COMMON_DIALOG				= 47;
	public const int FAQ_SCREEN					= 48;
	public const int DEVICE_OPTIONS_SCREEN		= 49;

	public const int KIDS_PROFILE	 	 		= 51;

	public const int PROFILE_ACTIVE	 	 		= 54;
	public const int DASHBOARD_CONTROLLER 		= 55;
	public const int DASHBOARD_INFO 			= 56;
	public const int DASHBOARDL_PROGRESS 		= 57;
	public const int SIGN_IN_UPSELL				= 58;
	public const int TIME_SPEND 				= 59;
	public const int SPLASH_PREMIUM				= 60;
	public const int PROMOTE_SUBJECTS 			= 61;
	public const int PROMOTE_LANGUAGES 			= 62;
	public const int TIME_LIMITS 				= 63;
	public const int VIOLENCE_FILTERS 			= 64;
	public const int SIGIN_UP_UPSELL 			= 65;
	public const int UPSELL_CONGRATURATION 		= 66;
	public const int STAR_CHART 				= 67;
	public const int RECOMMENDED_APP 			= 68;
	public const int RECOMMENDED_BOOK 			= 69;
	public const int APP_DETAILS 				= 70;
	public const int BOOK_DETAILS 				= 71;
	public const int APP_LIST 					= 72;
	public const int BOOK_LIST 					= 73;
	public const int ADD_APPS 					= 74;
	public const int CONFIRM_DIALOG 			= 75;
	public const int ADD_SITE 					= 76;
	public const int ART_GALLERY 				= 77;
	public const int ART_LIST 					= 78;
	public const int LOADING_SPINNER			= 79;
	public const int RECORD_START 				= 80;
	public const int RECORD_AGAIN 				= 81;
	public const int RECORD_FINISH 				= 82;
	public const int CONGRATS					= 83;
	public const int CONGRATS_BACKGROUND		= 84;
	public const int WEBVIEW					= 85;
	public const int PROFILE_VIEW				= 86;
	public const int CHILD_LOCK_HELP			= 87;
	public const int FAQ_DIALOG 				= 88;
	public const int CANCEL_CHILD_LOCK 			= 89;
	public const int LEGAL_CONTENT				= 90;
	public const int MESSAGE 					= 91;
	public const int UPGRADE					= 92;
	public const int PAINT_VIEW 				= 93;
	public const int TELL_FRIEND 				= 94;
	public const int CONNECTION_ERROR			= 95;
	public const int ADD_FRIEND					= 96;
	public const int PAYWALL 					= 97;
	public const int DASHBOARD_READING 			= 98;
	public const int CREATE_CHILD_NEW 			= 99;
	public const int SIGN_IN_FREE 				= 100;
	public const int PANEL_MARKETING_SCREEN 	= 101;
	public const int SIGN_UP_AFTER_INPUT_CREDITCARD = 102;
	public const int TRIAL_MESSAGE 				= 103;
	public const int CONGRATURATION				= 104;
	public const int PREMIUM_ELIGIBLE 			= 105;
	public const int PAY_GEMS_COMFIRM			= 106;
	
	// Sean: vzw
	public const int REGION_APP					= 901;

	// Sean: vzw
	public const int TABLET_SETTINGS			= 1001;

	//Tutorials
	public const int TUTORIAL_MAIN_PROCESS				= 1101;
	public const int TUTORIAL_CONTROL_VIOLENCE			= 1102;

	//Honda: Error Message
	public const int ERROR_MESSAGE			= 1201;
	public const int NO_INTERNET            = 1202;
	public const int LOADING_PAGE           = 1203;
	public const int TIMES_UP				= 1204;
	//Cathy : Loading Spinner
	public const int LOADING_SPINNER_ELEPHANT        = 1205;
	//Honda
	public const int FOTA_POPUP				= 1206;
	public const int PARENT_BIRTH_CHECK     = 1207;
	//honda: Debug Mode
	public const int ADD_ITEMS_MANUALLY     = 1208;
}


public class ZoodlesScreenFactory : IScreenFactory
{
	public const float FADE_SPEED = 0.4f;

	public ZoodlesScreenFactory()
	{
		const string SCREEN_DIRECTORY = "Prefabs/Screens/";

		m_directoryMap = new Dictionary<int, string>();
		m_directoryMap.Add( UIScreen.SPLASH, 					SCREEN_DIRECTORY + "SplashScreen" 			);
		m_directoryMap.Add( UIScreen.PROFILE_SELECTION, 		SCREEN_DIRECTORY + "ProfileScreen" 			);
		m_directoryMap.Add( UIScreen.ENTRANCE, 					SCREEN_DIRECTORY + "EntranceScreen" 		);
		m_directoryMap.Add( UIScreen.SPLASH_BACKGROUND,			SCREEN_DIRECTORY + "SplashBackgroundScreen" );
		m_directoryMap.Add( UIScreen.LOADING_ENTRANCE,			SCREEN_DIRECTORY + "LoadingEntranceScreen" 	);
		m_directoryMap.Add( UIScreen.CORNER_PROFILE_INFO,		SCREEN_DIRECTORY + "CornerProfileScreen" 	);
		m_directoryMap.Add( UIScreen.MAP,						SCREEN_DIRECTORY + "MapScreen"	 			);
		m_directoryMap.Add( UIScreen.REGION_LANDING,			SCREEN_DIRECTORY + "RegionLandingScreen"	);
		m_directoryMap.Add( UIScreen.REGION_LANDING_BACKGROUND,	SCREEN_DIRECTORY + "RegionLandingBackScreen");
		m_directoryMap.Add( UIScreen.ACTIVITY_PANEL,			SCREEN_DIRECTORY + "ActivityPanelScreen"	);
		m_directoryMap.Add( UIScreen.VIDEO_ACTIVITY,			SCREEN_DIRECTORY + "VideoActivityScreen"	);
		m_directoryMap.Add( UIScreen.GAME_ACTIVITY,				SCREEN_DIRECTORY + "GameActivityScreen"		);
		m_directoryMap.Add( UIScreen.ZOODLES_INTRO,				SCREEN_DIRECTORY + "ZoodlesIntroScreen"		);
		m_directoryMap.Add( UIScreen.BOOK_ACTIVITY,			    SCREEN_DIRECTORY + "BookActivityScreen"	    );
		m_directoryMap.Add( UIScreen.PAINT_ACTIVITY,			SCREEN_DIRECTORY + "PaintActivityScreen"	);
		m_directoryMap.Add( UIScreen.CREATE_ACCOUNT_SELECTION,	SCREEN_DIRECTORY + "CreateAccountScreen"	);
		m_directoryMap.Add( UIScreen.CREATE_ACCOUNT_BACKGROUND,	SCREEN_DIRECTORY + "SignUpBackgroundScreen" );
		m_directoryMap.Add( UIScreen.SET_ACCOUNT,				SCREEN_DIRECTORY + "SetAccountByEmailScreen");
		m_directoryMap.Add( UIScreen.BOOK_READER,			    SCREEN_DIRECTORY + "BookReaderScreen"   	);
		m_directoryMap.Add( UIScreen.BOOK_PAGE,			        SCREEN_DIRECTORY + "BookPageScreen"        	);
		m_directoryMap.Add( UIScreen.SIGN_IN,			        SCREEN_DIRECTORY + "SignInScreen"        	);
		m_directoryMap.Add( UIScreen.BIRTHYEAR,			        SCREEN_DIRECTORY + "BirthYearScreen"       	);
		m_directoryMap.Add( UIScreen.CREATE_CHILDPROFILE,		SCREEN_DIRECTORY + "CreateChildScreen"      );
		m_directoryMap.Add( UIScreen.SELECT_AVATAR,				SCREEN_DIRECTORY + "AddProfileScreen"      	);
		m_directoryMap.Add( UIScreen.SET_BIRTHYEAR,				SCREEN_DIRECTORY + "BirthYearSetScreen"    	);
		m_directoryMap.Add( UIScreen.DASHBOARD_COMMON,			SCREEN_DIRECTORY + "DashBoard/DashboardCommonScreen");
		m_directoryMap.Add( UIScreen.APP_INFO,					SCREEN_DIRECTORY + "DashBoard/Setting/AppInfoScreen");
		m_directoryMap.Add( UIScreen.LEFT_MENU,					SCREEN_DIRECTORY + "DashBoard/LeftMenuScreen");
		m_directoryMap.Add( UIScreen.PLAN_DEATILS,				SCREEN_DIRECTORY + "DashBoard/Setting/Dialog/PlanDetailsScreen");
		m_directoryMap.Add( UIScreen.CANCEL_SUB,				SCREEN_DIRECTORY + "DashBoard/Setting/Dialog/CancelSubscriptionScreen");
		m_directoryMap.Add( UIScreen.ERROR,						SCREEN_DIRECTORY + "ErrorScreen"    	     );
		m_directoryMap.Add( UIScreen.FUN_ACTIVITY,				SCREEN_DIRECTORY + "FunActivityScreen"    	 );
		m_directoryMap.Add( UIScreen.UPSELL_SPLASH,				SCREEN_DIRECTORY + "Upsell/UpsellSplashScreen" );
		m_directoryMap.Add( UIScreen.VIEW_PREMIUM,				SCREEN_DIRECTORY + "Upsell/UpsellPremiunScreen");
		m_directoryMap.Add( UIScreen.BUY_GEMS,					SCREEN_DIRECTORY + "Upsell/UpsellGemsScreen"   );
		m_directoryMap.Add( UIScreen.PAYMENT,					SCREEN_DIRECTORY + "Upsell/UpsellPaymentScreen");
		m_directoryMap.Add( UIScreen.PAY_CONFIRM,				SCREEN_DIRECTORY + "Upsell/PaymentConfirmation");
		m_directoryMap.Add( UIScreen.PAY_GEMS_COMFIRM,			SCREEN_DIRECTORY + "Upsell/PaymentGemsConfirmation");
		m_directoryMap.Add( UIScreen.THANK,						SCREEN_DIRECTORY + "DashBoard/Setting/Dialog/ThankDialogScreen");
		m_directoryMap.Add( UIScreen.SIGN_OUT,					SCREEN_DIRECTORY + "DashBoard/Setting/Dialog/SignOutScreen");
		m_directoryMap.Add( UIScreen.SENT_FEED_BACK,			SCREEN_DIRECTORY + "DashBoard/Setting/Dialog/SendFeedbackScreen");
		m_directoryMap.Add( UIScreen.HOME_REQUEST,				SCREEN_DIRECTORY + "ChildLock/ChildLockHomeSelectionScreen");
		m_directoryMap.Add( UIScreen.HOME_RESET,				SCREEN_DIRECTORY + "ChildLock/ChildLockResetHomeScreen");
		m_directoryMap.Add( UIScreen.CHILD_LOCK_SCREEN,			SCREEN_DIRECTORY + "DashBoard/Setting/ChildLockScreen");
		m_directoryMap.Add( UIScreen.NOTIFICATION,				SCREEN_DIRECTORY + "DashBoard/Setting/NotificationScreen");
		m_directoryMap.Add( UIScreen.SETTING_COMMON,			SCREEN_DIRECTORY + "DashBoard/SettingCommonScreen");
		m_directoryMap.Add( UIScreen.COMMON_DIALOG,				SCREEN_DIRECTORY + "DashBoard/Setting/Dialog/CommonDialogScreen");
		m_directoryMap.Add( UIScreen.FAQ_SCREEN,				SCREEN_DIRECTORY + "DashBoard/Setting/FAQScreen");
		m_directoryMap.Add( UIScreen.DEVICE_OPTIONS_SCREEN,		SCREEN_DIRECTORY + "DashBoard/Setting/DeviceOptionScreen");
		m_directoryMap.Add( UIScreen.DASHBOARDL_PROGRESS,		SCREEN_DIRECTORY + "DashBoard/Profile/Overview/OverallProgressScreen");
		m_directoryMap.Add( UIScreen.KIDS_PROFILE,				SCREEN_DIRECTORY + "KidsProfile/KidsProfileScreen");
		m_directoryMap.Add( UIScreen.PROFILE_ACTIVE,			SCREEN_DIRECTORY + "KidsProfile/ProfileActiveScreen");
		m_directoryMap.Add( UIScreen.DASHBOARD_CONTROLLER,		SCREEN_DIRECTORY + "DashBoard/DashboardControllerScreen");
		m_directoryMap.Add( UIScreen.DASHBOARD_INFO,			SCREEN_DIRECTORY + "DashBoard/Profile/Overview/InfoScreen");
		m_directoryMap.Add( UIScreen.SIGN_IN_UPSELL,			SCREEN_DIRECTORY + "SignInUpSellScreen");
		m_directoryMap.Add( UIScreen.TIME_SPEND,				SCREEN_DIRECTORY + "DashBoard/Profile/Overview/TimeSpendScreen");
		m_directoryMap.Add( UIScreen.SPLASH_PREMIUM,			SCREEN_DIRECTORY + "SplashPremiumScreen");
		m_directoryMap.Add( UIScreen.PROMOTE_SUBJECTS,			SCREEN_DIRECTORY + "DashBoard/Profile/Controls/PromoteSubjectsScreen");
		m_directoryMap.Add( UIScreen.PROMOTE_LANGUAGES,			SCREEN_DIRECTORY + "DashBoard/Profile/Controls/PromoteLanguagesScreen");
		m_directoryMap.Add( UIScreen.TIME_LIMITS,				SCREEN_DIRECTORY + "DashBoard/Profile/Controls/TimeLimitsScreen");
		m_directoryMap.Add( UIScreen.VIOLENCE_FILTERS,			SCREEN_DIRECTORY + "DashBoard/Profile/Controls/ViolenceFiltersScreen");
		m_directoryMap.Add( UIScreen.SIGIN_UP_UPSELL,			SCREEN_DIRECTORY + "CreateAccountUpSellScreen");
		m_directoryMap.Add( UIScreen.UPSELL_CONGRATURATION,		SCREEN_DIRECTORY + "UpsellCongratulationScreen");
		m_directoryMap.Add( UIScreen.STAR_CHART,				SCREEN_DIRECTORY + "DashBoard/Profile/StarChart/StarChartScreen");
		m_directoryMap.Add( UIScreen.RECOMMENDED_APP,			SCREEN_DIRECTORY + "DashBoard/Profile/Overview/RecommendedAppScreen");
		m_directoryMap.Add( UIScreen.RECOMMENDED_BOOK,			SCREEN_DIRECTORY + "DashBoard/Profile/Overview/RecommendedBookScreen");
		m_directoryMap.Add( UIScreen.APP_DETAILS,				SCREEN_DIRECTORY + "DashBoard/Profile/Overview/AppDetailDialogScreen");
		m_directoryMap.Add( UIScreen.BOOK_DETAILS,				SCREEN_DIRECTORY + "DashBoard/Profile/Overview/BookDetailDialogScreen");
		m_directoryMap.Add( UIScreen.APP_LIST,					SCREEN_DIRECTORY + "DashBoard/Profile/Overview/AppListDialogScreen");
		m_directoryMap.Add( UIScreen.BOOK_LIST,					SCREEN_DIRECTORY + "DashBoard/Profile/Overview/BookListDialogScreen");
		m_directoryMap.Add( UIScreen.ADD_APPS,					SCREEN_DIRECTORY + "DashBoard/Profile/Controls/AddAppScreen");
		m_directoryMap.Add( UIScreen.CONFIRM_DIALOG,			SCREEN_DIRECTORY + "DashBoard/Profile/Overview/NoticeDialogScreen");
		m_directoryMap.Add( UIScreen.ADD_SITE,					SCREEN_DIRECTORY + "DashBoard/Profile/Controls/AddSiteScreen");
		m_directoryMap.Add( UIScreen.ART_GALLERY,				SCREEN_DIRECTORY + "DashBoard/Profile/Overview/ArtGalleryScreen");
		m_directoryMap.Add( UIScreen.ART_LIST,					SCREEN_DIRECTORY + "DashBoard/Profile/Overview/ArtListDialogScreen");
		m_directoryMap.Add( UIScreen.LOADING_SPINNER, 			SCREEN_DIRECTORY + "LoadingSpinner");
		m_directoryMap.Add( UIScreen.RECORD_START,				SCREEN_DIRECTORY + "DashBoard/Profile/Overview/RecordStartScreen");
		m_directoryMap.Add( UIScreen.RECORD_AGAIN,				SCREEN_DIRECTORY + "DashBoard/Profile/Overview/RecordAgainScreen");
		m_directoryMap.Add( UIScreen.RECORD_FINISH,				SCREEN_DIRECTORY + "DashBoard/Profile/Overview/RecordFinishScreen");
		m_directoryMap.Add( UIScreen.CONGRATS, 					SCREEN_DIRECTORY + "CongratsScreen");
		m_directoryMap.Add( UIScreen.CONGRATS_BACKGROUND, 		SCREEN_DIRECTORY + "CongratsBackgroundScreen");
		m_directoryMap.Add( UIScreen.WEBVIEW,					SCREEN_DIRECTORY + "WebViewScreen");
		m_directoryMap.Add( UIScreen.PROFILE_VIEW,				SCREEN_DIRECTORY + "ProfileViewScreen");
		m_directoryMap.Add( UIScreen.CHILD_LOCK_HELP,			SCREEN_DIRECTORY + "DashBoard/Setting/Dialog/ChildLockHelpScreen");
		m_directoryMap.Add( UIScreen.FAQ_DIALOG,				SCREEN_DIRECTORY + "DashBoard/Setting/Dialog/FAQDialogScreen");
		m_directoryMap.Add( UIScreen.CANCEL_CHILD_LOCK,			SCREEN_DIRECTORY + "DashBoard/Setting/Dialog/TurnOffChildLockScreen");
		m_directoryMap.Add( UIScreen.LEGAL_CONTENT,				SCREEN_DIRECTORY + "LegalWebScreen");
		m_directoryMap.Add( UIScreen.MESSAGE,					SCREEN_DIRECTORY + "MessageScreen");
		m_directoryMap.Add( UIScreen.UPGRADE,					SCREEN_DIRECTORY + "DashBoard/Profile/StarChart/UpgradeScreen");
		m_directoryMap.Add( UIScreen.PAINT_VIEW,				SCREEN_DIRECTORY + "PaintViewScreen");

		m_directoryMap.Add( UIScreen.TELL_FRIEND,				SCREEN_DIRECTORY + "TellAFriendScreen");
		m_directoryMap.Add( UIScreen.CONNECTION_ERROR,			SCREEN_DIRECTORY + "ConnectionErrorScreen");
		m_directoryMap.Add( UIScreen.ADD_FRIEND,				SCREEN_DIRECTORY + "AddFriendScreen");

		m_directoryMap.Add( UIScreen.PAYWALL,					SCREEN_DIRECTORY + "DashBoard/PremiumPaywallScreen");
		m_directoryMap.Add( UIScreen.DASHBOARD_READING,			SCREEN_DIRECTORY + "DashBoard/Profile/Overview/RecordAReadingScreen");
		m_directoryMap.Add( UIScreen.CREATE_CHILD_NEW,			SCREEN_DIRECTORY + "NewCreateChildScreen"      );
		m_directoryMap.Add( UIScreen.SIGN_IN_FREE,				SCREEN_DIRECTORY + "SignInFreeScreen");
		m_directoryMap.Add( UIScreen.TRIAL_MESSAGE,				SCREEN_DIRECTORY + "TrialMessageScreen");
		m_directoryMap.Add( UIScreen.PANEL_MARKETING_SCREEN,	SCREEN_DIRECTORY + "PanelMarketingScreen"      );
		m_directoryMap.Add( UIScreen.SIGN_UP_AFTER_INPUT_CREDITCARD,	SCREEN_DIRECTORY + "SetAccountScreen"  );
		m_directoryMap.Add( UIScreen.CONGRATURATION,			SCREEN_DIRECTORY + "CongraturationScreen"  );
		m_directoryMap.Add( UIScreen.PREMIUM_ELIGIBLE,			SCREEN_DIRECTORY + "PremiumEligibleScreen"  );

		// Sean: vzw
		m_directoryMap.Add( UIScreen.REGION_APP,				SCREEN_DIRECTORY + "RegionAppScreen" );

		// Kevin: vzw
		m_directoryMap.Add( UIScreen.TABLET_SETTINGS,				SCREEN_DIRECTORY + "DashBoard/TabletSettingsScreen" );

		//Add_YOUR_APP
		m_directoryMap.Add( UIScreen.TUTORIAL_MAIN_PROCESS,				SCREEN_DIRECTORY + "Tutorials/TutMainProcess" );
		m_directoryMap.Add( UIScreen.TUTORIAL_CONTROL_VIOLENCE,				SCREEN_DIRECTORY + "Tutorials/TutControlViolenceScreen" );
		// end vzw

		//honda
		m_directoryMap.Add( UIScreen.ERROR_MESSAGE,				SCREEN_DIRECTORY + "ErrorMessage" );
		m_directoryMap.Add( UIScreen.NO_INTERNET,				SCREEN_DIRECTORY + "NoInternetScreen" );
		m_directoryMap.Add( UIScreen.LOADING_PAGE,				SCREEN_DIRECTORY + "LoadingPage" );
		m_directoryMap.Add( UIScreen.TIMES_UP,					SCREEN_DIRECTORY + "TimesUp" );

		// Cathy
		m_directoryMap.Add( UIScreen.LOADING_SPINNER_ELEPHANT,	SCREEN_DIRECTORY + "LoadingSpinnerElephant" );

		//honda
		m_directoryMap.Add( UIScreen.FOTA_POPUP,				SCREEN_DIRECTORY + "FOTAPopup" );
		m_directoryMap.Add( UIScreen.PARENT_BIRTH_CHECK,		SCREEN_DIRECTORY + "CheckParentBirthPopup" );
	
		//honda: debug mode
		m_directoryMap.Add( UIScreen.ADD_ITEMS_MANUALLY, 		SCREEN_DIRECTORY + "AddItemManuallyPopup" );


	}

	public Dictionary<int,string> getDirectoryMap()
	{
		return m_directoryMap;
	}
	
	public UICanvas getScreen( int p_screen )
	{
		switch( p_screen )
		{
			case UIScreen.SPLASH: 				return new SplashCanvas() 			as UICanvas;
			case UIScreen.SPLASH_PREMIUM: 		return new SplashCanvas() 			as UICanvas;
			case UIScreen.SPLASH_BACKGROUND:	return new SplashBackCanvas() 		as UICanvas;
			case UIScreen.PROFILE_SELECTION:	return new ProfileCanvas() 			as UICanvas;
			case UIScreen.ENTRANCE:				return new EntranceCanvas() 		as UICanvas;
			case UIScreen.LOADING_ENTRANCE:		return new LoadingEntranceCanvas() 	as UICanvas;
			case UIScreen.CORNER_PROFILE_INFO:	return new CornerProfileCanvas() 	as UICanvas;
			case UIScreen.MAP:					return new MapCanvas() 				as UICanvas;
			case UIScreen.REGION_LANDING:		return new RegionLandingCanvas() 	as UICanvas;
			case UIScreen.ACTIVITY_PANEL:		return new ActivityPanelCanvas() 	as UICanvas;
			case UIScreen.VIDEO_ACTIVITY:		return new VideoActivityCanvas() 	as UICanvas;
			case UIScreen.GAME_ACTIVITY:		return new GameActivityCanvas() 	as UICanvas;
			case UIScreen.BOOK_ACTIVITY:		return new BookActivityCanvas() 	as UICanvas;
			case UIScreen.ZOODLES_INTRO:		return new ZoodlesIntroCanvas() 	as UICanvas;
			case UIScreen.PAINT_ACTIVITY:		return new PaintActivityCanvas() 	as UICanvas;
			case UIScreen.SET_ACCOUNT: 			return new SetAccountCanvas()   	as UICanvas;
			case UIScreen.BOOK_READER:		    return new BookReaderCanvas() 	    as UICanvas;
			case UIScreen.BOOK_PAGE:		    return new BookPageCanvas() 	    as UICanvas;
			case UIScreen.SIGN_IN:		    	return new SignInCanvas() 	    	as UICanvas;
			case UIScreen.BIRTHYEAR:		    return new BirthYearCanvas() 	    as UICanvas;
			case UIScreen.CREATE_ACCOUNT_SELECTION:	return new CreateAccountSelectCanvas() as UICanvas;
			case UIScreen.CREATE_ACCOUNT_BACKGROUND:return new CreateAccountBackCanvas()   as UICanvas;
			case UIScreen.CREATE_CHILDPROFILE:		return new CreateChildProfileCanvas()  as UICanvas;
			case UIScreen.SELECT_AVATAR:			return new SelectAvatarCanvas()   as UICanvas;
			case UIScreen.SET_BIRTHYEAR:			return new SetBirthYearCanvas()   as UICanvas;
			case UIScreen.DASHBOARD_COMMON:		return new DashBoardCommonCanvas()    as UICanvas;
			case UIScreen.APP_INFO:				return new AppInfoCanvas()  		  as UICanvas;
			case UIScreen.LEFT_MENU:			return new LeftMenuCanvas()  	  	  as UICanvas;
			case UIScreen.PLAN_DEATILS:			return new PlanDetails()  			  as UICanvas;
			case UIScreen.CANCEL_SUB:			return new CancelSubscriptionCanvas() as UICanvas;
			case UIScreen.ERROR:				return new ErrorCanvas()  			  as UICanvas;
            case UIScreen.FUN_ACTIVITY:         return new FunActivityCanvas()        as UICanvas;
			case UIScreen.UPSELL_SPLASH:        return new UpsellSplashCanvas()       as UICanvas;
			case UIScreen.VIEW_PREMIUM:         return new ViewPremiumCanvas()     	  as UICanvas;
			case UIScreen.BUY_GEMS:         	return new BuyGemsCanvas()     	  	  as UICanvas;
			case UIScreen.PAYMENT:         		return new PaymentCanvas()     	  	  as UICanvas;
			case UIScreen.PAY_CONFIRM:   		return new PayConfirmCanvas()  	  	  as UICanvas;
			case UIScreen.THANK:		   		return new ThankCanvas()	  	  	  as UICanvas;
			case UIScreen.SIGN_OUT:		   		return new SignOutConfirmCanvas()	  as UICanvas;
			case UIScreen.SENT_FEED_BACK:  		return new SentFeedBackCanvas()	  	  as UICanvas;
			case UIScreen.HOME_RESET:  			return new ResetHomeCanvas()	  	  as UICanvas;
			case UIScreen.HOME_REQUEST:  		return new RequestHomeCanvas()	  	  as UICanvas;
			case UIScreen.CHILD_LOCK_SCREEN:  	return new ChildLockCanvas()	  	  as UICanvas;
			case UIScreen.NOTIFICATION:  		return new SettingNotificationCanvas() as UICanvas;
			case UIScreen.SETTING_COMMON:  		return new SettingCommonCanvas()  	  as UICanvas;
			case UIScreen.COMMON_DIALOG:  		return new CommonDialogCanvas()  	  as UICanvas;
			case UIScreen.FAQ_SCREEN:			return new FAQCanvas() 					as UICanvas;
			case UIScreen.DEVICE_OPTIONS_SCREEN:return new DeviceOptionsCanvas()  	  as UICanvas;
			case UIScreen.KIDS_PROFILE: 		return new KidsProfileCanvas() 			as UICanvas;
			case UIScreen.PROFILE_ACTIVE: 		return new ProfileActivityCanvas() 		as UICanvas;
			case UIScreen.DASHBOARD_CONTROLLER: return new DashBoardControllerCanvas() 	as UICanvas;
			case UIScreen.DASHBOARD_INFO: 		return new DashBoardProfileInfoCanvas() as UICanvas;
			case UIScreen.SIGN_IN_UPSELL: 		return new SignInUpSellCanvas() 		as UICanvas;
			case UIScreen.DASHBOARDL_PROGRESS: 	return new OverallProgressCanvas() 		as UICanvas;
			case UIScreen.TIME_SPEND: 			return new TimeSpendCanvas() 			as UICanvas;
			case UIScreen.PROMOTE_SUBJECTS: 	return new PromoteSubjectsCanvas() 		as UICanvas;
			case UIScreen.PROMOTE_LANGUAGES: 	return new PromoteLanguagesCanvas() 	as UICanvas;
			case UIScreen.TIME_LIMITS: 			return new TimeLimitsCanvas() 			as UICanvas;
			case UIScreen.VIOLENCE_FILTERS: 	return new ViolenceFiltersCanvas() 		as UICanvas;
			case UIScreen.SIGIN_UP_UPSELL: 		return new CreateAccountUpsellCanvas() 	as UICanvas;
			case UIScreen.UPSELL_CONGRATURATION:return new UpsellCongratulationCanvas()	as UICanvas;
			case UIScreen.STAR_CHART:			return new StarChartCanvas() 			as UICanvas;
			case UIScreen.RECOMMENDED_APP: 		return new RecommendedAppCanvas()		as UICanvas;
			case UIScreen.RECOMMENDED_BOOK: 	return new RecommendedBookCanvas() 		as UICanvas;
			case UIScreen.APP_DETAILS: 			return new AppDetailsCanvas() 			as UICanvas;
			case UIScreen.BOOK_DETAILS: 		return new BookDetailsCanvas() 			as UICanvas;
			case UIScreen.APP_LIST: 			return new AppListCanvas() 				as UICanvas;
			case UIScreen.BOOK_LIST: 			return new BookListCanvas() 			as UICanvas;

			case UIScreen.ADD_APPS: 			return new AddAppCanvas() 				as UICanvas;
			case UIScreen.CONFIRM_DIALOG: 		return new ConfirmDialogCanvas() 		as UICanvas;
			case UIScreen.ADD_SITE: 			return new AddSiteCanvas() 				as UICanvas;

			case UIScreen.ART_GALLERY: 			return new ArtGalleryCanvas() 			as UICanvas;
			case UIScreen.ART_LIST: 			return new ArtListCanvas() 				as UICanvas;

			case UIScreen.RECORD_START: 		return new RecordStartCanvas() 			as UICanvas;
			case UIScreen.RECORD_AGAIN: 		return new RecordAgainCanvas() 			as UICanvas;
			case UIScreen.RECORD_FINISH: 		return new RecordFinishCanvas() 		as UICanvas;
			case UIScreen.CONGRATS: 			return new CongratsCanvas() 			as UICanvas;
			case UIScreen.PROFILE_VIEW: 		return new ProfileViewCanvas() 			as UICanvas;
			
			case UIScreen.CHILD_LOCK_HELP: 		return new ChildLockHelpCanvas() 		as UICanvas;

			case UIScreen.FAQ_DIALOG: 			return new FAQDialogCanvas() 			as UICanvas;
			case UIScreen.CANCEL_CHILD_LOCK: 	return new CancelLockConfirmCanvas() 	as UICanvas;
			case UIScreen.MESSAGE: 				return new MessageCanvas() 				as UICanvas;

			case UIScreen.UPGRADE: 				return new UpgradeCanvas() 				as UICanvas;
			case UIScreen.PAINT_VIEW: 			return new PaintViewCanvas() 			as UICanvas;
			case UIScreen.TELL_FRIEND: 			return new TellAFriendCanvas() 			as UICanvas;
			case UIScreen.ADD_FRIEND: 			return new AddFriendCanvas() 			as UICanvas;
			
			case UIScreen.PAYWALL: 				return new PaywallCanvas() 				as UICanvas;
			case UIScreen.DASHBOARD_READING: 	return new RecordAReadingCanvas() 		as UICanvas;

			case UIScreen.CREATE_CHILD_NEW: 	return new CreateChildCanvas() 			as UICanvas;
			case UIScreen.SIGN_IN_FREE: 		return new SignInFreeCanvas() 			as UICanvas;
			case UIScreen.TRIAL_MESSAGE: 		return new TrialMessageCanvas() 		as UICanvas;
			case UIScreen.PANEL_MARKETING_SCREEN: return new PanelMarkingScreenCanvas() as UICanvas;
			case UIScreen.SIGN_UP_AFTER_INPUT_CREDITCARD: return new SignUpAfterInoutCreditCardCanvas() as UICanvas;
			case UIScreen.CONGRATURATION:		 return new CongratulationCanvas() 		as UICanvas;
			case UIScreen.PREMIUM_ELIGIBLE:		 return new PremiumEligibleCanvas() 	as UICanvas;


			// Sean: vzw
			case UIScreen.REGION_APP:			return new RegionAppCanvas() 			as UICanvas;

			//Kevin : vzw
			case UIScreen.TABLET_SETTINGS:			return new UICanvas() 			as UICanvas;
			case UIScreen.TUTORIAL_MAIN_PROCESS:			return new UICanvas() 			as UICanvas;
			case UIScreen.TUTORIAL_CONTROL_VIOLENCE:			return new UICanvas() 			as UICanvas;
			// end vzw

			//honda: vzw
			case UIScreen.ERROR_MESSAGE:			return new UICanvas() 			as UICanvas;
			case UIScreen.NO_INTERNET:              return new UICanvas()			as UICanvas;
			case UIScreen.LOADING_PAGE:             return new UICanvas()			as UICanvas;
			case UIScreen.TIMES_UP:					return new UICanvas()			as UICanvas;

			// Cathy: vzw
			case UIScreen.LOADING_SPINNER_ELEPHANT: return new UICanvas()			as UICanvas;

			//honda
			case UIScreen.FOTA_POPUP:				return new UICanvas()			as UICanvas;
			case UIScreen.PARENT_BIRTH_CHECK:		return new UICanvas()			as UICanvas;

			//honda: debug mode
			case UIScreen.ADD_ITEMS_MANUALLY:		return new UICanvas()			as UICanvas;

		}
		return new UICanvas();
	}

	
	private Dictionary<int, string> m_directoryMap;
}
