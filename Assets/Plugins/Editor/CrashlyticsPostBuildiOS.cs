using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using CLS.UnityEditor.iOS.Xcode;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Fabric.Model;
using System.Linq;

public class CrashlyticsPostBuildiOS {

	private static string getUUIDForPbxproj() {
		return System.Guid.NewGuid ().ToString ("N").Substring (0, 24).ToUpper ();
	}

	[PostProcessBuild(100)]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string path) {

		Settings settings = Settings.Instance;

		CheckiOSVersion ();

		if (settings.EnableCrashlytics &&
			(string.IsNullOrEmpty(settings.ApiKey) ||
		     string.IsNullOrEmpty(settings.BuildSecret))) {
			FabricUtils.Error ("Unable to find API Key or Build Secret. Fabric was not added to the player.");
			return;
		}

		// BuiltTarget.iOS is not defined in Unity 4, so we just use strings here
		if (buildTarget.ToString () == "iOS" || buildTarget.ToString () == "iPhone") {
			string projPath = Path.Combine (path, "Unity-iPhone.xcodeproj/project.pbxproj");

			PBXProject project = new PBXProject();
			project.ReadFromString(File.ReadAllText(projPath));

			string target = project.TargetGuidByName("Unity-iPhone");

			if (!project.HasFramework("Security.framework")) {
				FabricUtils.Log ("Adding Security.framework to Xcode project");
				project.AddFrameworkToProject(target, "Security.framework", false);
			}

			string frameworksDir = Path.Combine(Directory.GetCurrentDirectory (), "Assets/Plugins/iOS");

			if (!Directory.Exists (Path.Combine(path, "Frameworks/Plugins/iOS/Crashlytics.framework"))) {
				FabricUtils.Log ("Adding Crashlytics.Framework to Xcode project");

				AddThirdPartyFrameworkToProject(project, target, Path.Combine (frameworksDir, "Crashlytics.framework"), path,
				                                "Frameworks/Plugins/iOS/Crashlytics.framework");
			}

			if (!Directory.Exists (Path.Combine(path, "Frameworks/Plugins/iOS/Fabric.framework"))) {
				FabricUtils.Log ("Adding Fabric.framework to Xcode project");

				AddThirdPartyFrameworkToProject(project, target, Path.Combine (frameworksDir, "Fabric.framework"), path,
				                                "Frameworks/Plugins/iOS/Fabric.framework");
			}

			string libzGUID = project.AddFile("usr/lib/libz.dylib", "Libraries/libz.dylib", PBXSourceTree.Sdk);
			project.AddFileToBuild(target, libzGUID);
			string libcppGUID = project.AddFile("usr/lib/libc++.dylib", "Libraries/libc++.dylib", PBXSourceTree.Sdk);
			project.AddFileToBuild(target, libcppGUID);

			FabricUtils.Log ("Adding Framework Search Paths (\"Frameworks/Plugins/iOS\") to Xcode project");
			project.AddBuildProperty(target, "FRAMEWORK_SEARCH_PATHS",
			                         "$(inherited) $(PROJECT_DIR)/Frameworks/Plugins/iOS");

			FabricUtils.Log ("Setting Debug Information Format to DWARF with dSYM File in Xcode project.");
			project.SetBuildProperty(target, "DEBUG_INFORMATION_FORMAT", "dwarf-with-dsym");
		
			File.WriteAllText(projPath, project.WriteToString());

			// Add Fabric to Info.Plist
			Dictionary<string, PlistElementDict> kitsDict = new Dictionary<string, PlistElementDict>();

			if (settings.EnableCrashlytics)
				kitsDict.Add("Crashlytics", new PlistElementDict ());
			
			AddFabricKitsToPlist(path, kitsDict);

			if (settings.EnableCrashlytics)
				AddFabricRunScriptBuildPhase(projPath);

			if (!settings.EnableCrashlytics)
				FabricUtils.Log ("Crashlytics disabled. Crashlytics will not be initialized on app launch.");
		}
	}

	private static void CheckiOSVersion ()
	{
		var settings = Settings.Instance;
		iOSTargetOSVersion[] oldiOSVersions = {
			iOSTargetOSVersion.iOS_4_0,
			iOSTargetOSVersion.iOS_4_1,
			iOSTargetOSVersion.iOS_4_2,
			iOSTargetOSVersion.iOS_4_3,
			iOSTargetOSVersion.iOS_5_0,
			iOSTargetOSVersion.iOS_5_1
		};
		var isOldiOSVersion = oldiOSVersions.Contains (PlayerSettings.iOS.targetOSVersion);
		
		if (settings.EnableCrashlytics && isOldiOSVersion) {
			FabricUtils.Error ("Crashlytics requires iOS 6+. Please change the Target iOS Version in Player Settings to iOS 6 or higher.");
		}
	}

	private static void AddFabricRunScriptBuildPhase (string projPath)
	{
		// Shell Script Build Phase
		var xcodeProjectLines = File.ReadAllLines (projPath);
		foreach (var line in xcodeProjectLines) {
			if (line.Contains("Fabric.framework/run"))
				return;
		}

		var settings = Settings.Instance;
		var scriptUUID = getUUIDForPbxproj ();
		var inBuildPhases = false;
		var sb = new StringBuilder ();			
		
		FabricUtils.Log ("Adding Fabric.framework/run Run Script Build Phase to Xcode project");
		
		foreach (var line in xcodeProjectLines) {
			if (line.Contains ("/* Begin PBXResourcesBuildPhase section */")) {
				sb.AppendLine (line);
				
				sb.Append (
					"\t\t" + scriptUUID + " /* ShellScript */ = {\n" +
					"\t\t\tisa = PBXShellScriptBuildPhase;\n" +
					"\t\t\tbuildActionMask = 2147483647;\n" +
					"\t\t\tfiles = (\n" +
					"\t\t\t);\n" +
					"\t\t\tinputPaths = (\n" +
					"\t\t\t);\n" +
					"\t\t\toutputPaths = (\n" +
					"\t\t\t);\n" +
					"\t\t\trunOnlyForDeploymentPostprocessing = 0;\n" +
					"\t\t\tshellPath = \"/bin/sh -x\";\n" +
					"\t\t\tshellScript = \"chmod u+x ./Frameworks/Plugins/iOS/Fabric.framework/run\n" +
						                  "./Frameworks/Plugins/iOS/Fabric.framework/run " + settings.ApiKey + " " + settings.BuildSecret + " --skip-check-update\";\n" +
					"\t\t};\n"
					);
			} else if (line.Contains ("buildPhases = (")) {
				inBuildPhases = true;
				sb.AppendLine(line);
			} else if (inBuildPhases && line.Contains(");")) {
				inBuildPhases = false;
				sb.AppendLine ("\t\t\t\t" + scriptUUID + " /* ShellScript */,");
				sb.AppendLine (line);
				
			} else {
				sb.AppendLine(line);
			}
			
		}
		
		File.WriteAllText(projPath, sb.ToString());
	}

	// Takes the build path where Info.plist is located
	// and a Dictionary<string, PlistElementDict> (kits) where
	// key: the KitName and
	// value: a PlistElementDict containing the KitInfo
	private static void AddFabricKitsToPlist (string buildPath, Dictionary<string, PlistElementDict> kits)
	{
		Settings settings = Settings.Instance;
		string plistPath = Path.Combine (buildPath, "Info.plist");
		
		PlistDocument plist = new PlistDocument();
		plist.ReadFromFile(plistPath);
		
		PlistElementDict plistFabric = plist.root.CreateDict("Fabric");
		plistFabric.SetString("APIKey", settings.ApiKey);
		
		PlistElementArray plistFabricKits = plistFabric.CreateArray("Kits");
		
		foreach (KeyValuePair<string, PlistElementDict> entry in kits) {
			PlistElementDict plistKitDict = plistFabricKits.AddDict();
			plistKitDict.SetString("KitName", entry.Key);
			plistKitDict["KitInfo"] = entry.Value;
		}

		plist.root.SetString("CrashlyticsUnityVersion", Fabric.Editor.version.ToString());
		
		plist.WriteToFile(plistPath);
	}

	// Copy and add a framework (Link Phase) to a PBXProject
	//
	// PBXProject project: the project to modify
	// string target: the target project's GUID
	// string framework: the path to the framework to add
	// string projectPath: the path to add the framework in the project, relative to the project root
	private static void AddThirdPartyFrameworkToProject(PBXProject project, string target,
	                                             string framework, string buildPath, string projectPath)
	{
		DirectoryCopy(framework, Path.Combine(buildPath, projectPath), true);
		string guid = project.AddFile (projectPath, projectPath);
		project.AddFileToBuild(target, guid);
	}

	// MSDN
	private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
	{
		// Get the subdirectories for the specified directory.
		DirectoryInfo dir = new DirectoryInfo(sourceDirName);
		DirectoryInfo[] dirs = dir.GetDirectories();

		if (!dir.Exists)
		{
			throw new DirectoryNotFoundException(
				"Source directory does not exist or could not be found: "
				+ sourceDirName);
		}

		// If the destination directory doesn't exist, create it.
		if (!Directory.Exists(destDirName))
		{
			Directory.CreateDirectory(destDirName);
		}

		// Get the files in the directory and copy them to the new location.
		FileInfo[] files = dir.GetFiles();
		foreach (FileInfo file in files)
		{
			string temppath = Path.Combine(destDirName, file.Name);
			file.CopyTo(temppath, false);
		}

		// If copying subdirectories, copy them and their contents to new location.
		if (copySubDirs)
		{
			foreach (DirectoryInfo subdir in dirs)
			{
				string temppath = Path.Combine(destDirName, subdir.Name);
				DirectoryCopy(subdir.FullName, temppath, copySubDirs);
			}
		}
	}
}
