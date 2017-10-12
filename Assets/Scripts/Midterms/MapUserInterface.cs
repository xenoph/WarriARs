using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUserInterface : MonoBehaviour {

	public Button OpenStatsButton;
	public GameObject StatsCanvas;

	private bool _statsOpen;
	private PlayerCtrl _player;

	private void Awake() {
		OpenStatsButton.onClick.AddListener(delegate { ToggleStats(); });
		_player = GameObject.Find("Player").GetComponent<PlayerCtrl>();
		if(!_player.HasDragon) {
			OpenStatsButton.gameObject.SetActive(false);
		}
	}	

	public void ShowStatsButton() {
		OpenStatsButton.gameObject.SetActive(true);
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