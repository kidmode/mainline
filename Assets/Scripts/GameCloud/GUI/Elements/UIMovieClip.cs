using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIMovieClip: UIElement 
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		m_movieclip = p_gameObject.GetComponent<GAFMovieClip>();
		this.alpha = 1.0f;
	}

	public override void dispose(bool p_deep)
	{
		base.dispose( p_deep );
	}

	public void play()
	{
		m_movieclip.play();
	}

	public void stop()
	{
		m_movieclip.stop();
	}

	public void gotoFrame(uint p_frame)
	{
		if (m_movieclip.isPlaying())
		{
			m_movieclip.gotoAndPlay(p_frame);
		}
		else
		{
			m_movieclip.gotoAndStop(p_frame);
		}
	}

	public void setSequence(string p_name, bool p_playImmediately)
	{
		m_movieclip.setSequence(p_name, p_playImmediately);
	}

	public float duration()
	{
		return m_movieclip.duration();
	}

	public override float alpha 
	{
		get 
		{
			return base.alpha;
		}
		set 
		{
			base.alpha = value;

			List<Material> l_materials = m_movieclip.resource.materials;
			int l_numMaterials = l_materials.Count;
			for (int i = 0; i < l_numMaterials; ++i)
			{
				Material l_material = l_materials[i];
				l_material.SetFloat("_Alpha", value);
			}
		}
	}

	public bool loop
	{
		get 
		{ 
			return m_movieclip.getAnimationWrapMode() == GAFWrapMode.Loop; 
		}
		set 
		{ 
			if (value)
				m_movieclip.setAnimationWrapMode(GAFWrapMode.Loop);
			else
				m_movieclip.setAnimationWrapMode(GAFWrapMode.Once);
		}
	}
	
	private GAFMovieClip m_movieclip;
}
