using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BookReading : System.Object
{
	public BookReading( )
	{
	}
	
	public BookReading( Hashtable p_table )
	{
		fromHashtable( p_table );
	}
	
	public int 		id 				{ get; set;}
	public int 		bookId 			{ get; set;}
	public string 	coverUrl 		{ get; set;}
	
	public Hashtable readingPageTable = new Hashtable();

	
	public void fromHashtable( Hashtable p_table )
	{
		if( p_table == null )
			return;
		
		if( p_table.ContainsKey( BookReadingTable.COLUMN_ID ) )
			id = (int)( (double)p_table[ BookReadingTable.COLUMN_ID ] );
		
		if( p_table.ContainsKey( BookReadingTable.COLUMN_BOOK_ID ) )
			bookId = (int)( (double)p_table[ BookReadingTable.COLUMN_BOOK_ID ] );
		
		if( p_table.ContainsKey( BookReadingTable.COLUMN_COVER_URL ) )
			coverUrl = p_table[ BookReadingTable.COLUMN_COVER_URL ] as string;

		if( p_table.ContainsKey( BookReadingTable.COLUMN_PAGES ) )
		{
			ArrayList l_tempPageList = p_table[ BookReadingTable.COLUMN_PAGES ] as ArrayList;
			_readPages( l_tempPageList );
		}
	}

	public void setPagePosition( Hashtable p_bookPageTable )
	{
		foreach( BookPage l_bookPage in p_bookPageTable.Values )
		{
			if( readingPageTable.ContainsKey( l_bookPage.id ) )
			{
				(readingPageTable[l_bookPage.id] as BookReadingPage).position = l_bookPage.position;
			}
		}
	}

	//-------------------- Private Implementation -------------------

	private void _readPages( ArrayList p_tempPageList )
	{
		if( p_tempPageList == null )
			return;
		
		foreach( Hashtable t in p_tempPageList )
		{
			BookReadingPage l_page = new BookReadingPage( t );
			readingPageTable.Add( l_page.pageId, l_page );
		}
	}
}


public class BookReadingTable
{
	public const string TABLE_NAME = "book_reading";
	
	public const string COLUMN_ID = "id";
	public const string COLUMN_BOOK_ID = "book_id";
	public const string COLUMN_COVER_URL = "cover_url";
	public const string COLUMN_PAGES = "pages";
}
