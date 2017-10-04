using UnityEngine;

public class PlayerController : MonoBehaviour {

    public bool local = false;

    private Vector3 targetPosition;

	void Start() {
        if(local) {
            GameController.instance.playerController = this;
            GameController.instance.cameraController.target = this.transform;
        }
	}
	
	void FixedUpdate() {
        if(local) {
            if(GameController.instance.mapInitializer != null) {
                Vector3 pos2world = ConvertPositions.ConvertLocationToVector3(GameController.instance.currentLocation, GameController.instance.mapInitializer.map);
                if(pos2world != targetPosition) {
                    targetPosition = pos2world;
                }
            }
        }
        targetPosition.y = 0f;
        float distance = Vector3.Distance(transform.position, targetPosition);
        float speed = distance > 20f ? distance : 20f;
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        transform.LookAt(targetPosition);
    }
}
