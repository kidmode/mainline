using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class App : System.Object
{
	public App()
    {

    }

	public App( Hashtable p_table )
    {
        fromHashtable( p_table );
    }

    public void print()
    {
       
    }

    public void fromHashtable( Hashtable p_table )
    {
        DebugUtils.Assert( p_table != null );

		if( p_table.ContainsKey( AppTable.COLUMN_NAME ) )
			name = p_table[ AppTable.COLUMN_NAME ].ToString();

		if (p_table.ContainsKey (AppTable.COLUMN_AGE_MIN))
			ageMin = int.Parse (p_table [AppTable.COLUMN_AGE_MIN].ToString ());

		if( p_table.ContainsKey( AppTable.COLUMN_AGE_MAX ) )
			ageMax = int.Parse (p_table[ AppTable.COLUMN_AGE_MAX ].ToString ());

		if( p_table.ContainsKey( AppTable.COLUMN_VIOLENCE ) )
			violence = null ==p_table[ AppTable.COLUMN_VIOLENCE ]||"null".Equals(p_table[ AppTable.COLUMN_VIOLENCE ].ToString ()) ? 0 : int.Parse (p_table[ AppTable.COLUMN_VIOLENCE ].ToString ());

		if( p_table.ContainsKey( AppTable.COLUMN_DESCRIPTION ) )
			description = p_table[ AppTable.COLUMN_DESCRIPTION ].ToString();

		if (p_table.ContainsKey (AppTable.COLUMN_PACKAGE_NAME))
			packageName = p_table[ AppTable.COLUMN_PACKAGE_NAME ].ToString();

		if( p_table.ContainsKey( AppTable.COLUMN_GEMS ) )
			gems =int.Parse(p_table[ AppTable.COLUMN_GEMS ].ToString());

		if( p_table.ContainsKey( AppTable.COLUMN_FREQUENCY ) )
			frequency =int.Parse(p_table[ AppTable.COLUMN_FREQUENCY].ToString());
		
		if (p_table.ContainsKey (AppTable.COLUMN_ICON))
			iconUrl = p_table[ AppTable.COLUMN_ICON ].ToString();
			//iconUrl = "https://www.baidu.com/img/bdlogo.png";

		if( p_table.ContainsKey( AppTable.COLUMN_ID ) )
			id = int.Parse (p_table[ AppTable.COLUMN_ID ].ToString ());

		iconDownload = false;

		own = false;

		getSubject (p_table,subjects,AppTable.COLUMN_ART);
		getSubject (p_table,subjects,AppTable.COLUMN_MUSIC);
		getSubject (p_table,subjects,AppTable.COLUMN_SAFETY);
		getSubject (p_table,subjects,AppTable.COLUMN_MATH);
		getSubject (p_table,subjects,AppTable.COLUMN_LANGUAGE);
		getSubject (p_table,subjects,AppTable.COLUMN_SCIENCE);
		getSubject (p_table,subjects,AppTable.COLUMN_LIFE_SKILLS);
		getSubject (p_table,subjects,AppTable.COLUMN_FINE_MONTOR_SKILLS);
		getSubject (p_table,subjects,AppTable.COLUMN_CREATIVE_DEVELOPMENT);
		getSubject (p_table,subjects,AppTable.COLUMN_SOCIAL_SCIENCE);
		getSubject (p_table,subjects,AppTable.COLUMN_CONITIVE_DEVELOPMENT);

	}



    public string   name        { get; set; }
    public int   	id		    { get; set; }
    public int      ageMin      { get; set; }
    public int      ageMax      { get; set; }
    public int      violence    { get; set; }
    public string   description { get; set; }
	public string   iconUrl     { get; set; }
	public string 	packageName	{ get; set; }
	public int   	gems        { get; set; }
	public int   	frequency   { get; set; }
	public bool   	own		    { get; set; }
	public Texture2D icon       { get; set; }
	public bool 	iconDownload { get; set; }
	public Dictionary< string, int > subjects = new Dictionary< string, int > ();

    //-------------------- Private Implementation -------------------
	private void getSubject(Hashtable p_table,Dictionary< string, int > p_subjects,string p_subjectName)
	{
		if (p_table.ContainsKey (p_subjectName))
			subjects [p_subjectName] = int.Parse(p_table [p_subjectName].ToString());
	}

}



public class AppTable
{
	public const string COLUMN_NAME       		   	    = "name";
	public const string COLUMN_AGE_MIN      		    = "age_min";
	public const string COLUMN_AGE_MAX      		    = "age_max";
	public const string COLUMN_VIOLENCE     		    = "violence";
	public const string COLUMN_DESCRIPTION  		    = "description";
	public const string COLUMN_ICON 		  		    = "icon";
	public const string COLUMN_ID 		  		        = "id";
	public const string COLUMN_GEMS		  		        = "gems";
	public const string COLUMN_FREQUENCY	  		    = "frequency";
	public const string COLUMN_PACKAGE_NAME	  		    = "package_name";

	public const string COLUMN_ART 		  		        = "1";
	public const string COLUMN_MUSIC 		  		    = "3";
	public const string COLUMN_SAFETY 		  		    = "5";
	public const string COLUMN_MATH		  		        = "33";
	public const string COLUMN_LANGUAGE	  		        = "60";
	public const string COLUMN_SCIENCE	  		        = "102";
	public const string COLUMN_LIFE_SKILLS 		        = "128";
	public const string COLUMN_FINE_MONTOR_SKILLS       = "134";
	public const string COLUMN_CREATIVE_DEVELOPMENT     = "136";
	public const string COLUMN_SOCIAL_SCIENCE		    = "143";
	public const string COLUMN_CONITIVE_DEVELOPMENT     = "152";
}


