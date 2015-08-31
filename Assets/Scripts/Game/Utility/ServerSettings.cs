using System;
using System.Collections;
using System.Collections.Generic;

public class ServerSettings
{
	private ServerSettings ()
	{
		m_zpLevels = new ZpLevels();
		m_experienceTable = new ExperienceTable();
	}

	public ZpLevels levels { get { return m_zpLevels; }	}
	public ExperienceTable experience { get { return m_experienceTable; } }

	private ZpLevels m_zpLevels;
	private ExperienceTable m_experienceTable;

	public static ServerSettings getInstance()
	{
		if (s_instance == null)
			s_instance = new ServerSettings();

		return s_instance;
	}

	private static ServerSettings s_instance = null;
}

public class ZpLevels
{
	public ZpLevels()
	{}

	public int getLevelPoints(int p_level)
	{
		_init();
		
		//===============================
		//===============================
		//Kevin 
		//Added to avoid bug when p_level is greater and equal to 102
		if( m_table["101"] == null){
			
			return 153;
			
		}
		
		if(m_table[p_level.ToString()] == null)
			return int.Parse(m_table["101"].ToString());
		//===============================End Bug avoid //===============================//===============================
		
		return int.Parse(m_table[p_level.ToString()].ToString());
	}


	private void _init()
	{
		if (m_inited)
			return;

		LocalSetting l_setting = LocalSetting.find("ServerSetting");
		string l_data = l_setting.getString("level_zps", "{}");
		m_table = MiniJSON.MiniJSON.jsonDecode(l_data) as Hashtable;

		m_inited = true;
	}

	private Hashtable m_table = null;
	private bool m_inited = false;
}

public class ExperienceTable
{
	public const string SETUP_KID_PROFILE	= "setup_a_kid_profile";
	public const string COMPLETE_BOOK		= "completing_a_book";
	public const string COMPLETE_GAME		= "completing_a_mini_game";
	public const string WATCH_VIDEO			= "watching_a_video";
	public const string PLAY_GAME			= "playing_a_game";
	public const string USE_ARTSTUDIO		= "using_Art_Studio";
	public const string RECORD_AUDIO		= "recording_audio_for_book";

	public ExperienceTable()
	{}

	public int getZPs(string p_type)
	{
		return int.Parse(m_table[p_type].ToString());
	}

	private void _init()
	{
		if (m_inited)
			return;
		
		LocalSetting l_setting = LocalSetting.find("ServerSetting");
		string l_data = l_setting.getString("experience_points", "{}");
		m_table = MiniJSON.MiniJSON.jsonDecode(l_data) as Hashtable;
		
		m_inited = true;
	}
	
	private Hashtable m_table = null;
	private bool m_inited = false;
}
