using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SelectAvatarState : GameState 
{	
	//Public variables
	
	
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );

		m_game = p_gameController.game;

		m_avatarImgPath = "icon_avatar_gen";

		_setupScreen( p_gameController.getUI() );

		if (null == m_postData) 
		{
			m_postData = new Hashtable();
		}

	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
		if (gotoPrevious)
		{
			p_gameController.changeState(ZoodleState.CREATE_CHILD);
			gotoPrevious = false;
		}
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		m_lastIndex = -1;
		p_gameController.getUI().removeScreen( m_selectAvatarCanvas );
	}
	
	
	//---------------- Private Implementation ----------------------
	
	private void _setupScreen( UIManager p_uiManager )
	{
		m_selectAvatarCanvas = p_uiManager.createScreen( UIScreen.SELECT_AVATAR, true, 1 );

		m_titleArea = m_selectAvatarCanvas.getView( "titleArea" ) as UIElement;
		m_avatarSwipe = m_selectAvatarCanvas.getView( "avatarSwipeList" ) as UISwipeList;
	//	m_usePhotoButton = m_selectAvatarCanvas.getView( "usePhotoButton" ) as UIButton;
		m_saveButton = m_selectAvatarCanvas.getView( "saveButton" ) as UIButton;
		m_saveButton.addClickCallback (toSaveAvatar);
		m_avatarSwipe.active = false;
	//	m_usePhotoButton.active = false;
		m_saveButton.active = false;

		m_backButton = m_selectAvatarCanvas.getView ("backButton") as UIButton;
		m_backButton.addClickCallback (toBack);

		m_titleArea.tweener.addAlphaTrack (0.0f, 1.0f, 1.0f,onTitleTweenFinish);

		m_avatarSwipe.addClickListener( "Prototype", onClickAvatarBtn );

	}

	private void toBack( UIButton p_button )
	{
		gotoPrevious = true;
	}

	private void toSaveAvatar( UIButton p_button )
	{
		SessionHandler.getInstance ().selectAvatar = m_avatarImgPath;
		m_gameController.connectState(ZoodleState.CREATE_CHILD,int.Parse(m_gameController.stateName));
		m_game.gameController.changeState (ZoodleState.CREATE_CHILD);
	}

	private void onTitleTweenFinish( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_avatarSwipe.tweener.addAlphaTrack (0.0f, 1.0f, 1.0f, onAvatarSelectTweenFinish);
	}

	private void onAvatarSelectTweenFinish( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		//m_usePhotoButton.tweener.addAlphaTrack (0.0f, 1.0f, 1.0f);
		m_saveButton.tweener.addAlphaTrack (0.0f, 1.0f, 1.0f);
	}
	
	private void onClickAvatarBtn(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		
		if (-1 != m_lastIndex) 
		{
			(p_list.getData()[m_lastIndex] as AvatarButton).isSelected = false;
		}
		m_lastIndex = p_index;
		(p_data as AvatarButton).isSelected = true;

		m_avatarImgPath = (p_data as AvatarButton).name;
	}

	//Private variables

	private UIElement 	m_titleArea;
	private UISwipeList m_avatarSwipe;
	//private UIButton 	m_usePhotoButton;
	private UIButton 	m_saveButton;
	private UIButton 	m_backButton;
	private Game 		m_game;

	private string		m_avatarImgPath;

	private UICanvas    m_selectAvatarCanvas;

	private Hashtable 	m_postData;
	private int 		m_lastIndex = -1;

	private bool 		gotoPrevious = false;
}
