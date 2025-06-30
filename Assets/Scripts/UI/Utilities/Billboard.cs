using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera cam;
    void Awake() => cam = Camera.main;
    void LateUpdate() { if (cam) transform.forward = cam.transform.forward; }
}

