using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TryTrialState : GameState
{
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );

		m_response = null;
		Hashtable l_params = new Hashtable();
		l_params[ZoodlesConstants.PARAM_TOKEN] = SessionHandler.getInstance().token.getSecret();
		Server.request("",  ZoodlesConstants.REST_TRY_TRIAL, l_params, CallMethod.POST, _requestComplete ); 
	}

	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );

		if(null != m_response)
		{
			p_gameController.changeState(ZoodleState.UPSELL_CONGRATURATION);
		}
	}

	private void _requestComplete(string status, HttpsWWW p_www)
	{
		if(null != p_www.error)
		{
			return ;
		}
		else
		{
			SessionHandler.getInstance().token.setTry(true);
			SessionHandler.getInstance().token.setCurrent(true);
			m_response = p_www.text;
		}
	}

	private string m_response = null;
}

