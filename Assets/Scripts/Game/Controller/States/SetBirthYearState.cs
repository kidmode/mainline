using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class SetBirthYearState : GameState 
{	
	//Public variables
	
	
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );

		//Kev
		UIManager l_ui = p_gameController.getUI();
		SplashBackCanvas splashCanvas = l_ui.findScreen (UIScreen.SPLASH_BACKGROUND) as SplashBackCanvas;
		if(splashCanvas != null)
			splashCanvas.gameObject.SetActive(false);
		//Kev
		
		_setupScreen( p_gameController.getUI() );

		gotoProfileScreen = false;

		m_inputNum = "";
		m_starLabel.text = "";

		GAUtil.logScreen("BirthYearScreen");
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
		if (gotoPrevious) 
		{
			SessionHandler.getInstance().clearUserData(false);
			p_gameController.changeState(ZoodleState.SET_UP_ACCOUNT);
			gotoPrevious = false;
		}
		if (gotoProfileScreen) 
		{
			if(null != SessionHandler.getInstance().kidList && SessionHandler.getInstance().kidList.Count > 0)
			{
				p_gameController.changeState( ZoodleState.PROFILE_SELECTION );
			}
			else
			{
				m_gameController.connectState(ZoodleState.CREATE_CHILD_NEW,int.Parse(m_gameController.stateName));
				p_gameController.changeState( ZoodleState.CREATE_CHILD_NEW );
			}
			
			gotoProfileScreen = false;
		}
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_birthCanvas );
	}
	
	
	//---------------- Private Implementation ----------------------
	
	private void _setupScreen( UIManager p_uiManager )
	{
		m_birthCanvas = p_uiManager.createScreen( UIScreen.SET_BIRTHYEAR, true, 1 );

		m_backButton = m_birthCanvas.getView("backButton") as UIButton;
		m_backButton.addClickCallback (toBack);
		List<Vector3> l_posList = new List<Vector3>();
		l_posList.Add( m_backButton.transform.localPosition + new Vector3( -100, 0, 0 ) );
		l_posList.Add( m_backButton.transform.localPosition );
		m_backButton.tweener.addPositionTrack( l_posList, 1.0f, null, Tweener.Style.QuadOutReverse );


		m_panel = m_birthCanvas.getView("panel") as UIElement;
		m_title = m_birthCanvas.getView("topicTextGroup") as UIElement;
		m_title.tweener.addAlphaTrack (0.0f, 1.0f, 1.0f,null);
		m_panel.tweener.addAlphaTrack (0.0f, 1.0f, 1.0f,onTitleTweenFinish);

		m_setButton = m_birthCanvas.getView("setButtonArea").getView("setButton") as UIButton;
		m_setButton.active = false;
		m_setButton.addClickCallback(onSetBirthYear);
		m_starLabel = m_birthCanvas.getView("panel").getView ("birthArea").getView ("starText") as UILabel;

		ArrayList l_numBtnArray = m_birthCanvas.getView ("panel").getView ("numBoard").getViewsByTag ("numBtn");
		int l_count = l_numBtnArray.Count;
		for(int l_i = 0; l_i < l_count; l_i++)
		{
			(l_numBtnArray[l_i] as UIButton).addClickCallback(clickNumButton);
 		}

		m_deleteButton = m_birthCanvas.getView("deleteButton") as UIButton;
		m_deleteButton.addClickCallback(deleteNumber);

		m_exitMessageButton = m_birthCanvas.getView("exitButton") as UIButton;
		m_exitMessageButton.addClickCallback (onExitClick);
	}

	private void onTitleTweenFinish( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		m_setButton.tweener.addAlphaTrack (0.0f, 1.0f, 0.5f);
	}

	private void toBack( UIButton p_button )
	{
		gotoPrevious = true;
	}

	private void onSetBirthYear( UIButton p_button )
	{
		if (m_inputNum.Length == 4)
		{
			int l_pin = int.Parse(m_inputNum);

			if( DateTime.Now.Year - l_pin > 13 )
			{
				RequestQueue.Request l_request = new SetPinRequest(l_pin);
				l_request.handler += _setPinComplete;
				RequestQueue l_queue = new RequestQueue();
				l_queue.add(l_request);
				l_queue.request(RequestType.RUSH);
				p_button.removeClickCallback (onSetBirthYear);
				SwrveComponent.Instance.SDK.NamedEvent("SignUp.SET_BIRTHDAY_YEAR");
			}
			else
			{
				p_button.removeClickCallback (onSetBirthYear);
				UIElement l_messagePanel = m_birthCanvas.getView( "messagePanel" );

				List<Vector3> l_pointListIn = new List<Vector3>();
				l_pointListIn.Add( l_messagePanel.transform.localPosition );
				l_pointListIn.Add( l_messagePanel.transform.localPosition + new Vector3( 0, 800, 0 ));
				l_messagePanel.tweener.addPositionTrack( l_pointListIn, 0f, onShowFinish, Tweener.Style.Standard, false );
			}
		}
	}

	private void onShowFinish( UIElement p_element, Tweener.TargetVar p_target )
	{
		m_setButton.addClickCallback (onSetBirthYear);
	}

	private void onExitClick( UIButton p_button )
	{
		UIElement l_messagePanel = m_birthCanvas.getView( "messagePanel" );
		
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( l_messagePanel.transform.localPosition );
		l_pointListOut.Add( l_messagePanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_messagePanel.tweener.addPositionTrack( l_pointListOut, 0f );

		m_starLabel.text = string.Empty;
		m_inputNum = string.Empty;
	}

	private void _setPinComplete(WWW p_response)
	{
		m_inputNum = "";
		m_starLabel.text = "";
		gotoProfileScreen = true;
	}

	private void clickNumButton( UIButton p_button )
	{
		if(m_inputNum.Length < 4)
		{
			m_inputNum = m_inputNum + p_button.name.Substring (3);
			m_starLabel.text = m_starLabel.text + "* ";

		}
	}

	private void deleteNumber( UIButton p_button )
	{
		if (m_inputNum.Length > 0)
		{
			m_inputNum = m_inputNum.Substring(0, m_inputNum.Length - 1);
			m_starLabel.text = m_starLabel.text.Substring(0, m_inputNum.Length * 2);
		}
	}

	//Private variables
	
	private UIElement	m_title;
	private UIElement 	m_panel;
	private UIButton 	m_backButton;
	private UIButton 	m_setButton;
	private UILabel 	m_starLabel;
	private UIButton	m_deleteButton;
	private UIButton 	m_exitMessageButton;

	private string 		m_inputNum;

	private UICanvas    m_birthCanvas;

	private bool 		gotoPrevious = false;
	private bool 		gotoProfileScreen = false;
}
