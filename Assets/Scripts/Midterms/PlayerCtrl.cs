using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCtrl : MonoBehaviour {
	public Camera _cam;
	public Light dirLight;
	public Vector3 targetPosition;

	public float turnSpeed = 50f;
	public float moveSpeed = 50f;

	public MidtermFightSceneController midtermFightScene;
	private int dragonID = 0;

	void Start() {
		targetPosition = transform.position;
	}

	public void register() {
		_cam.gameObject.SetActive(false);
		dirLight.gameObject.SetActive(false);
		midtermFightScene.SetUpScene(dragonID);
	}

	public void UnloadFightScene() {
		AsyncOperation ao = SceneManager.UnloadSceneAsync("fight1");
		StartCoroutine(unloadedFight(ao));
	}

	IEnumerator unloadedFight(AsyncOperation ao) {
		while(!ao.isDone)
			yield return null;
		_cam.gameObject.SetActive(true);
		dirLight.gameObject.SetActive(true);
	}

	IEnumerator clickedDragon(MapDragons md) {
		while(Vector3.Distance(transform.position, targetPosition) >= 0.1f)
			yield return null;
		
		dragonID = md.dragonID;
		SceneManager.LoadSceneAsync("fight1", LoadSceneMode.Additive);
	}
	
	void Update() {
		if(!_cam.gameObject.activeInHierarchy)
			return;
		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
      		transform.Rotate(-Vector3.up * turnSpeed * Time.deltaTime);
		} else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
			transform.Rotate(Vector3.up * turnSpeed * Time.deltaTime);
		}

		if(Input.GetMouseButtonDown(0)) {
			RaycastHit hit; 
   			Ray ray = _cam.ScreenPointToRay(Input.mousePosition); 
   			if(Physics.Raycast(ray, out hit, 1000.0f)) {
				MapDragons md = hit.transform.GetComponent<MapDragons>();
				targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
				if(md != null) {
					StartCoroutine(clickedDragon(md));
				}
			}
		}

		if(Vector3.Distance(transform.position, targetPosition) >= 0.1f) {
        	float step = moveSpeed * Time.deltaTime;
        	transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
		}
	}
}
