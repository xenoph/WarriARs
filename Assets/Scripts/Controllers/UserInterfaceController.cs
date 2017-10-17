using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceController : MonoBehaviour {

	public GameObject MapCanvas;
	public GameObject BattleCanvas;

	public SceneController SceneController;

	public LoadingScreen LoadingScreen;

	public void ToggleLoadingScreen(string loadText) {
		if(LoadingScreen.gameObject.activeSelf) {
			LoadingScreen.LoadingText.text = loadText;
			LoadingScreen.gameObject.SetActive(true);
		} else {
			LoadingScreen.gameObject.SetActive(false);
		}
	}

	public void SetUpBattleCanvas(int myHealth, int oppHealth, string myName, string oppName) {

	}

	private void GetAbilityNames() {

	}

	public void UpdateHealth(int health) {

	}
}