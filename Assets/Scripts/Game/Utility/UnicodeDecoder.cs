using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;

public static class UnicodeDecoder
{
	public static string CoverHtmlLabel(string str)
	{
		string outStr = "";

		outStr = str.Replace( "<p>", "" );
		outStr = outStr.Replace( "</p>", "\\r\\n\\r\\n" );
		outStr = outStr.Replace( "<br />", "\\r\\n" );
		outStr = outStr.Replace( "<br/>", "\\r\\n" );
		outStr = outStr.Replace( "<center>", "" );
		outStr = outStr.Replace( "</center>", "" );

		while( outStr.Contains("<h1") )
		{
			int l_labelh1Index = outStr.IndexOf("<h1");
			int l_labelh1EndIndex = outStr.IndexOf (">", l_labelh1Index);
			outStr = outStr.Remove(l_labelh1Index, l_labelh1EndIndex - l_labelh1Index + 1);
			outStr = outStr.Replace( "</h1>", "" );
		}

		while( outStr.Contains("<h2") )
		{
			int l_labelh2Index = outStr.IndexOf("<h2");
			int l_labelh2EndIndex = outStr.IndexOf (">", l_labelh2Index);
			outStr = outStr.Remove(l_labelh2Index, l_labelh2EndIndex - l_labelh2Index + 1);
			outStr = outStr.Replace( "</h2>", "" );
		}

		while( outStr.Contains("<h3") )
		{
			int l_labelh3Index = outStr.IndexOf("<h3");
			int l_labelh3EndIndex = outStr.IndexOf (">", l_labelh3Index);
			outStr = outStr.Remove(l_labelh3Index, l_labelh3EndIndex - l_labelh3Index + 1);
			outStr = outStr.Replace( "</h3>", "" );
		}

		return outStr;
	}
	
	public static string Unicode(string str) 
	{ 
		string outStr = ""; 
		Regex reg = new Regex(@"(?i)//u([0-9a-f]{4})"); 
		outStr = reg.Replace(str, delegate(Match m1) 
		                     { 
			return ((char)Convert.ToInt32(m1.Groups[1].Value, 16)).ToString(); 
		}); 
		return outStr; 
	}
	
	public static string UnicodeToChinese(string unicodeString)   
	{   
		if (string.IsNullOrEmpty(unicodeString))   
			return string.Empty;   
		
		string outStr = unicodeString;   
		
		Regex re = new Regex("\\\\u[0123456789abcdef]{4}", RegexOptions.IgnoreCase);   
		MatchCollection mc = re.Matches(unicodeString);   
		foreach (Match ma in mc)   
		{   
			outStr = outStr.Replace(ma.Value, ConverUnicodeStringToChar(ma.Value).ToString());   
		}   
		return outStr;   
	}
	
	public static char ConverUnicodeStringToChar(string str)   
	{   
		char outStr = Char.MinValue;   
		outStr = (char)int.Parse(str.Remove(0, 2),  System.Globalization.NumberStyles.HexNumber);   
		return outStr;   
	}
}
