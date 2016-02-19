using UnityEngine;
using System;

public class SettingCache : System.Object
{
	public void init()
	{
		active = false;
	}

	public bool childLockSwitch
	{
		get { return m_childLockSwitch;   }
		set 
		{ 
			active = true;
			m_childLockSwitch = value; 
		}
	}

	public bool verifyBirth
	{
		get{return m_verifyBirth;}
		set
		{
			active = true;
			m_verifyBirth = value;
		}
	}

	public bool freeWeeklyApp
	{
		get{return m_freeWeeklyApp;}
		set
		{
			active = true;
			m_freeWeeklyApp = value;
		}
	}

	public bool smartSelect
	{
		get{return m_smartSelect;}
		set
		{
			active = true;
			m_smartSelect = value;
		}
	}

	public bool newAddApp
	{
		get{return m_newAddApp;}
		set
		{
			m_newAddApp = value;
		}
	}

	public int musicVolum
	{
		get{return m_musicVolum;}
		set
		{
			active = true;
			m_musicVolum = value;
		}
	}

	public int effectsVolum
	{
		get{return m_effectsVolum;}
		set
		{
			active = true;
			m_effectsVolum = value;
		}
	}

	public int masterVolum
	{
		get{return m_masterVolum;}
		set
		{
			active = true;
			m_masterVolum = value;
		}
	}

	public bool allowCall
	{
		get{return m_allowCall;}
		set
		{
			active = true;
			m_allowCall = value;
		}
	}
	
	public bool tip
	{
		get{return m_tip;}
		set
		{
			active = true;
			m_tip = value;
		}
	}

	public string childLockPassword
	{
		get{return m_childLockPassword;}
		set
		{
			active = true;
			m_childLockPassword = value;
		}
	}

	public bool active
	{
		get{return m_enableCache;}
		set
		{

			m_enableCache = value;

			if(onSettingCacheActiveChanged != null)
				onSettingCacheActiveChanged(m_enableCache);

		}
	}

	

	//Event need for device options
	public static event Action<bool> onSettingCacheActiveChanged;  

	private bool m_childLockSwitch;
	private bool m_verifyBirth;
	private bool m_freeWeeklyApp;
	private bool m_smartSelect;
	private bool m_newAddApp;
	private int  m_musicVolum;
	private int  m_effectsVolum;
	private int  m_masterVolum;
	private bool m_allowCall;
	private bool m_tip;
	private string m_childLockPassword;
	private bool m_enableCache = false;
}