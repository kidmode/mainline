using UnityEngine;
using System.Collections;

public class ReadingPage : System.Object
{
    public int      duration    { get; set; }
    public int      id          { get; set; }
    public int      pageId      { get; set; }
    public string   playbackUrl { get; set; }
    public string   slug        { get; set; }

    public ReadingPage( Hashtable p_pageTable )
    {
        fromHashtable( p_pageTable );
    }

    public void fromHashtable( Hashtable p_table )
    {
        DebugUtils.Assert(p_table != null);

        if (p_table.ContainsKey(ReadingPageTable.COLUMN_ID))
            id = (int)((double)p_table[ReadingPageTable.COLUMN_ID]);

        if (p_table.ContainsKey(ReadingPageTable.COLUMN_DURATION))
            duration = (int)((double)p_table[ReadingPageTable.COLUMN_DURATION] );

        if (p_table.ContainsKey(ReadingPageTable.COLUMN_PAGE_ID))
            pageId = (int)((double)p_table[ReadingPageTable.COLUMN_PAGE_ID] );

        if (p_table.ContainsKey(ReadingPageTable.COLUMN_PLAYBACK_URL))
            playbackUrl = p_table[ReadingPageTable.COLUMN_PLAYBACK_URL] as string;

        if (p_table.ContainsKey(ReadingPageTable.COLUMN_SLUG))
            slug = p_table[ReadingPageTable.COLUMN_SLUG] as string;
    }
}

public class ReadingPageTable : System.Object
{
    public const string TABLE_NAME          = "readings_page";

    public const string COLUMN_DURATION     = "duration";
    public const string COLUMN_ID           = "id";
    public const string COLUMN_PAGE_ID      = "page_id";
    public const string COLUMN_PLAYBACK_URL = "playback_url";
    public const string COLUMN_SLUG         = "slug";




}
