using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using System;
using Fabric.Model;

public class CrashlyticsPostBuildAndroid : MonoBehaviour {

	[PostProcessBuild(100)]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string path) {
		
		if (buildTarget == BuildTarget.Android) {
			SendBuildEvent ();
		}
	
	}

	public static void SendBuildEvent () {
		Settings settings = Settings.Instance;

		FabricUtils.Log ("Sending build information");

		if (string.IsNullOrEmpty(settings.ApiKey)) {
			FabricUtils.Error ("API key not found");
			return;
		}

		var bundleId = PlayerSettings.bundleIdentifier;
		WWWForm form = new WWWForm();
		form.AddField("app_name", bundleId);
		form.AddField("app_identifier", bundleId);
		form.AddField("base_identifier", bundleId);
		form.AddField("platform_id", "android");

		var headers = new Dictionary<string, string> ();
		headers.Add("X-CRASHLYTICS-DEVELOPER-TOKEN", "771b48927ee581a1f2ba1bf60629f8eb34a5b63f");
		headers.Add("X-CRASHLYTICS-API-KEY", settings.ApiKey);
		headers.Add("X-CRASHLYTICS-API-CLIENT-ID", "io.fabric.tools.unity");
		headers.Add("X-CRASHLYTICS-API-CLIENT-DISPLAY-VERSION", "1.0.0");

		string url = "https://api.crashlytics.com/spi/v1/platforms/android/apps/" + bundleId + "/built";
		byte[] rawData = form.data;
		WWW www = new WWW(url, rawData, headers);

		var timeout = false;
		var t0 = DateTime.UtcNow;
		while (!www.isDone) {
			var t1 = DateTime.UtcNow;
			var delta = (int)(t1-t0).TotalSeconds;
			if (delta > 5) {
				timeout = true;
				break;
			}
		};

		if (timeout) {
			FabricUtils.Warn ("Timed out waiting for build event response. If this is a production build, you may want to build again to ensure it has been properly sent.");
		} else if (string.IsNullOrEmpty(www.error)) {
			FabricUtils.Log ("Build information sent");
		} else {
			FabricUtils.Error ("Could not send build event. Error: " + www.error);
		}
	}

	static string SerializeToJSON (Dictionary<string, string> dict) {
		var json = "{";
		foreach (var line in dict) {
			json += string.Format("\"{0}\":\"{1}\",", line.Key, line.Value);
		}
		json += "}";
		return json;
	}
}
