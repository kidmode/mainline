using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SignUpAfterInoutCreditCardCanvas : UICanvas 
{
	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );
		
		tweener.addAlphaTrack( 0.0f, 1.0f, ZoodlesScreenFactory.FADE_SPEED );

		m_dialog = getView ("dialog") as UIElement;
		m_dialogMovePosition = 800;
		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_title = getView ("setupDialogTitleText") as UILabel;
		l_title.text = Localization.getString (Localization.TXT_102_LABEL_TITLE);
		UILabel l_facebookText = getView ("useFacebookButtonText") as UILabel;
		l_facebookText.text = Localization.getString (Localization.TXT_102_BUTTON_FACEBOOK);
		UILabel l_noticeText = getView ("noticeText") as UILabel;
		l_noticeText.text = Localization.getString (Localization.TXT_102_LABEL_USEADDRESS);
		UILabel l_createFreeButton = getView("createFreeAccountButton").getView ("createAccountText") as UILabel;
		l_createFreeButton.text = Localization.getString (Localization.TXT_102_BUTTON_CREATE_FREE_ACCOUNT);
		UILabel l_createButton = getView("createAccountButton").getView ("createAccountText") as UILabel;
		l_createButton.text = Localization.getString (Localization.TXT_102_BUTTON_CREATE_ACCOUNT);
		UILabel l_backButton = getView("backButton").getView ("btnText") as UILabel;
		l_backButton.text = Localization.getString (Localization.TXT_BUTTON_BACK);

		
		UILabel l_eamiltext = getView ("emailPlaceholder") as UILabel;
		l_eamiltext.text = Localization.getString (Localization.TXT_102_INPUTFIELD_EMAILPLACEHOLDER);
		UILabel l_pwdtext = getView("pwdPlaceholder") as UILabel;
		l_pwdtext.text = Localization.getString (Localization.TXT_102_INPUTFIELD_PWDPLACEHOLDER);
		UILabel l_repwdtext = getView("repwdPlaceholder") as UILabel;
		l_repwdtext.text = Localization.getString (Localization.TXT_102_INPUTFIELD_REPWDPLACEHOLDER);
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
		tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
	}

	public void setOriginalPosition()
	{
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_dialog.transform.localPosition );
		l_pointListOut.Add( m_dialog.transform.localPosition - new Vector3( 0, m_dialogMovePosition, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	public void setOutPosition()
	{
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_dialog.transform.localPosition );
		l_pointListOut.Add( m_dialog.transform.localPosition + new Vector3( 0, m_dialogMovePosition, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f );
	}
	
	//-- Private Implementation --
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}

	private UIElement m_dialog;
	private int m_dialogMovePosition;
}
