using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour {
	public string id;

	public float lat, lng;

	public void Collect() {
		JSONObject json = new JSONObject();
		json.AddField("id", this.id);
		json.AddField("lat", this.lat);
		json.AddField("lng", this.lng);
		GameController.instance.Socket.Emit("collectCoin", json);
		Destroy(gameObject);
	}
}
