using UnityEngine;
using System.Collections;

public class UIMoveLeftRight : MonoBehaviour {

	[SerializeField]
	private Transform pointMoveLeft;
	[SerializeField]
	private Transform pointMoveRight;


	public float speed = 300;

	public enum State{
		NONE,
		MOVELEFT,
		MOVERIGHT
	}
	public State state;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(state == State.MOVELEFT){
			
			if(gameObject.transform.position.x > pointMoveLeft.position.x){
				
				Vector3 pos = gameObject.transform.position;

				pos.x = pos.x - speed * Time.deltaTime;  

				if(pos.x < pointMoveLeft.position.x){

					pos.x = pointMoveLeft.position.x;

				}

				gameObject.transform.position = pos;

			}else{
				
				state = State.NONE;
				
			}
			
		}else if(state == State.MOVERIGHT){
			
			if(gameObject.transform.position.x < pointMoveRight.position.x){
				
				Vector3 pos = gameObject.transform.position;
				
				pos.x = pos.x + speed * Time.deltaTime;

				if(pos.x > pointMoveRight.position.x){
					
					pos.x = pointMoveRight.position.x;
					
				}

				gameObject.transform.position = pos;
				
			}else{
				
				state = State.NONE;
				
			}
			
		}
	
	}


	public void startMoveLeft(){
		
		state = State.MOVELEFT;
		
	}

	public void setPosToLeft(){

		state = State.NONE;

		Vector3 pos = gameObject.transform.position;

		pos = pointMoveLeft.position;

		gameObject.transform.position = pos;

	}
	
	public void startMoveRight(){
		
		state = State.MOVERIGHT;
		
	}
}
