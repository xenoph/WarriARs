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

	private int _myUsedAbility;
	private int _oppUsedAbility;
	private int _goingFirst;
	private bool _oppDead;
	private int _myDamage;
	private int _dmgTaken;

	private GameObject _myChampion;
	private GameObject _oppChampion;

	private string _oppSocketID;
	private string _oppPlayerID;

	private ChampionAbilityController _myAbController;
	private ChampionAbilityController _oppAbController;

	private int _myMaxHealth;
	private int _oppMaxHealth;
	private int _myHealth;
	private int _oppHealth;
	private bool _dead;

	private int _coinGain;
	private int _xpGain;

	private void Start() {
		GameController.instance.battleController = this;
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
		var myChampPrefab = ConvertChampionNumberToString.GetChampionPrefab(myChampTypeNumber);
		var oppChampPrefab = ConvertChampionNumberToString.GetChampionPrefab(oppChampTypeNumber);

		_oppPlayerID = GameController.instance.BRController.RequestedPlayerID;
		_oppSocketID = GameController.instance.BRController.RequestedSocketID;

		battleID = GameController.instance.BRController.BattleID;

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
		json.AddField("ability", _myUsedAbility); //_usedAbilityID);
		json.AddField("percentage", percentage);
		Socket.Emit("usedAbility", json);
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
		SetUpChampionHealth();
		GameController.instance.InterfaceController.SetHealthBarsText(_myHealth, _oppHealth, _myMaxHealth, _oppMaxHealth);
	}

	private void RequestChampionHealth() {
		var json = CreateJSON();

		Socket.Emit("getChampionHealth", json);
	}

	/// <summary>
	/// Catches information from the server related to used abilities in a round
	/// </summary>
	/// <param name="obj"></param>
	private void OnOpponentAbilityUsed(SocketIOEvent obj) {
		_dmgTaken = int.Parse(obj.data["damage"].str);
		_myDamage = int.Parse(obj.data["myDamage"].str);
		_oppUsedAbility = int.Parse(obj.data["abilityNumber"].str);
		_goingFirst = int.Parse(obj.data["goingFirst"].str);
		if(int.Parse(obj.data["opponentDead"].str) == 0) {
			_oppDead = false;
			_myHealth -= _dmgTaken;
		} else {
			_oppDead = true;
		}
		_oppHealth -= _myDamage;
		if(_myHealth <= 0) { _dead = true; }
		PlayAbilityEffects();
	}


	/// <summary>
	/// Plays Ability effect from the Champion that goes first and checks if the other champion died from the ability
	/// </summary>
	private void PlayAbilityEffects() {
		if(_goingFirst == 1) {
			_myAbController.PlayAbilityEffect(_myUsedAbility);
			GameController.instance.InterfaceController.SetOppHealthBars (_oppHealth, _oppMaxHealth, _myDamage);
			if(_oppDead) {
				StartCoroutine(EndBattle(GameController.instance.InterfaceController.MyName.text));
			} else {
				StartCoroutine(PlayOtherAbility(false));
			}
		} else {
			_oppAbController.PlayAbilityEffect(_oppUsedAbility);
			GameController.instance.InterfaceController.SetMyHealthBars (_myHealth, _myMaxHealth, _dmgTaken);
			if(_dead) {
				StartCoroutine(EndBattle(GameController.instance.InterfaceController.OpponentName.text));
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
			GameController.instance.InterfaceController.SetOppHealthBars (_oppHealth, _oppMaxHealth, _myDamage);
		} else {
			_oppAbController.PlayAbilityEffect(_oppUsedAbility);
			GameController.instance.InterfaceController.SetMyHealthBars (_myHealth, _myMaxHealth, _dmgTaken);
		}

		yield return new WaitForSeconds(AbilityTimer);
		if(_dead) { StartCoroutine(EndBattle(GameController.instance.InterfaceController.OpponentName.text)); }
		else if(_oppDead) { StartCoroutine(EndBattle(GameController.instance.InterfaceController.MyName.text)); }
		else { SetNewRound(); }
	}

	private IEnumerator EndBattle(string winner) {
		GameController.instance.AController.SwitchAudioSource ();
		if(winner != null) {
			GameController.instance.InterfaceController.WinningText.text = winner + " WON!";
			yield return new WaitForSeconds(3f);
			GameController.instance.InterfaceController.GainText.text = "You gain: " + _xpGain + " experience & " + _coinGain + " coins!";
		}
		yield return new WaitForSeconds(6f);
		ClearUsedAbilities();
		ClearChampions();
		GameController.instance.InterfaceController.WinningText.text = "";
		GameController.instance.InterfaceController.GainText.text = "";
		GameController.instance.SceneController.ToggleBattleScene("battle", "map", "Loading map....");
	}

	private void SetNewRound() {
		ClearUsedAbilities();
	}

	private void UsedAbility(int ab) {
		_myUsedAbility = ab;
		GameController.instance.InterfaceController.ToggleAbilityButtons();
		GameController.instance.InterfaceController.AbilityBarAnimator.SetBool("Hide", true);
		GameController.instance.InterfaceController.NeedleBar.StartNeedle();
	}

	private void ClearUsedAbilities() {
		//Since 0 is an ability number (0-2), we set 9 when resetting as that would never be used
		_myUsedAbility = 9;
		_oppUsedAbility = 9;
		GameController.instance.InterfaceController.ToggleAbilityButtons();
	}

	private void ClearChampions() {
		_myAbController = null;
		_oppAbController = null;
		_myHealth = 0;
		_oppHealth = 0;

		Destroy(_myChampion);
		Destroy(_oppChampion);
		_myChampion = null;
		_oppChampion = null;
	}

	private void TimedOut(SocketIOEvent obj) {
		GameController.instance.InterfaceController.ToggleAbilityButtons();
	}

	private void BattleQuit(SocketIOEvent obj) {
		StartCoroutine(EndBattle(null));
	}

	private void SetUpSocketConnections() {
		Socket.On("usedAbility", OnOpponentAbilityUsed);
		Socket.On("timedOut", TimedOut);
		Socket.On("battleQuit", BattleQuit);
		Socket.On("levelUp", OnGetLevelupInfo);
		Socket.On("xpGain", OnEndMatchGain);
	}

	private void OnEndMatchGain(SocketIOEvent obj) {
		_coinGain = int.Parse(obj.data["coinGain"].str);
		GameController.instance.InterfaceController.RequestCoinCount();
		_xpGain = int.Parse(obj.data["xpGain"].str);
	}

	private void SetUpChampionHealth() {
		_myHealth = GameController.instance.BRController.HealthValues[0];
		_oppHealth = GameController.instance.BRController.HealthValues[1];

		_myMaxHealth = GameController.instance.BRController.HealthValues[2];
		_oppMaxHealth = GameController.instance.BRController.HealthValues[3];
	}

	private void OnGetLevelupInfo(SocketIOEvent obj) {
		GameController.instance.InterfaceController.LevelledUp = true;
		var luController = GameController.instance.InterfaceController.LevelUpCanvas.GetComponent<LevelUpController>();
		luController.SetTopPanel(int.Parse(obj.data["currentXP"].str), int.Parse(obj.data["nextLevelXP"].str), obj.data["dragonName"].str);
		luController.SetMidPanel(int.Parse(obj.data["dragonLevel"].str));
		luController.SetBottomPanel(obj.data["dragonAttack"].f, obj.data["dragonAttackGain"].f, 
									obj.data["dragonDefense"].f, obj.data["dragonDefenseGain"].f,
									obj.data["dragonHealth"].f, obj.data["dragonHealthGain"].f,
									obj.data["dragonSpeed"].f, obj.data["dragonSpeedGain"].f);
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

		return json;
	}
}