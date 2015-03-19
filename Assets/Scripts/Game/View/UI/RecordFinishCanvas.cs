using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RecordFinishCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);

		_setupElement ();
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
	
	//----------------- Private Implementation -------------------
	
	private void _setupElement()
	{
		List<object> l_dataList = new List<object>();
		
		foreach( Kid l_kid in SessionHandler.getInstance().recordKidList )
		{
			l_dataList.Add( l_kid );
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
		Kid l_kid = p_data as Kid;
		
		l_kidAvatar.setTexture( l_kid.kid_photo );
		l_kidName.text = l_kid.name;
	}
}
