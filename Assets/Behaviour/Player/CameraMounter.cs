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
        Focus();
    }
    private void Update()
    { 
        if (transform != MainCamera.transform.parent && Focused) Unfocus();
        if (Focused)
        {
            FP_Hands.transform.localPosition = HandOffset;
        }
    }
    public void Unfocus()
    {
        Focused = false;
        FP_Hands.transform.parent = transform.parent;
        FP_Hands.transform.position = Vector3.zero;
        FP_Hands.SetActive(false);
    }
    public void Focus()
    {
        Focused = true;
        //Hands
        FP_Hands.SetActive(true);
        FP_Hands.transform.parent = MainCamera.transform;
        FP_Hands.transform.localPosition = HandOffset;
        //Camera
        MainCamera.transform.parent = transform;
        CameraRotation camRot = transform.parent.GetComponent<CameraRotation>();
        camRot.playerBody = transform.parent.gameObject;
        camRot.cameraObj = MainCamera.gameObject;
        MainCamera.transform.localPosition = Vector3.zero;
    }
}
