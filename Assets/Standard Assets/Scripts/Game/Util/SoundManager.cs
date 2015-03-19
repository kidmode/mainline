using UnityEngine;
using System.Collections;

public class SoundManager : object 
{
	//private static string SOUND_PREFERENCE_KEY = "SoundManagerPref";
	//private static string SOUND_SOUND_PREF_KEY = "soundsMuted";
	//private static string SOUND_MUSIC_PREF_KEY = "musicMuted";
	
	private AudioSource m_audioSource;
	private AudioSource m_mainTheme;
	private bool m_isMutedForMusic;
	
	static public SoundManager getInstance()
	{
		if (null == s_instance)
		{
			s_instance = new SoundManager();
		}
		
		return s_instance;
	}
	
	private static SoundManager s_instance = null;		
	
	public SoundManager()
	{			
		/*if (null == p_priv)
		{
			throw new Error("SoundManager is a Singleton class. Use getInstance() to retrieve an existing instance.");
		}
		m_soundList = new Object();
		var l_cookie:SharedObject = SharedObject.getLocal(SOUND_PREFERENCE_KEY);
		
		m_musicMuted = loadPreference(SOUND_MUSIC_PREF_KEY, false);
		m_soundsMuted = loadPreference(SOUND_SOUND_PREF_KEY, false);*/
		
		m_musicMuted = false;
		m_soundsMuted = false;
		m_isMutedForMusic = false;
	}
	
	public void updateSystemSound()
	{
		if (GCSPlugin.IsMusicPlaying())
		{
			if (m_isMutedForMusic == false)
			{
				m_isMutedForMusic = true;
				musicVolume = m_musicVolume; //use setter to disable sound  on audio clip but retain value.
			}
		}
		else
		{
			if (m_isMutedForMusic)
			{
				m_isMutedForMusic = false;
				musicVolume = m_musicVolume; // Use setter to enable sound on audio clip.
			}
		}
	}
	
	public void stopMusic()
	{
		GameObject l_gameObject = GameObject.FindGameObjectWithTag("AudioSource");
		AudioSource l_audioSource = l_gameObject.audio;
		l_audioSource.Stop();
	}

	public AudioSource play(string p_name, double p_offset, 
		int p_loops, string p_channelName, object p_transform, bool p_isMusic)
	{
		AudioClip l_clip = Resources.Load("Sounds/" + p_name) as AudioClip;
		AudioSource l_audioSource = null;

		if (l_clip == null)
		{
		 // Debug.Log(p_name + " not found");
		  return null;
		}
		
		if (p_isMusic == false)
		{			
			if (m_isMutedForMusic)
			{
		 		AudioSource.PlayClipAtPoint(l_clip, new Vector3 (5, 100, 2), m_musicVolume); 
			}
			else
			{
				AudioSource.PlayClipAtPoint(l_clip, new Vector3 (5, 100, 2), m_effectVolume); 
			}
 		}
 		else
 		{ 		
			GameObject l_gameObject = GameObject.FindGameObjectWithTag("AudioSource");
			l_audioSource = l_gameObject.audio;
			
			if (l_audioSource.clip != l_clip
				|| l_audioSource.isPlaying == false)
			{
				l_audioSource.clip = l_clip;
				l_audioSource.volume = m_musicVolume;
				// set loop
				l_audioSource.loop = (p_loops == -1) ? false : true;
				l_audioSource.Play();
			}
		}
		
 		return l_audioSource;
		//AudioSource.PlayClipAtPoint(clip, Vector3(0, 0));
		
		/*var l_soundObject:SoundManagerObject;
		var l_nameLookup:String;
		
		l_nameLookup = createLookupName(p_name, p_channelName);
			
		l_soundObject = m_soundList[l_nameLookup];
		
		if (null == l_soundObject)
		{
			var l_assetMgr:GOAssetManager = GOAssetManager.createInstance();
			var l_sound:Sound = (l_assetMgr.getAsset(p_name) as Sound);

			// could not find the Sound file, so return a 'null' channel
			if (null == l_sound)
			{
				trace("WARNING: Failed to find sound named '" + p_name + "'");
				return null;
			}
				
			l_soundObject = new SoundManagerObject(p_name, l_sound, p_channelName, p_isMusic);
			// save off sound object into cache
			m_soundList[l_nameLookup] = l_soundObject;
		}
		
		var l_channel:SoundChannel = l_soundObject.play(p_offset, p_loops, p_transform);
		
		if ((m_soundsMuted && (false == p_isMusic)) 
			|| (m_musicMuted && p_isMusic))
		{
			mute(p_name, p_channelName);
		}
	
		var l_ref:SoundReference = new SoundReference(l_soundObject);
		
		return l_ref;*/
	}
	
	public float audioTime()
	{
		GameObject l_gameObject = GameObject.FindGameObjectWithTag("AudioSource");
		AudioSource l_audioSource = l_gameObject.audio;
			
		if (l_audioSource.isPlaying)
		{
			return l_audioSource.time;
		}
		
		return 0;
	}
	
	public AudioSource play(string p_name, double p_offset, 
		int p_loops, string p_channelName, object p_transform)
	{
		return play(p_name, p_offset, p_loops, p_channelName, p_transform, false);
	}

	public AudioSource play(string p_name, double p_offset, int p_loops)
	{
		return play(p_name, p_offset, p_loops, null, null, false);
	}

	public AudioSource play(string p_name, double p_offset)
	{
		return play(p_name, p_offset, 0, null, null, false);
	}
	
	public AudioSource play(string p_name)
	{
		return play(p_name, 0, 0, null, null, false);
	}
	
	public AudioSource playMainTheme(string p_name, double p_offset, 
		int p_loops, string p_channelName, object p_transform, bool p_isMusic)
	{
		m_mainTheme = play(p_name, p_offset, p_loops, p_channelName, p_transform, p_isMusic );
		return m_mainTheme;
	}
	
	public bool isMainThemePlaying()
	{
		if ( m_mainTheme != null && m_mainTheme.isPlaying )
			return true;
		else
			return false; 
	}
	
	public void setMainThemeLoop(bool p_loop)
	{
		if ( m_mainTheme != null )
			m_mainTheme.loop = p_loop;
	}
	
	
	
	
	public void stop(string p_name, string p_channelName)
	{
		/*if (null == m_soundList)
			return;			
		
		if (null == p_name)
		{
			// if no name specified, stop all sounds and clear
			// all out as well (possibly switching levels, games, etc)
			for (var i:String in m_soundList)
			{
				m_soundList[i].stop();
				m_soundList[i].destroy();
				delete m_soundList[i];
			}
			return;
		}			

		var l_nameLookup:String = createLookupName(p_name, p_channelName);			
		
		var l_sndObject:SoundManagerObject = m_soundList[l_nameLookup];
		if (null != l_sndObject)
		{
			l_sndObject.stop();
		}*/
	}
	
	public void stop()
	{
		stop(null, null);
	}
	
	public void stop(string p_name)
	{
		stop(p_name, null);
	}
	
	public AudioSource mainTheme
	{
		get
		{
			return m_mainTheme;
		}
	}
	
	public bool soundsMuted
	{
		get
		{
			return m_soundsMuted;
		}
	}
	
	public bool musicMuted
	{
		get
		{
			return m_musicMuted;
		}
	}
	
	public float effectVolume
	{
		get
		{
			return m_effectVolume;
		}
		set
		{
			if (value >= 1.0)
			{
				value = 1.0f;
			}
			else if(value <= 0)
			{
				value = 0.0f;
			}
			m_effectVolume = value;	
		}
	}
	
	public float musicVolume
	{
		get
		{
			return m_musicVolume;
		}
		set
		{
			if (value >= 1.0)
			{
				value = 1.0f;
			}
			else if(value <= 0)
			{
				value = 0.0f;
			}
			m_musicVolume = value;	
			GameObject l_gameObject = GameObject.FindGameObjectWithTag("AudioSource");
			AudioSource l_audioSource = l_gameObject.audio;
			if (l_audioSource.clip != null && l_audioSource.isPlaying == true)
			{
				if (m_isMutedForMusic)
				{
					l_audioSource.volume = 0.0f;
				}
				else
				{
					l_audioSource.volume = m_musicVolume;
				}
			}
		}
	}

	public float masterVolume
	{
		get
		{
			return m_masterVolume;
		}
		set
		{
			if (value >= 1.0)
			{
				value = 1.0f;
			}
			else if(value <= 0)
			{
				value = 0.0f;
			}
			m_masterVolume = value;
			musicVolume = m_musicVolume * m_masterVolume;
			effectVolume = m_effectVolume * m_masterVolume;
		}
	}
	
	
	/*private function savePreference(p_key:String, p_value:boolean):void
	{
		/*var l_cookie:SharedObject = SharedObject.getLocal(SOUND_PREFERENCE_KEY);
		if ((null != l_cookie) && (null != l_cookie.data))
		{
			l_cookie.data[p_key] = p_value;
		}	*/		
	//}*/
	
	/*private function loadPreference(p_key:String, p_defaultValue:boolean):boolean
	{
		/*var l_cookie:SharedObject = SharedObject.getLocal(SOUND_PREFERENCE_KEY);
		if (l_cookie.data[p_key] != undefined)
		{
			return l_cookie.data[p_key];
		}	
		
		return p_defaultValue;*/
		//return false;
	//}
	
	/*public function set soundsMuted(value:boolean)
	{
		/*var l_sndObject:SoundManagerObject = null;

		if (m_soundsMuted == p_val)
			return;
		
		m_soundsMuted = p_val;
		
		savePreference(SOUND_SOUND_PREF_KEY, p_val);
		
		for (var i:String in m_soundList)
		{
			l_sndObject = m_soundList[i];
			
			if (false == l_sndObject.isMusic)
			{
				l_sndObject.mute(m_soundsMuted);
			}
		}	*/		
	//}*/

	/*public function set musicMuted(value:Boolean)
	{
		/*var l_sndObject:SoundManagerObject = null;

		if (m_musicMuted == p_val)
			return;
		
		m_musicMuted = p_val;
		
		savePreference(SOUND_MUSIC_PREF_KEY, p_val);
		
		for (var i:String in m_soundList)
		{
			l_sndObject = m_soundList[i];
			
			if (true == l_sndObject.isMusic)
			{
				l_sndObject.mute(m_musicMuted);
			}
		}	*/		
	//}		
	
	//private function mute(p_name:String):void
	//{
	//	mute(p_name, null);	
	//}
	
	//private function mute():void
	//{
	//	mute(null, null);
	//}
	
	//private function mute(p_name:String, p_channelName:String):void
	//{
		/*var l_sndObject:SoundManagerObject = null;
		
		if (null != p_name)
		{				
			var l_lookupName:String = createLookupName(p_name, p_channelName);
			l_sndObject = m_soundList[l_lookupName];
			
			if (null != l_sndObject)
			{
				if (l_sndObject.isMusic)
					l_sndObject.mute(m_musicMuted);
				else
					l_sndObject.mute(m_soundsMuted);
					
				if (false == l_sndObject.muted)
				{
					if (l_sndObject.isMusic)
						m_musicMuted = false;
					else
						m_soundsMuted = false;
				}
			}
		}
		else
		{
			m_musicMuted = !m_musicMuted;
			m_soundsMuted = !m_soundsMuted;
			
			for (var i:String in m_soundList)
			{
				l_sndObject = m_soundList[i];
				
				var l_result:Boolean = (l_sndObject.isMusic ? m_musicMuted : m_soundsMuted);
				l_sndObject.mute(l_result);
			}
		}*/
	//}
	
	//private function createLookupName(p_name:String, p_channelName:String):String
	//{
		/*if (null == p_name)
			return null;
			
		if (null != p_channelName)
			return p_name + ":" + p_channelName;
		
		return p_name;*/
	//}
	
	//private var m_allMuted:Boolean;
	private bool m_soundsMuted;
	private bool m_musicMuted;
	private object m_soundList;
	
	private float m_effectVolume = 0.5f;
	private float m_musicVolume = 0.5f;
	private float m_masterVolume = 0.5f;
}	