using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;

public class MapInitializer : MonoBehaviour {

	public AbstractMap map;
	public AbstractTileProvider tileProvider;

	public double startLat, startLng;

	void Start() {
		GameController.instance.mapInitializer = this;
		//tileProvider.Camera = Camera.main;
		GameController.instance.SetupMap();
		map.Initialize(new Vector2d(startLat, startLng), map.Zoom);
	}
}
