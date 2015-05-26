using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UILabel : UIElement 
{
	public override void init (GameObject p_gameObject)
	{
		base.init( p_gameObject );
		m_label = p_gameObject.GetComponent<Text>();
		baseElement = m_label;

		DebugUtils.Assert( m_label != null );

		m_outline 	= p_gameObject.GetComponent<Outline>();
		m_shadow	= p_gameObject.GetComponent<Shadow>();
		m_alpha 	= color.a;
	}

	public override void update()
	{
		base.update();
	}

	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}

	public Vector2 calcSize()
	{
		GUIStyle l_style = new GUIStyle();
		l_style.font = m_label.font;
		l_style.fontSize = m_label.fontSize;
		return l_style.CalcSize(new GUIContent(m_label.text));
	}

	public override float alpha
	{
		set 
		{ 
			if (value != base.alpha)
			{
				base.alpha = value;
				color = new Color( color.r, color.g, color.b, value ); 
			}
		}
		get { return color.a; }
	}

	public Color color 
	{
		set { m_label.color = value; 	}
		get { return m_label.color; 	}
	}


	public string text
	{
		get { return m_label.text;	}
		set 
		{
			if (null == value
			    || !value.Equals(m_label.text) )
			{
				m_label.text = value;	
			}
		}
	}

    public TextAnchor alignment
    {
        get { return m_label.alignment;     }
        set { m_label.alignment = value;    }
    }

	//Add Helper functions herez.
	public Outline outline
	{
		get { return m_outline; 	}
		set { m_outline = value;	}
	}

	public Shadow shadow
	{
		get { return m_shadow; 	}
		set { m_shadow = value;	}
	}

	public int fontSize
	{
		get { return m_label.fontSize; }
		set { m_label.fontSize = value; }
	}

	private Text 	m_label;
	private Outline m_outline;
	private Shadow	m_shadow;
}
