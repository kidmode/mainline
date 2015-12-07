using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.IO;
using Fabric.Model;
using UnityEngine;
using System;

namespace Fabric
{
	namespace API
	{
		public class V1
		{
			private static readonly string OrganizationsEndpoint = "/api/v2/organizations";
			private static readonly string AppsEndpointAndroid = "/platforms/android/apps?include=sdk_kits";
			private static readonly string AppsEndpointIOS = "/platforms/ios/apps?include=sdk_kits";

			private Net.OAuth.Client client;
			private Net.OAuth.Client.Token token;
			private string URI;

			public class UnauthorizedException : System.Exception
			{
			}

			public class ApiException : System.Exception
			{
			}

			public V1(string URI, Net.OAuth.Client client, Net.OAuth.Client.Token token)
			{
				this.URI = URI;
				this.client = client;
				this.token = token;
			}

			public List<Organization> Organizations()
			{
				HttpWebResponse response = HttpGet (
					URI + OrganizationsEndpoint, client, token, 2u
				);

				using (Stream stream = response.GetResponseStream ()) {
					return Parser.ParseOrganizations (stream);
				}
			}

			public List<App> ApplicationsFor(string organizationName, List<Organization> organizations)
			{
				if (organizations == null) {
					FabricUtils.Error ("No organizations exist!");
					return null;
				}

				Organization matching = organizations.Find (l => l.Name.Equals (organizationName));

				if (matching == null) {
					FabricUtils.Error ("Nothing matches {0}", organizationName);
					return null;
				}

				HttpWebResponse response = HttpGet (
					URI + OrganizationsEndpoint + "/" + matching.Id + AppsEndpointAndroid, client, token, 2u
				);

				List<App> apps = new List<App> ();

				using (Stream stream = response.GetResponseStream ()) {
					apps.AddRange (Parser.ParseApps (stream));
				}

				response = HttpGet (
					URI + OrganizationsEndpoint + "/" + matching.Id + AppsEndpointIOS, client, token, 2u
				);

				using (Stream stream = response.GetResponseStream ()) {
					apps.AddRange (Parser.ParseApps (stream));
				}

				return apps;
			}

			// Downloading app icons doesn't require OAuth signing.
			public static IEnumerator DownloadTexture(string URI, Action<Texture2D> onComplete)
			{
				WWW www = new WWW (URI);

				while (!www.isDone) {
					yield return null;
				}

				onComplete (String.IsNullOrEmpty (www.error) ? www.texture : null);
			}

			private static HttpWebResponse HttpGet(string URI, Net.OAuth.Client client, Net.OAuth.Client.Token token, uint tries)
			{
				HttpWebRequest request = null;

				try {
					request = (HttpWebRequest)System.Net.WebRequest.Create (URI);
					
					request.Method = "GET";
					request.ContentType = "application/x-www-form-urlencoded";
					request.Accept = "application/json";

					Net.OAuth.Client.Sign (request, token);

					return (HttpWebResponse)request.GetResponse ();
				} catch (WebException e) {
					if (((HttpWebResponse)e.Response).StatusCode == HttpStatusCode.Unauthorized) {
						throw new UnauthorizedException();
					}

					throw new ApiException();
				}
			}
		}
	}
}