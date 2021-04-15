using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CameraMounter : MonoBehaviour
{
    public Camera MainCamera;
    public GameObject FP_Hands;
    [Space]
    public GameObject[] BodyParts;
    [Space]
    public Vector3 HandOffset;
    public bool Focused;

    public GameObject LookAtIKObject;

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
            MainCamera.transform.localPosition = new Vector3(0, transform.parent.GetComponent<CustomPlayerMovement>().HeightBuffer - 0.1f, 0);
            if (transform.parent.GetComponent<Mirror.NetworkIdentity>().isLocalPlayer) LookAtIKObject.transform.position = MainCamera.transform.position + MainCamera.transform.forward;
        }
    }

    [ButtonGroup]
    public void Focus()
    {
        Focused = true;
        //Hands
        FP_Hands.SetActive(true);
        FP_Hands.transform.parent = MainCamera.transform;
        FP_Hands.transform.localPosition = HandOffset;
        FP_Hands.transform.localRotation = Quaternion.identity;
        //Body
        setActiveBodyParts(false);
        //Camera
        MainCamera.transform.parent = transform;
        CameraRotation camRot = transform.parent.GetComponent<CameraRotation>();
        camRot.playerBody = transform.parent.gameObject;
        camRot.cameraObj = MainCamera.gameObject;
        MainCamera.transform.localPosition = Vector3.zero;
    }
    [ButtonGroup]
    public void Unfocus()
    {
        Focused = false;
        if(MainCamera.transform.parent == transform) MainCamera.transform.parent = null;
        transform.parent.GetComponent<CameraRotation>().cameraObj = null;
        //Hands
        FP_Hands.transform.parent = transform.parent;
        FP_Hands.transform.position = Vector3.zero;
        FP_Hands.SetActive(false);
        //Body
        setActiveBodyParts(true);
    }
    void setActiveBodyParts(bool state)
    {
        foreach (GameObject part in BodyParts)
        {
            part.SetActive(state);
        }
    }
}
