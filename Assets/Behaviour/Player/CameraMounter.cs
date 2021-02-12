using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMounter : Mirror.NetworkBehaviour
{
    Camera MainCamera;

    private void Awake()
    {
        MainCamera = Camera.main;
    }
    // Start is called before the first frame update
    public void Start()
    {
        if (!isLocalPlayer) return;
        MainCamera.transform.parent = transform;
        transform.parent.GetComponent<CameraRotation>().playerBody = transform.parent.gameObject;
        transform.parent.GetComponent<CameraRotation>().cameraObj = MainCamera.gameObject;
        MainCamera.transform.localPosition = Vector3.zero;
    }
}
