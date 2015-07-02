using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class KidsProfileState : GameState
{

	//--------------------Public Interface -----------------------

	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );

		if (m_gotoBack)
		{
			m_gotoBack = false;
			int l_nextState = p_gameController.getConnectedState(ZoodleState.KIDS_PROFILE);
			if (l_nextState != 0)
			{
				m_kidsProfileCanvas.tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
				p_gameController.changeState(l_nextState);
			}
		}
		//	_changeToPrevious( p_gameController );
	}

	public override void enter (GameController p_gameController)
	{
		base.enter ( p_gameController );

		m_uiManager = m_gameController.getUI();

		_createMainView ( m_gameController );
		_setupElements();

		RequestQueue l_request = new RequestQueue ();
		l_request.add (new GetKidRequest(SessionHandler.getInstance().currentKid.id, onRequestComplete));
		l_request.request ();

		GAUtil.logScreen("KidsProfileScreen");
	}

	public override void exit (GameController p_gameController)
	{
		base.exit (p_gameController);

		m_lastIndex = -1;

		m_uiManager.removeScreen( UIScreen.KIDS_PROFILE );
		m_uiManager.removeScreen( UIScreen.PROFILE_ACTIVE );
		m_uiManager.removeScreen( UIScreen.SELECT_AVATAR );
	}

	//----------------- Private Implementation -------------------

	private void onRequestComplete(WWW p_response)
	{
		if (p_response.error != null)
			m_gameController.changeState(ZoodleState.SERVER_ERROR);
		else
		{
			string l_string = "";
			
			l_string = UnicodeDecoder.Unicode(p_response.text);
			l_string = UnicodeDecoder.UnicodeToChinese(l_string);
			l_string = UnicodeDecoder.CoverHtmlLabel(l_string);

			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(l_string) as Hashtable;
			Kid l_currentKid = new Kid(l_data);
			l_currentKid.requestPhoto();

			SessionHandler.getInstance().currentKid = l_currentKid;

			List<Kid> l_kidList = SessionHandler.getInstance().kidList;
			for (int i = 0; i < l_kidList.Count; ++i)
			{
				if (l_kidList[i].id == l_currentKid.id)
				{
					if(null != l_kidList[i].appList)
						l_currentKid.appList = l_kidList[i].appList;
					if(null != l_kidList[i].topRecommendedApp)
						l_currentKid.topRecommendedApp = l_kidList[i].topRecommendedApp;
					l_kidList[i] = l_currentKid;
					break;
				}
			}

			if( null != m_kidsProfileCanvas )
			{
				//			m_kidsProfileCanvas.refreshInfo ();
				m_profileActivityCanvas.SetupLocalizition ();
				m_infoSwipeList.active = true;
			}
		}
	}

	private void onBackButtonClick( UIButton p_button )
	{
		m_gotoBack = true;
	}
	
	private void onEditPhotoButtonClick( UIButton p_button )
	{
		moveIn( m_editPictureCanvas.getView( "mainPanel" ) );
		moveOut( m_profileActivityCanvas.getView( "panel" ) );
		moveOut( m_kidsProfileCanvas.getView( "mainPanel" ) );

		GAUtil.logScreen("EditPhotoScreen");
	}

	private void onEditBackClick( UIButton p_button )
	{
		SessionHandler.getInstance ().selectAvatar = null;
		moveOut( m_editPictureCanvas.getView( "mainPanel" ) );
		moveIn( m_profileActivityCanvas.getView( "panel" ) );
		moveIn( m_kidsProfileCanvas.getView( "mainPanel" ) );

		GAUtil.logScreen("KidsProfileScreen");
	}

	private void moveIn( UIElement p_element )
	{
		List<Vector3> l_pointListIn = new List<Vector3>();
		l_pointListIn.Add( p_element.transform.localPosition );
		l_pointListIn.Add( p_element.transform.localPosition + new Vector3( 0, 800, 0 ));
		p_element.tweener.addPositionTrack( l_pointListIn, 0f );
	}

	private void moveOut( UIElement p_element )
	{
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( p_element.transform.localPosition );
		l_pointListOut.Add( p_element.transform.localPosition - new Vector3( 0, 800, 0 ));
		p_element.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	private void _createMainView( GameController p_gameController )
	{
		m_editPictureCanvas = m_uiManager.createScreen( UIScreen.SELECT_AVATAR, false, 3 );
		m_profileActivityCanvas = m_uiManager.createScreen( UIScreen.PROFILE_ACTIVE, false, 2) 	as ProfileActivityCanvas;
		m_kidsProfileCanvas = m_uiManager.createScreen( UIScreen.KIDS_PROFILE, true, 0 ) 		as KidsProfileCanvas;
	}

	private void _setupElements()
	{
		moveOut( m_editPictureCanvas.getView( "mainPanel" ) );

		m_backButton = m_kidsProfileCanvas.getView( "backButton" ) as UIButton;
		m_backButton.addClickCallback( onBackButtonClick );
		
		m_editPhotoButton = m_kidsProfileCanvas.getView( "editPhotoButton" ) as UIButton;
		m_editPhotoButton.addClickCallback( onEditPhotoButtonClick );
		//TODO load data

		m_saveButton = m_editPictureCanvas.getView( "saveButton" ) as UIButton;
		m_saveButton.addClickCallback( onSaveClick );

		m_editBackButton = m_editPictureCanvas.getView( "backButton" ) as UIButton;
		m_editBackButton.addClickCallback( onEditBackClick );

		m_avatarSwipeList = m_editPictureCanvas.getView( "avatarSwipeList" ) as UISwipeList;
		m_avatarSwipeList.addClickListener( "Prototype", onClickAvatarBtn );

		m_avatarImage = m_kidsProfileCanvas.getView( "avatarIcon" ) as UIImage;

		m_infoSwipeList = m_profileActivityCanvas.getView ("profileSwipeList") as UISwipeList;
		m_infoSwipeList.active = false;
	}

	private void onClickAvatarBtn(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		
		if (-1 != m_lastIndex) 
		{
			(p_list.getData()[m_lastIndex] as AvatarButton).isSelected = false;
		}
		m_lastIndex = p_index;
		(p_data as AvatarButton).isSelected = true;
		
		m_avatarImagePath = (p_data as AvatarButton).name;
	}

	private void onSaveClick( UIButton p_button )
	{
		p_button.removeClickCallback (onSaveClick);
		SessionHandler.getInstance ().selectAvatar = m_avatarImagePath;
		if(null == SessionHandler.getInstance().selectAvatar)
		{
			SessionHandler.getInstance().selectAvatar = "icon_avatar_gen";
		}
		
		string l_url = "@absolute:";
		if(Application.platform == RuntimePlatform.Android)
		{
			l_url += "jar:file://"+Application.dataPath+"!/assets/"+ SessionHandler.getInstance().selectAvatar + ".png";
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			l_url += Application.dataPath + "/Raw/" + SessionHandler.getInstance().selectAvatar + ".png";
		}
		else
		{
			l_url += "file://" + Application.dataPath + "/StreamingAssets/" + SessionHandler.getInstance().selectAvatar + ".png";
		}

		RequestQueue l_queue = new RequestQueue();
		l_queue.add(new ImageRequest("newAvatar", l_url));
		l_queue.add(new UpdatePhotoRequest("newAvatar", onUpdateFinish ));
		l_queue.request(RequestType.SEQUENCE);
		
		moveOut( m_editPictureCanvas.getView( "mainPanel" ) );
		moveIn( m_kidsProfileCanvas.getView( "messagePanel" ) );
	}

	private void onUpdateFinish(WWW p_response)
	{
		Kid l_kid = SessionHandler.getInstance ().currentKid;
		
		l_kid.kid_photo = Resources.Load("GUI/2048/common/avatars/" + SessionHandler.getInstance().selectAvatar) as Texture2D;

		foreach( Kid l_kidData in SessionHandler.getInstance().kidList )
		{
			if( l_kidData.id == l_kid.id )
			{
				l_kidData.kid_photo = l_kid.kid_photo;
			}
		}
		ImageCache.saveCacheImage(SessionHandler.getInstance().selectAvatar, l_kid.kid_photo);//cynthia

		SessionHandler.getInstance ().currentKid = l_kid;
		
		if(null != m_avatarImage)
			m_avatarImage.setTexture ( l_kid.kid_photo );
		SessionHandler.getInstance ().selectAvatar = null;
		
		moveOut( m_kidsProfileCanvas.getView( "messagePanel" ) );
		moveIn( m_profileActivityCanvas.getView( "panel" ) );
		moveIn( m_kidsProfileCanvas.getView( "mainPanel" ) );

		m_saveButton.addClickCallback( onSaveClick );
	}

	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		UICanvas p_canvas = p_element as UICanvas;
		p_canvas.isTransitioning = false;
	}

	private UIManager m_uiManager;

	private UICanvas m_editPictureCanvas;
	private KidsProfileCanvas m_kidsProfileCanvas;
	private ProfileActivityCanvas m_profileActivityCanvas;

	private UIButton m_backButton;
	private UIButton m_editPhotoButton;
	private UIButton m_saveButton;
	private UIButton m_editBackButton;
	private int		 m_lastIndex = -1;

	private UIImage m_avatarImage;

	private UISwipeList m_avatarSwipeList;
	private UISwipeList m_infoSwipeList;

	private bool m_gotoBack;

	private string m_avatarImagePath;
}
