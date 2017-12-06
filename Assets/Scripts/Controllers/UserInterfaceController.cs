using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UserInterfaceController : MonoBehaviour {

	public GameObject MapCanvas;
	public GameObject BattleCanvas;
	public GameObject LevelUpCanvas;
	public LoadingScreen LoadingScreen;
	public Sprite[] TypeSprites;

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
	public TextMeshProUGUI MyHealthText;
	public Image MyHealthBar;
	public TextMeshProUGUI OpponentHealthText;
	public Image OpponentHealthBar;
	public Image MyTypeImage;
	public Image OpponentTypeImage;
	[HideInInspector]
	public int MyChampionType;
	[HideInInspector]
	public int OpponentChampionType;

	[Header("Nameplates")]
	public TextMeshProUGUI MyName;
	public TextMeshProUGUI OpponentName;
	public Text MyDragonTypeText;
	public Text OpponentDragonTypeText;

	[Header("Animations")]
	public Animator AbilityBarAnimator;

	[Header("Floating Battle Text")]
	public Animator MyBattleTextAnimator;
	public TextMeshProUGUI MyBattleText;
	public Animator OpponentBattleTextAnimator;
	public TextMeshProUGUI OpponentBattleText;

	public TextMeshProUGUI WinningText;
	public TextMeshProUGUI GainText;

	[Header("GameObjects")]
	public CombatNeedleBar NeedleBar;

	[Header("Ability Buttons")]
	public Button[] AbilityButtons;

	[Header("Main Interface")]
	[Header("Buttons")]
	public Button ShopButton;
	public Button ChampionButton;

	public GameObject ChampionStatsPanel;
	public GameObject MainInterface;

	[Header("Shop")]
	public GameObject ShopPanel;

	public Button BuyHealthPotionButton;
	public Button BuyReviveButton;
	public Button CloseShopButton;

	[Header("Dragon Picker")]
	public Animator DragonPickerAnimator;
	public GameObject DragonPickerPanel;

	public Button DragonPickerFireButton;
	public Button DragonPickerWaterButton;
	public Button DragonPickerWoodButton;
	public Button DragonPickerMetalButton;
	public Button DragonPickerEarthButton;

	[Header("LevelUP")]
	public TextMeshProUGUI LevelUpHeaderText;
	public Text LevelUpHealthText;
	public Text LevelUpStrengthText;
	public Text LevelUpSpeedText;

	[HideInInspector]
	public List<string> AbilityIDs;

	[HideInInspector]
	public bool LevelledUp;

	private bool _showingRequest;
	private bool _showingReceived;

	private void Awake() {
		GameController.instance.InterfaceController = this;
		MainInterface.SetActive(false);
		SetButtonDelegates();
	}

	public void ShowBattleRequestPanel(string socketID, string playerID, string username) {
		if(_showingRequest) { return; }
		_showingRequest = true;
		SendRequestInfoText.text = "Request battle with " + username + "?";
		SendRequestAnimator.SetTrigger("Show");

		SendRequestButton.onClick.RemoveAllListeners();
		SendRequestButton.onClick.AddListener(delegate { SendBattleRequest(socketID, playerID); });

		CancelRequestButton.onClick.RemoveAllListeners();
		CancelRequestButton.onClick.AddListener(delegate { CancelBattleRequest(); });
	}

	public void HideBattleRequestPanel() {
		GameController.instance.BRController.AwaitRequestFeedback = false;
		SendRequestAnimator.SetTrigger("Hide");
		_showingRequest = false;
	}

	public void ShowReceivedRequestPanel(string name) {
		if(_showingReceived) { return; }
		_showingReceived = true;
		ReceiveReqestInfoText.text = name + " has requested a battle!";
		ReceiveRequestAnimator.gameObject.GetComponent<AudioSource>().Play();
		ReceiveRequestAnimator.SetTrigger("Show");
		GameController.instance.PlayerBusy = true;

		AcceptRequestButton.onClick.RemoveAllListeners();
		AcceptRequestButton.onClick.AddListener(delegate { AcceptBattleRequest(); });

		DeclineRequestButton.onClick.RemoveAllListeners();
		DeclineRequestButton.onClick.AddListener(delegate { RefuseBattleRequest(); });
	}

	public void HideReceivedRequestPanel() {
		ReceiveRequestAnimator.SetTrigger("Hide");
		_showingReceived = false;
	}

	public void ToggleLoadingScreen(string loadText) {
		if(!LoadingScreen.gameObject.activeSelf) {
			if(loadText == null) {
				return;
			}
			LoadingScreen.LoadingText.text = loadText;
			LoadingScreen.gameObject.SetActive(true);
		} else {
			LoadingScreen.gameObject.SetActive(false);
		}
	}

	public void ToggleMainInterface() {
		if(MainInterface.activeSelf) {
			MainInterface.SetActive(false);
		} else {
			MainInterface.SetActive(true);
		}
	}

	public void ToggleLevelUp() {
		if(LevelUpCanvas.activeSelf) {
			LevelUpCanvas.SetActive(false);
		} else {
			LevelUpCanvas.SetActive(true);
			var luController = LevelUpCanvas.GetComponent<LevelUpController>();
			luController.IsWaitingForClose = true;
			luController.LevelChangeAnimator.SetTrigger("LevelUp");
			StartCoroutine(luController.SetNewLevel());
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

	public void SetTypeSprites() {
		SetTypeImageAndName(MyChampionType, MyTypeImage, MyDragonTypeText);
		SetTypeImageAndName(OpponentChampionType, OpponentTypeImage, OpponentDragonTypeText);
	}

	public void SetAbilityButtonNames(List<string> abNames) {
		for(int i = 0; i < AbilityButtons.Length; i++) {
			if(abNames[i] == "N/A")
				AbilityButtons[i].interactable = false;
			AbilityButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = abNames[i];
		}
	}

	public void SetAbilityButtonDelegates(List<string> abIds) {
		for(int i = 0; i < abIds.Count; i++) {
			AbilityIDs.Add(abIds[i]);
			AbilityButtons[i].onClick.RemoveAllListeners();
		}

		AbilityButtons[0].onClick.AddListener(delegate { GameController.instance.battleController.UseAbility1(); });
		AbilityButtons[1].onClick.AddListener(delegate { GameController.instance.battleController.UseAbility2(); });
		AbilityButtons[2].onClick.AddListener(delegate { GameController.instance.battleController.UseAbility3(); });
	}

	public void SetHealthBarsText(int myHealth, int oppHealth, int myMaxHealth, int oppMaxHealth) {
		MyHealthText.text = myHealth.ToString() + " / " + myMaxHealth.ToString();
		MyHealthBar.fillAmount = (float)myHealth / (float)myMaxHealth;

		OpponentHealthText.text = oppHealth.ToString() + " / " + oppMaxHealth.ToString();
		OpponentHealthBar.fillAmount = (float)oppHealth / (float)oppMaxHealth;
	}

	public void SetMyHealthBars(int myHealth, int myMaxHealth, int dmgToMe) {
		MyBattleText.text = dmgToMe.ToString ();
		MyBattleTextAnimator.SetTrigger ("Show");
		if(myHealth < 0) { myHealth = 0; }
		MyHealthText.text = myHealth.ToString() + " / " + myMaxHealth.ToString();
		MyHealthBar.fillAmount = (float)myHealth / (float)myMaxHealth;
	}

	public void SetOppHealthBars(int oppHealth, int oppMaxHealth, int dmgToOpp) {
		OpponentBattleText.text = dmgToOpp.ToString ();
		OpponentBattleTextAnimator.SetTrigger ("Show");
		if(oppHealth < 0) { oppHealth = 0; }
		OpponentHealthText.text = oppHealth.ToString() + " / " + oppMaxHealth.ToString();
		OpponentHealthBar.fillAmount = (float)oppHealth / (float)oppMaxHealth;
	}

	public void ShowDragonPickerPanel() {
		SetDragonPickerButtonDelegates();
		DragonPickerAnimator.SetBool("Show", true);
	}

	public void ToggleAbilityButtons() {
		foreach(var butt in AbilityButtons) {
			if(butt.GetComponentInChildren<TextMeshProUGUI>().text.ToLower() == "n/a") {
				butt.interactable = false;
			} else {
				butt.interactable = !butt.interactable;
			}
		}
	}

	private void AcceptBattleRequest() {
		HideReceivedRequestPanel();
		GameController.instance.BRController.AcceptRequest();
	}

	private void RefuseBattleRequest() {
		HideReceivedRequestPanel();
		GameController.instance.PlayerBusy = false;
	}

	private void SendBattleRequest(string socketID, string playerID) {
		if(GameController.instance.BRController.AwaitRequestFeedback) { return; }
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

	private void SetButtonDelegates() {
		ShopButton.onClick.AddListener(delegate { ShowShop(); });
		CloseShopButton.onClick.AddListener(delegate { HideShop(); });
	}

	private void ShowShop() {
		ShopPanel.transform.GetChild(0).GetComponent<Animator>().SetBool("Show", true);
		ShopButton.onClick.RemoveAllListeners();
		ShopButton.onClick.AddListener(delegate { HideShop(); });
	}

	private void HideShop() {
		ShopPanel.transform.GetChild(0).GetComponent<Animator>().SetBool("Show", false);
		ShopButton.onClick.RemoveAllListeners();
		ShopButton.onClick.AddListener(delegate { ShowShop(); });
	}

	private void SetTypeImageAndName(int typeNumber, Image typeImage, Text typeName) {
		switch (typeNumber)
		{
			case 0:
				typeImage.sprite = TypeSprites.Where(n => n.name == "UI_Element_Fire").FirstOrDefault();
				typeName.text = "FIRE DRAGON";
				break;
			case 1:
				typeImage.sprite = TypeSprites.Where(n => n.name == "UI_Element_Water").FirstOrDefault();
				typeName.text = "WATER DRAGON";
				break;
			case 2:
				typeImage.sprite = TypeSprites.Where(n => n.name == "UI_Element_Wood").FirstOrDefault();
				typeName.text = "WOOD DRAGON";
				break;
			case 3:
				typeImage.sprite = TypeSprites.Where(n => n.name == "UI_Element_Earth").FirstOrDefault();
				typeName.text = "EARTH DRAGON";
				break;
			case 4:
				typeImage.sprite = TypeSprites.Where(n => n.name == "UI_Element_Metal").FirstOrDefault();
				typeName.text = "METAL DRAGON";
				break;

			default:
				break;
		}
	}

	private void SetDragonPickerButtonDelegates() {
		DragonPickerFireButton.onClick.AddListener(delegate { PickDragon("fire"); });
		DragonPickerWaterButton.onClick.AddListener(delegate { PickDragon("water"); });
		DragonPickerWoodButton.onClick.AddListener(delegate { PickDragon("wood"); });
		DragonPickerMetalButton.onClick.AddListener(delegate { PickDragon("metal"); });
		DragonPickerEarthButton.onClick.AddListener(delegate { PickDragon("earth"); });
	}

	private void PickDragon(string dType) {
		var json = new JSONObject();
		json.AddField ("playerID", GameController.instance.currentPlayerID);
		json.AddField("dragonType", dType);
		GameController.instance.Socket.Emit("pickedDragon", json);
		DragonPickerAnimator.SetBool("Show", false);
	}
}
