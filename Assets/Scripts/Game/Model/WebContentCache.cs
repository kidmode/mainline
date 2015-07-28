using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	//end

	public void startRequests()
	{
		m_sessionHandler = SessionHandler.getInstance();

		clear();

		m_webContentRequest = new RequestQueue();
		m_webContentRequest.add(new WebContentRequest(_requestWebContentComplete));
		m_webContentRequest.request(RequestType.SEQUENCE);

		//honda
		addBookList();
		//honda: comment out request book list due to book list is put in the local place
//		m_bookListRequest = new RequestQueue();
//		m_bookListRequest.add(new BookListRequest(true, _requestBookListComplete));
//		m_bookListRequest.request();
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
	//end

	public void clear()
	{
		isLoading = false;
		isFinishedLoading = false;
		isFinishedLoadingWebContent = false;
		isFinishedLoadingBooks = false;
		loadWebContentFail = false;
		loadBookFail = false;

		m_sessionHandler.resetKidCacheLists();

		if (null != m_webContentRequest)
		{
			m_webContentRequest.dispose();
			m_webContentRequest = null;
		}

		//honda: comment out request book list due to book list is put in the local place
//		if (null != m_bookListRequest)
//		{
//			m_bookListRequest.dispose();
//			m_bookListRequest = null;
//		}

		if (null != m_bookContentList)
		{
			for (int i = 0; i < m_bookContentList.Count; ++i)
			{
				BookInfo l_info = m_bookContentList[i] as BookInfo;
				l_info.dispose();
			}
		}
	}

	private void _requestWebContentComplete(WWW p_response)
	{
		isFinishedLoadingWebContent = true;
		if (onLoadingCompleted != null)
		{
			onLoadingCompleted();
			onLoadingCompleted = null;
		}

		if (p_response.error == null)
		{
			List<object> l_contentList = new List<object> ();
			string text = p_response.text;
			ArrayList l_data = MiniJSON.MiniJSON.jsonDecode(p_response.text) as ArrayList;
			foreach (object o in l_data)
			{
				l_contentList.Add(new WebContent(o as Hashtable));
			}
			m_sessionHandler.webContentList = l_contentList;
		}
		else
		{
			loadWebContentFail = true;
		}

		processFinishedRequests();
	}

	private void _requestBookListComplete(WWW p_response)
	{
		isFinishedLoadingBooks = true;

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
				}
			}
		}
		else
		{
			loadBookFail = true;
		}
	}

	private void processFinishedRequests()
	{
		isFinishedLoading = isFinishedLoadingWebContent
							&& isFinishedLoadingBooks;

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
//	private RequestQueue m_bookListRequest;
	private RequestQueue m_webContentRequest;
	private List<object> m_bookContentList;
	private SessionHandler m_sessionHandler;
}
