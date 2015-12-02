using UnityEngine;
using System.Collections;

namespace Fabric
{
	namespace Detail
	{
		public class Strings
		{
			public static string Unwrap(string text, char starting, char ending)
			{
				int s = text.IndexOf (starting);
				int e = text.LastIndexOf (ending);
				
				return s == -1 || e == -1 ?
					text :
						text.Substring (s + 1, text.Length - s - 1).Substring (0, e - 1);
			}
		}
	}
}