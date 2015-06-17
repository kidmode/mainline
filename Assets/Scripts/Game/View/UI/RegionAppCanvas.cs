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
		
//		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );
//		setupLocalization ();
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
	
	
//	//-- Private Implementation --
//	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
//	{
//		UICanvas l_canvas = p_element as UICanvas;
//		l_canvas.isTransitioning = false;
//	}

	private UISwipeList 	m_appSwipeList;
	
}
