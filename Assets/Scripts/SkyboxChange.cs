using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxChange : MonoBehaviour {

	public Material skobyx;

	// Use this for initialization
	void Start () {
		RenderSettings.skybox = skobyx;
	}
	
	// Update is called once per frame
	void OnDisable () {

		RenderSettings.skybox = null;
		
	}
}
