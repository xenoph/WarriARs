using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BattleController : MonoBehaviour {

	public Camera battleCam, mainCam;
	public string battleID;

	public Transform SpawnLocationLocalChampion;
	public Transform SpawnLocationOpponentChampion;

	public string LocalChampionName;
	public string OpponentChampionName;

	public int LocalChampionHealth;
	public int OpponentChampionHealth;

	public GameObject[] ChampionPrefabs;

	private Dictionary<string, string> _battleData;

	private void Start() {
		GameController.instance.battleController = this;
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		GameController.instance.loadingScreen.gameObject.SetActive(false);
		mainCam = Camera.main;
		mainCam.gameObject.SetActive(false);
		battleCam.gameObject.SetActive(true);
	}

	public void SetUpBattle(Dictionary<string, string> data) {
		_battleData = data;
		//receive own champion stats + ability ids
		//receive opponent prefabname + stats
		SpawnChampions();
	}

	private void SetInterfaceElements() {
	}

	private void SpawnChampions() {
		//Spawn in prefabs from given names
		//Give them health
	}

	private void SendAbility() {
		//send ability id
	}

	private void GetOpponentAbilityUsed(JSONObject jSON) {
		//receive ability used by opponent
	}

	private void PlayAbilities() {
		//Make prefabs play abilities in turn and show changes in UI.
	}

	private void OnDisable() {
		mainCam.gameObject.SetActive(true);
		Screen.orientation = ScreenOrientation.Portrait;
		GameController.instance.battleController = null;
	}
}