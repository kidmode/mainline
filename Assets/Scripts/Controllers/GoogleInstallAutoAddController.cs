using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class GoogleInstallAutoAddController : MonoBehaviour {

	public static GoogleInstallAutoAddController Instance;

	public  GoogleInstallAutoAddController instance;

	public Text displayInfo;

	public int hasLuanchedGoogle;

	public static event Action OnNewAppAdded;

	void Awake(){

		Instance = this;

	}

	// Use this for initialization
	void Start () {

		setLocalAppNamesSortedByAddedTime();

//		ArrayList list = KidMode.getSelectedAppsSorted();

		Instance = this;
	
	}
	
	// Update is called once per frame
	void Update () {

		Instance = this;
	
	}

	public void checkList(){

//		Invoke("autoAdd", 1.0f);

		InvokeRepeating("autoAdd", 0.0f, 1.0f);

	}

//	private void checkList (){
//
////		KidMode.getAllSystemApps();
//		
//		List<object> l_list = KidMode.getApps();
//
////		List<object> l_listSystemApps = 
//
//		Debug.Log(" *1111111111111111111111111  h  l_list " + l_list.Count);
//
////		Debug.Log(" *1111111111111111111111111 l_listSystemApps " + l_listSystemApps.Count);
//
//	}

	private void autoAdd (){

//		KidMode.getAllSystemApps();

//		int hasLuanchedGoogle = PlayerPrefs.GetInt("hasLaunchedGoogle");

//		if(hasLuanchedGoogle == 1){
			
			List<object> l_list =  KidMode.getApps();

//			Debug.Log(" ******************************* 0-00000000000000000000000000000000000000  h  l_list " + l_list.Count);
			
			List<object> lastLocalAppsList = KidMode.getLastLocalApps();
			
			ArrayList lastLocalAppsArrayList = new ArrayList(lastLocalAppsList);
			
			List<object> selectedList = KidMode.getSelectedAppsNames();
			
			ArrayList selectedArrayList = new ArrayList(selectedList);

			ArrayList newlySelectedArrayList = new ArrayList();

			if(lastLocalAppsList.Count < l_list.Count){

				setLocalAppNamesSortedByAddedTime();

				ArrayList sortedAppNames = getLocallAppNamesSoretedByAddedTime();

				for (int sortedAppNamesIndex = 0; sortedAppNamesIndex < sortedAppNames.Count; sortedAppNamesIndex++) {


					if(selectedArrayList.Contains(sortedAppNames[sortedAppNamesIndex])){

						newlySelectedArrayList.Add(sortedAppNames[sortedAppNamesIndex]);



					}

					if(!lastLocalAppsList.Contains(sortedAppNames[sortedAppNamesIndex])){

						newlySelectedArrayList.Add(sortedAppNames[sortedAppNamesIndex]);

					}

				}

				hasLuanchedGoogle = 0;


			
//				foreach (AppInfo l_app in l_list)
//				{
//
//
//					if(!lastLocalAppsList.Contains(l_app.packageName)){
//
//						Debug.Log(" !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!  ADDED " );
//						
////						selectedArrayList.Add(l_app.packageName);
//
//						newlySelectedArrayList.Add(l_app.packageName);
//
//						hasLuanchedGoogle = 0;
//						
//					}
//
//	//				PlayerPrefs.SetString( "addedAppList", MiniJSON.MiniJSON.jsonEncode(selectedArrayList) );
//					//				selectedList.add
//					//l_app
//					
//				}
//
//				for (int selectedArrayListIndex = 0; selectedArrayListIndex < selectedArrayList.Count; selectedArrayListIndex++) {
//
//					newlySelectedArrayList.Add(selectedArrayList[selectedArrayListIndex]);
//
//				}


//				ArrayList selectedArrayListNames = new ArrayList();
//
//				for (int i = 0; i < selectedArrayList.Count; i++) {
//
//					AppInfo info = selectedArrayList[i] as AppInfo;
//
//					selectedArrayListNames.Add(info.packageName);
//				}



				string json =  MiniJSON.MiniJSON.jsonEncode(newlySelectedArrayList);

//				Debug.Log(" !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!  json "  + json);

//				string refJson =  MiniJSON.MiniJSON.jsonEncode(lastLocalAppsArrayList); 
//
//				Debug.Log(" !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!  refJson "  + refJson);

				PlayerPrefs.SetString( "addedAppList", json );

				if(OnNewAppAdded != null)
					OnNewAppAdded();

				CancelInvoke("autoAdd");

//			}
			


//			hasLuanchedGoogle = 0;
			
		}


	}


	//=============================================
	public void setLocalAppNamesSortedByAddedTime()
	{
		//		#if UNITY_ANDROID && !UNITY_EDITOR
		
		string localAppSortedByDateString = PlayerPrefs.GetString( "localAppNamesSortedAddedTime");

		ArrayList listSortedAppNames = MiniJSON.MiniJSON.jsonDecode( localAppSortedByDateString ) as ArrayList;


		ArrayList newlySelectedArrayList = new ArrayList();

		if( null == listSortedAppNames )
		{

			listSortedAppNames = new ArrayList();



		}

		//====
		//default apps
		TextAsset defaultApps = Resources.Load("Data/Default_Native_Apps") as TextAsset;
		string[] names = defaultApps.text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
		List<string> defaultAppsList = new List<string>(names);

		//==========
		//Adding new apps to the sorted list;
		//so it is at the front
		List<object> l_list =  KidMode.getApps();

		foreach (AppInfo l_app in l_list)
		{

			if(!listSortedAppNames.Contains(l_app.packageName) && !defaultAppsList.Contains(l_app.packageName)){

				newlySelectedArrayList.Add(l_app.packageName);

			}

		}


		//Add the rest of the sorted apps to the sorted list
		for (int listSortedAppNamesIndex = 0; listSortedAppNamesIndex < listSortedAppNames.Count; listSortedAppNamesIndex++) {

			string packageName = listSortedAppNames[listSortedAppNamesIndex] as string;
			if(!defaultAppsList.Contains(packageName))
				newlySelectedArrayList.Add(listSortedAppNames[listSortedAppNamesIndex]);
			
		}

		//Now add the default apps 
		//Add default apps now
		//

		foreach (string name in defaultAppsList)
		{
			newlySelectedArrayList.Add(name);
		}



		PlayerPrefs.SetString( "localAppNamesSortedAddedTime", MiniJSON.MiniJSON.jsonEncode(newlySelectedArrayList) );
		
	}

	public ArrayList getLocallAppNamesSoretedByAddedTime(){

		string localAppSortedByDateString = PlayerPrefs.GetString( "localAppNamesSortedAddedTime");
		
		ArrayList listSortedAppNames = MiniJSON.MiniJSON.jsonDecode( localAppSortedByDateString ) as ArrayList;

		if( null == listSortedAppNames )
		{
			
			listSortedAppNames = new ArrayList();
			
		}

		return listSortedAppNames;

	}





	public void checkKidModeDefault(){



		displayInfo.text = "" + KidMode.isHomeLauncherKidMode();

	}

	//========================================
	//
	public void saveAppListJson(){

	}

	public void loadAppListJson(){

	}

//	public void 



}


