using UnityEngine;
using UnityEditor;
using Fabric.Model;
using System;

namespace Fabric
{
	namespace View
	{
		public class LoginPage : Page
		{
			private string password;

			private Controller.LoginAction<string, Controller.LoginStatus> onLoginButtonClick;
			private Controller.LoginStatus status = Controller.LoginStatus.Unknown;

			public LoginPage(Controller.LoginAction<string, Controller.LoginStatus> onLoginButtonClick)
			{
				this.onLoginButtonClick = onLoginButtonClick;
			}

			#region Components
			private static class Components
			{
				private static GUIStyle LogoStyle = new GUIStyle ();
				private static GUIStyle LogoTextStyle = new GUIStyle ();
				private static GUIStyle LogoSloganStyle = new GUIStyle ();
				
				private static GUIStyle FieldStyle = new GUIStyle (GUI.skin.textField);

				private static readonly GUIStyle BorderStyle = new GUIStyle (GUI.skin.label);
				private static readonly GUIStyle PowerButtonStyle = new GUIStyle (GUI.skin.button);

				private static readonly GUIStyle ErrorStyle = new GUIStyle ();
				
				private static readonly Color32 BorderColor = new Color32 (255, 255, 255, 76);
				private static readonly Color32 ErrorColor = View.Render.FromHex (0xF39C12);
				
				private static readonly Texture2D Logo = Fabric.Resources.Manager.Load ("fabric-icon.png");
				private static readonly Texture2D LogoText = Fabric.Resources.Manager.Load ("image.logo-text@2x.png");
				private static readonly Texture2D LogoSlogan = Fabric.Resources.Manager.Load ("image.power.on.words.png");
				private static readonly Texture2D PowerButton = Fabric.Resources.Manager.Load ("control.button.power.inactive@2x.png");
				private static readonly Texture2D PowerButtonHover = Fabric.Resources.Manager.Load ("control.button.power.hover@2x.png");
				private static readonly Texture2D PowerButtonClicked = Fabric.Resources.Manager.Load ("control.button.power.active@2x.png");

				private static readonly int topMargin = 20;

				static Components()
				{
					ErrorStyle.normal.textColor = ErrorColor;
					
					LogoStyle.fixedHeight = 99;
					LogoStyle.fixedWidth = 95;
					LogoStyle.margin.top = 20 + topMargin;
					
					LogoTextStyle.fixedHeight = 52;
					LogoTextStyle.fixedWidth = 163;
					LogoTextStyle.margin.top = 5;
					
					LogoSloganStyle.fixedHeight = 11;
					LogoSloganStyle.fixedWidth = 203;
					LogoSloganStyle.margin.top = 5;
					LogoSloganStyle.margin.bottom = 7;
					
					PowerButtonStyle.fixedWidth = 75;
					PowerButtonStyle.fixedHeight = 72;
					PowerButtonStyle.normal.background = PowerButton;
					PowerButtonStyle.hover.background = PowerButtonHover;
					PowerButtonStyle.active.background = PowerButtonClicked;
					
					FieldStyle.fixedHeight = 33;
					FieldStyle.fixedWidth = 280;
					FieldStyle.alignment = TextAnchor.MiddleLeft;

					FieldStyle.contentOffset = new Vector2 (0, 2);
					FieldStyle.fontSize = 15;

					int fieldWidth = (int)FieldStyle.fixedWidth;
					int fieldHeight = (int)FieldStyle.fixedHeight;

					FieldStyle.focused.textColor = Color.black;
					FieldStyle.wordWrap = false;
					FieldStyle.margin.bottom = 46;
					FieldStyle.margin.top = 10 + 17;
					FieldStyle.normal.background = View.Render.MakeBackground (fieldWidth, fieldHeight, View.Render.LBlue);
					FieldStyle.active.background = View.Render.MakeBackground (fieldWidth, fieldHeight, View.Render.LBlue);
					FieldStyle.focused.background = View.Render.MakeBackground (fieldWidth, fieldHeight, View.Render.LBlue);
					FieldStyle.active.textColor = Color.white;
					FieldStyle.normal.textColor = Color.white;
					FieldStyle.focused.textColor = Color.white;

					BorderStyle.normal.background = View.Render.MakeBackground (290, 45, BorderColor);
					BorderStyle.normal.textColor = Color.white;
					BorderStyle.contentOffset = new Vector2 (2, 1);
					BorderStyle.fontStyle = FontStyle.Bold;
				}

				private static void RenderBorder(string caption, int top, GUIStyle style)
				{
					GUI.Label (new Rect (style.margin.left - 3, top, style.fixedWidth + 4, style.fixedHeight + 21), caption, BorderStyle);
				}

				public static void RenderEmailField(string bound, Action<string> set, Action onChange)
				{
					GUILayout.BeginHorizontal ();
					// Borders need to be absolutely positioned.
					RenderBorder ("Email", 237 + topMargin, FieldStyle);

					if (Event.current.type == EventType.KeyDown) {
						onChange ();
					}

					Color old = GUI.skin.settings.cursorColor;

					GUI.skin.settings.cursorColor = Color.white;
					set (EditorGUILayout.TextField (bound ?? "", FieldStyle));
					GUI.skin.settings.cursorColor = old;

					GUILayout.EndHorizontal ();
				}
				
				public static void RenderPasswordField(string bound, Action<string> set, Action onChange)
				{
					GUILayout.BeginHorizontal ();
					// Borders need to be absolutely positioned.
					RenderBorder ("Password", 299 + topMargin, FieldStyle);

					bound = bound == null ? "" : bound;

					Color old = GUI.skin.settings.cursorColor;
					
					GUI.skin.settings.cursorColor = Color.white;
					set (EditorGUILayout.PasswordField ("", bound, FieldStyle));
					GUI.skin.settings.cursorColor = old;

					if (Event.current.type == EventType.KeyDown) {
						onChange ();
					}

					GUILayout.EndHorizontal ();
					EditorGUILayout.Space ();
				}
				
				public static void RenderLoginButton(Action onClick)
				{
					GUILayout.BeginHorizontal ();
					GUILayout.FlexibleSpace ();
					EditorGUILayout.LabelField("", GUILayout.Width (0));
					
					if (GUILayout.Button ("", PowerButtonStyle)) {
						onClick();
					}
					
					GUILayout.FlexibleSpace ();
					GUILayout.EndHorizontal ();
				}

				public static void RenderLogo()
				{
					GUILayout.Label (Logo, LogoStyle);
					EditorGUILayout.Space ();
					
					GUILayout.Label (LogoText, LogoTextStyle);
					EditorGUILayout.Space ();
					
					GUILayout.Label (LogoSlogan, LogoSloganStyle);
					EditorGUILayout.Space ();
				}

				public static void RenderLoginStatus(Controller.LoginStatus status)
				{
					string message = "";

					switch (status) {
					case Controller.LoginStatus.Unauthorized:
						message = "Incorrect credentials";
						break;
					case Controller.LoginStatus.Other:
						message = "Network error";
						break;
					}

					GUILayout.Label (message, ErrorStyle);
				}

				public static void Reposition(Rect position)
				{
					View.Render.Center (position, LogoStyle);
					View.Render.Center (position, LogoTextStyle);
					View.Render.Center (position, LogoSloganStyle);
					View.Render.Center (position, FieldStyle);

					ErrorStyle.margin.left = FieldStyle.margin.left;
				}
			}
			#endregion

			public override void RenderImpl(Rect position)
			{
				Components.Reposition (position);
				Components.RenderLogo ();
				Components.RenderLoginStatus (status);
				Components.RenderEmailField (Settings.Instance.Email, value => Settings.Instance.Email = value, () => status = Controller.LoginStatus.Unknown);
				Components.RenderPasswordField (password, value => password = value, () => status = Controller.LoginStatus.Unknown);
				Components.RenderLoginButton (delegate() { onLoginButtonClick (password, out status); });
			}
		}
	}
}