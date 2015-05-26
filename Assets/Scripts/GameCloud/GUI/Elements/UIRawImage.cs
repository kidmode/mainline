using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIRawImage : UIElement 
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);

		m_image = p_gameObject.GetComponent<RawImage>();
		baseElement = m_image;
		m_alpha = m_image.color.a;
	}


	public void loadTexture(string p_path)
	{
		texture = Resources.Load(p_path) as Texture2D;
		m_resourceTexture = texture;
	}
	
	



	public override void dispose(bool p_deep)
	{
		base.dispose( p_deep );

		if (null != m_resourceTexture)
		{
			Resources.UnloadAsset(m_resourceTexture);
			m_resourceTexture = null;
		}
	}

	public override float alpha
	{
		get { return m_image.color.a; }
		set 
		{ 
			if (value != base.alpha)
			{
				base.alpha 		= value;
				Color l_color 	= m_image.color;
				l_color.a 		= value;

				m_image.color 	= l_color;
				m_image.SetAllDirty();
			}
		}
	}

	public Color color
	{
		get { return m_image.color; }
		set 
		{ 
			m_image.color = value; 
			m_image.SetAllDirty();	
		}
	}
    
    public Texture2D    texture
    {
        get { return m_image.texture as Texture2D;   }
        set 
        { 
            m_image.texture = value;
            m_image.SetAllDirty();
        }
    }

	private RawImage m_image;
	private Texture2D m_resourceTexture;
}
