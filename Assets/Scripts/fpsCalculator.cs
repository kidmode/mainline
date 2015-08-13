using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class fpsCalculator : MonoBehaviour {


	private Text m_text;
	private float updateInterval = 1.0f;
	private float lastInterval = 0.0f;
	private long frames = 0;

	// Use this for initialization
	void Start () {
//		m_text = GameObject.FindObjectOfType<Text>();
		m_text = GameObject.FindGameObjectWithTag("FPS").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		++frames;
		float timeNow = Time.realtimeSinceStartup;
		if (timeNow > lastInterval + updateInterval)
		{
			float fps = frames / (timeNow - lastInterval);
			float ms = 1000.0f / Mathf.Max(fps, 0.00001f);
			m_text.text = "5101(614)" + ms.ToString("f1") + "ms " + fps.ToString("f2") + "FPS";
			frames = 0;
			lastInterval = timeNow;
		}
	}
}
