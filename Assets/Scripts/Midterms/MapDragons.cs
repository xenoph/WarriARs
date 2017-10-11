using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDragons : MonoBehaviour {
	[Range(1, 3)]
	public int dragonID = 1;

	void Start () {
		Animator anim = GetComponent<Animator>();
 
        float randomIdleStart = Random.Range(0, anim.GetCurrentAnimatorStateInfo(0).length);
        anim.Play("idle", 0, randomIdleStart);
	}
	
	void Update() {
		
	}
}
