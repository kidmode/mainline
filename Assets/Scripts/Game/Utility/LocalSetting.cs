//
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class LocalSetting
{
	private LocalSetting( string p_name, bool p_usePrefs )
	{
		m_name = p_name;
		m_usePrefs = p_usePrefs;
		_load();
	}

	public void setString( string p_key, string p_value )
	{
		if( m_usePrefs )
		{
			if( !hasKey( p_key ) || PlayerPrefs.GetString( m_name + p_key ) != p_value )
			{
				PlayerPrefs.SetString( m_name + p_key, p_value );
				m_dirty = true;
			}
		}
		else if( !hasKey( p_key ) || !m_table[p_key].Equals( p_value ) )
		{
			m_table[p_key] = p_value;
			m_dirty = true;
		}

		_save();
	}

	public string getString( string p_key, string p_defaultValue )
	{
		if( m_usePrefs )
			return PlayerPrefs.GetString( m_name + p_key, p_defaultValue );
		else if( m_table.Contains( p_key ) )
			return m_table[p_key].ToString();
		else
		    return p_defaultValue;
	}

	public void setInt( string p_key, int p_value )
	{
		if( m_usePrefs )
		{
			if( !hasKey( p_key ) || PlayerPrefs.GetInt( m_name + p_key ) != p_value )
			{
				PlayerPrefs.SetInt( m_name + p_key, p_value );
				m_dirty = true;
			}
		}
		else if( !hasKey( p_key ) || !m_table[p_key].Equals( p_value.ToString() ) )
		{
			m_table[p_key] = p_value.ToString();
			m_dirty = true;
		}

		_save();
	}

	public int getInt( string p_key, int p_defaultValue )
	{
		if( m_usePrefs )
			return PlayerPrefs.GetInt( m_name + p_key, p_defaultValue );
		else if( m_table.Contains( p_key ) )
			return int.Parse( m_table[p_key].ToString() );
		else
			return p_defaultValue;
	}

	public void setFloat( string p_key, float p_value )
	{
		if( m_usePrefs )
		{
			if( !hasKey( p_key ) || PlayerPrefs.GetFloat( m_name + p_key ) != p_value )
			{
				PlayerPrefs.SetFloat( m_name + p_key, p_value );
				m_dirty = true;
			}
		}
		else if( !hasKey( p_key ) || !m_table[p_key].Equals( p_value.ToString() ) )
		{
			m_table[p_key] = p_value.ToString();
			m_dirty = true;
		}

		_save();
	}

	public float getFloat( string p_key, float p_defaultValue )
	{
		if( m_usePrefs )
			return PlayerPrefs.GetFloat( m_name + p_key, p_defaultValue );
		else if( m_table.Contains( p_key ) )
			return float.Parse( m_table[p_key].ToString() );
		else
			return p_defaultValue;
	}

	public void setBool( string p_key, bool p_value )
	{
		if( m_usePrefs )
		{
			int l_value = ( p_value ? 1 : 0 );
			if( !hasKey( p_key ) || PlayerPrefs.GetInt( m_name + p_key ) != l_value )
			{
				PlayerPrefs.SetInt( m_name + p_key, l_value );
				m_dirty = true;
			}
		}
		else if( !hasKey( p_key ) || !m_table[p_key].Equals( p_value.ToString() ) )
		{
			m_table[p_key] = p_value.ToString();
			m_dirty = true;
		}

		_save();
	}

	public bool getBool( string p_key, bool p_defaultValue )
	{
		if( m_usePrefs )
			return PlayerPrefs.GetInt( m_name + p_key, p_defaultValue ? 1 : 0 ) == 1 ? true : false;
		else if( m_table.Contains( p_key ))
			return bool.Parse( m_table[p_key].ToString() );
		else
			return p_defaultValue;
	}

	public bool hasKey( string p_key )
	{
		if( m_usePrefs )
			return PlayerPrefs.HasKey( m_name + p_key );
		else
			return m_table.Contains( p_key );
	}

	private void _load()
	{
		if( m_usePrefs )
			return;

		string l_path = _path();
		_Debug.log (l_path);
		if( File.Exists( l_path ) )
		{
			StreamReader l_reader = File.OpenText( l_path );
			string l_data = l_reader.ReadToEnd();
			l_reader.Close();
			m_table = MiniJSON.MiniJSON.jsonDecode( l_data ) as Hashtable;
		}
		else
			m_table = new Hashtable();
	}

	private void _save()
	{
		if( !m_dirty )
			return;

		if( m_usePrefs )
			PlayerPrefs.Save();
		else
		{
			FileStream l_stream = null;
			string l_path = _path();
			if( !File.Exists( l_path ) )
				l_stream = File.Create( l_path );
			else
				l_stream = File.OpenWrite( l_path );

			string l_data = MiniJSON.MiniJSON.jsonEncode( m_table );
			byte[] l_bytes = Encoding.UTF8.GetBytes( l_data );
			l_stream.Write( l_bytes, 0, l_bytes.Length );
			l_stream.SetLength( l_bytes.Length );
			l_stream.Close();
		}

		m_dirty = false;
	}

	private string _path()
	{
		return Application.persistentDataPath + "/" + m_name + ".txt";
	}

	public void delete()
	{
		string l_path = _path ();
		if (File.Exists (l_path))
		{
			File.Delete (l_path);
		}
	}

	private string m_name;
	private bool m_usePrefs;
	private Hashtable m_table;
	private bool m_dirty;

	public static LocalSetting find( string p_name, bool p_usePrefs = true )
	{
		if( s_settings.ContainsKey(p_name) == false )
			s_settings.Add( p_name, new LocalSetting( p_name, p_usePrefs ) );
		
		return s_settings[p_name];
	}

	private static Dictionary<string, LocalSetting> s_settings = new Dictionary<string, LocalSetting>();
}

