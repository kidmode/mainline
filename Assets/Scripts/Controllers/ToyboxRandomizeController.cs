using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;

public class ToyboxRandomizeController : MonoBehaviour {


	public List<object> webContentList;

	private ArrayList videoSortedList;

	private ArrayList gameSortedList;

	private ArrayList videoShownRecordList;
	
	private ArrayList gameShownRecordList;

	public static ToyboxRandomizeController Instance;

	void Awake(){

		Instance= this;

	}

	void OnDisable(){

	}

	// Use this for initialization
	void Start () {


	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void startList(ArrayList list){

		list = new ArrayList();

	}

	public void restartAllRecordList(){

		videoShownRecordList = getRecordList( WebContent.VIDEO_TYPE);

		gameShownRecordList = getRecordList(WebContent.GAME_TYPE);

//		debug_WriteInfo();

	}


	ArrayList getRecordList(int listType){

		ArrayList returnArrayList = new ArrayList();

		for (int i = 0; i < SessionHandler.getInstance().webContentList.Count; i++) {

			WebContent content = SessionHandler.getInstance().webContentList[i] as WebContent;

			if(content.category == listType){

				ShownToyboxRecord record = new ShownToyboxRecord(content);

				returnArrayList.Add(record);

			}
			
		}

		return returnArrayList;

	}

	//Returns sorted recorded List from lowest to highest 
	ArrayList getSortedRecordList(ArrayList recordList){

		ArrayList returnList = new ArrayList(); 

//		ArrayList sortingList = recordList;



		while(returnList.Count < recordList.Count){

			int minShownCount = 40000;

			int minShownCountIndex = -1;

			for (int i = 0; i < recordList.Count; i++) {
				
				ShownToyboxRecord record = recordList[i] as ShownToyboxRecord;

				if(!returnList.Contains(record)){

					if(record.shownCount <= minShownCount){

						minShownCount = record.shownCount;

						minShownCountIndex = i;

					}

				}

			}

			returnList.Add(recordList[minShownCountIndex]);

		}

		return returnList;

//		for (int i = 0; i < sortingList.Count; i++) {
//
//
//		}

	}

	ArrayList getListofContentType(int listType){

		ArrayList returnList = new ArrayList(); 

		for (int i = 0; i < SessionHandler.getInstance().webContentList.Count; i++) {
			
			WebContent content = SessionHandler.getInstance().webContentList[i] as WebContent;

			if(content.category == listType){

				returnList.Add(content);

			}else if(content.category == listType){

				returnList.Add(content);

			}

		}

		return returnList;

	}

	//From a record list get a list of record shown that are of the lowest shown count
	ArrayList getLowestShownCountGroup(ArrayList recordList){

		ArrayList returnList = new ArrayList();

		ShownToyboxRecord lowestShownRecord = recordList[0] as ShownToyboxRecord;

		for (int i = 0; i < recordList.Count; i++) {

			ShownToyboxRecord record= recordList[i] as ShownToyboxRecord;

			if(lowestShownRecord.shownCount == record.shownCount){

				returnList.Add(record);

			}

		}

		return returnList;
		
	}

	//From a record list get a list of with record count
	ArrayList getLowestShownCountGroup(ArrayList recordList, int matchingCount){
		
		ArrayList returnList = new ArrayList();
		
		for (int i = 0; i < recordList.Count; i++) {
			
			ShownToyboxRecord record= recordList[i] as ShownToyboxRecord;
			
			if(record.shownCount == matchingCount){
				
				returnList.Add(record);
				
			}
			
		}
		
		return returnList;
		
	}



	//Get video sorted List
	public List<WebContent> getVideoList(){
		//Get max returned list
		int maxListCount = 30;
		if(SessionHandler.getInstance().token.isPremium()){
			maxListCount = 50;
		}

//		maxListCount = 46;

		ArrayList videoAllList = getListofContentType(WebContent.VIDEO_TYPE);

		ArrayList returnList = new ArrayList();

		if(videoAllList.Count <= maxListCount){

//			returnList = videoAllList;

			while(videoAllList.Count > 0){
				
				WebContent content = videoAllList[Random.Range(0, videoAllList.Count)] as WebContent;
				
				returnList.Add(content);
				
				videoAllList.Remove(content);
				
			}

//			returnList = videoAllList;
		}else{//Start sorting

			ArrayList sortedRecordList = getSortedRecordList(videoShownRecordList);

			debug_WriteInfo();


			while(returnList.Count < maxListCount){

				ArrayList lowestShownCountGroup = getLowestShownCountGroup(sortedRecordList);

				//========
				//Check to make sure there is no repeats
				//is the lowest count group is less than 6 and the totall count is around the same as max count
				//The lowest group will always show up and therefore doesn't seem to be rotating
				//see if we could get the rest of the group and add those to the lowest count group if this occurs
				int lowRepeateCount = 6;

				if(lowestShownCountGroup.Count <= lowRepeateCount){ //

					if(sortedRecordList.Count > lowRepeateCount * 2){

						int lowestShowCount = 0;

						//remove all the lowest count from the sorted group
						for (int i = 0; i < lowestShownCountGroup.Count; i++) {

							ShownToyboxRecord shownRecord = lowestShownCountGroup[i] as ShownToyboxRecord;

							lowestShowCount = shownRecord.shownCount;

							
						}


						ArrayList secondLowestShownCountGroup = getLowestShownCountGroup(sortedRecordList, lowestShowCount);

						for (int i = 0; i < secondLowestShownCountGroup.Count; i++) {

							ShownToyboxRecord shownRecord = lowestShownCountGroup[i] as ShownToyboxRecord;

							lowestShownCountGroup.Add(shownRecord);

						}

					}
				}


				while(lowestShownCountGroup.Count > 0){

					ShownToyboxRecord randomShownRecord = lowestShownCountGroup[Random.Range(0, lowestShownCountGroup.Count)] as ShownToyboxRecord;

					if(!returnList.Contains(randomShownRecord.content)){

						randomShownRecord.shownCount++;
						
						returnList.Add(randomShownRecord.content);

					}



					lowestShownCountGroup.Remove(randomShownRecord);

					if(sortedRecordList.Contains(randomShownRecord))
						sortedRecordList.Remove(randomShownRecord);


					if(returnList.Count == maxListCount){
						
						break;
						
					}



				}

			}

//			Debug.LogError("  videoAllList  sdf sad f sadf asdf s sortedRecordList.Count " + sortedRecordList.Count);

		}


		//=== = = = == = = = = = = == = == = = = = == = = = = == == 
//		Debug.LogError("  videoAllList  sdf sad f sadf asdf sad f VIdeo only " + videoAllList.Count);

		//------

		List<WebContent> list = new List<WebContent>(returnList.Count);
		foreach (WebContent instance in returnList)
		{
			list.Add(instance);
		}
		return list;

	}

	//Get video sorted List
	public List<WebContent> getGamesList(){

		Debug.LogError(" -- - - - - -- - - - - - - -  getGamesList  -- - - - - -- - - - - - - - ");

		//Get max returned list
		int maxListCount = 30;
		if(SessionHandler.getInstance().token.isPremium()){
			maxListCount = 50;
		}
		
//		maxListCount = 210;
		
		ArrayList gameAllList = getListofContentType(WebContent.GAME_TYPE);
		
		ArrayList returnList = new ArrayList();

		Debug.LogError(" -- - - - - -- - - - - - - -  gameAllList " + gameAllList.Count);
		
		if(gameAllList.Count <= maxListCount){

			while(gameAllList.Count > 0){
				
//				ArrayList lowestShownCountGroup = getLowestShownCountGroup(sortedRecordList);
				
				WebContent content = gameAllList[Random.Range(0, gameAllList.Count)] as WebContent;
				
				returnList.Add(content);
				
				gameAllList.Remove(content);
				
			}
			
//			returnList = gameAllList;
			
		}else{//Start sorting
			
			ArrayList sortedRecordList = getSortedRecordList(gameShownRecordList);

			Debug.LogError(" -- - - - - -- - - - - - - -  sortedRecordList " + sortedRecordList.Count);
			
			
//			debug_WriteInfo();
			
			
			while(returnList.Count < maxListCount){
				
				ArrayList lowestShownCountGroup = getLowestShownCountGroup(sortedRecordList);
				
				ShownToyboxRecord randomShownRecord = lowestShownCountGroup[Random.Range(0, lowestShownCountGroup.Count)] as ShownToyboxRecord;
				
				randomShownRecord.shownCount++;
				
				returnList.Add(randomShownRecord.content);
				
				sortedRecordList.Remove(randomShownRecord);
				
			}
			
			Debug.LogError("  videoAllList  sdf sad f sadf asdf s sortedRecordList.Count " + sortedRecordList.Count);
			
		}
		
		
		//=== = = = == = = = = = = == = == = = = = == = = = = == == 
		Debug.LogError("  videoAllList  sdf sad f sadf asdf sad f VIdeo only " + gameAllList.Count);
		
		//------
		
		List<WebContent> list = new List<WebContent>(returnList.Count);
		foreach (WebContent instance in returnList)
		{
			list.Add(instance);
		}
		return list;
		
	}


	private void debug_WriteInfo(){

		#if UNITY_ANDROID && !UNITY_EDITOR
		return;
		
		#endif

		StreamWriter writer = new StreamWriter(Application.dataPath + "/Resources/_Debug/sortingAppList.txt"); // Does this work?
		
		string debugString = "";
		
		for (int i = 0; i < videoShownRecordList.Count; i++) {
			
			ShownToyboxRecord record = videoShownRecordList[i] as ShownToyboxRecord;
			
			debugString = debugString + "\n   id: " + record.content.id + "    " + record.shownCount; 
			
		}
		
		writer.WriteLine(debugString);
		writer.Close();

	}

}


public class ShownToyboxRecord{

	public WebContent content;

	public int shownCount;

	public ShownToyboxRecord(WebContent content){

		this.content = content;

		shownCount = 0;

	}

}
