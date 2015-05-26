using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RecordStartCanvas : UICanvas {

	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);

		m_normalIcon = Resources.Load ("GUI/2048/common/buttons/bt_circle_up") as Texture2D;
		m_selectedIcon = Resources.Load ("GUI/2048/common/buttons/bt_circle_down") as Texture2D;
		
		setupLocalization ();
		_setupElement ();
	}
	
	public void setupLocalization()
	{
		UILabel l_title = getView("titleArea").getView ("Text") as UILabel;
		l_title.text =  Localization.getString (Localization.TXT_80_LABEL_TITLE);
		UILabel l_message = getView("messageText") as UILabel;
		l_message.text =  Localization.getString (Localization.TXT_80_LABEL_MESSAGE);
		UILabel l_select = getView("selectButton").getView("Text") as UILabel;
		l_select.text =  Localization.getString (Localization.TXT_80_BUTTON_SELECT_ALL);
		UILabel l_save = getView("saveButton").getView("Text") as UILabel;
		l_save.text =  Localization.getString (Localization.TXT_80_BUTTON_SAVE);
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
	}

	public override void exitingTransition ()
	{
		base.exitingTransition ();
	}

	public void reload()
	{
		_setupElement ();
	}

	//----------------- Private Implementation -------------------

	private void _setupElement()
	{
		List<object> l_dataList = new List<object>();

		foreach( Kid l_kid in SessionHandler.getInstance().kidList )
		{
			l_dataList.Add( l_kid );
		}

		if( null != SessionHandler.getInstance().recordKidList && 0 < SessionHandler.getInstance().recordKidList.Count )
		{
			foreach( Kid l_selected in SessionHandler.getInstance().recordKidList )
			{
				foreach( Kid l_all in SessionHandler.getInstance().kidList )
				{
					if( l_selected.id == l_all.id )
					{
						l_dataList.Remove( l_all );
					}
				}
			}
		}

		UISwipeList l_swipeList = getView( "kidSwipeList" ) as UISwipeList;
		l_swipeList.setData( l_dataList );
		l_swipeList.setDrawFunction( onListDraw );
		l_swipeList.redraw();
	}

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UIImage l_kidAvatar = p_element.getView( "kidAvatar" ) as UIImage;
		UILabel l_kidName = p_element.getView( "kidNameText" ) as UILabel;
		UIImage l_kidIcon = p_element.getView( "kidIcon" ) as UIImage;
		Kid l_kid = p_data as Kid;

		l_kidAvatar.setTexture( l_kid.kid_photo );
		l_kidName.text = l_kid.name;

		bool l_isSelected = false;

		foreach( Kid l_selected in SessionHandler.getInstance ().recordKidList )
		{
			if( l_selected.id == l_kid.id )
			{
				l_isSelected = true;
				break;
			}
		}

		if( l_isSelected )
		{
			l_kidIcon.setTexture( m_selectedIcon );
		}
		else
		{
			l_kidIcon.setTexture( m_normalIcon );
		}
	}

	private Texture2D m_normalIcon;
	private Texture2D m_selectedIcon;
}
