using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class WebContentCache : object
{
	public bool isLoading;
	public bool isFinishedLoading;
	public bool isFinishedLoadingWebContent;
	public bool isFinishedLoadingBooks;
	public bool loadWebContentFail;
	public bool loadBookFail;

	//honda
	public delegate void onLoadingCompletedEvent();
	public event onLoadingCompletedEvent onLoadingCompleted;

	private bool isDoneWithWebcontentLoading = false;
	private bool isDoneWithBookLoading = false;
	//end
	
	public void startRequests()
	{
		m_sessionHandler = SessionHandler.getInstance();

		clear();

		doWebContentRequest();

		//honda: fetch data from local
//		addBookList();
		//honda: request from the server
		doBookListRequest();
		//end
	}

	//honda
	public void startRequests(onLoadingCompletedEvent completedEvent)
	{
		if (completedEvent != null)
		{
			onLoadingCompleted += completedEvent;
		}
		startRequests();
	}

	public void doWebContentRequest()
	{
		m_webContentRequest = new RequestQueue();
		m_webContentRequest.add(new WebContentRequest(_requestWebContentComplete));
		m_webContentRequest.request(RequestType.SEQUENCE);
	}

	public void doBookListRequest()
	{
		m_bookListRequest = new RequestQueue();
		m_bookListRequest.add(new BookListRequest(true, _requestBookListComplete));
		m_bookListRequest.request();
	}
	//end

	public void clear()
	{
		isLoading = false;
		isFinishedLoading = false;
		isFinishedLoadingWebContent = false;
		isFinishedLoadingBooks = false;
		loadWebContentFail = false;
		loadBookFail = false;

		m_sessionHandler.resetWebBookListsCache();

		if (null != m_webContentRequest)
		{
			m_webContentRequest.dispose();
			m_webContentRequest = null;
		}

		//honda: comment out request book list due to book list is put in the local place
		if (null != m_bookListRequest)
		{
			m_bookListRequest.dispose();
			m_bookListRequest = null;
		}

		if (null != m_bookContentList)
		{
			for (int i = 0; i < m_bookContentList.Count; ++i)
			{
				BookInfo l_info = m_bookContentList[i] as BookInfo;
				l_info.dispose();
			}
		}
	}

	private void _requestWebContentComplete(HttpsWWW p_response)
	{

//		StreamWriter writer = new StreamWriter(Application.dataPath + "/Resources/_Debug/DownloadedWebContentRequest.txt"); // Does this work?
//		writer.WriteLine(p_response.text);
//		writer.Close();
//
//		Debug.LogError("   &&*& & & & & &* *& * &* *& & *& *& &*& *& N   _requestWebContentComplete " );
//
//		Debug.LogError(p_response.text);



		if (p_response.error == null)
		{
			List<object> l_contentList = new List<object> ();
			ArrayList l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as ArrayList;
			Debug.Log("WebContent: " + p_response.text);
			foreach (object o in l_data)
			{

				WebContent newWebContent = new WebContent(o as Hashtable);

				if(newWebContent.category == WebContent.VIDEO_TYPE){
					Debug.LogError("  0 0 00 0 0 0 0 0 0 0 0 0 0 it is Web content ");
				}else if(newWebContent.category == WebContent.GAME_TYPE){
					Debug.LogError("  0 0 00 0 0 0 0 0 0 0 0 0 0 it is game ");
				}

				l_contentList.Add(newWebContent);
			}
			m_sessionHandler.webContentList = l_contentList;
			isFinishedLoadingWebContent = true;
		}
		else
		{
			loadWebContentFail = true;
			Debug.Log("spinner: webcontent error = " + p_response.error);
		}

		isDoneWithWebcontentLoading = true;
		processFinishedRequests();

		ToyboxRandomizeController.Instance.restartAllRecordList();

	}

	private void _requestBookListComplete(HttpsWWW p_response)
	{
		if (p_response.error == null)
		{
			string l_string = p_response.text;
			l_string = UnicodeDecoder.Unicode (l_string);
			l_string = UnicodeDecoder.UnicodeToChinese (l_string);
			l_string = UnicodeDecoder.CoverHtmlLabel (l_string);
			Hashtable l_jsonResponse = MiniJSON.MiniJSON.jsonDecode (l_string) as Hashtable;

			if (l_jsonResponse.ContainsKey ("jsonResponse"))
			{
				Hashtable l_response = l_jsonResponse ["jsonResponse"] as Hashtable;
				if (l_response.ContainsKey ("response"))
				{
					Hashtable l_bookData = l_response ["response"] as Hashtable;
					ArrayList l_books = l_bookData ["books"] as ArrayList;
					foreach (object o in l_books)
					{
						Book l_book = new Book (o as Hashtable);
						m_sessionHandler.addBook(l_book.id, l_book);
					}
					
					ArrayList l_readings = l_bookData ["readings"] as ArrayList;
					foreach (object o in l_readings)
					{
						BookReading l_bookReading = new BookReading (o as Hashtable);
						
						if (0 == l_bookReading.readingPageTable.Count)
						{
							continue;
						}

						m_sessionHandler.readingTable[l_bookReading.id] = l_bookReading;
					}
					
					List<object> l_contentList = new List<object>();
					List<int> l_recordedBookList = new List<int>();
					
					foreach (BookReading l_bookReading in m_sessionHandler.readingTable.Values)
					{
						string l_bookName = (m_sessionHandler.bookTable[l_bookReading.bookId] as Book).title;
						BookInfo l_bookInfo = new BookInfo(l_bookName, BookState.Recorded, l_bookReading.coverUrl, l_bookReading.bookId, l_bookReading.id);
						l_contentList.Add(l_bookInfo);
						l_recordedBookList.Add (l_bookReading.bookId);
					}

					foreach (Book l_book in m_sessionHandler.bookTable.Values)
					{
						BookState l_state = BookState.Free;
						Token l_token = m_sessionHandler.token;
						
						if (l_token.isPremium() 
						    || l_token.isCurrent() 
						    || l_book.gems == 0 
						    || l_book.owned)
						{
							l_state = BookState.NotRecorded;
						} 
						else
						{
							l_state = BookState.NeedToBuy;
						}
						
						string l_record = LocalSetting.find("User").getString(l_book.id.ToString(), string.Empty);
						if (!l_record.Equals(string.Empty))
						{
							ArrayList l_list = MiniJSON.MiniJSON.jsonDecode(l_record) as ArrayList;
							
							foreach(object l_id in l_list)
							{
								if (l_id.ToString() == m_sessionHandler.currentKid.id.ToString() 
								    && l_state == BookState.NotRecorded)
								{
									l_state = BookState.Recorded;
								}
							}
						}
						
						BookInfo l_bookInfo = new BookInfo(l_book.title, l_state, l_book.coverUrl, l_book.id);
						
						if (l_recordedBookList.Contains(l_book.id))
						{
							continue;
						}
						
						l_contentList.Add(l_bookInfo);
					}
					
					m_sessionHandler.bookContentList = l_contentList;
					m_bookContentList = l_contentList;

					isFinishedLoadingBooks = true;
				}
			}
		}
		else
		{
			loadBookFail = true;
		}

		isDoneWithBookLoading = true;
		processFinishedRequests();
	}

	private void processFinishedRequests()
	{
		if (isDoneWithWebcontentLoading && isDoneWithBookLoading)
		{
			if (onLoadingCompleted != null)
			{
				onLoadingCompleted();
				onLoadingCompleted = null;
			}
		}

		isFinishedLoading = isFinishedLoadingWebContent && isFinishedLoadingBooks;
		if (isFinishedLoading)
		{
			isLoading = false;
		}
	}

	private void addBookList()
	{
		string language = "en";
		if (Application.systemLanguage == SystemLanguage.Spanish)
			language = "es";
		else
			language = "en";
		string fileName = "Books_" + language;
		string filePath = "Books/Data/" + fileName;
		TextAsset booklist = Resources.Load(filePath) as TextAsset;

		string l_string = booklist.text;
		l_string = UnicodeDecoder.Unicode (l_string);
		l_string = UnicodeDecoder.UnicodeToChinese (l_string);
		l_string = UnicodeDecoder.CoverHtmlLabel (l_string);
		Hashtable l_bookData = MiniJSON.MiniJSON.jsonDecode (l_string) as Hashtable;

		ArrayList l_books = l_bookData ["books"] as ArrayList;
		foreach (object o in l_books)
		{
			Book l_book = new Book (o as Hashtable);
			m_sessionHandler.addBook(l_book.id, l_book);
		}
		
		ArrayList l_readings = l_bookData ["readings"] as ArrayList;
		foreach (object o in l_readings)
		{
			BookReading l_bookReading = new BookReading (o as Hashtable);
			
			if (0 == l_bookReading.readingPageTable.Count)
			{
				continue;
			}
			
			m_sessionHandler.readingTable[l_bookReading.id] = l_bookReading;
		}
		
		List<object> l_contentList = new List<object>();
		List<int> l_recordedBookList = new List<int>();
		
		foreach (BookReading l_bookReading in m_sessionHandler.readingTable.Values)
		{
			string l_bookName = (m_sessionHandler.bookTable[l_bookReading.bookId] as Book).title;
			BookInfo l_bookInfo = new BookInfo(l_bookName, BookState.Recorded, l_bookReading.coverUrl, l_bookReading.bookId, l_bookReading.id);
			l_contentList.Add(l_bookInfo);
			l_recordedBookList.Add (l_bookReading.bookId);
		}
		
		foreach (Book l_book in m_sessionHandler.bookTable.Values)
		{
			BookState l_state = BookState.NotRecorded;
			
			string l_record = LocalSetting.find("User").getString(l_book.id.ToString(), string.Empty);
			if (!l_record.Equals(string.Empty))
			{
				ArrayList l_list = MiniJSON.MiniJSON.jsonDecode(l_record) as ArrayList;
				
				foreach(object l_id in l_list)
				{
					if (l_id.ToString() == m_sessionHandler.currentKid.id.ToString() 
					    && l_state == BookState.NotRecorded)
					{
						l_state = BookState.Recorded;
					}
				}
			}
			
			BookInfo l_bookInfo = new BookInfo(l_book.title, l_state, l_book.coverUrl, l_book.id);
			
			if (l_recordedBookList.Contains(l_book.id))
			{
				continue;
			}
			
			l_contentList.Add(l_bookInfo);
		}
		
		m_sessionHandler.bookContentList = l_contentList;
		m_bookContentList = l_contentList;
		
		isFinishedLoadingBooks = true;
	}
	
	//honda: comment out request book list due to book list is put in the local place
	private RequestQueue m_bookListRequest;
	private RequestQueue m_webContentRequest;
	private List<object> m_bookContentList;
	private SessionHandler m_sessionHandler;
}
