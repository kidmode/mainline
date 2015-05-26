using UnityEngine;
using System.Collections;

public class SettingNotificationCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		SetupLocalizition ();

		m_notificationPanel = getView ("contentPanel");
		m_notificationPanel.active = false;
		tweener.addAlphaTrack( 0.0f, 1.0f, 0.1f,onShowFinish );
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
		tweener.addAlphaTrack( 1.0f, 0.0f, 0.0f, onFadeFinish );
	}

	private void onShowFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		m_notificationPanel.active = true;
	}
	
	//-- Private Implementation --
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private void SetupLocalizition()
	{
		UILabel l_title = getView("mainTitleText") as UILabel;
		UILabel l_titleApps= getView("WeeklyAppsNotification").getView("titleText") as UILabel;
		UILabel l_titleSmart= getView("smartSelectNotification").getView("titleText") as UILabel;
		UILabel l_titleNew= getView("NewAppAddNotification").getView("titleText") as UILabel;
		UILabel l_desApps= getView("WeeklyAppsNotification").getView("desText") as UILabel;
		UILabel l_desSmart= getView("smartSelectNotification").getView("desText") as UILabel;
		UILabel l_desNew= getView("NewAppAddNotification").getView("desText") as UILabel;
		
		l_title.text = Localization.getString( Localization.TXT_45_LABEL_TITLE );
		l_titleApps.text = Localization.getString( Localization.TXT_45_LABEL_TITLE_APPS );
		l_titleSmart.text = Localization.getString( Localization.TXT_45_LABEL_TITLE_SMART );
		l_titleNew.text = Localization.getString( Localization.TXT_45_LABEL_TITLE_NEW );
		l_desApps.text = Localization.getString( Localization.TXT_45_LABEL_DES_APPS );
		l_desSmart.text = Localization.getString( Localization.TXT_45_LABEL_DES_SMART );
		l_desNew.text = Localization.getString( Localization.TXT_45_LABEL_DES_NEW );
	}

	private UIElement m_notificationPanel;
}