using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum OutputMode
{
	DISABLE,
	CONSOLE,
	RUNTIME
}

public class _Debug : MonoBehaviour
{
	public void Awake()
	{
		_init();
	}

	public void onClear()
	{
		_clear();
	}

	public void onSwitch()
	{
		m_output.SetActive(!m_output.activeSelf);
	}

	public void onBeginDrag(BaseEventData p_eventData)
	{
		PointerEventData l_eventData = p_eventData as PointerEventData;
		m_dragPosition = l_eventData.position;
	}

	public void onEndDrag(BaseEventData p_eventData)
	{
		PointerEventData l_eventData = p_eventData as PointerEventData;
		float l_delta = l_eventData.position.y - m_dragPosition.y;
		if (l_delta > 0)
			m_index = Mathf.Min(m_index + 1, m_messages.Count / LINE_COUNT);
		else if (l_delta < 0)
			m_index = Mathf.Max(m_index - 1, 0);

		_fillMessages(m_index);
	}

	private void _init()
	{
		if (GameObject.FindObjectOfType<EventSystem>() == null)
			throw new MissingComponentException("Missing Event System Component!!!");

		Canvas l_canvas = GetComponent<Canvas>();
		l_canvas.worldCamera = Camera.main;

		m_output = _findObject("_Output");
		m_content = _findObject("_Content");
		m_line = _findObject("_Line");
		m_lines = new GameObject[LINE_COUNT];
		for (int i = 0; i < LINE_COUNT; ++i)
		{
			GameObject l_newLine = Object.Instantiate(m_line) as GameObject;
			l_newLine.transform.parent = m_content.transform;
			l_newLine.transform.localPosition = new Vector3(0, -(i + 1) * LINE_HEIGHT, 0);
			l_newLine.transform.localScale = new Vector3(1f, 1f, 1f);
			m_lines[i] = l_newLine;
		}

		_clear();
	}
	
	private GameObject _findObject(string p_name)
	{
		RectTransform[] l_transforms = GetComponentsInChildren<RectTransform>(true);
		for (int i = 0; i < l_transforms.Length; ++i)
		{
			if (l_transforms[i].name == p_name)
				return l_transforms[i].gameObject;
		}

		return null;
	}

	private void _log(string p_message, Color p_color)
	{
		m_messages.Add(new Message(string.Format("{0} > {1}", (m_messages.Count + 1), p_message), p_color));

		if ((m_index + 1) * LINE_COUNT >= m_messages.Count)
		{
			_fillMessages(m_index);
		}
	}

	private void _clear()
	{
		for (int i = 0; i < LINE_COUNT; ++i)
		{
			Text l_text = m_lines[i].GetComponent<Text>();
			l_text.text = "";
		}
		m_messages.Clear();
	}

	private void _fillMessages(int p_index)
	{
		p_index *= LINE_COUNT;
		for (int i = 0; i < LINE_COUNT; ++i)
		{
			Message l_message = (p_index + i) < m_messages.Count ? m_messages[p_index + i] as Message : new Message("", Color.white);
			Text l_text = m_lines[i].GetComponent<Text>();
			l_text.text = l_message.content.Length > 300 ? l_message.content.Substring(0, 300) : l_message.content;
			l_text.color = l_message.color;
		}
	}

	class Message
	{
		public string content { get; set; }
		public Color color { get; set; }

		public Message(string p_content, Color p_color)
		{
			content = p_content;
			color = p_color;
		}
	}

	private GameObject m_output;
	private GameObject m_content;
	private GameObject m_line;
	private GameObject[] m_lines;
	private int m_index = 0;
	private ArrayList m_messages = new ArrayList();
	private Vector2 m_dragPosition;

	private const float LINE_HEIGHT = 15.0f;
	private const int LINE_COUNT = 19;

	public static void log(object p_message)
	{
		if (s_mode == OutputMode.DISABLE)
			return;

		Debug.Log(p_message);

		if (s_mode == OutputMode.RUNTIME)
			_instance._log(p_message.ToString(), Color.white);
	}

	public static void logError(object p_message)
	{
		if (s_mode == OutputMode.DISABLE)
			return;
		
		Debug.LogError(p_message);

		if (s_mode == OutputMode.RUNTIME)
			_instance._log(p_message.ToString(), Color.red);
	}

	public static void logWarning(object p_message)
	{
		if (s_mode == OutputMode.DISABLE)
			return;
		
		Debug.LogWarning(p_message);

		if (s_mode == OutputMode.RUNTIME)
			_instance._log(p_message.ToString(), Color.yellow);
	}

	public static OutputMode mode
	{
		set
		{
			s_mode = value;
		}
	}

	private static _Debug _instance
	{
		get
		{
			if (s_instance == null)
			{
				GameObject l_debug = GameObject.Find("_Debug");
				if (l_debug == null)
					l_debug = Object.Instantiate(Resources.Load("_Debug/_Debug") as Object) as GameObject;
				s_instance = l_debug.GetComponent<_Debug>();
			}

			return s_instance;
		}
	}

	private static OutputMode s_mode = OutputMode.CONSOLE;
	private static _Debug s_instance = null;
}
