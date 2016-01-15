using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Reading : System.Object
{
    public const string RECORDING_STATE = "recording";
    public const string FINISHED_STATE  = "finished";

    public int      bookId        { get; set; }
    public string   createdAt     { get; set; }
    public int      id            { get; set; }
    public bool     isDefault     { get; set; }
    public bool     isMicOnly     { get; set; }
    public string   snapshotUrl   { get; set; }
    public string   state         { get; set; }

    public List<ReadingPage> pageList = new List<ReadingPage>();


    public Reading ( Hashtable p_table )
    {
        DebugUtils.Assert(p_table != null);

        if (p_table.ContainsKey(ReadingTable.COLUMN_ID))
            id = (int)((double)p_table[ReadingTable.COLUMN_ID]);

        if (p_table.ContainsKey(ReadingTable.COLUMN_BOOK_ID))
            bookId = (int)((double)p_table[ReadingTable.COLUMN_BOOK_ID]);

        if (p_table.ContainsKey(ReadingTable.COLUMN_CREATED_TIME))
            createdAt = p_table[ReadingTable.COLUMN_CREATED_TIME] as string;

        if (p_table.ContainsKey(ReadingTable.COLUMN_IS_DEFAULT))
            isDefault = (bool)p_table[ReadingTable.COLUMN_IS_DEFAULT];

        if (p_table.ContainsKey(ReadingTable.COLUMN_MIC_ONLY))
            isMicOnly = (bool)p_table[ReadingTable.COLUMN_MIC_ONLY];

        if (p_table.ContainsKey(ReadingTable.COLUMN_SNAPSHOT_URL))
            snapshotUrl = p_table[ReadingTable.COLUMN_SNAPSHOT_URL] as string;

        if (p_table.ContainsKey(ReadingTable.COLUMN_STATE))
            state = p_table[ReadingTable.COLUMN_STATE] as string;

        if (p_table.ContainsKey(ReadingTable.COLUMN_PAGES))
        {
            ArrayList l_tempPageList = p_table[ReadingTable.COLUMN_PAGES] as ArrayList;
            _readPages( l_tempPageList );
        }
    }


    
 //-------------------- Private Implementation -------------------
    private void _readPages( ArrayList p_tempPageList )
    {
        if (p_tempPageList == null)
            return;

        foreach (Hashtable t in p_tempPageList)
        {
            ReadingPage l_page = new ReadingPage(t);

//			Debug.LogError("  l_page " + l_page.playbackUrl);


            pageList.Add( l_page );
        }
    }
}


public class ReadingTable
{
    public const string TABLE_NAME          = "readings";

    public const string COLUMN_ID           = "id";
    public const string COLUMN_CREATED_TIME = "created_at";
    public const string COLUMN_BOOK_ID      = "book_id";
    public const string COLUMN_IS_DEFAULT   = "is_default";
    public const string COLUMN_MIC_ONLY     = "mic_only";
    public const string COLUMN_PAGES        = "pages";
    public const string COLUMN_READER       = "reader";
    public const string COLUMN_SNAPSHOT_URL = "snapshot_url";
    public const string COLUMN_STATE        = "state";
}
