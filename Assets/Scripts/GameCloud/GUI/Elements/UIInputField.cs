using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIInputField : UIElement
{
	public override void init(GameObject p_gameObject)
	{
		base.init(p_gameObject);
		m_input = p_gameObject.GetComponent<InputField>();
		listener.onSelect += _onSelect;
		listener.onDeselect += _onDeselect;
	}

	public override void update()
	{
		base.update();

		_update();
	}

	public string text
	{
		set
		{
			m_input.text = value;
		}
		get
		{
			return m_input.text;
		}
	}

	private void _onSelect(UIElement p_element)
	{
		if (s_selected != this)
			s_updated = false;
		s_selected = this;
	}

	private void _onDeselect(UIElement p_element)
	{
		if (s_selected == this)
		{
			s_selected = null;
			s_updated = true;
		}
	}

	private InputField m_input = null;

	private static void _update()
	{
		if (TouchScreenKeyboard.visible && s_selected != null)
		{
			if (!s_updated)
			{
				UICanvas l_canvas = s_selected.canvas;
				if (s_modifiers.Count == 0)
				{
					for (int i = 0; i < l_canvas.getChildCount(); ++i)
					{

						NoOffset noOffset = l_canvas.getChildAt(i).gameObject.GetComponent<NoOffset>();

						if(noOffset == null)
							s_modifiers.Add(new UIElementModifier(l_canvas.getChildAt(i)));
						else{

							Debug.Log("noOffset " + l_canvas.getChildAt(i).gameObject.name);
						}
					}
				}
				
				Vector2 l_canvasPos = _localToCanvasPoint(s_selected);
				Vector2 l_canvasSize = l_canvas.gameObject.GetComponent<RectTransform>().sizeDelta;
				float l_target = l_canvasSize.y * 0.25f;
				if (l_canvasPos.y < l_target)
					_offsetCanvas(l_target - l_canvasPos.y);
				s_updated = true;
			}
		}
		else
		{
			_reset();
		}
	}

	private static void _offsetCanvas(float p_offset)
	{
		for (int i = 0; i < s_modifiers.Count; ++i)
		{
			s_modifiers[i].y += p_offset;
		}
	}
	
	private static void _reset()
	{
		if (!s_updated)
			return;
		
		for (int i = 0; i < s_modifiers.Count; ++i)
		{
			s_modifiers[i].reset();
		}

		s_modifiers.Clear();
		s_updated = false;
	}
	
	private static Vector3 _localToCanvasPoint(UIElement p_element)
	{
		Vector3 l_point = new Vector3();
		while (p_element != null && false == (p_element is UICanvas))
		{
			Vector3 l_pos = p_element.gameObject.transform.localPosition;
			Vector3 l_parentScale = (p_element.parent is UICanvas ? new Vector3(1f, 1f, 1f) : p_element.parent.gameObject.transform.localScale);
			l_point.x += (l_pos.x * l_parentScale.x);
			l_point.y += (l_pos.y * l_parentScale.y);
			l_point.z += (l_pos.z * l_parentScale.z);

			p_element = p_element.parent;
		}

		return l_point;
	}

	private static UIElement s_selected = null;
	private static bool s_updated = false;
	private static List<UIElementModifier> s_modifiers = new List<UIElementModifier>();

	class UIElementModifier
	{
		public UIElementModifier(UIElement p_element)
		{
			m_element = p_element;
			m_yPos = y;
		}
		
		public void reset()
		{
			y = m_yPos;
		}
		
		public float y
		{
			get
			{
				return m_element.y;
			}
			set
			{
				m_element.y = value;
			}
		}
		
		private UIElement m_element = null;
		private float m_yPos = 0.0f;
	}
}


