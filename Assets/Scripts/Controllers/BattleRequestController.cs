﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class BattleRequestController : MonoBehaviour {

	public SocketIOComponent Socket;
	[HideInInspector]
	public UserInterfaceController InterfaceController;

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
	}

	private void OnReceiveBattleRequest(SocketIOEvent obj) {

	}

	private void OnReceiveBattleInformation(SocketIOEvent obj) {
		GameController.instance.SceneController.ToggleBattleScene("map", "battle", "Loading battle...");

		var myHealth = int.Parse(obj.data["myChampionHealth"].str);
		var oppHealth = int.Parse(obj.data["opponentChampionHealth"].str);
		InterfaceController.SetUpBattleCanvas(myHealth, oppHealth, obj.data["myChampionName"].str, obj.data["opponentChampionName"].str);

		string[] abIds = new string[] { obj.data["ability1"].str, obj.data["ability2"].str, obj.data["ability3"].str };
		GetAbilityNames(abIds);
	}

	private void GetAbilityNames(string[] abIds) {
		var json = new JSONObject();
		for(int i = 0; i < abIds.Length; i++) {
			json.AddField("Ability" + (i + 1), abIds[i]);
		}

		Socket.Emit("GetAbilityNames", json);
	}
}