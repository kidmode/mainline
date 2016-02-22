using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ExtendedContentSizeFitter : ContentSizeFitter {

	[AddComponentMenu("Layout/ExtendedContentSizeFitter")]

	public delegate void RectTransformChangedEvent();
	public event RectTransformChangedEvent rectTransformChanged;

	// Use this for initialization
	void Start () {
	
	}

	void OnDestroy()
	{
		rectTransformChanged = null;
		base.OnDestroy();
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange ();
		if (rectTransformChanged != null) 
		{
			rectTransformChanged();
		}
	}
}
