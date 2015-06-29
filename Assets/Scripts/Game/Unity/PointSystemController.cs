using UnityEngine;
using System.Collections;

public class PointSystemController : MonoBehaviour {

	public static PointSystemController Instance;

	[SerializeField]
	private float pointSystemRewardTime = 5.0f;

	public enum PointRewardState {

		No_Point,
		OK

	}

	[SerializeField]
	private PointRewardState pointState;

	void Awake(){

		Instance = this;

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setPointOK(PointRewardState state){

		pointState = state;

	}

	public PointRewardState pointSystemState(){

		return pointState;

	}

	public void startPointSystemTimer(){

		Invoke ("setPointRewardOK", pointSystemRewardTime);

	}

	public void stopPointSystemTimer(){
		
		CancelInvoke ("setPointRewardOK");
		
	}

	void setPointRewardOK(){

		pointState = PointRewardState.OK;

	}

}
