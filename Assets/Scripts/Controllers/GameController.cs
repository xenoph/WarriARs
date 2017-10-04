using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	public static GameController instance;

	[Header("Interfaces")]
	public Warning warning;
	public LoginScreen loginScreen;

	[Header("Map")]
	public MapInitializer mapInitializer;

	[Header("Networking")]
	public NetworkServer networkServer;
	
	public string currentPlayerID;

	public Location currentLocation;

	void Awake() {
		instance = this;
		SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
		currentLocation = new Location();
		#if UNITY_EDITOR
			currentLocation.Latitude = 64.0186d;
			currentLocation.Longitude = 11.4938d;
			currentLocation.IsActive = true;
		#else
			StartCoroutine(currentLocation.StartLocationServices());
		#endif
	}

	public void LoadGame() {
		SceneManager.LoadSceneAsync("map", LoadSceneMode.Additive);
		loginScreen.gameObject.SetActive(false);
	}

	public void SetupMap() {
		if(!currentLocation.Failed && currentLocation.IsActive) {
			mapInitializer.startLat = currentLocation.Latitude;
			mapInitializer.startLng = currentLocation.Longitude;
		} else {
			Debug.LogError("No start location found.");
		}
	}

	void Start() {
		
	}

	void LateUpdate() {
		if(warning != null) {
			warning.showing = currentLocation.Failed || !currentLocation.IsActive;
			warning.text.text = "Failed to update your location.\nCheck if your Location Service is enabled.";
		}
	}
}
