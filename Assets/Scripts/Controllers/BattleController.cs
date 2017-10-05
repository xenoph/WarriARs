using UnityEngine;

public class BattleController : MonoBehaviour {

	void Start() {
		GameController.instance.battleController = this;
		Screen.orientation = ScreenOrientation.LandscapeLeft;
	}
	
	void Update() {
		
	}

	void OnDisable() {
		Screen.orientation = ScreenOrientation.Portrait;
		GameController.instance.battleController = null;
	}
}
