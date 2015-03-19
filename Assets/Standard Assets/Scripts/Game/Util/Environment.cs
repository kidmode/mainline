using UnityEngine;
using System.Collections;

namespace GCS
{
	public class Environment : object 
	{
		public Environment() 
		{			
		}
		
		public static string KEY_ACTIVE_ENVIRONMENT = "active_environment";
		public static string KEY_GAME_SERVER_HOST = "game_server_host";
		public static string KEY_GAME_SERVER_SECURE_HOST = "game_server_secure_host";
		public static string KEY_SERVER_VERSION = "server_version";
		public static string KEY_GAME_STATIC_HOST = "game_static_host";
		public static string KEY_ENCRIPTED = "encrypted";

		public static void init()
		{
			TextAsset l_asset = Resources.Load( "Data/environment" ) as TextAsset;
			string l_text = (l_asset).text;			
			Resources.UnloadAsset(l_asset);
			Hashtable l_config = MiniJSON.MiniJSON.jsonDecode(l_text) as Hashtable;
			string l_active;
			
			if (null == l_config)
			{
				Debug.Log("[ERROR]: Failed to load environment settings.");
				return;	
			}
			  
			l_active = l_config[KEY_ACTIVE_ENVIRONMENT] as string;
			s_serverVersion = int.Parse( l_config[KEY_SERVER_VERSION].ToString() );
			s_encrypted = l_config[KEY_ENCRIPTED].ToString().Equals("1") ? true : false;
		
			Debug.Log("[Environment] active_environment: " + l_active);
		
			if (null == l_active)
			{
				Debug.Log("[ERROR]: Could not find a valid 'active_environment' in the environment settings.");
				return;
			}
			
			s_variables = l_config[l_active] as Hashtable;

			s_activeEnvironment = l_active;
			s_host = getVariable(KEY_GAME_SERVER_HOST);
			s_secureHost = getVariable(KEY_GAME_SERVER_SECURE_HOST);
			s_staticHost = getVariable( KEY_GAME_STATIC_HOST );
			s_serviceURL = (s_host + APP_SERVER_PATH);
			s_baseURL = s_host;
	
			
			Debug.Log ("[Environment] active_environment: " + s_activeEnvironment);
			Debug.Log("[Environment] game_static_host: " + s_staticHost );
			Debug.Log("[Environment] game_server_host: " + s_host);	
			Debug.Log("[Environment] serviceURL: " + s_serviceURL);
		}
		
		public static string getVariable(string p_key)
		{
			if (null == s_variables)
				return null;
				
			string l_value = s_variables[p_key] as string;
			if (null == l_value)
			{
				Debug.Log("[WARNING] Environment variable '" + p_key + "' returned null. This could indicate an issue with the environment setup.");
			}
			
			return l_value;	
		}

		public static string getActiveEnvironment()
		{
			return s_activeEnvironment;
		}
		
		public static string getHost()
		{
			return s_host;
		}
		
		public static string getSecureHost()
		{
			return s_secureHost;
		}
		
		public static string getServiceURL()
		{
			return s_serviceURL;
		}
		
		public static string getStaticURL()
		{
			return s_staticHost;
		}
		
		public static string getVersion()
		{
			return s_version;
		}
		
		public static int getServerVersion()
		{
			return s_serverVersion;
		}
		
		public static bool getEncrypted()
		{
			return s_encrypted;
		}
		
		public static void setVersion(string p_ver)
		{
			s_version = p_ver;
		}
		
		public static string getBaseURL()
		{
			return s_baseURL;
		}
	
		public static bool isLowMemDevice()
		{
#if UNITY_IPHONE	
			UnityEngine.iPhoneGeneration[] l_genList = 
				new UnityEngine.iPhoneGeneration[]
				{
					iPhoneGeneration.iPhone,
					iPhoneGeneration.iPhone3G,
					iPhoneGeneration.iPhone3GS,
					iPhoneGeneration.iPodTouch1Gen,
					iPhoneGeneration.iPodTouch2Gen,
					iPhoneGeneration.iPodTouch3Gen,
					iPhoneGeneration.iPodTouch4Gen,
					iPhoneGeneration.iPhone4			
				};
	
			UnityEngine.iPhoneGeneration l_generation = iPhone.generation;
			int l_len = l_genList.Length;
			for (int i = 0; i < l_len; ++i)
			{
				UnityEngine.iPhoneGeneration l_genEntry = l_genList[i];
				if (l_genEntry == l_generation)
					return true;
			}
#endif
			return false;
		}

				
		public static string APP_SERVER_PATH = "/";
		public static string PLATFORM_SERVER_PATH = "/";
		private static Hashtable s_variables = null;
		private static string s_activeEnvironment = null;
		private static string s_host = null;
		private static string s_secureHost = null;
		private static string s_serviceURL = null;
		private static string s_baseURL = null;
		private static string s_staticHost = null;
		private static string s_version = null;
		private static int s_serverVersion;
		private static bool s_encrypted = false;
	}
}