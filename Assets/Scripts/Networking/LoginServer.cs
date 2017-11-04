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
			if(PlayerPrefs.HasKey("remember-me")) {
				StartCoroutine(Relog(PlayerPrefs.GetString("Prefill-email"), PlayerPrefs.GetString("remember-me")));
			}
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
			StartCoroutine(Login("publictestaccount", "tester123", true));
	}

	IEnumerator Login(string email, string password, bool dontRemember = false) {
		failed.text = "";
		passwordField.text = "";
		thisButton.interactable = false;
		if(!dontRemember)
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
				if(dontRemember) {
					PlayerPrefs.DeleteKey("remember-me");
				} else {
					PlayerPrefs.SetString("remember-me", res.currentID);
				}
				GameController.instance.networkServer.SetUID(res.currentID);
				GameController.instance.networkServer.Connect();
			}
        }
    }

	IEnumerator Relog(string email, string _currentID, bool dontRemember = false) {
		failed.text = "";
		passwordField.text = "";
		thisButton.interactable = false;
        WWWForm details = new WWWForm();
		details.AddField("email", email);
		details.AddField("currentID", _currentID);
        UnityWebRequest www = UnityWebRequest.Post("https://warriars.fun/relog", details);
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
				if(dontRemember) {
					PlayerPrefs.DeleteKey("remember-me");
				} else {
					PlayerPrefs.SetString("remember-me", res.currentID);
				}
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
