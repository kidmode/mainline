using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectAvatarCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );

		SetupLocalizition ();
		
		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );

		m_textureDown = Resources.Load("GUI/2048/common/buttons/bt_circle_down") as Texture2D;
		m_textureUp = Resources.Load("GUI/2048/common/buttons/bt_circle_up") as Texture2D;
		_setupList();
	}
	
	public override void update()
	{
		base.update();
	}
	
	public override void dispose( bool p_deep )
	{
		base.dispose( p_deep );
	}

	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UIButton l_button = p_element as UIButton;
		DebugUtils.Assert( l_button != null );
		
		AvatarButton l_profile = p_data as AvatarButton;
		Texture2D l_childAvatar = Resources.Load(l_profile.path) as Texture2D;
		DebugUtils.Assert( l_childAvatar != null );

		UIImage l_image = l_button.getView( "avatarIcon" ) as UIImage;
		l_image.setTexture( l_childAvatar );
		if(l_profile.isSelected)
		{
			l_button.buttonImage = m_textureDown;
		}
		else
		{
			l_button.buttonImage = m_textureUp;
		}
	}

	private void _setupList()
	{
		string l_imagePath = "GUI/2048/common/avatars/";
		UISwipeList l_swipe = getView( "avatarSwipeList" ) as UISwipeList;
		
		List< System.Object > infoData = new List< System.Object >();
		infoData.Add (new AvatarButton(l_imagePath + "icon_avatar_m1","icon_avatar_m1",AvatarImgType.AVATAR));
		infoData.Add (new AvatarButton(l_imagePath + "icon_avatar_m2","icon_avatar_m2",AvatarImgType.AVATAR));
		infoData.Add (new AvatarButton(l_imagePath + "icon_avatar_m3","icon_avatar_m3",AvatarImgType.AVATAR));
		infoData.Add (new AvatarButton(l_imagePath + "icon_avatar_m4","icon_avatar_m4",AvatarImgType.AVATAR));
		infoData.Add (new AvatarButton(l_imagePath + "icon_avatar_f1","icon_avatar_f1",AvatarImgType.AVATAR));
		infoData.Add (new AvatarButton(l_imagePath + "icon_avatar_f2","icon_avatar_f2",AvatarImgType.AVATAR));
		infoData.Add (new AvatarButton(l_imagePath + "icon_avatar_f3","icon_avatar_f3",AvatarImgType.AVATAR));
		infoData.Add (new AvatarButton(l_imagePath + "icon_avatar_f4","icon_avatar_f4",AvatarImgType.AVATAR));
		infoData.Add (new AvatarButton(l_imagePath + "profile_animal_bird","profile_animal_bird",AvatarImgType.AVATAR));
		infoData.Add (new AvatarButton(l_imagePath + "profile_animal_elephant","profile_animal_elephant",AvatarImgType.AVATAR));
		infoData.Add (new AvatarButton(l_imagePath + "profile_animal_giraffe","profile_animal_giraffe",AvatarImgType.AVATAR));
		infoData.Add (new AvatarButton(l_imagePath + "profile_animal_lion","profile_animal_lion",AvatarImgType.AVATAR));
		infoData.Add (new AvatarButton(l_imagePath + "profile_animal_monkey","profile_animal_monkey",AvatarImgType.AVATAR));
		infoData.Add (new AvatarButton(l_imagePath + "profile_animal_tiger","profile_animal_tiger",AvatarImgType.AVATAR));

		l_swipe.setData( infoData );
		l_swipe.setDrawFunction( onListDraw );
		l_swipe.redraw();
	}
	
	public override void enteringTransition()
	{
		base.enteringTransition();
		tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
	}
	

	//-- Private Implementation --
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private void SetupLocalizition()
	{
		UILabel l_back 		= getView ("backButton").getView("btnText") as UILabel;
		UILabel l_top 		= getView ("topic") as UILabel;
		UILabel l_notice 	= getView ("notice") as UILabel;
		UILabel l_save 		= getView ("saveBtnText") as UILabel;
		
		l_back.text 	= Localization.getString (Localization.TXT_BUTTON_BACK);
		l_top.text 		= Localization.getString (Localization.TXT_24_LABEL_TOP);
		l_notice.text 	= Localization.getString (Localization.TXT_24_LABEL_NOTICE);
		l_save.text 	= Localization.getString (Localization.TXT_24_LABEL_SAVE);
	}

	private Texture2D m_addChildTex;
	private Texture2D m_textureDown;
	private Texture2D m_textureUp;
}

public class AvatarButton : System.Object
{
	public const int ADD_PROFILE_CODE = -666;
	
	public string name;
	public string path;
	public AvatarImgType avatarImgType;
	public bool	isSelected = false;

	
	public AvatarButton( string p_path, string p_name,AvatarImgType p_type )
	{
		name 			= p_name;
		path 			= p_path;
		avatarImgType 	= p_type;
	}
	
}

public enum AvatarImgType
{
	AVATAR,
	PHOTO
}
