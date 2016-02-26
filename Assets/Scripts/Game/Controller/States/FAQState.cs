using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FAQState : GameState 
{
	//Public variables
	
	//Standard state flow	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );
		
		m_requestQueue = new RequestQueue ();

		_setupScreen( p_gameController.getUI() );

	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		p_gameController.getUI().removeScreen( m_faqCanvas );
		p_gameController.getUI().removeScreen( m_faqDialogCanvas );
	}
	
	//---------------- Private Implementation ----------------------

	private void _setupScreen( UIManager p_uiManager )
	{
		m_faqDialogCanvas = p_uiManager.createScreen (UIScreen.FAQ_DIALOG, true, 12);
		m_faqCanvas = p_uiManager.createScreen( UIScreen.FAQ_SCREEN, true, 2 ) as UICanvas;

		m_faqSwipeList = m_faqCanvas.getView ("FAQSwipeList") as UISwipeList;
		m_faqSwipeList.addClickListener ( "Prototype", showDialog);


		m_questionLabel = m_faqDialogCanvas.getView("questionText") as UILabel;
	//	m_answerLabel 	= m_faqDialogCanvas.getView("answerText") 	as UILabel;
		m_titleLabel 	= m_faqDialogCanvas.getView("titleText") 	as UILabel;
		m_exitButton 	= m_faqDialogCanvas.getView("exitButton") 	as UIButton;
		m_exitButton.addClickCallback (onCloseDialog);

	}

	private void onCloseDialog(UIButton p_button)
	{
		m_gameController.getUI().changeScreen(UIScreen.FAQ_DIALOG,false);
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_newPanel = m_faqDialogCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_newPanel.transform.localPosition );
		l_pointListOut.Add( l_newPanel.transform.localPosition - new Vector3( 0, dialogDisplacement, 0 ));
		l_newPanel.tweener.addPositionTrack(l_pointListOut, 0.0f);
	}

	private void showDialog( UISwipeList p_list, UIButton p_button, System.Object p_data, int p_index )
	{
		m_gameController.getUI ().changeScreen (UIScreen.FAQ_DIALOG,true);
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_newPanel = m_faqDialogCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_newPanel.transform.localPosition );
		l_pointListIn.Add( l_newPanel.transform.localPosition + new Vector3( 0, dialogDisplacement, 0 ));
		l_newPanel.tweener.addPositionTrack(l_pointListIn, 0.0f);

		string[] l_data = p_data as string[];

		m_questionLabel.text = l_data[1];
		m_titleLabel.text = "FAQ" + (p_index + 1).ToString ();

		//faqcontent

		UILabel l_faqContent = m_faqDialogCanvas.getView (l_data[0].ToString()) as UILabel;

		if (null != l_faqContent)
		{
			if(null != m_showFaq)
				m_showFaq.active = false;
			l_faqContent.active = true;
			m_showFaq = l_faqContent;

			ScrollRect scrollFaq = l_faqContent.gameObject.GetComponentInChildren<ScrollRect>();

			scrollFaq.verticalNormalizedPosition = 1.0f;

		}
	}




	private bool checkInternet()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable 
		    || KidMode.isAirplaneModeOn() || !KidMode.isWifiConnected())
		{
			m_gameController.getUI().createScreen(UIScreen.ERROR_MESSAGE, false, 6);
			
			ErrorMessage error = GameObject.FindWithTag("ErrorMessageTag").GetComponent<ErrorMessage>() as ErrorMessage;
			if (error != null)
				error.onClick += onClickExit;
			
			return false;
		}
		return true;
	}
	
	private void onClickExit()
	{
		ErrorMessage error = GameObject.FindWithTag("ErrorMessageTag").GetComponent<ErrorMessage>() as ErrorMessage;
		error.onClick -= onClickExit;;
		m_gameController.changeState (ZoodleState.CONTROL_APP);
	}


	//Private variables

	private UICanvas 	m_faqDialogCanvas;
	private UICanvas    m_faqCanvas;

	private UISwipeList m_faqSwipeList;
	
	private UIButton    m_childModeButton;

	private RequestQueue m_requestQueue;

	private bool 		canMoveLeftMenu = true;

	//dialog part
	private UILabel 	m_questionLabel;
	private UILabel 	m_answerLabel;
	private UILabel 	m_titleLabel;
	private UIButton 	m_exitButton;
	private UILabel 	m_showFaq;
	
	//Kevin
	private float dialogDisplacement = 1090.0f;

}