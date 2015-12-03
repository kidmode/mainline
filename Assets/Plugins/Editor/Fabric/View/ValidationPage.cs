using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System;

namespace Fabric 
{
	namespace View
	{
		public class ValidationPage : Page
		{
			private static readonly Texture2D Rocket = Fabric.Resources.Manager.Load ("image.rocket.png");
			private static readonly Texture2D[] Spinner = {
				Fabric.Resources.Manager.Load ("spinner_0.gif"),
				Fabric.Resources.Manager.Load ("spinner_1.gif"),
				Fabric.Resources.Manager.Load ("spinner_2.gif"),
				Fabric.Resources.Manager.Load ("spinner_3.gif"),
				Fabric.Resources.Manager.Load ("spinner_4.gif"),
				Fabric.Resources.Manager.Load ("spinner_5.gif"),
				Fabric.Resources.Manager.Load ("spinner_6.gif"),
				Fabric.Resources.Manager.Load ("spinner_7.gif"),
				Fabric.Resources.Manager.Load ("spinner_8.gif"),
				Fabric.Resources.Manager.Load ("spinner_9.gif"),
				Fabric.Resources.Manager.Load ("spinner_10.gif"),
				Fabric.Resources.Manager.Load ("spinner_11.gif")
			};
			private static Rect SpinnerPos = new Rect (150, 150, 28, 28);

			private static readonly View.Animation.Driver Driver = new View.Animation.Driver ((uint)Spinner.Length);

			private Func<Controller.ActivationStatus> Validate;
			private Action Reset;

			#region Components
			private static class Components
			{
				private static readonly GUIStyle StatusStyle = new GUIStyle ();
				private static GUIStyle SpinnerStyle = new GUIStyle ();
				private static GUIStyle RocketStyle = new GUIStyle ();
				private static GUIStyle ErrorStyle = new GUIStyle (GUI.skin.label);

				private static Color ErrorColor = View.Render.FromHex (0xF39C12);
				
				static Components()
				{
					StatusStyle.normal.textColor = Color.white;
					StatusStyle.fontSize = 14;
					StatusStyle.margin.left = 20;
					StatusStyle.wordWrap = true;

					RocketStyle.fixedWidth = 277;
					RocketStyle.margin.top = 70;

					ErrorStyle.margin.top = 20;
					ErrorStyle.normal.textColor = ErrorColor;
					ErrorStyle.fontSize = 14;
					ErrorStyle.fixedWidth = 195;
				}

				public static void RenderMessage(string message)
				{
					EditorGUILayout.Space ();
					GUILayout.Label (message, StatusStyle);
				}

				public static void RenderSpinnerMessage(string message, bool isError)
				{
					EditorGUILayout.Space ();

					ErrorStyle.normal.textColor = isError ? ErrorColor : Color.white;

					if (GUILayout.Button (message, ErrorStyle)) {
						Application.OpenURL ("mailto:support@fabric.io");
					}
				}

				public static void RenderRocket()
				{
					EditorGUILayout.Space ();
					GUILayout.Label (Rocket, RocketStyle);
				}

				public static void RenderSpinner(bool visible)
				{
					Texture2D frame = visible ? Spinner [Driver.Frame] : null;
					GUI.Label (SpinnerPos, frame, SpinnerStyle);
				}

				public static void Reposition(Rect position)
				{
					View.Render.Center (position, RocketStyle);
					View.Render.Center (position, ErrorStyle);

					SpinnerPos.x = ErrorStyle.margin.left - 28 - 4;
					SpinnerPos.y = 400;
				}
			}
			#endregion

			public ValidationPage(Func<Controller.ActivationStatus> validate, Action reset)
			{
				this.Validate = validate;
				this.Reset = reset;
			}

			public override void RenderImpl(Rect position)
			{
				RenderHeader ("Verifying...");
				Components.Reposition (position);
				Components.RenderMessage ("Your setup is complete!");
				Components.RenderMessage ("To finish the onboarding process, build and launch your app.");
				Components.RenderRocket ();

				bool visible = false;
				bool isError = false;
				string message = "Waiting for an app launch...";

				switch (Validate ()) {
				case Controller.ActivationStatus.TimedOut:
					isError = true;
					message = "It's been a while. Need some help?\nsupport@fabric.io";
					visible = false;
					break;
				case Controller.ActivationStatus.Checking:
					visible = true;
					break;
				}

				// We must always render these components, given the current architecture. Adding and
				// removing components needs to be done in Update().
				Components.RenderSpinner (visible);
				Components.RenderSpinnerMessage (message, isError);

				KeyValuePair<string, Action>? button = null;
				if (!visible) {
					button = new KeyValuePair<string, Action> ("Retry!", delegate() {
						Reset ();
					});
				}

				RenderFooter (null, button);
			}
		}
	}
}