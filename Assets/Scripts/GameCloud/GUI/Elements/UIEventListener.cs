using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UIEventListener : EventTrigger
{
	public delegate void EventTriggerDelegate(UIElement p_element);

	public EventTriggerDelegate onSelect = null;
	public EventTriggerDelegate onDeselect = null;

	public override void OnSelect(BaseEventData p_eventData)
	{
		if (onSelect != null)
			onSelect(m_element);
	}

	public override void OnDeselect(BaseEventData p_eventData)
	{
		if (onDeselect != null)
			onDeselect(m_element);
	}

	private UIElement m_element = null;

	public static UIEventListener attach(UIElement p_element)
	{
		UIEventListener l_listener = p_element.gameObject.GetComponent<UIEventListener>();
		if (l_listener == null)
		{
			l_listener = p_element.gameObject.AddComponent<UIEventListener>();
			l_listener.m_element = p_element;
		}
		
		return l_listener;
	}	
}

