using UnityEngine;
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

	public void getImage()
	{
		string contentName = "bookpage_" + id + "_" + position;
		string imagePath = "Books/Images/" + contentName;
		Texture2D texture = Resources.Load(imagePath) as Texture2D;
		pageImage = texture;
	}

	public void requestImage()
	{
		if( null != pageImage || m_requested )
			return;

		string contentName = "bookpage_" + id + "_" + position + ".jpg";
		Texture2D texture = ImageCache.getCacheImage(contentName);
		pageImage = texture;
		if (pageImage == null)
		{
			RequestQueue l_queue = new RequestQueue();
			l_queue.add(new ImageRequest("content", imageUrl, onPageImageRecieved));
			l_queue.request(RequestType.RUSH);
			m_requested = true;
		}
		else
			Debug.Log(contentName + " cached");
	}

	public void dispose()
	{
		if (pageImage != null)
		{
//			GameObject.Destroy(pageImage);
			Resources.UnloadAsset(pageImage);
			pageImage = null;
		}
	}

	private void onPageImageRecieved(WWW p_response)
	{
		if( p_response.error == null )
		{
			pageImage = p_response.texture;

			string name = "bookpage_" + id + "_" + position + ".jpg";
			Debug.Log(name);
			ImageCache.saveCacheImage(name, pageImage);
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
	public string imageName   { get; set; }

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

		if( p_table.ContainsKey( BookPageTable.COLUMN_IMAGE ) )
			imageName = p_table[ BookPageTable.COLUMN_IMAGE ] as string;

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
	public const string COLUMN_IMAGE = "page_image";
}