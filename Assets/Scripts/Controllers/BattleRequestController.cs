using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class BattleRequestController : MonoBehaviour {

	public SocketIOComponent Socket;
	public UserInterfaceController InterfaceController;
	public SceneController SceneController;

	private void Awake() {
		Socket = GetComponent<SocketIOComponent>();
	}

	public void SendBattleRequest(int id) {
		var json = new JSONObject();
		json.AddField("playerRequested", GameController.instance.playerController.PlayerID);
		json.AddField("playerReceivingRequest", id);
		Socket.Emit("BattleRequest", json);
	}

	public void InitialiseAIBattle() {

	}

	public void SetUserInterface() {

	}

	private void SetupSocketReceivers() {
		Socket.On("getBattleRequest", OnReceiveBattleRequest);
		Socket.On("getBattleInfo", OnReceiveBattleInformation);
	}

	private void OnReceiveBattleRequest(SocketIOEvent obj) {

	}

	private void OnReceiveBattleInformation(SocketIOEvent obj) {
		SceneController.ToggleBattleScene("map", "battle", "Loading battle...");
		//Extract needed data to set up UI
		//InterfaceController.SetUpBattleCanvas();
	}
}