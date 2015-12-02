using UnityEngine;
using System.Collections;

namespace Fabric
{
	namespace Model
	{
		public sealed class Organization
		{
			public string Name;
			public string Id;
			public string ApiKey;
			public string BuildSecret;

			public Organization(string name, string id, string apiKey, string buildSecret)
			{
				Name = name;
				Id = id;
				ApiKey = apiKey;
				BuildSecret = buildSecret;
			}
		}
	}
}