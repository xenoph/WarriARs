using UnityEngine;
using UnityEngine.SceneManagement;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class MapInitializer : MonoBehaviour {
	[Range(1, 100)]
	public int mapBounds = 10;
	public AbstractMap map;
	public CameraBoundsTileProvider tileProvider;

	public double startLat, startLng;

	[HideInInspector]
	public bool inPosition = false;

	void Start() {
		GameController.instance.mapInitializer = this;
        tileProvider.SetCamera(Camera.main);
		GameController.instance.SetupMap();
		map.Initialize(new Vector2d(startLat, startLng), map.Zoom);
		//Screen.orientation = ScreenOrientation.Portrait;
	}

	void LateUpdate() {
		if(inPosition && isPlayerOutsideMapBounds()) {
			Debug.Log("Reloading map");
			GameController.instance.playerController.targetPosition = Vector3.zero;
			SceneManager.UnloadSceneAsync("map");
			GameController.instance.loadingScreen.LoadingText.text = "Loading, please wait...";
			SceneManager.LoadSceneAsync("map", LoadSceneMode.Additive);
		}
		if(inPosition)
			return;
		if(Vector3.Distance(GameController.instance.playerController.transform.position, GameController.instance.playerController.targetPosition) <= 0.1f) {
			GameController.instance.loadingScreen.gameObject.SetActive(false);
			inPosition = true;
		}
	}

	private bool isPlayerOutsideMapBounds() {
		int positive = mapBounds * 1000;
		int negative = positive * -1;
		return (GameController.instance.playerController.transform.position.x >= positive
				|| GameController.instance.playerController.transform.position.z >= positive
				|| GameController.instance.playerController.transform.position.x <= negative
				|| GameController.instance.playerController.transform.position.z <= negative);
	}
}
