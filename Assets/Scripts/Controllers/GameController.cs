using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	public static GameController instance;

	[Header("Interfaces")]
	public Warning warning;
	public LoginScreen loginScreen;
	public LoadingScreen loadingScreen;

	[Header("Game")]
	public MapInitializer mapInitializer;
	public BattleController battleController;

	[Header("Networking")]
	public NetworkServer networkServer;
	public string currentPlayerID;
    public PlayerController playerController;

    [Header("Prefabs")]
    public GameObject playerPrefab;

    [Header("Camera")]
    public CameraController cameraController;

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
        SpawnPlayer(true);
	}

    public void SpawnPlayer(bool local) {
        GameObject player = (GameObject) Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        PlayerController controller = player.GetComponent<PlayerController>();
        controller.local = local;
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
