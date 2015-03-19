using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignInCacheState : GameState
{
	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);

		m_queue = new RequestQueue();
		m_queue.add(new ClientIdRequest(getclientIdComplete));
		m_queue.add(new GetUserSettingRequest(checkError));

		LocalSetting l_setting = LocalSetting.find("ServerSetting");
		if (!l_setting.hasKey(ZoodlesConstants.ZPS_LEVEL))
			m_queue.add(new GetLevelsInfoRequest(checkError));
		if (!l_setting.hasKey(ZoodlesConstants.EXPERIENCE_POINTS))
			m_queue.add(new GetExperiencePointsInfoRequest(checkError));
		if (!l_setting.hasKey(ZoodlesConstants.CATEGORIES))
			m_queue.add(new GetCategoriesInfoRequest(checkError));
		if (!l_setting.hasKey(ZoodlesConstants.TAGS))
			m_queue.add(new GetTagsInfoRequest(checkError));
		if (!l_setting.hasKey(ZoodlesConstants.SUBJECTS))
			m_queue.add(new GetSubjectsInfoRequest(checkError));

		m_queue.add(new GetKidListRequest(onGetKidsComplete));
		m_queue.request(RequestType.SEQUENCE);

		p_gameController.getUI().createScreen(UIScreen.LOADING_SPINNER);
	}

	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		if (m_queue.isCompleted())
		{
			if (SessionHandler.getInstance().hasPin)
			{
				if (LocalSetting.find("User").getBool("UserTry",true))
				{
					if (SessionHandler.getInstance().kidList.Count > 0)
					{
						p_gameController.changeState(ZoodleState.PROFILE_SELECTION);
					}
					else
					{
						p_gameController.changeState(ZoodleState.CREATE_CHILD);
					}
				}
				else
				{
					p_gameController.changeState(ZoodleState.SIGN_UP_UPSELL);
				}
			}
			else
			{
				p_gameController.changeState(ZoodleState.SET_BIRTHYEAR);
			}
		}
		if (m_toSignInState) 
		{
			m_toSignInState = false;
			p_gameController.changeState(ZoodleState.SIGN_IN);
		}
	}

	private void onGetKidsComplete(WWW p_response)
	{
		List<Kid> l_kidList = new List<Kid>();
		ArrayList l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as ArrayList;
		foreach(object o in l_data)
		{
			Kid l_kid = new Kid( o as Hashtable );
			l_kid.requestPhoto();
			l_kidList.Add( l_kid );
		}
		
		SessionHandler.getInstance().kidList = l_kidList;
		if (l_kidList.Count > 0)
			SessionHandler.getInstance().currentKid = l_kidList[0];
	}

	private void checkError(WWW p_response)
	{
		if(null != p_response.error)
		{
			m_queue.reset();
			SessionHandler.getInstance().SignInFail = true;
			m_toSignInState = true;
		}
	}

	private void getclientIdComplete(WWW p_response)
	{
		if(p_response.error == null)
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			SessionHandler.getInstance ().clientId = l_data.ContainsKey("id") ? double.Parse(l_data["id"].ToString()) : -1;
		}
		else
		{
			m_queue.reset();
			SessionHandler.getInstance().SignInFail = true;
			m_toSignInState = true;
		}
	}

	public override void exit(GameController p_gameController)
	{
		p_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER);

		base.exit(p_gameController);
	}

	private RequestQueue m_queue = null;
	private bool 		 m_toSignInState = false;
}

