using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignInCacheState : GameState
{
	public override void enter(GameController p_gameController)
	{
		base.enter(p_gameController);

		m_loginSuccess = false;
		_setupScreen(p_gameController.getUI());
		{
			CrittercismAndroid.SetUsername(SessionHandler.getInstance().username);

			//	SessionHandler.getInstance ().clientId = 600;
			m_queue = new RequestQueue();
			//		m_queue.add(new ClientIdRequest(getclientIdComplete));
			m_queue.add(new GetUserSettingRequest(checkError));
			//TODO: need to fix this
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
			p_gameController.getUI().createScreen(UIScreen.LOADING_SPINNER_ELEPHANT);
		}

	}

	public override void update(GameController p_gameController, int p_time)
	{
		base.update(p_gameController, p_time);

		if (m_loginSuccess)
		{
			m_loginSuccess = false;
			if (SessionHandler.getInstance().hasPin)
			{
				if (LocalSetting.find("User").getBool("UserTry",true))
				{
					if( SessionHandler.getInstance().token.isCurrent() )
					{
						if( SessionHandler.getInstance().token.isPremium() )
						{
							if (null != SessionHandler.getInstance().kidList && SessionHandler.getInstance().kidList.Count > 0)
							{
								p_gameController.changeState(ZoodleState.PROFILE_SELECTION);
							}
							else
							{
								m_gameController.connectState(ZoodleState.CREATE_CHILD_NEW,int.Parse(m_gameController.stateName));
								p_gameController.changeState(ZoodleState.CREATE_CHILD_NEW);
							}
						}
						else
						{
							//honda: remove spinner
							m_gameController.getUI().removeScreenImmediately(UIScreen.LOADING_SPINNER_ELEPHANT);

							m_queue.reset();
							m_queue.add( new PremiumDetailsRequest( onGetDetailsComplete ) );
							m_queue.request( RequestType.SEQUENCE );
						}
					}
					else
					{
						//honda: remove spinner
						m_gameController.getUI().removeScreenImmediately(UIScreen.LOADING_SPINNER_ELEPHANT);

						m_messageText.text = Localization.getString (Localization.TXT_103_LABEL_EXPIRED);
						m_continueText.text = Localization.getString (Localization.TXT_103_BUTTON_NOTHANKS);
						
						UIElement l_panel = m_trialMessageCanvas.getView( "mainPanel" );
						List<Vector3> l_pointListIn = new List<Vector3>();
						l_pointListIn.Add( l_panel.transform.localPosition );
						l_pointListIn.Add( l_panel.transform.localPosition + new Vector3( 0, 800, 0 ));
						l_panel.tweener.addPositionTrack( l_pointListIn, 0f );
					}
				}
				else
				{
					p_gameController.connectState(ZoodleState.SIGN_IN_UPSELL, int.Parse(p_gameController.stateName));
					p_gameController.changeState(ZoodleState.SIGN_IN_UPSELL);
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

	private void onGetKidsComplete(HttpsWWW p_response)
	{
		if(null == p_response.error)
		{
			string l_string = "";
			
			l_string = UnicodeDecoder.Unicode(p_response.text);
			l_string = UnicodeDecoder.UnicodeToChinese(l_string);
			l_string = UnicodeDecoder.CoverHtmlLabel(l_string);
			
			List<Kid> l_kidList = new List<Kid>();
			ArrayList l_data = MiniJSON.MiniJSON.jsonDecode(l_string) as ArrayList;
			//honda: get kids time left data
			string timeLeftStr = SessionHandler.LoadKidsTimeLeft();
			ArrayList timeLeftList = null;
			if (timeLeftStr != null && timeLeftStr.Length > 0)
				timeLeftList = MiniJSON.MiniJSON.jsonDecode(timeLeftStr) as ArrayList;
			//end
			foreach(object o in l_data)
			{
				Kid l_kid = new Kid( o as Hashtable );
				l_kid.requestPhoto();
				//honda: request time limits info
				l_kid.requestTimeLimits();
				if (timeLeftList != null)
				{
					foreach (object i in timeLeftList)
					{
						Hashtable item = i as Hashtable;
						foreach(string key in item.Keys)
						{
							Debug.Log(key + ": " + item[key]);
						}
						if (l_kid.id == Convert.ToInt32(item["id"]))
						{
							l_kid.timeLeft = Convert.ToInt32(item["timeLeft"]);
							l_kid.lastPlay = item["lastPlay"].ToString();
							break;
						}
					}
				}
				//end
				l_kidList.Add( l_kid );
			}
			
			SessionHandler.getInstance().kidList = l_kidList;
			if (l_kidList.Count > 0)
			{
				SessionHandler.getInstance().getAllKidApplist();
				SessionHandler.getInstance().currentKid = l_kidList[0];
				SessionHandler.getInstance().getBooklist();
			}
			//cynthia
			ArrayList l_list = new ArrayList();
			foreach (Kid k in l_kidList) {
				l_list.Add(k.toHashTable());
			}
			String encodedString = MiniJSON.MiniJSON.jsonEncode(l_list);
			SessionHandler.SaveKidList(encodedString);

			m_loginSuccess = true;
		}
		else
		{
			m_queue.reset();
			SessionHandler.getInstance().SignInFail = true;
			m_toSignInState = true;
		}

	}

	private void checkError(HttpsWWW p_response)
	{
		if(null != p_response.error)
		{
			if (SessionHandler.getInstance().token.isExist()) //cynthia
			{
				m_loginSuccess = true;
			}
			else
			{
				m_queue.reset();
				SessionHandler.getInstance().SignInFail = true;
				m_toSignInState = true;
			}
		}
	}

//	private void getclientIdComplete(HttpsWWW p_response)
//	{
//		if(p_response.error == null)
//		{
//			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
//			SessionHandler.getInstance ().clientId = l_data.ContainsKey("id") ? double.Parse(l_data["id"].ToString()) : -1;
////			SessionHandler.getInstance ().clientId = 600;
//		}
//		else
//		{
//			m_queue.reset();
//			SessionHandler.getInstance().SignInFail = true;
//			m_toSignInState = true;
//		}
//	}

	public override void exit(GameController p_gameController)
	{
		p_gameController.getUI().removeScreen(UIScreen.TRIAL_MESSAGE);
		p_gameController.getUI().removeScreen(UIScreen.LOADING_SPINNER_ELEPHANT);

		base.exit(p_gameController);
	}

	private void _setupScreen(UIManager p_uiManager)
	{
		m_trialMessageCanvas = p_uiManager.createScreen(UIScreen.TRIAL_MESSAGE, false, 2);
		
		m_subscriptionButton = m_trialMessageCanvas.getView ("subscriptionButton") 	as UIButton;
		m_continueButton	 = m_trialMessageCanvas.getView ("continueButton") 		as UIButton;
		m_exitButton		 = m_trialMessageCanvas.getView ("exitButton") 			as UIButton;
		m_messageText		 = m_trialMessageCanvas.getView ("messageText") 		as UILabel;
		m_continueText		 = m_trialMessageCanvas.getView ("continueText") 		as UILabel;
		
		m_subscriptionButton.addClickCallback ( onSubscriptionClick );
		m_continueButton.addClickCallback ( onContinueClick );
		m_exitButton.addClickCallback ( onContinueClick );
	}

	private void onGetDetailsComplete(HttpsWWW p_response)
	{
		if( null == p_response.error )
		{
			Hashtable l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			l_data = (l_data["jsonResponse"] as Hashtable)["response"] as Hashtable;
			
			int l_trialDays = (int)((double)l_data["trial_days"]);

			m_messageText.text = string.Format(Localization.getString (Localization.TXT_103_LABEL_TRIALDAY),l_trialDays);
			m_continueText.text = Localization.getString (Localization.TXT_103_BUTTON_CONTINUE_TRIAL);
			
			UIElement l_panel = m_trialMessageCanvas.getView( "mainPanel" );
			List<Vector3> l_pointListIn = new List<Vector3>();
			l_pointListIn.Add( l_panel.transform.localPosition );
			l_pointListIn.Add( l_panel.transform.localPosition + new Vector3( 0, 800, 0 ));
			l_panel.tweener.addPositionTrack( l_pointListIn, 0f );
		}
	}
	
	private void onSubscriptionClick(UIButton p_button)
	{
		if(string.Empty.Equals(SessionHandler.getInstance().PremiumJson))
		{
			Server.init (ZoodlesConstants.getHttpsHost());
			m_queue.reset ();
			m_queue.add (new GetPlanDetailsRequest(viewPremiumRequestComplete));
			m_queue.request ( RequestType.SEQUENCE );
		}
		else
		{
			m_gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName) );
			m_gameController.changeState( ZoodleState.VIEW_PREMIUM );
		}
	}
	
	private void viewPremiumRequestComplete(HttpsWWW p_response)
	{
		Server.init (ZoodlesConstants.getHttpsHost());
		if(null == p_response.error)
		{
			SessionHandler.getInstance ().PremiumJson = p_response.text;
			m_gameController.connectState( ZoodleState.VIEW_PREMIUM, int.Parse(m_gameController.stateName) );
			m_gameController.changeState( ZoodleState.VIEW_PREMIUM );
		}
		else
		{
			setErrorMessage(m_gameController,Localization.getString(Localization.TXT_STATE_11_FAIL),Localization.getString(Localization.TXT_STATE_11_FAIL_DATA));
		}
	}
	
	private void onContinueClick(UIButton p_button)
	{
		if (null != SessionHandler.getInstance().kidList && SessionHandler.getInstance().kidList.Count > 0)
		{
			m_gameController.changeState(ZoodleState.PROFILE_SELECTION);
		}
		else
		{
			m_gameController.connectState(ZoodleState.CREATE_CHILD_NEW,int.Parse(m_gameController.stateName));
			m_gameController.changeState(ZoodleState.CREATE_CHILD_NEW);
		}
	}

	private RequestQueue m_queue = null;
	private bool 		 m_toSignInState = false;
	private bool		 m_loginSuccess = false;
	
	private UIButton 	m_subscriptionButton;
	private UIButton 	m_continueButton;
	private UIButton 	m_exitButton;
	private UILabel 	m_messageText;
	private UILabel 	m_continueText;
	private UICanvas 	m_trialMessageCanvas;
}

