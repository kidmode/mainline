using UnityEngine;
using System.Collections;

public class GAFSceneController : MonoBehaviour 
{
	public void RunDemoScene(string _SceneName)
	{
		Application.LoadLevel(_SceneName);
	}

	private void Update()
	{
		if (Input.GetMouseButtonUp(0))
		{
			if (Application.loadedLevelName != "Main")
				Application.LoadLevel("Main");
		}
	}
}
