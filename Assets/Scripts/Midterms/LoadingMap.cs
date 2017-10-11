using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mapbox.Unity.Map;

public class LoadingMap : MonoBehaviour {

	public Slider loadingBar;

	void Start() {
		AsyncOperation ao = SceneManager.LoadSceneAsync("map", LoadSceneMode.Additive);
		StartCoroutine(loadingMap(ao));
	}
	
	IEnumerator loadingMap(AsyncOperation ao) {
		while(ao.progress < 1f) {
			loadingBar.value = ao.progress - .1f;
			yield return null;
		}
		while(!ao.isDone)
			yield return null;
		PlayerCtrl player = GameObject.FindObjectOfType<PlayerCtrl>();
		GameObject world = GameObject.FindObjectOfType<BasicMap>().gameObject;
		while(player == null || world == null)
			yield return null;
		loadingBar.value = .95f;
		while(world.transform.childCount == 0)
			yield return new WaitForSeconds(1f);
		loadingBar.value = 1f;
		loadingBar.transform.parent.gameObject.SetActive(false);
		transform.GetComponent<Camera>().enabled = false;
		transform.GetComponent<AudioListener>().enabled = false;
		player.setActiveCamAndLight(true);
		SceneManager.UnloadSceneAsync("LoadMap");
	}
}
