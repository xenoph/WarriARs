﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public bool local = false;

    public Vector3 targetPosition { get; private set; }

    private NetworkServer server;
    private Location lastLocation;

	void Start() {
        if(local) {
            GameController.instance.playerController = this;
            GameController.instance.cameraController.target = this.transform;
            server = GameController.instance.networkServer;
        }
        lastLocation = GameController.instance.currentLocation;        
        //SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("map"));
	}
	
	void FixedUpdate() {
        if(local) {
            if(GameController.instance.mapInitializer != null) {
                Vector3 pos2world = ConvertPositions.ConvertLocationToVector3(GameController.instance.currentLocation, GameController.instance.mapInitializer.map);
                if(pos2world != targetPosition) {
                    targetPosition = pos2world;
                    if(CalculateDistance.ReturnCalculatedDistance(lastLocation.Latitude, lastLocation.Longitude, GameController.instance.currentLocation.Latitude, GameController.instance.currentLocation.Longitude) >= 2)
                        server.Move((float) GameController.instance.currentLocation.Latitude, (float) GameController.instance.currentLocation.Longitude, 0);
                    
                    lastLocation = GameController.instance.currentLocation;
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
