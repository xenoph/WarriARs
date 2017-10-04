using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour {
	public bool showing = false;
	public Image background;
	public Text text;

	void Start() {
		GameController.instance.warning = this;
	}

	void LateUpdate() {
		if(background.gameObject.activeInHierarchy != showing) {
			background.gameObject.SetActive(showing);
		}
	}
}
