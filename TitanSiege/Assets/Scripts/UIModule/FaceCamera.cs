using UnityEngine;

public class FaceCamera : MonoBehaviour {
   
    void Awake() {
        enabled = false;
    } 
    void OnBecameVisible() {
        enabled = true;
        transform.forward = Camera.main.transform.forward;
    }
    void OnBecameInvisible() {
        enabled = false;
    }
}
