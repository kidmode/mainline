using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewsInfo : System.Object
{
	public Texture2D icon;
	public string message;
	public int number;

	public NewsInfo( Texture2D p_icon, string p_message, int p_number )
	{
		icon 	= p_icon;
		message = p_message;
		number	= p_number;
	}
	
}

public class EntranceCanvas : UICanvas
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );

		
		_setupList();
	}

	public override void update()
	{
		base.update();
	}


	public override void enteringTransition(  )
	{
		base.enteringTransition( );
	}

	public override void exitingTransition( )
	{
		base.exitingTransition( );

	}
 
	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}



//----------------- Private Implementation -------------------


	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UIButton l_button = p_element as UIButton;
		DebugUtils.Assert( l_button != null );

		NewsInfo l_info = p_data as NewsInfo;
		DebugUtils.Assert( l_info != null );

		UILabel l_messageLabel = l_button.getView( "messageLabel" ) as UILabel;
		l_messageLabel.text = l_info.message;

		UILabel l_numberLabel = l_button.getView( "newsNumber" ) as UILabel;
		l_numberLabel.text = l_info.number.ToString();
	}
	
	private void _setupList()
	{
		UISwipeList l_swipe = getView( "newsFeedSwipeList" ) as UISwipeList;
		
		List< System.Object > infoData = new List< System.Object >();
		infoData.Add( new NewsInfo( null, "Four new videos have been added to the library", 4 ) );
		infoData.Add( new NewsInfo( null, "You have two new messages in your inbox", 2 ) );
		infoData.Add( new NewsInfo( null, "Six new coloring pages have been added to the Studio", 6 ) );
		infoData.Add( new NewsInfo( null, "This is a really cool message.", 1337 ) );
		
		
		l_swipe.setData( infoData );
		l_swipe.setDrawFunction( onListDraw );
		l_swipe.redraw();
	}


}
