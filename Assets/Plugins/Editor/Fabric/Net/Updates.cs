using UnityEngine;
using System.Collections;
using System;

namespace Fabric {
	namespace Net {
		public class Updates {

			public static IEnumerator GetLatestVersion(Action<Version> callback) {
				WWW www = new WWW ("https://s3.amazonaws.com/ssl-download-crashlytics-com/unity-fabric/crashlytics/version");
				
				while (!www.isDone) {
					yield return null;
				}
				
				Version latestVersion = null;
				if (string.IsNullOrEmpty (www.error)) {
					latestVersion = new Version (www.text);
				}
				
				callback (latestVersion);
			}		

		}
	}
}
