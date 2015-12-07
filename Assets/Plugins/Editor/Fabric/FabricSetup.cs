using UnityEditor;
using System.IO;
using UnityEngine;
using System.Xml;
using Fabric.Model;

//
// Contains setup methods required for Fabric pre-Unity build
//
public class FabricSetup {
	public static void EnableCrashlytics (bool checkSetupComplete) {
		var settings = Settings.Instance;
		if (checkSetupComplete && !settings.SetupComplete) {
			FabricUtils.Error ("Please first prepare Crashlytics in the Fabric menu to obtain your login credentials.");
			return;
		}

		settings.EnableCrashlytics = true;
		SetScriptExecutionOrder ();
		EnableCrashlyticsiOS ();
		EnableCrashlyticsAndroid ();
	}

	public static void DisableCrashlytics () {
		Settings.Instance.EnableCrashlytics = false;
		DisableCrashlyticsiOS ();
		DisableCrashlyticsAndroid ();
	}

	// This is needed so that we don't miss any logged exceptions from
	// initialization scripts
	static int crashlyticsScriptPriority = -100;	
	public static void SetScriptExecutionOrder () {
		string crashlyticsLib = typeof (Crashlytics).Name;

		foreach (MonoScript script in MonoImporter.GetAllRuntimeMonoScripts()) {
			if (script.name == crashlyticsLib) {
				if (MonoImporter.GetExecutionOrder(script) != crashlyticsScriptPriority) {
					FabricUtils.Log ("Changing script execution order for Crashlytics");
					MonoImporter.SetExecutionOrder(script, crashlyticsScriptPriority);
				} else {
					FabricUtils.Log ("Script execution order for Crashlytics already set");
				}
			}
		}
	}
	
	//
	// iOS-specific
	//

	private static void EnableCrashlyticsiOS () {
		// In the case of iOS, this is currently taken care of at post-build time via Settings.Instance.EnableCrashlytics
	}

	private static void DisableCrashlyticsiOS () {
		// In the case of iOS, this is currently taken care of at post-build time via Settings.Instance.EnableCrashlytics
	}

	//
	// Android-specific
	//
	
	private static void EnableCrashlyticsAndroid () {
		FabricSetup.BootstrapTopLevelManifest ();
		FabricSetup.BootstrapFabricManifest ();
		FabricSetup.InjectMetadataIntoFabricManifest ("io.fabric.ApiKey", Settings.Instance.ApiKey);
		FabricSetup.InjectMetadataIntoFabricManifest ("io.fabric.unity.crashlytics.version", Fabric.Editor.version.ToString ());
		FabricSetup.ToggleApplicationInTopLevelManifest ();
	}
	
	private static void DisableCrashlyticsAndroid () {
		FabricSetup.ToggleApplicationInTopLevelManifest ();
	}

	static string outputFabricManifestRelativePath = "Plugins/Android/Crashlytics/AndroidManifest.xml";
	static string outputFabricManifestPath = Path.Combine(Application.dataPath, outputFabricManifestRelativePath);
	
	static string outputTopLevelManifestRelativePath = "Plugins/Android/AndroidManifest.xml";
	static string outputTopLevelManifestPath = Path.Combine(Application.dataPath, outputTopLevelManifestRelativePath);

	static string fabricApplicationName = "com.crashlytics.unity.crashlyticswrapper.FabricApplication";
	
	static void BootstrapFabricManifest() {
		var inputManifestPath = Path.Combine(Application.dataPath, "Plugins/Android/Crashlytics/Template-AndroidManifest.xml");
		
		FabricUtils.Log ("Writing " + outputFabricManifestRelativePath);
		File.Copy(inputManifestPath, outputFabricManifestPath, true);
	}

	static void BootstrapTopLevelManifest() {
		// Unity <5.2
		var inputManifestPath = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines/AndroidPlayer/AndroidManifest.xml");

		if (!File.Exists (inputManifestPath)) {
			// Unity >5.2
			inputManifestPath = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines/AndroidPlayer/Apk/AndroidManifest.xml");

			if (!File.Exists (inputManifestPath)) {
				FabricUtils.Error ("Could not find Unity's AndroidManifest.xml file in " + EditorApplication.applicationContentsPath);
				return;
			}
		}

		if (!File.Exists (outputTopLevelManifestPath)) {
			FabricUtils.Log ("Writing " + outputTopLevelManifestRelativePath);
			File.Copy(inputManifestPath, outputTopLevelManifestPath);
		}
	}

	static void InjectMetadataIntoFabricManifest (string key, string value) {
		XmlDocument doc = new XmlDocument();
		
		doc.Load(outputFabricManifestPath);
		if (doc == null) {
			FabricUtils.Error ("Could not open " + outputFabricManifestRelativePath);
			return;
		}
		
		// Get android namespace
		var applicationNodes = doc.GetElementsByTagName("application");
		if (applicationNodes.Count < 1) {
			FabricUtils.Error ("Could not find <application> tag in " + outputFabricManifestRelativePath);
			return;
		}
		var applicationNode = applicationNodes [0];
		var androidNs = applicationNode.GetNamespaceOfPrefix("android");
		
		var metaElements = doc.GetElementsByTagName("meta-data");
		foreach (XmlElement metaElement in metaElements) {
			if (metaElement.GetAttribute("name", androidNs) == key) {
				if (metaElement.GetAttribute("value", androidNs) == value) {
					FabricUtils.Log (string.Format("Metadata with key {0} already added to {1}", key, outputFabricManifestRelativePath));
					return;
				} else {
					metaElement.ParentNode.RemoveChild(metaElement);
				}
			}
		}

		FabricUtils.Log ("Adding metadata with key {0} to {1}", key, outputFabricManifestRelativePath);

		var metaNode = doc.CreateElement("meta-data");
		metaNode.SetAttribute("name", androidNs, key);
		metaNode.SetAttribute("value", androidNs, value);
		applicationNode.AppendChild (metaNode);
		
		doc.Save (outputFabricManifestPath);
	}

	static void ToggleApplicationInTopLevelManifest () {
		XmlDocument doc = new XmlDocument();		
		doc.Load(outputTopLevelManifestPath);
		if (doc == null) {
			FabricUtils.Error ("Could not open " + outputTopLevelManifestRelativePath);
			return;
		}
		
		var applicationNodes = doc.GetElementsByTagName("application");
		if (applicationNodes.Count != 1) {
			FabricUtils.Error ("Manifest does not have one (and only one) <application> tag: " + outputTopLevelManifestRelativePath);
			return;
		}		
		var applicationNode = applicationNodes [0];
		var androidNs = applicationNode.GetNamespaceOfPrefix("android");

		Settings settings = Settings.Instance;
		if (settings.EnableCrashlytics) {
			FabricUtils.Log ("Enabling Crashlytics in: " + outputTopLevelManifestRelativePath);			
			var applicationNameAttr = doc.CreateNode(XmlNodeType.Attribute, "name", androidNs);
			applicationNameAttr.Value = fabricApplicationName;
			applicationNode.Attributes.SetNamedItem(applicationNameAttr);
		} else {
			var applicationNameAttr = applicationNode.Attributes.GetNamedItem("name", androidNs);
			if (applicationNameAttr != null && applicationNameAttr.Value == fabricApplicationName) {
				FabricUtils.Log ("Disabling Crashlytics in: " + outputTopLevelManifestRelativePath);			
				applicationNode.Attributes.RemoveNamedItem("name", androidNs);
			}
		}

		doc.Save (outputTopLevelManifestPath);
	}

}