using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RecommendedAppsArrowDirection : MonoBehaviour {

	[SerializeField]
	private ScrollRect recommendedAppsScrollRect;
	[SerializeField]
	private int minContentSizeToShowArrow = 4;

	// Use this for initialization
	void Start () {

		recommendedAppsScrollRect.onValueChanged.AddListener(onValueChanged);
	
		showArrowIfLargerThenScrollContentSize();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void onValueChanged(Vector2 scrollRectPos)
	{
		if (!showArrowIfLargerThenScrollContentSize())
		{
			return;
		}

		if (scrollRectPos.y > 0.01)
		{
			this.gameObject.SetActive(true);
		}
		else
		{
			this.gameObject.SetActive(false);
		}
	}

	private bool showArrowIfLargerThenScrollContentSize()
	{
		bool result = false;
		if (SessionHandler.getInstance().currentKid.appList == null || SessionHandler.getInstance().currentKid.appList.Count < minContentSizeToShowArrow)
		{
			this.gameObject.SetActive(false);
		}
		else if (SessionHandler.getInstance().currentKid.appList.Count >= minContentSizeToShowArrow)
		{
			this.gameObject.SetActive(true);
			result = true;
		}
		return result;
	}
}
