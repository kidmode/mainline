using UnityEngine;
using System.Collections;
using System;

public class DashBoardProfileInfoCanvas : UICanvas
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		SetupLocalizition ();
		
		_setupProfileInfo( );
		refreshInfo();
	}
	
	public override void update()
	{
		base.update();
	}
	
	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}
	
	public override void enteringTransition()
	{
		base.enteringTransition();
		tweener.addAlphaTrack( 1.0f, 0.0f, 0.0f, onFadeFinish );
	}

	public void refreshInfo()
	{
		Kid l_kid = SessionHandler.getInstance ().currentKid;

		m_childAvatar.setTexture( l_kid.kid_photo );
		m_childNameLabel.text 	= l_kid.name;
		m_starNumberLabel.text 	= l_kid.stars.ToString("N0");
//		m_gemNumberLabel.text 	= l_kid.gems.ToString("N0");
		m_levelLabel.text 		= l_kid.level.ToString ();
		int l_age = 0;
		DateTime l_date = DateTime.Parse (l_kid.birthday);
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
		l_kid.age = l_age;
		m_ageLabel.text 		= l_kid.age.ToString();
	}

	//------------------ Private Implementation --------------------
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private void _setupProfileInfo( )
	{
		GameObject l_obj 	= GameObject.FindWithTag( "GameController" );
		Game l_game 		= l_obj.GetComponent< Game >();
		DebugUtils.Assert( l_game != null );

		if (SessionHandler.getInstance ().kidList.Count > 0 && null == SessionHandler.getInstance ().currentKid)
		{
			SessionHandler.getInstance ().currentKid = SessionHandler.getInstance ().kidList [0];
		}

		m_childAvatar 			= getView( "avatarIcon" ) 	as UIImage;
		m_childNameLabel 		= getView( "childName" 	) 	as UILabel;
		m_starNumberLabel 		= getView( "zoodlePointsText" ) 	as UILabel;
//		m_gemNumberLabel 		= getView( "gemsText" 	) 	as UILabel;
		m_ageLabel 				= getView( "ageText" ) 		as UILabel;
		m_levelLabel 			= getView( "levelText" ) 	as UILabel;
	}

	private void SetupLocalizition()
	{
		UILabel l_title = getView("titleText") as UILabel;
		UILabel l_edit = getView("editProfileButton").getView("Text") as UILabel;
		UILabel l_zp = getView("zoodlePointsTitle") as UILabel;
//		UILabel l_gems = getView("gemsTitle") as UILabel;
		UILabel l_app = getView("appPartTitle") as UILabel;
		UILabel l_subjects = getView("subjectsText") as UILabel;
		UILabel l_free = getView("appFreeText") as UILabel;
		UILabel l_sponsored = getView("sponsoredText") as UILabel;

		UILabel l_years = getView("ageContent").getView("Text") as UILabel;
		
		l_title.text = Localization.getString( Localization.TXT_56_LABEL_TITLE );
		l_edit.text = Localization.getString( Localization.TXT_56_LABEL_EDIT );
		l_zp.text = Localization.getString( Localization.TXT_56_LABEL_ZP );
//		l_gems.text = Localization.getString( Localization.TXT_56_LABEL_GEMS );
		l_app.text = Localization.getString( Localization.TXT_56_LABEL_APP );
		l_subjects.text = Localization.getString( Localization.TXT_56_LABEL_SUBJECTS );
		l_free.text = Localization.getString( Localization.TXT_56_LABEL_FREE );
		l_sponsored.text = Localization.getString( Localization.TXT_56_LABEL_SPONSORED );

		l_years.text = Localization.getString( Localization.TXT_56_LABEL_YEARS );
	}

	
	private UIImage m_childAvatar;
	
	private UILabel m_childNameLabel;
	private UILabel m_starNumberLabel;
//	private UILabel m_gemNumberLabel;
	private UILabel m_ageLabel;
	private UILabel m_levelLabel;
}
