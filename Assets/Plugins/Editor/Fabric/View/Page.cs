using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Fabric
{
	namespace View
	{
		public abstract class Page
		{
			private static readonly GUIStyle HeaderStyle = new GUIStyle ();
			private static readonly GUIStyle HeaderLineStyle = new GUIStyle ();
			private static readonly GUIStyle FooterLineStyle = new GUIStyle ();
			private static readonly GUIStyle RButtonStyle = new GUIStyle (GUI.skin.button);
			private static readonly GUIStyle LButtonStyle = new GUIStyle (GUI.skin.button);

			private static Rect FooterPosition = new Rect ();
			private static Rect LButtonPosition = new Rect ();
			private static Rect RButtonPosition = new Rect ();

			static Page()
			{
				HeaderStyle.normal.textColor = Color.white;
				HeaderStyle.fontSize = 15;
				HeaderStyle.fontStyle = FontStyle.Bold;
				HeaderStyle.margin.left = 20;
				HeaderStyle.margin.top = 20;
				HeaderStyle.margin.bottom = 20;
				HeaderStyle.wordWrap = true;

				HeaderLineStyle.fixedHeight = 1;
				HeaderLineStyle.normal.background = View.Render.MakeBackground (1, 1, new Color32 (255, 255, 255, 76));

				FooterLineStyle.fixedHeight = 1;
				FooterLineStyle.normal.background = View.Render.MakeBackground (1, 1, new Color32 (255, 255, 255, 76));

				LButtonStyle.fixedHeight = 29;
				LButtonStyle.fixedWidth = 52;
				LButtonStyle.margin.left = 20;
				LButtonStyle.margin.top = 25;
				LButtonStyle.margin.bottom = 20;
				
				RButtonStyle.fixedHeight = 29;
				RButtonStyle.fixedWidth = 52;
				RButtonStyle.margin.left = 5;
				RButtonStyle.margin.top = 25;
				RButtonStyle.margin.bottom = 20;
				RButtonStyle.margin.right = 20;
			}

			public static void RenderHeader(string message)
			{
				EditorGUILayout.Space ();
				GUILayout.Label (message, HeaderStyle);
				GUILayout.Label ("", HeaderLineStyle);
				EditorGUILayout.Space ();
			}

			public static void RenderFooter (KeyValuePair<string, Action>? lButton, KeyValuePair<string, Action>? rButton)
			{
				GUI.Label (FooterPosition, "", FooterLineStyle);

				GUILayout.BeginHorizontal ();

				if (lButton.HasValue) {
					RenderButton (lButton.Value.Key, lButton.Value.Value, LButtonStyle, LButtonPosition);
				}
				GUILayout.FlexibleSpace ();

				if (rButton.HasValue) {
					RenderButton (rButton.Value.Key, rButton.Value.Value, RButtonStyle, RButtonPosition);
				}
				
				GUILayout.EndHorizontal ();
			}
			
			private static void RenderButton(string caption, Action onClick, GUIStyle style, Rect position)
			{
				if (GUI.Button (position, caption, style)) {
					onClick ();
				}
			}

			private static void Reposition(Rect position)
			{
				FooterPosition.width = position.width;
				FooterPosition.height = 30;
				FooterPosition.x = 0;
				FooterPosition.y = position.height - (HeaderStyle.margin.top + HeaderStyle.margin.bottom + 15 + 13);

				LButtonPosition.width = 52;
				LButtonPosition.height = 29;
				LButtonPosition.x = 20;
				LButtonPosition.y = position.height - (29 + 20);

				RButtonPosition.width = 52;
				RButtonPosition.height = 29;
				RButtonPosition.x = position.width - (20 + 52);
				RButtonPosition.y = position.height - (29 + 20);
			}

			public void Render(Rect position)
			{
				EditorGUI.DrawRect (new Rect (0, 0, position.width, position.height), View.Render.Lerped);
				Reposition (position);
				RenderImpl (position);
			}

			public abstract void RenderImpl(Rect position);
		}
	}
}