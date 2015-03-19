using UnityEngine;
using System.Collections;

public class SoundReference : object 
{
	public SoundReference(object p_obj) 
	{
		m_data = p_obj;
	}
	
	public object data
	{
		get
		{
			return m_data;
		}
	}
	
	private object m_data;
}