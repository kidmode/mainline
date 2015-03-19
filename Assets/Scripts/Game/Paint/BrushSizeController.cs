using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrushSizeController : System.Object
{
	public BrushSizeController() {}

	public void registerButton(UIButton p_button, float p_size)
	{
		if (m_selectedSizeButton == null)
		{
			m_selectedSizeButton = p_button;
			m_selectedSize = p_size;
			p_button.enabled = false;
		}

		if (null == m_buttons)
		{
			m_buttons = new List<UIButton>();
			m_sizes = new Dictionary<UIButton, float>();
		}

		m_buttons.Add(p_button);
		m_sizes[p_button] = p_size;
		p_button.addClickCallback(onSizeButtonClicked);
	}

	public float getCurrentSize()
	{
		return m_selectedSize;
	}

	public void onSizeButtonClicked(UIButton p_button)
	{
		if (m_selectedSizeButton != p_button)
		{
			m_selectedSizeButton.enabled = true;

			m_selectedSizeButton = p_button;
			m_selectedSizeButton.enabled = false;
			m_selectedSize = m_sizes[p_button];
		}
	}

	public void dispose()
	{
		m_selectedSizeButton = null;
		m_selectedSize = 0;

		if (null != m_buttons)
		{
			int l_numButtons = m_buttons.Count;
			for (int i = 0; i < l_numButtons; ++i)
			{
				UIButton l_button = m_buttons[i];
				l_button.removeClickCallback(onSizeButtonClicked);
			}

			m_buttons.Clear();
			m_buttons = null;

			m_sizes.Clear();
			m_sizes = null;
		}
	}

	private float m_selectedSize;
	private UIButton m_selectedSizeButton;
	private List<UIButton> m_buttons;
	private Dictionary<UIButton, float> m_sizes;
}
