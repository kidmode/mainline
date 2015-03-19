using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FAQCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		_setupElement ();
		
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
		tweener.addAlphaTrack( 1.0f, 0.0f, 0.0f, onFadeFinish );
	}

	//-- Private Implementation --
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private void _setupElement()
	{
		UISwipeList l_swipeList = getView ("FAQSwipeList") as UISwipeList;

		List<object> l_data = new List<object> ();
		string[] l_FAQ1 = new string[3];
		l_FAQ1[0] = "1.";
		l_FAQ1[1] = "How do I add another child to Kid Mode?";
		l_FAQ1[2] = "";
		string[] l_FAQ2 = new string[3];
		l_FAQ2[0] = "2.";
		l_FAQ2[1] = " How do I delete or edit a child's profile?";
		l_FAQ2[2] = "";
		string[] l_FAQ3 = new string[3];
		l_FAQ3[0] = "3.";
		l_FAQ3[1] = " How do I play Flash games (in Android)? ";
		l_FAQ3[2] = "";
		string[] l_FAQ4 = new string[3];
		l_FAQ4[0] = "4.";
		l_FAQ4[1] = "How do I set up child lock correctly? Why can my kid still escape from Kid Mode, even when child lock is ON?";
		l_FAQ4[2] = "";
		string[] l_FAQ5 = new string[3];
		l_FAQ5[0] = "5.";
		l_FAQ5[1] = "Why aren't YouTube videos playing correctly in Kid Mode?";
		l_FAQ5[2] = "";
		string[] l_FAQ6 = new string[3];
		l_FAQ6[0] = "6.";
		l_FAQ6[1] = "How do I block the HOME button on iOS devices?";
		l_FAQ6[2] = "";
		string[] l_FAQ7 = new string[3];
		l_FAQ7[0] = "7.";
		l_FAQ7[1] = "Why does Zoodles say I paid for Premium but cannot cancel (actually HTC FREE PREMIUM)?";
		l_FAQ7[2] = "";
		string[] l_FAQ8 = new string[3];
		l_FAQ8[0] = "8.";
		l_FAQ8[1] = "How do I add my installed apps into Kid Mode?";
		l_FAQ8[2] = "";
		l_data.Add (l_FAQ1);
		l_data.Add (l_FAQ2);
		l_data.Add (l_FAQ3);
		l_data.Add (l_FAQ4);
		l_data.Add (l_FAQ5);
		l_data.Add (l_FAQ6);
		l_data.Add (l_FAQ7);
		l_data.Add (l_FAQ8);
		
		l_swipeList.setData (l_data);
		l_swipeList.setDrawFunction (onListDraw);
		l_swipeList.redraw ();
	}

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UILabel l_indexLabel = p_element.getView ("indexText") as UILabel;
		UILabel l_contentLabel = p_element.getView ("contentText") as UILabel;

		string[] l_data = p_data as string[];

		l_indexLabel.text = l_data[0];
		l_contentLabel.text = l_data[1];

	}

}