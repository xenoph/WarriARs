using UnityEngine;
using UnityEngine.SceneManagement;

public class LogMeOut : MonoBehaviour {

	public void Logout() {
		if(PlayerPrefs.HasKey("remember-me")) {
			PlayerPrefs.DeleteKey("remember-me");
			GameController.instance.networkServer.Disconnect();
			SceneManager.LoadScene("main");
		}
	}
}
