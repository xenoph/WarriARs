﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceController : MonoBehaviour {

	public GameObject MapCanvas;
	public GameObject BattleCanvas;

	public LoadingScreen LoadingScreen;
	public Button[] AbilityButtons;

	private Dictionary<string, string> _abilityInfo;

	private void Awake() {
		GameController.instance.InterfaceController = this;
	}

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

	public void GetAbilityInformation(Dictionary<string, string> abInfo) {
		
	}

	public void SetAbilityButtonNames(List<string> abNames) {
		for(int i = 0; i < AbilityButtons.Length; i++) {
			AbilityButtons[i].GetComponentInChildren<Text>().text = abNames[i];
		}
	}

	public void SetAbilityButtonDelegates(List<string> abIds) {
		
	}

	public void UpdateHealth(int health) {

	}
}