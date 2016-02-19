using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppDetailsCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);

		m_dialog = getView ("mainPanel") as UIElement;
		m_dialogMovePosition = 800;

		SetupLocalizition ();
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

	private void SetupLocalizition()
	{
		UILabel l_top = getView ("titleArea").getView("Text") as UILabel;
		UILabel l_safety = getView ("safetyLabel") as UILabel;
		UILabel l_age = getView ("ageLabel") as UILabel;

		l_top.text = Localization.getString (Localization.TXT_70_LABEL_TITLE);
		l_safety.text = Localization.getString (Localization.TXT_70_LABEL_SAFETY);
		l_age.text = Localization.getString (Localization.TXT_70_LABEL_AGES);
	}

	public void moveInDialog()
	{
		if(null != m_uiManager)
		{
			m_uiManager.changeScreen(UIScreen.APP_DETAILS, true);
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
			m_uiManager.changeScreen(UIScreen.APP_DETAILS, false);
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
