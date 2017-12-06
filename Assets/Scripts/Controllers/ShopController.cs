using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
using TMPro;

#pragma warning disable 0414

public class ShopController : MonoBehaviour {

	public Animator ShopPanelAnimator;

	public GameObject ShopPanel;

	public Button BuyHealthButton;
	public Button BuyReviveButton;

	public Button ReturnButton;

	public TextMeshProUGUI SuccessText;

	private bool _shopActive = false;
	private bool _currencyRespond;
	private bool _currencyValidated;

	private string _purchase;
	private int _purchaseCost;

	private int _totalCurrency;
	private int pid;

	private void Awake() {
		SetShopButtons();
	}

	private void Start() {
		SetSocketReceivers();
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
		GameController.instance.Socket.On("getCurrencyValidation", OnCurrencyValidation);
	}

	private void SetShopButtons() {
		BuyHealthButton.onClick.AddListener (delegate { BuyHealth (); });
		BuyReviveButton.onClick.AddListener (delegate { BuyRevive (); });
		ReturnButton.onClick.AddListener(delegate { TogglePanel(); });
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
		GameController.instance.Socket.Emit("checkForCurrency", json);
		StartCoroutine(AwaitCurrencyCheck());
	}

	private void OnCurrencyValidation(SocketIOEvent obj) {
		if(obj.data["validation"].str == 0.ToString()) {
			_currencyValidated = false;
			SuccessText.text = "Purchase Failed";
			Invoke("RemoveSuccessText", 2f);
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
			if(timer <= 0) {
				yield break;
			}
			yield return new WaitForSeconds(1f);
		}

		if(_currencyValidated) {
			ActivatePurchase();
		}
		
		EnableButtons();
	}

	private void ActivatePurchase() {
		SuccessText.text = "Purchase Successful";
		Invoke("RemoveSuccessText", 2f);
		if (_purchase == "health") {
			AddHealthToChampion ();
		} else {
			ReviveChampion ();
		}

		GameController.instance.InterfaceController.RequestCoinCount();
	}

	private void RemoveSuccessText() {
		SuccessText.text = "";
	}

	private void ReviveChampion() {
		var json = CreateJSON ();
		json.AddField("pid", pid);
		GameController.instance.Socket.Emit ("reviveChampion", json);
	}

	private void AddHealthToChampion() {
		var json = CreateJSON ();
		json.AddField("pid", pid);
		json.AddField ("healthAmount", 500);
		GameController.instance.Socket.Emit ("addHealth", json);
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