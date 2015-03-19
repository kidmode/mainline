using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeftMenuCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );

		if(SessionHandler.getInstance ().token.isTried())
		{
			UIElement l_tryArea = getView("tryPremiumButtonArea") as UIElement;
			l_tryArea.active = false;
		}

		if (SessionHandler.getInstance ().token.isCurrent() || SessionHandler.getInstance ().token.isPremium())
		{
			UIElement l_tryArea = getView("tryPremiumButtonArea") as UIElement;
			l_tryArea.active = false;
			UIElement l_buyGemsArea = getView("buyGemsButtonArea") as UIElement;
			l_buyGemsArea.active = false;
		}

		m_displaySpeed = 0.2f;

		_setupList ();
	}
	
	public override void update()
	{
		base.update();
	}

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UIButton l_button = p_element as UIButton;
		DebugUtils.Assert( l_button != null );
		
		Kid l_kid = (Kid)p_data;
		Texture2D l_childAvatar = l_kid.kid_photo;
		DebugUtils.Assert( l_childAvatar != null );
		
		UIImage l_image = l_button.getView( "childImage" ) as UIImage;
		l_image.setTexture( l_childAvatar );

		UILabel l_label = l_button.getView( "childName" ) as UILabel;
		l_label.text = l_kid.name;
	}
	
	private void _setupList( )
	{
		m_sliderDownPanel = getView ("sildeDownPanel") as UIElement;
		UISwipeList l_swipe = getView( "childSwipeList" ) as UISwipeList;
		
		List< System.Object > infoData = new List< System.Object >();
		List<Kid> l_kidList = SessionHandler.getInstance ().kidList;
		Kid l_creatKid = new Kid ();
		l_creatKid.name = "Add A Child";
		l_creatKid.kid_photo = Resources.Load("GUI/2048/common/icon/icon_profile_add") as Texture2D;
		infoData.Add (l_creatKid);
		if (null != l_kidList)
		{
			int l_kidCount = l_kidList.Count;
			for(int i = 0; i < l_kidCount; i++)
			{
				infoData.Add( l_kidList[i] );
			}
		}
		l_swipe.setData( infoData );
		l_swipe.setDrawFunction( onListDraw );
		l_swipe.redraw();
	}
	 
	public void showKids(TweenFinishedHandler p_function)
	{
		if (!isAllChildShow) 
		{
			Vector3 l_position = m_sliderDownPanel.transform.localPosition;
			List<Vector3> l_posList = new List<Vector3> ();
			l_posList.Add (l_position);
			l_posList.Add (l_position + new Vector3 (0, -300, 0));
			m_sliderDownPanel.tweener.addPositionTrack (l_posList, m_displaySpeed, p_function, Tweener.Style.QuadOutReverse);
			isAllChildShow= true;
		}
		else
		{
			Vector3 l_position = m_sliderDownPanel.transform.localPosition;
			List<Vector3> l_posList = new List<Vector3> ();
			l_posList.Add (l_position);
			l_posList.Add (l_position + new Vector3 (0, 300, 0));
			m_sliderDownPanel.tweener.addPositionTrack (l_posList, m_displaySpeed, p_function, Tweener.Style.QuadOutReverse);
			isAllChildShow= false;
		}
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

	public float displaySpeed
	{
		get
		{
			return m_displaySpeed;
		}
		set
		{
			m_displaySpeed = value;
		}
	}

	//-- Private Implementation --
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private bool 		isAllChildShow = false;
	private float 		m_displaySpeed;
	private GameController		m_gameController;
	private UIElement 	m_sliderDownPanel;
	private UIButton 	m_showProfileButton;
}
