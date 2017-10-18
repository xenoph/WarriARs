using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using UnityEngine;

public class InputController : MonoBehaviour {

	private void Update() {
		if(GameController.instance.battleController != null) { return; }
#if UNITY_EDITOR
		GetMouseClick();
#elif UNITY_IOS || UNITY_ANDROID
		DetectTouchInput();
#endif
	}

	private void GetMouseClick() {
		if(Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			var mousePos = Input.mousePosition;
			Ray ray = Camera.main.ScreenPointToRay(mousePos);
			if(Physics.Raycast(ray, out hit, 200)) {
				if(hit.transform.tag == "AICompanion") {
					if(!GameController.instance.playerController.ChampionAlive) { return; }
					GameController.instance.BRController.InitialiseAIBattle(hit.transform.GetComponent<AIController>().AId);
				} else if(hit.transform.tag == "Player") {
					Debug.Log("player");
				} else if(hit.transform.parent.transform.tag == "Map") {
					var abMap = hit.transform.parent.gameObject.GetComponent<AbstractMap>();
					var loc = hit.point.GetGeoPosition(abMap.CenterMercator, abMap.WorldRelativeScale);
					//GManager.CurrentPlayer.SetNewLocation((float)loc.x, (float)loc.y, GManager);
				}
			}
		}
	}

	private void DetectTouchInput() {
		if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
			Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)) {
				if(hit.transform.tag == "Player" && hit.transform.gameObject.GetComponent<PlayerController>() != GameController.instance.playerController) {
					if(!GameController.instance.playerController.ChampionAlive) { return; }
					//Get id from opponent
					GameController.instance.BRController.SendBattleRequest(0);
				} else if(hit.transform.tag == "AICompanion") {
					//Get ai data
					GameController.instance.BRController.InitialiseAIBattle(hit.transform.GetComponent<AIController>().AId);
				}
			}
		}
	}
}
