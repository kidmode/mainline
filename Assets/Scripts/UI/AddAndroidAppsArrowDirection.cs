using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AddAndroidAppsArrowDirection : MonoBehaviour {

	[SerializeField]
	private ScrollRect addAndroidAppsScrollRect;
	[SerializeField]
	private int minContentSizeToShowArrow = 4;

	private int totalApps = 0;

	// Use this for initialization
	void Start () {
	
		addAndroidAppsScrollRect.onValueChanged.AddListener(onValueChanged);

		#if UNITY_ANDROID && !UNITY_EDITOR
		totalApps = KidMode.getApps().Count;
		#elif UNITY_EDITOR
		totalApps = 6;
		#endif
		showArrowIfLargerThenScrollContentSize();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void onValueChanged(Vector2 scrollRectPos)
	{
		if (!showArrowIfLargerThenScrollContentSize())
		{
			return;
		}
		
		if (scrollRectPos.y > 0.03)
		{
			this.gameObject.SetActive(true);
			if (this.transform.eulerAngles.z == 0)
			{
				this.transform.eulerAngles = new Vector3(0, 0, 180);
			}
		}
		else
		{
			this.gameObject.SetActive(true);
			if (this.transform.eulerAngles.z > 179.0f)
			{
				this.transform.eulerAngles = new Vector3(0, 0, 0);
			}
		}
	}
	
	private bool showArrowIfLargerThenScrollContentSize()
	{
		bool result = false;
		if (totalApps < minContentSizeToShowArrow)
		{
			this.gameObject.SetActive(false);
		}
		else if (totalApps >= minContentSizeToShowArrow)
		{
			this.gameObject.SetActive(true);
			result = true;
		}
		return result;
	}

}
