using UnityEngine;

public class BattleController : MonoBehaviour {

	public Camera battleCam, mainCam;
	public string battleID;



	private void Start() {
		GameController.instance.battleController = this;
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		GameController.instance.loadingScreen.gameObject.SetActive(false);
		mainCam = Camera.main;
		mainCam.gameObject.SetActive(false);
		battleCam.gameObject.SetActive(true);
	}

	public void SetUpBattle(JSONObject jSON) {
		//receive own champion stats + ability ids
		//receive opponent prefabname + stats
	}

	private void SpawnChampions() {

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