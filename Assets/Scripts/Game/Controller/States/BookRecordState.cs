using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;

public class BookRecordState : GameState
{

	//consts
	private const float LOADING_WEIGHT	= 1.0f;
	private const float LOADING_START	= 80.0f;

    private const float PAGE_FADE_TIME = 0.4f;

	//--------------------Public Interface -----------------------
	
	public override void enter( GameController p_gameController )
	{
		base.enter( p_gameController );

        m_book          = SessionHandler.getInstance().currentBook;

        m_audioSource   = GameObject.FindObjectOfType<AudioSource>();

        _setupPageAudio();

        UIManager l_ui      = p_gameController.getUI();

		m_recordAgainCanvas = l_ui.createScreen( UIScreen.RECORD_AGAIN, false, 6 );
		m_recordStartCanvas = l_ui.createScreen( UIScreen.RECORD_START, false, 5 ) as RecordStartCanvas;
		m_recordFinishCanvas = l_ui.createScreen( UIScreen.RECORD_FINISH, false, 4 );
		m_bookControlCanvas = l_ui.createScreen( UIScreen.DASHBOARD_CONTROLLER, false, 3 ) as DashBoardControllerCanvas;
		m_bookPageCanvas    = l_ui.createScreen( UIScreen.BOOK_PAGE,    true, 2  );
		m_bookReaderCanvas  = l_ui.createScreen( UIScreen.BOOK_READER,  true, 0  );

		m_isRunning = false;
		m_length = 0;
		m_runningTime = 0;
		m_recordedCount = 0;
		m_uploadedCount = 0;
		m_request = new RequestQueue ();

        _setupElements();

        _loadFirstPage();

		SoundManager.getInstance().stopMusic();

		GAUtil.logScreen("BookRecordScreen");
	}
	
	public override void update( GameController p_gameController, int p_time )
	{
		base.update( p_gameController, p_time );

        _updatePageImage();

		if( true == m_isRunning )
		{
			m_length += 1 * Time.deltaTime;
			m_timeSlider.value = (m_length / 360);
			m_runningTime = Mathf.CeilToInt(m_length);
			m_statusLabel.text = new System.TimeSpan(0, 0, m_runningTime).ToString();
			if( !m_isRecording && m_currentClip != null )
			{
				if( m_runningTime > m_currentClip.length )
				{
					m_isRunning = false;
					m_statusLabel.text = new System.TimeSpan(0, 0, 0).ToString();
					m_timeSlider.value = 0;
					m_stopToggle.isOn = true;
				}
			}
			if( m_length >= 359 )
			{
				m_stopToggle.isOn = true;
			}
		}

		if( m_confirmBack )
		{
			m_confirmBack = false;
			int l_nextState = p_gameController.getConnectedState(ZoodleState.BOOK_RECORD);
			if (l_nextState != 0)
			{
				p_gameController.changeState(l_nextState);
			}
		}

		if( m_setMessage )
		{
			m_messageTitleLabel.text = "Upload Result";
			m_messageContentLabel.text = "Upload progress : " + m_uploadedCount + "/" + m_recordedCount;
			
			m_setMessage = false;
		}

		if( m_recordedCount > 0 && m_uploadedCount > 0 && m_recordedCount == m_uploadedCount )
		{
			m_exitMessageButton.active = true;
			
			m_messageTitleLabel.text = "Upload Result";
			m_messageContentLabel.text = "Upload successed.";
		}
	}
	
	public override void exit( GameController p_gameController )
	{
		base.exit( p_gameController );
		
		_onRecordingFinish();
        _stopRecording();

        m_audioSource.pitch = 1.0f;
        m_audioSource.Stop();

		for( int i = 0; i < m_book.pageList.Count; i++ )
		{
			if(File.Exists(Application.persistentDataPath + "//" + m_book.id + "//" + m_book.pageList[i].id + ".wav"))
			{
				File.Delete(Application.persistentDataPath + "//" + m_book.id + "//" + m_book.pageList[i].id + ".wav");
			}
		}

        UIManager l_ui = p_gameController.getUI();
        l_ui.removeScreen( m_bookReaderCanvas   );
        l_ui.removeScreen( m_bookPageCanvas     );
		l_ui.removeScreen( m_bookControlCanvas	);
		l_ui.removeScreen( m_recordStartCanvas	);
		l_ui.removeScreen( m_recordFinishCanvas	);
		l_ui.removeScreen( m_recordAgainCanvas 	);

		SessionHandler.getInstance ().recordKidList = null;
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

//	public AudioClip currentClip
//	{
//		get
//		{
//			if (m_pageAudio == null) 
//				return null;
//			
//			return m_pageAudio[pageIndex];
//		}
//	}
   
//------------------------ Private Implementation ---------------------------

#region Listeners

	private void onSendButtonClicked( UIButton p_button )
	{
		p_button.removeClickCallback( onSendButtonClicked );
		
		m_gameController.getUI ().changeScreen (UIScreen.RECORD_FINISH, false);
		m_gameController.getUI ().changeScreen (UIScreen.RECORD_AGAIN, true);

		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_currentPanel = m_recordAgainCanvas.getView ("messagePanel");
		l_pointListIn.Add( l_currentPanel.transform.localPosition );
		l_pointListIn.Add( l_currentPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListIn, 0f );
		m_messageTitleLabel.text = "Uploading Data";
		m_messageContentLabel.text = "Please wait.";
		m_exitMessageButton.active = false;
		
		m_request.reset ();
		m_request.add( new CreateReadingsRequest( onCreateReadingFinish ) );
		m_request.request (RequestType.RUSH);
	}

	private void onCreateReadingFinish( WWW p_response )
	{
		if (p_response.error != null)
			m_gameController.changeState(ZoodleState.SERVER_ERROR);
		else
		{
			m_uploadedCount = 0;
			Hashtable l_jsonResponse = MiniJSON.MiniJSON.jsonDecode(p_response.text) as Hashtable;
			if(l_jsonResponse.ContainsKey("jsonResponse"))
			{
				Hashtable l_response = l_jsonResponse["jsonResponse"] as Hashtable;
				if(l_response.ContainsKey("response"))
				{
					Hashtable l_bookData = l_response["response"] as Hashtable;

					if( l_bookData == null )
					{
						m_confirmBack = true;
						return;
					}

					ArrayList l_pages = l_bookData["pages"] as ArrayList;

					Hashtable l_bookReadingPageTable = new Hashtable();

					foreach( Hashtable l_pageData in l_pages )
					{
						BookReadingPage l_page = new BookReadingPage( l_pageData );
						l_bookReadingPageTable.Add(l_page.pageId, l_page);
					}

					try
					{
						for( int i = 0; i < m_book.pageList.Count; i++ )
						{
							if(!File.Exists(Application.persistentDataPath + "//" + m_book.id + "//" + m_book.pageList[i].id + ".wav"))
							{
								return;
							}

							string l_slug = (l_bookReadingPageTable[m_book.pageList[i].id] as BookReadingPage).audioSlug;

							string l_url = "/audios/story/" + l_slug;

							UploadHelper l_upload = new UploadHelper(l_url);
							l_upload.AddTextParameter("token", SessionHandler.getInstance().token.getSecret());
	//						l_upload.AddTextParameter("time_seconds", Mathf.CeilToInt(l_clip.length).ToString());
							l_upload.AddTextParameter("time_seconds", "0");
							l_upload.AddFileParameter("file",new FileInfo(Application.persistentDataPath + "//" + m_book.id + "//" + m_book.pageList[i].id + ".wav"));

							l_upload.SendAsync(_onUploadComplete);
						}
					}
					catch(Exception)
					{
						m_exitMessageButton.active = true;
						
						m_messageTitleLabel.text = "Upload Result";
						m_messageContentLabel.text = "Uploading failed.Please try again.";
					}
				}
			}
		}
	}

	private void _onUploadComplete(object p_sender, UploadDataCompletedEventArgs p_event)
	{
//		_Debug.log(Encoding.UTF8.GetString(p_event.Result));

		m_uploadedCount++;
		m_setMessage = true;
	}

	private void onAddButtonClicked( UIButton p_button )
	{
		m_gameController.getUI ().changeScreen (UIScreen.RECORD_FINISH,false);
		m_gameController.getUI ().changeScreen (UIScreen.RECORD_START,true);
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_currentPanel = m_recordStartCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_currentPanel.transform.localPosition );
		l_pointListIn.Add( l_currentPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListIn, 0f );
	}

	private void onExitRerecordButtonClicked( UIButton p_button )
	{
		m_gameController.getUI ().changeScreen (UIScreen.RECORD_AGAIN,false);
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_currentPanel = m_recordAgainCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_currentPanel.transform.localPosition );
		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	private void onExitMessageButtonClicked( UIButton p_button )
	{
		m_gameController.getUI ().changeScreen (UIScreen.RECORD_AGAIN,false);
		m_gameController.getUI ().changeScreen (UIScreen.RECORD_FINISH,true);
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_currentPanel = m_recordAgainCanvas.getView ("messagePanel");
		l_pointListOut.Add( l_currentPanel.transform.localPosition );
		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	private void onExitSelectedButtonClicked( UIButton p_button )
	{
		m_gameController.getUI ().changeScreen (UIScreen.RECORD_FINISH,false);
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_currentPanel = m_recordFinishCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_currentPanel.transform.localPosition );
		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	private void onExitUnselectedButtonClicked( UIButton p_button )
	{
		m_gameController.getUI ().changeScreen (UIScreen.RECORD_START,false);
		m_gameController.getUI ().changeScreen (UIScreen.RECORD_FINISH,true);
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_currentPanel = m_recordStartCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_currentPanel.transform.localPosition );
		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );
	}

	private void onYesButtonClicked( UIButton p_button )
	{
		m_gameController.getUI ().changeScreen (UIScreen.RECORD_AGAIN,false);
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_currentPanel = m_recordAgainCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_currentPanel.transform.localPosition );
		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );

		if(File.Exists(Application.persistentDataPath + "//" + m_book.id + "//" + currentPage.id + ".wav"))
		{
			File.Delete(Application.persistentDataPath + "//" + m_book.id + "//" + currentPage.id + ".wav");
			m_recordedCount--;
		}

		_startRecording();
	}

	private void onNoButtonClicked( UIButton p_button )
	{
		m_gameController.getUI ().changeScreen (UIScreen.RECORD_AGAIN,false);
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_currentPanel = m_recordAgainCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_currentPanel.transform.localPosition );
		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );

		if( m_recordedCount >= m_book.pageList.Count )
			_allRecorded ();
	}

	private void onSelectKid( UISwipeList p_list, UIButton p_button, object p_data, int p_index )
	{
		Kid l_kid = p_data as Kid;
		bool l_isBreak = false;
		
		if( 0 < SessionHandler.getInstance().recordKidList.Count )
		{
			for( int i = 0; i < SessionHandler.getInstance().recordKidList.Count; i++ )
			{
				if( SessionHandler.getInstance().recordKidList[i].id == l_kid.id )
				{
					SessionHandler.getInstance().recordKidList.RemoveAt(i);
					l_isBreak = true;
					break;
				}
			}
			if( !l_isBreak )
			{
				SessionHandler.getInstance().recordKidList.Add( l_kid );
			}
		}
		else
		{
			SessionHandler.getInstance().recordKidList.Add( l_kid );
		}
	}

	private void onSaveButtonClicked( UIButton p_button )
	{
		m_gameController.getUI ().changeScreen (UIScreen.RECORD_START,false);
		m_gameController.getUI ().changeScreen (UIScreen.RECORD_FINISH,true);
		List<Vector3> l_pointListOut = new List<Vector3>();
		UIElement l_currentPanel = m_recordStartCanvas.getView ("mainPanel");
		l_pointListOut.Add( l_currentPanel.transform.localPosition );
		l_pointListOut.Add( l_currentPanel.transform.localPosition - new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListOut, 0f );

		List<object> l_data = new List<object> ();
		foreach( Kid l_kid in SessionHandler.getInstance().recordKidList)
		{
			l_data.Add(l_kid);
		}

		m_selectedSwipeList.setData (l_data);
		m_recordStartCanvas.reload ();
	}

	private void onSelectButtonClicked( UIButton p_button )
	{
		SessionHandler.getInstance ().recordKidList.Clear ();
		SessionHandler.getInstance ().recordKidList.AddRange(SessionHandler.getInstance ().kidList);
	}

    private void onFadeFinish( UIElement p_element, Tweener.TargetVar p_targetVar )
	{
		UICanvas p_canvas = p_element as UICanvas;
		p_canvas.isTransitioning = false;
	}
	

	private void onBackClicked( UIButton p_button )
	{
		m_confirmBack = true;
	}

    private void onRecordClicked( UIToggle p_toggle, bool p_isOn )
	{
		if(p_isOn)
		{
			_Debug.log( "on record" );
			if( m_isRecording )
			{
				_onRecordingFinish();
				_stopRecording();
				m_isRunning = false;
				if( null != m_currentClip )
				{
					_askRecord();
					return;
				}

				if( m_recordedCount >= m_book.pageList.Count )
					_allRecorded ();
			}
			else
			{
				if( null != m_currentClip )
				{
					_askRecord();
					return;
				}
				_startRecording();
			}
		}
    }

    private void onStopClicked(UIToggle p_toggle, bool p_isOn)
	{
		if(p_isOn)
		{
			_onRecordingFinish();
			_stopRecording();
			_stopPageAudio();

			if( m_recordedCount >= m_book.pageList.Count )
				_allRecorded ();
		}
    }

    private void onPlayClicked(UIToggle p_toggle, bool p_isOn )
    {
		if(p_isOn)
		{			
			if( Microphone.IsRecording( m_micDevice ) )
			{
				_onRecordingFinish();
				_stopRecording();
			}

			_playPageAudio(pageIndex);
		}
    }

    private void onNextPage(UIButton p_button)
    {
        _nextPage();
    }

    private void onPreviousPage(UIButton p_button)
	{
        _previousPage();
    }
#endregion

	private void setInputEnable( bool p_enable )
	{
		m_previousPageButton.enabled = p_enable;
		m_nextPageButton.enabled	 = p_enable;
		m_backButton.enabled		 = p_enable;
		m_recordToggle.enabled		 = p_enable;
		m_playToggle.enabled		 = p_enable;
		m_stopToggle.enabled		 = p_enable;
	}

    private void _loadFirstPage()
    {
        m_canSetImage   = true;
        pageIndex       = 0;

		
		m_timeSlider.value = 0;
		m_statusLabel.text = new System.TimeSpan(0, 0, 0).ToString();

        _loadCurrentPageContent();
        _setPageContent();

		for( int i = 0; i < m_book.pageList.Count; i++ )
		{
			if(File.Exists(Application.persistentDataPath + "//" + m_book.id + "//" + m_book.pageList[i].id + ".wav"))
			{
				File.Delete(Application.persistentDataPath + "//" + m_book.id + "//" + m_book.pageList[i].id + ".wav");
			}
		}
    }

    private IEnumerator _loadPage( int p_index )
    {
		m_stopToggle.isOn = true;

        pageIndex = p_index;

		UnityEngine.Object.Destroy (m_currentClip);
		m_currentClip = null;

		if(File.Exists(Application.persistentDataPath + "//" + m_book.id + "//" + currentPage.id + ".wav"))
		{
			setInputEnable(false);
			WWW l_localAudio = new WWW("file:///" + Application.persistentDataPath + "/" + m_book.id + "/" + currentPage.id + ".wav");
			yield return l_localAudio;
			setInputEnable(true);
			if( p_index <= 0 )
			{
				m_previousPageButton.enabled = false;
			}
			if( p_index >= m_book.pageList.Count - 1 )
			{
				m_nextPageButton.enabled = false;
			}
			
			if(l_localAudio.error != null)
			{
				yield break;
			}
			
			m_currentClip = l_localAudio.GetAudioClip(false, false, AudioType.WAV);
			l_localAudio.Dispose();
			GC.Collect();
		}

		if( null == m_currentClip )
		{
			m_timeSlider.value = 0;
			m_statusLabel.text = new System.TimeSpan(0, 0, 0).ToString();
		}
		else
		{
			m_timeSlider.value = (m_currentClip.length / 360);
			int l_time = Mathf.CeilToInt(m_currentClip.length);
			m_statusLabel.text = new System.TimeSpan(0, 0, l_time).ToString();
		}

        _loadCurrentPageContent();
		m_canSetImage = true;
		_setPageContent();

		if( m_recordedCount >= m_book.pageList.Count )
			_allRecorded ();
	}

	private void _askRecord()
	{		
		m_gameController.getUI ().changeScreen (UIScreen.RECORD_AGAIN, true);
		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_currentPanel = m_recordAgainCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_currentPanel.transform.localPosition );
		l_pointListIn.Add( l_currentPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListIn, 0f );
	}

	private void _allRecorded()
	{
		m_gameController.getUI ().changeScreen (UIScreen.RECORD_FINISH, true);

		m_messageLabel.text = "Your recording of " + m_book.title + " is being processed and will soon be available to these recipients:";

		List<Vector3> l_pointListIn = new List<Vector3>();
		UIElement l_currentPanel = m_recordFinishCanvas.getView ("mainPanel");
		l_pointListIn.Add( l_currentPanel.transform.localPosition );
		l_pointListIn.Add( l_currentPanel.transform.localPosition + new Vector3( 0, 800, 0 ));
		l_currentPanel.tweener.addPositionTrack( l_pointListIn, 0f );
	}

	private void _onRecordingFinish()
	{
//		if( m_micDevice == null  )
//			return;
		if( !m_isRecording )
			return;

		Microphone.End (m_micDevice);

		AudioClip l_clip = new AudioClip();
		l_clip = AudioClip.Create( "clip", FREQUENCY * m_runningTime,
		                               m_newClip.channels, m_newClip.frequency, false, false);
		float[] l_newData = new float[ FREQUENCY * m_runningTime ];
		m_newClip.GetData ( l_newData, 0 );
		l_clip.SetData ( l_newData, 0 );

		UnityEngine.Object.Destroy(m_newClip);
		m_newClip = null;

		if(File.Exists(Application.persistentDataPath + "//" + m_book.id + "//" + currentPage.id + ".wav"))
		{
			File.Delete(Application.persistentDataPath + "//" + m_book.id + "//" + currentPage.id + ".wav");
		}

		//TODO handle exception when no enough space in disk.
		AudioSave.Save( m_book.id + "//" + currentPage.id, l_clip );
		m_recordedCount++;

		GAUtil.logRecord("Audio", (int)(l_clip.length * 1000));

		m_currentClip = l_clip;
	}


    private void _startRecording()
    {
//        if( m_micDevice == null )
//            return;

		m_timeSlider.value = 0;
		m_statusLabel.text = new System.TimeSpan(0, 0, 0).ToString();
		m_isRunning = true;
		m_isRecording = true;
		m_length = 0;

        m_newClip = Microphone.Start(m_micDevice, false, 360, FREQUENCY);
    }

    private void _stopRecording()
	{
		m_isRunning = false;
		m_isRecording = false;
		m_length = 0;
		m_runningTime = 0;
    }

	private void _playPageAudio( int p_index )
    {
        if( p_index < 0 || 
		   p_index >=  m_book.pageList.Count || 
            m_audioSource == null )
        {
            _Debug.logError("Index is out of bounds or Audio source is null!");
            return;
        }

		if (m_currentClip == null)
		{
			_Debug.logError("Audio Clip for page: " + p_index + " is null!");
			return;
		}

		m_timeSlider.value = 0;
		m_statusLabel.text = new System.TimeSpan(0, 0, 0).ToString();
		m_isRunning = true;
		m_length = 0;
		
		m_audioSource.pitch = 1.0f;
		m_audioSource.PlayOneShot (m_currentClip);
    }

    private void _stopPageAudio()
    {
        if (m_audioSource == null)
            return;

        m_audioSource.pitch = 0.0f;

		m_isRunning = false;
		m_length = 0;
		m_runningTime = 0;

		if( null == m_currentClip )
		{
			m_timeSlider.value = 0;
			m_statusLabel.text = new System.TimeSpan(0, 0, 0).ToString();
		}
		else
		{
			m_timeSlider.value = (m_currentClip.length / 360);
			int l_time = Mathf.CeilToInt(m_currentClip.length);
			m_statusLabel.text = new System.TimeSpan(0, 0, l_time).ToString();
		}
    }

    private void _loadCurrentPageContent( )
	{
		m_pageImage.active = false;
		currentPage.requestImage();
    }

    private void _setPageContent()
    {
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
		m_backButton = m_bookPageCanvas.getView("backButton") as UIButton;
        m_backButton.addClickCallback(onBackClicked);

		m_recordToggle = m_bookPageCanvas.getView("recordToggle") as UIToggle;
        m_recordToggle.addValueChangedCallback( onRecordClicked );

		m_playToggle = m_bookPageCanvas.getView("playToggle") as UIToggle;
        m_playToggle.addValueChangedCallback(onPlayClicked);

		m_stopToggle = m_bookPageCanvas.getView("stopToggle") as UIToggle;
        m_stopToggle.addValueChangedCallback( onStopClicked );

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

		m_selectedSwipeList 	= m_recordFinishCanvas.getView( "kidSwipeList" ) 	as UISwipeList;
		m_addKidButton 			= m_recordFinishCanvas.getView( "addButton" ) 		as UIButton;
		m_sendButton 			= m_recordFinishCanvas.getView( "sendButton" ) 		as UIButton;
		m_exitSelectedButton 	= m_recordFinishCanvas.getView( "exitButton" ) 		as UIButton;
		m_messageLabel 			= m_recordFinishCanvas.getView( "messageText" ) 	as UILabel;
		m_exitSelectedButton.addClickCallback ( onExitSelectedButtonClicked );
		m_addKidButton.addClickCallback ( onAddButtonClicked );
		m_sendButton.addClickCallback ( onSendButtonClicked );

		m_unselectedSwipeList 	= m_recordStartCanvas.getView( "kidSwipeList" ) as UISwipeList;
		m_selectAllButton 		= m_recordStartCanvas.getView( "selectButton" ) as UIButton;
		m_saveButton 			= m_recordStartCanvas.getView( "saveButton" ) 	as UIButton;
		m_exitUnselectedButton 	= m_recordStartCanvas.getView( "exitButton" ) 	as UIButton;
		m_unselectedSwipeList.addClickListener ( "Prototype", onSelectKid );
		m_exitUnselectedButton.addClickCallback ( onExitUnselectedButtonClicked );
		m_selectAllButton.addClickCallback ( onSelectButtonClicked );
		m_saveButton.addClickCallback ( onSaveButtonClicked );

		m_yesButton 			= m_recordAgainCanvas.getView( "yesButton" ) 	as UIButton;
		m_noButton 				= m_recordAgainCanvas.getView( "noButton" ) 	as UIButton;
		m_exitRerecordButton 	= m_recordAgainCanvas.getView( "mainPanel" ).getView( "exitButton" )	 as UIButton;
		m_exitMessageButton 	= m_recordAgainCanvas.getView( "messagePanel" ).getView( "exitButton" )	 as UIButton;
		m_messageTitleLabel 	= m_recordAgainCanvas.getView( "messagePanel" ).getView( "titleText" )	 as UILabel;
		m_messageContentLabel 	= m_recordAgainCanvas.getView( "messagePanel" ).getView( "contentText" ) as UILabel;
		m_yesButton.addClickCallback( onYesButtonClicked );
		m_noButton.addClickCallback( onNoButtonClicked );
		m_exitRerecordButton.addClickCallback( onExitRerecordButtonClicked );
		m_exitMessageButton.addClickCallback( onExitMessageButtonClicked );
		m_exitMessageButton.active = false;
    }


    private void _nextPage()
    {
        int l_nextPageIndex = pageIndex + 1;
        m_gameController.game.StartCoroutine( _loadPage( l_nextPageIndex ) );
    }

    private void _previousPage()
    {
		int l_previousPageIndex = pageIndex - 1;
		m_gameController.game.StartCoroutine( _loadPage( l_previousPageIndex ) );
    }

    private void _setupPageAudio()
    {
        if( m_book == null ) 
            return;

        string[] l_deviceList = Microphone.devices;
        if( l_deviceList.Length > 0 )
            m_micDevice = l_deviceList[0];

		m_audioSource.pitch = 1.0f;
    }
	
	private const int FREQUENCY = 44100;

    private Book        m_book;

    private UIImage  	m_pageImage;

    private UILabel     m_pageContentLabel;
    private UIButton    m_previousPageButton;
    private UIButton    m_nextPageButton;

    private UIButton    m_backButton;
    private UIToggle    m_recordToggle;
    private UIToggle    m_stopToggle;
    private UIToggle    m_playToggle;

    private UILabel     m_statusLabel;
	private UISlider	m_timeSlider;
	private bool 		m_isRunning;
	private float 		m_length;

	private UICanvas    m_bookReaderCanvas;
	private UICanvas    m_bookPageCanvas;
	private DashBoardControllerCanvas 	m_bookControlCanvas;
	private UICanvas 	m_recordFinishCanvas;
	private RecordStartCanvas 			m_recordStartCanvas;
	private UICanvas 	m_recordAgainCanvas;

	private UISwipeList m_selectedSwipeList;
	private UIButton 	m_addKidButton;
	private UIButton 	m_sendButton;
	private UIButton	m_exitSelectedButton;
	private UILabel 	m_messageLabel;

	private UISwipeList m_unselectedSwipeList;
	private UIButton 	m_selectAllButton;
	private UIButton 	m_saveButton;
	private UIButton	m_exitUnselectedButton;

	private UIButton 	m_yesButton;
	private UIButton 	m_noButton;
	private UIButton 	m_exitRerecordButton;
	private UIButton 	m_exitMessageButton;
	private UILabel 	m_messageTitleLabel;
	private UILabel 	m_messageContentLabel;

    private int         m_pageIndex     = 0;
	private bool        m_confirmBack 	= false;
    private bool        m_canSetImage   = false;
    private string      m_micDevice;

	private int 		m_runningTime 	= 0;
	private AudioClip 	m_currentClip;
	private AudioClip 	m_newClip;
	private bool 		m_isRecording 	= false;
	private int 		m_recordedCount	= 0;
	private int 		m_uploadedCount = 0;
    private AudioSource m_audioSource;

	private RequestQueue m_request;

	private bool 		m_setMessage = false;
}
