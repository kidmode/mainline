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
		return differenceInMilliseconds;
	}
	
	private static void OnDayChanged()
	{
		var handler = DayChanged;
		if (handler != null)
			handler(null, null);
	}
	
	public static event EventHandler<EventArgs> DayChanged;
}