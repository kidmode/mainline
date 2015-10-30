using UnityEngine;
using System;

public class GAUtil
{
	public static void startSession(string p_label)
	{
		logEvent(new EventHitBuilder()
			.SetEventCategory("user")
			.SetEventAction("UserSessionStart")
			.SetEventLabel(p_label)
			.SetEventValue(0)
			.SetCustomDimension(3, "ltime")
		    .SetCustomMetric(3, DateTime.Now.ToString())
		    .SetCustomDimension(4, "UserSource")
			.SetCustomMetric(4, Application.isMobilePlatform ? "HTC" : "Normal"));
	}

	public static void stopSession(string p_label)
	{
		logEvent(new EventHitBuilder()
			.SetEventCategory("user")
			.SetEventAction("UserSessionEnd")
			.SetEventLabel(p_label)
			.SetEventValue(0)
			.SetCustomDimension(2, "region"));
	}

	public static void changeKid()
	{
		logEvent(new EventHitBuilder()
			.SetEventCategory("user")
			.SetEventAction("ChangeKid")
			.SetCustomDimension(1, "KidID")
			.SetCustomMetric(1, "" + SessionHandler.getInstance().currentKid.id));
	}

	public static void logVisit(string p_label, int p_duration)
	{
		logEvent(new EventHitBuilder()
			.SetEventCategory("asset")
			.SetEventAction("Visit")
		    .SetEventLabel(p_label)
		    .SetEventValue(p_duration)
			.SetCustomDimension(1, "KidID")
			.SetCustomMetric(1, "" + SessionHandler.getInstance().currentKid.id)
			.SetCustomDimension(3, "ltime")
		    .SetCustomMetric(3, DateTime.Now.ToString()));
	}

	public static void logRecord(string p_label, int p_duration)
	{
		logEvent(new EventHitBuilder()
			.SetEventCategory("asset")
			.SetEventAction("Record")
			.SetEventLabel(p_label)
			.SetEventValue(p_duration)
			.SetCustomDimension(1, "KidID")
			.SetCustomMetric(1, "" + SessionHandler.getInstance().currentKid.id)
			.SetCustomDimension(3, "ltime")
			.SetCustomMetric(3, DateTime.Now.ToString()));
	}

	public static void logInstallApp(string p_appName)
	{
		logEvent(new EventHitBuilder()
		         .SetEventCategory("app")
		         .SetEventAction("Install")
		         .SetEventLabel(p_appName)
		         .SetEventValue(0)
		         .SetCustomDimension(1, "KidID")
		         .SetCustomMetric(1, "" + SessionHandler.getInstance().currentKid.id)
		         .SetCustomDimension(3, "ltime")
		         .SetCustomMetric(3, DateTime.Now.ToString()));
	}

	public static void logEvent(string p_category, string p_action, string p_label, long p_value)
	{
		logEvent(new EventHitBuilder()
			.SetEventCategory(p_category)
			.SetEventAction(p_action)
			.SetEventLabel(p_label)
			.SetEventValue(p_value)
			.SetCustomDimension(1, "KidID")
			.SetCustomMetric(1, SessionHandler.getInstance().currentKid.id.ToString()));
	}

	public static void logEvent(EventHitBuilder p_builder)
	{
		_init();

		s_googleAnalytics.LogEvent(p_builder);
	}

	public static void logItem(ItemHitBuilder p_builder)
	{
		_init();

		s_googleAnalytics.LogItem(p_builder);
	}

	public static void logTransaction(TransactionHitBuilder p_builder)
	{
		_init();

		s_googleAnalytics.LogTransaction(p_builder);
	}

	public static void logTransaction(string p_transID, double p_revenue, double p_tax, string p_currencyCode)
	{
		logTransaction(new TransactionHitBuilder()
			.SetTransactionID(p_transID)
			.SetAffiliation("HTC")
			.SetRevenue(p_revenue)
			.SetTax(p_tax)
			.SetShipping(0)
			.SetCurrencyCode(p_currencyCode));
	}

	public static void logScreen(AppViewHitBuilder p_builder)
	{
		_init();

		s_googleAnalytics.LogScreen(p_builder);
	}

	public static void logScreen(string p_screen)
	{
		Kid l_kid = SessionHandler.getInstance().currentKid;
		logScreen(new AppViewHitBuilder()
			.SetScreenName(p_screen)
			.SetCustomDimension(1, "KidID")
			.SetCustomMetric(1, (l_kid != null ? l_kid.id.ToString() : "-1")));
	}

	public static void logException(ExceptionHitBuilder p_builder)
	{
		_init();

//		s_googleAnalytics.LogException(p_builder);
	}

	public static void logException(string p_exceptionDescription, bool p_isFatal)
	{
		logException(new ExceptionHitBuilder()
			.SetExceptionDescription(p_exceptionDescription)
			.SetFatal(p_isFatal));
	}

	private static void _init()
	{
		if (s_inited)
			return;
		
		s_googleAnalytics = UnityEngine.Object.FindObjectOfType(typeof(GoogleAnalyticsV3)) as GoogleAnalyticsV3;
		if (s_googleAnalytics != null)
			s_googleAnalytics.SetUserIDOverride(SessionHandler.getInstance().username);

		s_inited = true;
	}

	private static GoogleAnalyticsV3 s_googleAnalytics = null;
	private static bool s_inited = false;
}

