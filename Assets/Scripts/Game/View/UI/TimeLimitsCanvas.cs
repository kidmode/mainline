using UnityEngine;
using System.Collections;

public class TimeLimitsCanvas : UICanvas
{
	public override void init (GameObject p_gameObject)
	{
		base.init (p_gameObject);
		
		SetupLocalizition ();
		
		_setupElement();



	}
	
	public override void update ()
	{
		base.update ();
	}
	
	public override void dispose (bool p_deep)
	{
		base.dispose (p_deep);

		ControlTimeState.onControlValueChanged -= onControlValueChanged;

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
	
	public void setTimeLimits( Hashtable p_hashTable )
	{
		DebugUtils.Assert( p_hashTable != null);
		
		if( 0 == p_hashTable.Count )
			return;
		
		m_data = p_hashTable;

		_setupData();

		ControlTimeState.onControlValueChanged += onControlValueChanged;

	}

	#region Event
	//-----Event
	//Kevin
	private void onControlValueChanged(bool value){
		
		if(value){
			
			mSaveButton.enabled = true;
			
			if(SessionHandler.getInstance().token.isPremium()){
				
				m_iconLock.gameObject.SetActive(false);
				
			}else {
				
				m_iconLock.gameObject.SetActive(true);
				
			}
			
		}else {
			
			mSaveButton.enabled = false;
			
		}
		
	}
	
	#endregion
	
	//------------------ Private Implementation --------------------
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
	{
		UICanvas l_canvas = p_element as UICanvas;
		l_canvas.isTransitioning = false;
	}
	
	private void _setupElement( )
	{
		m_weekGroup 			= getView( "weekGroup" ) as UIToggleGroup;
		m_weekThirtyMin 		= m_weekGroup.getView( "thirtyMinToggle" )	 as UIToggle;
		m_weekOneHour 			= m_weekGroup.getView( "oneHourToggle" )	 as UIToggle;
		m_weekTwoHours 			= m_weekGroup.getView( "twoHoursToggle" )	 as UIToggle;
		m_weekFourHours 		= m_weekGroup.getView( "fourHoursToggle" )	 as UIToggle;
		m_weekUnlimited 		= m_weekGroup.getView( "unlimitedToggle" )	 as UIToggle;
		
		m_weekendGroup 			= getView( "weekendGroup" ) as UIToggleGroup;
		m_weekendThirtyMin 		= m_weekendGroup.getView( "thirtyMinToggle" )	 as UIToggle;
		m_weekendOneHour 		= m_weekendGroup.getView( "oneHourToggle" )		 as UIToggle;
		m_weekendTwoHours 		= m_weekendGroup.getView( "twoHoursToggle" )	 as UIToggle;
		m_weekendFourHours 		= m_weekendGroup.getView( "fourHoursToggle" )	 as UIToggle;
		m_weekendUnlimited 		= m_weekendGroup.getView( "unlimitedToggle" )	 as UIToggle;

		//New Save Button
		mSaveButton = getView ("saveButton") as UIButton;
		
		//Kevin, set save button to gray / not interative at the start
		mSaveButton.enabled = false;

		
		m_iconLock = getView ("lockIcon") as UIImage;
		
		m_iconLock.gameObject.SetActive(false);

	}
	
	private void _setupData()
	{	
		if( (bool)m_data[ "weekday_disabled" ] )
		{
			m_weekUnlimited.isOn = true;
		}
		else
		{
			switch( m_data[ "weekday_limit" ].ToString() )
			{
			case "30" :
				m_weekThirtyMin.isOn = true;
				break;
			case "60" :
				m_weekOneHour.isOn = true;
				break;
			case "120" :
				m_weekTwoHours.isOn = true;
				break;
			case "240" :
				m_weekFourHours.isOn = true;
				break;
			default :
				break;
			}
		}

		if( (bool)m_data[ "weekend_disabled" ] )
		{
			m_weekendUnlimited.isOn = true;
		}
		else
		{
			switch( m_data[ "weekend_limit" ].ToString() )
			{
			case "30" :
				m_weekendThirtyMin.isOn = true;
				break;
			case "60" :
				m_weekendOneHour.isOn = true;
				break;
			case "120" :
				m_weekendTwoHours.isOn = true;
				break;
			case "240" :
				m_weekendFourHours.isOn = true;
				break;
			default :
				break;
			}
		}
	}

	private void SetupLocalizition()
	{
		UILabel l_top = getView("titleText") as UILabel;
		UILabel l_week = getView("weekLabel") as UILabel;
		UILabel l_weekend = getView("weekendLabel") as UILabel;
		UILabel l_week30 = getView("weekGroup").getView("thirtyMinToggle").getView("toogleLabel") as UILabel;
		UILabel l_week1 = getView("weekGroup").getView("oneHourToggle").getView("toogleLabel") as UILabel;
		UILabel l_week2 = getView("weekGroup").getView("twoHoursToggle").getView("toogleLabel") as UILabel;
		UILabel l_week4 = getView("weekGroup").getView("fourHoursToggle").getView("toogleLabel") as UILabel;
		UILabel l_week0 = getView("weekGroup").getView("unlimitedToggle").getView("toogleLabel") as UILabel;
		UILabel l_weekend30 = getView("weekendGroup").getView("thirtyMinToggle").getView("toogleLabel") as UILabel;
		UILabel l_weekend1 = getView("weekendGroup").getView("oneHourToggle").getView("toogleLabel") as UILabel;
		UILabel l_weekend2 = getView("weekendGroup").getView("twoHoursToggle").getView("toogleLabel") as UILabel;
		UILabel l_weekend4 = getView("weekendGroup").getView("fourHoursToggle").getView("toogleLabel") as UILabel;
		UILabel l_weekend0 = getView("weekendGroup").getView("unlimitedToggle").getView("toogleLabel") as UILabel;
		
		l_top.text = Localization.getString( Localization.TXT_63_LABEL_TITLE );
		l_week.text = Localization.getString( Localization.TXT_63_LABEL_WEEK );
		l_weekend.text = Localization.getString( Localization.TXT_63_LABEL_WEEKEND );
		l_week30.text = Localization.getString( Localization.TXT_63_LABEL_HALF );
		l_week1.text = Localization.getString( Localization.TXT_63_LABEL_ONE );
		l_week2.text = Localization.getString( Localization.TXT_63_LABEL_TWO );
		l_week4.text = Localization.getString( Localization.TXT_63_LABEL_FOUR );
		l_week0.text = Localization.getString( Localization.TXT_63_LABEL_UNLIMITED );
		l_weekend30.text = Localization.getString( Localization.TXT_63_LABEL_HALF );
		l_weekend1.text = Localization.getString( Localization.TXT_63_LABEL_ONE );
		l_weekend2.text = Localization.getString( Localization.TXT_63_LABEL_TWO );
		l_weekend4.text = Localization.getString( Localization.TXT_63_LABEL_FOUR );
		l_weekend0.text = Localization.getString( Localization.TXT_63_LABEL_UNLIMITED );
	}
	
	private UIToggleGroup m_weekGroup;
	private UIToggle m_weekThirtyMin;
	private UIToggle m_weekOneHour;
	private UIToggle m_weekTwoHours;
	private UIToggle m_weekFourHours;
	private UIToggle m_weekUnlimited;
	
	private UIToggleGroup m_weekendGroup;
	private UIToggle m_weekendThirtyMin;
	private UIToggle m_weekendOneHour;
	private UIToggle m_weekendTwoHours;
	private UIToggle m_weekendFourHours;
	private UIToggle m_weekendUnlimited;
	
	private Hashtable m_data;

	//Kevin
	//New Save Button
	private UIButton mSaveButton;
	
	private UIImage m_iconLock;

}
