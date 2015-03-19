using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ProfileInfo : System.Object
{
	public const int ADD_PROFILE_CODE = -666;
	
	public string name;
	public int level;
	public int gems;
	public int stars;
	public Texture2D m_avatar;
	
	public ProfileInfo( string p_name, int p_level, int p_gems, int p_stars,Texture2D p_avatar )
	{
		name 	= p_name;
		level	= p_level;
		gems 	= p_gems;
		stars	= p_stars;
		m_avatar = p_avatar;
	}
	
}

public class User : Object
{
	public Kid  currentKid;

    public List<Kid>    kidList = new List<Kid>();

	private int m_level;
}
