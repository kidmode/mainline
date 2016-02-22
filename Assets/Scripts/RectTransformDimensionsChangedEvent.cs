using UnityEngine;
using UnityEngine.EventSystems;
using System;


using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public delegate void OnWindowResize();

public class RectTransformDimensionsChangedEvent : UIBehaviour{
	
	public static RectTransformDimensionsChangedEvent instance = null;
	public OnWindowResize windowResizeEvent;
	
	void Awake() {
		instance = this;
	}
	
	protected override void OnRectTransformDimensionsChange ()
	{
		base.OnRectTransformDimensionsChange ();
		if(windowResizeEvent != null)
			windowResizeEvent();
	}
}
