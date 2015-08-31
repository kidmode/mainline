using System.Collections;
using System.Timers;
using System;

static class MidnightNotifier
{
	private static readonly Timer timer;
	
	static MidnightNotifier()
	{
		timer = new Timer(GetSleepTime());
		timer.Elapsed += (s, e) =>
		{
			OnDayChanged();
			timer.Interval = GetSleepTime();
		};
		timer.Start();
	}
	
	private static double GetSleepTime()
	{
		DateTime midnightTonight = DateTime.Today.AddDays(1);
		double differenceInMilliseconds = (midnightTonight - DateTime.Now).TotalMilliseconds;
		//this is test case
//		return 60 * 1000;
		//this is real thing
		return differenceInMilliseconds;
	}
	
	private static void OnDayChanged()
	{
		var handler = DayChanged;
		if (handler != null)
			handler(null, null);
	}
	
	public static event EventHandler<EventArgs> DayChanged;

	public static void resetMidnightTimer()
	{
		timer.Interval = GetSleepTime();
		timer.Start();
	}

	public static void stopMidnightNotifier()
	{
		if (timer.Enabled)
			timer.Stop();
	}
}