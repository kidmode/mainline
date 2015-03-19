using UnityEngine;
using System.Collections;

public class GAFException : System.Exception 
{
	private TagRecord m_Record = null;

	public GAFException(string _Message) : base(_Message)
	{
	}

	public GAFException(string _Message, TagRecord _Record) : base(_Message)
	{
		m_Record = _Record;
	}

	public TagRecord record
	{
		get
		{
			return m_Record;
		}
	}
}
