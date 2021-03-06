﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

	public UserInterfaceController UserInterfaceController;

	public void ToggleBattleScene(string currentScene, string loadScene, string loadText) {
		if(loadText != null) {
			GameController.instance.InterfaceController.ToggleLoadingScreen(loadText);
		}
		SceneManager.UnloadSceneAsync(currentScene);
		GameController.instance.playerController.ShowMesh(false);
		AsyncOperation load = SceneManager.LoadSceneAsync(loadScene, LoadSceneMode.Additive);
		StartCoroutine(AwaitLoadedScene(load, loadScene));
	}

	private IEnumerator AwaitLoadedScene(AsyncOperation load, string loadScene) {
		GameController.instance.networkServer.UnloadOthers();
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
			GameController.instance.InterfaceController.BattleCanvas.SetActive(false);
			GameController.instance.InterfaceController.MapCanvas.SetActive(true);
			GameController.instance.playerController.ShowMesh(true);
			GameController.instance.PlayerBusy = false;

			if(GameController.instance.InterfaceController.LevelledUp) {
				GameController.instance.InterfaceController.LevelledUp = false;
				GameController.instance.InterfaceController.ToggleLevelUp();
			}
		}

		GameController.instance.InterfaceController.ToggleLoadingScreen(null);
		yield break;
	}
}