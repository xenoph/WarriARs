using UnityEngine;

public class LookAtCamera : MonoBehaviour {
 
 	void Update() {
    	Vector3 v = Camera.main.transform.position - transform.position;
    	v.x = v.z = 0.0f;
    	transform.LookAt( Camera.main.transform.position - v ); 
    	transform.Rotate(0,180,0);
	}
}
