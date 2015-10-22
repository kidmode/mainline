using UnityEngine;
using System.Collections;

public class ParticleSortingLayer : MonoBehaviour {

	public int startSortingOrder = 8;

	// Use this for initialization
	void Start ()
	{
		// Set the sorting layer of the particle system.

//		return;
		GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = "Default";
		GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingOrder = startSortingOrder;

		//particleSystem.renderer.camera
	}
	
	// Update is called once per frame
	void Update () {
//		return;
//		particleSystem.renderer.sortingLayerName = "Default";
//		particleSystem.renderer.sortingOrder = startSortingOrder;
//	
	}
}
