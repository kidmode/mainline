using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PaletteController : System.Object
{
	public const int DEFAULT_BUTTON_INDEX = 2;

	public PaletteController() {}
	
	public void registerButtons(UIElement p_palette)
	{
		int l_numChildren = p_palette.getChildCount();
		for (int i = 0; i < l_numChildren; ++i)
		{
			UIButton l_button = p_palette.getChildAt(i) as UIButton;
			unselectButton(l_button);

			//Initialize button list if necessary
			if (null == m_buttons)
			{
				m_buttons = new List<UIButton>();
			}

			//Add callback
			l_button.addClickCallback(onColorSelected);

			//Add button to list for cleanup of callbacks
			m_buttons.Add(l_button);
		}

		if (m_currentColorButton == null)
		{
			UIButton l_selectedButton = m_buttons[DEFAULT_BUTTON_INDEX];
			selectButton(l_selectedButton);
		}
	}
	
	public Color getCurrentColor()
	{
		return m_selectedColor;
	}
	
	public void onColorSelected(UIButton p_button)
	{
		if (m_currentColorButton != p_button)
		{
			unselectButton();
			selectButton(p_button);
		}
	}
	
	public void dispose()
	{
		m_currentColorButton = null;

		if (null != m_buttons)
		{
			int l_numButtons = m_buttons.Count;
			for (int i = 0; i < l_numButtons; ++i)
			{
				UIButton l_button = m_buttons[i];
				l_button.removeClickCallback(onColorSelected);
			}
			
			m_buttons.Clear();
			m_buttons = null;
		}
	}

	private void updateSelectedColor()
	{
		m_selectedColor = getColorFromImageComponent(m_currentColorButton);
	}

	private void selectButton()
	{
		selectButton(m_currentColorButton);
	}

	private void selectButton(UIButton p_button)
	{
		//Disable input
		p_button.enabled = false;

		//Show checkmark
		UIElement l_child = p_button.getChildAt(0);
		l_child.active = true;

		//Hold pointer
		m_currentColorButton = p_button;

		//Update selection color
		updateSelectedColor();
	}

	private void unselectButton()
	{
		unselectButton(m_currentColorButton);
	}

	private void unselectButton(UIButton p_button)
	{
		//Enable input
		p_button.enabled = true;

		//Disable checkmark
		UIElement l_child = p_button.getChildAt(0);
		l_child.active = false;
	}

	private Color getColorFromImageComponent(UIButton p_button)
	{
		GameObject l_object = p_button.gameObject;
		Image l_image = l_object.GetComponent<Image>();
		Color l_color = l_image.color;
		return l_color;
	}
	
	private Color m_selectedColor;
	private UIButton m_currentColorButton;
	private List<UIButton> m_buttons;
}
