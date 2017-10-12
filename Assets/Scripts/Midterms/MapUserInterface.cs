using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapUserInterface : MonoBehaviour {

	public Button OpenStatsButton;
	public GameObject Diamonds;
	public GameObject StatsCanvas;

	public int diamondAmount = 0;
	public TextMeshProUGUI diamondsAmountText;

	private bool _statsOpen;
	private PlayerCtrl _player;

	private void Awake() {
		OpenStatsButton.onClick.AddListener(delegate { ToggleStats(); });
		_player = GameObject.Find("Player").GetComponent<PlayerCtrl>();
		if(!_player.HasDragon) {
			OpenStatsButton.gameObject.SetActive(false);
		}
	}

	private void Start() {
		diamondsAmountText = Diamonds.GetComponentInChildren<TextMeshProUGUI>();
		Diamonds.SetActive(false);
	}

	void FixedUpdate() {
		diamondsAmountText.text = diamondAmount.ToString();
	}

	public void GiveDiamond(int amount = 1) {
		diamondAmount += amount;
	}

	public int GetDiamondAmount() {
		return diamondAmount;
	}

	public void ShowStatsButton() {
		OpenStatsButton.gameObject.SetActive(true);
		Diamonds.SetActive(true);
	}

	private void ToggleStats() {
		if(_statsOpen) {
			StatsCanvas.SetActive(false);
			_statsOpen = false;
		} else {
			StatsCanvas.SetActive(true);
			_statsOpen = true;
		}
	}
}