using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System;

//=============================================
//Kevin
//Build menu for building the app so some settings could be set automatically
//=============================================

public class BuildPlayer : MonoBehaviour
{



	//=============================================
	//Build Menu for Main Line
	//=============================================


	#region Android MAIN_LINE

	[MenuItem( "Build/Android/MAINLINE_RELEASE_STAGING" )]
	public static void Build_MAINLINE_RELEASE_STAGING()
	{

		writeServerEnvironment(SERVER_STAGING);

		BuildOptions androidBuildOptions = BuildOptions.ShowBuiltPlayer;
		
		build("MainLine", "Assets/keystore/android", "android", "android", "android", androidBuildOptions);

//		writeServerEnvironment();

		return;

	}
	
	
	[MenuItem( "Build/Android/MAINLINE_RELEASE_PRODUCTION" )]
	public static void Build_MAINLINE_RELEASE_PRODUCTION()
	{

		writeServerEnvironment(SERVER_PRODUCTION);
//		Texture2D icon = ImageCache.getCacheImage
//		Texture2D[] icons = new Texture2D[]{icon, icon, icon, icon};


		
//		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
//			PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, icons);




		BuildOptions androidBuildOptions = BuildOptions.ShowBuiltPlayer;
		
		build("MainLine", "Assets/keystore/android", "android", "android", "android", androidBuildOptions);

	}

	[MenuItem( "Build/Android/MAINLINE_RELEASE_PLAY" )]
	public static void Build_MAINLINE_RELEASE_PLAY()
	{
		
		BuildOptions androidBuildOptions = BuildOptions.ShowBuiltPlayer | BuildOptions.AutoRunPlayer;
		
		build("MainLine", "Assets/keystore/android", "android", "android", "android", androidBuildOptions);
		
	}
	

	//Note this is not for debug MODE which is another branch
	[MenuItem( "Build/Android/MAINLINE_DEV_DEBUG_RELEASE_PLAY" )]
	public static void Build_MAINLINE_DEV_DEBUG_RELEASE_PLAY()
	{

		writeServerEnvironment(SERVER_PRODUCTION);

		BuildOptions androidBuildOptions = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.AutoRunPlayer | BuildOptions.ShowBuiltPlayer;
		
		build("MainLine", "Assets/keystore/android", "android", "android", "android", androidBuildOptions);

	}

	//Staging
	[MenuItem( "Build/Android/MAINLINE_DEV_DEBUG_STAGING_PLAY" )]
	public static void Build_MAINLINE_DEV_DEBUG_STAGING_PLAY()
	{
		
		writeServerEnvironment(SERVER_STAGING);
		
		BuildOptions androidBuildOptions = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.AutoRunPlayer | BuildOptions.ShowBuiltPlayer;
		
		build("MainLine", "Assets/keystore/android", "android", "android", "android", androidBuildOptions);
		
	}
	
	
	#endregion
	
	#region BuildFunctions
	
	private static void  build(string branch, string keystoreName, string keystorePass, string keyAlias, string keyAliasPass, BuildOptions buildOptions )
	{

		//====+++==   ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		//Bundle Code
		string newBundleCodeWriteString = "public static class CurrentBundleVersion{public static readonly string version = \"" + 
			PlayerSettings.bundleVersion + "(" + PlayerSettings.Android.bundleVersionCode + ")\";}";
		
		StreamWriter writer = new StreamWriter(Application.dataPath  + "/Standard Assets/Scripts/Config/CurrentBundleVersion.cs"); 
		
		writer.WriteLine(newBundleCodeWriteString);
		
		writer.Close();


		//====+++==   ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		//Set Icons for build
		//====+++==   ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		Texture2D icon192 = getTextureAtLocation(Application.dataPath + "/Icons/app_icon_192.png");
		Texture2D icon144 = getTextureAtLocation(Application.dataPath + "/Icons/app_icon_144.png");
		Texture2D icon96 = getTextureAtLocation(Application.dataPath + "/Icons/app_icon_96.png");
		Texture2D icon72 = getTextureAtLocation(Application.dataPath + "/Icons/app_icon_72.png");
		Texture2D icon48 = getTextureAtLocation(Application.dataPath + "/Icons/app_icon_48.png");
		Texture2D icon36 = getTextureAtLocation(Application.dataPath + "/Icons/app_icon_36.png");
		
		Texture2D[] icons = new Texture2D[]{icon192, icon144, icon96, icon72, icon48, icon36};
		
		PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, icons);
		//====+++==   ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		//====+++==   ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		//====+++==   ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


		
		AssetDatabase.Refresh();

//		return;

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
		

		//PlayerSettings.SetIconsForTargetGroup
		//Start prompt for build path
		String path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
		
		Debug.Log("  path " + path);
		
		if(path == ""){
			
			Debug.Log(" no path");
			return;
			
		}
		
		
		//======================================== ========== ========================================
		//Get Environment Info
		//======================================== ========== ========================================
		string buildSettingEnvironmentString = "";
		
		TextAsset l_asset = Resources.Load( "Data/environment" ) as TextAsset;
		string l_text = (l_asset).text;			
		Resources.UnloadAsset(l_asset);
		Hashtable l_config = MiniJSON.MiniJSON.jsonDecode(l_text) as Hashtable;
		string l_active;
		
		
		l_active = l_config[GCS.Environment.KEY_ACTIVE_ENVIRONMENT] as string;
		
		if(l_active == "gcs_prod" ){
			
			buildSettingEnvironmentString = "prod";
			
		}else if(l_active == "gcs_staging" ){
			
			buildSettingEnvironmentString = "staging";
			
		}
		
		Debug.Log("  l_active environemnt " + l_active);
		
		
		
		//======================================== ========== ========================================
		
		
		//======================================== ========== ========================================
		//Get more Build Settings
		//======================================== ========== ========================================
		//		PlayerSettings.Android.bundleVersionCode
		
		//		PlayerSettin
		
		//======================================== ========== ========================================
		
		
		//"yyyy_MM_dd_HH_mm"
		
		
		
		//Set the file name of the build
		string targetFileName = "KidMode" + "_" + System.DateTime.Now.ToString( "MM_dd_yyyy_mm_HH" ) + "_" + 
			buildSettingEnvironmentString + "_u467_v" + PlayerSettings.bundleVersion + "(" +
				PlayerSettings.Android.bundleVersionCode + ")" + "_" + branch + ".apk";
		
		
		
		Debug.Log("  targetFileName " + targetFileName);
		
//		return;
		
		
		//Get the string array of enabled build scenes
		string[] scenesArray = getEnabledBuildScenes();
		
		Debug.Log("  scenesArray " + scenesArray[0]);
		
		
		//Start Building the player
		BuildPipeline.BuildPlayer( scenesArray , path + "/" + targetFileName , BuildTarget.Android , buildOptions );
		
	}

	private static Texture2D getTextureAtLocation(string texturePath){

		
		byte[] bytes = File.ReadAllBytes(texturePath); 
		
		Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		texture.LoadImage(bytes);
		return texture;

	}

	//Get all the scenes that are enabled in the build settings
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


	public const int SERVER_DEV = 1;

	public const int SERVER_STAGING = 2;

	public const int SERVER_PRODUCTION = 3;

	public const int SERVER_LOCAL = 4;



	private static void writeServerEnvironment(int serverType){

		string serverTypeString = "";

		if(serverType == SERVER_DEV){

			serverTypeString = "gcs_dev";

		}else if(serverType == SERVER_STAGING){

			serverTypeString = "gcs_staging";
			
		}else if(serverType == SERVER_PRODUCTION){

			serverTypeString = "gcs_prod";
			
		}else if(serverType == SERVER_LOCAL){

			serverTypeString = "gcs_local";
			
		}

		//====+++==   ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		//Bundle Code
		string newEnvironmentCode = "{\n" + 
			"\t\"active_environment\":\"" + serverTypeString + "\",\n" +
				"\t\"server_version\": 1,\n" +
				"\t\"encrypted\":\"0\",\n\n" +

				"\t\"gcs_dev\":\n" +
				"\t{\n" +
				"\t\t\"game_server_host\":\"http://dev.zoodles.com\",\n" +
				"\t\t\"game_server_secure_host\":\"https://dev.zoodles.com\",\n" +
				"\t\t\"game_static_host\":\"http://dev.zoodles.com\",\n" +
				"\t\t\"game_platform_host\":\"http://dev.zoodles.com\"\n" +
				"\t},\n\n" +
				"\t\"gcs_staging\":\n" +
				"\t{\n" +
				"\t\t\"game_server_host\":\"http://staging.zoodles.com\",\n" +
				"\t\t\"game_server_secure_host\":\"https://staging.zoodles.com\",\n" +
				"\t\t\"game_static_host\":\"http://staging.zoodles.com\",\n" +
				"\t\t\"game_platform_host\":\"http://staging.zoodles.com\"\n" +
				"\t},\n\n" +
				"\t\"gcs_prod\":\n" +
				"\t{\n" +
				"\t\t\"game_server_host\":\"http://www.zoodles.com\",\n" +
				"\t\t\"game_server_secure_host\":\"https://www.zoodles.com\",\n" +
				"\t\t\"game_static_host\":\"http://www.zoodles.com\",\n" +
				"\t\t\"game_platform_host\":\"http://www.zoodles.com\"\n" +
				"\t},\n\n" +
				"\t\"gcs_local\":\n" +
				"\t{\n" +
				"\t\t\"game_server_host\":\"http://192.168.206.7\",\n" +
				"\t\t\"game_server_secure_host\":\"https://192.168.206.7\",\n" +
				"\t\t\"game_static_host\":\"http://192.168.206.7\",\n" +
				"\t\t\"game_platform_host\":\"http://192.168.206.7\"\n" +
				"\t}\n" +
				"}";

		StreamWriter writer = new StreamWriter(Application.dataPath  + "/Resources/Data/environment.txt"); 
		
		writer.WriteLine(newEnvironmentCode);


		//Kevin
		//Could do the following but. . .. . . no line breaks and formats so hard to edit so don't use for now
//		TextAsset l_asset = Resources.Load( "Data/environment" ) as TextAsset;
//		string l_text = (l_asset).text;			
//		Resources.UnloadAsset(l_asset);
//		Hashtable l_config = MiniJSON.MiniJSON.jsonDecode(l_text) as Hashtable;
//
//		writer.WriteLine(MiniJSON.MiniJSON.jsonEncode(l_config));

		
		writer.Close();


	}
	#endregion
}