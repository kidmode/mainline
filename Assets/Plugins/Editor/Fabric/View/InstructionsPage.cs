using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Fabric
{
	namespace View
	{
		public class InstructionsPage : Page
		{
			private KeyValuePair<string, Action> apply;
			private KeyValuePair<string, Action> back;

			public InstructionsPage(Action onApplyClicked, Action onBackClicked)
			{
				this.apply = new KeyValuePair<string, Action>("Apply", onApplyClicked);
				this.back = new KeyValuePair<string, Action>("Back", onBackClicked);
			}

			#region Components
			private static class Components
			{
				private static readonly GUIStyle InstructionsStyle = new GUIStyle ();

				static Components()
				{
					InstructionsStyle.normal.textColor = Color.white;
					InstructionsStyle.fontSize = 14;
					InstructionsStyle.margin.top = 18;
					InstructionsStyle.margin.bottom = 1;
					InstructionsStyle.margin.left = 40;
					InstructionsStyle.wordWrap = true;
				}

				public static void RenderInstructions()
				{
					GUILayout.Label ("◈ Set Crashlytics script execution order to -100", InstructionsStyle);
					
					EditorGUILayout.Space ();
					GUILayout.Label ("◈ Inject API key/secret in AndroidManifest", InstructionsStyle);
					
					EditorGUILayout.Space ();
					GUILayout.Label ("◈ Inject the Crashlytics application in AndroidManifest", InstructionsStyle);
					
					EditorGUILayout.Space ();
				}
			}
			#endregion

			public override void RenderImpl(Rect position)
			{
				RenderHeader ("Fabric will make the following changes:");
				RenderFooter (back, apply);
				Components.RenderInstructions ();
			}
		}
	}
}