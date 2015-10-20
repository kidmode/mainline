using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Sean: After create kid, show kid profile

public class ProfileViewState : GameState 
{
	
	//Standard state flow	
	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);
		m_gameController = p_gameController;
		_setupScreen(p_gameController.getUI());
	}
	
	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);
	}
	
	public override void exit(GameController p_gameController)
	{
		base.exit(p_gameController);

		p_gameController.getUI().removeScreenImmediately(m_profileViewCanvas);
	}

	//---------------- Private Implementation ----------------------
	
	private void _setupScreen(UIManager p_uiManager)
	{
		m_backScreen = p_uiManager.findScreen( UIScreen.SPLASH_BACKGROUND );
		if (m_backScreen == null)
		{
			m_backScreen = p_uiManager.createScreen(UIScreen.SPLASH_BACKGROUND, true, -1);
			(m_backScreen as SplashBackCanvas).setDown();
		}
		m_profileViewCanvas = p_uiManager.createScreen (UIScreen.PROFILE_VIEW, true);
		m_kidProfile = m_profileViewCanvas.getView ("kidProfileImage");
		m_buttonArea = m_profileViewCanvas.getView ("buttonArea");
		m_kidProfile.active = false;
		m_buttonArea.active = false;
		m_profileImage = m_profileViewCanvas.getView ("avatarIcon") as UIImage;
		m_kidName = m_profileViewCanvas.getView ("kidName") as UILabel;
		m_topicText = m_profileViewCanvas.getView ("topicText") as UILabel;
		m_addChildButton = m_profileViewCanvas.getView ("addChildButton") as UIButton;
		m_finishButton = m_profileViewCanvas.getView ("finishButton") as UIButton;
		m_backButton = m_profileViewCanvas.getView ("backButton") as UIButton;
		List<Kid> l_kidList = SessionHandler.getInstance ().kidList;
		if(null != l_kidList && l_kidList.Count > 0)
		{
			Kid l_newCreatedKid = l_kidList [l_kidList.Count - 1];
			m_profileImage.setTexture (l_newCreatedKid.kid_photo);
			m_kidName.text = l_newCreatedKid.name;
			m_topicText.text = l_newCreatedKid.name + Localization.getString(Localization.TXT_STATE_54_CREATED);
		}
		m_addChildButton.addClickCallback (toCreateKidScreen);
		m_finishButton.addClickCallback (toProfileSelectScreen);
		m_backButton.addClickCallback (toCreateKidScreen);
		m_kidProfile.tweener.addAlphaTrack(0.0f, 1.0f, 0.5f,onkidProfileTweenFinish);
	}

	private void toCreateKidScreen(UIButton p_button)
	{
		p_button.removeClickCallback (toCreateKidScreen);
		SessionHandler.getInstance().CreateChild = true;
		m_gameController.changeState (ZoodleState.CREATE_CHILD_NEW);
	}

	private void toProfileSelectScreen(UIButton p_button)
	{
		p_button.removeClickCallback (toProfileSelectScreen);

		if (!SessionHandler.getInstance().token.isCurrent() && !SessionHandler.getInstance().token.isPremium())
		{
			m_gameController.connectState (ZoodleState.SIGN_IN_UPSELL, int.Parse(m_gameController.stateName));
			m_gameController.changeState (ZoodleState.SIGN_IN_UPSELL);
		}
		else
		{
			m_gameController.changeState (ZoodleState.PROFILE_SELECTION);
		}

	}

	private void onkidProfileTweenFinish(UIElement p_element, Tweener.TargetVar p_targetVar)
	{
		m_buttonArea.tweener.addAlphaTrack(0.0f, 1.0f, 0.2f);
	}
	
	private UICanvas m_backScreen;
	private UIElement m_kidProfile;
	private UIElement m_buttonArea;
	private UIImage m_profileImage;
	private UILabel m_kidName;
	private UILabel m_topicText;
	private UIButton m_addChildButton;
	private UIButton m_finishButton;
	private UIButton m_backButton;
	private UICanvas m_profileViewCanvas;
}
