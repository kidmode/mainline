using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GoogleInstallAutoAddController : MonoBehaviour {

	public static GoogleInstallAutoAddController Instance;

	public  GoogleInstallAutoAddController instance;

	public Text displayInfo;

	public int hasLuanchedGoogle;

	void Awake(){

		Instance = this;

	}

	// Use this for initialization
	void Start () {

		Instance = this;
	
	}
	
	// Update is called once per frame
	void Update () {

		Instance = this;
	
	}

	public void testList(){

//		Invoke("autoAdd", 1.0f);

		InvokeRepeating("autoAdd", 0.0f, 1.0f);

	}

	private void checkList (){

//		KidMode.getAllSystemApps();
		
		List<object> l_list = KidMode.getApps();

//		List<object> l_listSystemApps = 

		Debug.Log(" *1111111111111111111111111  h  l_list " + l_list.Count);

//		Debug.Log(" *1111111111111111111111111 l_listSystemApps " + l_listSystemApps.Count);

	}

	private void autoAdd (){

//		KidMode.getAllSystemApps();

//		int hasLuanchedGoogle = PlayerPrefs.GetInt("hasLaunchedGoogle");

		if(hasLuanchedGoogle == 1){
			
			List<object> l_list =  KidMode.getApps();

			Debug.Log(" ******************************* 0-00000000000000000000000000000000000000  h  l_list " + l_list.Count);
			
			List<object> lastLocalAppsList = KidMode.getLastLocalApps();
			
			ArrayList lastLocalAppsArrayList = new ArrayList(lastLocalAppsList);
			
			List<object> selectedList = KidMode.getSelectedAppsNames();
			
			ArrayList selectedArrayList = new ArrayList(selectedList);

			ArrayList newlySelectedArrayList = new ArrayList();

			if(lastLocalAppsList.Count < l_list.Count){
			
				foreach (AppInfo l_app in l_list)
				{


					if(!lastLocalAppsList.Contains(l_app.packageName)){

						Debug.Log(" !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!  ADDED " );
						
//						selectedArrayList.Add(l_app.packageName);

						newlySelectedArrayList.Add(l_app.packageName);

						hasLuanchedGoogle = 0;
						
					}

	//				PlayerPrefs.SetString( "addedAppList", MiniJSON.MiniJSON.jsonEncode(selectedArrayList) );
					//				selectedList.add
					//l_app
					
				}

				for (int selectedArrayListIndex = 0; selectedArrayListIndex < selectedArrayList.Count; selectedArrayListIndex++) {

					newlySelectedArrayList.Add(selectedArrayList[selectedArrayListIndex]);

				}


//				ArrayList selectedArrayListNames = new ArrayList();
//
//				for (int i = 0; i < selectedArrayList.Count; i++) {
//
//					AppInfo info = selectedArrayList[i] as AppInfo;
//
//					selectedArrayListNames.Add(info.packageName);
//				}



				string json =  MiniJSON.MiniJSON.jsonEncode(newlySelectedArrayList);

				Debug.Log(" !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!  json "  + json);

				string refJson =  MiniJSON.MiniJSON.jsonEncode(lastLocalAppsArrayList); 

				Debug.Log(" !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!  refJson "  + refJson);

				PlayerPrefs.SetString( "addedAppList", json );

			}
			


//			hasLuanchedGoogle = 0;
			
		}


	}

	public void checkKidModeDefault(){



		displayInfo.text = "" + KidMode.isHomeLauncherKidMode();

	}

//	public void 



}
