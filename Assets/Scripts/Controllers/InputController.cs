using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputController : MonoBehaviour {

	private void Update() {
		if(GameController.instance.battleController != null || GameController.instance.PlayerBusy) { return; }
		
		Input.simulateMouseWithTouches = true;
#if UNITY_EDITOR || UNITY_WEBGL
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
					GameController.instance.PlayerBusy = true;
					GameController.instance.BRController.InitialiseAIBattle(hit.transform.GetComponent<PlayerController>().SocketID, hit.transform.GetComponent<AIController>().AId);
				} else if(hit.transform.tag == "Player" && hit.transform.GetComponent<PlayerController>() != GameController.instance.playerController) {
					PlayerController hitPlayerController = hit.transform.GetComponent<PlayerController>();
					GameController.instance.PlayerBusy = true;
					GameController.instance.InterfaceController.ShowBattleRequestPanel(hitPlayerController.SocketID, hitPlayerController.PlayerID, hitPlayerController.username);
					
					/*if(hit.transform.GetComponent<AIController>()) {
						GameController.instance.PlayerBusy = true;
						GameController.instance.playerController.opponentUsername = hit.transform.GetComponent<PlayerController>().username;
						//SceneManager.LoadSceneAsync("fight1", LoadSceneMode.Additive);
						GameController.instance.BRController.InitialiseAIBattle(hit.transform.GetComponent<PlayerController>().SocketID, hit.transform.GetComponent<PlayerController>().PlayerID);
					} else {
						GameController.instance.PlayerBusy = true;
						GameController.instance.InterfaceController.ShowBattleRequestPanel(hitPlayerController.SocketID, hitPlayerController.PlayerID);
					}*/
				} else if(hit.transform.parent.transform.tag == "Map") {
					//var abMap = hit.transform.parent.gameObject.GetComponent<AbstractMap>();
					//var loc = hit.point.GetGeoPosition(abMap.CenterMercator, abMap.WorldRelativeScale);
					GameController.instance.playerController.targetPosition = hit.point;
					//GManager.CurrentPlayer.SetNewLocation((float)loc.x, (float)loc.y, GManager);
				}
			}
		}

		if(Input.GetAxis("Horizontal") != 0f) {
			if(GameController.instance.playerController != null)
				GameController.instance.playerController.transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * -1f);
		}
	}

	private Vector3 startPos = Vector3.zero;

	void LateUpdate() {
		//Needs testing
		if (Input.touchCount == 1 && GameController.instance.playerController != null) {
            Touch touch = Input.GetTouch(0);
			/*
            Ray r = Camera.main.ScreenPointToRay(touch.position);
			RaycastHit hit;
			if(Physics.Raycast(r, out hit, 200f)) {
				Debug.Log(hit.point);
			}
			*/
			Vector3 axis = touch.deltaPosition;
			float dir = axis.x < 0 ? 1f : -1f;
            GameController.instance.playerController.transform.Rotate(Vector3.up, axis.magnitude * dir);
        }
	}

	private void DetectTouchInput() {
		if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
			Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)) {
				if(hit.transform.tag == "Player" && hit.transform.gameObject.GetComponent<PlayerController>() != GameController.instance.playerController) {
					if(hit.transform.GetComponent<AIController>()) {
						GameController.instance.PlayerBusy = true;
						GameController.instance.playerController.opponentUsername = hit.transform.GetComponent<PlayerController>().username;
						//SceneManager.LoadSceneAsync("fight1", LoadSceneMode.Additive);
						GameController.instance.BRController.InitialiseAIBattle(hit.transform.GetComponent<PlayerController>().SocketID, hit.transform.GetComponent<PlayerController>().PlayerID);
					} else {
						//if(!GameController.instance.playerController.ChampionAlive) { return; }
						GameController.instance.PlayerBusy = true;
						GameController.instance.InterfaceController.ShowBattleRequestPanel(hit.transform.GetComponent<PlayerController>().SocketID, hit.transform.GetComponent<PlayerController>().PlayerID, hit.transform.GetComponent<PlayerController>().username);
					}
				} else if(hit.transform.tag == "AICompanion") {
					GameController.instance.PlayerBusy = true;
					GameController.instance.BRController.InitialiseAIBattle(hit.transform.GetComponent<PlayerController>().SocketID, hit.transform.GetComponent<AIController>().AId);
				}
			}
		}
	}
}
