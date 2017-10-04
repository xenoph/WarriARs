using UnityEngine;

public class LoginScreen : MonoBehaviour {

	void Start() {
		GameController.instance.loginScreen = this;
	}
}
