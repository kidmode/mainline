package com.gamecloudstudios;

import java.io.IOException;
import java.util.Hashtable;
import java.util.Iterator;
import java.util.Map;
import java.util.Map.Entry;

import android.media.AudioManager;
import android.media.MediaPlayer;
import android.util.Log;
import android.webkit.JavascriptInterface;

public class HtmlAudioInterface 
{
	public HtmlAudioInterface()
	{
		m_mediaPlayers = new Hashtable<String, MediaPlayer>();
	}
	
	@JavascriptInterface
	public void playSound(String p_url, String p_isLooping, String p_volume)
	{		
//		Log.v("sound request", p_url + " " + p_volume + " " + p_isLooping);
		boolean l_isLooping = p_isLooping.equals("true");
		float l_volume = Float.parseFloat(p_volume);
		
		if (m_mediaPlayers != null 
				&& m_mediaPlayers.containsKey(p_url))
		{
			playCachedresource(p_url, l_isLooping, l_volume);
		}
		else
		{
		    try 
		    {
		    	streamAndCacheSound(p_url, l_isLooping, l_volume);
			} catch (IllegalArgumentException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (SecurityException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (IllegalStateException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
	}
	
	@JavascriptInterface
	public void loopSound(String sound) {}
	
	@JavascriptInterface
	public void stopSound(String sound) {}
	
	private void streamAndCacheSound(String p_url, boolean p_isLoop, float p_volume) 
			throws IllegalArgumentException, SecurityException, IllegalStateException, IOException
	{
		MediaPlayer l_player = new MediaPlayer();
		l_player.setAudioStreamType(AudioManager.STREAM_MUSIC);
    	l_player.setDataSource(p_url);
    	l_player.setLooping(p_isLoop);
		l_player.setVolume(p_volume, p_volume);
		
    	l_player.setOnPreparedListener(new MediaPlayer.OnPreparedListener() 
		{
		    @Override
		    public void onPrepared(MediaPlayer p_player) 
		    {
		    	p_player.start();
		    }
		});
    	
    	l_player.prepareAsync();
		m_mediaPlayers.put(p_url, l_player);									
	}
	
	private void playCachedresource(String p_url, boolean p_isLoop, float p_volume)
	{
		MediaPlayer l_player = m_mediaPlayers.get(p_url);
		if (l_player.isPlaying()) l_player.seekTo(0);
		l_player.setLooping(p_isLoop);
		l_player.setVolume(p_volume, p_volume);
		l_player.start();		
	}
	
	public void destroy() 
	{
		//Stop and release all sounds
		Iterator<Entry<String, MediaPlayer>> it = m_mediaPlayers.entrySet().iterator();
	    while (it.hasNext()) 
	    {
	        Map.Entry<String, MediaPlayer> pairs = (Map.Entry<String, MediaPlayer>)it.next();
	        MediaPlayer l_player = pairs.getValue();
	        l_player.stop();
	        l_player.release();
	        it.remove();
	    }
	    
	    //Release all references to media players
	    m_mediaPlayers = new Hashtable<String, MediaPlayer>();
	}
	
	private Hashtable<String, MediaPlayer> m_mediaPlayers;
}