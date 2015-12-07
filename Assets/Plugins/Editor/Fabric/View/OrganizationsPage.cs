using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Fabric.Model;
using System;

namespace Fabric
{
	namespace View
	{
		public class OrganizationsPage : Page
		{	
			private Action<Organization> onOrganizationSelected;
			private Func<List<Organization>> fetchOrganizations;
			
			public OrganizationsPage(Action<Organization> onOrganizationSelected, Func<List<Organization>> fetchOrganizations)
			{
				this.onOrganizationSelected = onOrganizationSelected;
				this.fetchOrganizations = fetchOrganizations;
			}

			#region Components
			private static class Components
			{
				private static readonly GUIStyle RowStyle = new GUIStyle (GUI.skin.button);
				private static readonly GUIStyle ScrollStyle = new GUIStyle (GUI.skin.scrollView);

				private static readonly Color32 SelectedColor = View.Render.LBlue;
				private static readonly Color32 RowColor = View.Render.FromHex (0x2B6591);

				private static Vector2 scroll;

				private static readonly int padding = 16;

				static Components()
				{
					RowStyle.padding = new RectOffset (padding, padding, padding, padding);
					RowStyle.alignment = TextAnchor.MiddleLeft;
					RowStyle.fontSize = 14;
					RowStyle.normal.textColor = Color.white;

					int rowHeight = RowStyle.normal.background.height;
					int rowWidth = RowStyle.normal.background.width;

					RowStyle.normal.background = View.Render.MakeBackground (rowWidth, rowHeight, RowColor);
					RowStyle.hover.background = View.Render.MakeBackground (rowWidth, rowHeight, SelectedColor);

					ScrollStyle.margin.top = 18;
					ScrollStyle.margin.bottom = 0;
					ScrollStyle.margin.left = 18;
					ScrollStyle.margin.right = 16;
				}

				public static void RenderOrganizationsList(ICollection<Organization> organizations, Action<Organization> onSelected)
				{
					if (organizations == null)
						return;

					scroll = GUILayout.BeginScrollView (scroll, ScrollStyle);

					for (int i = 0; i < 1; ++i) {
						foreach (Organization organization in organizations) {
							if (GUILayout.Button (organization.Name, RowStyle)) {
								onSelected (organization);
							}
						}
					}

					GUILayout.EndScrollView ();
				}
			}
			#endregion

			public override void RenderImpl(Rect position)
			{
				RenderHeader ("Please select your organization");
				Components.RenderOrganizationsList (fetchOrganizations (), onOrganizationSelected);
			}
		}
	}
}