using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArrowDirectionScript : MonoBehaviour {

	[SerializeField]
	private ScrollRect scrollRect;
	[SerializeField]
	private RectTransform scrollViewSize;
	[SerializeField]
	private RectTransform contentSize;
	[SerializeField]
	private ExtendedContentSizeFitter contentSizeFitter;

	void Start () {
		
		scrollRect.onValueChanged.AddListener(onValueChanged);
		if (contentSizeFitter != null)
			contentSizeFitter.rectTransformChanged += receiveRectTransformChanged;

		showArrowIfLargerThenScrollContentSize();
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
		if (scrollViewSize.rect.height >= contentSize.rect.height)
		{
			this.gameObject.SetActive(false);
		}
		else if (scrollViewSize.rect.height < contentSize.rect.height)
		{
			this.gameObject.SetActive(true);
			result = true;
		}
		return result;
	}

	private void receiveRectTransformChanged()
	{
		showArrowIfLargerThenScrollContentSize();
		if (contentSizeFitter != null)
			contentSizeFitter.rectTransformChanged -= receiveRectTransformChanged;
	}
}
