using System;
[Serializable]
public class PwdStore
{
	public PwdStore ()
	{

	}

	public string cerPwd
	{ 
		get
		{
			return p_cerPwd;
		} 
		set
		{
			p_cerPwd = value;
		}
	
	}
	public string encPwd 
	{
		get
		{
			return p_encPwd;
		} 
		set
		{
			p_encPwd = value;
		}
	}

	private string p_cerPwd = "";
	private string p_encPwd = "";
}


