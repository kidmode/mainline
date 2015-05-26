﻿using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;

public class BookPage : System.Object
{
    public BookPage( )
    {
    }

    public BookPage( Hashtable p_table )
    {
        fromHashtable( p_table );
    }

	public void requestImage()
	{
		if( null != pageImage || m_requested )
			return;

		_Debug.log("URL: " + imageUrl);
		RequestQueue l_queue = new RequestQueue();
		l_queue.add(new ImageRequest("content", imageUrl, onPageImageRecieved));
		l_queue.request(RequestType.RUSH);
		m_requested = true;
	}

	public void dispose()
	{
		if (pageImage != null)
		{
			GameObject.Destroy(pageImage);
		}

		pageImage = null;
	}

	private void onPageImageRecieved(WWW p_response)
	{
		if( p_response.error == null )
		{
			pageImage = p_response.texture;
		}
		else
		{
			_Debug.log( p_response.error );
			m_requested = false;
		}
	}

    public int id           { get; set; }
    public int position     { get; set; }
    
    public string content   { get; set; }
    public string imageUrl   { get; set; }
	public Texture2D pageImage { get; set; }

	private bool m_requested = false;
    
    public void fromHashtable( Hashtable p_table )
    {
        if( p_table == null )
            return;

        if( p_table.ContainsKey( BookPageTable.COLUMN_ID ) )
            id = (int)( (double)p_table[ BookPageTable.COLUMN_ID ] );
        
        if( p_table.ContainsKey( BookPageTable.COLUMN_POSITION ) )
            position = (int)( (double)p_table[ BookPageTable.COLUMN_POSITION ] );
        
        if( p_table.ContainsKey( BookPageTable.COLUMN_CONTENT ) )
		{
			content = p_table[ BookPageTable.COLUMN_CONTENT ] as string;
			if( content != null )
				content = content.Trim();
		}
        
        if( p_table.ContainsKey( BookPageTable.COLUMN_PAGE_URL ) )
            imageUrl = p_table[ BookPageTable.COLUMN_PAGE_URL ] as string;

		m_requested = false;
	}
}

public class BookPageTable
{
    public const string TABLE_NAME = "pages";

    public const string COLUMN_ID = "id";
    public const string COLUMN_CONTENT = "content";
    public const string COLUMN_PAGE_URL = "page_url";
    public const string COLUMN_POSITION = "position";
}