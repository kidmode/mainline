/*
 * Project: VZW
 * Author: Sean Chiu
 */

using UnityEngine;
using System.Collections;

public class RegionAppCanvas: UICanvas
{

	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );

		m_emptyTexture = new Texture2D (1, 1);

		m_appSwipeList = getView("appScrollView") as UISwipeList;

		_setupList();
	}
	
	public void setupLocalization()
	{
//		UILabel l_title = getView ("topicText") as UILabel;
//		l_title.text = Localization.getString (Localization.TXT_99_LABEL_TITLE);
//		UILabel l_cardTopic = getView ("cardTopicText") as UILabel;
//		l_cardTopic.text = Localization.getString (Localization.TXT_99_LABEL_CARD_TITLE);
//		UILabel l_enterChildNameLabel = getView ("enterChildNameLabel") as UILabel;
//		l_enterChildNameLabel.text = Localization.getString (Localization.TXT_99_LABEL_ENTER_NAME);
//		UILabel l_enterChildAgeLabel = getView ("enterChildAgeLabel") as UILabel;
//		l_enterChildAgeLabel.text = Localization.getString (Localization.TXT_99_LABEL_ENTER_AGE);
//		UILabel l_createProfileText = getView ("createProfileText") as UILabel;
//		l_createProfileText.text =  Localization.getString (Localization.TXT_99_BUTTON_CREATE_PROFILE);
//		UILabel l_backLabel = getView ("btnText") as UILabel;
//		l_backLabel.text = Localization.getString (Localization.TXT_BUTTON_BACK);
//		
//		UILabel l_name = getView ("childFirstNameTextPlaceHolder") as UILabel;
//		UILabel l_year = getView ("YearTextPlaceholder") as UILabel;
//		UILabel l_month = getView ("MonthTextPlaceholder") as UILabel;
//		
//		l_name.text = Localization.getString (Localization.TXT_99_LABEL_NAME);
//		l_year.text = Localization.getString (Localization.TXT_23_LABEL_YEAR);
//		l_month.text = Localization.getString (Localization.TXT_23_LABEL_MONTH);
	}
	
//	public override void update()
//	{
//		base.update();
//	}
//	
//	public override void dispose( bool p_deep )
//	{
//		base.dispose( p_deep );
//	}
//	
//	public override void enteringTransition()
//	{
//		base.enteringTransition();
//		tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
//	}
	
	
	//-- Private Implementation --
	private void _setupList()
	{
		m_appSwipeList.setDrawFunction(onListDraw);
	}

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UIButton l_button = p_element as UIButton;
		DebugUtils.Assert( l_button != null );

		AppInfo l_info = p_data as AppInfo;
		DebugUtils.Assert( l_info != null );

		UIImage l_rawImage = p_element.getView("icon") as UIImage;
		UIImage l_appImage = p_element.getView("appIcon") as UIImage;
		UILabel l_appName = p_element.getView("appName") as UILabel;

		if (l_appImage != null)
			l_appImage.active = false;
	
		if (l_rawImage == null)
            return;

		l_appName.text = l_info.appName;
		if (l_appName.active == false)
		{
			l_appName.active = true;
		}

		Vector2 l_textSize = l_appName.calcSize();
		RectTransform l_transform = l_appName.gameObject.GetComponent<RectTransform>();
		float l_scale = Mathf.Min(l_transform.sizeDelta.x / l_textSize.x, 1.0f);
		l_transform.localScale = new Vector3(l_scale, l_scale, 1);


		if( l_info.appIcon == null )
		{
			l_info.appIcon = ImageCache.getCacheImage(l_info.packageName + ".png");
			if(l_info.appIcon == null)
				l_appImage.setTexture( m_emptyTexture );
			else {
				l_appImage.setTexture(l_info.appIcon);
				l_appImage.active = true;
				l_rawImage.active = false;
			}
		}
		else
		{
			l_appImage.setTexture(l_info.appIcon);
			l_appImage.active = true;
			l_rawImage.active = false;
		}
	}
//	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
//	{
//		UICanvas l_canvas = p_element as UICanvas;
//		l_canvas.isTransitioning = false;
//	}

	private UISwipeList 	m_appSwipeList;

	private Texture2D m_emptyTexture;

}
