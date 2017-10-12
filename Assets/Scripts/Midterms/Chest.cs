using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {

	public void Collect() {
		transform.GetChild(2).gameObject.SetActive(false);
		transform.GetChild(3).gameObject.SetActive(true);
		Destroy(gameObject, 3f);
	}

	void OnTriggerEnter() {
		GetComponent<Animator>().SetTrigger("OpenChest");
		transform.GetChild(2).gameObject.SetActive(true);
	}
}
