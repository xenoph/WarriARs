using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

	public UserInterfaceController UserInterfaceController;

	public void ToggleBattleScene(string currentScene, string loadScene, string loadText) {
		GameController.instance.InterfaceController.ToggleLoadingScreen(loadText);
		SceneManager.UnloadSceneAsync(currentScene);
		GameController.instance.playerController.gameObject.SetActive(false);
		AsyncOperation load = SceneManager.LoadSceneAsync(loadScene, LoadSceneMode.Additive);
		StartCoroutine(AwaitLoadedScene(load, loadScene));
	}

	private IEnumerator AwaitLoadedScene(AsyncOperation load, string loadScene) {
		while(!load.isDone) {
			yield return null;
		}
		if(loadScene == "battle") {
			while(GameController.instance.battleController == null &&
					!GameController.instance.InterfaceController.BattleCanvas.activeSelf) {
				yield return null;
			}
			GameController.instance.battleController.SetUpBattle();
		} else {
			while(GameObject.Find("World").transform.childCount < 1) {
				yield return null;
			}
			GameController.instance.playerController.gameObject.SetActive(true);
		}

		GameController.instance.InterfaceController.ToggleLoadingScreen(null);
		yield break;
	}
}