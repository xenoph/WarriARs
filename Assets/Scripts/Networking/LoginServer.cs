using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class LoginServer : MonoBehaviour {

	public InputField emailField;
	public InputField passwordField;
	public Text failed;

	private Button thisButton;

	void Start() {
		thisButton = GetComponent<Button>();
		thisButton.interactable = true;
		if(PlayerPrefs.HasKey("Prefill-email") && emailField != null) {
			emailField.text = PlayerPrefs.GetString("Prefill-email");
		}
	}

	public void Register() {
		Application.OpenURL("https://warriars.fun/register");
	}
	
	public void LoginToServer() {
		if(gameObject.activeInHierarchy)
			StartCoroutine(Login(emailField.text, passwordField.text));
	}

	public void LoginToTest() {
		if(gameObject.activeInHierarchy)
			StartCoroutine(Login("publictestaccount@oisann.net", "tester123"));
	}

	IEnumerator Login(string email, string password) {
		failed.text = "";
		passwordField.text = "";
		thisButton.interactable = false;
		PlayerPrefs.SetString("Prefill-email", email);
        WWWForm details = new WWWForm();
		details.AddField("email", email);
		details.AddField("password", password);
        UnityWebRequest www = UnityWebRequest.Post("https://warriars.fun/login", details);
        yield return www.Send();
		
		thisButton.interactable = true;
        if(www.isNetworkError) {
            Debug.Log("Error: " + www.error);
        } else {
            responds res = JsonUtility.FromJson<responds>(www.downloadHandler.text);
			if(!string.IsNullOrEmpty(res.error)) {
				failed.text = res.error;
			} else {
				GameController.instance.loadingScreen.gameObject.SetActive(true);
				GameController.instance.loadingScreen.LoadingText.text = "Connecting to server...";
				GameController.instance.networkServer.SetUID(res.currentID);
				GameController.instance.networkServer.Connect();
			}
        }
    }

	private struct responds {
		public string error, currentID, username;
	}

	private int getCurrentTimestamp() {
        return (int) (System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
    }
}
