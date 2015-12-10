using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GAF
{
	[AddComponentMenu("")]
	public class GAFBehaviour : MonoBehaviour
	{
		private Transform _cachedTransform = null;

		public Transform cachedTransform
		{
			get
			{
				if (!_cachedTransform)
				{
					_cachedTransform = base.transform;
				}

				return _cachedTransform;
			}
		}
	}
}
