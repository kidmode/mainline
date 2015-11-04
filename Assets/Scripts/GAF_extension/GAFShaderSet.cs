using UnityEngine;
using System.Collections;

public class GAFShaderSet : MonoBehaviour {

	public string shaderName = "Particles/VertexLit Blended";

	void Awake(){

		setShader();

	}

	// Use this for initialization
	void Start () {
	
		setShader();

	}
	
	// Update is called once per frame
	void Update () {

		setShader();
	
	}

	void setShader(){

		Renderer gafRenderer = gameObject.GetComponent<Renderer>();
		
		Shader betterUpdateShader = Shader.Find(shaderName);
		
		//					gafRenderer.material.shader = betterUpdateShader;
		
		//					Debug.Log("gafClips[i] " + gafClips[i].gameObject.name);
		//
		
		int size = 0;
		
		for (int matIndex = 0; matIndex < gafRenderer.sharedMaterials.Length; matIndex++) {
			
			gafRenderer.sharedMaterials[matIndex].shader = betterUpdateShader;
			
			size++;
			
			//						if(gafRenderer.materials[matIndex].GetFloat("_Alpha") >= 1.0f){
			////							
			//							gafRenderer.materials[matIndex].shader = betterUpdateShader;
			////							
			//						}
			
		}

	}
}
