﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GAF
{
	[System.Serializable]
	[AddComponentMenu("")]
	#if UNITY_4_5
	[DisallowMultipleComponent]
	#endif
	public class GAFMaskObject : GAFObject
	{
		#region Members

		[HideInInspector][SerializeField] private Texture2D m_MaskTexture = null;

		#endregion // Members

		#region Interface

		public override Texture2D getTexture()
		{
			return m_MaskTexture;
		}

		protected override void resetBaseData()
		{
			base.resetBaseData();

			initTexture();
		}

		public override void reload(GAFRenderProcessor _Processor)
		{
			setCurrentState(new GAFObjectStateData((uint)getID()));

			resetBaseData();

			updateToState(getCurrentState(), _Processor, true);
		}

		public override void updateToState(GAFObjectStateData _State, GAFRenderProcessor _Processor, bool _Refresh)
		{
			var movieClip = getMovieClip();
			setCurrentState(_State);

			if (movieClip != null)
			{
				float scale	= movieClip.settings.pixelsPerUnit / movieClip.settings.scale;
				setStatePosition(new Vector3((_State.tX) / scale, (-_State.tY) / scale, _State.zOrder));
				transform.localPosition = getStatePosition() + (Vector3)getPositionOffset();
			}
		}

		#endregion // Interface

		#region Properties

		#endregion // Properties

		#region Implementation

		protected override void initMeshData()
		{
			// Empty
		}

		protected override void resetRenderer()
		{
			// Empty
		}

		private void initTexture()
		{
			GAFAtlasElementData element = getAtlasElementData();
			GAFTexturesData info = getTexturesData();

			var movieClip = getMovieClip();

			int csf = (int)movieClip.settings.csf;

			m_MaskTexture = new Texture2D(
				  (int)(element.width * csf)
				, (int)(element.height * csf)
				, TextureFormat.ARGB32
				, false);

			Color[] textureColor = getTexture().GetPixels();
			for (uint i = 0; i < textureColor.Length; ++i)
				textureColor[i] = Color.black;

			m_MaskTexture.SetPixels(textureColor);
			m_MaskTexture.Apply();

			Texture2D atlasTexture = movieClip.resource.getTexture(System.IO.Path.GetFileNameWithoutExtension(info.getFileName(csf)));
			Color[] maskTexturePixels = atlasTexture.GetPixels(
				  (int)(element.x * csf)
				, (int)(atlasTexture.height - element.y * csf - element.height * csf)
				, (int)(element.width * csf)
				, (int)(element.height * csf));

			m_MaskTexture.SetPixels(
				  0
				, 0
				, (int)(element.width * csf)
				, (int)(element.height * csf)
				, maskTexturePixels);

			m_MaskTexture.Apply(true);

			m_MaskTexture.filterMode = FilterMode.Bilinear;
			m_MaskTexture.wrapMode = TextureWrapMode.Clamp;

			m_MaskTexture.Apply();
		}

		#endregion // Implementation
	}
}