using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public delegate void ButtonClickCallback(UIButton p_button);

public class UIButton : UIElement
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		m_button = p_gameObject.GetComponent< Button >();
		m_button.onClick.AddListener( listenerCallbackDispatcher );
		m_callbacks = new List<ButtonClickCallback>();
		baseElement = m_button;
		m_alpha = m_button.colors.normalColor.a;

		DebugUtils.Assert( m_button != null );
	}
	
	public override void update()
	{
		base.update();
	}
	
	public override void dispose( bool p_deep )
	{
		m_button.onClick.RemoveListener( listenerCallbackDispatcher );
		base.dispose( p_deep );
		removeAllCallbacks();
		
		if (null != m_callbacks)
		{
			m_callbacks.Clear();
			m_callbacks = null;
		}
	}

	public override float alpha
	{
		get { return m_button.colors.normalColor.a; }
		set 
		{
			if (value != m_button.colors.normalColor.a)
			{
				base.alpha = value;
				_setAlphaOfColorBlock( m_button.colors, value );
			}
		}
	}

	//This function can get alpha accurate than above after set it.
	public float currentAlpha
	{
		get { return m_colorBlock.normalColor.a; }
		set 
		{
			base.alpha = value;
			_setAlphaOfColorBlock( m_button.colors, value );
		}
	}

	public bool enabled 
	{
		get { return m_button.interactable; }
		set { m_button.interactable = value; }
	}

//-- Transitions --
	public void setTransitionMode( Button.Transition p_mode )
	{
		m_button.transition = p_mode;
	}


	public Graphic targetGraphic
	{
		get { return m_button.targetGraphic; 	}
		set { m_button.targetGraphic = value; 	}
	}

	public SpriteState spriteStates
	{
		get { return m_button.spriteState; 	}
		set { m_button.spriteState = value; }
	}

	public ColorBlock colorStates
	{
		get { return m_button.colors; 	}
		set { m_button.colors = value;  }
	}

	public Texture2D buttonImage
	{
		get 
		{
			Image l_image = m_button.image;
			Sprite l_sprite = l_image.sprite;
			return 	l_sprite.texture;
		}
		set 
		{
			Image l_image = m_button.image;
			Sprite l_sprite = l_image.sprite;
			Rect l_spriteRect = l_sprite.rect;
			Vector2 l_spritePivot = new Vector2(0f, 0f); 
			l_image.sprite = Sprite.Create(value,l_spriteRect,l_spritePivot); 
		}
	}

	public void setButtonImageAndRect(Texture2D l_texture, Rect l_rect)
	{
		Image l_image = m_button.image;
		_Debug.log (l_image);
		_Debug.log (l_texture);
		Vector2 l_spritePivot = new Vector2(0f, 0f); 
		m_button.image.sprite = Sprite.Create(l_texture,l_rect,l_spritePivot); 
	}

	public AnimationTriggers animationStates
	{
		get { return m_button.animationTriggers; 	}
		set { m_button.animationTriggers = value;	}
	}

//-- Callbackz --

	public void addClickCallback( ButtonClickCallback p_action )
	{
		m_callbacks.Add(p_action);
	}

	public void removeClickCallback( ButtonClickCallback p_action )
	{
		m_callbacks.Remove(p_action);
	}

	public void removeAllCallbacks()
	{
		m_callbacks.Clear();
	}

//------------------- Private Implementation ------------------

	private void listenerCallbackDispatcher()
	{	
		int l_numCallbacks = m_callbacks.Count;
		for (int i = 0; i < l_numCallbacks; ++i)
		{
			ButtonClickCallback l_callback = m_callbacks[i];
			l_callback(this);
		}
	}

	private void _setAlphaOfColorBlock( ColorBlock p_colorBlock, float p_alpha )
	{
		Color l_color = Color.white;

		l_color = p_colorBlock.normalColor;
		l_color.a = p_alpha;
		p_colorBlock.normalColor = l_color;
		
		l_color = p_colorBlock.disabledColor;
		l_color.a = p_alpha;
		p_colorBlock.disabledColor = l_color;
		
		l_color = p_colorBlock.highlightedColor;
		l_color.a = p_alpha;
		p_colorBlock.highlightedColor = l_color;
		
		l_color = p_colorBlock.pressedColor;
		l_color.a = p_alpha;
		p_colorBlock.pressedColor = l_color;

		m_colorBlock = p_colorBlock;
	}

	private Button m_button;
	private ColorBlock m_colorBlock;
	private List<ButtonClickCallback> m_callbacks;
}
