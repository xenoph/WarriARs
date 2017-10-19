using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class BattleRequestController : MonoBehaviour {

	public SocketIOComponent Socket;

	public int RequestTimer;

	[HideInInspector]
	public UserInterfaceController InterfaceController;
	[HideInInspector]
	public bool AwaitRequestFeedback = false;

	private List<string> _abIds;

	private void Start() {
		InterfaceController = GameController.instance.InterfaceController;
		SetupSocketReceivers();
	}

	public void SendBattleRequest(string id) {
		var json = new JSONObject();
		json.AddField("playerRequested", GameController.instance.playerController.PlayerID);
		json.AddField("playerReceivingRequest", id);
		Socket.Emit("BattleRequest", json);
	}

	public void InitialiseAIBattle(string id) {
		var json = new JSONObject();
		json.AddField("playerRequested", GameController.instance.playerController.PlayerID);
		json.AddField("aiID", id);
		Socket.Emit("AIBattle", json);
	}

	public void SetUserInterface() {

	}

	private void SetupSocketReceivers() {
		Socket.On("getBattleRequest", OnReceiveBattleRequest);
		Socket.On("getBattleInfo", OnReceiveBattleInformation);
		Socket.On("getAbilityNames", OnReceiveAbilityNames);
	}

	private void OnReceiveBattleRequest(SocketIOEvent obj) {
		GameController.instance.InterfaceController.ShowReceivedRequestPanel(obj.data["sender"].str);
	}

	private void OnReceiveBattleInformation(SocketIOEvent obj) {
		for(int i = 0; i < obj.data.Count; i++) {
			Debug.Log(obj.data[i].str);
		}

		var myHealth = int.Parse(obj.data["myChampionHealth"].str);
		var oppHealth = int.Parse(obj.data["opponentChampionHealth"].str);
		GameController.instance.InterfaceController.SetUpBattleCanvas(myHealth, oppHealth, obj.data["myChampionName"].str, obj.data["opponentChampionName"].str);

		GameController.instance.SceneController.ToggleBattleScene("map", "battle", "Loading battle...");
		Debug.Log("battle loaded?");

		_abIds = new List<string>() { obj.data["ability1"].str, obj.data["ability2"].str, obj.data["ability3"].str };
		GetAbilityNames();
	}

	private void GetAbilityNames() {
		var json = new JSONObject();
		for(int i = 0; i < _abIds.Count; i++) {
			json.AddField("Ability" + (i + 1), _abIds[i]);
		}

		Socket.Emit("GetAbilityNames", json);
	}

	private void OnReceiveAbilityNames(SocketIOEvent obj) {
		var abNames = new List<string>();
		for(int i = 0; i < _abIds.Count; i++) {
			abNames.Add(obj.data[_abIds[i]].str);
		}

		InterfaceController.SetAbilityButtonNames(abNames);
		InterfaceController.SetAbilityButtonDelegates(_abIds);

		_abIds = null;
	}

	public IEnumerator BattleRequestTimer() {
		int timer = 0;
		while(timer <= RequestTimer && AwaitRequestFeedback) {
			timer++;
			if(timer == RequestTimer) {
				StopRequest();
				GameController.instance.PlayerBusy = false;
			}
			yield return new WaitForSeconds(1);
		}
	}

	public void StopRequest() {
		AwaitRequestFeedback = false;
		GameController.instance.InterfaceController.HideBattleRequestPanel();
	}
}