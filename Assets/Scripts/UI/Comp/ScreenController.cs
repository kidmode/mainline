using UnityEngine;
using System.Collections;

/*		
 *
 *
*/

public class ScreenController : MonoBehaviour {

	public GameObject[] screens;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void closeAllScreens(){
		
		for (int i = 0; i < screens.Length; i++) {
			
			screens[i].gameObject.SetActive(false);
		}
		
	}

	public void showScreenAtIndex(int index){
		
		closeAllScreens();
		
		screens[index].gameObject.SetActive(true);
		
	}
	
	public void showInitialScreen(){
		
		showScreenAtIndex(0);
		
	}

}
