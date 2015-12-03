using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

namespace Fabric
{
	namespace View
	{
		public class DashboardPage : Page
		{
			private Func<Texture2D> DownloadIcon;
			private Func<string> DashboardUrl;

			public DashboardPage(Func<Texture2D> downloadIcon, Func<string> dashboardUrl)
			{
				this.DownloadIcon = downloadIcon;
				this.DashboardUrl = dashboardUrl;
			}

			#region Components
			private static class Components
			{
				private static GUIStyle IconStyle = new GUIStyle (GUI.skin.button);
				private static readonly GUIStyle BorderStyle = new GUIStyle (GUI.skin.label);
				private static readonly GUIStyle MessageStyle = new GUIStyle ();
				private static readonly Texture2D placeholder = Resources.Manager.Load ("image.icon.placeholder.png");

				private static Rect IconPosition = new Rect (100, 192, 192, 192);

				static Components()
				{
					IconStyle.margin.top = 250;
					IconStyle.fixedWidth = 192;
					IconStyle.fixedHeight = 192;

					BorderStyle.normal.background = View.Render.MakeBackground (200, 200, View.Render.DBlue);
					BorderStyle.normal.textColor = Color.white;
					BorderStyle.contentOffset = new Vector2 (2, 1);
					BorderStyle.fontStyle = FontStyle.Bold;

					MessageStyle.normal.textColor = Color.white;
					MessageStyle.fontSize = 14;
					MessageStyle.margin.left = 20;
					MessageStyle.wordWrap = true;
				}

				private static Texture2D LoadIcon(Func<Texture2D> downloadIcon)
				{
					Texture2D[] textures;
					Texture2D texture = null;

					if ((textures = PlayerSettings.GetIconsForTargetGroup (BuildTargetGroup.Android)) != null && textures[0] != null) {
						texture = textures[0];
					}

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6
					var buildTargetGroup = BuildTargetGroup.iPhone;
#else
					var buildTargetGroup = BuildTargetGroup.iOS;
#endif
					
					if ((textures = PlayerSettings.GetIconsForTargetGroup (buildTargetGroup)) != null && textures[0] != null) {
						texture = textures[0];
					}

					if (texture == null) {
						texture = downloadIcon ();
					}

					return texture ?? placeholder;
				}

				public static void Reposition(Rect position)
				{
					IconPosition.x = position.width / 2 - IconPosition.width / 2 - 2;
					View.Render.Center (position, IconStyle);
				}

				private static void RenderBorder (int top, GUIStyle style)
				{
					GUI.Label (new Rect (style.margin.left - 6, top, style.fixedWidth + 9, style.fixedHeight + 11), "", BorderStyle);
				}

				public static void RenderMessage (string message)
				{
					EditorGUILayout.Space ();
					GUILayout.Label (message, MessageStyle);
				}

				public static void RenderIcon(Func<Texture2D> downloadIcon, Func<string> dashboardUrl)
				{
					Texture2D background = LoadIcon (downloadIcon);

					IconStyle.normal.background = background;
					IconStyle.hover.background = background;
					IconStyle.active.background = background;

					RenderBorder (187, IconStyle);
					if (GUI.Button (IconPosition, "", IconStyle)) {
						Application.OpenURL (dashboardUrl ());
					}
				}
			}
			#endregion

			public override void RenderImpl(Rect position)
			{
				RenderHeader ("We're all done!");
				Components.Reposition (position);
				Components.RenderMessage ("Click on your app icon to go to the Crashlytics dashboard.");
				Components.RenderIcon (DownloadIcon, DashboardUrl);
			}
		}
	}
}