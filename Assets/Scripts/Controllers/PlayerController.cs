using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour {

    public bool local = false;
    public bool ChampionAlive = true;

    public Vector3 targetPosition;// { get; private set; }
    public Vector3 lastWorldpos;

    public int diff = 1;
    public string opponentUsername;

    //[HideInInspector]
    public string PlayerID;
    //[HideInInspector]
    public string SocketID;

    public string username;

    public TextMeshProUGUI overheadUsername;

    private NetworkServer server;
    private Location lastLocation;

	void Start() {
        if(local) {
            GameController.instance.playerController = this;
            GameController.instance.cameraController.target = this.transform;
            server = GameController.instance.networkServer;
            StartCoroutine(UpdatePositionPeriodically());
        }
        //lastLocation = GameController.instance.currentLocation;
        lastLocation = new Location();
        //SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("map"));
	}

    private IEnumerator UpdatePositionPeriodically() {
        while(local && server != null) {
            yield return new WaitForSeconds(0.5f);
            UpdateServerPosition();
        }
    }

    private void UpdateServerPosition() {
        server.Move((float) GameController.instance.currentLocation.Latitude, (float) GameController.instance.currentLocation.Longitude, 0);
        lastLocation.SetLocation(GameController.instance.currentLocation.Latitude, GameController.instance.currentLocation.Longitude);
    }
	
	void FixedUpdate() {
        if(local) {
            if(GameController.instance.mapInitializer != null) {
                targetPosition = ConvertPositions.ConvertLocationToVector3(GameController.instance.currentLocation, GameController.instance.mapInitializer.map);
                lastWorldpos = ConvertPositions.ConvertLocationToVector3(lastLocation, GameController.instance.mapInitializer.map);
                lastWorldpos = new Vector3(lastWorldpos.x, 0f, lastWorldpos.z);

                float dist = Vector3.Distance(targetPosition, lastWorldpos);
                if(dist > 1f) {
                    UpdateServerPosition();
                }
            }
            if(overheadUsername != null) {
                overheadUsername.gameObject.SetActive(false);
                overheadUsername = null;
            }
        } else {
            if(overheadUsername != null) {
                overheadUsername.text = username;
                if(username.StartsWith("Mod ")) {
                    overheadUsername.color = new Color(213f / 255f, 25f / 255f, 203f / 255f, 1f);
                } else {
                    overheadUsername.color = new Color(1f, 1f, 1f, 1f);
                }
            }   
        }

        string sceneShouldBeIn = local ? "main" : "map";
        if(gameObject.scene.name != sceneShouldBeIn) {
            if(SceneManager.GetSceneByName(sceneShouldBeIn) != null) {
                SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName(sceneShouldBeIn));
            } else {
                Destroy(gameObject);
            }
        }

        if(string.IsNullOrEmpty(PlayerID) && string.IsNullOrEmpty(SocketID) && string.IsNullOrEmpty(username))
            Destroy(gameObject);

        targetPosition = new Vector3(targetPosition.x, 0f, targetPosition.z);
        float distance = Vector3.Distance(transform.position, targetPosition);
        float speed = distance > 20f ? distance : 20f;
        float step = speed * Time.deltaTime;
        if(distance > 30f) {
            transform.position = targetPosition;
        } else {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }
        transform.LookAt(targetPosition);
    }
}
