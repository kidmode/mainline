using UnityEngine;
using System.Collections;

public class GAFShaderSet : MonoBehaviour {

	public string shaderName = "Particles/VertexLit Blended";

	// Use this for initialization
	void Start () {
	
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
		
		for (int matIndex = 0; matIndex < gafRenderer.materials.Length; matIndex++) {
			
			gafRenderer.materials[matIndex].shader = betterUpdateShader;
			
			size++;
			
			//						if(gafRenderer.materials[matIndex].GetFloat("_Alpha") >= 1.0f){
			////							
			//							gafRenderer.materials[matIndex].shader = betterUpdateShader;
			////							
			//						}
			
		}

	}
}
