﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

#pragma warning disable 0414

public class ShopController : MonoBehaviour {

	public Animator ShopPanelAnimator;

	public GameObject ShopPanel;

	public Button BuyHealthButton;
	public Button BuyReviveButton;

	private bool _shopActive = false;
	private bool _currencyRespond;
	private bool _currencyValidated;

	private string _purchase;
	private int _purchaseCost;

	private int _totalCurrency;
	private int pid;

	private SocketIOComponent _socket;

	private void Awake() {
		SetShopButtons();
	}

	public void TogglePanel() {
		if(_shopActive) {
			ShopPanelAnimator.SetBool("Show", false);
			_shopActive = false;
		} else {
			ShopPanelAnimator.SetBool("Show", true);
			_shopActive = true;
		}
	}

	private void SetSocketReceivers() {
		_socket.On("getCurrencyValidation", OnCurrencyValidation);
	}

	private void SetShopButtons() {
		BuyHealthButton.onClick.AddListener (delegate { BuyHealth (); });
		BuyReviveButton.onClick.AddListener (delegate { BuyRevive (); });
	}

	private void BuyHealth() {
		CheckForCurrency("health");
	}

	private void BuyRevive() {
		CheckForCurrency("revive");
	}

	private void CheckForCurrency(string obj) {
		DisableButtons();
		_purchase = obj;
		var json = CreateJSON ();
		json.AddField("type", obj);
		_socket.Emit("checkForCurrency", json);
		StartCoroutine(AwaitCurrencyCheck());
	}

	private void OnCurrencyValidation(SocketIOEvent obj) {
		if(obj.data["validation"].str == 0.ToString()) {
			_currencyValidated = false;
		} else {
			_currencyValidated = true;
			_purchaseCost = int.Parse(obj.data["cost"].str);
			_totalCurrency = int.Parse(obj.data["currentCurrency"].str);
			pid = int.Parse(obj.data["pid"].str);
		}

		_currencyRespond = true;
	}

	private IEnumerator AwaitCurrencyCheck() {
		var timer = 20;
		while(!_currencyRespond) {
			timer--;
			if(timer >= 0) {
				yield break;
			}
			yield return new WaitForSeconds(0.5f);
		}

		if(_currencyValidated) {
			ActivatePurchase();
		}
		
		EnableButtons();
	}

	private void ActivatePurchase() {
		if (_purchase == "health") {
			AddHealthToChampion ();
		} else {
			ReviveChampion ();
		}
	}

	private void ReviveChampion() {
		var json = CreateJSON ();
		json.AddField("pid", pid);
		_socket.Emit ("reviveChampion", json);
	}

	private void AddHealthToChampion() {
		var json = CreateJSON ();
		json.AddField("pid", pid);
		json.AddField ("healthAmount", 500);
		_socket.Emit ("addHealth", json);
	}

	private void DisableButtons() {
		BuyHealthButton.interactable = false;
		BuyReviveButton.interactable = false;
	}

	private void EnableButtons() {
		BuyHealthButton.interactable = true;
		BuyReviveButton.interactable = true;
	}

	private JSONObject CreateJSON() {
		var json = new JSONObject ();
		json.AddField ("playerID", GameController.instance.playerController.PlayerID);
		json.AddField ("socketID", GameController.instance.playerController.SocketID);
		return json;
	}
}