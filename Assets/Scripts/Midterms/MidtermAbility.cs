using UnityEngine;

public class MidtermAbility : MonoBehaviour {
	public GameObject tooltip;

	void Start() {
		tooltip.SetActive(false);
	}
	
	void Update() {
		
	}

	public void OnHoverEnter() {
		//tooltip.SetActive(true);
	}

	public void OnHoverExit() {
		//tooltip.SetActive(false);
	}
}
