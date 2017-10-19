using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SocketIO;

public class BattleController : MonoBehaviour {

	public Camera battleCam, mainCam;
	public string battleID;

	public int BattleTimer;

	public GameObject SpawnLocationLocalChampion;
	public GameObject SpawnLocationOpponentChampion;

	public GameObject[] ChampionPrefabs;

	private Dictionary<string, string> _battleData;
	private SocketIOComponent Socket;

	private int _myHealth;
	private int _oppHealth;
	private bool _dead;

	private void Start() {
		GameController.instance.battleController = this;
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		GameController.instance.loadingScreen.gameObject.SetActive(false);
		mainCam = Camera.main;
		mainCam.gameObject.SetActive(false);
		battleCam.gameObject.SetActive(true);

		Socket = GameController.instance.BRController.Socket;
		SetUpSocketConnections();
	}

	public IEnumerator RunBattleTimer() {
		var timer = 0;
		while(timer < BattleTimer) {
			timer++;
			yield return new WaitForSeconds(1);
		}
	}

	public void SetUpBattle() {
		var myName = GameController.instance.InterfaceController.MyName.text;
		var oppName = GameController.instance.InterfaceController.OpponentName.text;

		_myHealth = GameController.instance.BRController.MyHealth;
		_oppHealth = GameController.instance.BRController.OppHealth;

		SpawnChampions(myName, oppName);
		StartCoroutine(RunBattleTimer());
	}

	private void SpawnChampions(string myname, string oppname) {
		var myPrefab = ChampionPrefabs.Where(n => n.name == myname).FirstOrDefault();
		var myChamp = Instantiate(myPrefab, new Vector3(-5.5f, 0.5f, 1f), Quaternion.identity);

		var oppPrefab = ChampionPrefabs.Where(n => n.name == oppname).FirstOrDefault();
		var oppChamp = Instantiate(oppPrefab, new Vector3(5.5f, 0.5f, 1f), Quaternion.identity);
	}

	public void SendAbility(string id) {
		var json = new JSONObject();
		json.AddField("ability", id);
		Socket.Emit("usedAbility", json);
		StopCoroutine(RunBattleTimer());
	}

	private void OnOpponentAbilityUsed(SocketIOEvent obj) {
		var dmgTaken = int.Parse(obj.data["damage"].str);
		_myHealth -= dmgTaken;
		if(_myHealth <= 0) { _dead = true; }
		//PlayAbilities(obj.data["ability"].str);
	}

	private void PlayAbilities(string abilityName) {
		//Make prefabs play abilities in turn and show changes in UI.
	}

	private void SetUpSocketConnections() {
		Socket.On("usedAbility", OnOpponentAbilityUsed);
	}

	private void OnDisable() {
		mainCam.gameObject.SetActive(true);
		GameController.instance.battleController = null;
	}
}