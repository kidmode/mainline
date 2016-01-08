using UnityEngine;
using System.Collections;

public class RecordingDownloadController : MonoBehaviour {

	public static RecordingDownloadController Instance;

	ArrayList ReadingsList;

	void Awake(){

		InternetPingController.OnInternetConnectionOkay += OnInternetConnectionOkay;

		Instance = this;

	}

	void OnDisable(){

		InternetPingController.OnInternetConnectionOkay -= OnInternetConnectionOkay;

	}

	// Use this for initialization
	void Start () {



		loadBookList();
	
		Invoke("testGetbookReading", 3.0f);

	}


	
	// Update is called once per frame
	void Update () {
	
	}

	void OnInternetConnectionOkay(){

		Debug.Log("   sdfjasdf asdfsd f  dsf a OnInternetConnectionOkay " );

		loadBookList();
		     

	}

	private void testGetbookReading(){

		getBookReading(8);

	}

	private void getBookReading(int bookId){

		RequestQueue l_request = new RequestQueue ();
		l_request.add ( new GetBookReadingRequest( bookId ) );
		l_request.request (RequestType.RUSH);

	}

	void loadBookList(){

//		foreach( Book l_book in SessionHandler.getInstance().bookList )
//		{
//			m_bookDataTable[l_book.id] = l_book;
//			
//			string l_record = LocalSetting.find("User").getString(l_book.id.ToString(), string.Empty);
//			if( !l_record.Equals(string.Empty) )
//			{
//				ArrayList l_list = MiniJSON.MiniJSON.jsonDecode(l_record) as ArrayList;
//				
//				foreach( object l_id in l_list )
//				{
//					if( l_id.ToString() == SessionHandler.getInstance().currentKid.id.ToString() )
//					{
//						m_recordedBookList.Add( l_book.id );
//					}
//				}
//			}
//		}
		
		RequestQueue l_request = new RequestQueue ();
		l_request.add ( new GetReadingsRequest( _requestReadingListComplete ) );
		l_request.request (RequestType.RUSH);

	}

	private void _requestReadingListComplete(HttpsWWW p_response)
	{
			
		if (p_response.error == null)
		{

			ReadingsList = new ArrayList();

			string l_string = "";
			l_string = UnicodeDecoder.Unicode(p_response.text);
			l_string = UnicodeDecoder.UnicodeToChinese(l_string);
			l_string = UnicodeDecoder.CoverHtmlLabel(l_string);
			
			ArrayList l_data = MiniJSON.MiniJSON.jsonDecode(l_string) as ArrayList;
			foreach (object o in l_data)
			{
				BookReading l_bookReading = new BookReading(o as Hashtable);
				
				
				Reading read = new Reading(o as Hashtable);
				
//				Debug.LogError("  00000000000000000000000000000000000  read.bookId " + read.bookId);
//				
//				Debug.LogError("   000000000000000000000000000000000000000000   read.created " + read.createdAt);

				ReadingsList.Add(read);


			}

		}



	}

	public string getPlaybackURL(int bookId, int pageIndex, int pageId){

		for (int i = 0; i < ReadingsList.Count; i++) {

			Reading read = ReadingsList[i] as Reading;


			if(read.bookId == bookId){

				for (int j = 0; j < read.pageList.Count; j++) {

					ReadingPage page = read.pageList[j];

					if(page.pageId == pageId){

						return page.playbackUrl;

					}

				}
				
			}

//			if(read.bookId == bookId){
//
//				ReadingPage page = read.pageList[pageIndex] ;
//
//				Debug.Log(" getPlaybackURL duraiton " + page.duration);
//
//				return page.playbackUrl;
//
//			}


		}

		return null;

	}

	public int requestPageIndex(int bookId, int pageIndex){

		for (int i = 0; i < ReadingsList.Count; i++) {
			
			Reading read = ReadingsList[i] as Reading;
			
			if(read.bookId == bookId){
				
				ReadingPage page = read.pageList[pageIndex] ;
				
				Debug.Log(" getPlaybackURL duraiton " + page.duration);
				
				return page.pageId;
				
			}
			
			
		}

		return -1;

	}



}
