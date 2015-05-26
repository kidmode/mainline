using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProfileCanvas : UICanvas
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		SetupLocalizition ();
		_setupList();
	}

	public override void update()
	{
		base.update();
	}

	public override void enteringTransition(  )
	{
		base.enteringTransition( );
	}

	public override void exitingTransition( )
	{
		base.exitingTransition( );
	}
 
	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}

	//----------------- Private Implementation -------------------

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UIButton l_button = p_element as UIButton;
		DebugUtils.Assert( l_button != null );
		
		Kid l_kid = p_data as Kid;
		
		UILabel l_label = l_button.getView( "name" ) as UILabel;
		l_label.text = l_kid.name;
		
		if( l_kid.gems != ProfileInfo.ADD_PROFILE_CODE && null != l_kid.kid_photo )
		{
			UIImage l_image = l_button.getView( "avatarIcon" ) as UIImage;
			l_image.setTexture( l_kid.kid_photo );
		}
	}

	private void _setupList()
	{
		UISwipeList l_swipe = null;
		UISwipeList l_multiKidSwipe = getView( "profileSwipeList" ) as UISwipeList;
		UISwipeList l_oneKidSwipe = getView( "oneProfileSwipeList" ) as UISwipeList;
		List<Kid> l_kidList = SessionHandler.getInstance ().kidList;
		if(null!=l_kidList)
		{
			int l_kidCount = l_kidList.Count;
			if(1 == l_kidCount)
			{
				l_multiKidSwipe.active = false;
				l_oneKidSwipe.active = true;
				l_swipe = l_oneKidSwipe;
			}
			else
			{
				l_multiKidSwipe.active = true;
				l_oneKidSwipe.active = false;
				l_swipe = l_multiKidSwipe;
			}
			List< System.Object > infoData = new List< System.Object >();
			
			if (null != l_kidList)
			{
				for(int i = 0; i < l_kidCount; i++)
				{
					infoData.Add( l_kidList[i] );
				}
			}
			l_swipe.setData( infoData );
			l_swipe.setDrawFunction( onListDraw );
			l_swipe.redraw();
		}
	}
	
	private ProfileInfo _createNewProfile()
	{
		ProfileInfo newProfile = new ProfileInfo( "New Person", 0, 1, 0,null );
		return newProfile;
	}
	
	private void _setCurrentProfile( ProfileInfo p_profile )
	{
		
	}

	private void SetupLocalizition()
	{
		UILabel l_quitLabel 			= getView("quitButton")			.getView("btnText") as UILabel;
		UILabel l_facebookPostLabel 	= getView("facebookButton")		.getView("btnText") as UILabel;
		UILabel l_parentDashboardLabel 	= getView("parentDashButton")	.getView("btnText") as UILabel;
		UILabel l_emailInviteLabel 		= getView("inviteButton")		.getView("btnText") as UILabel;
		UILabel l_tryFreeLabel 			= getView("tryFreeButton")		.getView("btnText") as UILabel;
		UILabel l_addChildLabel 		= getView("addChildButton")		.getView("btnText") as UILabel;

		UILabel l_titleLabel = getView("introLabel") as UILabel;
		UILabel l_termsLabel = getView("termsLabel") as UILabel;
		UILabel l_polocyLabel = getView("policyLabel") as UILabel;

		l_quitLabel.text 			= Localization.getString(Localization.TXT_BUTTON_QUIT);
		l_facebookPostLabel.text 	= Localization.getString(Localization.TXT_BUTTON_FACEBOOK);
		l_parentDashboardLabel.text = Localization.getString(Localization.TXT_BUTTON_PARENT_DASHBOARD);
		l_emailInviteLabel.text 	= Localization.getString(Localization.TXT_BUTTON_EMAIL_INVITE);
		l_tryFreeLabel.text 		= Localization.getString(Localization.TXT_2_LABEL_TRY);

		l_titleLabel.text = Localization.getString(Localization.TXT_LABEL_PROFILE_INFO);
		l_termsLabel.text = Localization.getString(Localization.TXT_2_LABEL_TERMS);
		l_polocyLabel.text = Localization.getString(Localization.TXT_2_LABEL_POLICY);
		l_addChildLabel.text = Localization.getString(Localization.TXT_STATE_1_ADD_CHILD);
	}

	private Texture2D m_addChildTex;
}
