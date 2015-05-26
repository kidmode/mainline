using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIComboBox : UIElement
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );

		m_currentData = new ComboBoxData ("null","null");
		m_button = getView("checkButton") as UIButton;
		m_swipeList = getView ("swipeList") as UISwipeList;
		m_swipeList.active = false;
	}
	
	public override void update()
	{
		base.update();
	}

	private void onButtonClick(UIButton p_button)
	{
		m_swipeList.active = m_swipeList.active?false:true;
	}

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		ComboBoxData l_data = p_data as ComboBoxData;
		UILabel l_name = p_element.getView ("entryName") as UILabel; 
		l_name.text = l_data.entryName;
		if(l_data.selected)
		{
			p_element.alpha = m_alpha;
		}
		else
		{
			p_element.alpha = 1.0f;
		}
	}

	public void setSwipeListDate(List<object> p_comboBoxDate)
	{
		if(null != m_swipeList && null != p_comboBoxDate)
		{
			m_swipeList.setData( p_comboBoxDate );
			m_swipeList.setDrawFunction( onListDraw );
			m_swipeList.redraw();
			m_swipeList.addClickListener("Prototype", onEntrySelected);
			m_button.addClickCallback (onButtonClick);
			m_currentData = p_comboBoxDate.Count-1 >=0? p_comboBoxDate[p_comboBoxDate.Count-1] as ComboBoxData:new ComboBoxData ("null","null");
			UILabel l_buttonText = m_button.getView ("Text") as UILabel;
			l_buttonText.text = m_currentData.entryName;
		}
	}

	private void onEntrySelected(UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index)
	{
		if(null != m_currentSelectButton)
		{
			m_currentSelectButton.alpha = 1.0f;
		}
		p_button.alpha = m_alpha;
		m_currentSelectButton = p_button;
		UILabel l_buttonText = m_button.getView ("Text") as UILabel;
		ComboBoxData l_currentData = p_data as ComboBoxData;
		m_currentData = l_currentData;
		l_buttonText.text = l_currentData.entryName;
		m_swipeList.active = false;
		if(m_lastIndex != -1)
		{
			(p_list.getData()[m_lastIndex] as ComboBoxData).selected = false;
		}
		m_lastIndex = p_index;
		l_currentData.selected = true;
	}

	public ComboBoxData currentData
	{
		get{return m_currentData;}
		set{m_currentData = value;}
	}
	private int m_lastIndex = -1;
	private float m_alpha = 0.4f;
	private UIButton m_currentSelectButton;
	private ComboBoxData m_currentData;
	private UISwipeList m_swipeList;
	private UIImage m_backImage;
	private UIButton m_button;
}

public class ComboBoxData
{
	public ComboBoxData(string p_name, object p_value)
	{
		entryName = p_name;
		entryValue = p_value;
		m_selected = false;
	}
	public ComboBoxData()
	{}

	public string entryName
	{
		get{return m_name;}
		set{m_name = value;}
	}
	public object entryValue
	{
		get{return m_value;}
		set{m_value = value;}
	}
	public  bool  selected
	{
		get{return m_selected;}
		set{m_selected = value;}
	}
	private bool   m_selected;
	private string m_name;
	private object m_value;
}
