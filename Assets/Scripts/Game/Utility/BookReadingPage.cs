using UnityEngine;
using System.Collections;

public class BookReadingPage : System.Object
{
	public BookReadingPage( )
	{
	}
	
	public BookReadingPage( Hashtable p_table )
	{
		fromHashtable( p_table );
	}

	public int 		id 				{ get; set;}
	public int 		pageId 			{ get; set;}
	public string 	audioSlug 		{ get; set;}
	public float 	audioDuration 	{ get; set;}
	public string 	audioUrl 		{ get; set;}
	public int 		position 		{ get; set;}

	public void fromHashtable( Hashtable p_table )
	{
		if( p_table == null )
			return;
		
		if( p_table.ContainsKey( BookReadingPageTable.COLUMN_ID ) )
			id = (int)( (double)p_table[ BookReadingPageTable.COLUMN_ID ] );
		
		if( p_table.ContainsKey( BookReadingPageTable.COLUMN_PAGE_ID ) )
			pageId = (int)( (double)p_table[ BookReadingPageTable.COLUMN_PAGE_ID ] );
		
		if( p_table.ContainsKey( BookReadingPageTable.COLUMN_AUDIO_SLUG ) )
			audioSlug = p_table[ BookReadingPageTable.COLUMN_AUDIO_SLUG ] as string;

		if( p_table.ContainsKey( BookReadingPageTable.COLUMN_AUDIO_DURATION ) )
			if( p_table[ BookReadingPageTable.COLUMN_AUDIO_DURATION ] != null )
				audioDuration = (float)( (double)p_table[ BookReadingPageTable.COLUMN_AUDIO_DURATION ] );
		
		if( p_table.ContainsKey( BookReadingPageTable.COLUMN_AUDIO_URL ) )
			audioUrl = p_table[ BookReadingPageTable.COLUMN_AUDIO_URL ] as string;
	}
}

public class BookReadingPageTable
{
	public const string TABLE_NAME = "book_reading_pages";
	
	public const string COLUMN_ID = "id";
	public const string COLUMN_PAGE_ID = "page_id";

	//work for audio API
	public const string COLUMN_AUDIO_SLUG = "audio_slug";
	public const string COLUMN_AUDIO_DURATION = "audio_duration";
	public const string COLUMN_AUDIO_URL = "audio_playback_url";

//	//work for video API
//	public const string COLUMN_AUDIO_SLUG = "slug";
//	public const string COLUMN_AUDIO_DURATION = "duration";
//	public const string COLUMN_AUDIO_URL = "playback_url";
}
