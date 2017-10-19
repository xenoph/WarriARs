using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceController : MonoBehaviour {

	public GameObject MapCanvas;
	public GameObject BattleCanvas;

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

	public LoadingScreen LoadingScreen;
	public Button[] AbilityButtons;

	[Header("Battle Interface Elements")]
	public Text MyHealth;
	public Text MyName;

	public Text OpponentHealth;
	public Text OpponentName;

	private Dictionary<string, string> _abilityInfo;

	private void Awake() {
		GameController.instance.InterfaceController = this;
	}

	public void ShowBattleRequestPanel(string id) {
		SendRequestInfoText.text = "Request battle with " + id + "?";
		SendRequestAnimator.SetTrigger("Show");

		SendRequestButton.onClick.RemoveAllListeners();
		SendRequestButton.onClick.AddListener(delegate { SendBattleRequest(id); });

		CancelRequestButton.onClick.RemoveAllListeners();
		CancelRequestButton.onClick.AddListener(delegate { CancelBattleRequest(); });
	}

	public void HideBattleRequestPanel() {
		SendRequestAnimator.SetTrigger("Hide");
	}

	public void ShowReceivedRequestPanel(string name) {
		ReceiveReqestInfoText.text = name + " has requested a battle!";
		ReceiveRequestAnimator.SetTrigger("Show");

		AcceptRequestButton.onClick.RemoveAllListeners();

		DeclineRequestButton.onClick.RemoveAllListeners();
	}

	public void HideReceivedRequestPanel() {

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
		MyHealth.text = myHealth.ToString();
		MyName.text = myName;

		OpponentHealth.text = oppHealth.ToString();
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
			AbilityButtons[i].onClick.AddListener(delegate { GameController.instance.battleController.SendAbility(abIds[i - 1]); });
		}
	}

	private void SendBattleRequest(string id) {
		SendRequestInfoText.text = "Requesting battle....";
		GameController.instance.BRController.AwaitRequestFeedback = true;
		StartCoroutine(GameController.instance.BRController.BattleRequestTimer());
		GameController.instance.BRController.SendBattleRequest(id);
	}

	private void CancelBattleRequest() {
		StopCoroutine(GameController.instance.BRController.BattleRequestTimer());
		GameController.instance.BRController.StopRequest();
		GameController.instance.PlayerBusy = false;
	}
}