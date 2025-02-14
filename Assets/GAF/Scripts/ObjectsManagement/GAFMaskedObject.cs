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
	[ExecuteInEditMode]
	[AddComponentMenu("")]
	#if UNITY_4_5
	[DisallowMultipleComponent]
	#endif
	public class GAFMaskedObject : GAFObject
	{
		#region Members

		private IGAFObject m_Mask = null;

		#endregion Members

		#region Interface

		public override void updateToState(GAFObjectStateData _State, GAFRenderProcessor _Processor, bool _Refresh)
		{
			base.updateToState(_State, _Processor, _Refresh);

			m_Mask = _State.maskID > 0 ? getManager().objectsDict[(int)_State.maskID] : null;
		}

		#endregion // Interface

		#region Implementation

		protected override void updateColor(GAFObjectStateData _State, bool _Refresh)
		{
			var currentState = getCurrentState();

			if (_Refresh ||
				currentState.alpha != _State.alpha)
			{
				if (renderer.sharedMaterial != null)
					renderer.sharedMaterial.SetFloat("_Alpha", _State.alpha);

				currentState.alpha = _State.alpha;
				setCurrentState(currentState);
			}

			if (_Refresh ||
				currentState.colorMatrix != _State.colorMatrix)
			{
				if (renderer.sharedMaterial != null)
				{
					Color mult		= Color.white;
					Color shift		= new Color(0,0,0,0);
					if (_State.colorMatrix != null)
					{
						mult	= new Color(_State.colorMatrix.multipliers.r / 255f, _State.colorMatrix.multipliers.g / 255f, _State.colorMatrix.multipliers.b / 255f);
						shift	= new Color(_State.colorMatrix.offsets.x / 255f, _State.colorMatrix.offsets.y / 255f, _State.colorMatrix.offsets.z / 255f, _State.colorMatrix.offsets.w / 255f);
					}

					renderer.sharedMaterial.SetColor("_ColorMult", mult);
					renderer.sharedMaterial.SetColor("_ColorShift", shift);
				}

				currentState.colorMatrix = _State.colorMatrix;
				setCurrentState(currentState);
			}
		}

		private void applyMask()
		{
			var movieClip = getMovieClip();
			Matrix4x4 maskTransform = Matrix4x4.identity;

			maskTransform.m00 = m_Mask.getCurrentState().a;
			maskTransform.m01 = m_Mask.getCurrentState().c;
			maskTransform.m10 = m_Mask.getCurrentState().b;
			maskTransform.m11 = m_Mask.getCurrentState().d;

#if GAF_USING_TK2D
			float screenHeight 		= 0;
			float screenWidth  		= 0;
			Vector2 cameraPosShift	= Vector2.zero;
		
			tk2dCamera tk2d_camera = Camera.current.GetComponent<tk2dCamera>();
			if (tk2d_camera != null)
			{
				tk2dCameraSettings cameraSettings = tk2d_camera.CameraSettings;
				if (cameraSettings.orthographicType == tk2dCameraSettings.OrthographicType.PixelsPerMeter)
					screenHeight = tk2d_camera.nativeResolutionHeight / cameraSettings.orthographicPixelsPerMeter;
				else
					screenHeight = tk2d_camera.CameraSettings.orthographicSize * 2;

				screenWidth  	= Camera.current.aspect * screenHeight;
				cameraPosShift	= Camera.current.transform.position - new Vector3(screenWidth / 2f, -screenHeight / 2f);
			}
			else
			{
				screenHeight 	= Camera.current.orthographicSize * 2;
				screenWidth  	= Camera.current.aspect * screenHeight;
				cameraPosShift	= Camera.current.transform.position - new Vector3(screenWidth / 2f, -screenHeight / 2f);
			}
#else
			float screenHeight = Camera.current.orthographicSize * 2;
			float screenWidth = Camera.current.aspect * screenHeight;
			Vector2 cameraPosShift = Camera.current.transform.position - new Vector3(screenWidth / 2f, -screenHeight / 2f);
#endif // GAF_USING_TK2D

			float scaleX = Mathf.Sqrt((maskTransform.m00 * maskTransform.m00) + (maskTransform.m01 * maskTransform.m01));
			float scaleY = Mathf.Sqrt((maskTransform.m11 * maskTransform.m11) + (maskTransform.m10 * maskTransform.m10));

			float scale = movieClip.settings.pixelsPerUnit * m_Mask.getAtlasElementData().scale * movieClip.settings.csf;
			float sizeXUV = (float)screenWidth / (m_Mask.getTexture().width / scale * scaleX * transform.parent.localScale.x * Camera.current.aspect);
			float sizeYUV = (float)screenHeight / (m_Mask.getTexture().height / scale * scaleY * transform.parent.localScale.y);

			float maskWidth = (float)m_Mask.getTexture().width / movieClip.settings.csf;
			float maskHeight = (float)m_Mask.getTexture().height / movieClip.settings.csf;

			float pivotX = m_Mask.getAtlasElementData().pivotX / maskWidth;
			float pivotY = (maskHeight - m_Mask.getAtlasElementData().pivotY) / maskHeight;

			float moveX = (-m_Mask.getLocalPosition().x - getManager().transform.position.x + cameraPosShift.x) / screenWidth;
			float moveY = -1f - (m_Mask.getLocalPosition().y + getManager().transform.position.y - cameraPosShift.y) / screenHeight;

			Matrix4x4 _transform = Matrix4x4.identity;
			_transform *= Matrix4x4.TRS(new Vector3(pivotX, pivotY, 0f), Quaternion.identity, Vector3.one);
			_transform *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(sizeXUV, sizeYUV, 1f));
			_transform *= Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -transform.parent.localRotation.eulerAngles.z), Vector3.one);
			_transform *= maskTransform;
			_transform *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1f / scaleX, 1f / scaleY, 1f));
			_transform *= Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Camera.current.aspect, 1f, 1f));
			_transform *= Matrix4x4.TRS(new Vector3(moveX, moveY, 0f), Quaternion.identity, Vector3.one);

			renderer.sharedMaterial.SetMatrix("_TransformMatrix", _transform);
			renderer.sharedMaterial.SetTexture("_MaskMap", m_Mask.getTexture());
		}

		#endregion // Implementation

		#region MonoBehavior

		private void OnWillRenderObject()
		{
			var movieClip = getMovieClip();
			if (movieClip != null &&
				movieClip.asset != null &&
				movieClip.asset.isLoaded &&
				movieClip.resource != null &&
				movieClip.resource.isReady &&
				transform.parent != null &&
				renderer != null &&
				renderer.sharedMaterial != null)
			{
				if (m_Mask != null &&
					m_Mask.getCurrentState() != null &&
					m_Mask.getCurrentState().alpha > 0 &&
					m_Mask.getTexture() != null &&
					m_Mask.getAtlasElementData() != null)
				{
					applyMask();
				}
				else
				{
					renderer.sharedMaterial.SetTexture("_MaskMap", null);
				}
			}
		}

		#endregion // MonoBehaviur
	}
}