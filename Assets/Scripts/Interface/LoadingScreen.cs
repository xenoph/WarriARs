using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {

	public Text LoadingText;

	void Start() {
		GameController.instance.loadingScreen = this;
		this.gameObject.SetActive(false);
	}
}
