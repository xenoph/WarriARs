using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public bool local = false;
    public bool ChampionAlive = true;

    public Vector3 targetPosition;// { get; private set; }
    public Vector3 lastWorldpos;

    public int diff = 1;
    public string opponentUsername;

    [HideInInspector]
    public string PlayerID;
    [HideInInspector]
    public string SocketID;

    public string username;

    private NetworkServer server;
    private Location lastLocation;

	void Start() {
        if(local) {
            GameController.instance.playerController = this;
            GameController.instance.cameraController.target = this.transform;
            server = GameController.instance.networkServer;
        }
        //lastLocation = GameController.instance.currentLocation;
        lastLocation = new Location();
        //SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("map"));
	}
	
	void FixedUpdate() {
        if(local) {
            if(GameController.instance.mapInitializer != null) {
                targetPosition = ConvertPositions.ConvertLocationToVector3(GameController.instance.currentLocation, GameController.instance.mapInitializer.map);
                lastWorldpos = ConvertPositions.ConvertLocationToVector3(lastLocation, GameController.instance.mapInitializer.map);
                lastWorldpos = new Vector3(lastWorldpos.x, 0f, lastWorldpos.z);

                float dist = Vector3.Distance(targetPosition, lastWorldpos);
                if(dist > 1f) {
                    server.Move((float) GameController.instance.currentLocation.Latitude, (float) GameController.instance.currentLocation.Longitude, 0);
                    lastLocation.SetLocation(GameController.instance.currentLocation.Latitude, GameController.instance.currentLocation.Longitude);
                }
            }
        }
        targetPosition = new Vector3(targetPosition.x, 0f, targetPosition.z);
        float distance = Vector3.Distance(transform.position, targetPosition);
        float speed = distance > 20f ? distance : 20f;
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        transform.LookAt(targetPosition);
    }
}
