using UnityEngine;
using System.Collections;
/**
 * Abstraction for the token associated with a login.
 */
public class Token
{
    public Token()
    {
        m_hash      = null;
        m_premium   = false;
        m_vpc       = ZoodlesConstants.USER_VPC_NONE;
		m_try 		= false;
		m_current 	= false;
		_read();
    }

	public void setToken( string p_token )
	{
		m_token = p_token;
		m_hash = null;
		m_premium = false;
		m_vpc = ZoodlesConstants.USER_VPC_NONE;
		m_try 		= false;
		m_current 	= false;
		_write();
	}

	public void setToken( string p_token, bool p_premium,bool p_try, bool p_current, int p_vpc )
	{
		m_token = p_token;
		m_hash = null;
		m_premium = p_premium;
		m_vpc = p_vpc;
		m_try = p_try;
		m_current 	= p_current;
		_write();
	}

	public void clear()
	{
		m_token = "";
		m_hash = null;
		m_premium = false;
		m_vpc = 0;
		m_current 	= false;

		_clear();
	}
	
    public string getSecret()
    {
        return m_token;
    }

	public bool isExist()
	{
		if (null != m_token)
		{
			return m_token != "";
		}

		return false;
	}

    /**
     * Is the user free premium.
     * @return
     */
    public bool isPremium()
    {
        return m_premium;
    }

	/**
     * Is the user hava tried.
     * @return
     */
	public bool isTried()
	{
		return m_try;
	}

	/**
     * Is the user hava tried.
     * @return
     */
	public bool isCurrent()
	{
		return m_current;
	}

    /**
     * Have we obtained verified parental consent.
     * @return
     */
    public int getVPC()
    {
        return m_vpc;
    }

    public void setVPC( int p_vpc)
    {
        m_vpc = p_vpc;
    }

	public string getHash()
	{
		return m_hash;
	}

	public void setTry(bool p_bool)
	{
		m_try = p_bool;
		_write ();
	}

	public void setCurrent(bool p_bool)
	{
		m_current = p_bool;
		_write ();
	}

	public void setPremium(bool p_bool)
	{
		m_premium = p_bool;
		_write ();
	}

 //  /**
 //   * Returns an MD5 hash of the login token.
 //   * 
 //   * @return
 //   */
 //  public string getMD5Hash()
 //  {
 //      if (TextUtils.isEmpty(token)) return null;
 //
 //      if (hash == null)
 //      {
 //          hash = MD5.getHash(token);
 //      }
 //
 //      return hash;
 //  }

	private void _read()
	{
		LocalSetting l_setting = LocalSetting.find( "User" );
		m_token = l_setting.getString( ZoodlesConstants.USER_TOKEN, "" );
		m_premium = l_setting.getBool( ZoodlesConstants.USER_PREMIUM, false );
		m_try = l_setting.getBool( ZoodlesConstants.USER_TRY, false );
		m_current = l_setting.getBool( ZoodlesConstants.USER_CURRENT, false );
	}

	private void _write()
	{
		LocalSetting l_setting = LocalSetting.find( "User" );
		l_setting.setString( ZoodlesConstants.USER_TOKEN, m_token );
		l_setting.setBool( ZoodlesConstants.USER_PREMIUM, m_premium );
		l_setting.setBool( ZoodlesConstants.USER_TRY, m_try );
		l_setting.setBool( ZoodlesConstants.USER_CURRENT, m_current );
	}

	private void _clear()
	{
		LocalSetting l_setting = LocalSetting.find( "User" );
		l_setting.setString( ZoodlesConstants.USER_TOKEN, "" );
		l_setting.setBool( ZoodlesConstants.USER_PREMIUM, false );
		l_setting.setBool( ZoodlesConstants.USER_TRY, false );
		l_setting.setBool( ZoodlesConstants.USER_CURRENT, false );
	}

    private string m_token;
    private string m_hash;

    private bool m_premium;
	private bool m_current;
	private bool m_try;
    private int m_vpc;			// see User model object for constants
}
