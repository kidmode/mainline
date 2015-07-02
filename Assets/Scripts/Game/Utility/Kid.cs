using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


public class Kid : System.Object
{
    public Kid()
    {
        maxViolence = ViolenceRating.Default;
    }

	public Kid( string p_name, int p_id, string p_photo )
    {
        name        = p_name;
        id          = p_id;
        photo       = p_photo;

        maxViolence = ViolenceRating.Default;
    }

	public Kid( Hashtable p_table )
    {
        fromHashtable( p_table );
		if (SessionHandler.getInstance().selectAvatar !=  null)
			kid_photo = Resources.Load("GUI/2048/common/avatars/" + SessionHandler.getInstance().selectAvatar) as Texture2D;
    }

	public void requestPhoto()
	{
		if (!m_photoRequested)
		{
			kid_photo = ImageCache.getCacheImage(wholeName+".png");
			if (kid_photo == null)
			{
				kid_photo = Resources.Load("GUI/2048/common/avatars/icon_avatar_gen") as Texture2D;
				if( photo != null )
					Server.request( photo, null, CallMethod.GET, _requestPhotoComplete );
			}
		}
	}

    public void print()
    {
        _Debug.log("*********************************");
        _Debug.log(id);
        _Debug.log(photo);
        _Debug.log(name);
        _Debug.log(allowVideoMail);
        _Debug.log(birthday);
        _Debug.log(weightCognitiveDevelopment);
        _Debug.log(weightCreativeDevelopment);
        _Debug.log(weightLifeSkills);
        _Debug.log(weightMath);
        _Debug.log(weightReading);
        _Debug.log(weightScience);
        _Debug.log(weightSocialStudies);
		_Debug.log(gems);
		_Debug.log(stars);
        _Debug.log("*********************************");
    }

	public void fromHashtable( Hashtable p_table )
    {
        DebugUtils.Assert( p_table != null );
	
		if (p_table.ContainsKey(KidsTable.COLUMN_ID))
            id = (int)((double)p_table[KidsTable.COLUMN_ID]);

		if (p_table.ContainsKey(KidsTable.COLUMN_NAME))
		{
			wholeName = p_table[KidsTable.COLUMN_NAME] as string;
			List<string> l_list = new List<String>();
			l_list = separateChildName(wholeName);
			foreach(string l_name in l_list)
			{
				if(!string.Empty.Equals(l_name))
				{
					name = l_name;
					break;
				}
			}
		}

		if (p_table.ContainsKey(KidsTable.COLUMN_BIRTHDAY))
		{
            birthday = p_table[KidsTable.COLUMN_BIRTHDAY] as string;

			int l_age = 0;
			DateTime l_date = DateTime.Parse (birthday);
			l_age = DateTime.Now.Year - l_date.Year;
			DateTime l_now = DateTime.Now;
			
			if( l_now.Month < l_date.Month )
			{
				l_age--;
			}
			else if( l_now.Month == l_date.Month && l_now.Day < l_date.Day )
			{
				l_age--;
			}
			age = l_age;
		}

		if (p_table.ContainsKey(KidsTable.COLUMN_MAX_VIOLENCE))
            maxViolence = (ViolenceRating)((double)p_table[KidsTable.COLUMN_MAX_VIOLENCE]);

		if (p_table.ContainsKey(KidsTable.COLUMN_ALLOW_VIDEO_MAIL))
            allowVideoMail = (bool)p_table[KidsTable.COLUMN_ALLOW_VIDEO_MAIL];

		if (p_table.ContainsKey(KidsTable.COLUMN_WEIGHT_MATH))
            weightMath = (int)((double)p_table[KidsTable.COLUMN_WEIGHT_MATH]);

		if (p_table.ContainsKey(KidsTable.COLUMN_WEIGHT_READING))
            weightReading = (int)((double)p_table[KidsTable.COLUMN_WEIGHT_READING]);

		if (p_table.ContainsKey(KidsTable.COLUMN_WEIGHT_SCIENCE))
            weightScience = (int)((double)p_table[KidsTable.COLUMN_WEIGHT_SCIENCE]);

		if (p_table.ContainsKey(KidsTable.COLUMN_WEIGHT_SOCIAL_STUDIES))
            weightSocialStudies = (int)((double)p_table[KidsTable.COLUMN_WEIGHT_SOCIAL_STUDIES]);

		if (p_table.ContainsKey(KidsTable.COLUMN_WEIGHT_COGNITIVE_DEVELOPMENT))
            weightCognitiveDevelopment = (int)((double)p_table[KidsTable.COLUMN_WEIGHT_COGNITIVE_DEVELOPMENT]);

		if (p_table.ContainsKey(KidsTable.COLUMN_WEIGHT_CREATIVE_DEVELOPMENT))
            weightCreativeDevelopment = (int)((double)p_table[KidsTable.COLUMN_WEIGHT_CREATIVE_DEVELOPMENT]);

		if (p_table.ContainsKey(KidsTable.COLUMN_WEIGHT_LIFE_SKILLS))
            weightLifeSkills = (int)((double)p_table[KidsTable.COLUMN_WEIGHT_LIFE_SKILLS]);

		if (p_table.ContainsKey(KidsTable.COLUMN_LEVEL))
			level = int.Parse(p_table[KidsTable.COLUMN_LEVEL].ToString());

		if (p_table.ContainsKey(KidsTable.COLUMN_GEM))
			gems = (int)((double)p_table[KidsTable.COLUMN_GEM]);

		if (p_table.ContainsKey(KidsTable.COLUMN_STAR))
			stars = (int)((double)p_table[KidsTable.COLUMN_STAR]);

		if (p_table.ContainsKey(KidsTable.COLUMN_PHOTO))
			photo = (string)p_table[KidsTable.COLUMN_PHOTO];

		if (p_table.ContainsKey(KidsTable.LANGUAGE_COUNT))
			languageCount = int.Parse(p_table[KidsTable.LANGUAGE_COUNT].ToString());

		if (p_table.ContainsKey(KidsTable.VIDEO_WATCHED_COUNT))
			videoWatchedCount = int.Parse(p_table[KidsTable.VIDEO_WATCHED_COUNT].ToString());

		if (p_table.ContainsKey(KidsTable.GAME_PLAYED_COUNT))
			gamePlayedCount = int.Parse(p_table[KidsTable.GAME_PLAYED_COUNT].ToString());


    }

	public Hashtable toHashTable()
	{
		Hashtable ret = new Hashtable();

		ret.Add(KidsTable.COLUMN_ID, id);
		if (wholeName != null)
			ret.Add(KidsTable.COLUMN_NAME, wholeName);
		if (birthday != null)
			ret.Add(KidsTable.COLUMN_BIRTHDAY, birthday);
		ret.Add(KidsTable.COLUMN_MAX_VIOLENCE, (double)maxViolence);
		if (allowVideoMail != null)
			ret.Add(KidsTable.COLUMN_ALLOW_VIDEO_MAIL, allowVideoMail);
		ret.Add(KidsTable.COLUMN_WEIGHT_MATH, weightMath);
		ret.Add(KidsTable.COLUMN_WEIGHT_READING, weightReading);
		ret.Add(KidsTable.COLUMN_WEIGHT_SCIENCE, weightScience);
		ret.Add(KidsTable.COLUMN_WEIGHT_SOCIAL_STUDIES, weightSocialStudies);
		ret.Add(KidsTable.COLUMN_WEIGHT_COGNITIVE_DEVELOPMENT, weightCognitiveDevelopment);
		ret.Add(KidsTable.COLUMN_WEIGHT_CREATIVE_DEVELOPMENT, weightCreativeDevelopment);
		ret.Add(KidsTable.COLUMN_WEIGHT_LIFE_SKILLS, weightLifeSkills);
		ret.Add(KidsTable.COLUMN_LEVEL, level);
		ret.Add(KidsTable.COLUMN_GEM, gems);
		ret.Add(KidsTable.COLUMN_STAR, stars);
		if (photo != null)
			ret.Add(KidsTable.COLUMN_PHOTO, photo);
		ret.Add(KidsTable.LANGUAGE_COUNT, languageCount);
		ret.Add(KidsTable.VIDEO_WATCHED_COUNT, videoWatchedCount);
		ret.Add(KidsTable.GAME_PLAYED_COUNT, gamePlayedCount);

		return ret;
	}

//	private List<DataItem> HashtableToDataItem ( Hashtable p_table )
//	{
//		tempdataitems = new List<DataItem>(p_table.Count);
//		foreach (string key in p_table.Keys)
//		{
//			tempdataitems.Add(new DataItem(key, p_table[key].ToString()));
//		}
//		return tempdataitems;
//	}
//
//	private string SerializeJobData( Hashtable p_table )
//	{	
//		XmlSerializer serializer = new XmlSerializer(typeof(List<DataItem>));
//		StringWriter sw = new StringWriter();
//		XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
//		ns.Add("","");
//		
//		serializer.Serialize(sw, tempdataitems,ns);
//		
//		return sw.ToString();
//	}
//
//	private Hashtable DeserializeData(string RawData)
//	{
//		Hashtable myTable = new Hashtable();
//		XmlSerializer xs = new XmlSerializer(typeof(List<DataItem>));
//		StringReader sr = new StringReader(RawData);
//		
//		List<DataItem> templist = (List<DataItem>)xs.Deserialize(sr);
//		
//		foreach (DataItem di in templist)
//		{
//			myTable.Add(di.Key, di.Value);
//		}
//		return myTable;
//	}

	private List<string> separateChildName(string p_childName)
	{
		List<string> l_nameList = new List<string>();
		if(string.Empty.Equals(p_childName))
		{
			l_nameList.Add(string.Empty);
			l_nameList.Add(string.Empty);
		}
		else
		{
			if(p_childName.StartsWith(ZoodlesConstants.BLANK))
			{
				l_nameList.Add(string.Empty);
				l_nameList.Add(p_childName.Replace(ZoodlesConstants.BLANK,string.Empty));
			}
			else
			{
				if(p_childName.Contains(ZoodlesConstants.BLANK))
				{
					string[] l_nameArray = p_childName.Split(ZoodlesConstants.BLANK.ToCharArray());
					if(l_nameArray.Length >= 2)
					{
						l_nameList.Add(l_nameArray[0].ToString());
						l_nameList.Add(l_nameArray[1].ToString());
					}
					else
					{
						l_nameList.Add(string.Empty);
						l_nameList.Add(p_childName.Replace(ZoodlesConstants.BLANK,string.Empty));
					}
				}
				else
				{
					l_nameList.Add(p_childName);
					l_nameList.Add(string.Empty);
				}
			}
		}
		
		return l_nameList;
	}


    public int      id              { get; set; }
    public string   photo           { get; set; }
	public string   name            { get; set; }
	public string   wholeName       { get; set; }
    public bool     allowVideoMail  { get; set; }
    public string   birthday        { get; set; }
	public Texture2D kid_photo		{ get; set; }
	public int 		age				{ get; set; }

    public int      weightCognitiveDevelopment  { get; set; }
    public int      weightCreativeDevelopment   { get; set; }
    public int      weightLifeSkills            { get; set; }
    public int      weightMath                  { get; set; }
    public int      weightReading               { get; set; }
    public int      weightScience               { get; set; }
    public int      weightSocialStudies         { get; set; }
	public int		level						{ get; set; }
	public int      gems         				{ get; set; }
	public int      stars         				{ get; set; }
    public bool     timesUp                     { get; set; }

	public int		languageCount				{ get; set; }
	public int		videoWatchedCount			{ get; set; }
	public int		gamePlayedCount				{ get; set; }
	public App		topRecommendedApp			{ get; set; }
	public List<object> appList					{ get; set; }

    public ViolenceRating maxViolence           { get; set; }
//	public List<DataItem> tempdataitems;

    //-------------------- Private Implementation -------------------
	private void _requestPhotoComplete( WWW p_www )
	{
		if (p_www.error == null && p_www.texture.width != 8 && p_www.texture.height != 8)
		{
			kid_photo = p_www.texture;
			m_photoRequested = true;
			ImageCache.saveCacheImage(wholeName+".png", kid_photo);
		}
	}	

	public void dispose()
	{
		if (null != kid_photo)
		{
			GameObject.Destroy(kid_photo);
			kid_photo = null;
		}
	}

	private bool m_photoRequested = false;
}

public class KidsTable
{
    public const string TABLE_NAME       					= "kids";
                 
	public const string COLUMN_ID    						= "id";
	public const string COLUMN_PHOTO     					= "photo";
	public const string COLUMN_NAME 						= "name";
	public const string COLUMN_ALLOW_VIDEO_MAIL				= "allow_video_mail";
	public const string COLUMN_BIRTHDAY						= "birthday";
                 
	public const string COLUMN_MAX_VIOLENCE					= "max_violence";
	             
	public const string COLUMN_WEIGHT_COGNITIVE_DEVELOPMENT	= "weight_cognitive_development";
	public const string COLUMN_WEIGHT_CREATIVE_DEVELOPMENT	= "weight_creative_development";
	public const string COLUMN_WEIGHT_LIFE_SKILLS			= "weight_life_skills";
	public const string COLUMN_WEIGHT_MATH					= "weight_math";
	public const string COLUMN_WEIGHT_READING				= "weight_reading";
	public const string COLUMN_WEIGHT_SCIENCE				= "weight_science";
	public const string COLUMN_WEIGHT_SOCIAL_STUDIES		= "weight_social_studies";

	public const string COLUMN_GEM							= "gems";
	public const string COLUMN_STAR							= "zps";
	public const string COLUMN_LEVEL						= "level";

	public const string LANGUAGE_COUNT						= "language_count";
	public const string VIDEO_WATCHED_COUNT					= "videos_watched_count";
	public const string GAME_PLAYED_COUNT					= "games_played_coun";
}

