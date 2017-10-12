using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour {

	void Start() {
		Invoke("DestroyMe", 2f);
	}

	private void DestroyMe() {
		Destroy(gameObject);
	}
}