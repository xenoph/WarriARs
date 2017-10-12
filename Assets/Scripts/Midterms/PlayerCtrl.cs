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

	public bool HasDragon = false;

	public List<GameObject> dragons = new List<GameObject>();

	public MidtermFightSceneController midtermFightScene;
	private int dragonID = 0;
	private bool sceneLoaded = false;

	void Start() {
		setActiveCamAndLight(false);
		targetPosition = transform.position;
		for(int i = 1; i < dragons.Count; i++) {
			dragons[i].SetActive(false);
		}
	}

	public void setActiveCamAndLight(bool enabled = false) {
		_cam.gameObject.SetActive(enabled);
		dirLight.gameObject.SetActive(enabled);
	}

	public void register() {
		setActiveCamAndLight(false);
		midtermFightScene.SetUpScene(dragonID);
	}

	public void UnloadFightScene() {
		if(dragonID <= dragons.Count) {
			//Removes the dragon you just killed
			dragons[dragonID - 1].transform.GetChild(0).gameObject.SetActive(false);
			//Adds the reward chest for the killed dragon
			dragons[dragonID - 1].transform.GetChild(1).gameObject.SetActive(true);
			//next dragon
			dragonID++;
			//enable the next dragon
			if(dragonID <= dragons.Count)
				dragons[dragonID - 1].SetActive(true);
		}
		AsyncOperation ao = SceneManager.UnloadSceneAsync("fight1");
		StartCoroutine(unloadedFight(ao));
	}

	IEnumerator unloadedFight(AsyncOperation ao) {
		while(!ao.isDone)
			yield return null;
		setActiveCamAndLight(true);
		sceneLoaded = false;
	}

	IEnumerator clickedDragon(MapDragons md) {
		if(sceneLoaded)
			yield break;
		sceneLoaded = true;
		while(Vector3.Distance(transform.position, targetPosition) >= 0.1f)
			yield return null;
		if(SceneManager.GetSceneByName("fight1").isLoaded)
			yield break;
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

			Vector3 _direction = (targetPosition - transform.position).normalized;
			Quaternion _lookRotation = Quaternion.LookRotation(_direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 5f);
		}
	}
}
