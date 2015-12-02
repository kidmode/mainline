using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using Fabric.Net;

namespace Fabric
{
	[InitializeOnLoad]
	public class CrashlyticsOnLoad {
		private static Version localVersion = Editor.version;

		static CrashlyticsOnLoad() {
			CheckForUpdates ();
		}

		private static void CheckForUpdates() {
			Detail.Runner.StartCoroutine (Updates.GetLatestVersion ((latestVersion) => {
				if (latestVersion != null && latestVersion.CompareTo(localVersion) > 0) {
					FabricUtils.Warn ("A new version of Crashlytics is available");
					FabricUtils.Warn ("Download it today at https://fabric.io/downloads");
					FabricUtils.Warn ("Current Crashlytics version: {0}. New version: {1}", localVersion, latestVersion);
				}
			}));
		}
	}
}
