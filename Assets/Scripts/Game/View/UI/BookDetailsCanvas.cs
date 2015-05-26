using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BookDetailsCanvas : UICanvas
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );
		
		m_dialog = getView ("dialog") as UIElement;
		m_dialogMovePosition = 800;
	}
	
	public override void update()
	{
		base.update();
	}
	
	public void setUIManager(UIManager p_UIManager)
	{
		m_uiManager = p_UIManager;
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
	
	public void setOriginalPosition()
	{
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_dialog.transform.localPosition );
		l_pointListOut.Add( m_dialog.transform.localPosition - new Vector3( 0, m_dialogMovePosition, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f );
	}
	
	public void setOutPosition()
	{
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_dialog.transform.localPosition );
		l_pointListOut.Add( m_dialog.transform.localPosition + new Vector3( 0, m_dialogMovePosition, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	public void setAuthor(string p_author)
	{
		setText ("author: "+p_author,"aurhorNameText");
	}

	public void setIllustrator(string p_illustrator)
	{
		setText ("illustrator: "+p_illustrator,"illustratorNameText");
	}

	public void setBookName(string p_bookName)
	{
		setText (p_bookName,"bookNameText");
		setText (p_bookName,"dialogText");
	}

	public void setText(string p_text,string p_elementName)
	{
		UILabel l_element = getView (p_elementName) as UILabel;
		if (null != l_element)
			l_element.text = p_text;
	}

	public void setBookIcon(Texture2D p_texture)
	{
		UIImage l_icon = getView ("bookImage") as UIImage;
		if(null != l_icon && null != p_texture)
			l_icon.setTexture (p_texture);
	}

	//-- Private Implementation --
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}
	
	private UIManager m_uiManager;
	private UIElement m_dialog;
	private int m_dialogMovePosition;
}
