using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {

	void OnTriggerEnter() {
		GetComponent<Animator>().SetTrigger("OpenChest");
		transform.GetChild(2).gameObject.SetActive(true);
	}
}
