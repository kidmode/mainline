//Used in JS classes to convert value types in setters and from untyped collections.
using UnityEngine;
using System.Collections;

public class Caster : object
{
	public static long toLong(object obj)
	{
		if (obj is System.Int64)
		{
			return (long) obj;
		}
		else
		{
			return long.Parse(obj.ToString());	
		}
	}
	
	public static double toDouble(object obj)
	{
		if (obj is System.Double)
		{
			return (double) obj;
		}
		else
		{
			return double.Parse(obj.ToString());	
		}
	}

	public static float toFloat(object obj)
	{
		if (obj is System.Single)
		{
			return (float) obj;
		}
		else
		{
			return float.Parse(obj.ToString());
		}		
	}

	public static int toInt(object obj)
	{
		if (obj is System.Int32)
		{
			return (int) obj;
		}
		else
		{
			return int.Parse(obj.ToString());
		}
	}

	public static bool toBoolean(object obj)
	{
		if (obj is System.Boolean)
		{
			return (bool) obj;
		}
		else
		{
			return bool.Parse(obj.ToString());
		}		
	}

	public static Vector2 toVector2(object obj)
	{
		return (Vector2) obj;
	}

	public static DictionaryEntry toDictionaryEntry(object obj)
	{
		return (DictionaryEntry) obj;
	}

	public static TextAnchor toTextAnchor(object obj)
	{
		return (TextAnchor) obj;
	}

}
