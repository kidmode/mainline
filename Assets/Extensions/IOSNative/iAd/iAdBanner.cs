////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

 

using UnityEngine;
using System;
using UnionAssets.FLE;
using System.Collections;
using System.Collections.Generic;
#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class iAdBanner : EventDispatcherBase {

	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _IADCreateBannerAd(int gravity,  int id);
	
	[DllImport ("__Internal")]
	private static extern void _IADCreateBannerAdPos(int x, int y, int id);
	
	[DllImport ("__Internal")]
	private static extern void _IADShowAd(int id);
	
	[DllImport ("__Internal")]
	private static extern void _IADHideAd(int id);
	#endif


	private bool _IsLoaded = false;
	private bool _IsOnScreen = false;
	private bool firstLoad = true;
	
	private bool _ShowOnLoad = true;

	private int _id;
	private TextAnchor _anchor;

	public Action AdLoadedAction  			= delegate {};
	public Action FailToReceiveAdAction  	= delegate {};
	public Action AdWiewLoadedAction  		= delegate {};
	public Action AdViewActionBeginAction  	= delegate {};
	public Action AdViewFinishedAction 		= delegate {};



	


	public iAdBanner(TextAnchor anchor,  int id) {
		_id = id;
		_anchor = anchor;
		
		
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_IADCreateBannerAd(gravity, _id);
		#endif
	}
	
	public iAdBanner(int x, int y,  int id) {
		_id = id;
		
		
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_IADCreateBannerAdPos(x, y, _id);
		#endif
		
		
	}


	public void Hide() { 
		if(!_IsOnScreen) {
			return;
		}
		
		_IsOnScreen = false;
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_IADHideAd(_id);
		#endif
	}
	
	
	public void Show() { 
		
		if(_IsOnScreen) {
			return;
		}
		
		_IsOnScreen = true;
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			_IADShowAd(_id);
		#endif
	}

	
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------


	public int id {
		get {
			return _id;
		}
	}
	

	
	
	public bool IsLoaded {
		get {
			return _IsLoaded;
		}
	}
	
	public bool IsOnScreen {
		get {
			return _IsOnScreen;
		}
	}
	
	public bool ShowOnLoad {
		get {
			return _ShowOnLoad;
		} 
		
		set {
			_ShowOnLoad = value;
		}
	}
	
	public TextAnchor anchor {
		get {
			return _anchor;
		}
	}
	

	public int gravity {
		get {
			switch(_anchor) {
			case TextAnchor.LowerCenter:
				return IOSGravity.BOTTOM | IOSGravity.CENTER;
			case TextAnchor.LowerLeft:
				return IOSGravity.BOTTOM | IOSGravity.LEFT;
			case TextAnchor.LowerRight:
				return IOSGravity.BOTTOM | IOSGravity.RIGHT;
				
			case TextAnchor.MiddleCenter:
				return IOSGravity.CENTER;
			case TextAnchor.MiddleLeft:
				return IOSGravity.CENTER | IOSGravity.LEFT;
			case TextAnchor.MiddleRight:
				return IOSGravity.CENTER | IOSGravity.RIGHT;
				
			case TextAnchor.UpperCenter:
				return IOSGravity.TOP | IOSGravity.CENTER;
			case TextAnchor.UpperLeft:
				return IOSGravity.TOP | IOSGravity.LEFT;
			case TextAnchor.UpperRight:
				return IOSGravity.TOP | IOSGravity.RIGHT;
			}
			
			return IOSGravity.TOP;
		}
	}

	

	public void didFailToReceiveAdWithError() {
		FailToReceiveAdAction();
		dispatch(iAdEvent.FAIL_TO_RECEIVE_AD);
	}
	
	
	public void bannerViewDidLoadAd() {
		_IsLoaded = true;
		if(ShowOnLoad && firstLoad) {
			Show();
			firstLoad = false;
		}

		AdLoadedAction();
		dispatch(iAdEvent.AD_LOADED);

	}
	

	public void bannerViewWillLoadAd(){
		AdWiewLoadedAction();
		dispatch(iAdEvent.AD_VIEW_LOADED);
	}
	
	
	public void bannerViewActionDidFinish(){
		AdViewFinishedAction();
		dispatch(iAdEvent.AD_VIEW_ACTION_FINISHED);
	}
		

	public void bannerViewActionShouldBegin() {
		AdViewActionBeginAction();
		dispatch(iAdEvent.AD_VIEW_ACTION_BEGIN);
	}
}

