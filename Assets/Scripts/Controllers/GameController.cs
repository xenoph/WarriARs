﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	public static GameController instance;

	[Header("Interfaces")]
	public Warning warning;
	public LoginScreen loginScreen;
	public LoadingScreen loadingScreen;
	public BattleRequestController BRController;

	[Header("Game")]
	public MapInitializer mapInitializer;
	public BattleController battleController;
	public UserInterfaceController InterfaceController;
	public SceneController SceneController;

	[Header("Networking")]
	public NetworkServer networkServer;
	public string currentPlayerID;
    public PlayerController playerController;

    [Header("Prefabs")]
    public GameObject playerPrefab;

    [Header("Camera")]
    public CameraController cameraController;

    public Location currentLocation;

	public bool PlayerBusy;

	void Awake() {
		instance = this;
		SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
		currentLocation = new Location();
		#if UNITY_EDITOR
			currentLocation.Latitude = 64.0186d;
			currentLocation.Longitude = 11.4938d;
			currentLocation.IsActive = true;
		#else
			StartCoroutine(currentLocation.StartLocationServices());
		#endif
	}

	public void LoadGame(string username) {
		SceneManager.LoadSceneAsync("map", LoadSceneMode.Additive);
		loginScreen.gameObject.SetActive(false);
        SpawnPlayer(true, username);
	}

    public PlayerController SpawnPlayer(bool local, string username) {
        GameObject player = (GameObject) Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        PlayerController controller = player.GetComponent<PlayerController>();
		controller.username = username;
        controller.local = local;
		return controller;
    }

    public void SetupMap() {
		if(!currentLocation.Failed && currentLocation.IsActive) {
			mapInitializer.startLat = currentLocation.Latitude;
			mapInitializer.startLng = currentLocation.Longitude;
		} else {
			Debug.LogError("No start location found.");
		}
	}

	void StopBattle() {
		loadingScreen.gameObject.SetActive(true);
		loadingScreen.LoadingText.text = "Loading map...";
		SceneManager.UnloadSceneAsync("battle");
		SceneManager.LoadSceneAsync("map", LoadSceneMode.Additive);
		playerController.gameObject.SetActive(true);
	}

	void LateUpdate() {
		if(warning != null) {
			warning.showing = currentLocation.Failed || !currentLocation.IsActive;
			if(warning.showing)
				warning.text.text = "Failed to update your location.\nCheck if your Location Service is enabled.";
		}
	}

	public void UnloadFightScene() {
		AsyncOperation ao = SceneManager.UnloadSceneAsync("fight1");
		StartCoroutine(unloadedFight(ao));
	}

	IEnumerator unloadedFight(AsyncOperation ao) {
		while(!ao.isDone)
			yield return null;
		//InterfaceController.
	}
}
