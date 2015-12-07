using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Fabric
{
	namespace View
	{
		public class PrefabPage : Page
		{
			private KeyValuePair<string, Action> next;
			private Action crashlyticsInitDragAction;

			public PrefabPage(
				Action onNextClicked,
				Action crashlyticsInitDragAction,
				EditorApplication.HierarchyWindowItemCallback acceptPrefabDrop
			)
			{
				this.next = new KeyValuePair<string, Action> ("Next", onNextClicked);
				this.crashlyticsInitDragAction = crashlyticsInitDragAction;

				EditorApplication.hierarchyWindowItemOnGUI += acceptPrefabDrop;
			}

			#region Components
			private static class Components
			{
				private static GUIStyle IconStyle = new GUIStyle (GUI.skin.button);
				private static readonly GUIStyle BorderStyle = new GUIStyle (GUI.skin.label);
				private static readonly GUIStyle MessageStyle = new GUIStyle ();
				private static readonly Texture2D PrefabIcon = Resources.Manager.Load ("prefab-box.png");
				
				private static bool pressed = false;
				private static Rect PrefabIconPosition = new Rect (0, 150, 192, 192);
				
				static Components()
				{
					IconStyle.fixedWidth = 192;
					IconStyle.fixedHeight = 192;
					IconStyle.normal.background = null;
					IconStyle.hover.background = View.Render.MakeBackground (192, 192, View.Render.DBlue);
					
					BorderStyle.normal.background = View.Render.MakeBackground (200, 200, View.Render.DBlue);
					BorderStyle.normal.textColor = Color.white;
					BorderStyle.contentOffset = new Vector2 (2, 1);
					BorderStyle.fontStyle = FontStyle.Bold;
					
					MessageStyle.normal.textColor = Color.white;
					MessageStyle.fontSize = 14;
					MessageStyle.margin.left = 20;
					MessageStyle.wordWrap = true;
				}
				
				public static void Reposition(Rect position)
				{
					PrefabIconPosition.x = position.width / 2 - PrefabIconPosition.width / 2;
					View.Render.Center (position, IconStyle);
				}
				
				public static void RenderMessage (string message)
				{
					EditorGUILayout.Space ();
					GUILayout.Label (message, MessageStyle);
				}
				
				public static void RenderIcon()
				{
					GUI.Label (PrefabIconPosition, PrefabIcon, IconStyle);
				}

				public static void PrepareDragAndDrop(Action crashlyticsInitDragAction)
				{
					if (PrefabIconPosition.Contains (Event.current.mousePosition)) {
						if (Event.current.type == EventType.mouseDown) {
							pressed = true;
						}
						if (Event.current.type == EventType.mouseUp) {
							pressed = false;
						}
					}

					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

					Event current = Event.current;
					if (pressed && current.type == EventType.MouseDrag) {
						crashlyticsInitDragAction ();
						current.Use ();
					}
				}
			}
			#endregion

			public override void RenderImpl(Rect position)
			{
				RenderHeader ("You're almost there...");
				RenderFooter (null, next);
				Components.Reposition (position);
				Components.RenderMessage ("1) Open your <b>first</b> scene");
				Components.RenderMessage ("2) Drag and drop the prefab into the hierarchy");
				Components.RenderIcon ();
				Components.PrepareDragAndDrop (crashlyticsInitDragAction);
			}
		}
	}
}
