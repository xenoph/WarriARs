﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class NetworkServer : MonoBehaviour {
	static SocketIOComponent socket;

	public static ServerVersion VERSION = new ServerVersion(0, 3, 0);
	public static long PING = 0;

	public static string SERVER_SEED { get; private set; }
	public static string USERNAME { get; private set; }

	private Dictionary<string, PlayerController> otherPlayers = new Dictionary<string, PlayerController>();

	void Start() {
		//InvokeRepeating("UpdatePlayersOnline", 0f, 10f);
		GameController.instance.networkServer = this;
		socket = GetComponent<SocketIOComponent>();
		socket.On("init", OnInit);
		socket.On("loggedin", OnLoggedIn);
		socket.On("failedLogin", OnFailedLogin);
		socket.On("login", OnLogin);
		socket.On("move", OnMove);
		socket.On("quit", OnQuit);
		socket.On("game_pong", OnPong);
	}

	/*
	private void UpdatePlayersOnline() {
		if(playersOnline.IsActive()) {
			string match = Regex.Match(socket.url, @"ws(s?)://(.+?)/socket").Groups[2].Value;
			StartCoroutine(PlayersOnline(match));
		}
	}

	struct res { public string online; }
	private IEnumerator PlayersOnline(string url) {
		WWW www = new WWW("https://" + url + "");
		yield return www;
		res r = new res();
		r = JsonUtility.FromJson<res>(www.text);
		int amount = -1;
		int.TryParse(r.online, out amount);
		playersOnline.text = "Players Online: " + amount.ToString("n0");
	}
	*/

	public void UnloadOthers() {
		otherPlayers.Clear();
	}

	public void Disconnect() {
		socket.Close();
	}

	public void SetUID(string uid) {
		GameController.instance.currentPlayerID = uid;
	}

	public void SetUsername(string username) {
		USERNAME = username;
	}

	public void Connect() {
		/*if(string.IsNullOrEmpty(GameController.instance.currentPlayerID))
			SetUID(SystemInfo.deviceUniqueIdentifier);*/
		StartCoroutine(ConnectToServer());
	}

	private IEnumerator ConnectToServer() {
		while(!GameController.instance.currentLocation.IsActive)
			yield return null;
		socket.Connect();
	}

	private void OnFailedLogin(SocketIOEvent obj) {
		Debug.LogError(obj.data);
	}

	private void OnInit(SocketIOEvent obj) {
		Debug.Log(obj.data);
		string[] v = obj.data["version"].str.Split(".".ToCharArray()[0]);
		int major, minor, hotfix;
		int.TryParse(v[0], out major);
		int.TryParse(v[1], out minor);
		int.TryParse(v[2], out hotfix);
		CurrentVersion.serverVersion = new ServerVersion(major, minor, hotfix);
		if(major == VERSION.major && minor == VERSION.minor) {
			if(hotfix > VERSION.hotfix)
				Debug.LogWarning("The server is running a newer hotfix. This should be fine.");
			if(hotfix < VERSION.hotfix)
				Debug.LogWarning("The server is running an older hotfix. This might cause problems, but should work fine.");
			SERVER_SEED = obj.data["seed"].str;
			JSONObject json = new JSONObject();
			json.AddField("id", GameController.instance.currentPlayerID);
			json.AddField("lat", (float) GameController.instance.currentLocation.Latitude);
			json.AddField("lng", (float) GameController.instance.currentLocation.Longitude);
			socket.Emit("register", json);
			Ping();
		} else {
			Debug.LogError("Your version is not compatible with the server version. Try updating the game.");
			socket.Close();
		}
	}

	private void OnLoggedIn(SocketIOEvent obj) {
		//interfaceManager.LoadPlayer(obj);
		PlayerController pc = GameController.instance.LoadGame(obj.data["username"].str);
		pc.PlayerID = obj.data["id"].str;
		pc.SocketID = obj.data["socket"].str;
	}

	public void Move(float lat, float lng, int dist) {
		JSONObject json = new JSONObject();
		json.AddField("id", GameController.instance.currentPlayerID);
		json.AddField("lat", lat);
		json.AddField("lng", lng);
		json.AddField("distance", dist);
		socket.Emit("updatePosition", json);
	}

	private void OnLogin(SocketIOEvent obj) {
		if(!otherPlayers.ContainsKey(obj.data["id"].str)) {
			PlayerController other = GameController.instance.SpawnPlayer(false, obj.data["username"].str);
			other.PlayerID = obj.data["id"].str;
			other.SocketID = obj.data["socket"].str;

			if(obj.data["ai"].str == "true") {
				other.gameObject.AddComponent<AIController>();
			}

			Location newLoc = new Location();
			double lat, lng;
			double.TryParse(obj.data["lat"].str, out lat);
			double.TryParse(obj.data["lng"].str, out lng);
			newLoc.SetLocation(lat, lng);
			other.targetPosition = ConvertPositions.ConvertLocationToVector3(newLoc, GameController.instance.mapInitializer.map);
			newLoc = null;

			otherPlayers.Add(obj.data["id"].str, other);
		}
	}

	private void OnQuit(SocketIOEvent obj) {
		if(otherPlayers.ContainsKey(obj.data["id"].str)) {
			PlayerController other;
			otherPlayers.TryGetValue(obj.data["id"].str, out other);
			otherPlayers.Remove(obj.data["id"].str);
			Destroy(other.gameObject);
		} else {
			Debug.LogWarning("Player " + obj.data["username"].str + " not found. Could not delete them.");
		}
	}

	private void OnPong(SocketIOEvent obj) {
		long currentTime = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
		long sentTime = long.Parse(obj.data["time"].str);
		PING = (currentTime - sentTime);
		Invoke("Ping", .25f);
	}

	private void Ping() {
		JSONObject json = new JSONObject();
		json.AddField("time", (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond).ToString());
		socket.Emit("game_ping", json);
	}

	private void OnMove(SocketIOEvent obj) {
		if(otherPlayers.ContainsKey(obj.data["id"].str)) {
			PlayerController other;
			otherPlayers.TryGetValue(obj.data["id"].str, out other);
			Location newLoc = new Location();
			double lat, lng;
			double.TryParse(obj.data["lat"].str, out lat);
			double.TryParse(obj.data["lng"].str, out lng);
			newLoc.SetLocation(lat, lng);
			other.targetPosition = ConvertPositions.ConvertLocationToVector3(newLoc, GameController.instance.mapInitializer.map);
			newLoc = null;
		} else {
			OnLogin(obj);
			Debug.LogWarning("Player " + obj.data["username"].str + " not found. Adding them.");
		}
	}

	 public static string Sha1Sum2(string str) {
        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
        byte[] bytes = encoding.GetBytes(str);
        var sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
        return System.BitConverter.ToString(sha.ComputeHash(bytes));
    }

	public struct ServerVersion {
		public int major;
		public int minor;
		public int hotfix;
		public ServerVersion(int major, int minor, int hotfix) {
			this.major = major;
			this.minor = minor;
			this.hotfix = hotfix;
		}
		public override string ToString() {
			return major + "." + minor + "." + hotfix;
		}
	}
}