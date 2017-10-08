using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public GameObject deathEffect;

	// Use this for initialization
	void Start () {
		Instantiate (deathEffect);
		Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
