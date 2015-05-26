using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarChartCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		
		SetupLocalizition ();

		m_starIcon = Resources.Load( "GUI/2048/common/icon/icon_star_rating" ) as Texture2D;
		m_noStarIcon = Resources.Load( "GUI/2048/common/bgCards/bg_star_rating" ) as Texture2D;
	}

	public override void update ()
	{
		base.update ();
	}

	public override void dispose (bool p_deep)
	{
		base.dispose (p_deep);
	}

	public override void enteringTransition ()
	{
		base.enteringTransition ();
	}

	public override void exitingTransition ()
	{
		base.exitingTransition ();
	}

	public void setData( List<System.Object> p_info )
	{
		UISwipeList l_swipe = getView ( "starSwipeList" ) as UISwipeList;
		l_swipe.setDataWithoutMove ( p_info );
		l_swipe.setDrawFunction ( onListDraw );
		l_swipe.redraw ();
	}

	public void setRootTable(Hashtable p_table)
	{
		m_rootTable.Clear ();
		m_rootTable = p_table.Clone() as Hashtable;
	}

	//----------------- Private Implementation -------------------

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UIElement l_prototype = p_element;
		DebugUtils.Assert( l_prototype != null );

		//TODO
		StarChartInfo l_info = p_data as StarChartInfo;

		UILabel l_contentLabel	 = l_prototype.getView( "contentText" ) as UILabel;
		UIElement l_starList	 = l_prototype.getView( "starList" );

		l_contentLabel.text = m_rootTable[l_info.id.ToString()].ToString();

		if( l_info.starCount >= 1 )
		{
			UIImage l_starOne = l_starList.getView("starOne") as UIImage;
			l_starOne.setTexture( m_starIcon );
		}
		else
		{
			UIImage l_starOne = l_starList.getView("starOne") as UIImage;
			l_starOne.setTexture( m_noStarIcon );
		}

		if( l_info.starCount >= 2 )
		{
			UIImage l_starTwo = l_starList.getView("starTwo") as UIImage;
			l_starTwo.setTexture( m_starIcon );
		}
		else
		{
			UIImage l_starTwo = l_starList.getView("starTwo") as UIImage;
			l_starTwo.setTexture( m_noStarIcon );
		}

		if( l_info.starCount >= 3 )
		{
			UIImage l_starThree = l_starList.getView("starThree") as UIImage;
			l_starThree.setTexture( m_starIcon );
		}
		else
		{
			UIImage l_starThree = l_starList.getView("starThree") as UIImage;
			l_starThree.setTexture( m_noStarIcon );
		}

		if( l_info.starCount >= 4 )
		{
			UIImage l_starFour = l_starList.getView("starFour") as UIImage;
			l_starFour.setTexture( m_starIcon );
		}
		else
		{
			UIImage l_starFour = l_starList.getView("starFour") as UIImage;
			l_starFour.setTexture( m_noStarIcon );
		}

		if( l_info.starCount >= 5 )
		{
			UIImage l_starFive = l_starList.getView("starFive") as UIImage;
			l_starFive.setTexture( m_starIcon );
		}
		else
		{
			UIImage l_starFive = l_starList.getView("starFive") as UIImage;
			l_starFive.setTexture( m_noStarIcon );
		}
	}

	private void SetupLocalizition()
	{
		UILabel l_top = getView ("titleText") as UILabel;
		
		l_top.text = Localization.getString (Localization.TXT_67_LABEL_TITLE);

		UILabel l_math = getView("mathToggle").getView("Text") as UILabel;
		UILabel l_reading = getView("readingToggle").getView("Text") as UILabel;
		UILabel l_science = getView("scienceToggle").getView("Text") as UILabel;
		UILabel l_social = getView("socialToggle").getView("Text") as UILabel;
		UILabel l_cognitive = getView("cognitiveToggle").getView("Text") as UILabel;
		UILabel l_creative = getView("creativeToggle").getView("Text") as UILabel;
		UILabel l_life = getView("lifeSkillsToggle").getView("Text") as UILabel;

		l_math.text = Localization.getString( Localization.TXT_COURSE_MATH );
		l_reading.text = Localization.getString( Localization.TXT_COURSE_READING );
		l_science.text = Localization.getString( Localization.TXT_COURSE_SCIENCE );
		l_social.text = Localization.getString( Localization.TXT_COURSE_SOCIAL );
		l_cognitive.text = Localization.getString( Localization.TXT_COURSE_COGNITIVE );
		l_creative.text = Localization.getString( Localization.TXT_COURSE_CREATIVE );
		l_life.text = Localization.getString( Localization.TXT_COURSE_LIFE );
	}
	
	private Texture2D m_starIcon;
	private Texture2D m_noStarIcon;

	private Hashtable m_rootTable = new Hashtable();
}

public class StarChartInfo : System.Object
{
	public int id;
	public int starCount;

	public StarChartInfo()
	{}

	public StarChartInfo(int p_id, int p_starCount)
	{
		id = p_id;
		starCount = p_starCount;
	}
}