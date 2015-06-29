using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class BookReaderState : GameState {
	
	//consts
	private const float LOADING_WEIGHT	= 1.0f;
	private const float LOADING_START	= 80.0f;
	
	private const float PAGE_FADE_TIME = 0.4f;
	
	//--------------------Public Interface -----------------------
	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );

		m_book          = SessionHandler.getInstance().currentBook;
		m_bookReading 	= SessionHandler.getInstance ().currentBookReading;
		
		m_audioSource   = GameObject.FindObjectOfType<AudioSource>();
		
		_setupPageAudio();
		
		UIManager l_ui      = p_gameController.getUI();

		m_bookControlCanvas = l_ui.createScreen( UIScreen.DASHBOARD_CONTROLLER, false, 3 ) as DashBoardControllerCanvas;
		m_bookPageCanvas    = l_ui.createScreen( UIScreen.BOOK_PAGE,    true, 2  );
		m_bookReaderCanvas  = l_ui.createScreen( UIScreen.BOOK_READER,  true, 0  );
		
		m_isRunning = false;
		m_isPlayed = false;
		m_finishReading = false;
		m_length = 0;
		
		_setupElements();

		PointSystemController.Instance.setPointOK (PointSystemController.PointRewardState.No_Point);
		PointSystemController.Instance.startPointSystemTimer();

//		_loadFirstPage();
		_loadPage (0);
		
		SoundManager.getInstance().stopMusic();

		m_duration = 0;

		RequestQueue l_queue = new RequestQueue();
		l_queue.add(new VisitBookRequest());
		l_queue.request(RequestType.RUSH);

		GAUtil.logScreen("BookReaderScreen");
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );

		m_duration += p_time;
		
		_updatePageImage();
		
		if( m_confirmBack )
		{
			m_confirmBack = false;
			int l_nextState = p_gameController.getConnectedState(ZoodleState.BOOK_ACTIVITY);
			if (l_nextState != 0)
			{
//				if(l_nextState == ZoodleState.OVERVIEW_BOOK)
//				{
//					p_gameController.changeState( l_nextState );
//				}
//				else
//				{
//					p_gameController.connectState( ZoodleState.CONGRATS_STATE, l_nextState );
//					p_gameController.changeState( ZoodleState.CONGRATS_STATE );
//				}

				if (PointSystemController.Instance.pointSystemState () == PointSystemController.PointRewardState.OK) {
					
					p_gameController.connectState( ZoodleState.CONGRATS_STATE, l_nextState );
					p_gameController.changeState( ZoodleState.CONGRATS_STATE );
					
				} else {
					
					PointSystemController.Instance.stopPointSystemTimer();
					
					p_gameController.changeState( l_nextState );
					
				}


			}
		}

		if( m_isRunning )
		{
			m_length += 1 * Time.deltaTime;
			m_timeSlider.value = (m_length / m_totalLength);
			if( m_currentPageAudio != null )
			{
				if( m_length > m_currentPageAudio.length )
				{
					m_isRunning = false;
					m_statusLabel.text = Localization.getString(Localization.TXT_STATE_10_FINISH);
					m_timeSlider.value = 1;
				}
			}
		}
		else
		{
			if(!m_isPlayed 
//			   && null != m_bookReading
			   && null != m_book
			   && null != currentPage
			   && File.Exists(Application.persistentDataPath + "//" +  m_book.id + "//" + currentPage.id + ".wav") )
			{
				m_gameController.game.StartCoroutine( _playPageAudio( pageIndex ) );
			}
		}
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		
		m_audioSource.pitch = 1.0f;
		m_audioSource.Stop();
		
		UIManager l_ui = p_gameController.getUI();
		l_ui.removeScreen( m_bookReaderCanvas   );
		l_ui.removeScreen( m_bookPageCanvas     );
		l_ui.removeScreen( m_bookControlCanvas	);
		
		SessionHandler.getInstance ().recordKidList = null;

//		for( int i = 0; i < m_book.pageList.Count; i++ )
//		{
//			if(null != m_book
//			   	&& null != m_bookReading
//			   	&& null != m_book.pageList
//				&& File.Exists(Application.persistentDataPath + "//" +  m_book.id + "//" + m_bookReading.id + "//" + m_book.pageList[i].id + ".wav"))
//			{
//				File.Delete(Application.persistentDataPath + "//" +  m_book.id + "//" + m_bookReading.id + "//" + m_book.pageList[i].id + ".wav");
//			}
//		}

		GAUtil.logVisit("Book", m_duration);

		int readingtime = (int)Math.Ceiling(m_duration * 0.001);
		if (m_finishReading)
		{
			Dictionary<string,string> payload = new Dictionary<string,string>() { {"BookName", m_book.title},{"Duration", readingtime.ToString()}};
			SwrveComponent.Instance.SDK.NamedEvent("Book.FINISH",payload);
		}
		else
		{
			Dictionary<string,string> payload = new Dictionary<string,string>() { {"BookName", m_book.title},{"Duration", readingtime.ToString()}};
			SwrveComponent.Instance.SDK.NamedEvent("Book.NOTFINISH",payload);
		}

	}
	
	
	public int pageIndex
	{
		get { return m_pageIndex;   }
		set { m_pageIndex = value;  }
	}
	
	public BookPage currentPage
	{
		get
		{
			if (m_book == null) 
				return null;
			
			return m_book.pageList[pageIndex];
		}
	}

	public BookReadingPage currentBookReadingPage
	{
		get
		{
			if( currentPage == null )
				return null;

			if( m_bookReading == null )
				return null;

			return m_bookReading.readingPageTable[currentPage.id] as BookReadingPage;
		}
	}
	
	//------------------------ Private Implementation ---------------------------  
	
	#region Listeners
	
	private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		UICanvas p_canvas = p_element as UICanvas;
		p_canvas.isTransitioning = false;

	}

	private void onBackClicked( UIButton p_button )
	{
		m_confirmBack = true;
	}

	private void onNextPage(UIButton p_button)
	{
		Debug.Log ("    onNextPage  ");

		_nextPage();
	}
	
	private void onPreviousPage(UIButton p_button)
	{
		_previousPage();
	}

	#endregion
	
	private void _loadFirstPage()
	{
		Debug.Log ("    _loadFirstPage  ");

		m_canSetImage   = true;
		pageIndex       = 0;
		
		
		m_timeSlider.value = 0;
		m_statusLabel.text = Localization.getString(Localization.TXT_STATE_10_LOADING);
		
		_loadCurrentPageContent();
//		if(m_bookReading != null)
//		{
		_loadCurrentAudio ();
//		}
//		else
//		{
//			m_statusLabel.text = Localization.getString(Localization.TXT_STATE_10_NOTRECORDED);
//		}
		_setPageContent();
	}
	
	private void _loadPage( int p_index )
	{
		_stopPageAudio();

		pageIndex = p_index;
		m_timeSlider.value = 0;
		m_statusLabel.text = Localization.getString(Localization.TXT_STATE_10_LOADING);
		m_isPlayed = false;


		if( File.Exists(Application.persistentDataPath + "//" +  m_book.id + "//" + currentPage.id + ".wav") )
		{
			m_gameController.game.StartCoroutine( _playPageAudio( pageIndex ) );
		}
		else
		{
			if( m_bookReading != null )
			{
				_loadCurrentAudio ();
			}
			else
			{
				m_statusLabel.text = Localization.getString(Localization.TXT_STATE_10_NOTRECORDED);
			}
		}

		_loadCurrentPageContent();
		m_canSetImage = true;
		_setPageContent ();
	}

	private void setInputEnable( bool p_enable )
	{
		m_previousPageButton.enabled = p_enable;
		m_nextPageButton.enabled	 = p_enable;
		m_backButton.enabled		 = p_enable;
		m_libraryButton.enabled		 = p_enable;
	}

	private IEnumerator _playPageAudio( int p_index )
	{
		if( p_index < 0 || 
		   p_index >= m_book.pageList.Count || 
		   m_audioSource == null )
		{
			_Debug.logError("Index is out of bounds or Audio source is null!");
			yield break;
		}

//		if( null == currentBookReadingPage )
//		{
//			_Debug.logError( "No BookReadingPage for current page" );
//			yield break;
//		}
		
		if( File.Exists(Application.persistentDataPath + "//" +  m_book.id + "//" + currentPage.id + ".wav") )
		{
			setInputEnable(false);
			WWW l_localAudio = new WWW("file:///" + Application.persistentDataPath + "//" +  m_book.id + "//" + currentPage.id + ".wav");
			yield return l_localAudio;
			setInputEnable(true);
			if( pageIndex <= 0 )
			{
				m_previousPageButton.enabled = false;
			}
			if( pageIndex >= m_book.pageList.Count - 1 )
			{
				m_nextPageButton.enabled = false;
				m_finishReading = true;
			}
			
			if(l_localAudio.error != null)
			{
				yield break;
			}
			
			m_currentPageAudio = l_localAudio.GetAudioClip(false, false, AudioType.WAV);
			l_localAudio.Dispose();
			GC.Collect();
		}

		if (m_currentPageAudio == null)
		{
			m_statusLabel.text = Localization.getString(Localization.TXT_STATE_10_LOADING);
			_Debug.logError("Audio Clip for page: " + p_index + " is null!");
			yield break;
		}
		
		m_timeSlider.value = 0;
		m_statusLabel.text = Localization.getString(Localization.TXT_STATE_10_PLAYING);
		m_isRunning = true;
		m_length = 0;
		m_totalLength = m_currentPageAudio.length;
		
		m_audioSource.pitch = 1.0f;
		m_audioSource.PlayOneShot( m_currentPageAudio );
		m_isPlayed = true;
	}
	
	private void _stopPageAudio()
	{
		if (m_audioSource == null)
			return;
		
		m_audioSource.pitch = 0.0f;
		UnityEngine.Object.Destroy(m_currentPageAudio);
		m_currentPageAudio = null;
		m_isRunning = false;
		m_length = 0;
		m_totalLength = 0;
		m_timeSlider.value = 1;
		m_statusLabel.text = Localization.getString(Localization.TXT_STATE_10_FINISH);
	}
	
	private void _loadCurrentPageContent( )
	{
		m_pageImage.active = false;
		currentPage.requestImage();
	}

	private void _loadCurrentAudio( )
	{
		if(File.Exists(Application.persistentDataPath + "//" +  m_book.id + "//" + currentPage.id + ".wav"))
		{
			File.Delete(Application.persistentDataPath + "//" +  m_book.id + "//" + currentPage.id + ".wav");
		}
		
		string l_url    = currentBookReadingPage.audioUrl;
		_Debug.log("URL: " + l_url);
		RequestQueue l_queue = new RequestQueue();
		l_queue.add(new AudioRequest(l_url, m_book.id, currentPage.id));
		l_queue.request(RequestType.RUSH);
	}
	
	private void _setPageContent()
	{
		string l_string = currentPage.content;
		l_string = UnicodeDecoder.Unicode(l_string);
		l_string = UnicodeDecoder.UnicodeToChinese(l_string);
		l_string = UnicodeDecoder.CoverHtmlLabel(l_string);

		m_pageContentLabel.text = currentPage.content;
	}
	
	private void _updatePageImage()
	{
		if( !m_canSetImage || currentPage.pageImage == null )
			return;

		m_pageImage.setTexture(currentPage.pageImage);
		m_pageImage.active = true;

		m_canSetImage = false;
	}
	
	private void _setupElements()
	{
		m_libraryButton = m_bookPageCanvas.getView("libraryButton") as UIButton;
		m_backButton 	= m_bookPageCanvas.getView("backButton") 	as UIButton;
		m_recordToggle 	= m_bookPageCanvas.getView("recordToggle") 	as UIToggle;
		m_playToggle 	= m_bookPageCanvas.getView("playToggle") 	as UIToggle;
		m_stopToggle 	= m_bookPageCanvas.getView("stopToggle") 	as UIToggle;

		m_libraryButton.addClickCallback(onBackClicked);

		m_libraryButton.active 	= true;
		m_backButton.active 	= false;
		m_recordToggle.active 	= false;
		m_playToggle.active 	= false;
		m_stopToggle.active 	= false;
		
		m_statusLabel = m_bookReaderCanvas.getView("statusLabel") as UILabel;
		
		m_bookControlCanvas.setupDotList( m_book.pageList.Count );
		
		m_previousPageButton = m_bookControlCanvas.getView("leftButton") as UIButton;
		m_previousPageButton.addClickCallback(onPreviousPage);
		
		m_nextPageButton = m_bookControlCanvas.getView("rightButton") as UIButton;
		m_nextPageButton.addClickCallback(onNextPage);
		
		m_pageContentLabel = m_bookPageCanvas.getView("pageContent") as UILabel;
		
		m_pageImage = m_bookPageCanvas.getView("coverImg") as UIImage;
		
		m_timeSlider = m_bookReaderCanvas.getView( "timeSlider" ) as UISlider;
		m_timeSlider.value = 0;
	}
	
	
	private void _nextPage()
	{
		int l_nextPageIndex = pageIndex + 1;
		_loadPage( l_nextPageIndex );
	}
	
	private void _previousPage()
	{
		int l_previousPageIndex = pageIndex - 1;
		_loadPage( l_previousPageIndex );
	}
	
	private void _setupPageAudio()
	{
		if( m_book == null ) 
			return;
		
		m_audioSource.pitch = 1.0f;
	}

	private Book        m_book;
	private BookReading m_bookReading;
	
	private UIImage  	m_pageImage;
	
	private UILabel     m_pageContentLabel;
	private UIButton    m_previousPageButton;
	private UIButton    m_nextPageButton;

	private UIButton 	m_libraryButton;
	private UIButton    m_backButton;
	private UIToggle    m_recordToggle;
	private UIToggle    m_stopToggle;
	private UIToggle    m_playToggle;
	
	private UILabel     m_statusLabel;
	private UISlider	m_timeSlider;
	private bool 		m_isRunning;
	private bool 		m_isPlayed;
	private float 		m_length;
	private float 		m_totalLength;
	
	private UICanvas    m_bookReaderCanvas;
	private UICanvas    m_bookPageCanvas;
	private DashBoardControllerCanvas 	m_bookControlCanvas;
	
	private int         m_pageIndex     = 0;
	private bool        m_confirmBack 	= false;
	private bool        m_canSetImage   = false;
	private string      m_micDevice;

	private AudioSource m_audioSource;
	private AudioClip m_currentPageAudio = null;

	private int			m_duration = 0;

	private bool 		m_finishReading;
}
