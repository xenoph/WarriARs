using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class BattleRequestController : MonoBehaviour {

	public SocketIOComponent Socket;

	private void Awake() {
		Socket = GetComponent<SocketIOComponent>();
	}

	public void SendBattleRequest() {

	}

	private void SetupSocketReceivers() {
		Socket.On("getBattleRequest", OnReceiveBattleRequest);
		Socket.On("getBattleInfo", OnReceiveBattleInformation);
	}

	private void OnReceiveBattleRequest(SocketIOEvent obj) {

	}

	private void OnReceiveBattleInformation(SocketIOEvent obj) {
		var json = new JSONObject();
		for(int i = 0; i < obj.data.Count; i++) {
			json.AddField(obj.data[i].str, obj.data[i].str);
		}
		GameController.instance.StartBattle(json);
	}
}