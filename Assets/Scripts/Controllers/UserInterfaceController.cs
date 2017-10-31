using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceController : MonoBehaviour {

	public GameObject MapCanvas;
	public GameObject BattleCanvas;
	public LoadingScreen LoadingScreen;

	[Header("Send Request Panel")]
	public Animator SendRequestAnimator;
	public Text SendRequestInfoText;
	public Button SendRequestButton;
	public Button CancelRequestButton;

	[Header("Receive Request Panel")]
	public Animator ReceiveRequestAnimator;
	public Text ReceiveReqestInfoText;
	public Button AcceptRequestButton;
	public Button DeclineRequestButton;

	[Header("Battle Interface Elements")]
	[Header("Health Bars")]
	public Text MyHealthText;
	public Slider MyHealthBar;
	public Text OpponentHealthText;
	public Slider OpponentHealthBar;

	[Header("Nameplates")]
	public Text MyName;
	public Text OpponentName;

	[Header("Animations")]
	public Animator AbilityBarAnimator;

	[Header("GameObjects")]
	public CombatNeedleBar NeedleBar;

	[Header("Ability Buttons")]
	public Button[] AbilityButtons;

	[Header("Main Interface")]
	[Header("Buttons")]
	public Button ShopButton;
	public Button ChampionButton;

	public GameObject ShopPanel;
	public GameObject ChampionStatsPanel;

	private void Awake() {
		GameController.instance.InterfaceController = this;
	}

	public void ShowBattleRequestPanel(string socketID, string playerID) {
		SendRequestInfoText.text = "Request battle with " + playerID + "?";
		SendRequestAnimator.SetTrigger("Show");

		SendRequestButton.onClick.RemoveAllListeners();
		SendRequestButton.onClick.AddListener(delegate { SendBattleRequest(socketID, playerID); });

		CancelRequestButton.onClick.RemoveAllListeners();
		CancelRequestButton.onClick.AddListener(delegate { CancelBattleRequest(); });
	}

	public void HideBattleRequestPanel() {
		SendRequestAnimator.SetTrigger("Hide");
	}

	public void ShowReceivedRequestPanel(string name) {
		ReceiveReqestInfoText.text = name + " has requested a battle!";
		ReceiveRequestAnimator.SetTrigger("Show");
		GameController.instance.PlayerBusy = true;

		AcceptRequestButton.onClick.RemoveAllListeners();
		AcceptRequestButton.onClick.AddListener(delegate { AcceptBattleRequest(); });

		DeclineRequestButton.onClick.RemoveAllListeners();
		DeclineRequestButton.onClick.AddListener(delegate { RefuseBattleRequest(); });
	}

	public void HideReceivedRequestPanel() {
		ReceiveRequestAnimator.SetTrigger("Hide");
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
		MyHealthText.text = myHealth.ToString();
		MyName.text = myName;

		OpponentHealthText.text = oppHealth.ToString();
		OpponentName.text = oppName;
		
		MapCanvas.SetActive(false);
		BattleCanvas.SetActive(true);
	}

	public void SetAbilityButtonNames(List<string> abNames) {
		for(int i = 0; i < AbilityButtons.Length; i++) {
			AbilityButtons[i].GetComponentInChildren<Text>().text = abNames[i];
		}
	}

	public void SetAbilityButtonDelegates(List<string> abIds) {
		for(int i = 0; i < abIds.Count; i++) {
			AbilityButtons[i].onClick.RemoveAllListeners();
			AbilityButtons[i].onClick.AddListener(delegate { GameController.instance.battleController.UseAbility(abIds[i - 1], i); });
		}
	}

	public void ToggleAbilityButtons() {
		if(AbilityButtons[0].interactable) {
			foreach(var butt in AbilityButtons) {
				butt.interactable = false;
			}
		} else {
			foreach(var butt in AbilityButtons) {
				butt.interactable = true;
			}
		}
	}

	private void AcceptBattleRequest() {
		GameController.instance.BRController.AcceptRequest();
	}

	private void RefuseBattleRequest() {
		HideReceivedRequestPanel();
		GameController.instance.PlayerBusy = false;
	}

	private void SendBattleRequest(string socketID, string playerID) {
		SendRequestInfoText.text = "Requesting battle....";
		GameController.instance.BRController.AwaitRequestFeedback = true;
		StartCoroutine(GameController.instance.BRController.BattleRequestTimer());
		GameController.instance.BRController.SendBattleRequest(socketID, playerID);
	}

	private void CancelBattleRequest() {
		StopCoroutine(GameController.instance.BRController.BattleRequestTimer());
		GameController.instance.BRController.StopRequest();
		GameController.instance.PlayerBusy = false;
	}

	private void ToggleShopWindow() {
	}
}