using UnityEngine;
using System.Collections;

public class CreateChildProfileCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		SetupLocalizition ();
		
		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );
	}
	
	public override void update()
	{
		base.update();
	}

	
	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}
	
	public override void enteringTransition()
	{
		base.enteringTransition();
		tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
	}
	
	
	//-- Private Implementation --
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private void SetupLocalizition()
	{
		UILabel l_back 		= getView ("backButton").getView("btnText") as UILabel;
		UILabel l_top 		= getView ("topicText") as UILabel;
		UILabel l_notice 	= getView ("noticeText") as UILabel;
		UILabel l_cardTop 	= getView ("cardTopicText") as UILabel;
		UILabel l_name 		= getView ("enterChildNameLabel") as UILabel;
		UILabel l_birthday 	= getView ("enterChildAgeLabel") as UILabel;
		UILabel l_year 		= getView ("YearTextLabel") as UILabel;
		UILabel l_month 	= getView ("MonthTextLabel") as UILabel;
		UILabel l_picture 	= getView ("addPictureText") as UILabel;
		UILabel l_create 	= getView ("createProfileText") as UILabel;
		UILabel l_delete 	= getView ("deletePictureText") as UILabel;

		UILabel l_deleteTitle 	= getView ("titleText") as UILabel;
		UILabel l_deleteContent = getView ("noticeText1") as UILabel;
		UILabel l_deleteConfirm = getView ("noticeText3") as UILabel;
		UILabel l_confirm 	= getView ("confirmButtonText") as UILabel;
		UILabel l_cancel 	= getView ("cancelButtonText") as UILabel;

		
		l_back.text 	= Localization.getString (Localization.TXT_BUTTON_BACK);
		l_top.text 		= Localization.getString (Localization.TXT_23_LABEL_TOP);
		l_notice.text 	= Localization.getString (Localization.TXT_23_LABEL_NOTICE);
		l_cardTop.text 	= Localization.getString (Localization.TXT_23_LABEL_CARDTOP);
		l_name.text 	= Localization.getString (Localization.TXT_23_LABEL_NAME);
		l_birthday.text = Localization.getString (Localization.TXT_23_LABEL_BIRTHDAY);
		l_year.text 	= Localization.getString (Localization.TXT_23_LABEL_YEAR);
		l_month.text 	= Localization.getString (Localization.TXT_23_LABEL_MONTH);
		l_picture.text 	= Localization.getString (Localization.TXT_23_LABEL_PICTURE);
		l_create.text 	= Localization.getString (Localization.TXT_23_LABEL_CREATE);
		l_delete.text 	= Localization.getString (Localization.TXT_23_LABEL_DELETE);

		l_deleteTitle.text	 = Localization.getString (Localization.TXT_23_LABEL_DELETE_TITLE);
		l_deleteContent.text = Localization.getString (Localization.TXT_23_LABEL_DELETE_CONTENT);
		l_deleteConfirm.text = Localization.getString (Localization.TXT_23_LABEL_DELETE_CONFIRM);
		l_confirm.text 	= Localization.getString (Localization.TXT_75_BUTTON_CONFIRM);
		l_cancel.text 	= Localization.getString (Localization.TXT_75_BUTTON_CANCEL);


	}
}
