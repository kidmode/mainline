using UnityEngine;
using UnityEditor;
using System;
using Fabric.View;
using Fabric.Model;

namespace Fabric
{
	public class Editor : EditorWindow
	{
		public static Version version = new Version ("0.9.0");

		#region Instance
		private static Editor instance;
		private static Editor Instance
		{
			get {
				if (instance == null)
					instance = GetFabricEditorWindow () as Editor;
				return instance;
			}
		}
		#endregion

		[MenuItem("Fabric/Prepare Crashlytics", false, 0)]
		public static void Init ()
		{
			instance = GetFabricEditorWindow () as Editor;
		}

		[MenuItem("Fabric/Crashlytics/Enable Crashlytics", false, 1)]
		public static void EnableCrashlytics ()
		{
			FabricSetup.EnableCrashlytics (true);
		}

		[MenuItem("Fabric/Crashlytics/Enable Crashlytics", true, 1)]
		public static bool ValidateEnableCrashlytics ()
		{
			return !Settings.Instance.EnableCrashlytics;
		}

		[MenuItem("Fabric/Crashlytics/Disable Crashlytics", false, 1)]
		public static void DisableCrashlytics ()
		{
			FabricSetup.DisableCrashlytics ();
		}

		[MenuItem("Fabric/Crashlytics/Disable Crashlytics", true, 1)]
		public static bool ValidateDisableCrashlytics ()
		{
			return Settings.Instance.EnableCrashlytics;
		}

		public void OnGUI ()
		{
			Controller.PageFromState ().Render (Instance.position);
			Repaint ();
		}

		private static EditorWindow GetFabricEditorWindow ()
		{
			return EditorWindow.GetWindowWithRect(
				typeof (Editor),
				new Rect(100, 100, View.Render.InitialWindowWidth, View.Render.InitialWindowHeight),
				false,
				"Fabric"
			);
		}
	}
}
