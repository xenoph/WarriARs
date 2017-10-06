using UnityEngine;

public class BattleController : MonoBehaviour {

	public Camera battleCam, mainCam;
	public string battleID;

	void Start() {
		GameController.instance.battleController = this;
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		GameController.instance.loadingScreen.gameObject.SetActive(false);
		mainCam = Camera.main;
		mainCam.gameObject.SetActive(false);
		battleCam.gameObject.SetActive(true);
	}
	
	void Update() {
		
	}

	void OnDisable() {
		mainCam.gameObject.SetActive(true);
		Screen.orientation = ScreenOrientation.Portrait;
		GameController.instance.battleController = null;
	}
}
