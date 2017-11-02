using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SocketIO;
using System;

public class BattleController : MonoBehaviour {

	public Camera battleCam, mainCam;
	public string battleID;

	public GameObject SpawnLocationLocalChampion;
	public GameObject SpawnLocationOpponentChampion;
	public GameObject[] ChampionPrefabs;

	public float AbilityTimer;

	private Dictionary<string, string> _battleData;
	private SocketIOComponent Socket;

	private int _myHealth;
	//private int _oppHealth;
	private bool _dead;

	private string _usedAbilityID;
	private int _myUsedAbility;
	private int _oppUsedAbility;
	private int _goingFirst;
	private bool _oppDead;

	private GameObject _myChampion;
	private GameObject _oppChampion;

	private string _oppSocketID;
	private string _oppPlayerID;

	private ChampionAbilityController _myAbController;
	private ChampionAbilityController _oppAbController;

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

	public void SetUpBattle() {
		var myChampTypeNumber = GameController.instance.InterfaceController.MyChampionType;
		var oppChampTypeNumber = GameController.instance.InterfaceController.OpponentChampionType;
		var myChampPrefab = GetChampionPrefab(myChampTypeNumber);
		var oppChampPrefab = GetChampionPrefab(oppChampTypeNumber);

		_myHealth = GameController.instance.BRController.MyHealth;
		//_oppHealth = GameController.instance.BRController.OppHealth;

		_oppPlayerID = GameController.instance.BRController.RequestedPlayerID;
		_oppSocketID = GameController.instance.BRController.RequestedSocketID;

		GameController.instance.InterfaceController.SetTypeSprites();
		SpawnChampions(myChampPrefab, oppChampPrefab);
	}

	public void UseAbility1() {
		UsedAbility(0);
	}

	public void UseAbility2() {
		UsedAbility(1);
	}

	public void UseAbility3() {
		UsedAbility(2);
	}

	/// <summary>
	/// Sends the Ability ID and number (for effects on clients) + percentage of needle hit, to the server.
	/// </summary>
	/// <param name="percentage"></param>
	public void SendAbility(float percentage) {
		var json = CreateJSON();
		json.AddField("ability", _usedAbilityID);
		json.AddField("percentage", percentage);
		Socket.Emit("usedAbility", json);

		_usedAbilityID = null;
	}

	/// <summary>
	/// Spawns Champions into the battle scene, then gets and stores their Ability Controllers
	/// </summary>
	/// <param name="myname">Name of Local Champion.</param>
	/// <param name="oppname">Name of opponent Champion.</param>
	private void SpawnChampions(string myname, string oppname) {
		var myPrefab = ChampionPrefabs.Where(n => n.name == myname).FirstOrDefault();
		_myChampion = Instantiate(myPrefab, new Vector3(-5.5f, 0.5f, 1f), Quaternion.Euler(0f, 90f, 0f));
		_myAbController = _myChampion.GetComponent<ChampionAbilityController>();

		var oppPrefab = ChampionPrefabs.Where(n => n.name == oppname).FirstOrDefault();
		_oppChampion = Instantiate(oppPrefab, new Vector3(5.5f, 0.5f, 1f), Quaternion.Euler(0f, -90f, 0f));
		_oppAbController = _oppChampion.GetComponent<ChampionAbilityController>();
	}

	/// <summary>
	/// Catches information from the server related to used abilities in a round
	/// </summary>
	/// <param name="obj"></param>
	private void OnOpponentAbilityUsed(SocketIOEvent obj) {
		var dmgTaken = int.Parse(obj.data["damage"].str);
		_oppUsedAbility = int.Parse(obj.data["abilityNumber"].str);
		_goingFirst = int.Parse(obj.data["goingFirst"].str);
		if(int.Parse(obj.data["opponentDead"].str) == 0) {
			_oppDead = false;
			_myHealth -= dmgTaken;
		} else {
			_oppDead = true;
		}
		if(_myHealth <= 0) { _dead = true; }
		PlayAbilityEffects();
	}

	/// <summary>
	/// Plays Ability effect from the Champion that goes first and checks if the other champion died from the ability
	/// </summary>
	private void PlayAbilityEffects() {
		if(_goingFirst == 1) {
			_myAbController.PlayAbilityEffect(_myUsedAbility);
			if(_oppDead) {
				StartCoroutine(EndBattle());
			} else {
				StartCoroutine(PlayOtherAbility(false));
			}
		} else {
			_oppAbController.PlayAbilityEffect(_oppUsedAbility);
			if(_dead) {
				StartCoroutine(EndBattle());
			} else {
				StartCoroutine(PlayOtherAbility(true));
			}
		}
	}

	/// <summary>
	/// Plays the Ability effect from the second Champion after a set amount of time - if it's not dead
	/// </summary>
	/// <param name="player"></param>
	/// <returns></returns>
	private IEnumerator PlayOtherAbility(bool player) {
		yield return new WaitForSeconds(AbilityTimer);
		if(player) {
			_myAbController.PlayAbilityEffect(_myUsedAbility);
		} else {
			_oppAbController.PlayAbilityEffect(_oppUsedAbility);
		}

		if(_dead || _oppDead) { StartCoroutine(EndBattle()); }
		else { SetNewRound(); }
	}

	private IEnumerator EndBattle() {
		ClearUsedAbilities();
		yield return new WaitForSeconds(2f);
		ClearChampions();
		GameController.instance.SceneController.ToggleBattleScene("battle", "map", "Loading map....");
	}

	private void SetNewRound() {
		ClearUsedAbilities();
	}

	private void UsedAbility(int ab) {
		GameController.instance.InterfaceController.ToggleAbilityButtons();
		_usedAbilityID = GameController.instance.InterfaceController.AbilityIDs[ab];
		GameController.instance.InterfaceController.AbilityBarAnimator.SetBool("Hide", true);
		GameController.instance.InterfaceController.NeedleBar.StartNeedle();
	}

	private void ClearUsedAbilities() {
		_usedAbilityID = null;
		//Since 0 is an ability number (0-2), we set 9 when resetting as that would never be used
		_myUsedAbility = 9;
		_oppUsedAbility = 9;
		GameController.instance.InterfaceController.ToggleAbilityButtons();
	}

	private void ClearChampions() {
		_myAbController = null;
		_oppAbController = null;
		_myHealth = 0;
		//_oppHealth = 0;

		Destroy(_myChampion);
		Destroy(_oppChampion);
		_myChampion = null;
		_oppChampion = null;
	}

	private void TimedOut(SocketIOEvent obj) {
		GameController.instance.InterfaceController.ToggleAbilityButtons();
	}

	private void SetUpSocketConnections() {
		Socket.On("usedAbility", OnOpponentAbilityUsed);
		Socket.On("timedOut", TimedOut);
	}

	private void OnDisable() {
		mainCam.gameObject.SetActive(true);
		GameController.instance.battleController = null;
	}

	private JSONObject CreateJSON() {
		var json = new JSONObject();
		json.AddField("sendingPlayerID", GameController.instance.playerController.PlayerID);
		json.AddField("sendingSocketID", GameController.instance.playerController.SocketID);
		json.AddField("battleID", battleID);
		//json.AddField("receivingPlayerID", _oppPlayerID);
		//json.AddField("receivingSocketID", _oppSocketID);

		return json;
	}

	private string GetChampionPrefab(int num) {
		switch (num) {
			case 0:
				return "PRE_Champion_Fire";

			case 1:
				return "PRE_Champion_Water";

			case 2:
				return "PRE_Champion_Wood";

			case 3:
				return "PRE_Champion_Earth";

			case 4:
				return "PRE_Champion_Metal";

			default:
				return null;
		}
	}
}