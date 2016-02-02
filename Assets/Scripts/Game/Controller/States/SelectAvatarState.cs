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

		if (m_queue == null)
			m_queue = new RequestQueue();
		else
			m_queue.reset();

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
			if( SessionHandler.getInstance().CreateChild )
			{
				p_gameController.changeState(ZoodleState.CREATE_CHILD_NEW);				
			}
			else
			{
				p_gameController.changeState(ZoodleState.CREATE_CHILD);
			}
			gotoPrevious = false;
		}

		if (m_queue.isCompleted())
		{
			if(SessionHandler.getInstance().CreateChild)
			{
				m_gameController.changeState(ZoodleState.PROFILE_VIEW);
				m_gameController.game.removePDMenuBar();
				if(SessionHandler.getInstance().kidList.Count == 1)
				{
					SessionHandler.getInstance().getAllKidApplist();
					if( null == SessionHandler.getInstance().currentKid )
					{
						SessionHandler.getInstance().currentKid = SessionHandler.getInstance().kidList[0];
					}
					SessionHandler.getInstance().getBooklist();
				}
				SessionHandler.getInstance().CreateChild = false;
			}
			else
			{
				m_gameController.changeState(ZoodleState.OVERVIEW_INFO);
				m_gameController.game.setPDMenuBarVisible(true, true);
			}
		}

		//Image request
		if(null != m_imageWWW && m_imageWWW.isDone)
		{
			loadImageComplete(m_imageWWW);
			m_imageWWW = null;
		}
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		m_lastIndex = -1;
		p_gameController.getUI().removeScreen( UIScreen.SELECT_AVATAR );
		p_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);		
	}
	
	
	//---------------- Private Implementation ----------------------
	
	private void _setupScreen( UIManager p_uiManager )
	{
		m_selectAvatarCanvas = p_uiManager.createScreen( UIScreen.SELECT_AVATAR, false, 1 );

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
		if (Application.internetReachability == NetworkReachability.NotReachable 
		    || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected())
		{
			m_gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
			return;
		}
		if( !SessionHandler.getInstance ().CreateChild )
		{
			SessionHandler.getInstance ().selectAvatar = m_avatarImgPath;
			m_gameController.connectState(ZoodleState.CREATE_CHILD,int.Parse(m_gameController.stateName));
			m_game.gameController.changeState(ZoodleState.CREATE_CHILD);
			SwrveComponent.Instance.SDK.NamedEvent("AddChild.end", null);
		}
		else
		{
			p_button.removeClickCallback(toSaveAvatar);
			
			SessionHandler.getInstance ().selectAvatar = m_avatarImgPath;
			string l_url = "@absolute:";
			if (Application.platform == RuntimePlatform.Android)
			{
				l_url += "jar:file://"+Application.dataPath+"!/assets/"+ m_avatarImgPath + ".png";
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				l_url += Application.dataPath + "/Raw/" + m_avatarImgPath + ".png";
			}
			else
			{
				l_url += "file://" + Application.dataPath + "/StreamingAssets/" + m_avatarImgPath + ".png";
			}
			
//			m_queue.add(new ImageRequest("childAvatar", l_url));
//			m_queue.add(new CreateChildRequest( SessionHandler.getInstance().inputedChildName, SessionHandler.getInstance().inputedbirthday, "childAvatar"));
//			m_queue.request(RequestType.SEQUENCE);
			m_imageWWW = new WWW(l_url);
			m_selectAvatarCanvas.active = false;
			m_gameController.getUI().createScreen(UIScreen.LOADING_SPINNER_ELEPHANT, false, 2);
			Dictionary<string,string> payload = new Dictionary<string,string>() { {"Avatar_png", l_url}};
			SwrveComponent.Instance.SDK.NamedEvent("AddChild.end", payload);
		}

	}

	private void loadImageComplete(WWW image)
	{
		byte[] l_bytes = image.bytes;
		image.Dispose();
		CreateChildRequest createChildRequest = new CreateChildRequest(SessionHandler.getInstance().inputedChildName, SessionHandler.getInstance().inputedbirthday,l_bytes);
		createChildRequest.timeoutHandler = serverRequestTimeout;
		m_queue.add(createChildRequest);
		m_queue.request();
	}

	private void serverRequestTimeout()
	{
		gotoPrevious = true;
		m_queue.reset();
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

	private RequestQueue m_queue;

	private WWW 		m_imageWWW;
}
