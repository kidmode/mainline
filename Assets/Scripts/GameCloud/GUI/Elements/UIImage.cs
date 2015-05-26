using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIImage : UIElement 
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		m_image = p_gameObject.GetComponent<Image>();
		m_resourcesSprite = m_image.sprite;
		baseElement = m_image;
		m_alpha = m_image.color.a;
	}

	public void setTexture(string p_path)
	{
		Texture2D l_texture = Resources.Load(p_path) as Texture2D;

		if (l_texture != m_resourcesTexture)
		{
			unloadResources();

			m_resourcesTexture = l_texture;
			setTexture(l_texture);
		}
	}
	
	public void setAndOwnTexture(Texture2D p_texture)
	{
		if (m_texture != p_texture)
		{
			unloadResources();

			Sprite l_sprite = createSprite(p_texture);
			m_image.sprite = l_sprite;
			m_sprite = l_sprite;
			m_texture = p_texture;
		}
	}
	
	public void setTexture(Texture2D p_texture)
	{
		if (null == p_texture)
				return;
		if (null == m_sprite
		    || m_sprite.texture != p_texture)
		{
			disposeOfSprite();

			Sprite l_sprite = createSprite(p_texture);
			m_image.sprite = l_sprite;
			m_sprite = l_sprite;
		}
	}

	public void setSprite(Sprite p_sprite)
	{
		m_image.sprite = p_sprite;
	}

	public override void dispose(bool p_deep)
	{
		base.dispose( p_deep );
		unloadResources();
	}

	private void unloadResources()
	{
		disposeOfResourcesSprite();
		disposeOfResourcesTexture();
		disposeOfTexture();
		disposeOfSprite();
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

	public Image.FillMethod fillMethod
	{
		get { return m_image.fillMethod; 	}
		set 
		{ 
			m_image.fillMethod = value;	
			m_image.SetAllDirty();
		}
	}

	public float fillAmount
	{
		get { return m_image.fillAmount; 	}
		set 
		{ 	
			m_image.fillAmount = value;	
			m_image.SetAllDirty();
		}
	}

	public int fillOrigin
	{
		get { return m_image.fillOrigin; 	}
		set 
		{ 
			m_image.fillOrigin = value;
			m_image.SetAllDirty();	
		}
	}

	public bool fillClockwise
	{
		get { return m_image.fillClockwise; 	}
		set 
		{ 
			m_image.fillClockwise = value;	
			m_image.SetAllDirty();
		}
	}

	public Image.Type imageType
	{
		get { return m_image.type; 	}
		set { m_image.type = value; }
	}

	private Sprite createSprite(Texture2D p_texture)
	{
		Sprite l_sprite = Sprite.Create(p_texture, 
		                                new Rect(0, 0, p_texture.width, p_texture.height), 
		                                new Vector2(0, 0));
		return l_sprite;
	}

	private void disposeOfResourcesTexture()
	{
		if (null != m_resourcesTexture)
		{
			Resources.UnloadAsset(m_resourcesTexture);
			m_resourcesTexture = null;
		}
	}

	private void disposeOfResourcesSprite()
	{
		if (null != m_resourcesTexture)
		{
			Resources.UnloadAsset(m_resourcesSprite);
			m_resourcesSprite = null;
		}
	}

	private void disposeOfTexture()
	{
		destroyObject(m_texture);
		m_texture = null;
	}

	private void disposeOfSprite()
	{
		destroyObject(m_sprite);
		m_sprite = null;
	}

	private void destroyObject(Object p_object)
	{
		if (null != p_object)
		{
			GameObject.Destroy(p_object);
		}
	}

	private Image m_image;
	private Sprite m_sprite;
	private Sprite m_resourcesSprite;
	private Texture2D m_texture;
	private Texture2D m_resourcesTexture;
}
