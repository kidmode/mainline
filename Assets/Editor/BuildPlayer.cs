using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System;

public class BuildPlayer : MonoBehaviour
{






	#region Android MAIN_LINE
	
	
	[MenuItem( "Build/Android/MAINLINE_RELEASE" )]
	public static void Build_MAINLINE_RELEASE()
	{
		Dictionary<string , string> stringsParams = new Dictionary<string , string>();
		
		BuildOptions androidBuildOptions = BuildOptions.ShowBuiltPlayer;
		
		build("MainLine", "Assets/keystore/android", "android", "android", "android", androidBuildOptions);
	}
	
	
	[MenuItem( "Build/Android/MAINLINE_DEV_DEBUG_PLAY" )]
	public static void Build_MAINLINE_DEV_DEBUG_PLAY()
	{
		Dictionary<string , string> stringsParams = new Dictionary<string , string>();
		
		BuildOptions androidBuildOptions = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.AutoRunPlayer | BuildOptions.ShowBuiltPlayer;
		
		build("MainLine", "Assets/keystore/android", "android", "android", "android", androidBuildOptions);
	}
	
	
	#endregion
	
	#region BuildFunctions

	private static void  build(string branch, string keystoreName, string keystorePass, string keyAlias, string keyAliasPass, BuildOptions buildOptions )
     {
 		//=============================================
 		//=============================================
 		//Player settings
 		//=============================================
 		
//         PlayerSettings.productName = nodeChineseName.InnerText;//"Slots in Time";
// 		
//         PlayerSettings.bundleIdentifier = bundleID;
// 		
//         PlayerSettings.bundleVersion = Config.version;
//         PlayerSettings.Android.bundleVersionCode = Config.versionCode;
// 		

		//Setup Keystore and passwords

		PlayerSettings.Android.keystoreName = keystoreName + ".keystore";
		PlayerSettings.Android.keystorePass = keystorePass;
		
		PlayerSettings.Android.keyaliasName = keyAlias;
		PlayerSettings.Android.keyaliasPass = keyAliasPass;


		//Start prompt for build path
		String path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");

		Debug.Log("  path " + path);

		if(path == ""){

			Debug.Log(" no path");
			return;

		}

		//Set the file name of the build
		string targetFileName = "KidMode" + "_"  + branch + "_" + System.DateTime.Now.ToString( "yyyyMMddHHmm" ) + ".apk";

		//Get the string array of enabled build scenes
		string[] scenesArray = getEnabledBuildScenes();

		Debug.Log("  scenesArray " + scenesArray[0]);


		//Start Building the player
		BuildPipeline.BuildPlayer( scenesArray , path + "/" + targetFileName , BuildTarget.Android , buildOptions );

     }


	private static string[] getEnabledBuildScenes(){

		List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
		List<string> enabledScenes = new List<string>();
		foreach (EditorBuildSettingsScene scene in scenes)
		{
			if (scene.enabled)
			{
				enabledScenes.Add(scene.path);
			}
		}

		string[] scenesArray = enabledScenes.ToArray();

		return scenesArray;

	}

	#endregion
}