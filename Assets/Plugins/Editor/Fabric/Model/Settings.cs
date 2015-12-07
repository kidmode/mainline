using UnityEngine;
using UnityEditor;
using System;
using System.IO;

using Fabric.Net.OAuth;

namespace Fabric
{
	namespace Model
	{
		public class Settings : ScriptableObject
		{
			private static readonly string SettingsAssetName = "FabricSettings";
			private static readonly string SettingsPath = "Editor Default Resources";
			private static readonly string SettingsAssetExtension = "asset";

			#region Instance
			private static Settings instance;
			public static Settings Instance
			{
				get {
					if (instance == null) {
						string assetNameWithExtension = string.Join (".", new string[] {
							SettingsAssetName,
							SettingsAssetExtension
						});

						if ((instance = EditorGUIUtility.Load (assetNameWithExtension) as Settings) == null) {
							instance = CreateInstance<Settings> ();

							if (!Directory.Exists (Path.Combine (Application.dataPath, SettingsPath))) {
								AssetDatabase.CreateFolder ("Assets", SettingsPath);
							}

							AssetDatabase.CreateAsset (instance, Path.Combine (Path.Combine ("Assets", SettingsPath), assetNameWithExtension));
						}
					}

					return instance;
				}
			}
			#endregion

			#region ApiKey
			[SerializeField]
			private string apiKey;

			[HideInInspector]
			public string ApiKey
			{
				get { return Instance.apiKey; }
				set {
					if (value != Instance.apiKey) {
						Instance.setupComplete = false;
						Instance.lastSetupFailed = false;
					}

					Instance.apiKey = value;
					EditorUtility.SetDirty(Instance);
				}
			}
			#endregion

			#region BuildSecret
			[SerializeField]
			private string buildSecret;

			[HideInInspector]
			public string BuildSecret
			{
				get { return Instance.buildSecret; }
				set {
					Instance.buildSecret = value;
					EditorUtility.SetDirty (Instance);
				}
			}
			#endregion

			#region Email
			[SerializeField]
			private string email;

			[HideInInspector]
			public string Email
			{
				get { return Instance.email; }
				set {
					Instance.email = value;
					EditorUtility.SetDirty (Instance);
				}
			}
			#endregion

			#region OAuth Token
			[SerializeField]
			private string rawToken;

			[HideInInspector]
			private Fabric.Net.OAuth.Client.Token token;

			[HideInInspector]
			public Fabric.Net.OAuth.Client.Token Token
			{
				get {
					if (Instance.token == null)
						Instance.token = Fabric.Net.OAuth.Client.parse (rawToken);
					return Instance.token;
				}
				set {
					Instance.token = value;
					Instance.rawToken = value == null ? null : value.ToString ();
					EditorUtility.SetDirty (Instance);
				}
			}
			#endregion

			#region Organization
			[SerializeField]
			private string organization;

			[HideInInspector]
			public string Organization
			{
				get { return Instance.organization; }
				set {
					Instance.organization = value;
					EditorUtility.SetDirty (Instance);
				}
			}
			#endregion

			#region App Icon
			[SerializeField]
			private string iconUrl;

			[HideInInspector]
			public string IconUrl
			{
				get { return Instance.iconUrl; }
				set {
					Instance.iconUrl = value;
					EditorUtility.SetDirty (Instance);
				}
			}
			#endregion

			#region Dashboard Url
			[SerializeField]
			private string dashboardUrl;

			[HideInInspector]
			public string DashboardUrl
			{
				get { return Instance.dashboardUrl; }
				set {
					Instance.dashboardUrl = value;
					EditorUtility.SetDirty (Instance);
				}
			}
			#endregion

			#region EnabledCrashlytics
			[SerializeField]
			[HideInInspector]
			private bool enabledCrashlytics = false;

			[HideInInspector]
			public bool EnableCrashlytics
			{
				get { return Instance.enabledCrashlytics; }
				set {
					Instance.enabledCrashlytics = value;
					EditorUtility.SetDirty (Instance);
				}
			}
			#endregion

			#region SetupComplete
			[SerializeField]
			private bool setupComplete = false;

			[HideInInspector]
			public bool SetupComplete
			{
				get { return Instance.setupComplete; }
				set {
					Instance.setupComplete = value;
					EditorUtility.SetDirty (Instance);
				}
			}
			#endregion

			#region Activated
			[SerializeField]
			private bool activated = false;

			[HideInInspector]
			public bool Activated
			{
				get { return Instance.activated; }
				set {
					Instance.activated = value;
					EditorUtility.SetDirty (Instance);
				}
			}
			#endregion

			#region LastSetupFailed
			[SerializeField]
			private bool lastSetupFailed = false;

			[HideInInspector]
			public bool LastSetupFailed
			{
				get { return Instance.lastSetupFailed; }
				set {
					Instance.lastSetupFailed = value;
					EditorUtility.SetDirty(Instance);
				}
			}
			#endregion

			#region Sequence
			[SerializeField]
			private uint flowSequence = 0;

			[HideInInspector]
			public uint FlowSequence
			{
				get { return Instance.flowSequence; }
				set {
					Instance.flowSequence = value;
					EditorUtility.SetDirty(Instance);
				}
			}
			#endregion
		}
	}
}