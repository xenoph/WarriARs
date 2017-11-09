using UnityEngine;
using TMPro;

public class CurrentVersion : MonoBehaviour {

	TextMeshProUGUI versionElement;
	public static NetworkServer.ServerVersion serverVersion = new NetworkServer.ServerVersion(-99, -99, -99);

	private NetworkServer.ServerVersion lastVersion = new NetworkServer.ServerVersion(-99, -99, -99);

	void Start() {
		versionElement = GetComponent<TextMeshProUGUI>();
		versionElement.text = "v. " + NetworkServer.VERSION.ToString();
	}

	void LateUpdate() {
		if(serverVersion.ToString() != lastVersion.ToString()) {
			versionElement.text = "v. " + NetworkServer.VERSION.ToString() + " - Server: " + serverVersion.ToString();
			lastVersion = serverVersion;
		}
	}
}
