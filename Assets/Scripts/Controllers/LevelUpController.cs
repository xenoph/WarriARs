using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class LevelUpController : MonoBehaviour {

	public GameObject ChampionSpawnParent;

	private GameObject _spawnedChampion;
	
	private void SpawnChampion(string champName) {
		_spawnedChampion = Instantiate(Resources.Load(champName, typeof(GameObject)), ChampionSpawnParent.transform.position, Quaternion.identity, ChampionSpawnParent.transform) as GameObject;
	}

	private void OnGetChampion(SocketIOEvent obj) {
		var champType = ConvertChampionNumberToString.GetChampionPrefab(int.Parse(obj.data["champTypeNumber"].str));
		SpawnChampion(champType);
	}

	private void RequestChampion() {
		var json = new JSONObject();
		json.AddField("playerID", GameController.instance.playerController.PlayerID);
		json.AddField("socketID", GameController.instance.playerController.SocketID);
		GameController.instance.Socket.Emit("getChampionData", json);
	}

	private void OnEnable() {
		RequestChampion();
	}

	private void OnDisable() {
		Destroy(_spawnedChampion);
		_spawnedChampion = null;
	}
}