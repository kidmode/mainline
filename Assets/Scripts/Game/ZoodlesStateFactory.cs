using UnityEngine;
using System.Collections;

public class ZoodleState
{
	public const int NO_STATE           = -1;
	public const int INITIALIZE_GAME    = 0;
	public const int PROFILE_SELECTION  = 1;
	public const int LOADING_ENTRANCE   = 2;
	public const int ENTRANCE           = 3;
	public const int MAP                = 4;
	public const int REGION_LANDING     = 5;
	public const int ZOODLES_ANIMATION  = 6;
    public const int PAINT_ACTIVITY     = 7;
	public const int CREATE_ACCOUNT_SELECTION = 8;
	public const int SET_UP_ACCOUNT 	= 9;
    public const int BOOK_ACTIVITY      = 10;
	public const int SIGN_IN      		= 11;
	public const int BIRTHYEAR     		= 12;
	public const int CREATE_CHILD   	= 13;
	public const int SELECT_AVATAR   	= 14;
	public const int SET_BIRTHYEAR     	= 15;
	public const int ERROR_STATE		= 16;
	public const int UPSELL_SPLASH     	= 18;
	public const int VIEW_PREMIUM     	= 19;
	public const int BUY_GEMS	     	= 20;
	public const int PAYMENT 			= 21;
	public const int PAY_CONFIRM		= 22;
	public const int CALL_SERVER		= 23;
	public const int CHECK_HOME_BUTTON	= 24;
	public const int REQUEST_HOME_STATE	= 25;
	public const int RESET_HOME_STATE	= 26;
	public const int CHILD_LOCK_STATE	= 27;
	public const int NOTIFICATION_STATE	= 28;
	public const int SETTING_STATE		= 29;
	public const int FAQ_STATE			= 30;
	public const int DEVICE_OPTIONS_STATE = 31;
	public const int REGION_BOOKS		= 32;
	public const int KIDS_PROFILE		= 33;
	public const int SIGN_IN_UPSELL 	= 37;
	public const int SIGN_UP_UPSELL 	= 39;
	public const int TRY_TRIAL_STATE 	= 40;
	public const int UPSELL_CONGRATURATION 	= 41;
	public const int SIGN_IN_CACHE			= 42;
	public const int DASHBOARD_STAR_CHART 	= 43;
	public const int BOOK_RECORD 		= 44;
	public const int OVERVIEW_INFO 		= 45;
	public const int OVERVIEW_PROGRESS 	= 46;
	public const int OVERVIEW_TIMESPENT = 47;
	public const int OVERVIEW_ART 		= 48;
	public const int CONGRATS_STATE		= 49;
	public const int OVERVIEW_APP 	 	= 50;
	public const int OVERVIEW_BOOK 	 	= 51;
	public const int VIDEO_VIEW			= 52;
	public const int GAME_VIEW			= 53;
	public const int PROFILE_VIEW		= 54;
	public const int CONTROL_SUBJECT 	= 55;
	public const int CONTROL_LANGUAGE 	= 56;
	public const int CONTROL_TIME 		= 57;
	public const int CONTROL_VIOLENCE 	= 58;
	public const int CONTROL_APP 		= 59;
	public const int CONTROL_SITE 		= 60;
	public const int PAINT_VIEW 		= 61;
	public const int SERVER_ERROR		= 62;
	public const int GOTO_PARENT_DASHBOARD = 63;
	public const int OVERVIEW_READING 	= 64;
	public const int PLAN_PAYMENT		= 65;
	public const int REGION_VIDEO 		= 66;
	public const int REGION_GAME 		= 67;
	public const int REGION_FUN 		= 68;
	public const int CREATE_CHILD_NEW 	= 69;
	public const int SIGN_IN_FREE 		= 70;
	public const int CONGRATURATION		= 71;
	public const int PAY_GEMS_CONFIRM	= 72;
	//honda
	public const int LOADING_PAGE       = 101;
	public const int PREMIUM_ELIGIBLE_PAGE = 102;
	//end
}

public class ZoodlesStateFactory : IGameStateFactory
{
	public void addStates( GameController p_gameController )
	{
		p_gameController.addState( ZoodleState.INITIALIZE_GAME, 	new InitializeGameState() 		);
		p_gameController.addState( ZoodleState.PROFILE_SELECTION, 	new ProfileState() 				);
		p_gameController.addState( ZoodleState.LOADING_ENTRANCE, 	new LoadingEntranceState() 		);
		p_gameController.addState( ZoodleState.ENTRANCE, 			new EntranceState() 			);
		p_gameController.addState( ZoodleState.MAP, 				new MapState()		 			);
		p_gameController.addState( ZoodleState.REGION_LANDING, 		new RegionLandingState()		); 
		p_gameController.addState( ZoodleState.ZOODLES_ANIMATION, 	new ZoodlesAnimatedIntroState()	);
		p_gameController.addState( ZoodleState.PAINT_ACTIVITY, 		new PaintActivityState()		);
		p_gameController.addState( ZoodleState.CREATE_ACCOUNT_SELECTION, new CreateAccountSelectState());
		p_gameController.addState( ZoodleState.SET_UP_ACCOUNT, 		new SetUpAccountState()			); 
        p_gameController.addState( ZoodleState.BOOK_ACTIVITY,       new BookReaderState()           );
		p_gameController.addState( ZoodleState.SIGN_IN,       		new SignInState()           	); 
		p_gameController.addState( ZoodleState.BIRTHYEAR,       	new BirthYearState()           	); 
		p_gameController.addState( ZoodleState.CREATE_CHILD,       	new CreateChildProfileState() 	); 
		p_gameController.addState( ZoodleState.SELECT_AVATAR,       new SelectAvatarState() 		); 
		p_gameController.addState( ZoodleState.SET_BIRTHYEAR,       new SetBirthYearState() 		);
		p_gameController.addState( ZoodleState.ERROR_STATE,       	new ErrorState() 				);
		p_gameController.addState( ZoodleState.UPSELL_SPLASH,       new UpsellSplashState() 		);
		p_gameController.addState( ZoodleState.VIEW_PREMIUM,       	new ViewPremiumState() 			);
		p_gameController.addState( ZoodleState.BUY_GEMS,       		new BuyGemsState() 				);
		p_gameController.addState( ZoodleState.PAYMENT,       		new PaymentState() 				);
		p_gameController.addState( ZoodleState.PAY_CONFIRM,       	new PayConfirmState()			);
		p_gameController.addState( ZoodleState.CALL_SERVER,       	new CallServerState()			);
		p_gameController.addState( ZoodleState.CHECK_HOME_BUTTON, 	new CheckHomeButtonState() 		);
		p_gameController.addState( ZoodleState.REQUEST_HOME_STATE, 	new RequestHomeState() 			);
		p_gameController.addState( ZoodleState.RESET_HOME_STATE, 	new ResetHomeState()	 		);
		p_gameController.addState( ZoodleState.CHILD_LOCK_STATE, 	new SettingChildLockState()	 	);
		p_gameController.addState( ZoodleState.NOTIFICATION_STATE, 	new NotificationState()	 		);
		p_gameController.addState( ZoodleState.SETTING_STATE,	 	new SettingAppInfoState()		);
		p_gameController.addState( ZoodleState.FAQ_STATE,	 		new FAQState()					);
		p_gameController.addState( ZoodleState.DEVICE_OPTIONS_STATE,new DeviceOptionsState()		);
		p_gameController.addState( ZoodleState.REGION_BOOKS,		new RegionBooksState()			);
		p_gameController.addState( ZoodleState.SIGN_IN_UPSELL,		new SignInUpsellState()			);
		p_gameController.addState( ZoodleState.SIGN_UP_UPSELL,		new CreateAccountUpSellState()	);
		p_gameController.addState( ZoodleState.KIDS_PROFILE,       	new KidsProfileState()			);
		p_gameController.addState( ZoodleState.TRY_TRIAL_STATE,		new TryTrialState()				);
		p_gameController.addState( ZoodleState.UPSELL_CONGRATURATION,new UpsellCongraturationsState());
		p_gameController.addState( ZoodleState.SIGN_IN_CACHE,  		new SignInCacheState()			);
		p_gameController.addState( ZoodleState.DASHBOARD_STAR_CHART,new DashBoardStarChartState()	);
		p_gameController.addState( ZoodleState.OVERVIEW_INFO,		new OverviewInfoState()			);
		p_gameController.addState( ZoodleState.OVERVIEW_PROGRESS,	new OverviewProgressState()		);
		p_gameController.addState( ZoodleState.OVERVIEW_TIMESPENT,	new OverviewTimeSpentState()	);
		p_gameController.addState( ZoodleState.OVERVIEW_ART,		new OverviewArtState()			);
		p_gameController.addState( ZoodleState.CONGRATS_STATE,		new CongratsState()				);
		p_gameController.addState( ZoodleState.OVERVIEW_APP,		new OverviewAppState()			);
		p_gameController.addState( ZoodleState.BOOK_RECORD,			new BookRecordState()			);
		p_gameController.addState( ZoodleState.OVERVIEW_BOOK,		new OverviewBookState()			);
		p_gameController.addState( ZoodleState.VIDEO_VIEW,			new VideoViewState()			);
		p_gameController.addState( ZoodleState.GAME_VIEW,			new GameViewState()				);
		p_gameController.addState( ZoodleState.PROFILE_VIEW,		new ProfileViewState()			);
		p_gameController.addState( ZoodleState.CONTROL_SUBJECT,		new ControlSubjectState()		);
		p_gameController.addState( ZoodleState.CONTROL_LANGUAGE,	new ControlLanguageState()		);
		p_gameController.addState( ZoodleState.CONTROL_TIME,		new ControlTimeState()			);
		p_gameController.addState( ZoodleState.CONTROL_VIOLENCE,	new ControlViolenceState()		);
		p_gameController.addState( ZoodleState.CONTROL_APP,			new ControlAppState()			);
		p_gameController.addState( ZoodleState.CONTROL_SITE,		new ControlSiteState()			);
		p_gameController.addState( ZoodleState.PAINT_VIEW,			new PaintViewSate()				);
		p_gameController.addState( ZoodleState.SERVER_ERROR,		new ServerErrorState()			);
		p_gameController.addState( ZoodleState.GOTO_PARENT_DASHBOARD, new GotoParentDashboardState());
		p_gameController.addState( ZoodleState.OVERVIEW_READING, 	new OverviewReadingState()		);
		p_gameController.addState( ZoodleState.PLAN_PAYMENT,		new PlanPaymentState()			);
		p_gameController.addState( ZoodleState.REGION_GAME,			new RegionGameState()			);
		p_gameController.addState( ZoodleState.REGION_VIDEO,		new RegionVideoState()			);
		p_gameController.addState( ZoodleState.REGION_FUN,			new RegionFunState()			);
		p_gameController.addState( ZoodleState.CREATE_CHILD_NEW,	new CreateChildState() 			);
		p_gameController.addState( ZoodleState.SIGN_IN_FREE,		new SignInFreeState() 			);
		p_gameController.addState( ZoodleState.CONGRATURATION,		new CongraturationsState() 		);
		p_gameController.addState( ZoodleState.PAY_GEMS_CONFIRM,	new PayGemsConfirmState()		);
		//honda
		p_gameController.addState( ZoodleState.LOADING_PAGE,   		new LoadingPageState()			);
		p_gameController.addState( ZoodleState.PREMIUM_ELIGIBLE_PAGE, new PremiumEligibleState()  	);
		//end
	}

	public int initialState
	{
		get { return ZoodleState.CHECK_HOME_BUTTON; }
	}
}