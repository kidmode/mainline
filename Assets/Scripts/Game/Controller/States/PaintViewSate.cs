using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PaintViewSate : GameState
{
	public override void enter (GameController p_gameController)
	{
		base.enter (p_gameController);

		m_drawingList = SessionHandler.getInstance ().drawingList;
		m_currentDrawing = SessionHandler.getInstance().currentDrawing;
		m_currentIndex = 0;

		_setupScreen(p_gameController.getUI());
	}

	public override void update (GameController p_gameController, int p_time)
	{
		base.update (p_gameController, p_time);

		if( true == m_canSetImage && null != m_currentDrawing.largeIcon)
		{
			setupImage();
		}
	}

	public override void exit (GameController p_gameController)
	{
		base.exit (p_gameController);

		UIManager l_ui = p_gameController.getUI ();
		l_ui.removeScreen( UIScreen.PAINT_VIEW );
	}

	private void _setupScreen(UIManager p_manager)
	{
		m_paintViewCanvas = p_manager.createScreen ( UIScreen.PAINT_VIEW, false, 0 );

		m_backButton 	= m_paintViewCanvas.getView ("backButton")	 as UIButton;
		m_editButton 	= m_paintViewCanvas.getView ("editButton")	 as UIButton;
		m_deleteButton 	= m_paintViewCanvas.getView ("deleteButton") as UIButton;
		m_leftButton 	= m_paintViewCanvas.getView ("leftButton")	 as UIButton;
		m_rightButton 	= m_paintViewCanvas.getView ("rightButton")	 as UIButton;
		m_artImage 		= m_paintViewCanvas.getView ("artImage")	 as UIImage;
		m_loadingLabel 	= m_paintViewCanvas.getView ("loadingText")	 as UILabel;
		m_messagePanel 	= m_paintViewCanvas.getView ("messagePanel");
		m_confirmDialog = m_paintViewCanvas.getView ("confirmDialog");
		m_yesButton = m_confirmDialog.getView ("yesButton")			 as UIButton;
		m_noButton = m_confirmDialog.getView ("noButton")			 as UIButton;

		m_backButton	.addClickCallback (onBackButtonClick);
		m_editButton	.addClickCallback (onEditButtonClick);
		m_deleteButton	.addClickCallback (onDeleteButtonClick);
		m_leftButton	.addClickCallback (onLeftButtonClick);
		m_rightButton	.addClickCallback (onRightButtonClick);
		m_yesButton		.addClickCallback (onYesButtonClick);
		m_noButton		.addClickCallback (onNoButtonClick);


		if( true == SessionHandler.getInstance().IsParent )
		{
			m_editButton.enabled = false;
		}

		getCurrentIndex ();
		setupElement ();
	}

	private void getCurrentIndex()
	{
		if( null == m_drawingList || 0 == m_drawingList.Count || null == m_currentDrawing )
			return;
		for( int i = 0; i < m_drawingList.Count; i++ )
		{
			if( m_drawingList[i].id == m_currentDrawing.id )
			{
				m_currentIndex = i;
			}
		}
	}

	private void setupImage()
	{
		if( null == m_currentDrawing.largeIcon )
		{
			m_loadingLabel.active = true;
			m_artImage.active = false;
			m_canSetImage = true;
			RequestQueue l_queue = new RequestQueue();
			l_queue.add(new ImageRequest("icon", m_currentDrawing.largeUrl, _requestIconComplete));
			l_queue.request(RequestType.RUSH);

			disableControl();
		}
		else
		{
			m_artImage.setTexture ( m_currentDrawing.largeIcon );
			m_loadingLabel.active = false;
		}
	}

	private void _requestIconComplete(WWW p_response)
	{
		m_artImage = m_paintViewCanvas.getView ("artImage")	 as UIImage;
		if( null == m_artImage )
		{
			return;
		}

		if (p_response.error == null)
		{
			m_currentDrawing.largeIcon = p_response.texture;
			m_loadingLabel.active = false;
		}
		else
		{
			m_loadingLabel.active = true;
			m_loadingLabel.text = Localization.getString(Localization.TXT_STATE_61_FAIL);
		}
		m_artImage.active = true;
		m_drawingList [m_currentIndex] = m_currentDrawing;
		enableControl();
		setupElement ();
	}

	private void disableControl()
	{
//		m_backButton	.enabled = false;
		m_editButton	.enabled = false;
		m_deleteButton	.enabled = false;
		m_leftButton	.enabled = false;
		m_rightButton	.enabled = false;
	}

	private void enableControl()
	{
//		m_backButton	.enabled = true;
		m_editButton	.enabled = true;
		m_deleteButton	.enabled = true;
		m_leftButton	.enabled = true;
		m_rightButton	.enabled = true;
	}
	
	private void setupElement()
	{
		if( m_currentIndex <= 0 )
		{
			m_leftButton.enabled = false;
			m_currentIndex = 0;
		}
		else
		{
			m_leftButton.enabled = true;
		}
		
		if( m_currentIndex >= (m_drawingList.Count - 1) )
		{
			m_rightButton.enabled = false;
			m_currentIndex = (m_drawingList.Count - 1);
		}
		else
		{
			m_rightButton.enabled = true;
		}
		
		m_currentDrawing = m_drawingList[m_currentIndex];
		SessionHandler.getInstance ().currentDrawing = m_currentDrawing;
		setupImage ();
	}

	private void onBackButtonClick( UIButton p_button )
	{
		int l_state = m_gameController.getConnectedState (ZoodleState.PAINT_VIEW);
		m_gameController.changeState (l_state);
	}

	private void onEditButtonClick( UIButton p_button )
	{
		p_button.removeClickCallback (onEditButtonClick);

//		m_gameController.connectState (ZoodleState.PAINT_ACTIVITY, ZoodleState.REGION_FUN);

		m_gameController.changeState(ZoodleState.PAINT_ACTIVITY);
	}

	private void onDeleteButtonClick( UIButton p_button )
	{
		List<Vector3> l_pointListIn = new List<Vector3>();
		l_pointListIn.Add( m_confirmDialog.transform.localPosition );
		l_pointListIn.Add( m_confirmDialog.transform.localPosition + new Vector3( 0, 800, 0 ));
		m_confirmDialog.tweener.addPositionTrack( l_pointListIn, 0f );
	}

	private void onDeleteFinish (WWW p_response)
	{
		if( null == p_response.error )
		{
			m_drawingList.RemoveAt (m_currentIndex);

			List<Vector3> l_pointListOut = new List<Vector3>();
			l_pointListOut.Add( m_messagePanel.transform.localPosition );
			l_pointListOut.Add( m_messagePanel.transform.localPosition - new Vector3( 0, 800, 0 ));
			m_messagePanel.tweener.addPositionTrack( l_pointListOut, 0f );

			if( m_drawingList.Count > 0 )
			{
				SessionHandler.getInstance().drawingList = m_drawingList;
				setupElement();
				m_deleteButton.addClickCallback (onDeleteButtonClick);
			}
			else
			{
				int l_state = m_gameController.getConnectedState (ZoodleState.PAINT_VIEW);
				m_gameController.changeState (l_state);
			}
		}
	}

	private void onLeftButtonClick( UIButton p_button )
	{
		m_currentIndex--;

		setupElement ();
	}

	private void onRightButtonClick( UIButton p_button )
	{
		m_currentIndex++;
		
		setupElement ();
	}

	private void onYesButtonClick( UIButton p_button )
	{
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_confirmDialog.transform.localPosition );
		l_pointListOut.Add( m_confirmDialog.transform.localPosition - new Vector3( 0, 800, 0 ));
		m_confirmDialog.tweener.addPositionTrack( l_pointListOut, 0f );

		List<Vector3> l_pointListIn = new List<Vector3>();
		l_pointListIn.Add( m_messagePanel.transform.localPosition );
		l_pointListIn.Add( m_messagePanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		m_messagePanel.tweener.addPositionTrack( l_pointListIn, 0f );

		p_button.removeClickCallback (onDeleteButtonClick);
		RequestQueue l_request = new RequestQueue ();
		l_request.add (new DeleteDrawingRequest(onDeleteFinish));
		l_request.request (RequestType.RUSH);
	}

	private void onNoButtonClick( UIButton p_button )
	{
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_confirmDialog.transform.localPosition );
		l_pointListOut.Add( m_confirmDialog.transform.localPosition - new Vector3( 0, 800, 0 ));
		m_confirmDialog.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	private UICanvas 	m_paintViewCanvas;

	private UILabel 	m_loadingLabel;
	private UIButton 	m_backButton;
	private UIButton 	m_editButton;
	private UIButton 	m_deleteButton;
	private UIButton 	m_leftButton;
	private UIButton 	m_rightButton;
	private UIImage 	m_artImage;

	private UIElement 	m_messagePanel;
	private UIElement 	m_confirmDialog;
	private UIButton 	m_yesButton;
	private UIButton 	m_noButton;

	private List<Drawing> m_drawingList;
	private Drawing 	m_currentDrawing;
	private int 		m_currentIndex;

	private bool 		m_canSetImage = false;
}
