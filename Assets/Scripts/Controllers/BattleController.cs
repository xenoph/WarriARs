using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SocketIO;
using System;

public class BattleController : MonoBehaviour {

	public Camera battleCam, mainCam;
	public string battleID;

	public int BattleTimer;

	public GameObject SpawnLocationLocalChampion;
	public GameObject SpawnLocationOpponentChampion;
	public GameObject[] ChampionPrefabs;

	public float AbilityTimer;

	private Dictionary<string, string> _battleData;
	private SocketIOComponent Socket;

	private int _myHealth;
	private int _oppHealth;
	private bool _dead;

	private string _usedAbilityID;
	private int _myUsedAbility;
	private int _oppUsedAbility;
	private int _goingFirst;
	private bool _oppDead;

	private GameObject _myChampion;
	private GameObject _oppChampion;

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
		var myName = GameController.instance.InterfaceController.MyName.text;
		var oppName = GameController.instance.InterfaceController.OpponentName.text;

		_myHealth = GameController.instance.BRController.MyHealth;
		_oppHealth = GameController.instance.BRController.OppHealth;

		SpawnChampions(myName, oppName);
	}

	public void UseAbility(string id, int abNumber) {
		_myUsedAbility = abNumber;
		_usedAbilityID = id;
		GameController.instance.InterfaceController.AbilityBarAnimator.SetBool("Hide", true);
		GameController.instance.InterfaceController.NeedleBar.StartNeedle();
		_myChampion.GetComponent<ChampionAbilityController>().Ability1Effect.Play();
	}

	public void SendAbility(float percentage) {
		var json = new JSONObject();
		json.AddField("ability", _usedAbilityID);
		json.AddField("percentage", percentage);
		json.AddField("abilityNumber", _myUsedAbility);
		Socket.Emit("usedAbility", json);

		_usedAbilityID = null;
	}

	private void SpawnChampions(string myname, string oppname) {
		var myPrefab = ChampionPrefabs.Where(n => n.name == myname).FirstOrDefault();
		_myChampion = Instantiate(myPrefab, new Vector3(-5.5f, 0.5f, 1f), Quaternion.Euler(0f, 90f, 0f));
		_myAbController = _myChampion.GetComponent<ChampionAbilityController>();

		var oppPrefab = ChampionPrefabs.Where(n => n.name == oppname).FirstOrDefault();
		_oppChampion = Instantiate(oppPrefab, new Vector3(5.5f, 0.5f, 1f), Quaternion.Euler(0f, -90f, 0f));
		_oppAbController = _oppChampion.GetComponent<ChampionAbilityController>();
	}


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
		PlayAbilities();
	}

	private void PlayAbilities() {
		if(_goingFirst == 1) {
			_myAbController.PlayAbilityEffect(_myUsedAbility);
			StartCoroutine(PlayOtherAbility(false));
		} else {
			_oppAbController.PlayAbilityEffect(_oppUsedAbility);
			StartCoroutine(PlayOtherAbility(true));
		}
	}

	private IEnumerator PlayOtherAbility(bool player) {
		yield return new WaitForSeconds(AbilityTimer);
		if(player) {
			_myAbController.PlayAbilityEffect(_myUsedAbility);
		} else {
			_oppAbController.PlayAbilityEffect(_oppUsedAbility);
		}

		if(_dead || _oppDead) { EndBattle(); }
		else { SetNewRound(); }
	}

	private void EndBattle() {
		ClearUsedAbilities();
		ClearChampions();
		GameController.instance.SceneController.ToggleBattleScene("battle", "map", "Loading map....");
	}

	private void SetNewRound() {
		ClearUsedAbilities();
	}

	private void ClearUsedAbilities() {
		_usedAbilityID = null;
		//Since 0 is an ability number (0-2), we set 9 when resetting as that would never be used
		_myUsedAbility = 9;
		_oppUsedAbility = 9;
	}

	private void ClearChampions() {
		_myAbController = null;
		_oppAbController = null;
		_myHealth = 0;
		_oppHealth = 0;
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
}