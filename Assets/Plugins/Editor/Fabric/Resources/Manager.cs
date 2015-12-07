using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fabric
{
	namespace Resources
	{
		public class Manager
		{
			private static readonly string root = "Assets/Plugins/Editor/Fabric/Resources/";

			public static Texture2D Load(string resource)
			{
				return AssetDatabase.LoadAssetAtPath(root + resource, typeof(Texture2D)) as Texture2D;
			}
		}
	}
}