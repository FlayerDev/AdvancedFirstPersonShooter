using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CameraMounter : MonoBehaviour, IComponentInitializable
{
    public bool isPlayer = true;
    public Camera MainCamera;
    public GameObject FP_Hands;
    [Space]
    public GameObject[] BodyParts;
    [Space]
    public Vector3 HandOffset;
    public bool Focused;

    public GameObject LookAtIKObject;

    public void Init()
    {
        Awake();
        Start();
    }

    private void Awake()
    {
        MainCamera = Camera.main;
    }
    // Start is called before the first frame update
    public void Start()
    {
        if (!transform.parent.GetComponent<Mirror.NetworkIdentity>().hasAuthority)
        {
            print("CameraMounter:QuittingInit(Is'nt local player)");
            return;
        }
        Focus();
    }
    private void Update()
    { 
        if (transform != MainCamera.transform.parent && Focused) Unfocus();
        if (Focused)
        {
            FP_Hands.transform.localPosition = HandOffset;
            MainCamera.transform.localPosition = new Vector3(0, transform.parent.GetComponent<CustomPlayerMovement>().HeightBuffer - 0.1f, 0);
            if (transform.parent.GetComponent<Mirror.NetworkIdentity>().isLocalPlayer && isPlayer) 
                LookAtIKObject.transform.position = MainCamera.transform.position + MainCamera.transform.forward;
        }
    }

    [ButtonGroup]
    public void Focus()
    {
        if (isPlayer)
        {
            print("CameraMounter:isPlayer");
            Focused = true;
            //Hands
            FP_Hands.SetActive(true);
            FP_Hands.transform.parent = MainCamera.transform;
            FP_Hands.transform.localPosition = HandOffset;
            FP_Hands.transform.localRotation = Quaternion.identity;
            //Body
            setActiveBodyParts(false);
        }
        //Camera
        MainCamera.transform.parent = transform;
        CameraRotation camRot = transform.parent.GetComponent<CameraRotation>();
        camRot.playerBody = transform.parent.gameObject;
        camRot.cameraObj = MainCamera.gameObject;
        MainCamera.transform.localPosition = Vector3.zero;
        print("CameraMounter:Focused");
    }
    [ButtonGroup]
    public void Unfocus()
    {
        Focused = false;
        if(MainCamera.transform.parent == transform) MainCamera.transform.parent = null;
        transform.parent.GetComponent<CameraRotation>().cameraObj = null;
        if (isPlayer)
        {
            //Hands
            FP_Hands.transform.parent = transform.parent;
            FP_Hands.transform.position = Vector3.zero;
            FP_Hands.SetActive(false);
            //Body
            setActiveBodyParts(true);
        }
        print("CameraMounter:Unfocused");
    }
    void setActiveBodyParts(bool state)
    {
        foreach (GameObject part in BodyParts)
        {
            part.SetActive(state);
        }
    }
}
