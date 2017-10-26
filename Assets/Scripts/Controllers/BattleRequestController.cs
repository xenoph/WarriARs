﻿using System.Collections;
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

	public int MyHealth;
	public int OppHealth;

	private List<string> _abIds;
	private string _playerRequested;

	private void Start() {
		InterfaceController = GameController.instance.InterfaceController;
		SetupSocketReceivers();
	}

	public void SendBattleRequest(string socketID, string playerID) {
		var json = new JSONObject();
		json.AddField("requestedPlayerID", GameController.instance.playerController.PlayerID);
		json.AddField("requestedSocketID", GameController.instance.playerController.SocketID);

		json.AddField("playerReceivingRequest", socketID);
		Socket.Emit("BattleRequest", json);
	}

	public void InitialiseAIBattle(string socketID, string playerID) {
		var json = new JSONObject();
		json.AddField("requestedPlayerID", GameController.instance.playerController.PlayerID);
		json.AddField("requestedSocketID", GameController.instance.playerController.SocketID);

		json.AddField("aiID", socketID);
		Socket.Emit("AIBattle", json);
	}

	public void AcceptRequest() {
		var json = new JSONObject();
		json.AddField("playerAccepted", GameController.instance.playerController.PlayerID);
		json.AddField("playerRequested", _playerRequested);
		Socket.Emit("acceptedRequest", json);
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
		_playerRequested = obj.data["playerRequested"].str;
	}

	private void OnReceiveBattleInformation(SocketIOEvent obj) {
		MyHealth = int.Parse(obj.data["myChampionHealth"].str);
		OppHealth = int.Parse(obj.data["opponentChampionHealth"].str);
		GameController.instance.InterfaceController.SetUpBattleCanvas(MyHealth, OppHealth, obj.data["myChampionName"].str, obj.data["opponentChampionName"].str);

		GameController.instance.SceneController.ToggleBattleScene("map", "battle", "Loading battle...");

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
		for(int i = 0; i < obj.data.Count; i++) {
			abNames.Add(obj.data["ability" + (i + 1)].str);
		}

		GameController.instance.InterfaceController.SetAbilityButtonNames(abNames);
		GameController.instance.InterfaceController.SetAbilityButtonDelegates(_abIds);

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