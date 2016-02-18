using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Book : System.Object
{
    public Book()
    {
		isIconSet = false;
    }

    public Book( Hashtable p_table )
    {
        fromHashtable( p_table );
    }

    public void print()
    {
        //_Debug.log("*********************************");
        //_Debug.log(id);
        //_Debug.log(photo);
        //_Debug.log(name);
        //_Debug.log(allowVideoMail);
        //_Debug.log(birthday);
        //_Debug.log(weightCognitiveDevelopment);
        //_Debug.log(weightCreativeDevelopment);
        //_Debug.log(weightLifeSkills);
        //_Debug.log(weightMath);
        //_Debug.log(weightReading);
        //_Debug.log(weightScience);
        //_Debug.log(weightSocialStudies);
        //_Debug.log("*********************************");
    }

    public void fromHashtable( Hashtable p_table )
    {
        DebugUtils.Assert( p_table != null );

        if( p_table.ContainsKey( BookTable.COLUMN_ID ) )
            id = (int)( (double)p_table[ BookTable.COLUMN_ID ] );

        if( p_table.ContainsKey( BookTable.COLUMN_AUTHOR ))
            author = p_table[ BookTable.COLUMN_AUTHOR ] as string;

        if( p_table.ContainsKey( BookTable.COLUMN_ILLUSTRATOR ) )
            illustrator = p_table[ BookTable.COLUMN_ILLUSTRATOR ] as string;

        if( p_table.ContainsKey( BookTable.COLUMN_COVER_URL ) )
			coverUrl = p_table[ BookTable.COLUMN_COVER_URL ] as string;
			//coverUrl = "http://mat1.gtimg.com/www/images/qq2012/qqlogo_1x.png";
        
        if( p_table.ContainsKey( BookTable.COLUMN_SKU ) )
            googleSku = p_table[ BookTable.COLUMN_SKU ] as string;

        if( p_table.ContainsKey( BookTable.COLUMN_PRICE ) )
            price = p_table[ BookTable.COLUMN_PRICE ] as string;
        
        if( p_table.ContainsKey( BookTable.COLUMN_OWNED ) )
            owned = (bool)p_table[ BookTable.COLUMN_OWNED ];

        if( p_table.ContainsKey( BookTable.COLUMN_SORT_ORDER ) )
            sortOrder = (int)( (double)p_table[ BookTable.COLUMN_SORT_ORDER ] );

		if( p_table.ContainsKey( BookTable.COLUMN_TITLE ) )
			title = p_table[ BookTable.COLUMN_TITLE ] as string;

		if( p_table.ContainsKey( BookTable.COLUMN_GEMS ) )
			gems = (int)( (double)p_table[ BookTable.COLUMN_GEMS ] );

        if( p_table.ContainsKey( BookTable.COLUMN_PAGES ) )
        {
            ArrayList l_tempPageList = p_table[ BookTable.COLUMN_PAGES ] as ArrayList;
            _readPages( l_tempPageList );
        }

		if( p_table.ContainsKey( BookTable.COLUMN_TOP ) )
		{
			isTop = (bool)p_table[ BookTable.COLUMN_TOP ];
		}
		else
		{
			isTop = false;
		}

		if( p_table.ContainsKey( BookTable.COLUMN_IS_FAVORITE ) )
		{
			isFavorite = (bool)p_table[ BookTable.COLUMN_IS_FAVORITE ];
		}
		else
		{
			isFavorite = false;
		}
		
       // _addCoverPage();

        _sortPages();
    }



    public string   author      { get; set; }
	public string   title       { get; set; }
    public string   illustrator { get; set; }

    public string   coverUrl    { get; set; }
    public string   googleSku   { get; set; }
	public string   price       { get; set; }
	public int      gems	    { get; set; }

    public bool     owned       { get; set; }
	public bool		isTop		{ get; set; }
	public bool		isFavorite	{ get; set; }

    public int      id          { get; set; }
	public int      sortOrder   { get; set; }
	public Texture2D icon       { get; set; }

    public List< BookPage > pageList = new List< BookPage >();
	public Hashtable pageTable = new Hashtable();

	public bool		isIconSet	{get; set;}

    //-------------------- Private Implementation -------------------
    private void _readPages( ArrayList p_tempPageList )
    {
        if( p_tempPageList == null )
            return;

        foreach( Hashtable t in p_tempPageList )
        {
            BookPage l_page = new BookPage( t );
            pageList.Add( l_page );
			pageTable.Add( l_page.id, l_page );
        }
    }

    private void _addCoverPage()
    {
        BookPage l_page     = new BookPage();
        l_page.imageUrl     = coverUrl;
        l_page.position     = 0;
        l_page.id           = -1;
        
        string l_content    =  "Author: "      + author + "\n" +
                               "Illustrator: " + illustrator;
        l_page.content      = l_content;

        pageList.Add(l_page);
		pageTable.Add( l_page.id, l_page );
    }

    private void _sortPages()
    {
        if( pageList.Count <= 1 )
            return;

        pageList.Sort( _sortCallback );
    }

    private int _sortCallback( BookPage a, BookPage b )
    {
        return a.position.CompareTo( b.position );
    }

	public void dispose ()
	{
		if (null != pageList)
		{
			int l_numPages = pageList.Count;
			for (int i = 0; i < l_numPages; ++i)
			{
				BookPage l_page = pageList[i];
				l_page.dispose();
			}
			pageList.Clear();
			pageList = null;
		}

		if (pageTable != null)
		{
			pageTable.Clear();
			pageTable = null;
		}
	}
}



public class BookTable
{
    public const string TABLE_NAME       		    = "books";
                 
	public const string COLUMN_ID 				    = "id";
	public const string COLUMN_SKU 				    = "google_sku";
	public const string COLUMN_TITLE 			    = "title";
	public const string COLUMN_OWNED	 		    = "owned";
	public const string COLUMN_AUTHOR 			    = "author";
	public const string COLUMN_ILLUSTRATOR		    = "illustrator";
	public const string COLUMN_COVER_URL 		    = "cover_url";
	public const string COLUMN_REQUEST_COUNT 	    = "request_count";
	public const string COLUMN_PRICE 			    = "price";
	public const string COLUMN_GEMS 			    = "gems";

    public const string COLUMN_PAGES                = "pages";

	public const string COLUMN_TOP                	= "top";
	
	/**
	 * Some special column names used by queryAndOrderByOwnedAndRequestCountWithReadings
	 */
	public const string COLUMN_READING_URL		    = "reading_url";
	public const string COLUMN_READING_COUNT		= "reading_count";
	public const string COLUMN_DEFAULT_READING_ID	= "default_reading_id";
	public const string COLUMN_READING_ID		    = "reading_id";
	public const string COLUMN_SORT_ORDER		    = "sort_order";

	public const string COLUMN_IS_FAVORITE			= "is_favorite";
}


