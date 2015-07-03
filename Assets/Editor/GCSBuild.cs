//
// GCSBuild.cs
// Copyright 2013, GameCloud Studios, Inc. All rights reserved
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

using UnityEditor;
using UnityEngine;

public class GCSBuild : EditorWindow
{
	private static string s_versionFilePath = "Assets/Resources/Data/buildSettings.txt";
	
	static void UpdateVersion()
	{
		string l_version = "";
		// check for null version in the case where this is run from the Editor instead of
		// through the command line with the VERSION_STRING environment variable set
		
		
		//throw new Exception("TEST");
		
		/*foreach (DictionaryEntry var in System.Environment.GetEnvironmentVariables())
		{
            UnityEngine.Debug.Log("VARIABLE= " + var.Key + " = " + var.Value);
		}*/
		
		string l_tag = String.Empty;
		string l_tagRaw = System.Environment.GetEnvironmentVariable("VERSION_TAG");
		
		l_tag = (string.IsNullOrEmpty(l_tagRaw) ? String.Empty : (" (" + l_tagRaw + ")")); 
		
		l_version = System.Environment.GetEnvironmentVariable("VERSION_MAJOR") + "." +
			System.Environment.GetEnvironmentVariable("VERSION_MINOR") + "."
				+ System.Environment.GetEnvironmentVariable("VERSION_BUILD") + "." //l_days + "." +
				+ System.Environment.GetEnvironmentVariable("VERSION_REVISION")
				+ l_tag; //4233;
		
		UnityEngine.Debug.Log("VERSION_STRING: " + l_version);
		SaveVersionFile(l_version);			
		PlayerSettings.bundleVersion = System.Environment.GetEnvironmentVariable("VERSION_MAJOR") + "." +
			System.Environment.GetEnvironmentVariable("VERSION_MINOR") + "."
				+ System.Environment.GetEnvironmentVariable("VERSION_BUILD");
		
		// changed resources so force a Unity update
		//may not be needed: 
		AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);	
	}
	
	private static void SaveVersionFile(string p_version)
	{
		try
		{
			/*if (false == File.Exists(s_versionFilePath))
			{
				throw new Exception("File '" + s_versionFilePath + "' not found.");
			}*/
			
			FileInfo l_info = new FileInfo(s_versionFilePath);
			
			using (StreamWriter l_writer = l_info.CreateText())
			{
				if (null == l_writer)
				{
					throw new Exception("Error: Failed to write version file.");
				}
				
				l_writer.WriteLine("version," + p_version);
				l_writer.Flush();
				l_writer.Close();
			}
		}
		catch(Exception ex)
		{
			UnityEngine.Debug.Log(String.Format (
				"Failed to write version to buildSettings.txt: '{0}'",
				ex.Message));	
		}
		
		//EditorUtility.DisplayDialog ("GetVersion", "GetVersion returned: " + l_version, "OK");	
	}
	
	#if GCS_BUILD_INCOMPLETE
	[MenuItem ("GameCloud/Execute Dev Build...", false, 0)]
	public static void ExecuteDevBuild()
	{		
		// adjust dev settings
		ExecuteBuild (false);
	}
	
	[MenuItem ("GameCloud/Execute Production Build...", false, 0)]
	public static void ExecuteProductionBuild()
	{
		// adjust production settings
		ExecuteBuild (true);
	}	
	
	private static void ExecuteBuild(bool p_isProdBuild)
	{
		string l_version = GetVersion();
		//string l_workingDirectory = Directory.GetCurrentDirectory();
		bool l_shouldInstallTracking = true;
		
		PlayerSettings.bundleVersion = l_version;
		
		string l_xcodeDirectory = String.Format("{0}_{1}", 
		                                        (p_isProdBuild) ? "gcs_prod" : "gcs_dev",
		                                        l_version);
		
		MobageSettings.setBuildDirectory(l_xcodeDirectory);
		
		MobageSettings.Platform l_activePlatform = MobageSettings.Platform.iOSDevice;
		MobageSettings.setShouldLinkInstallTracking(l_shouldInstallTracking);
		
		MobageSettings.buildActivePlatform(l_activePlatform, l_shouldInstallTracking);		
		
		DeleteFile(l_xcodeDirectory + "/Default-568h@2x.png");
	}
	#endif	
	//[MenuItem ("GameCloud/Get Version...", false, 0)]
	private static string GetVersion()
	{
		string l_version = "0.0.0.0";
		string l_path = "Assets/Resources/Data/buildSettings.txt";		
		
		try
		{
			if (false == File.Exists(l_path))
			{
				throw new Exception("File '" + l_path + "' not found.");
			}
			
			String l_line;
			FileInfo l_info = new FileInfo(l_path);
			
			StreamReader l_reader = l_info.OpenText();
			
			while (null != (l_line = l_reader.ReadLine()))
			{
				string l_pattern = @"\s*([^,]+)(,)\s*(?<version>(?<major>[^.]+).(?<minor>[^.]+).(?<build>[^.]+).(?<revision>[^.]+))";
				//@"\s*([^,]+)(,)\s*(?<version>\S+)";
				RegexOptions l_options = (RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
				
				System.Text.RegularExpressions.Match m = Regex.Match(l_line, l_pattern, l_options);
				if (m.Success)
				{					
					Group l_vGroup = m.Groups["version"];
					if (null != l_vGroup)
					{				
						l_version = l_vGroup.Value;
					}
				}
				else
				{
					throw new Exception("Could not match version string.");
				}
			}
			l_reader.Close();
		}
		catch(Exception ex)
		{
			UnityEngine.Debug.Log(String.Format (
				"Failed to read version from buildSettings.txt: '{0}'",
				ex.Message));	
		}
		
		//EditorUtility.DisplayDialog ("GetVersion", "GetVersion returned: " + l_version, "OK");
		return l_version;	
	}
	
	private static void DeleteFile(string p_path)
	{
		if (File.Exists(p_path))
		{
			File.Delete(p_path);
			System.Diagnostics.Debug.WriteLine(String.Format("Deleted file: '{0}'", p_path));
		}
		else
		{
			throw new Exception(String.Format ("Failed to delete file: '{0}'", p_path));	
		}
	}
	
	static string[] GetBuildScenes()
	{
		List<string> names = new List<string>();
		
		foreach(EditorBuildSettingsScene e in EditorBuildSettings.scenes)
		{
			if(e==null)
				continue;
			
			if(e.enabled)
				names.Add(e.path);
		}
		return names.ToArray();
	}
	
	static string GetBuildPath()
	{
		return System.IO.Path.Combine(Directory.GetCurrentDirectory(), "build_gcs_dev"); //build/iPhone";
	}
	
	//[UnityEditor.MenuItem("GCSBuild/Test Command Line Build Step")]
	public static void BuildIphone()
	{
		UnityEngine.Debug.Log("iPhone Build\n------------------\n------------------");
		UpdateVersion();
		string[] scenes = GetBuildScenes();
		string path = System.Environment.GetEnvironmentVariable("NAME");
		//		string buildType = System.Environment.GetEnvironmentVariable("TYPE");
		if(scenes == null || scenes.Length==0 || path == null)
		{
			path = GetBuildPath();
		}
		
		PlayerSettings.bundleIdentifier = System.Environment.GetEnvironmentVariable("BUNDLE_ID");
		
		UnityEngine.Debug.Log(string.Format("Path: \"{0}\"", path));
		for(int i=0; i<scenes.Length; ++i)
		{
			UnityEngine.Debug.Log(string.Format("Scene[{0}]: \"{1}\"", i, scenes[i]));
		}
		
		UnityEngine.Debug.Log("Starting iPhone Build!");
		BuildPipeline.BuildPlayer(scenes, path, BuildTarget.iPhone, BuildOptions.None);		
	}
	
	
	//[UnityEditor.MenuItem("GCSBuild/Test Command Line Build Step Android")]
	public static void BuildAndroid()
	{
		UnityEngine.Debug.Log("Android Build\n------------------\n------------------");
		UpdateVersion();
		string[] scenes = GetBuildScenes();
		string path = System.Environment.GetEnvironmentVariable("NAME") + ".apk";
		//		string buildType = System.Environment.GetEnvironmentVariable("TYPE");
		//will put the .apk file in the root directory for the project along with Assets, library etc.
		
		if(scenes == null || scenes.Length==0 || path == null)
			return;
		
		PlayerSettings.bundleIdentifier = System.Environment.GetEnvironmentVariable("BUNDLE_ID");

		PlayerSettings.Android.bundleVersionCode++;

		string useAPKExpansion = System.Environment.GetEnvironmentVariable("SPLIT");
		
		if(useAPKExpansion == "true")
		{
			PlayerSettings.Android.useAPKExpansionFiles = true;
			UnityEngine.Debug.Log("Using APK Expansion Files");
		}
		else
		{
			PlayerSettings.Android.useAPKExpansionFiles = false;	
		}
		
		if (PlayerSettings.Android.useAPKExpansionFiles)
		{
			int l_numScenes = (scenes.Length + 1);
			string[] l_expandedScenes = new string[l_numScenes];
			
			// add expansion handling as first scene
			l_expandedScenes[0] = "Assets/Scenes/ExpansionPreloader.unity";
			
			for (int i = 1; i < l_numScenes; i++)
			{
				l_expandedScenes[i] = scenes[i - 1];
			}
			scenes = l_expandedScenes;
			
			// the bundle version code is needed for expansion (obb) files
			PlayerSettings.Android.bundleVersionCode = int.Parse(System.Environment.GetEnvironmentVariable("VERSION_REVISION"));
		}

		UnityEngine.Debug.Log(
			string.Format("keystore: [{0}/{1}] keyalias: [{2}/{3}]",
		        new string[] {System.Environment.GetEnvironmentVariable("KEYSTORE_NAME"),
				System.Environment.GetEnvironmentVariable("KEYSTORE_PASS"),
				System.Environment.GetEnvironmentVariable("KEYALIAS_NAME"),
				System.Environment.GetEnvironmentVariable("KEYALIAS_PASS")}));
		
		string l_keystorePath = System.Environment.GetEnvironmentVariable("KEYSTORE_NAME");
		if ((null != l_keystorePath) && (false == string.Empty.Equals(l_keystorePath)))
		{
			UnityEngine.Debug.Log("Assigning keystore information...");
			PlayerSettings.Android.keystoreName = l_keystorePath;
			PlayerSettings.Android.keystorePass = System.Environment.GetEnvironmentVariable("KEYSTORE_PASS");
			PlayerSettings.Android.keyaliasName = System.Environment.GetEnvironmentVariable("KEYALIAS_NAME");
			PlayerSettings.Android.keyaliasPass = System.Environment.GetEnvironmentVariable("KEYALIAS_PASS");
		}

		UnityEngine.Debug.Log(string.Format("Path: \"{0}\"", path));
		for(int i=0; i<scenes.Length; ++i)
		{
			UnityEngine.Debug.Log(string.Format("Scene[{0}]: \"{1}\"", i, scenes[i]));
		}
		
		UnityEngine.Debug.Log("Starting Android Build!");
		BuildPipeline.BuildPlayer(scenes, path, BuildTarget.Android, BuildOptions.None);
	}	
}
