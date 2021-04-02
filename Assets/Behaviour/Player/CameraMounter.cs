using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMounter : MonoBehaviour
{
    public Camera MainCamera;
    public GameObject FP_Hands;
    public Vector3 HandOffset;
    public bool Focused;

    private void Awake()
    {
        MainCamera = Camera.main;
    }
    // Start is called before the first frame update
    public void Start()
    {
        if (!transform.parent.GetComponent<Mirror.NetworkIdentity>().isLocalPlayer) return;
        Focus(); //TODO: Implement unfocus or hide FP Hands when not focused
    }
    private void Update()
    {
        if (transform != MainCamera.transform.parent)
        {
            Focused = false;
            return;
        }
        FP_Hands.transform.position = MainCamera.transform.position + HandOffset; // Note: this doesnt work!
        FP_Hands.transform.rotation = MainCamera.transform.rotation;
    }
    public void Focus()
    {
        Focused = true;
        MainCamera.transform.parent = transform;
        transform.parent.GetComponent<CameraRotation>().playerBody = transform.parent.gameObject;
        transform.parent.GetComponent<CameraRotation>().cameraObj = MainCamera.gameObject;
        MainCamera.transform.localPosition = Vector3.zero;
    }
}
