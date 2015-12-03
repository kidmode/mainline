using UnityEngine;
using UnityEditor;
using Fabric.Model;
using Fabric.View;
using System.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace Fabric
{
	public static class Controller
	{
		#region OAuth.Client
		private static Fabric.Net.OAuth.Client client;
		private static Fabric.Net.OAuth.Client Client
		{
			get {
				if (client == null)
					client = new Fabric.Net.OAuth.Client (Net.Constants.URI);
				return client;
			}
		}
		#endregion

		#region Fetch
		private static T Fetch<T>(Net.OAuth.Client.Token token, Func<T> call, uint tries = 2)
		{
			if (token == null || tries == 0) {
				return default (T);
			}
			
			try {
				return call ();
			} catch (API.V1.UnauthorizedException) {
				activationStatus = ActivationStatus.RefreshRequired;
				return default (T);
			}
		}
		#endregion

		#region Organizations
		private static List<Organization> organizations;
		private static List<Organization> Organizations(Net.OAuth.Client.Token token)
		{
			if (organizations == null) {
				organizations = Fetch (token, () => new API.V1 (Net.Constants.URI, Client, token).Organizations ());
			}

			return organizations;
		}
		#endregion

		#region Apps
		private static List<App> Apps(string organization, Net.OAuth.Client.Token token)
		{
			// There is no need to cache the value, since this will only be called from
			// the context of polling for activation.
			// If the flow isn't followed from the begining, this will cause 'SetDirty() can only be called from the main thread'
			return Fetch (token, () => new API.V1 (Net.Constants.URI, Client, token).ApplicationsFor (organization, Organizations (token)));
		}
		#endregion

		private static string password;
		
		private static Page login = new LoginPage (Login ());
		private static Page orgs = new OrganizationsPage (SelectOrganization (), FetchOrganizations ());
		private static Page prefab = new PrefabPage (AdvanceToValidationPage (), CrashlyticsInitDragAction (), CrashlyticsInitAcceptPrefabDrop);
		private static Page instructions = new InstructionsPage (ApplyInstructions (), BackToOrganizations ());
		private static Page validation = new ValidationPage (QueryForActivation (), ResetActivationStatus ());
		private static Page dashboard = new DashboardPage (DownloadIcon (), FetchDashboardUrl ());

		public enum LoginStatus { Unknown, Success, Unauthorized, Other };
		public delegate void LoginAction<T, U>(T password, out U status);

		#region FetchOrganizations
		public static Func<List<Organization>> FetchOrganizations()
		{
			return delegate() {
				return Organizations (Settings.Instance.Token);
			};
		}
		#endregion

		public enum ActivationStatus { Activated, Checking, TimedOut, RefreshRequired, Unknown };
		private static ActivationStatus activationStatus = ActivationStatus.Unknown;
		private static Texture2D Icon;
		private static string IconUrl;
		private static string DashboardUrl;

		private static Detail.Runner Runner = null;
		
		#region ActivationTimer
		private static Timer activationTimer = new Timer ();
		private static Timer ActivationTimer
		{
			get {
				string bundleIdentifier = PlayerSettings.bundleIdentifier;
				string organization = Settings.Instance.Organization;

				Net.OAuth.Client.Token token = Settings.Instance.Token;

				activationTimer.Stop ();
				activationTimer = new Timer ();
				activationTimer.Elapsed += delegate(object sender, ElapsedEventArgs e) {
					foreach (App app in Apps (organization, token)) {
						if (app.BundleIdentifier == bundleIdentifier) {
							activationTimer.Stop ();
							timeoutTimer.Stop ();

							activationStatus = ActivationStatus.Activated;
							IconUrl = app.IconUrl;
							DashboardUrl = app.DashboardUrl;
							break;
						}
					}
				};
				activationTimer.Interval = 10 * 1000;  // 10 seconds
				return activationTimer;
			}
		}
		#endregion

		#region TimeoutTimer
		private static Timer timeoutTimer = new Timer ();
		private static Timer TimeoutTimer
		{
			get {
				timeoutTimer.Stop ();
				timeoutTimer = new Timer ();
				timeoutTimer.Elapsed += delegate(object sender, ElapsedEventArgs e) {
					activationTimer.Stop ();
					activationTimer.Dispose ();
					activationStatus = ActivationStatus.TimedOut;
					timeoutTimer.Stop ();
					timeoutTimer.Dispose ();
				};
				timeoutTimer.Interval = 60 * 1000 * 10; // 10 minutes
				return timeoutTimer;
			}
		}
		#endregion

		#region ResetActivationStatus
		public static Action ResetActivationStatus()
		{
			return delegate() {
				activationStatus = ActivationStatus.Unknown;
			};
		}
		#endregion

		#region QueryForActivation
		public static Func<ActivationStatus> QueryForActivation()
		{
			return delegate() {
				if (activationStatus == ActivationStatus.Unknown) {
					ActivationTimer.Start ();
					TimeoutTimer.Start ();

					activationStatus = ActivationStatus.Checking;
				}

				if (activationStatus == ActivationStatus.RefreshRequired) {
					Settings.Instance.Token = Client.Refresh (Settings.Instance.Token);
					activationStatus = ActivationStatus.Unknown;
				}

				return activationStatus;
			};
		}
		#endregion

		#region DownloadIcon
		public static Func<Texture2D> DownloadIcon()
		{
			return delegate() {
				if (Icon == null && !string.IsNullOrEmpty (Settings.Instance.IconUrl) && Runner == null) {
					Runner = Detail.Runner.StartCoroutine(Fabric.API.V1.DownloadTexture (Settings.Instance.IconUrl, delegate (Texture2D texture) {
						Icon = texture;
					}));
				}

				return Icon;
			};
		}
		#endregion

		#region FetchDashboardUrl
		public static Func<string> FetchDashboardUrl()
		{
			return delegate() {
				return Settings.Instance.DashboardUrl;
			};
		}
		#endregion

		#region PageFromState
		public static Page PageFromState()
		{
			if (Settings.Instance.Activated || activationStatus == ActivationStatus.Activated) {
				if (DashboardUrl != null) {
					Settings.Instance.DashboardUrl = DashboardUrl;
				}
				if (IconUrl != null) {
					Settings.Instance.IconUrl = IconUrl;
				}

				Settings.Instance.Activated = true;
				AdvanceToDashboardPage ()();
			}

			if (Settings.Instance.Token == null) {
				organizations = null;
				return login;
			}
			
			if (String.IsNullOrEmpty (Settings.Instance.ApiKey) ||
			    String.IsNullOrEmpty (Settings.Instance.BuildSecret) ||
			    String.IsNullOrEmpty (Settings.Instance.Organization)) {
				return orgs;
			}
			
			if (!Settings.Instance.SetupComplete) {
				return instructions;
			}
			
			switch (Settings.Instance.FlowSequence) {
			case 0:
				if (!CrashlyticsInitAlreadyExists ()) {
					return prefab;
				}
				AdvanceToValidationPage ();
				goto case 1;
			case 1: return validation;
			case 2: return dashboard;
			}

			return dashboard;
		}
		#endregion

		#region Login
		private static LoginAction<string, LoginStatus> Login()
		{
			return delegate(string password, out LoginStatus status) {
				status = LoginStatus.Unknown;
				
				if (Settings.Instance.Email != null && password != null) {
					try {
						Settings.Instance.Token = Client.Get (Settings.Instance.Email, password);
						status = LoginStatus.Success;
						return;
					} catch (WebException e) {
						if (((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.Unauthorized) {
							status = LoginStatus.Unauthorized;
							return;
						}
						
						status = LoginStatus.Other;
					}
				}
			};
		}
		#endregion

		#region SelectOrganization
		private static Action<Organization> SelectOrganization()
		{
			return delegate(Organization organization) {
				Settings.Instance.ApiKey = organization.ApiKey;
				Settings.Instance.BuildSecret = organization.BuildSecret;
				Settings.Instance.FlowSequence = 0;
				Settings.Instance.Organization = organization.Name;
			};
		}
		#endregion

		#region ApplyInstructions
		private static Action ApplyInstructions()
		{
			return delegate() {
				try {
					FabricSetup.EnableCrashlytics (false);

					Settings.Instance.SetupComplete = true;
					Settings.Instance.LastSetupFailed = false;

					activationStatus = ActivationStatus.Unknown;
				} catch (System.Exception) {
					//Todo:
					
					Settings.Instance.SetupComplete = false;
					Settings.Instance.LastSetupFailed = true;
				}
			};
		}
		#endregion

		#region BackToOrganizations
		private static Action BackToOrganizations()
		{
			return delegate() {
				Settings.Instance.ApiKey = null;
				Settings.Instance.BuildSecret = null;
			};
		}
		#endregion

		#region AdvanceToValidationPage
		private static Action AdvanceToValidationPage()
		{
			return delegate() {
				activationStatus = ActivationStatus.Unknown;
				Settings.Instance.FlowSequence = 1;
			};
		}
		#endregion

		#region AdvanceToDashboardPage
		private static Action AdvanceToDashboardPage()
		{
			return delegate() {
				Settings.Instance.FlowSequence = 2;
			};
		}
		#endregion

		#region CrashlyticsInitDragAndDrop
		private static bool CrashlyticsInitAlreadyExists()
		{
			foreach (GameObject o in UnityEngine.Object.FindObjectsOfType (typeof (GameObject))) {
				if (o.name.Equals ("CrashlyticsInit") || o.name.Equals ("CrashlyticsInit(Clone)")) {
					return true;
				}
			}
			
			return false;
		}

		private static GameObject LoadPrefabAt (string location)
		{
			return AssetDatabase.LoadAssetAtPath (location, typeof (GameObject)) as GameObject;
		}
		
		private static bool InstallCrashlyticsInit()
		{
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6
			// This is a workaround for unity 4, since we cannot load unity 5 created prefabs. Going forward,
			// this will be the preferred method for adding Crashlytics.
			GameObject crashlytics = new GameObject ("CrashlyticsInit");
			crashlytics.AddComponent<Crashlytics>();
			return true;
#else
			GameObject prefab = LoadPrefabAt ("Assets/Plugins/CrashlyticsScripts/CrashlyticsInit.prefab");

			if (prefab == null) {
				FabricUtils.Log ("Couldn't load CrashlyticsInit.prefab from Asset database! Please drag it into your first scene manually");
				return false;
			}

			GameObject prefabObj = UnityEngine.Object.Instantiate (prefab) as GameObject;

			if (prefabObj != null) {
				return true;
			}


			FabricUtils.Log ("Couldn't instantiate CrashlyticsInit.prefab, please drag it into your first scene manually.");
			return false;
#endif
		}
		
		private static void CrashlyticsInitAcceptPrefabDrop(int pid, Rect rect)
		{
			if (Event.current.type == EventType.DragUpdated) {
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
			}

			if (Event.current.type != EventType.DragPerform) {
				return;
			}

			bool advance = true;

			if (!CrashlyticsInitAlreadyExists ()) {
				DragAndDrop.AcceptDrag ();
			
				if (!InstallCrashlyticsInit ()) {
					advance = false;
				}
			}

			if (advance) {
				AdvanceToValidationPage ()();
			}
			
			Event.current.Use ();
		}

		private static Action CrashlyticsInitDragAction ()
		{
			return delegate() {
				DragAndDrop.PrepareStartDrag ();
				DragAndDrop.paths = null;
				DragAndDrop.objectReferences = new UnityEngine.Object[0];
				DragAndDrop.SetGenericData ("object", Event.current);
				DragAndDrop.StartDrag ("CrashlyticsInit");
			};
		}
		#endregion
	}
}
