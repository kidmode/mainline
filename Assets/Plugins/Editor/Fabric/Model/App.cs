using UnityEngine;
using System.Collections;

namespace Fabric
{
	namespace Model
	{
		public class App
		{
			public string Name;
			public string BundleIdentifier;
			public string IconUrl;
			public string DashboardUrl;

			public App(string name, string bundleIdentifier, string iconUrl, string dashboardUrl)
			{
				Name = name;
				BundleIdentifier = bundleIdentifier;
				IconUrl = iconUrl;
				DashboardUrl = dashboardUrl;
			}
		}
	}
}