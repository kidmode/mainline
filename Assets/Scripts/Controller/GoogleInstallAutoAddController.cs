using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GoogleInstallAutoAddController : MonoBehaviour {

	public static GoogleInstallAutoAddController Instance;

	public  GoogleInstallAutoAddController instance;

	public Text displayInfo;

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

		Invoke("autoAdd", 1.0f);

		InvokeRepeating("checkList", 0.0f, 1.0f);

	}

	private void checkList (){
		
		List<object> l_list = KidMode.getApps();

		Debug.Log(" *1111111111111111111111111  h  l_list " + l_list.Count);

	}

	private void autoAdd (){

		int hasLuanchedGoogle = PlayerPrefs.GetInt("hasLaunchedGoogle");

		if(hasLuanchedGoogle == 1){
			
			List<object> l_list = KidMode.getApps();

			Debug.Log(" *******************************  h  l_list " + l_list.Count);
			
			List<object> lastLocalAppsList = KidMode.getLastLocalApps();
			
			
			
			List<object> selectedList = KidMode.getSelectedApps();
			
			ArrayList selectedArrayList = new ArrayList(selectedList);
			
			foreach (AppInfo l_app in l_list)
			{


				if(!lastLocalAppsList.Contains(l_app.packageName)){
					
					selectedArrayList.Add(l_app.packageName);
					
				}
				//				selectedList.add
				//l_app
				
			}
			
			PlayerPrefs.SetString( "addedAppList", MiniJSON.MiniJSON.jsonEncode(selectedArrayList) );

			hasLuanchedGoogle = 0;
			
		}


	}

	public void checkKidModeDefault(){



		displayInfo.text = "" + KidMode.isHomeLauncherKidMode();

	}

//	public void 



}
