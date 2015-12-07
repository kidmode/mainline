using System.Collections.Generic; 
using System.Collections;
using System.IO;
using Fabric.Model;

namespace Fabric
{
	namespace API
	{
		public class Parser
		{
			public static List<Organization> ParseOrganizations(Stream stream)
			{
				List<Organization> organizations = new List<Organization> ();
				
				using (StreamReader reader = new StreamReader (stream)) {
					string json = reader.ReadToEnd ();

					int nameIndex = 0;
					int idIndex = 0;
					int apiKeyIndex = 0;
					int buildSecretIndex = 0;
					
					while ((nameIndex = json.IndexOf ("name\":\"", nameIndex)) != -1 &&
					       (idIndex = json.IndexOf ("id\":\"", idIndex)) != -1 &&
					       (apiKeyIndex = json.IndexOf ("api_key\":\"", apiKeyIndex)) != -1 &&
					       (buildSecretIndex = json.IndexOf ("build_secret\":\"", buildSecretIndex)) != -1) {
						nameIndex += "name\":\"".Length;
						idIndex += "id\":\"".Length;
						apiKeyIndex += "api_key\":\"".Length;
						buildSecretIndex += "build_secret\":\"".Length;
						
						string name = json.Substring (nameIndex, json.IndexOf ("\"", nameIndex + 1) - nameIndex);
						string id = json.Substring (idIndex, json.IndexOf ("\"", idIndex + 1) - idIndex);
						string apiKey = json.Substring (apiKeyIndex, json.IndexOf ("\"", apiKeyIndex + 1) - apiKeyIndex);
						string secret = json.Substring (buildSecretIndex, json.IndexOf ("\"", buildSecretIndex + 1) - buildSecretIndex);

						organizations.Add (new Organization (name, id, apiKey, secret));
					}
				}
				
				return organizations;
			}

			public static List<App> ParseApps(Stream stream)
			{
				List<App> apps = new List<App> ();

				using (StreamReader reader = new StreamReader (stream)) {
					string json = reader.ReadToEnd ();

					int nameIndex = 0;
					int bundleIdentifierIndex = 0;
					int iconUrlIndex = 0;
					int dashboardUrlIndex = 0;

					while ((nameIndex = json.IndexOf ("name\":\"", nameIndex)) != -1 &&
					       (bundleIdentifierIndex = json.IndexOf ("bundle_identifier\":\"", bundleIdentifierIndex)) != -1 &&
					       (iconUrlIndex = json.IndexOf ("icon128_url\":\"", iconUrlIndex)) != -1 &&
					       (dashboardUrlIndex = json.IndexOf ("dashboard_url\":\"", dashboardUrlIndex)) != -1) {
						nameIndex += "name\":\"".Length;
						bundleIdentifierIndex += "bundle_identifier\":\"".Length;
						iconUrlIndex += "icon128_url\":\"".Length;
						dashboardUrlIndex += "dashboard_url\":\"".Length;
						
						string name = json.Substring (nameIndex, json.IndexOf ("\"", nameIndex + 1) - nameIndex);
						string bundleIdentifier = json.Substring (bundleIdentifierIndex, json.IndexOf ("\"", bundleIdentifierIndex + 1) - bundleIdentifierIndex);
						string iconUrl = json.Substring (iconUrlIndex, json.IndexOf ("\"", iconUrlIndex + 1) - iconUrlIndex);
						string dashboardUrl = json.Substring (dashboardUrlIndex, json.IndexOf ("\"", dashboardUrlIndex + 1) - dashboardUrlIndex);

						apps.Add (new App (name, bundleIdentifier, iconUrl, dashboardUrl));
					}
				}

				return apps;
			}
		}
	}
}