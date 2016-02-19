using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConfirmDialogCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);

		m_dialog = getView ("mainPanel") as UIElement;
		m_dialogMovePosition = 1660;

		setupLocalization ();
	}
	
	public void setupLocalization()
	{
		UILabel l_title = getView("titleArea").getView ("titleText") as UILabel;
		l_title.text =  Localization.getString (Localization.TXT_75_LABEL_TITLE);
		UILabel l_notice1 = getView("noticeText1") as UILabel;
		l_notice1.text =  Localization.getString (Localization.TXT_75_LABEL_NOTICE1);
		UILabel l_notice2 = getView("noticeText2") as UILabel;
		l_notice2.text =  Localization.getString (Localization.TXT_75_LABEL_NOTICE2);
		UILabel l_notice3 = getView("noticeText3") as UILabel;
		l_notice3.text =  Localization.getString (Localization.TXT_75_LABEL_NOTICE3);
		UILabel l_notice4 = getView("noticeText4") as UILabel;
		l_notice4.text =  Localization.getString (Localization.TXT_75_LABEL_NOTICE4);
		UILabel l_buyGems =getView("needMoreArea").getView("confirmButtonText") as UILabel;
		l_buyGems.text =  Localization.getString (Localization.TXT_75_BUTTON_BUY_GEMS);
		UILabel l_confirm =getView("costArea").getView("confirmButtonText") as UILabel;
		l_confirm.text =  Localization.getString (Localization.TXT_75_BUTTON_CONFIRM);
		UILabel l_cancel = getView("cancelButtonText") as UILabel;
		l_cancel.text =  Localization.getString (Localization.TXT_75_BUTTON_CANCEL);
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

	public void setUIManager(UIManager p_UIManager)
	{
		m_uiManager = p_UIManager;
	}

	public void moveInDialog()
	{
		if(null != m_uiManager)
		{
			m_uiManager.changeScreen(UIScreen.CONFIRM_DIALOG, true);
		}
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_dialog.transform.localPosition );
		l_pointListOut.Add( m_dialog.transform.localPosition + new Vector3( 0, m_dialogMovePosition, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f );
	}
	
	public void moveOutDialog()
	{
		if(null != m_uiManager)
		{
			m_uiManager.changeScreen(UIScreen.CONFIRM_DIALOG, false);
		}
		List<Vector3> l_pointListOut = new List<Vector3>();
		l_pointListOut.Add( m_dialog.transform.localPosition );
		l_pointListOut.Add( m_dialog.transform.localPosition - new Vector3( 0, m_dialogMovePosition, 0 ));
		m_dialog.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	private UIManager m_uiManager;
	private UIElement m_dialog;
	private int m_dialogMovePosition;
}
