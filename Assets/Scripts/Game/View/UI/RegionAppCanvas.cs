/*
 * Project: VZW
 * Author: Sean Chiu
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RegionAppCanvas: UICanvas
{

	public override void init( GameObject p_gameObject )
	{
		base.init( p_gameObject );

		m_emptyTexture = new Texture2D (1, 1);

		m_appSwipeList = getView("appScrollView") as UISwipeList;

		m_appSwipeList.addValueChangeListener(onScrollValueChanged);

		_setupList();

		setScrollView ();

		KidModeScrollViewUpdator.OnScrollViewValueChanged+= onScrollValueChanged;
	}


	private int currentDrawColumn = 0;

	private int visibleColumns = 8;

	private int totalColumnCount = 0; 

	private void onScrollValueChanged(Vector2 scrollValue)
	{

		Debug.Log("  scrollValue " + scrollValue.x.ToString());

		List<UIElement> listElements = m_appSwipeList.getListElements();

//		listElements.Count




		totalColumnCount = listElements.Count/2;

		if(listElements.Count % 2 == 1){

			totalColumnCount++;

		}



		currentDrawColumn = (int) Math.Round ( totalColumnCount * scrollValue.x );

		Debug.Log("  listElements " + listElements.Count + "       columnCount  " + totalColumnCount + "     currentDrawColumn " + currentDrawColumn);

	}

	private bool isInCurrentDrawGroup(int index){

		int checkInt = index + 1;

		int columnNum = checkInt/2;
		
		if(checkInt % 2 == 1){
			
			columnNum++;
			
		}



		//Start checking
		if(currentDrawColumn < 0 + visibleColumns / 2){

			if(columnNum < visibleColumns){

				return true;

			}

		}


		if(currentDrawColumn > totalColumnCount - visibleColumns / 2){
			
			if(columnNum > totalColumnCount - visibleColumns){
				
				return true;
				
			}
			
		}


		if( columnNum < currentDrawColumn + visibleColumns / 2 + 1 && columnNum > currentDrawColumn - visibleColumns / 2 ){

			return true;

		}

		return false;

	}

	
	public void setupLocalization()
	{
//		UILabel l_title = getView ("topicText") as UILabel;
//		l_title.text = Localization.getString (Localization.TXT_99_LABEL_TITLE);
//		UILabel l_cardTopic = getView ("cardTopicText") as UILabel;
//		l_cardTopic.text = Localization.getString (Localization.TXT_99_LABEL_CARD_TITLE);
//		UILabel l_enterChildNameLabel = getView ("enterChildNameLabel") as UILabel;
//		l_enterChildNameLabel.text = Localization.getString (Localization.TXT_99_LABEL_ENTER_NAME);
//		UILabel l_enterChildAgeLabel = getView ("enterChildAgeLabel") as UILabel;
//		l_enterChildAgeLabel.text = Localization.getString (Localization.TXT_99_LABEL_ENTER_AGE);
//		UILabel l_createProfileText = getView ("createProfileText") as UILabel;
//		l_createProfileText.text =  Localization.getString (Localization.TXT_99_BUTTON_CREATE_PROFILE);
//		UILabel l_backLabel = getView ("btnText") as UILabel;
//		l_backLabel.text = Localization.getString (Localization.TXT_BUTTON_BACK);
//		
//		UILabel l_name = getView ("childFirstNameTextPlaceHolder") as UILabel;
//		UILabel l_year = getView ("YearTextPlaceholder") as UILabel;
//		UILabel l_month = getView ("MonthTextPlaceholder") as UILabel;
//		
//		l_name.text = Localization.getString (Localization.TXT_99_LABEL_NAME);
//		l_year.text = Localization.getString (Localization.TXT_23_LABEL_YEAR);
//		l_month.text = Localization.getString (Localization.TXT_23_LABEL_MONTH);
	}



//	public override void update()
//	{
//		base.update();
//	}
//	
//	public override void dispose( bool p_deep )
//	{
//		base.dispose( p_deep );
//	}
//	
//	public override void enteringTransition()
//	{
//		base.enteringTransition();
//		tweener.addAlphaTrack( 1.0f, 0.0f, ZoodlesScreenFactory.FADE_SPEED, onFadeFinish );
//	}
	
	
	//-- Private Implementation --
	private void _setupList()
	{
		m_appSwipeList.setDrawFunction(onListDraw);
	}


	private void onListDraw( UIElement p_element, System.Object p_data, int p_index )
	{
		UIButton l_button = p_element as UIButton;
		DebugUtils.Assert( l_button != null );

		AppInfo l_info = p_data as AppInfo;
		DebugUtils.Assert( l_info != null );

		UIImage l_rawImage = p_element.getView("icon") as UIImage;
		UIImage l_appImage = p_element.getView("appIcon") as UIImage;
		UILabel l_appName = p_element.getView("appName") as UILabel;

//		l_appImage.gameObject.SetActive(true);
//
//		return;

		if(isInCurrentDrawGroup(p_index)){

			l_appName.gameObject.SetActive(true);
			l_rawImage.gameObject.SetActive(true);
			l_appImage.gameObject.SetActive(true);

//			p_element.gameObject.SetActive(true);

			if (l_appImage != null)
				l_appImage.active = false;
		
			if (l_rawImage == null)
	            return;


			if(appNameNeedTruncate(l_info.appName)){

				l_appName.text = trucatedAppName(l_info.appName);

			}else{

				l_appName.text = l_info.appName;

			}

			if (l_appName.active == false)
			{
				l_appName.active = true;
			}

	//		return;

			Vector2 l_textSize = l_appName.calcSize();
			RectTransform l_transform = l_appName.gameObject.GetComponent<RectTransform>();
			float l_scale = Mathf.Min(l_transform.sizeDelta.x / l_textSize.x, 1.0f);
			l_transform.localScale = new Vector3(l_scale, l_scale, 1);


			if( l_info.appIcon == null )
			{
				l_info.appIcon = ImageCache.getCacheImage(l_info.packageName + ".png");
				if(l_info.appIcon == null)
					l_appImage.setTexture( m_emptyTexture );
				else {
					l_appImage.setTexture(l_info.appIcon);
					l_appImage.active = true;
					l_rawImage.active = false;
	//				l_rawImage.active = true;
				}
			}
			else
			{
				l_appImage.setTexture(l_info.appIcon);
				l_appImage.active = true;
				l_rawImage.active = false;
	//			l_rawImage.active = true;
			}

		}else{

//			p_element.gameObject.SetActive(false);

//			l_appName.text = "NNNN";
			l_appName.gameObject.SetActive(false);
			l_rawImage.gameObject.SetActive(false);
			l_appImage.gameObject.SetActive(false);



		}



	}


	#region AppNameTruncate

	//Any app name over this length needs to be truncated
	private int needTruncateLength = 37;
	private bool appNameNeedTruncate(string name){

		if(name.Length > needTruncateLength){

			return true;

		}

		return false;


	}

	private string trucatedAppName(string name){

		string preName = name.Substring(0, needTruncateLength -1 - 6);

		string returnName = preName + " . . .";

		return returnName;

	}

	#endregion
//	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVariable )
//	{
//		UICanvas l_canvas = p_element as UICanvas;
//		l_canvas.isTransitioning = false;
//	}


	
	
	public override void update()
	{
		base.update();
		
		if (mMovingOffset == 0) {
			move = m_contentPanel.localPosition.x;
			moveTotal = move;
		}
		if (mMovingOffset > 0) {
			move += mMovingOffset * MOVESPEED;
			if (move <= moveTotal) {
				m_contentPanel.localPosition = new Vector3 (move, 0, 0);
			}
			else {
				if(!m_leftButton.active) {
					mMovingOffset = 0;
					moveTotal = m_contentPanel.localPosition.x;
				}
				else {
					moveTotal += (mMovingOffset * OFFSET);
					mMovingOffset = 0;
				}
				
			}
		} else if (mMovingOffset < 0) {
			
			move += mMovingOffset * MOVESPEED;
			if (move > moveTotal) {
				m_contentPanel.localPosition = new Vector3 (move, 0, 0);
			}
			else {
				if(!m_rightButton.active) {
					mMovingOffset = 0;
					moveTotal = m_contentPanel.localPosition.x;
				}
				else {
					moveTotal += (mMovingOffset * OFFSET);
					mMovingOffset = 0;
				}
				
			}
		}
		
	}
	
	private void SetupLocalizition()
	{		
		UILabel l_allVideosLabel 	= getView("allContentLabel") as UILabel;
		UILabel l_favorateLabel 	= getView("favorateLabel") as UILabel;
		UILabel l_infoLabel 		= getView("info") as UILabel;
		UILabel l_favorateInfoLabel = getView("favoriteInfo") as UILabel;
		UILabel l_headerLabel 		= getView("header") as UILabel;
		
		l_favorateLabel.text 		= Localization.getString(Localization.TXT_TAB_FAVORITES);
		l_allVideosLabel.text 		= Localization.getString(Localization.TXT_TAB_ALL_VIDEOS);
		//		l_infoLabel.text 			= Localization.getString(Localization.TXT_11_LABEL_INFO);
		//		l_favorateInfoLabel.text 	= Localization.getString(Localization.TXT_11_LABEL_FAVORITE);
		l_infoLabel.text 			= Localization.getString(Localization.TXT_LABEL_LOADING);
		l_favorateInfoLabel.text 	= Localization.getString(Localization.TXT_LABEL_LOADING);
		l_headerLabel.text 			= Localization.getString(Localization.TXT_11_LABEL_HEADER);
	}
	
	public void onLeftButtonClick( UIButton p_button )
	{
		mMovingOffset = 1;
		if (move == moveTotal) {
			moveTotal = move + mMovingOffset * OFFSET;
		}
	}
	
	public void onRightButtonClick( UIButton p_button )
	{
		mMovingOffset = -1;
		if (moveTotal == 0) {
			move = m_contentPanel.localPosition.x;
			moveTotal = move + mMovingOffset * OFFSET;
		} else if (move == moveTotal) {
			moveTotal = move + mMovingOffset * OFFSET;
		}
	}


	void setScrollView() {
		
		m_contentPanel = getView ("appScrollView").getChildAt(0).gameObject.GetComponent<RectTransform>();

		
		m_leftButton = getView( "contentArrowLeft" ) as UIButton;
		m_rightButton = getView( "contentArrowRight" ) as UIButton;

		
		m_leftButton.addClickCallback( onLeftButtonClick );
		m_rightButton.addClickCallback( onRightButtonClick );
	}


	private UISwipeList 	m_appSwipeList;

	private Texture2D m_emptyTexture;

	private RectTransform m_contentPanel;
	
	private UIButton m_leftButton;
	private UIButton m_rightButton;

	
	private float OFFSET = 500;
	private float MOVESPEED = 30;
	private float mMovingOffset = 0;
	private float move = 0;
	private float moveTotal;
	private bool isFavor = false; // true: favor false: all

}
