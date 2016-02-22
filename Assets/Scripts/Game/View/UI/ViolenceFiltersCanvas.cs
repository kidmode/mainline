using UnityEngine;
using System.Collections;

public class ViolenceFiltersCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		
		SetupLocalizition ();

		if( SessionHandler.getInstance().token.isPremium() || SessionHandler.getInstance().token.isCurrent() )
		{
			_setupElement();
			_setupData();
		}





	}
	
	public override void update ()
	{
		base.update ();
	}
	
	public override void dispose (bool p_deep)
	{
		base.dispose (p_deep);

		ControlViolenceState.onControlValueChangedToTrue -= onControlValueChangedToTrue;
	}
	
	public override void enteringTransition ()
	{
		base.enteringTransition ();
		tweener.addAlphaTrack( 1.0f, 0.0f, 0.0f, onFadeFinish );
	}
	
	public override void exitingTransition ()
	{
		base.exitingTransition ();
	}

	#region Event
	//-----Event
	//Kevin
	private void onControlValueChangedToTrue(){

		mSaveButton.enabled = true;
		
		if(SessionHandler.getInstance().token.isPremium()){
			
			m_iconLock.gameObject.SetActive(false);
			
		}else {
			
			m_iconLock.gameObject.SetActive(true);
			
		}

	}

	#endregion
	
	//------------------ Private Implementation --------------------
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}
	
	private void _setupElement()
	{
		m_levelZeroToggle 	= getView( "levelZeroToggle" )	 as UIToggle;
		m_levelOneToggle 	= getView( "levelOneToggle" )	 as UIToggle;
		m_levelTwoToggle 	= getView( "levelTwoToggle" )	 as UIToggle;
		m_levelThreeToggle 	= getView( "levelThreeToggle" )	 as UIToggle;
		m_levelFourToggle 	= getView( "levelFourToggle" )	 as UIToggle;

		//New Save Button
		mSaveButton = getView ("saveButton") as UIButton;
		
		//Kevin, set save button to gray / not interative at the start
		mSaveButton.enabled = false;

		
		//Setup event so we know when the setttings cache has changed
		ControlViolenceState.onControlValueChangedToTrue += onControlValueChangedToTrue;


		m_iconLock = getView ("lockIcon") as UIImage;
		
		m_iconLock.gameObject.SetActive(false);


	}

	private void _setupData()
	{
		if (SessionHandler.getInstance ().kidList.Count > 0 && SessionHandler.getInstance ().currentKid == null )
		{
			SessionHandler.getInstance ().currentKid = SessionHandler.getInstance ().kidList [0];
		}
		Kid l_kid = SessionHandler.getInstance ().currentKid;

		switch( l_kid.maxViolence )
		{
			case ViolenceRating.NoViolence:
				m_levelZeroToggle.isOn = true;
				break;
			case ViolenceRating.ViolentInnuendos:
				m_levelOneToggle.isOn = true;
				break;
			case ViolenceRating.ExplosionsButNoVisibleWeapons:
				m_levelTwoToggle.isOn = true;
				break;
			case ViolenceRating.VisibleWeapons:
				m_levelThreeToggle.isOn = true;
				break;
			case ViolenceRating.SimulatedPhysicalViolence:
				m_levelFourToggle.isOn = true;
				break;
		}
	}

	private void SetupLocalizition()
	{
		UILabel l_top = getView("titleText") as UILabel;
		UILabel l_titleLevel1 = getView("levelOneToggle").getView("toogleLabel") as UILabel;
		UILabel l_titleLevel2 = getView("levelTwoToggle").getView("toogleLabel") as UILabel;
		UILabel l_titleLevel3 = getView("levelThreeToggle").getView("toogleLabel") as UILabel;
		UILabel l_titleLevel4 = getView("levelFourToggle").getView("toogleLabel") as UILabel;
		UILabel l_titleLevel0 = getView("levelZeroToggle").getView("toogleLabel") as UILabel;
		UILabel l_contentLevel1 = getView("levelOneToggle").getView("contentLabel") as UILabel;
		UILabel l_contentLevel2 = getView("levelTwoToggle").getView("contentLabel") as UILabel;
		UILabel l_contentLevel3 = getView("levelThreeToggle").getView("contentLabel") as UILabel;
		UILabel l_contentLevel4 = getView("levelFourToggle").getView("contentLabel") as UILabel;
		UILabel l_contentLevel0 = getView("levelZeroToggle").getView("contentLabel") as UILabel;

		l_top.text = Localization.getString( Localization.TXT_64_LABEL_TITLE );
		l_titleLevel1.text = Localization.getString( Localization.TXT_64_LABEL_TITLE_LEVEL1 );
		l_titleLevel2.text = Localization.getString( Localization.TXT_64_LABEL_TITLE_LEVEL2 );
		l_titleLevel3.text = Localization.getString( Localization.TXT_64_LABEL_TITLE_LEVEL3 );
		l_titleLevel4.text = Localization.getString( Localization.TXT_64_LABEL_TITLE_LEVEL4 );
		l_titleLevel0.text = Localization.getString( Localization.TXT_64_LABEL_TITLE_LEVEL0 );
		l_contentLevel1.text = Localization.getString( Localization.TXT_64_LABEL_CONTENT_LEVEL1 );
		l_contentLevel2.text = Localization.getString( Localization.TXT_64_LABEL_CONTENT_LEVEL2 );
		l_contentLevel3.text = Localization.getString( Localization.TXT_64_LABEL_CONTENT_LEVEL3 );
		l_contentLevel4.text = Localization.getString( Localization.TXT_64_LABEL_CONTENT_LEVEL4 );
		l_contentLevel0.text = Localization.getString( Localization.TXT_64_LABEL_CONTENT_LEVEL0 );
	}
	
	private UIToggle m_levelZeroToggle;
	private UIToggle m_levelOneToggle;
	private UIToggle m_levelTwoToggle;
	private UIToggle m_levelThreeToggle;
	private UIToggle m_levelFourToggle;


	//Kevin
	//New Save Button
	private UIButton mSaveButton;

	private UIImage m_iconLock;

}
