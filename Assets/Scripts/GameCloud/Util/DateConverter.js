import System;

public class DateConverter
{
	private static var s_instance:DateConverter;
	
	public var TIME_SEC:int = 1;
	public var TIME_MIN:int = 60;
	public var TIME_HOUR:int = 3600;
	public var TIME_DAY:int = 86400;
	public var TIME_WEEK:int = 604800;
	
	public var TIME_STRINGS:Array = ["sec", "min", "hour", "day", "week"];
	public var TIME_SCALE:int[];
	
	private function DateConverter()
	{
		TIME_SCALE = new int[5];
		TIME_SCALE[0] = TIME_SEC;
		TIME_SCALE[1] = TIME_MIN;
		TIME_SCALE[2] = TIME_HOUR;
		TIME_SCALE[3] = TIME_DAY;
		TIME_SCALE[4] = TIME_WEEK;
	}
	
	public static function instance():DateConverter
	{
		if( null == s_instance )
			s_instance = new DateConverter();
			
		return s_instance;
	}
	
	//converts Date object from Server time (java) to .net Date object
	// they have different epochs and "tick" lengths -__-;
	public function convertServerToUnityDate( p_date:Number ):Date
	{
		var l_date:Date;
		l_date = new Date( p_date * 10000 ); //convert to .net ticks
		l_date = l_date.AddYears( 1969 ); //sync to java epoch
		
		return l_date;
	}
	
	//converts timespan to hours:minutes:seconds format
	public function convertToTicker( p_timeSpan:TimeSpan ):String
	{
		var l_string:String = String.Empty;
		
		var l_hours:int = p_timeSpan.Hours;
		var l_minutes:int = p_timeSpan.Minutes;
		var l_seconds:int = p_timeSpan.Seconds;
		
		l_string = l_string + ( l_hours ) + ":";
		l_string = l_string + ( l_minutes < 10 ? "0" + l_minutes : l_minutes ) + ":";
		l_string = l_string + ( l_seconds < 10 ? "0" + l_seconds : l_seconds );
		
		return l_string;
	}
	
	//TODO: Localize me! 
	//Converts timespan to largest time unit (days, hours, etc)
	public function convertToStringScaled( p_timeSpan:TimeSpan ):String
	{
		var l_sec:int = p_timeSpan.TotalSeconds;
		if( l_sec < 0 )
			l_sec *= -1;
//		l_sec = Mathf.Abs( l_sec );
		var l_string:String = String.Empty;
		var l_number:int;
		
		for( var i:int = 0; i < TIME_STRINGS.length-1; i++ )
		{
			if( l_sec >= TIME_SCALE[i] && l_sec < TIME_SCALE[i+1] )
			{
				l_number = (l_sec / TIME_SCALE[i]);
				l_string = "" + l_number + " " + TIME_STRINGS[i] + (l_number > 1 ? "s" : String.Empty);
				return l_string;
			}
		}
		
		l_number = (l_sec / TIME_SCALE[i-1] );
		l_string = "" + l_number + " " + TIME_STRINGS[i-1] + (l_number > 1 ? "s" : String.Empty);
		
		return l_string;
	}
	
	
	// convert java unix timestamp to datatime string
	public function convertToString( p_timestamp:long ):String
	{
		var time:DateTime = DateTime.MinValue;
		var startTime:DateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		time = startTime.AddMilliseconds(p_timestamp);
		
		return time.ToString();
	}
	
	// convert java unix timestamp to datatime
	public function convertToDateTime( p_timestamp:long ):DateTime
	{
		var time:DateTime = DateTime.MinValue;
		var startTime:DateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		time = startTime.AddMilliseconds(p_timestamp);
		
		return time;
	}
	
	public function convertToLeftTime( p_timestamp:long ):String
	{
		var l_timeStamp:long = p_timestamp;
		var l_lastDate:DateTime = DateConverter.instance().convertToDateTime(l_timeStamp);
		var l_now:DateTime = DateTime.Now;
		var l_span:TimeSpan = l_now - l_lastDate;
		var l_string:String = DateConverter.instance().convertToStringScaled( l_span );
		return l_string + Localization.getString("TXT_LABEL_TIME_LEFT");
	}
	
}
