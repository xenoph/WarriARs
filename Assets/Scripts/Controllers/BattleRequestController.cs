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

	public int MyHealth;
	public int OppHealth;

	private List<string> _abIds;
	public string RequestedPlayerID;
	public string RequestedSocketID;

	[HideInInspector]
	public string BattleID;

	[HideInInspector]
	public List<int> HealthValues;

	private string _myName;
	private string _oppName;

	private int _myChampNumber;
	private int _oppChampNumber;

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
		json.AddField("acceptedPlayerID", GameController.instance.playerController.PlayerID);
		json.AddField("acceptedSocketID", GameController.instance.playerController.SocketID);

		json.AddField("requestedPlayerID", RequestedPlayerID);
		json.AddField("requestedSocketID", RequestedSocketID);

		json.AddField("battleID", BattleID);
		Socket.Emit("acceptedRequest", json);
	}

	private void SetupSocketReceivers() {
		Socket.On("getBattleRequest", OnReceiveBattleRequest);
		Socket.On("getDeniedBattleRequest", OnReceiveDeniedBattleRequest);
		Socket.On("getBattleInfo", OnReceiveBattleInformation);
		Socket.On("getAbilityNames", OnReceiveAbilityNames);
	}

	private void OnReceiveBattleRequest(SocketIOEvent obj) {
		GameController.instance.InterfaceController.ShowReceivedRequestPanel(obj.data["sender"].str);
		RequestedPlayerID = obj.data["requestedPlayerID"].str;
		RequestedSocketID = obj.data["requestedSocketID"].str;
		BattleID = obj.data["battleID"].str;
	}

	private void OnReceiveDeniedBattleRequest(SocketIOEvent obj) {
		GameController.instance.InterfaceController.HideBattleRequestPanel();
	}

	private void OnReceiveBattleInformation(SocketIOEvent obj) {
		GameController.instance.InterfaceController.HideBattleRequestPanel();
		Debug.Log("got battle info");
		GameController.instance.AController.SwitchAudioSource ();
		BattleID = obj.data["battleID"].str;
		MyHealth = int.Parse(obj.data["myChampionHealth"].str);
		OppHealth = int.Parse(obj.data["opponentChampionHealth"].str);
		HealthValues = new List<int>() { int.Parse(obj.data["myChampionHealth"].str), 
										int.Parse(obj.data["opponentChampionHealth"].str),
										int.Parse(obj.data["myChampMaxHealth"].str),
										int.Parse(obj.data["oppChampMaxHealth"].str) };
		_abIds = new List<string>() { obj.data["ability1"].str, obj.data["ability2"].str, obj.data["ability3"].str };
		_myName = obj.data["myUsername"].str;
		_oppName = obj.data["oppUsername"].str;
		_myChampNumber = int.Parse(obj.data["myChampionType"].str);
		_oppChampNumber = int.Parse(obj.data["oppChampionType"].str);

		Debug.Log("finished saving data");

		StartCoroutine(GetAbilityNames(obj.data["battleID"].str));
	}

	private IEnumerator GetAbilityNames(string battleid) {
		Debug.Log("coroutine started");
		yield return new WaitForSeconds(1.5f);
		var json = new JSONObject();
		for(int i = 0; i < _abIds.Count; i++) {
			json.AddField("Ability" + (i + 1), _abIds[i]);
		}

		GameController.instance.InterfaceController.SetUpBattleCanvas(MyHealth, OppHealth, _myName, _oppName);
		GameController.instance.InterfaceController.MyChampionType = _myChampNumber;
		GameController.instance.InterfaceController.OpponentChampionType = _oppChampNumber;
		GameController.instance.SceneController.ToggleBattleScene("map", "battle", "Loading battle...");
		
		json.AddField("battleID", battleid);
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
