using System.Collections;
using UnityEngine;
using SocketIO;

public class NetworkServer : MonoBehaviour {
	static SocketIOComponent socket;

	public static string SERVER_SEED { get; private set; }
	public static string USERNAME { get; private set; }

	//public OtherPlayersManager otherPlayersManager;

	void Start() {
		//InvokeRepeating("UpdatePlayersOnline", 0f, 10f);
		GameController.instance.networkServer = this;
		socket = GetComponent<SocketIOComponent>();
		socket.On("init", OnInit);
		socket.On("loggedin", OnLoggedIn);
		socket.On("failedLogin", OnFailedLogin);
		//socket.On("login", OnLogin);
		//socket.On("move", OnMove);
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

	public void SetUID(string uid) {
		GameController.instance.currentPlayerID = uid;
	}

	public void SetUsername(string username) {
		USERNAME = username;
	}

	public void Connect() {
		if(string.IsNullOrEmpty(GameController.instance.currentPlayerID))
			GameController.instance.currentPlayerID = SystemInfo.deviceUniqueIdentifier;
		StartCoroutine(ConnectToServer());
	}

	private IEnumerator ConnectToServer() {
		while(!GameController.instance.currentLocation.IsActive)
			yield return null;
		socket.Connect();
	}

	private void OnFailedLogin(SocketIOEvent obj) {
		Debug.LogError(obj.data.str);
	}

	private void OnInit(SocketIOEvent obj) {
		Debug.Log(obj.data);
		SERVER_SEED = obj.data["seed"].str;
		JSONObject json = new JSONObject();
		json.AddField("id", GameController.instance.currentPlayerID);
		json.AddField("lat", (float) GameController.instance.currentLocation.Latitude);
		json.AddField("lng", (float) GameController.instance.currentLocation.Longitude);
		socket.Emit("register", json);
	}

	private void OnLoggedIn(SocketIOEvent obj) {
		//interfaceManager.LoadPlayer(obj);
		GameController.instance.LoadGame();
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
		/*if(otherPlayersManager != null) {
			otherPlayersManager.NewLogin(obj);
		}*/
	}

	private void OnMove(SocketIOEvent obj) {
		/*if(otherPlayersManager != null) {
			otherPlayersManager.UpdatePosition(obj);
		}*/
	}
}