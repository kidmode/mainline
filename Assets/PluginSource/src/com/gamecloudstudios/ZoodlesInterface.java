package com.gamecloudstudios;

import android.webkit.JavascriptInterface;

import com.unity3d.player.UnityPlayer;

public class ZoodlesInterface 
{	
	@JavascriptInterface
	public String getUser()
	{
		UnityPlayer.UnitySendMessage("SoundBridge", "getUser", "");
		while (m_user == null) { } //wait for thread
		return m_user;
	}
	
	@JavascriptInterface
	public String getToken()
	{
		UnityPlayer.UnitySendMessage("SoundBridge", "getToken", "");
		while (m_token == null) { } //wait for thread
		return m_token;
	}
	
	public void setUser(String p_user)
	{
		m_user = p_user;
	}
	
	public void setToken(String p_token)
	{
		m_token = p_token;
	}
	
	public void destroy()
	{
		
	}
		
	private String m_user = null;
	private String m_token = null;
	
}
