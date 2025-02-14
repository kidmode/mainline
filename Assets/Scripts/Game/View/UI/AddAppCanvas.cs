﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AddAppCanvas : UICanvas
{

	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		
		SetupLocalizition ();

		m_btnBgIcon = Resources.Load ( "GUI/2048/common/buttons/bt_dash_red_up" ) as Texture2D;
		m_btnIcon = Resources.Load ( "GUI/2048/common/icon/icon_delete" ) as Texture2D;

		m_normalBtnBgIcon = Resources.Load ( "GUI/2048/common/buttons/bt_dash_green_up" ) as Texture2D;
		m_normalBtnIcon = Resources.Load ( "GUI/2048/common/icon/icon_add" ) as Texture2D;
	}

	public override void update ()
	{
		base.update ();
	}

	public override void dispose (bool p_deep)
	{
		base.dispose (p_deep);
	}

	public override void enteringTransition ()
	{
		base.enteringTransition ();
		tweener.addAlphaTrack( 1.0f, 0.0f, 0.0f, onFadeFinish );
	}

	public override void exitingTransition ()
	{
		base.exitingTransition ();
	}

	public void firstLoadApp()
	{	
		reload ();

		UISwipeList l_swipe = getView ( "appSwipeList" ) as UISwipeList;
		l_swipe.setData ( m_dataList );
		
		l_swipe.setDrawFunction ( onListDraw );
		l_swipe.redraw ();
		l_swipe.active = true;
	}
	
	public void onButtonClicked(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		AppInfo l_appInfo = p_data as AppInfo;
		DebugUtils.Assert ( l_appInfo != null );
		
		l_appInfo.isAdded = !l_appInfo.isAdded;
		m_dataList [p_index] = l_appInfo;
		p_list.setDataWithoutMove (m_dataList);
	}
	//----------------- Private Implementation -------------------
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private void reload()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		m_dataList = new List<object> ();
		List<object> l_list = KidMode.getAppsSorted();
		foreach (AppInfo l_app in l_list)
		{
			m_dataList.Add(l_app);
		}
		#endif
	}

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UIElement l_prototype = p_element;
		DebugUtils.Assert( l_prototype != null );

		UILabel l_appName 		= l_prototype.getView ( "appNameText" ) as UILabel;
		UIImage l_appIcon 		= l_prototype.getView ( "iconImage" ) 	as UIImage;
		UILabel l_message 		= l_prototype.getView ( "messageText" ) as UILabel;
		UIImage l_buttonBgIcon 	= l_prototype.getView ( "btnBgImage" ) 	as UIImage;
		UIImage l_buttonIcon 	= l_prototype.getView ( "btnImage" ) 	as UIImage;

		AppInfo l_app = p_data as AppInfo;

		l_appName.text = l_app.appName;
		if (l_app.appIcon == null) {
			l_app.appIcon = ImageCache.getCacheImage (l_app.packageName + ".png");
		}

		if (null != l_app.appIcon)
			l_appIcon.setTexture (l_app.appIcon);


		if( l_app.isAdded )
		{
			l_message.text = Localization.getString (Localization.TXT_74_LABEL_ADDED);
			l_buttonBgIcon.setTexture( m_btnBgIcon );
			l_buttonIcon.setTexture( m_btnIcon );
		}
		else
		{
			l_message.text = string.Empty;
			l_buttonBgIcon.setTexture( m_normalBtnBgIcon );
			l_buttonIcon.setTexture( m_normalBtnIcon );
		}
	}

	private void SetupLocalizition()
	{
		UILabel l_top = getView ("titleImage").getView("titleText") as UILabel;
		
		l_top.text = Localization.getString (Localization.TXT_74_LABEL_TITLE);
	}

	private Texture2D m_btnBgIcon;
	private Texture2D m_btnIcon;

	private Texture2D m_normalBtnBgIcon;
	private Texture2D m_normalBtnIcon;

	private List<object> m_dataList;
}

public class AppInfo
{
	public string appName;
	public string packageName;
	public Texture2D appIcon;
	public bool isAdded = false;

	public void dispose()
	{
		if (null != appIcon)
		{
			GameObject.Destroy(appIcon);
			appIcon = null;
		}
	}
}