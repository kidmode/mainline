using UnityEngine;
using System.Collections;

public class WebContent : object
{
	/**
	 * Constant describing game type.
	 */
	public const int LINK_SWF     = 1;		// Link to SWF file
	public const int LINK_HTML    = 2;		// Link to HTML page
	public const int LINK_YOUTUBE = 3;		// Link to YouTube stream
	public const int GAME_NATIVE  = 4;		// Native package, installed on device

	public const int GAME_TYPE		= 2;
	public const int VIDEO_TYPE		= 3;

	/**
	 * Constant defining no sort index set, set this unusually high so that
	 * by default games that haven't been marked with a sorted index are put
	 * at the end
	 */
    public const int NO_SORT_INDEX = int.MaxValue;
	


    public WebContent()
    {

    }

    public WebContent( Hashtable p_table )
    {
        fromHashtable( p_table );
    }


	public int      id              { get; set; }	// unique ID in local database
	public int      serverId		{ get; set; }	// key from REST API backend
	public int      gameType        { get; set; }

	public string   icon            { get; set; }
	public string   youtubeId       { get; set; }
	public string   name            { get; set; }
	public string   swfUrl          { get; set; }
	public string   packageName	    { get; set; }	// For native games, android package name
	public string   targetActivity	{ get; set; }   // For native games, android Activity name for launching ( this is an optional value )

	public bool     favorite		{ get; set; }   // flag to indicate if this game is a favorite
	public bool     promoted		{ get; set; }   // flag to indicate if this game is commercially promoted
	public bool     widescreen		{ get; set; }   // For YouTube videos, flag indicates widescreen (16:9) video
    public bool     toybox          { get; set; }
	public bool 	recommend		{ get; set; }	// flag to indicate if this video or game is recommended
	public int 		category		{ get; set; }

    public string   iconSmall       { get; set; }
    public string   iconMedium      { get; set; }
    public string   iconLarge       { get; set; }


    public string url
    {
        get { return m_url; }
        set
        {
            m_url = value;
            _determineGameType();
			_formatURL();
        }
    }


    //Set member variables based off of a hashtable most likely retrieved from servers
    private void fromHashtable( Hashtable p_table )
    {
        DebugUtils.Assert( p_table != null );

        if (p_table.ContainsKey(WebContentTable.COLUMN_ID))
        {
            id = (int)((double)p_table[WebContentTable.COLUMN_ID]);
            serverId = id;
        }

        if (p_table.ContainsKey(WebContentTable.COLUMN_NAME))
            name = p_table[WebContentTable.COLUMN_NAME] as string;

		if (p_table.ContainsKey(WebContentTable.COLUMN_ICON))
			icon = p_table[WebContentTable.COLUMN_ICON] as string;

        if (p_table.ContainsKey(WebContentTable.COLUMN_ICON_SMALL))
            iconSmall = p_table[WebContentTable.COLUMN_ICON_SMALL] as string;

        if (p_table.ContainsKey(WebContentTable.COLUMN_ICON_MEDIUM))
            iconMedium = p_table[WebContentTable.COLUMN_ICON_MEDIUM] as string;

        if (p_table.ContainsKey(WebContentTable.COLUMN_ICON_LARGE))
            iconLarge = p_table[WebContentTable.COLUMN_ICON_LARGE] as string;

        if (p_table.ContainsKey(WebContentTable.COLUMN_FAVORITE))
            favorite = (bool)p_table[WebContentTable.COLUMN_FAVORITE];

        if (p_table.ContainsKey(WebContentTable.COLUMN_TOYBOX))
            toybox = (bool)p_table[WebContentTable.COLUMN_TOYBOX];

        if (p_table.ContainsKey(WebContentTable.COLUMN_PROMOTED))
            promoted = (bool)p_table[WebContentTable.COLUMN_PROMOTED];

        if (p_table.ContainsKey(WebContentTable.COLUMN_URL))
            url = p_table[WebContentTable.COLUMN_URL] as string;

        if (p_table.ContainsKey(WebContentTable.COLUMN_WIDESCREEN))
            widescreen = (bool)p_table[WebContentTable.COLUMN_WIDESCREEN];

        if (p_table.ContainsKey(WebContentTable.COLUMN_YOUTUBE_ID))
            youtubeId = p_table[WebContentTable.COLUMN_YOUTUBE_ID] as string;

        if (p_table.ContainsKey(WebContentTable.COLUMN_SWF_URL))
            swfUrl = p_table[WebContentTable.COLUMN_SWF_URL] as string;

		if (p_table.ContainsKey(WebContentTable.COLUMN_RECOMMEND))
			recommend = true;

		if (p_table.ContainsKey(WebContentTable.COLUMN_CATEGORY))
		{
			if( null != p_table[WebContentTable.COLUMN_CATEGORY])
			{
				category = (int)((double)p_table[WebContentTable.COLUMN_CATEGORY]);
			}
		}

        _determineGameType();
        //l_kid.print();
    }

   //************************** Private Implementation ****************************

    private string m_url = null;

	// these two sort indices are used to preserve the ordering
	// passed down from the server
	protected int m_linkSortIndex   = NO_SORT_INDEX;
	protected int m_favSortIndex    = NO_SORT_INDEX;


    private void _determineGameType()
    {
        gameType = LINK_HTML;

        if( youtubeId != null )
        {
            gameType = LINK_YOUTUBE;
            return;
        }

        if( swfUrl != null )
        {
            gameType = LINK_SWF;
            return;
        }

        //*TODO* get Native Game
        //if (mPackageName != null)
        //{
        //    mGameType = GAME_NATIVE;
        //    return;
        //}
    }

	private void _formatURL()
	{
		if (m_url.StartsWith("http://www.youtube.com"))
		{
			string[] l_params = m_url.Split("="[0]);
			m_url = "http://www.youtube.com/embed/" + l_params[1];
//#if UNITY_EDITOR
//			_Debug.log("URL: " + m_url);
//#endif
		}
	}
}



public class WebContentTable
{
    public const string TABLE_NAME       	    = "games";

	public const string COLUMN_ID        	    = "id";
	public const string COLUMN_KID_ID    	    = "kid_id";
	public const string COLUMN_SERVER_ID		= "server_id";
	public const string COLUMN_URL 			    = "url";
	public const string COLUMN_YOUTUBE_ID   	= "youtube_id";
	public const string COLUMN_NAME 			= "name";
    public const string COLUMN_ICON             = "icon";
    public const string COLUMN_ICON_SMALL       = "icon_small";
    public const string COLUMN_ICON_MEDIUM      = "icon_medium";
    public const string COLUMN_ICON_LARGE       = "icon_large";
	public const string COLUMN_SWF_URL          = "swf_url";
	public const string COLUMN_PACKAGE		    = "package";
	public const string COLUMN_GAME_TYPE		= "game_type";
	public const string COLUMN_FAVORITE         = "is_favorite";
	public const string COLUMN_PROMOTED		    = "promoted";
	public const string COLUMN_LINK_SORT_IDX    = "linkSortIndex";
	public const string COLUMN_FAV_SORT_IDX     = "favSortIndex";
	public const string COLUMN_TARGET_ACTIVITY  = "target_activity";
	public const string COLUMN_WIDESCREEN       = "widescreen";
    public const string COLUMN_TOYBOX           = "is_toybox";
	public const string COLUMN_RECOMMEND		= "top";
	public const string COLUMN_CATEGORY			= "category_id";
}
