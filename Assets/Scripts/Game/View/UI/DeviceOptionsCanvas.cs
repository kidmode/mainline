using UnityEngine;
using System.Collections;

public class DeviceOptionsCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		SetupLocalizition ();

		m_sliderArea = getView ("sliderArea");
		m_allowIncomingCall = getView ("smartSelect");
		m_displayHelpFulTips = getView ("addApp");
		m_allowIncomingCall.active = false;
		m_displayHelpFulTips.active = false;
		m_sliderArea.active = false;
		tweener.addAlphaTrack( 0.0f, 1.0f, 0.1f, onShowFinish );
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

	private void onShowFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		m_sliderArea.active = true;
		m_displayHelpFulTips.active = true;

		m_allowIncomingCall.active = true;
		UIImage disableImage = getView("disableImage") as UIImage;
		disableImage.alpha = 0.5f;
	}

	private void SetupLocalizition()
	{
		UILabel l_top = getView("topText") as UILabel;
		UILabel l_master = getView("masterText") as UILabel;
		UILabel l_music = getView("musicText") as UILabel;
		UILabel l_effects = getView("effectsText") as UILabel;
		UILabel l_call = getView("AllowCallButton").getView("titleText") as UILabel;
		UILabel l_tips = getView("tipButton").getView("titleText") as UILabel;
		UILabel l_refresh = getView("refreshButton").getView("purchaseBtnText") as UILabel;
		UILabel l_update = getView("noticePanel").getView("noticeText") as UILabel;
		UILabel l_close = getView("noticePanel").getView("closelText") as UILabel;
		
		l_top.text = Localization.getString( Localization.TXT_49_LABEL_TOP );
		l_master.text = Localization.getString( Localization.TXT_49_LABEL_MASTER );
		l_music.text = Localization.getString( Localization.TXT_49_LABEL_MUSIC );
		l_effects.text = Localization.getString( Localization.TXT_49_LABEL_EFFECTS );
		l_call.text = Localization.getString( Localization.TXT_49_LABEL_CALL );
		l_tips.text = Localization.getString( Localization.TXT_49_LABEL_TIPS );
		l_refresh.text = Localization.getString( Localization.TXT_49_LABEL_REFRESH );
		l_update.text = Localization.getString( Localization.TXT_49_LABEL_UPDATE);
		l_close.text = Localization.getString( Localization.TXT_49_LABEL_CLOSE );
	}

	private UIElement m_sliderArea;
	private UIElement m_allowIncomingCall;
	private UIElement m_displayHelpFulTips;

}